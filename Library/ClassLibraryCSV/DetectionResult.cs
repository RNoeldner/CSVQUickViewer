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

using JetBrains.Annotations;
using System.Collections.Generic;

namespace CsvTools
{
  public class DelimitedFileDetectionResult
  {
    public readonly string FileName;
    public bool ByteOrderMark;
    public int CodePageId;
    public bool HasFieldHeader;
    public bool IsJson;
    public bool NoDelimitedFile;
    public int SkipRows;
    [NotNull] public string IdentifierInContainer;

    public bool QualifyAlways;
    [NotNull] public string CommentLine;
    [NotNull] public string EscapeCharacter;
    [NotNull] public string FieldDelimiter;
    [NotNull] public string FieldQualifier;
    public RecordDelimiterType NewLine;

    public DelimitedFileDetectionResult(string fileName, int skipRows = 0, int codePageId = -1, bool byteOrderMark = false, bool qualifyAlways = false,
                                        string identifierInContainer = "", string commentLine = "#", string escapeCharacter = "\\", string fieldDelimiter = "",
                                        string fieldQualifier = "", bool hasFieldHeader = true, bool isJson = false, bool noDelimitedFile = false, RecordDelimiterType recordDelimiterType = RecordDelimiterType.None)
    {
      FileName = fileName ?? throw new System.ArgumentNullException(nameof(fileName));
      IdentifierInContainer = identifierInContainer?? string.Empty;
      SkipRows = skipRows<1 ? 0 : skipRows;
      CodePageId = codePageId<1 ? -1 : codePageId;
      ByteOrderMark= byteOrderMark;
      CommentLine = commentLine;
      EscapeCharacter = (escapeCharacter ?? string.Empty).WrittenPunctuation();
      FieldDelimiter = (fieldDelimiter ?? string.Empty).WrittenPunctuation();
      FieldQualifier = (fieldQualifier ?? string.Empty).WrittenPunctuation();
      HasFieldHeader = hasFieldHeader;
      IsJson = isJson;
      NoDelimitedFile = noDelimitedFile;
      QualifyAlways= qualifyAlways;
      NewLine= recordDelimiterType;
    }

    public ICsvFile CsvFile(IEnumerable<Column> columns)
    {
      var ret = new CsvFile(FileName)
      {
        FileFormat = new FileFormat()
        {
          QualifyAlways= QualifyAlways,
          CommentLine = CommentLine,
          EscapeCharacter= EscapeCharacter,
          FieldDelimiter = FieldDelimiter,
          FieldQualifier = FieldQualifier,
          NewLine =  NewLine
        },
        ByteOrderMark = ByteOrderMark,
        CodePageId = CodePageId,
        HasFieldHeader = HasFieldHeader,
        JsonFormat= IsJson,
        NoDelimitedFile = NoDelimitedFile,
        IdentifierInContainer = IdentifierInContainer,
        SkipRows = SkipRows
      };
      if (columns!= null)
        foreach (var col in columns)
          ret.ColumnCollection.Add(col);
      return ret;
    }
  }
}