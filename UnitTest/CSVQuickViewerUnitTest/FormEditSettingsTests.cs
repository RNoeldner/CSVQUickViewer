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
  public class FormEditSettingsTests
  {
    
    [TestMethod]
    public void FormEditSettings()
    {
      using (var frm = new FormEditSettings(new ViewSettings()))
        UnitTestWinFormHelper.ShowFormAndClose(frm);
    }

    [TestMethod()]
    public void FormEditSettingsTest1()
    {
      using (var frm = new FormEditSettings())
        UnitTestWinFormHelper.ShowFormAndClose(frm);
    }
  }
}