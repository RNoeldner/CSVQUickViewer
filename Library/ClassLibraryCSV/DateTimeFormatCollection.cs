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

using System.Collections.Generic;


namespace CsvTools
{
  public class DateTimeFormatCollection
  {
    private readonly Dictionary<string, DateTimeFormatInformation> m_DateLengthMinMax = new Dictionary<string, DateTimeFormatInformation>();

    public DateTimeFormatCollection(string file)
    {
      using (var reader = FileSystemUtils.GetStreamReaderForFileOrResource(file))
      {
        if (reader == null)
          return;

        while (!reader.EndOfStream)
        {
          var entry = reader.ReadLine();
          if (string.IsNullOrEmpty(entry) || entry[0] == '#')
            continue;
          Add(entry);
        }
      }
    }

    public IEnumerable<string> Keys => m_DateLengthMinMax.Keys;

    public IEnumerable<string> MatchingforLength(int length, bool checkNamedDates)
    {
      var result = new List<string>();
      foreach (var kvFormatInformation in m_DateLengthMinMax)
      {
        if ((checkNamedDates || !kvFormatInformation.Value.NamedDate) && length >= kvFormatInformation.Value.MinLength && length <= kvFormatInformation.Value.MaxLength)
          result.Add(kvFormatInformation.Key);
      }
      return result;
    }

    public void Replace(string[] customList)
    {
      m_DateLengthMinMax.Clear();
      foreach (var entry in customList)
        Add(entry);
    }

    public bool TryGetValue(string key, out DateTimeFormatInformation value) => m_DateLengthMinMax.TryGetValue(key, out value);

    private void Add(string entry)
    {
      if (string.IsNullOrWhiteSpace(entry))
        return;
      if (!m_DateLengthMinMax.ContainsKey(entry))
      {
        m_DateLengthMinMax.Add(entry, new DateTimeFormatInformation(entry));
      }
    }
  }
}