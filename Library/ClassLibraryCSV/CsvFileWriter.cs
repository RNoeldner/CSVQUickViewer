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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CsvTools
{
  /// <summary>
  ///   A Class to write CSV Files
  /// </summary>
  public class CsvFileWriter : BaseFileWriter, IFileWriter
  {
    private readonly ICsvFile m_CsvFile;
    private readonly string m_FieldQualifier;
    private readonly string m_FieldQualifierEscaped;
    private readonly char[] m_QualifyCharArray;

    /// <summary>
    ///   Initializes a new instance of the <see cref="CsvFileWriter" /> class.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="cancellationToken">A cancellation token to stop writing the file</param>
    public CsvFileWriter(ICsvFile file, CancellationToken cancellationToken)
      : base(file, cancellationToken)
    {
      Contract.Requires(file != null);
      m_CsvFile = file;

      // SOme caching of values need for quoting
      m_QualifyCharArray = new[] { (char)0x0a, (char)0x0d, m_CsvFile.FileFormat.FieldDelimiterChar };
      m_FieldQualifier = m_CsvFile.FileFormat.FieldQualifierChar.ToString();
      if (!string.IsNullOrEmpty(file.FileFormat.EscapeCharacter))
        m_FieldQualifierEscaped = file.FileFormat.EscapeCharacterChar + m_FieldQualifier;
      else
        m_FieldQualifierEscaped = m_FieldQualifier + m_FieldQualifier;
    }

    /// <summary>
    ///   Stores that data in the given stream.
    /// </summary>
    /// <param name="reader">The data reader.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="readerFileSetting">The file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   Number of rows written
    /// </returns>
    /// <exception cref="ApplicationException">No columns defined to be written.</exception>
    protected void DataReader2Stream(IDataReader reader, TextWriter writer, IFileSetting readerFileSetting,
      CancellationToken cancellationToken)
    {
      Contract.Requires(reader != null);
      Contract.Requires(writer != null);

      var skipEmptyLines = readerFileSetting == null || readerFileSetting.SkipEmptyLines;

      var columnInfos = GetColumnInformation(reader.GetSchemaTable(), readerFileSetting);
      var enumerable = columnInfos as ColumnInfo[] ?? columnInfos.ToArray();
      if (enumerable.IsEmpty())
        throw new ApplicationException("No columns defined to be written.");
      var recordEnd = m_CsvFile.FileFormat.NewLine.Replace("CR", "\r").Replace("LF", "\n").Replace(" ", "")
        .Replace("\t", "");

      HandleWriteStart();

      var numEmptyRows = 0;
      var numColumns = enumerable.Count();
      var
        sb = new StringBuilder(1024); // Assume a capacity of 1024 characters to start , data is flushed every 512 chars
      var hasFieldDelimiter = !m_CsvFile.FileFormat.IsFixedLength;
      if (!string.IsNullOrEmpty(m_CsvFile.Header))
      {
        sb.Append(ReplacePlaceHolder(StringUtils.HandleCRLFCombinations(m_CsvFile.Header, recordEnd)));
        if (!m_CsvFile.Header.EndsWith(recordEnd, StringComparison.Ordinal))
          sb.Append(recordEnd);
      }

      if (m_CsvFile.HasFieldHeader)
      {
        sb.Append(GetHeaderRow(enumerable));
        sb.Append(recordEnd);
      }

      while (reader.Read() && !cancellationToken.IsCancellationRequested)
      {
        NextRecord();
        if (sb.Length > 32768)
        {
          writer.Write(sb.ToString());
          sb.Length = 0;
        }

        var emptyColumns = 0;

        foreach (var columnInfo in enumerable)
        {
          var col = reader.GetValue(columnInfo.ColumnOridinalReader);
          if (col == DBNull.Value)
            emptyColumns++;

          sb.Append(TextEncodeField(m_CsvFile.FileFormat, col, columnInfo, false, reader, QualifyText));

          if (hasFieldDelimiter)
            sb.Append(m_CsvFile.FileFormat.FieldDelimiterChar);
        }

        if (hasFieldDelimiter)
          sb.Length--;
        if (emptyColumns == numColumns)
        {
          // Remove the delimiters again
          if (skipEmptyLines && hasFieldDelimiter)
            sb.Length -= numColumns;
          numEmptyRows++;
          // Stop if we do encounter the given number of consecutive empty rows
          if (readerFileSetting != null && readerFileSetting.ConsecutiveEmptyRows > 0 &&
              numEmptyRows > readerFileSetting.ConsecutiveEmptyRows)
            break;
          continue;
        }

        numEmptyRows = 0;
        sb.Append(recordEnd);
      }

      if (!string.IsNullOrEmpty(m_CsvFile.Footer))
        sb.Append(ReplacePlaceHolder(StringUtils.HandleCRLFCombinations(m_CsvFile.Footer, recordEnd)));
      else if (sb.Length > 0)
        sb.Length -= recordEnd.Length;

      writer.Write(sb.ToString());
    }

    /// <summary>
    ///   Writes the specified file reading from the given reader
    /// </summary>
    /// <param name="reader">A Data Reader with the data</param>
    /// <param name="fileSetting">The source setting or the data that could be different than the setting for is writer</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   Number of records written
    /// </returns>
    protected override void Write(IDataReader reader, IFileSetting fileSetting, CancellationToken cancellationToken)
    {
      Contract.Assume(!string.IsNullOrEmpty(m_CsvFile.FullPath));
      var dest = m_CsvFile.FullPath;
      try
      {
        if (ApplicationSetting.WritePreProcessing != null)
        {
          HandleProgress("Preparing processing");
          dest = ApplicationSetting.WritePreProcessing.Invoke(m_CsvFile);
        }
        FileSystemUtils.FileDelete(dest);

        using (var writer = new StreamWriter(dest, false,
          EncodingHelper.GetEncoding(m_CsvFile.CodePageId, m_CsvFile.ByteOrderMark), 8192))
        {
          DataReader2Stream(reader, writer, fileSetting, cancellationToken);
        }

        // PGP Encryption
        if (ApplicationSetting.WritePostProcessing == null) return;
        HandleProgress("Post processing");
        ApplicationSetting.WritePostProcessing.Invoke(m_CsvFile, ProcessDisplay, dest);
      }
      catch (Exception exc)
      {
        ErrorMessage = $"Could not write file '{m_CsvFile.FileName}'.\r\n{exc.ExceptionMessages()}";
        if (m_CsvFile.InOverview)
          throw;
      }
      finally
      {
        if (!m_CsvFile.FullPath.Equals(dest, StringComparison.OrdinalIgnoreCase))
          FileSystemUtils.FileDelete(dest);
        HandleWriteFinished();
      }
    }

    private string GetHeaderRow(IEnumerable<ColumnInfo> columnInfos)
    {
      Contract.Requires(columnInfos != null);

      var sb = new StringBuilder();
      foreach (var columnInfo in columnInfos)
      {
        Contract.Assume(columnInfo != null);
        sb.Append(TextEncodeField(m_CsvFile.FileFormat, columnInfo.Header, columnInfo, true, null, QualifyText));
        if (!m_CsvFile.FileFormat.IsFixedLength)
          sb.Append(m_CsvFile.FileFormat.FieldDelimiterChar);
      }

      if (!m_CsvFile.FileFormat.IsFixedLength)
        sb.Length--;
      return sb.ToString();
    }

    private string QualifyText(string displayAs, ColumnInfo columnInfo, FileFormat fileFormat)
    {
      var qualifyThis = fileFormat.QualifyAlways;
      if (!qualifyThis)
        if (fileFormat.QualifyOnlyIfNeeded)
          // Qualify the text if the delimiter is present
          qualifyThis = displayAs.Length > 0 && (displayAs.IndexOfAny(m_QualifyCharArray) > -1 ||
                                                 displayAs[0].Equals(fileFormat.FieldQualifierChar));
        else
          // quality any text or something containing a Qualify Char
          qualifyThis = columnInfo.DataType == DataType.String || columnInfo.DataType == DataType.TextToHtml ||
                        displayAs.IndexOfAny(m_QualifyCharArray) > -1;
      if (qualifyThis)
        return m_FieldQualifier + displayAs.Replace(m_FieldQualifier, m_FieldQualifierEscaped) + m_FieldQualifier;

      return displayAs;
    }
  }
}