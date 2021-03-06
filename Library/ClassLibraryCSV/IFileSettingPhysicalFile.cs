/*
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


namespace CsvTools
{
  public interface IFileSettingPhysicalFile : IFileSetting
  {
    /// <summary>
    ///   Gets or sets the name of the file, this value could be a relative path
    /// </summary>
    /// <value>The name of the file.</value>
    string FileName { get; set; }

    string RootFolder { get; set; }

    /// <summary>
    ///   The Size of the file in Byte
    /// </summary>
    long FileSize { get; set; }

    /// <summary>
    ///   Gets the full path of the Filename
    /// </summary>
    /// <value>The full path of the file <see cref="FileName" /> /&gt;</value>
    string FullPath { get; }

    /// <summary>
    ///   Gets the root folder of the Tool Setting
    /// </summary>
    /// <value>The root folder.</value>
    string Recipient { get; set; }

    /// <summary>
    ///   Passphrase for Decryption
    /// </summary>
    string Passphrase { get; set; }

    bool KeepUnencrypted { get; set; }

    /// <summary>
    ///   Force the refresh of full path information, a filename with placeholders might need to
    ///   check again if there is a new file
    /// </summary>
    void ResetFullPath();

    /// <summary>
    ///   Path to the file on sFTP Server
    /// </summary>
    string RemoteFileName { get; set; }

    /// <summary>
    ///   May store information on columns to show, filtering and sorting
    /// </summary>
    string ColumnFile { get; set; }

    string IdentifierInContainer { get; set; }

    /// <summary>
    ///   In case of creating a file, should the time of the latest source be used?
    ///   Default: <c>false</c> - Use the current datetime for the file, otherwise use the time of
    ///   the latest source
    /// </summary>
    bool SetLatestSourceTimeForWrite { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether tho throw an error if the remote file could not be
    ///   found .
    /// </summary>
    /// <value><c>true</c> if throw an error if not exists; otherwise, <c>false</c>.</value>
    bool ThrowErrorIfNotExists { get; set; }
  }
}