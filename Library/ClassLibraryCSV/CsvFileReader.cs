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
using System.Text;

namespace CsvTools
{
  /// <summary>
  ///   A data reader for CSV files
  /// </summary>
  public sealed class CsvFileReader : CsvFileReaderBase, IFileReader
  {
    public CsvFileReader(ICsvFile fileSetting, string timeZone, IProcessDisplay processDisplay)
      : base(fileSetting, timeZone, processDisplay)
    {
    }

    /// <summary>
    ///   Open the file Reader; Start processing the Headers and determine the maximum column size
    /// </summary>
    public void Open()
    {
      OpenStart();
    Retry:
      try
      {
        var fn = FileSystemUtils.SplitPath(m_CsvFile.FullPath);
        HandleShowProgress($"Opening text file {fn.FileName}");

        m_ImprovedStream?.Dispose();
        m_ImprovedStream = FunctionalDI.OpenRead(m_CsvFile);
        m_TextReader?.Dispose();
        m_TextReader = new ImprovedTextReader(m_ImprovedStream, m_CsvFile.CodePageId, m_CsvFile.SkipRows);
        m_CsvFile.CurrentEncoding = m_TextReader.CurrentEncoding;

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
    public bool Read()
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

      return (int)((m_ImprovedStream?.Percentage ?? 0) * cMaxValue);
    }

    private char EatNextCRLF(char character)
    {
      EndLineNumber++;
      if (EndOfFile) return '\0';
      var nextChar = Peek();
      if ((character != c_Cr || nextChar != c_Lf) && (character != c_Lf || nextChar != c_Cr)) return '\0';
      // New line sequence is either CRLF or LFCR, disregard the character
      m_TextReader.MoveNext();
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
            var oldPos = m_TextReader.BufferPos;
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

            // we have an issue we went into the next Buffer there is no way back.
            if (m_TextReader.BufferPos < oldPos)
            {
              HandleError(-1,
                 $"Line {StartLineNumber}{cLessColumns}\nAttempting to combined lines some line have been read that is now lost, please turn off Row Combination");
            }
            else
            {
              // return to the old position so reading the next row did not matter
              if (!hasWarningCombinedWarning)
                HandleWarning(-1, $"Line {StartLineNumber}{cLessColumns} ({rowLength}/{FieldCount}).");
              m_TextReader.BufferPos = oldPos;
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
    ///   Gets the next char from the buffer, but stay at the current position
    /// </summary>
    /// <returns>The next char</returns>
    private char Peek()
    {
      var res = m_TextReader.Peek();
      if (res == -1)
      {
        EndOfFile = true;
        // return a lf to determine the end of quoting easily
        return c_Lf;
      }
      return (char)res;
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
        var character = Peek();
        m_TextReader.MoveNext();

        var escaped = character == m_CsvFile.FileFormat.EscapeCharacterChar && !postData;
        // Handle escaped characters
        if (escaped)
        {
          var nextChar = Peek();
          if (!EndOfFile)
          {
            m_TextReader.MoveNext();
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
            var nextChar = Peek();
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
          var peekNextChar = Peek();
          // a "" should be regarded as " if the text is quoted
          if (m_CsvFile.FileFormat.DuplicateQuotingToEscape && peekNextChar == m_CsvFile.FileFormat.FieldQualifierChar)
          {
            // double quotes within quoted string means add a quote
            stringBuilder.Append(m_CsvFile.FileFormat.FieldQualifierChar);
            m_TextReader.MoveNext();
            //TODO: decide if we should have this its hard to explain but might make sense
            // special handling for "" that is not only representing a " but also closes the text
            peekNextChar = Peek();
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

      return ReadNextColumnEnd(columnNo, stringBuilder, storeWarnings, quoted, hadUnknownChar, hadDelimiterInValue, hadNbsp);
    }

    /// <summary>
    ///   Reads the record of the CSV file, this can span over multiple lines
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
        var character = Peek();
        m_TextReader.MoveNext();
        if (character != c_Cr && character != c_Lf)
          continue;
        EatNextCRLF(character);
        return;
      }
    }
  }
}