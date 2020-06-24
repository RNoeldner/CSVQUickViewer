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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CsvTools
{
  /// <summary>
  ///   Interface for a File Reader.
  /// </summary>
  public interface IFileReader : IDataReader
  {
    /// <summary>
    ///   Gets the end line number
    /// </summary>
    /// <value>The line number in which the record ended</value>
    long EndLineNumber { get; }

    /// <summary>
    ///   Determine if the data Reader is at the end of the file
    /// </summary>
    /// <returns>True if you can read; otherwise, false.</returns>
    bool EndOfFile { get; }

    /// <summary>
    ///   Gets the record number.
    /// </summary>
    /// <value>The record number.</value>
    long RecordNumber { get; }

    /// <summary>
    ///   Gets the start line number.
    /// </summary>
    /// <value>The line number in which the record started.</value>
    long StartLineNumber { get; }

    bool SupportsReset { get; }

    /// <summary>
    ///   Occurs before the initial open. Can be used to prepare the data like download it from a
    ///   remote location
    /// </summary>
    Func<Task> OnOpen { set; }

    [Obsolete("Use ReadAsync if possible")]
    // ReSharper disable once UnusedMemberInSuper.Global
    new bool Read();

    /// <summary>
    ///   Advances the data reader to the next result, when reading the results of batch SQL statements.
    /// </summary>
    /// <returns>
    ///   <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.
    /// </returns>
    [Obsolete("Not supported")]
    // ReSharper disable once UnusedMember.Global
    new bool NextResult();

    /// <summary>
    ///   Reads the next record of the current result set
    /// </summary>
    /// <returns>Awaitable bool, if true a record was read</returns>
    Task<bool> ReadAsync(CancellationToken token);

    /// <summary>
    ///   Event handler called if a warning or error occurred
    /// </summary>
    [UsedImplicitly]
    event EventHandler<WarningEventArgs> Warning;

    /// <summary>
    ///   Event to be raised once the reader is finished reading the file
    /// </summary>
    event EventHandler ReadFinished;

    /// <summary>
    ///   Event to be raised once the reader opened, the column information is now known
    /// </summary>
    event EventHandler<ICollection<IColumn>> OpenFinished;

    /// <summary>
    ///   Occurs when an open process failed, allowing the user to change the timeout or provide the
    ///   needed file etc.
    /// </summary>
    [UsedImplicitly]
    event EventHandler<RetryEventArgs> OnAskRetry;

    /// <summary>
    ///   Gets the column information for a given column number
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>A <see cref="Column" /> with all information on the column</returns>
    ImmutableColumn GetColumn(int column);

    /// <summary>
    ///   Opens the text file and begins to read the meta data, like columns
    /// </summary>
    /// <returns>Number of records in the file if known (use determineColumnSize), -1 otherwise</returns>
    Task OpenAsync(CancellationToken token);

    /// <summary>
    ///   Resets the position and buffer to the header in case the file has a header
    /// </summary>
    /// <param name="token"></param>
    Task ResetPositionToFirstDataRowAsync(CancellationToken token);
  }
}