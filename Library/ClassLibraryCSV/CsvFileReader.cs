﻿/*
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;

namespace CsvTools
{
  /// <summary>
  ///   A data reader for CSV files
  /// </summary>
  public sealed class CsvFileReader : BaseFileReader, IFileReader
  {
    /// <summary>
    ///   Constant: Line has fewer columns than expected
    /// </summary>
    public const string cLessColumns = " has fewer columns than expected";

    /// <summary>
    ///   Constant: Line has more columns than expected
    /// </summary>
    public const string cMoreColumns = " has more columns than expected";

    // Buffer size set to 64kB, if set to large the display in percentage will jump
    private const int c_BufferSize = 65536;

    /// <summary>
    ///   The carriage return character. Escape code is <c>\r</c>.
    /// </summary>
    private const char c_Cr = (char)0x0d;

    /// <summary>
    ///   The line-feed character. Escape code is <c>\n</c>.
    /// </summary>
    private const char c_Lf = (char)0x0a;

    /// <summary>
    ///   A non-breaking space..
    /// </summary>
    private const char c_Nbsp = (char)0xA0;

    private const char c_UnknownChar = (char)0xFFFD;

    /// <summary>
    ///   16k Buffer of the file data
    /// </summary>
    private readonly char[] m_Buffer = new char[c_BufferSize];

    private readonly ICsvFile m_CsvFile;

    /// <summary>
    ///   Length of the buffer (can be smaller then buffer size at end of file)
    /// </summary>
    private int m_BufferFilled;

    /// <summary>
    ///   Position in the buffer
    /// </summary>
    private int m_BufferPos = -1;

    private int m_ConsecutiveEmptyRows;

    private bool m_DisposedValue;

    /// <summary>
    ///   If the End of the line is reached this is true
    /// </summary>
    private bool m_EndOfLine;

    private bool m_HasQualifier;
    private string[] m_HeaderRow;
    private IImprovedStream m_ImprovedStream;

    /// <summary>
    ///   Number of Records in the text file, only set if all records have been read
    /// </summary>
    private ushort m_NumWarningsDelimiter;

    private ushort m_NumWarningsLinefeed;
    private ushort m_NumWarningsNbspChar;
    private ushort m_NumWarningsQuote;
    private ushort m_NumWarningsUnknownChar;

    private ReAlignColumns m_RealignColumns;

    /// <summary>
    ///   The TextReader to read the file
    /// </summary>
    private StreamReader m_TextReader;

    public CsvFileReader(ICsvFile fileSetting, string timeZone, IProcessDisplay processDisplay)
      : base(fileSetting, timeZone, processDisplay)
    {
      m_CsvFile = fileSetting;
      if (string.IsNullOrEmpty(m_CsvFile.FileName))
        throw new FileReaderException("FileName must be set");

      // if it can not be downloaded it has to exist
      if (string.IsNullOrEmpty(m_CsvFile.RemoteFileName) || !HasOpen())
        if (!FileSystemUtils.FileExists(m_CsvFile.FullPath))
          throw new FileNotFoundException(
            $"The file '{FileSystemUtils.GetShortDisplayFileName(m_CsvFile.FileName, 80)}' does not exist or is not accessible.",
            m_CsvFile.FileName);
      if (m_CsvFile.FileFormat.FieldDelimiterChar == c_Cr ||
          m_CsvFile.FileFormat.FieldDelimiterChar == c_Lf ||
          m_CsvFile.FileFormat.FieldDelimiterChar == ' ' ||
          m_CsvFile.FileFormat.FieldDelimiterChar == '\0')
        throw new FileReaderException(
          "The field delimiter character is invalid, please use something else than CR, LF or Space");

      if (m_CsvFile.FileFormat.FieldDelimiterChar == m_CsvFile.FileFormat.EscapeCharacterChar)
        throw new FileReaderException(
          $"The escape character is invalid, please use something else than the field delimiter character {FileFormat.GetDescription(m_CsvFile.FileFormat.EscapeCharacter)}.");

      m_HasQualifier |= m_CsvFile.FileFormat.FieldQualifierChar != '\0';

      if (!m_HasQualifier)
        return;
      if (m_CsvFile.FileFormat.FieldQualifierChar == m_CsvFile.FileFormat.FieldDelimiterChar)
        throw new ArgumentOutOfRangeException(
          $"The text quoting and the field delimiter characters of a delimited file cannot be the same. {m_CsvFile.FileFormat.FieldDelimiterChar}");
      if (m_CsvFile.FileFormat.FieldQualifierChar == c_Cr || m_CsvFile.FileFormat.FieldQualifierChar == c_Lf)
        throw new FileReaderException(
          "The text quoting characters is invalid, please use something else than CR or LF");
    }

    /// <summary>
    ///   Gets a value indicating whether this instance is closed.
    /// </summary>
    /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
    public bool IsClosed => m_TextReader == null;

    // To detect redundant calls

    public override void Close()
    {
      m_NumWarningsQuote = 0;
      m_NumWarningsDelimiter = 0;
      m_NumWarningsUnknownChar = 0;
      m_NumWarningsNbspChar = 0;

      m_TextReader?.Dispose();
      m_ImprovedStream?.Dispose();

      base.Close();
    }

    /// <summary>
    ///   Reads a stream of bytes from the specified column offset into the buffer as an array,
    ///   starting at the given buffer offset.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferOffset">
    ///   The index for <paramref name="buffer" /> to start the read operation.
    /// </param>
    /// <param name="length">The number of bytes to read.</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <returns>The actual number of bytes read.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///   The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount" />.
    /// </exception>
    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length) =>
      throw new NotImplementedException();

    /// <summary>
    ///   Returns an <see cref="IDataReader" /> for the specified column ordinal.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <returns>An <see cref="IDataReader" />.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///   The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount" />.
    /// </exception>
    public IDataReader GetData(int i) => throw new NotImplementedException();

    /// <summary>
    ///   Gets the data type information for the specified field.
    /// </summary>
    /// <param name="i">The index of the field to find.</param>
    /// <returns>The .NET type name of the column</returns>
    public string GetDataTypeName(int i) => GetFieldType(i).Name;

    /// <summary>
    ///   Return the value of the specified field.
    /// </summary>
    /// <param name="columnNumber">The index of the field to find.</param>
    /// <returns>The object will contain the field value upon return.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///   The index passed was outside the range of 0 through <see cref="IDataRecord.FieldCount" />.
    /// </exception>
    public override object GetValue(int columnNumber)
    {
      if (IsDBNull(columnNumber))
        return DBNull.Value;

      return GetTypedValueFromString(CurrentRowColumnText[columnNumber], GetTimeValue(columnNumber),
        GetColumn(columnNumber));
    }

    /// <summary>
    ///   Open the file Reader; Start processing the Headers and determine the maximum column size
    /// </summary>
    public void Open()
    {
      m_HasQualifier |= m_CsvFile.FileFormat.FieldQualifierChar != '\0';

      if (m_CsvFile.FileFormat.FieldQualifier.Length > 1 &&
          m_CsvFile.FileFormat.FieldQualifier.WrittenPunctuationToChar() == '\0')
        HandleWarning(-1,
          $"Only the first character of '{m_CsvFile.FileFormat.FieldQualifier}' is be used for quoting.");
      if (m_CsvFile.FileFormat.FieldDelimiter.Length > 1 &&
          m_CsvFile.FileFormat.FieldDelimiter.WrittenPunctuationToChar() == '\0')
        HandleWarning(-1,
          $"Only the first character of '{m_CsvFile.FileFormat.FieldDelimiter}' is used as delimiter.");

      BeforeOpen();
    Retry:
      try
      {
        var fn = FileSystemUtils.SplitPath(m_CsvFile.FullPath);
        HandleShowProgress($"Opening text file {fn.FileName}");

        ResetPositionToStartOrOpen();

        m_HeaderRow = ReadNextRow(false, false);
        if (m_HeaderRow == null || m_HeaderRow.GetLength(0) == 0)
        {
          InitColumn(0);
        }
        else
        {
          // Get the column count
          InitColumn(ParseFieldCount(m_HeaderRow));

          // Get the column names
          ParseColumnName(m_HeaderRow);
        }

        if (m_CsvFile.TryToSolveMoreColumns && m_CsvFile.FileFormat.FieldDelimiterChar != '\0')
          m_RealignColumns = new ReAlignColumns(FieldCount);

        FinishOpen();

        ResetPositionToFirstDataRow();
      }
      catch (Exception ex)
      {
        if (ShouldRetry(ex))
          goto Retry;

        Close();
        var appEx = new FileReaderException(
          "Error opening text file for reading.\nPlease make sure the file does exist, is of the right type and is not locked by another process.",
          ex);
        HandleError(-1, appEx.ExceptionMessages());
        HandleReadFinished();
        throw appEx;
      }
      finally
      {
        HandleShowProgress("");
      }
    }

    /// <summary>
    ///   Advances the <see cref="IDataReader" /> to the next record.
    /// </summary>
    /// <returns>true if there are more rows; otherwise, false.</returns>
    public override bool Read()
    {
      if (!CancellationToken.IsCancellationRequested)
      {
        var couldRead = GetNextRecord();
        InfoDisplay(couldRead);

        if (couldRead && !IsClosed)
          return true;
      }

      HandleReadFinished();
      return false;
    }

    /// <summary>
    ///   Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    ///   unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
      if (m_DisposedValue) return;
      // Dispose-time code should also set references of all owned objects to null, after disposing
      // them. This will allow the referenced objects to be garbage collected even if not all
      // references to the "parent" are released. It may be a significant memory consumption win if
      // the referenced objects are large, such as big arrays, collections, etc.
      if (disposing)
      {
        Close();
        if (m_TextReader != null)
        {
          m_TextReader.Dispose();
          m_TextReader = null;
        }

        base.Dispose(true);
      }

      m_DisposedValue = true;
    }

    #region Parsing

    /// <summary>
    ///   Resets the position and buffer to the header in case the file has a header
    /// </summary>
    public void ResetPositionToFirstDataRow()
    {
      ResetPositionToStartOrOpen();
      if (m_CsvFile.HasFieldHeader)
        // Read the header row, this could be more than one line
        ReadNextRow(false, false);
    }

    /// <summary>
    ///   Gets the relative position.
    /// </summary>
    /// <returns>A value between 0 and MaxValue</returns>
    protected override int GetRelativePosition()
    {
      // if we know how many records to read, use that
      if (m_CsvFile.RecordLimit > 0)
        return (int)((double)RecordNumber / m_CsvFile.RecordLimit * cMaxValue);

      return (int)(m_ImprovedStream.Percentage * cMaxValue);
    }

    private bool AllEmptyAndCountConsecutiveEmptyRows(IReadOnlyList<string> columns)
    {
      if (columns != null)
      {
        var rowLength = columns.Count;
        for (var col = 0; col < rowLength && col < FieldCount; col++)
          if (!string.IsNullOrEmpty(columns[col]))
          {
            m_ConsecutiveEmptyRows = 0;
            return false;
          }
      }

      m_ConsecutiveEmptyRows++;
      EndOfFile |= m_ConsecutiveEmptyRows >= m_CsvFile.ConsecutiveEmptyRows;
      return true;
    }

    private char EatNextCRLF(char character)
    {
      EndLineNumber++;
      if (EndOfFile) return '\0';
      var nextChar = NextChar();
      if ((character != c_Cr || nextChar != c_Lf) && (character != c_Lf || nextChar != c_Cr)) return '\0';
      // New line sequence is either CRLF or LFCR, disregard the character
      m_BufferPos++;
      // Very special a LF CR is counted as two lines.
      if (character == c_Lf && nextChar == c_Cr)
        EndLineNumber++;
      return nextChar;
    }

    /// <summary>
    ///   Gets a row of the CSV file
    /// </summary>
    /// <returns>
    ///   <c>NULL</c> if the row can not be read, array of string values representing the columns of
    ///   the row
    /// </returns>
    private bool GetNextRecord()
    {
      try
      {
      Restart:
        CurrentRowColumnText = ReadNextRow(true, true);

        if (!AllEmptyAndCountConsecutiveEmptyRows(CurrentRowColumnText))
        {
          // Regular row with data
          RecordNumber++;
        }
        else
        {
          if (EndOfFile)
            return false;

          // an empty line
          if (m_CsvFile.SkipEmptyLines)
            goto Restart;
          RecordNumber++;
        }

        var hasWarningCombinedWarning = false;
      Restart2:
        var rowLength = CurrentRowColumnText.Length;
        if (rowLength == FieldCount)
        {
          // Check if we have row that matches the header row
          if (m_HeaderRow != null && m_CsvFile.HasFieldHeader && !hasWarningCombinedWarning)
          {
            var isRepeatedHeader = true;
            for (var col = 0; col < FieldCount; col++)
              if (!m_HeaderRow[col].Equals(CurrentRowColumnText[col], StringComparison.OrdinalIgnoreCase))
              {
                isRepeatedHeader = false;
                break;
              }

            if (isRepeatedHeader && m_CsvFile.SkipDuplicateHeader)
            {
              RecordNumber--;
              goto Restart;
            }

            if (m_RealignColumns != null && !isRepeatedHeader)
              m_RealignColumns.AddRow(CurrentRowColumnText);
          }
        }
        // If less columns are present
        else if (rowLength < FieldCount)
        {
          // if we still have only one column and we should have a number of columns assume this was
          // nonsense like a report footer
          if (rowLength == 1 && EndOfFile && CurrentRowColumnText[0].Length < 10)
          {
            RecordNumber--;
            HandleWarning(-1, $"Last line {StartLineNumber}{cLessColumns}. Assumed to be a EOF marker and ignored.");
            return false;
          }

          if (!m_CsvFile.AllowRowCombining)
          {
            HandleWarning(-1, $"Line {StartLineNumber}{cLessColumns} ({rowLength}/{FieldCount}).");
          }
          else
          {
            var oldPos = m_BufferPos;
            var startLine = StartLineNumber;
            // get the next row
            var nextLine = ReadNextRow(true, true);
            StartLineNumber = startLine;

            // allow up to two extra columns they can be combined later
            if (nextLine != null && nextLine.Length > 0 && nextLine.Length + rowLength < FieldCount + 4)
            {
              var combined = new List<string>(CurrentRowColumnText);

              // the first column belongs to the last column of the previous ignore
              // NumWarningsLinefeed otherwise as this is important information
              m_NumWarningsLinefeed++;
              HandleWarning(rowLength - 1,
                $"Added first column from line {EndLineNumber}, assuming a linefeed has split the rows into an additional line.");
              combined[rowLength - 1] += ' ' + nextLine[0];

              for (var col = 1; col < nextLine.Length; col++)
                combined.Add(nextLine[col]);

              if (!hasWarningCombinedWarning)
              {
                HandleWarning(-1,
                  $"Line {StartLineNumber}-{EndLineNumber - 1}{cLessColumns}. Lines have been combined.");
                hasWarningCombinedWarning = true;
              }

              CurrentRowColumnText = combined.ToArray();
              goto Restart2;
            }

            if (m_BufferPos < oldPos)
            // we have an issue we went into the next Buffer there is no way back.
            {
              HandleError(-1,
                $"Line {StartLineNumber}{cLessColumns}\nAttempting to combined lines some line have been read that is now lost, please turn off Row Combination");
            }
            else
            {
              // return to the old position so reading the next row did not matter
              if (!hasWarningCombinedWarning)
                HandleWarning(-1, $"Line {StartLineNumber}{cLessColumns} ({rowLength}/{FieldCount}).");
              m_BufferPos = oldPos;
            }
          }
        }

        // If more columns are present
        if (rowLength > FieldCount && (m_CsvFile.WarnEmptyTailingColumns || m_RealignColumns != null))
        {
          // check if the additional columns have contents
          var hasContent = false;
          for (var extraCol = FieldCount; extraCol < rowLength; extraCol++)
          {
            if (string.IsNullOrEmpty(CurrentRowColumnText[extraCol]))
              continue;
            hasContent = true;
            break;
          }

          if (!hasContent) return true;
          if (m_RealignColumns != null)
          {
            HandleWarning(-1,
              $"Line {StartLineNumber}{cMoreColumns}. Trying to realign columns.");
            // determine which column could have caused the issue it could be any column, try to establish
            CurrentRowColumnText = m_RealignColumns.RealignColumn(CurrentRowColumnText, HandleWarning);
          }
          else
          {
            HandleWarning(-1,
              $"Line {StartLineNumber}{cMoreColumns} ({rowLength}/{FieldCount}). The data in extra columns is not read.");
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        HandleError(-1, ex.Message);
        EndOfFile = true;
        return false;
      }
    }

    /// <summary>
    ///   Indicates whether the specified Unicode character is categorized as white space.
    /// </summary>
    /// <param name="c">A Unicode character.</param>
    /// <returns><c>true</c> if the character is a whitespace</returns>
    private bool IsWhiteSpace(char c)
    {
      // Handle cases where the delimiter is a whitespace (e.g. tab)
      if (c == m_CsvFile.FileFormat.FieldDelimiterChar)
        return false;

      // See char.IsLatin1(char c) in Reflector
      if (c <= '\x00ff')
        return c == ' ' || c == '\t';

      return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
    }

    /// <summary>
    ///   Gets the next char from the buffer, but stay at the current position
    /// </summary>
    /// <returns>The next char</returns>
    private char NextChar()
    {
      Contract.Requires(m_Buffer != null);
      if (m_BufferPos >= m_BufferFilled)
      {
        ReadIntoBuffer();
        m_BufferPos = 0;
      }

      // If of file its does not matter what to return simply return something
      return EndOfFile ? c_Lf : m_Buffer[m_BufferPos];
    }

    /// <summary>
    ///   Gets the number of fields.
    /// </summary>
    private int ParseFieldCount(IList<string> headerRow)
    {
      Contract.Ensures(Contract.Result<int>() >= 0);
      if (headerRow == null || headerRow.Count == 0 || string.IsNullOrEmpty(headerRow[0]))
        return 0;

      var fields = headerRow.Count;

      // The last column is empty but we expect a header column, assume if a trailing separator
      if (fields <= 1)
        return fields;

      // check if the next lines do have data in the last column
      for (var additional = 0; !EndOfFile && additional < 10; additional++)
      {
        var nextLine = ReadNextRow(false, false);
        // if we have less columns than in the header exit the loop
        if (nextLine.GetLength(0) < fields)
          break;

        // special case of missing linefeed, the line is twice as long minus 1 because of the
        // combined column in the middle
        if (nextLine.Length == fields * 2 - 1)
          continue;

        while (nextLine.GetLength(0) > fields)
        {
          HandleWarning(fields, $"No header for last {nextLine.GetLength(0) - fields} column(s)".AddWarningId());
          fields++;
        }

        // if we have data in the column assume the header was missing
        if (!string.IsNullOrEmpty(nextLine[fields - 1]))
          return fields;
      }

      if (string.IsNullOrEmpty(headerRow[headerRow.Count - 1]))
      {
        HandleWarning(fields,
          "The last column does not have a column name and seems to be empty, this column will be ignored."
            .AddWarningId());
        return fields - 1;
      }

      return fields;
    }

    /// <summary>
    ///   Fills the buffer with data from the reader.
    /// </summary>
    /// <returns><c>true</c> if data was successfully read; otherwise, <c>false</c>.</returns>
    private void ReadIntoBuffer()
    {
      Contract.Requires(m_TextReader != null);
      Contract.Requires(m_Buffer != null);

      if (EndOfFile)
        return;
      m_BufferFilled = m_TextReader.Read(m_Buffer, 0, c_BufferSize);
      EndOfFile |= m_BufferFilled == 0;
      // Handle double decoding
      if (!m_CsvFile.DoubleDecode || m_BufferFilled <= 0)
        return;
      var result = Encoding.UTF8.GetChars(Encoding.GetEncoding(28591).GetBytes(m_Buffer, 0, m_BufferFilled));
      // The twice decode text is smaller
      m_BufferFilled = result.GetLength(0);
      result.CopyTo(m_Buffer, 0);
    }

    /// <summary>
    ///   Gets the next column in the buffer.
    /// </summary>
    /// <param name="columnNo">The column number for warnings</param>
    /// <param name="storeWarnings">If <c>true</c> warnings will be added</param>
    /// <returns>The next column null after the last column</returns>
    /// <remarks>
    ///   If NULL is returned we are at the end of the file, an empty column is read as empty string
    /// </remarks>
    private string ReadNextColumn(int columnNo, bool storeWarnings)
    {
      if (EndOfFile)
        return null;
      if (m_EndOfLine)
      {
        // previous item was last in line, start new line
        m_EndOfLine = false;
        return null;
      }

      var stringBuilder = new StringBuilder(5);
      var hadDelimiterInValue = false;
      var hadUnknownChar = false;
      var hadNbsp = false;
      var quoted = false;
      var preData = true;
      var postData = false;

      while (!EndOfFile)
      {
        // Increase position
        var character = NextChar();
        m_BufferPos++;

        var escaped = character == m_CsvFile.FileFormat.EscapeCharacterChar && !postData;
        // Handle escaped characters
        if (escaped)
        {
          var nextChar = NextChar();
          if (!EndOfFile)
          {
            m_BufferPos++;
            switch (m_CsvFile.FileFormat.EscapeCharacterChar)
            {
              // Handle \ Notation of common not visible characters
              case '\\' when nextChar == 'n':
                character = '\n';
                break;

              case '\\' when nextChar == 'r':
                character = '\r';
                break;

              case '\\' when nextChar == 't':
                character = '\t';
                break;

              case '\\' when nextChar == 'b':
                character = '\b';
                break;
              // in case a linefeed actually follows ignore the EscapeCharacterChar but handle the
              // regular processing
              case '\\' when nextChar == 'a':
                character = '\a';
                break;

              default:
                character = nextChar;
                break;
            }
          }
        }

        // in case we have a single LF
        if (!postData && m_CsvFile.TreatLFAsSpace && character == c_Lf && quoted)
        {
          var singleLF = true;
          if (!EndOfFile)
          {
            var nextChar = NextChar();
            if (nextChar == c_Cr)
              singleLF = false;
          }

          if (singleLF)
          {
            character = ' ';
            EndLineNumber++;
            if (m_CsvFile.WarnLineFeed)
              WarnLinefeed(columnNo);
          }
        }

        switch (character)
        {
          case c_Nbsp:
            if (!postData)
            {
              hadNbsp = true;
              if (m_CsvFile.TreatNBSPAsSpace)
                character = ' ';
            }

            break;

          case c_UnknownChar:
            if (!postData)
            {
              hadUnknownChar = true;
              if (m_CsvFile.TreatUnknowCharaterAsSpace)
                character = ' ';
            }

            break;

          case c_Cr:
          case c_Lf:
            var nextChar = EatNextCRLF(character);
            if (character == c_Cr && nextChar == c_Lf || character == c_Lf && nextChar == c_Cr)
              if (quoted && !postData)
              {
                stringBuilder.Append(character);
                stringBuilder.Append(nextChar);
                continue;
              }

            break;
        }

        // Finished with reading the column by Delimiter or EOF
        if (character == m_CsvFile.FileFormat.FieldDelimiterChar && !escaped && (postData || !quoted) || EndOfFile)
          break;

        // Finished with reading the column by Linefeed
        if ((character == c_Cr || character == c_Lf) && (preData || postData || !quoted))
        {
          m_EndOfLine = true;
          break;
        }

        // Only check the characters if not past end of data
        if (postData)
          continue;

        if (preData)
        {
          // whitespace preceding data
          if (IsWhiteSpace(character))
          {
            // Store the white spaces if we do any kind of trimming
            if (m_CsvFile.TrimmingOption == TrimmingOption.None)
              // Values will be trimmed later but we need to find out, if the filed is quoted first
              stringBuilder.Append(character);
            continue;
          }

          // data is starting
          preData = false;
          // Can not be escaped here
          if (m_HasQualifier && character == m_CsvFile.FileFormat.FieldQualifierChar && !escaped)
          {
            if (m_CsvFile.TrimmingOption != TrimmingOption.None)
              stringBuilder.Length = 0;
            // quoted data is starting
            quoted = true;
          }
          else
          {
            stringBuilder.Append(character);
          }

          continue;
        }

        if (m_HasQualifier && character == m_CsvFile.FileFormat.FieldQualifierChar && quoted && !escaped)
        {
          var peekNextChar = NextChar();
          // a "" should be regarded as " if the text is quoted
          if (m_CsvFile.FileFormat.DuplicateQuotingToEscape && peekNextChar == m_CsvFile.FileFormat.FieldQualifierChar)
          {
            // double quotes within quoted string means add a quote
            stringBuilder.Append(m_CsvFile.FileFormat.FieldQualifierChar);
            m_BufferPos++;
            //TODO: decide if we should have this its hard to explain but might make sense
            // special handling for "" that is not only representing a " but also closes the text
            peekNextChar = NextChar();
            if (m_CsvFile.FileFormat.AlternateQuoting && (peekNextChar == m_CsvFile.FileFormat.FieldDelimiterChar ||
                                                          peekNextChar == c_Cr ||
                                                          peekNextChar == c_Lf)) postData = true;
            continue;
          }

          // a single " should be regarded as closing when its followed by the delimiter
          if (m_CsvFile.FileFormat.AlternateQuoting && (peekNextChar == m_CsvFile.FileFormat.FieldDelimiterChar ||
                                                        peekNextChar == c_Cr || peekNextChar == c_Lf))
          {
            postData = true;
            continue;
          }

          // a single " should be regarded as closing if we do not have alternate quoting
          if (!m_CsvFile.FileFormat.AlternateQuoting)
          {
            postData = true;
            continue;
          }
        }

        hadDelimiterInValue |= character == m_CsvFile.FileFormat.FieldDelimiterChar;
        // all cases covered, character must be data
        stringBuilder.Append(character);
      }

      var returnValue = HandleTrimAndNBSP(stringBuilder.ToString(), quoted);

      if (!storeWarnings)
        return returnValue;
      // if (m_HasQualifier && quoted && m_StructuredWriterFile.WarnQuotesInQuotes &&
      // returnValue.IndexOf(m_StructuredWriterFile.FileFormat.FieldQualifierChar) != -1) WarnQuoteInQuotes(columnNo);
      if (m_HasQualifier && m_CsvFile.WarnQuotes && returnValue.IndexOf(m_CsvFile.FileFormat.FieldQualifierChar) > 0)
        WarnQuotes(columnNo);
      if (m_CsvFile.WarnDelimiterInValue && hadDelimiterInValue)
        WarnDelimiter(columnNo);

      if (m_CsvFile.WarnUnknowCharater)
        if (hadUnknownChar)
        {
          WarnUnknownChar(columnNo, false);
        }
        else
        {
          var numberQuestionMark = 0;
          var lastPos = -1;
          var length = returnValue.Length;
          for (var pos = length - 1; pos >= 0; pos--)
          {
            if (returnValue[pos] != '?')
              continue;
            numberQuestionMark++;
            // If we have at least two and there are two consecutive or more than 3+ in 12
            // characters, or 4+ in 16 characters
            if (numberQuestionMark > 2 && (lastPos == pos + 1 || numberQuestionMark > length / 4))
            {
              WarnUnknownChar(columnNo, true);
              break;
            }

            lastPos = pos;
          }
        }

      if (m_CsvFile.WarnNBSP && hadNbsp)
        WarnNbsp(columnNo);
      if (m_CsvFile.WarnLineFeed && (returnValue.IndexOf('\r') != -1 || returnValue.IndexOf('\n') != -1))
        WarnLinefeed(columnNo);

      return returnValue;
    }

    /// <summary>
    ///   Reads the row of the CSV file
    /// </summary>
    /// <param name="regularDataRow">
    ///   Set to <c>true</c> if its not the header row and the maximum size should be determined.
    /// </param>
    /// <param name="storeWarnings">Set to <c>true</c> if the warnings should be issued.</param>
    /// <returns>
    ///   <c>NULL</c> if the row can not be read, array of string values representing the columns of
    ///   the row
    /// </returns>
    private string[] ReadNextRow(bool regularDataRow, bool storeWarnings)
    {
    Restart:
      // Store the starting Line Number
      StartLineNumber = EndLineNumber;

      // If already at end of file, return null
      if (EndOfFile || m_TextReader == null)
        return null;

      var item = ReadNextColumn(0, storeWarnings);
      // An empty line does not have any data
      if (string.IsNullOrEmpty(item) && m_EndOfLine)
      {
        m_EndOfLine = false;
        if (m_CsvFile.SkipEmptyLines || !regularDataRow)
          // go to the next line
          goto Restart;

        // Return it as array of empty columns
        return new string[FieldCount];
      }

      // Skip commented lines
      if (m_CsvFile.FileFormat.CommentLine.Length > 0 &&
          !string.IsNullOrEmpty(item) &&
          item.StartsWith(m_CsvFile.FileFormat.CommentLine, StringComparison.Ordinal)
      ) // A commented line does start with the comment
      {
        if (m_EndOfLine)
          m_EndOfLine = false;
        else
          // it might happen that the comment line contains a Delimiter
          ReadToEOL();
        goto Restart;
      }

      var col = 0;
      var columns = new List<string>(FieldCount);

      while (item != null)
      {
        // If a column is quoted and does contain the delimiter and linefeed, issue a warning, we
        // might have an opening delimiter with a missing closing delimiter
        if (storeWarnings &&
            EndLineNumber > StartLineNumber + 4 &&
            item.Length > 1024 &&
            item.IndexOf(m_CsvFile.FileFormat.FieldDelimiterChar) != -1)
          HandleWarning(col,
            $"Column has {EndLineNumber - StartLineNumber + 1} lines and has a length of {item.Length} characters"
              .AddWarningId());

        if (item.Length == 0)
        {
          item = null;
        }
        else
        {
          if (StringUtils.ShouldBeTreatedAsNull(item, m_CsvFile.TreatTextAsNull))
          {
            item = null;
          }
          else
          {
            item = item.ReplaceCaseInsensitive(m_CsvFile.FileFormat.NewLinePlaceholder, Environment.NewLine)
              .ReplaceCaseInsensitive(m_CsvFile.FileFormat.DelimiterPlaceholder,
                m_CsvFile.FileFormat.FieldDelimiterChar)
              .ReplaceCaseInsensitive(m_CsvFile.FileFormat.QuotePlaceholder, m_CsvFile.FileFormat.FieldQualifierChar);

            if (regularDataRow && col < FieldCount)
              item = HandleTextAndSetSize(item, col, false);
          }
        }

        columns.Add(item);

        col++;
        item = ReadNextColumn(col, storeWarnings);
      }

      return columns.ToArray();
    }

    /// <summary>
    ///   Reads from the buffer until the line has ended
    /// </summary>
    private void ReadToEOL()
    {
      while (!EndOfFile)
      {
        var character = NextChar();
        m_BufferPos++;
        if (character != c_Cr && character != c_Lf)
          continue;
        EatNextCRLF(character);
        return;
      }
    }

    /// <summary>
    ///   Resets the position and buffer to the first line, excluding headers, use
    ///   ResetPositionToStart if you want to go to first data line
    /// </summary>
    private void ResetPositionToStartOrOpen()
    {
      if (m_ImprovedStream == null)
        m_ImprovedStream = ApplicationSetting.OpenReadS(m_CsvFile);

      if (m_BufferPos != 0 || RecordNumber != 0 || m_BufferFilled == 0)
      {
        m_ImprovedStream.ResetToStart(delegate (Stream str)
        {
          // in case we can not seek need to reopen the stream reader
          if (!str.CanSeek || m_TextReader == null)
          {
            m_TextReader?.Dispose();
            m_TextReader = new StreamReader(str, m_CsvFile.GetEncoding(), m_CsvFile.ByteOrderMark);
          }
          else
          {
            m_TextReader.BaseStream.Seek(0, SeekOrigin.Begin);
            // only discard the buffer
            m_TextReader.DiscardBufferedData();

            // we reset the position a BOM is showing again - TODO why this is happening, this
            // should actually not be needed
            if (m_CsvFile.ByteOrderMark && (m_CsvFile.CodePageId == 12001 && m_TextReader.Peek() > 65000 || //  UTF32Be
                                            m_CsvFile.CodePageId == 12000 && m_TextReader.Peek() > 65000 || //  UTF32Le
                                            m_CsvFile.CodePageId == 65000 && m_TextReader.Peek() > 65000 || // UTF7
                                            m_CsvFile.CodePageId == 65001 && m_TextReader.Peek() == 65279) // UTF8
            )
              m_TextReader.Read();

            if (m_CsvFile.ByteOrderMark && (m_CsvFile.CodePageId == 1201 || // UTF16Be
                                            m_CsvFile.CodePageId == 1200) // UTF16Le
                                        && m_TextReader.Peek() == 65279)
            {
              m_TextReader.Read();
              m_TextReader.Read();
            }
          }
        });

        m_CsvFile.CurrentEncoding = m_TextReader.CurrentEncoding;
        m_BufferFilled = 0;
      }

      m_BufferPos = 0;

      // End Line should be at 1, later on as the line is read the start line s set to this value
      StartLineNumber = 1;
      EndLineNumber = 1;
      RecordNumber = 0;
      m_EndOfLine = false;
      EndOfFile = false;

      // Skip the given number of lines <= so we do skip the right number
      while (EndLineNumber <= m_CsvFile.SkipRows && !EndOfFile)
        ReadToEOL();
    }

    #endregion Parsing

    #region Warning

    /// <summary>
    ///   Add warnings for delimiter.
    /// </summary>
    /// <param name="column">The column.</param>
    private void WarnDelimiter(int column)
    {
      m_NumWarningsDelimiter++;
      if (m_CsvFile.NumWarnings >= 1 && m_NumWarningsDelimiter > m_CsvFile.NumWarnings)
        return;
      HandleWarning(column,
        $"Field delimiter '{FileFormat.GetDescription(m_CsvFile.FileFormat.FieldDelimiter)}' found in field"
          .AddWarningId());
    }

    /// <summary>
    ///   Add warnings for Linefeed.
    /// </summary>
    /// <param name="column">The column.</param>
    private void WarnLinefeed(int column)
    {
      m_NumWarningsLinefeed++;
      if (m_CsvFile.NumWarnings >= 1 && m_NumWarningsLinefeed > m_CsvFile.NumWarnings)
        return;
      HandleWarning(column, "Linefeed found in field".AddWarningId());
    }

    /// <summary>
    ///   Add warnings for NBSP.
    /// </summary>
    /// <param name="column">The column.</param>
    private void WarnNbsp(int column)
    {
      m_NumWarningsNbspChar++;
      if (m_CsvFile.NumWarnings >= 1 && m_NumWarningsNbspChar > m_CsvFile.NumWarnings)
        return;
      HandleWarning(column,
        m_CsvFile.TreatNBSPAsSpace
          ? "Character Non Breaking Space found, this character was treated as space".AddWarningId()
          : "Character Non Breaking Space found in field".AddWarningId());
    }

    /// <summary>
    ///   Add warnings for quotes.
    /// </summary>
    /// <param name="column">The column.</param>
    private void WarnQuotes(int column)
    {
      m_NumWarningsQuote++;
      if (m_CsvFile.NumWarnings >= 1 && m_NumWarningsQuote > m_CsvFile.NumWarnings)
        return;
      HandleWarning(column,
        $"Field qualifier '{FileFormat.GetDescription(m_CsvFile.FileFormat.FieldQualifier)}' found in field"
          .AddWarningId());
    }

    /// <summary>
    ///   Add warnings for unknown char.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="questionMark"></param>
    private void WarnUnknownChar(int column, bool questionMark)
    {
      // in case the column is ignored do not warn
      if (Column[column].Ignore)
        return;

      m_NumWarningsUnknownChar++;
      if (m_CsvFile.NumWarnings >= 1 && m_NumWarningsUnknownChar > m_CsvFile.NumWarnings)
        return;
      if (questionMark)
      {
        HandleWarning(column, "Unusual high occurrence of ? this indicates unmapped characters.".AddWarningId());
        return;
      }

      HandleWarning(column,
        m_CsvFile.TreatUnknowCharaterAsSpace
          ? "Unknown Character '�' found, this character was replaced with space".AddWarningId()
          : "Unknown Character '�' found in field".AddWarningId());
    }

    #endregion Warning
  }
}