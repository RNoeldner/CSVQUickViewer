﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CsvTools
{
  public class DateTimeFormatCollection
  {
    private readonly Dictionary<string, DateTimeFormatInformation> m_DateLengthMinMax = new Dictionary<string, DateTimeFormatInformation>();

    public DateTimeFormatCollection()
    {
    }

    public DateTimeFormatCollection(string file)
    {
      var fileName = (FileSystemUtils.ExecutableDirectoryName() + "\\" + file).LongPathPrefix();
      if (!System.IO.File.Exists(fileName)) return;
      try
      {
        using (var reader = new System.IO.StreamReader(fileName, true))
        {
          while (!reader.EndOfStream)
          {
            var entry = reader.ReadLine();
            if (string.IsNullOrEmpty(entry) || entry[0] == '#')
              continue;
            Add(entry);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error reading  " + fileName);
        Debug.WriteLine(ex.Message);
      }
    }

    public IEnumerable<string> Keys => m_DateLengthMinMax.Keys;

    public IEnumerable<string> MatchingforLength(int length, bool checkNamedDates) => m_DateLengthMinMax.Where(x => checkNamedDates && x.Value.NamedDate && length >= x.Value.MinLength && length <= x.Value.MaxLength).Select(x => x.Key);

    public void Replace(string[] cusomList)
    {
      m_DateLengthMinMax.Clear();
      foreach (var entry in cusomList)
        Add(entry);
    }

    public bool TryGetValue(string key, out DateTimeFormatInformation value)
    {
      return m_DateLengthMinMax.TryGetValue(key, out value);
    }

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