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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Data;

namespace CsvTools.Tests
{
  [TestClass]
  public class DetailControlTests
  {
    private readonly Random random = new Random(Guid.NewGuid().GetHashCode());

    [TestMethod]
    [Timeout(3000)]
    public async System.Threading.Tasks.Task DetailControlTestAsync()
    {
      using (var dt = new DataTable())
      {
        dt.Columns.Add(new DataColumn { ColumnName = "ID", DataType = typeof(int) });
        dt.Columns.Add(new DataColumn { ColumnName = "Text", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) });
        dt.Columns.Add(new DataColumn { ColumnName = "Bool", DataType = typeof(bool) });
        for (var line = 1; line < 5000; line++)
        {
          var row = dt.NewRow();
          row[0] = line;
          row[1] = $"This is text {line / 2}";
          row[2] = new DateTime(2001, 6, 6).AddHours(line * 3);
          row[3] = line % 3 == 0;
          if (random.Next(1, 10) == 5)
            row.SetColumnError(random.Next(0, 3), "Error");
          if (random.Next(1, 50) == 5)
            row.RowError = "Row Error";
          dt.Rows.Add(row);
        }

        using (var dc = new DetailControl())
        {
          dc.HTMLStyle = UnitTestInitializeWin.HTMLStyle;
          dc.Show();
          dc.DataTable = dt;
          
          await dc.RefreshDisplayAsync(FilterType.All, UnitTestInitializeCsv.Token);
          dc.OnlyShowErrors = true ;
          dc.MoveMenu();
        }
      }
    }

    [TestMethod]
    [Timeout(3000)]
    public async System.Threading.Tasks.Task SortTestAsync()
    {
      using (var dt = new DataTable())
      {
        dt.Columns.Add(new DataColumn { ColumnName = "ID", DataType = typeof(int) });
        dt.Columns.Add(new DataColumn { ColumnName = "Text", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) });
        dt.Columns.Add(new DataColumn { ColumnName = "Bool", DataType = typeof(bool) });
        for (var line = 1; line < 5000; line++)
        {
          var row = dt.NewRow();
          row[0] = random.Next(1, 5000);
          row[1] = $"This is text {line / 2}";
          row[2] = new DateTime(2001, 6, 6).AddHours(line * 3);
          row[3] = line % 3 == 0;
          dt.Rows.Add(row);
        }

        using (var dc = new DetailControl())
        {
          dc.HTMLStyle = UnitTestInitializeWin.HTMLStyle;
          dc.Show();
          dc.DataTable = dt;
          await dc.RefreshDisplayAsync(FilterType.All, UnitTestInitializeCsv.Token);
          dc.Sort("ID", ListSortDirection.Ascending);
        }
      }
    }
  }
}