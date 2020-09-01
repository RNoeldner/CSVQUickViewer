﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CsvTools.Tests
{
  [TestClass]
  public class ControlsTests
  {
    [TestMethod]
    public async Task CsvTextDisplayShow()
    {
      var ctrl = new FormCsvTextDisplay();

      using (var frm = new TestForm())
      {
        frm.AddOneControl(ctrl);
        frm.Show();
        await ctrl.SetCsvFileAsync(UnitTestInitializeCsv.GetTestPath("BasicCSV.txt"), '"', '\t', '\0', 65001);

        frm.SafeInvoke(() => frm.Close());
      }
    }
  }
}