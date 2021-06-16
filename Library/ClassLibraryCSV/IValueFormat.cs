// /*
// * Copyright (C) 2014 Raphael Nöldner : http://csvquickviewer.com *
// * This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser Public
// * License as published by the Free Software Foundation, either version 3 of the License, or (at
// your option) any later version. *
// * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty
// * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser Public License for
// more details. *
// * You should have received a copy of the GNU Lesser Public License along with this program.
// * If not, see http://www.gnu.org/licenses/ . *
// */

using JetBrains.Annotations;

namespace CsvTools
{
  public interface IValueFormat
  {
    DataType DataType { [NotNull] get; }

    string DateFormat { [NotNull] get; }

    /// <summary>
    ///   The value will return the resulted Seperator, passing in "Colon" will return ":"
    /// </summary>
    string DateSeparator { [NotNull] get; }

    /// <summary>
    ///   The value will return the resulted Seperator, passing in "Dot" will return "."
    /// </summary>
    string TimeSeparator { [NotNull] get; }

    string NumberFormat { [NotNull] get; }

    /// <summary>
    ///   The value will return the resulted Seperator, passing in "Dot" will return "."
    /// </summary>
    string DecimalSeparator { get; }

    /// <summary>
    ///   The value will return the resulted Seperator, passing in "Dot" will return "."
    /// </summary>
    string GroupSeparator { get; }

    string True { [NotNull] get; }
    string False { [NotNull] get; }
    string DisplayNullAs { [NotNull] get; }
  }
}