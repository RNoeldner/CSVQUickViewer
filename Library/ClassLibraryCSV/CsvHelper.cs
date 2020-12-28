/*
 * Copyright (C) 2014 Raphael Nöldner : http://csvquickviewer.com
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser Public
 * License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser Public License for more details.
 *
 * You should have received a copy of the GNU Lesser Public License along with this program.
 * If not, see http://www.gnu.org/licenses/ .
 *
 */

using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CsvTools
{
	/// <summary>
	///   Helper class
	/// </summary>
	public static partial class CsvHelper
	{
		public static async Task<Tuple<DelimitedFileDetectionResult, IEnumerable<Column>>> AnalyseFileAsync(string fileName, bool guessJson,
																												 bool guessCodePage, bool guessDelimiter, bool guessQualifier, bool guessStartRow,
																												 bool guessHasHeader, bool guessNewLine, [NotNull] FillGuessSettings fillGuessSettings,
																												 [NotNull] IProcessDisplay processDisplay)
		{
			if (processDisplay == null) throw new ArgumentNullException(nameof(processDisplay));
			if (string.IsNullOrEmpty(fileName))
				return null;

			if (fileName.IndexOf('~') != -1)
				fileName = fileName.LongFileName();

			var fileInfo = new FileSystemUtils.FileInfo(fileName);

			if (!fileInfo.Exists)
				return null;
			Logger.Information("Examining file {filename}", FileSystemUtils.GetShortDisplayFileName(fileName, 40));
			Logger.Information($"Size of file: {StringConversion.DynamicStorageSize(fileInfo.Length)}");

			// load from Setting file
			if (fileName.EndsWith(CsvFile.cCsvSettingExtension, StringComparison.OrdinalIgnoreCase) ||
					FileSystemUtils.FileExists(fileName + CsvFile.cCsvSettingExtension))
			{
				if (!fileName.EndsWith(CsvFile.cCsvSettingExtension, StringComparison.OrdinalIgnoreCase))
					fileName += CsvFile.cCsvSettingExtension;
				// we defiantly have a the extension with the name
				var fileSettingSer = SerializedFilesLib.LoadCsvFile(fileName);
				fileSettingSer.FileName = fileName.Substring(0, fileName.Length - CsvFile.cCsvSettingExtension.Length);
				Logger.Information("Configuration read from setting file {filename}",
					FileSystemUtils.GetShortDisplayFileName(fileName, 40));

				// un-ignore all ignored columns
				foreach (var col in fileSettingSer.ColumnCollection.Where(x => x.Ignore))
					col.Ignore = false;

				return new Tuple<DelimitedFileDetectionResult, IEnumerable<Column>>(new DelimitedFileDetectionResult(fileSettingSer), fileSettingSer.ColumnCollection);
			}

			var setting = ManifestData.ReadManifestZip(fileName);
			if (setting != null)
			{
				Logger.Information("Data in zip {filename}", setting.Item1.IdentifierInContainer);
				return setting;
			}

			var settingFs = ManifestData.ReadManifestFileSystem(fileName);
			if (settingFs != null)
			{
				Logger.Information("Data in {filename}", settingFs.Item1.FileName);
				return settingFs;
			}

			// Determine from file
			var detectionResult = await GetDetectionResultFromFile(fileName, processDisplay,
				guessJson,
				guessCodePage,
				guessDelimiter,
				guessQualifier,
				guessStartRow,
				guessHasHeader,
				guessNewLine);

			if (detectionResult.IsJson)
				detectionResult.SkipRows = 0;

			processDisplay.SetProcess("Determining column format by reading samples", -1, true);
			var csv = detectionResult.CsvFile(null);
			await csv.FillGuessColumnFormatReaderAsync(
				true,
				false,
				fillGuessSettings,
				processDisplay.CancellationToken);

			return new Tuple<DelimitedFileDetectionResult, IEnumerable<Column>>(detectionResult, csv.ColumnCollection);
		}

		public static async Task UpdateDetectionResultFromStream([NotNull] IImprovedStream improvedStream, [NotNull] DelimitedFileDetectionResult detectionResult, [NotNull] IProcessDisplay display,
																																bool guessJson, bool guessCodePage, bool guessDelimiter, bool guessQualifier,
																																bool guessStartRow, bool guessHasHeader, bool guessNewLine)
		{
			if (!(guessJson || guessCodePage || guessDelimiter || guessStartRow || guessQualifier || guessHasHeader ||
						guessNewLine))
				return;

			if (guessCodePage)
			{
				if (display.CancellationToken.IsCancellationRequested)
					return;
				improvedStream.Seek(0, SeekOrigin.Begin);
				display.SetProcess("Checking Code Page", -1, true);
				var (codePage, bom) = await GuessCodePageFromStrean(improvedStream, display.CancellationToken).ConfigureAwait(false);
				detectionResult.CodePageId = codePage;
				detectionResult.ByteOrderMark = bom;
			}

			if (guessJson)
			{
				display.SetProcess("Checking Json format", -1, false);
				if (await IsJsonReadableFromStream(improvedStream, Encoding.GetEncoding(detectionResult.CodePageId), display.CancellationToken).ConfigureAwait(false))
					detectionResult.IsJson = true;
			}

			if (detectionResult.IsJson)
			{
				display.SetProcess("Detected Json file", -1, false);
				return;
			}

			display.SetProcess("Checking delimited text file", -1, true);
			char oldDelimiter = detectionResult.FieldDelimiter.WrittenPunctuationToChar();
			// from here on us the encoding to read the stream again
			if (guessStartRow && oldDelimiter != 0)
			{
				if (display.CancellationToken.IsCancellationRequested)
					return;
				using (var streamReader = await GetStreamReaderAtStart(improvedStream, detectionResult.CodePageId, detectionResult.SkipRows, display.CancellationToken))
				{
					detectionResult.SkipRows = GuessStartRowFromReader(streamReader, detectionResult.FieldDelimiter.WrittenPunctuationToChar(),
						detectionResult.FieldQualifier.WrittenPunctuationToChar(),
						detectionResult.CommentLine, display.CancellationToken);
				}
			}

			if (guessQualifier || guessDelimiter || guessNewLine)
			{
				using (var textReader = await GetStreamReaderAtStart(improvedStream, detectionResult.CodePageId, detectionResult.SkipRows, display.CancellationToken))
				{
					if (guessDelimiter)
					{
						if (display.CancellationToken.IsCancellationRequested)
							return;
						display.SetProcess("Checking Column Delimiter", -1, false);
						var (delimiter, noDelimiter) = GuessDelimiterFromReader(textReader, detectionResult.EscapeCharacterChar,
							display.CancellationToken);
						detectionResult.NoDelimitedFile = noDelimiter;
						detectionResult.FieldDelimiter = delimiter;
					}

					if (guessNewLine)
					{
						if (display.CancellationToken.IsCancellationRequested)
							return;
						display.SetProcess("Checking Record Delimiter", -1, false);
						improvedStream.Seek(0, SeekOrigin.Begin);
						detectionResult.NewLine = GuessNewlineFromReader(textReader, detectionResult.FieldQualifier.WrittenPunctuationToChar(),
							display.CancellationToken);
					}

					if (guessQualifier)
					{
						if (display.CancellationToken.IsCancellationRequested)
							return;
						display.SetProcess("Checking Qualifier", -1, false);
						var qualifier = GuessQualifierFromReader(textReader, detectionResult.FieldDelimiter.WrittenPunctuationToChar(),
							display.CancellationToken);
						if (qualifier != '\0')
							detectionResult.FieldQualifier = char.ToString(qualifier);
					}
				}
			}

			// find start row again , with possibly changed FieldDelimiter
			if (guessStartRow && oldDelimiter != detectionResult.FieldDelimiter.WrittenPunctuationToChar())
			{
				if (oldDelimiter != 0)
					Logger.Information("  Checking start row again because previously assumed delimiter has changed");
				if (display.CancellationToken.IsCancellationRequested)
					return;
				using (var streamReader2 = await GetStreamReaderAtStart(improvedStream, detectionResult.CodePageId, 0, display.CancellationToken))
				{
					streamReader2.ToBeginning();
					detectionResult.SkipRows = GuessStartRowFromReader(streamReader2, detectionResult.FieldDelimiter.WrittenPunctuationToChar(),
						detectionResult.FieldQualifier.WrittenPunctuationToChar(),
						detectionResult.CommentLine, display.CancellationToken);
				}
			}

			if (guessHasHeader)
			{
				if (display.CancellationToken.IsCancellationRequested)
					return;
				display.SetProcess("Checking for Header Row", -1, false);
				detectionResult.HasFieldHeader = (await GuessHasHeaderFromStream(improvedStream, detectionResult.CodePageId, detectionResult.SkipRows, detectionResult.CommentLine, detectionResult.FieldDelimiter.WrittenPunctuationToChar(), display.CancellationToken)).Item1;
			}
		}

		public static async Task<Tuple<int, bool>> GuessCodePageFromStrean([NotNull] IImprovedStream stream,
																																			 CancellationToken token)
		{
			// Read 256 kBytes
			var buff = new byte[262144];

			var length = await stream.ReadAsync(buff, 0, buff.Length, token).ConfigureAwait(false);
			if (length >= 2)
			{
				var byBom = EncodingHelper.GetEncodingByByteOrderMark(buff);
				if (byBom != null)
				{
					Logger.Information("Code Page: {encoding}", EncodingHelper.GetEncodingName(byBom, true));
					return new Tuple<int, bool>(byBom.CodePage, true);
				}
			}
			var detected = EncodingHelper.GuessEncodingNoBom(buff);
			if (detected.Equals(Encoding.ASCII))
				detected = Encoding.UTF8;
			Logger.Information("Code Page: {encoding}", EncodingHelper.GetEncodingName(detected, false));
			return new Tuple<int, bool>(detected.CodePage, false);
		}

		/// <summary>
		///   Guesses the delimiter for a files. Done with a rather simple csv parsing, and trying to
		///   find the delimiter that has the least variance in the read rows, if that is not possible
		///   the delimiter with the highest number of occurrences.
		/// </summary>
		/// <param name="setting">The CSVFile fileSetting</param>
		/// <param name="cancellationToken"></param>
		/// <returns>A character with the assumed delimiter for the file</returns>
		/// <remarks>No Error will not be thrown.</remarks>
		[NotNull]
		public static async Task<Tuple<string, bool>> GuessDelimiterFromStrean([NotNull] IImprovedStream improvedStream, int codePageId, int skipRows, char escapeCharacterChar,
																												 CancellationToken cancellationToken)
		{
			using (var textReader = await GetStreamReaderAtStart(improvedStream, codePageId, skipRows, cancellationToken))
			{
				textReader.ToBeginning();
				return GuessDelimiterFromReader(textReader, escapeCharacterChar, cancellationToken);
			}
		}

		public static Tuple<bool, string> GuessHasHeaderFromReader([NotNull] ImprovedTextReader reader, string comment,
																												 char delimiter, CancellationToken cancellationToken)
		{
			var headerLine = string.Empty;

			while (string.IsNullOrEmpty(headerLine) && !reader.EndOfStream)
			{
				cancellationToken.ThrowIfCancellationRequested();
				headerLine = reader.ReadLine();
				if (!string.IsNullOrEmpty(comment) && headerLine.TrimStart().StartsWith(comment))
					headerLine = string.Empty;
			}

			try
			{
				if (string.IsNullOrEmpty(headerLine))
					throw new ApplicationException("Empty Line");

				if (headerLine.NoControlCharacters().Length < headerLine.Replace("\t", "").Length)
					throw new ApplicationException($"Control Characters in Column {headerLine}");

				var headerRow = headerLine.Split(delimiter).Select(x => x.Trim('\"')).ToList();

				// get the average field count looking at the header and 12 additional valid lines
				var fieldCount = headerRow.Count;

				// if there is only one column the header be number of letter and might be followed by a
				// single number
				if (fieldCount < 2)
				{
					if (!(headerLine.Length > 2 && Regex.IsMatch(headerLine, @"^[a-zA-Z]+\d?$")))
						throw new ApplicationException($"Only one column: {headerLine}");
				}
				else
				{
					var counter = 1;
					while (counter < 12 && !cancellationToken.IsCancellationRequested && !reader.EndOfStream)
					{
						var dataLine = reader.ReadLine();
						if (string.IsNullOrEmpty(dataLine)
								|| !string.IsNullOrEmpty(comment) && dataLine.TrimStart().StartsWith(comment))
							continue;
						counter++;
						fieldCount += dataLine.Split(delimiter).Length;
					}

					var avgFieldCount = fieldCount / (double) counter;
					// The average should not be smaller than the columns in the initial row
					if (avgFieldCount < headerRow.Count)
						avgFieldCount = headerRow.Count;
					var halfTheColumns = (int) Math.Ceiling(avgFieldCount / 2.0);

					// use the same routine that is used in readers to determine the names of the columns
					var (newHeader, numIssues) = BaseFileReader.AdjustColumnName(headerRow, (int) avgFieldCount, null, null);

					// looking at the warnings raised
					if (numIssues >= halfTheColumns || numIssues > 2)
						throw new ApplicationException($"{numIssues} header where empty, duplicate or too long");

					// Columns are only one or two char, does not look descriptive
					if (newHeader.Count(x => x.Length < 3) > halfTheColumns)
						throw new ApplicationException(
							$"Headers '{string.Join("', '", newHeader.Where(x => x.Length < 3))}' very short");

					var numeric = headerRow.Where(header => Regex.IsMatch(header, @"^\d+$")).ToList();
					var specials = headerRow.Where(header => Regex.IsMatch(header, @"[^\w\d\-_\s<>#,.*\[\]\(\)+?!]")).ToList();
					if (numeric.Count + specials.Count >= halfTheColumns)
					{
						var msg = new StringBuilder();
						if (numeric.Count > 0)
						{
							msg.Append("Headers ");
							foreach (var header in numeric)
							{
								msg.Append("'");
								msg.Append(header.Trim('\"'));
								msg.Append("',");
							}

							msg.Length--;
							msg.Append(" numeric");
						}

						if (specials.Count > 0)
						{
							if (msg.Length > 0)
								msg.Append(" and ");
							msg.Append("Headers ");
							foreach (var header in specials)
							{
								msg.Append("'");
								msg.Append(header.Trim('\"'));
								msg.Append("',");
							}

							msg.Length--;
							msg.Append(" with uncommon characters");
						}

						throw new ApplicationException(msg.ToString());
					}
				}
			}
			catch (ApplicationException ex)
			{
				Logger.Information("  Without Header Row {reason}", ex.Message);
				return new Tuple<bool, string>(false, ex.Message);
			}

			Logger.Information("  Has Header Row");
			return new Tuple<bool, string>(true, "Header seems present");
		}

		public static async Task<Tuple<bool, string>> GuessHasHeaderFromStream([NotNull] IImprovedStream improvedStream, int codePageId, int skipRows, string commentLine, char fieldDelimiterChar, CancellationToken cancellationToken)
		{
			using (var reader = await GetStreamReaderAtStart(improvedStream, codePageId, skipRows, cancellationToken))
				return GuessHasHeaderFromReader(reader, commentLine, fieldDelimiterChar, cancellationToken);
		}

		/// <summary>
		///   Try to guess the new line sequence
		/// </summary>
		/// <param name="cancellationToken">A cancellation token</param>
		/// <returns>The NewLine Combination used</returns>
		[NotNull]
		public static async Task<RecordDelimiterType> GuessNewlineFromStream([NotNull] IImprovedStream improvedStream, int codePageId, int skipRows, char fieldQualifierChar, CancellationToken cancellationToken)
		{
			using (var textReader = await GetStreamReaderAtStart(improvedStream, codePageId, skipRows, cancellationToken))
				return GuessNewlineFromReader(textReader, fieldQualifierChar, cancellationToken);
		}

		/// <summary>
		///   Try to guess the new line sequence
		/// </summary>
		/// <param name="cancellationToken">A cancellation token</param>
		/// <returns>The NewLine Combination used</returns>
		[NotNull]
		public static async Task<string> GuessQualifierFromStream([NotNull] IImprovedStream improvedStream, int codePageId, int skipRows, char fieldDelimiterChar,
																														 CancellationToken cancellationToken)
		{
			using (var textReader = await GetStreamReaderAtStart(improvedStream, codePageId, skipRows, cancellationToken))
			{
				var qualifier = GuessQualifierFromReader(textReader, fieldDelimiterChar, cancellationToken);
				if (qualifier != '\0')
					return char.ToString(qualifier);
			}
			return null;
		}

		/// <summary>
		///   Guesses the start row of a CSV file Done with a rather simple csv parsing
		/// </summary>
		/// <param name="textReader">The stream reader with the data</param>
		/// <param name="delimiter">The delimiter.</param>
		/// <param name="quoteChar">The quoting char</param>
		/// <param name="commentLine">The characters for a comment line.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>The number of rows to skip</returns>
		/// <exception cref="ArgumentNullException">commentLine</exception>
		public static int GuessStartRowFromReader([NotNull] ImprovedTextReader textReader, char delimiter,
																		 char quoteChar,
																		 string commentLine, CancellationToken cancellationToken)
		{
			if (textReader == null) throw new ArgumentNullException(nameof(textReader));
			if (commentLine == null)
				throw new ArgumentNullException(nameof(commentLine));
			const int c_MaxRows = 50;

			textReader.ToBeginning();
			var columnCount = new List<int>(c_MaxRows);
			var rowMapping = new Dictionary<int, int>(c_MaxRows);
			var colCount = new int[c_MaxRows];
			var isComment = new bool[c_MaxRows];
			var quoted = false;
			var firstChar = true;
			var lastRow = 0;
			var retValue = 0;

			while (lastRow < c_MaxRows && !textReader.EndOfStream && !cancellationToken.IsCancellationRequested)
			{
				var readChar = textReader.Read();

				// Handle Commented lines
				if (firstChar && commentLine.Length > 0 && !isComment[lastRow] && readChar == commentLine[0])
				{
					isComment[lastRow] = true;

					for (var pos = 1; pos < commentLine.Length; pos++)
					{
						var nextChar = textReader.Peek();
						if (nextChar == commentLine[pos]) continue;
						isComment[lastRow] = false;
						break;
					}
				}

				// Handle Quoting
				if (readChar == quoteChar && !isComment[lastRow])
				{
					if (quoted)
					{
						if (textReader.Peek() != '"')
							quoted = false;
						else
							textReader.MoveNext();
					}
					else
					{
						quoted |= firstChar;
					}

					continue;
				}

				switch (readChar)
				{
					// Feed and NewLines
					case '\n':
						if (!quoted)
						{
							lastRow++;
							firstChar = true;
							if (textReader.Peek() == '\r')
								textReader.MoveNext();
						}

						break;

					case '\r':
						if (!quoted)
						{
							lastRow++;
							firstChar = true;
							if (textReader.Peek() == '\n')
								textReader.MoveNext();
						}

						break;

					default:
						if (!isComment[lastRow] && !quoted && readChar == delimiter)
						{
							colCount[lastRow]++;
							firstChar = true;
							continue;
						}

						break;
				}

				// Its still the first char if its a leading space
				if (firstChar && readChar != ' ')
					firstChar = false;
			}

			cancellationToken.ThrowIfCancellationRequested();
			// remove all rows that are comment lines...
			for (var row = 0; row < lastRow; row++)
			{
				rowMapping[columnCount.Count] = row;
				if (!isComment[row])
					columnCount.Add(colCount[row]);
			}

			// if we do not more than 4 proper rows do nothing
			if (columnCount.Count > 4)
			{
				// In case we have a row that is exactly twice as long as the row before and row after,
				// assume its missing a linefeed
				for (var row = 1; row < columnCount.Count - 1; row++)
					if (columnCount[row + 1] > 0 && columnCount[row] == columnCount[row + 1] * 2 &&
							columnCount[row] == columnCount[row - 1] * 2)
						columnCount[row] = columnCount[row + 1];
				cancellationToken.ThrowIfCancellationRequested();
				// Get the average of the last 15 rows
				var num = 0;
				var sum = 0;
				for (var row = columnCount.Count - 1; num < 10 && row > 0; row--)
				{
					if (columnCount[row] <= 0)
						continue;
					sum += columnCount[row];
					num++;
				}

				var avg = (int) (sum / (double) (num == 0 ? 1 : num));
				// If there are not many columns do not try to guess
				if (avg > 1)
				{
					// If the first rows would be a good fit return this
					if (columnCount[0] < avg)
					{
						for (var row = columnCount.Count - 1; row > 0; row--)
							if (columnCount[row] > 0)
							{
								if (columnCount[row] >= avg - avg/10) continue;
								retValue =  rowMapping[row];
								break;
							}
							// In case we have an empty line but the next line are roughly good match take that
							// empty line
							else if (row + 2 < columnCount.Count && columnCount[row + 1] == columnCount[row + 2] &&
											 columnCount[row + 1] >= avg - 1)
							{
								retValue = rowMapping[row + 1];
								break;
							}
						if (retValue==0)
							for (var row = 0; row < columnCount.Count; row++)
								if (columnCount[row] > 0)
								{
									retValue = rowMapping[row];
									break;
								}
					}
				}
			}
			Logger.Information("  Start Row: {row}", retValue);
			return retValue;
		}

		/// <summary>
		///   Determines the start row in the file
		/// </summary>
		/// <param name="cancellationToken">A cancellation token</param>
		/// <returns>The number of rows to skip</returns>
		[NotNull]
		public static async Task<int> GuessStartRowFromStream([NotNull] IImprovedStream improvedStream, int codePageID, char fieldDelimiterChar, char fieldQualifierChar, string commentLine, CancellationToken cancellationToken)
		{
			using (var streamReader = await GetStreamReaderAtStart(improvedStream, codePageID, 0, cancellationToken))
				return GuessStartRowFromReader(streamReader, fieldDelimiterChar, fieldQualifierChar, commentLine, cancellationToken);
		}

		/// <summary>
		///   Does check if quoting was actually used in the file
		/// </summary>
		/// <param name="setting">The setting.</param>
		/// <param name="cancellationToken">A cancellation token</param>
		/// <returns><c>true</c> if [has used qualifier] [the specified setting]; otherwise, <c>false</c>.</returns>
		[NotNull]
		public static async Task<bool> HasUsedQualifierFromStream([NotNull] IImprovedStream improvedStream, int codePageId, int skipRows, char fieldDelimiterChar, char fieldQualifierChar,
																												 CancellationToken cancellationToken)
		{
			// if we do not have a quote defined it does not matter
			if (fieldQualifierChar== '\0' || cancellationToken.IsCancellationRequested)
				return false;

			using (var streamReader = await GetStreamReaderAtStart(improvedStream, codePageId, skipRows, cancellationToken))
			{
				streamReader.ToBeginning();
				var isStartOfColumn = true;
				while (!streamReader.EndOfStream)
				{
					if (cancellationToken.IsCancellationRequested)
						return false;
					var c = (char) streamReader.Read();
					if (c == '\r' || c == '\n' || c == fieldDelimiterChar)
					{
						isStartOfColumn = true;
						continue;
					}

					// if we are not at the start of a column we can get the next char
					if (!isStartOfColumn)
						continue;
					// If we are at the start of a column and this is a ", we can stop, this is a real qualifier
					if (c == fieldQualifierChar)
						return true;
					// Any non whitespace will reset isStartOfColumn
					if (c <= '\x00ff')
						isStartOfColumn = c == ' ' || c == '\t';
					else
						isStartOfColumn = CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
				}
			}

			return false;
		}

		[NotNull]
		public static async Task<bool> IsJsonReadableFromStream([NotNull] IImprovedStream impStream, Encoding encoding,
																												CancellationToken cancellationToken)
		{
			if (!(impStream is Stream stream))
				return false;

			impStream.Seek(0, SeekOrigin.Begin);
			using (var streamReader = new StreamReader(stream, encoding, true, 4096, true))
			using (var jsonTextReader = new JsonTextReader(streamReader))
			{
				jsonTextReader.CloseInput = false;
				try
				{
					if (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						Logger.Information("  Detected Json file");
						if (jsonTextReader.TokenType == JsonToken.StartObject ||
								jsonTextReader.TokenType == JsonToken.StartArray ||
								jsonTextReader.TokenType == JsonToken.StartConstructor)
						{
							await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							return true;
						}
					}
				}
				catch (JsonReaderException)
				{
					//ignore
				}
			}

			return false;
		}

		/// <summary>
		///   Refreshes the settings assuming the file has changed, checks CodePage, Delimiter, Start
		///   Row and Header
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="display">The display.</param>
		/// <param name="guessJson">if true trying to determine if file is a JSOn file</param>
		/// <param name="guessCodePage">if true, try to determine the code page</param>
		/// <param name="guessDelimiter">if true, try to determine the delimiter</param>
		/// <param name="guessQualifier">if true, try to determine the qualifier for text</param>
		/// <param name="guessStartRow">if true, try to determine the number of skipped rows</param>
		/// <param name="guessHasHeader">
		///   if true, try to determine if the file does have a header row
		/// </param>
		/// <param name="guessNewLine">if true, try to determine what kind of new line we do use</param>
		[NotNull]
		public static async Task<DelimitedFileDetectionResult> GetDetectionResultFromFile(
			[NotNull] string fileName,
			[NotNull] IProcessDisplay display,
			bool guessJson = false,
			bool guessCodePage = true,
			bool guessDelimiter = true,
			bool guessQualifier = true,
			bool guessStartRow = true,
			bool guessHasHeader = true,
			bool guessNewLine = true)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentException("file name can not be empty", nameof(fileName));

			using (var improvedStream = FunctionalDI.OpenStream(new SourceAccess(fileName)))
			{
				var detectionResult = new DelimitedFileDetectionResult(fileName);
				await UpdateDetectionResultFromStream(improvedStream, detectionResult, display, guessJson, guessCodePage, guessDelimiter,
										guessQualifier, guessStartRow, guessHasHeader, guessNewLine);
				return detectionResult;
			}
		}

		private static async Task<int> CodePageResolve([NotNull] IImprovedStream improvedStream, int codePageId, CancellationToken cancellationToken)
		{
			if (codePageId < 0)
			{
				codePageId = (await GuessCodePageFromStrean(improvedStream, cancellationToken).ConfigureAwait(false)).Item1;
				improvedStream.Seek(0, SeekOrigin.Begin);
			}
			return codePageId;
		}

		[NotNull]
		private static DelimiterCounter GetDelimiterCounterFromReader([NotNull] ImprovedTextReader textReader, char escapeCharacter, int numRows, CancellationToken cancellationToken)
		{
			if (textReader == null) throw new ArgumentNullException(nameof(textReader));

			var dc = new DelimiterCounter(numRows);

			var quoted = false;
			var firstChar = true;
			var readChar = -1;
			//var contends = new StringBuilder();
			var textReaderPosition = new ImprovedTextReaderPositionStore(textReader);

			while (dc.LastRow < dc.NumRows && !textReaderPosition.AllRead() &&
						 !cancellationToken.IsCancellationRequested)
			{
				var lastChar = readChar;
				readChar = textReader.Read();
				//contends.Append(readChar);
				if (lastChar == escapeCharacter)
					continue;
				switch (readChar)
				{
					case '"':
						if (quoted)
						{
							if (textReader.Peek() != '"')
								quoted = false;
							else
								textReader.MoveNext();
						}
						else
						{
							quoted |= firstChar;
						}

						break;

					case '\n':
					case '\r':
						if (!quoted && !firstChar)
						{
							dc.LastRow++;
							firstChar = true;
							continue;
						}

						break;

					default:
						if (!quoted)
						{
							var index = dc.Separators.IndexOf((char) readChar);
							if (index != -1)
							{
								if (dc.SeparatorsCount[index, dc.LastRow] == 0)
									dc.SeparatorRows[index]++;
								++dc.SeparatorsCount[index, dc.LastRow];
								firstChar = true;
								continue;
							}
						}

						break;
				}

				// Its still the first char if its a leading space
				if (firstChar && readChar != ' ')
					firstChar = false;
			}

			return dc;
		}

		private static async Task<ImprovedTextReader> GetStreamReaderAtStart(IImprovedStream improvedStream, int codePageId, int skipRows, CancellationToken cancellationToken)
		{
			var textReader = new ImprovedTextReader(improvedStream, await CodePageResolve(improvedStream, codePageId, cancellationToken), skipRows);
			textReader.ToBeginning();
			return textReader;
		}

		/// <summary>
		///   Guesses the delimiter for a files. Done with a rather simple csv parsing, and trying to
		///   find the delimiter that has the least variance in the read rows, if that is not possible
		///   the delimiter with the highest number of occurrences.
		/// </summary>
		/// <param name="textReader">The StreamReader with the data</param>
		/// <param name="escapeCharacter">The escape character.</param>
		/// <param name="cancellationToken">A cancellation token</param>
		/// <returns>A character with the assumed delimiter for the file</returns>
		/// <exception cref="ArgumentNullException">streamReader</exception>
		/// <remarks>No Error will not be thrown.</remarks>
		[NotNull]
		private static Tuple<string, bool> GuessDelimiterFromReader([NotNull] ImprovedTextReader textReader, char escapeCharacter, CancellationToken cancellationToken)
		{
			if (textReader == null)
				throw new ArgumentNullException(nameof(textReader));
			var match = '\0';

			var dc = GetDelimiterCounterFromReader(textReader, escapeCharacter, 300, cancellationToken);
			var numberOfRows = dc.FilledRows;

			// Limit everything to 100 columns max, the sum might get too big otherwise 100 * 100
			var startRow = dc.LastRow > 60 ? 15 :
										 dc.LastRow > 20 ? 5 : 0;

			var neededRows = (((dc.FilledRows>20) ? numberOfRows * 75 : numberOfRows * 50) / 100);

			cancellationToken.ThrowIfCancellationRequested();
			var validSeparatorIndex = new List<int>();
			for (var index = 0; index < dc.Separators.Length; index++)
			{
				// only regard a delimiter if we have 75% of the rows with this delimiter we can still have
				// a lot of commented lines
				if (dc.SeparatorRows[index] == 0 || dc.SeparatorRows[index] < neededRows && numberOfRows > 3)
					continue;
				validSeparatorIndex.Add(index);
			}

			if (validSeparatorIndex.Count == 0)
			{
				// we can not determine by the numer of rwes That the delimiter with most occurance in general
				int maxNum = int.MinValue;
				for (var index = 0; index < dc.Separators.Length; index++)
				{
					var sumCount = 0;
					for (var row = startRow; row < dc.LastRow; row++)
						sumCount += dc.SeparatorsCount[index, row];
					if (sumCount > maxNum)
					{
						maxNum=sumCount;
						match = dc.Separators[index];
					}
				}
			}
			else if (validSeparatorIndex.Count == 1)
			{
				// if only one was found done here
				match = dc.Separators[validSeparatorIndex[0]];
			}
			else
			{
				// otherwise find the best
				foreach (var index in validSeparatorIndex)
					for (var row = startRow; row < dc.LastRow; row++)
						if (dc.SeparatorsCount[index, row] > 100)
							dc.SeparatorsCount[index, row] = 100;

				double? bestScore = null;
				var maxCount = 0;

				foreach (var index in validSeparatorIndex)
				{
					cancellationToken.ThrowIfCancellationRequested();
					var sumCount = 0;
					// If there are enough rows skip the first rows, there might be a descriptive introduction
					// this can not be done in case there are not many rows
					for (var row = startRow; row < dc.LastRow; row++)
						// Cut of at 50 Columns in case one row is messed up, this should not mess up everything
						sumCount += dc.SeparatorsCount[index, row];

					// If we did not find a match with variance use the absolute number of occurrences
					if (sumCount > maxCount && !bestScore.HasValue)
					{
						maxCount = sumCount;
						match = dc.Separators[index];
					}

					// Get the average of the rows
					var avg = (int) Math.Round((double) sumCount / (dc.LastRow - startRow), 0, MidpointRounding.AwayFromZero);

					// Only proceed if there is usually more then one occurrence and we have more then one row
					if (avg < 1 || dc.SeparatorRows[index] == 1)
						continue;

					// First determine the variance, low value means and even distribution
					double cutVariance = 0;
					for (var row = startRow; row < dc.LastRow; row++)
					{
						var dist = dc.SeparatorsCount[index, row] - avg;
						if (dist > 2 || dist < -2)
							cutVariance += 8;
						else
							switch (dist)
							{
								case 2:
								case -2:
									cutVariance += 4;
									break;

								case 1:
								case -1:
									cutVariance++;
									break;
							}
					}

					// The score is dependent on the average columns found and the regularity
					var score = Math.Abs(avg - Math.Round(cutVariance / (dc.LastRow - startRow), 2));
					if (bestScore.HasValue && !(score > bestScore.Value))
						continue;
					match = dc.Separators[index];
					bestScore = score;
				}
			}

			var hasDelimiter = match != '\0';
			if (!hasDelimiter)
			{
				Logger.Information("Not a delimited file");
				return new Tuple<string, bool>("TAB", false);
			}

			var result = match == '\t' ? "TAB" : match.ToString(CultureInfo.CurrentCulture);
			Logger.Information("  Column Delimiter: {delimiter}", result);
			return new Tuple<string, bool>(result, true);
		}

		private static RecordDelimiterType GuessNewlineFromReader([NotNull] ImprovedTextReader textReader,
																										char fieldQualifier,
																										CancellationToken token)
		{
			if (textReader == null) throw new ArgumentNullException(nameof(textReader));
			const int c_NumChars = 8192;

			var currentChar = 0;
			var quoted = false;

			const int c_Cr = 0;
			const int c_LF = 1;
			const int c_CrLf = 2;
			const int c_LFCr = 3;
			const int c_RecSep = 4;
			const int c_UnitSep = 5;

			int[] count = { 0, 0, 0, 0, 0, 0 };

			// \r = CR (Carriage Return) \n = LF (Line Feed)

			var textReaderPosition = new ImprovedTextReaderPositionStore(textReader);
			while (currentChar < c_NumChars && !textReaderPosition.AllRead() && !token.IsCancellationRequested)
			{
				var readChar = textReader.Read();
				if (readChar == fieldQualifier)
				{
					if (quoted)
					{
						if (textReader.Peek() != fieldQualifier)
							quoted = false;
						else
							textReader.MoveNext();
					}
					else
					{
						quoted = true;
					}
				}

				if (quoted)
					continue;

				switch (readChar)
				{
					case 30:
						count[c_RecSep]++;
						continue;
					case 31:
						count[c_UnitSep]++;
						continue;
					case 10:
					{
						if (textReader.Peek() == 13)
						{
							textReader.MoveNext();
							count[c_LFCr]++;
						}
						else
						{
							count[c_LF]++;
						}

						currentChar++;
						break;
					}
					case 13:
					{
						if (textReader.Peek() == 10)
						{
							textReader.MoveNext();
							count[c_CrLf]++;
						}
						else
						{
							count[c_Cr]++;
						}

						break;
					}
				}

				currentChar++;
			}

			var maxCount = count.Max();
			if (maxCount == 0)
				return RecordDelimiterType.None;
			var res = count[c_RecSep] == maxCount ? RecordDelimiterType.RS
								: count[c_UnitSep] == maxCount ? RecordDelimiterType.US
								: count[c_Cr] == maxCount ? RecordDelimiterType.CR
								: count[c_LF] == maxCount ? RecordDelimiterType.LF
								: count[c_LFCr] == maxCount ? RecordDelimiterType.LFCR
								: count[c_CrLf] == maxCount ? RecordDelimiterType.CRLF
								: RecordDelimiterType.None;
			Logger.Information("  Record Delimiter: {recorddelimiter}", res.Description());
			return res;
		}

		private static char GuessQualifierFromReader([NotNull] ImprovedTextReader textReader, char delimiter, CancellationToken cancellationToken)
		{
			if (textReader == null) throw new ArgumentNullException(nameof(textReader));

			const int c_MaxLine = 30;
			var possibleQuotes = new[] { '"', '\'' };
			var counter = new int[possibleQuotes.Length];

			var textReaderPosition = new ImprovedTextReaderPositionStore(textReader);
			var max = 0;
			// skip the first line it usually a header
			for (var lineNo = 0;
					 lineNo < c_MaxLine && !textReaderPosition.AllRead() && !cancellationToken.IsCancellationRequested;
					 lineNo++)
			{
				var line = textReader.ReadLine();
				// EOF
				if (line == null)
				{
					if (textReaderPosition.CanStartFromBeginning())
						continue;
					break;
				}

				var cols = line.Split(delimiter);
				foreach (var col in cols)
				{
					if (string.IsNullOrWhiteSpace(col))
						continue;

					var test = col.Trim();
					for (var testChar = 0; testChar < possibleQuotes.Length; testChar++)
					{
						if (test[0] != possibleQuotes[testChar]) continue;
						counter[testChar]++;
						// Ideally column need to start and end with the same characters (but end quote could be
						// on another line) if the start and end are indeed the same give it extra credit
						if (test.Length > 1 && test[0] == test[test.Length - 1])
							counter[testChar]++;
						if (counter[testChar] > max)
							max = counter[testChar];
					}
				}
			}

			var res = max < 1 ? '\0' : possibleQuotes.Where((t, testChar) => counter[testChar] == max).FirstOrDefault();
			if (res != '\0')
				Logger.Information("  Column Qualifier: {qualifier}", res);
			else
				Logger.Information("  No Column Qualifier");
			return res;
		}
	}
}