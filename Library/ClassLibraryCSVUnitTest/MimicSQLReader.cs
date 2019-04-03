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
using System.Linq;

namespace CsvTools.Tests
{
  public class MimicSQLReader
  {
    private List<IFileSetting> m_ReadSetting = new List<IFileSetting>();

    public void AddSetting(IFileSetting setting)
    {
      if (setting == null || string.IsNullOrEmpty(setting.ID))
      {
        throw new ArgumentNullException(nameof(setting));
      }

      if (!m_ReadSetting.Any(x => x.ID.Equals(setting.ID, StringComparison.OrdinalIgnoreCase)))
        m_ReadSetting.Add(setting);
    }

    public List<IFileSetting> ReadSettings { get => m_ReadSetting; }

    public IDataReader ReadData(string settingName, IProcessDisplay processDisplay)
    {
      var setting = m_ReadSetting.FirstOrDefault(x => x.ID == settingName);
      if (setting == null)
        throw new ApplicationException($"{settingName} not found");
      var reader = setting.GetFileReader(processDisplay);
      reader.Open();
      return reader;
    }
  }
}