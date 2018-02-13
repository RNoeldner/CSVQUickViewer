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

namespace CsvTools
{
  public class DummyToolSetting : IToolSetting
  {
    private readonly List<IFileSetting> m_Input = new List<IFileSetting>();
    private readonly List<IFileSetting> m_Output = new List<IFileSetting>();

    public virtual ICollection<IFileSetting> Input => m_Input;

    public virtual ICollection<IFileSetting> Output => m_Output;

    public virtual string RootFolder => ".";

    public virtual ICache<string, ValidationResult> ValidationResultCache => null;

    public virtual PGPKeyStorage PGPInformation { get; } = new PGPKeyStorage();

    public virtual TimeZoneInfo DestinationTimeZone => TimeZoneInfo.Local;
  }
}