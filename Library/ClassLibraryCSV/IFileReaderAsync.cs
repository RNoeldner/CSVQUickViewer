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
using System.Threading.Tasks;

namespace CsvTools
{
  /// <summary>
  ///   Interface for a File Reader.
  /// </summary>
  public interface IFileReaderAsync : IDataReaderAsync, IFileReader
  {
    /// <summary>
    ///   Opens the text file and begins to read the meta data, like columns
    /// </summary>
    /// <returns>Number of records in the file if known (use determineColumnSize), -1 otherwise</returns>
    Task OpenAsync();

    /// <summary>
    ///   Resets the position and buffer to the header in case the file has a header
    /// </summary>
    Task ResetPositionToFirstDataRowAsync();
  }
}