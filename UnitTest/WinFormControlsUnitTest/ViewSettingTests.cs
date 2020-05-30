﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsvTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvTools.Tests
{
  [TestClass()]
  public class ViewSettingTests
  {

    [TestMethod()]
    public void StoreViewSettingTest1()
    {
      using (var dgv = new DataGridView())
      {
        var dgvc = new DataGridViewColumn(new DataGridViewTextBoxCell())
        {
          DataPropertyName = "test",
          ValueType = typeof(string)
        };
        dgv.Columns.Add(dgvc);
        var columnFilters = new List<ToolStripDataGridViewColumnFilter>
        {
          new ToolStripDataGridViewColumnFilter(dgvc)
        };
        var text = ViewSetting.StoreViewSetting(dgv.Columns, columnFilters, null, SortOrder.Ascending);
        Assert.IsTrue(!string.IsNullOrEmpty(text));
      }
    }

    [TestMethod()]
    public void StoreViewSettingTest2()
    {
      using (var dgv = new DataGridView())
      {
        var dgvc = new DataGridViewColumn(new DataGridViewTextBoxCell())
        {
          DataPropertyName = "test", 
          ValueType = typeof(string)
        };
        dgv.Columns.Add(dgvc);
        var dgvc2 = new DataGridViewColumn(new DataGridViewTextBoxCell())
        {
          DataPropertyName = "test2",
          ValueType = typeof(string)
        };
        dgv.Columns.Add(dgvc2);
        var columnFilters = new List<ToolStripDataGridViewColumnFilter>
        {
          new ToolStripDataGridViewColumnFilter(dgvc),
          new ToolStripDataGridViewColumnFilter(dgvc2),
        };
      
        columnFilters[0].ColumnFilterLogic.ValueClusterCollection.ValueClusters.Add(new ValueCluster("display", "cond",string.Empty));
        columnFilters[0].ColumnFilterLogic.Active = true;

        columnFilters[1].ColumnFilterLogic.Operator = "=";
        columnFilters[1].ColumnFilterLogic.ValueText = "Halloween";
        columnFilters[1].ColumnFilterLogic.Active = true;

        var text = ViewSetting.StoreViewSetting(dgv.Columns, columnFilters, dgvc, SortOrder.Descending);
        ViewSetting.ReStoreViewSetting(text, dgv.Columns, new List<ToolStripDataGridViewColumnFilter>(), i => columnFilters[i], null);
      }
    }
  }
}