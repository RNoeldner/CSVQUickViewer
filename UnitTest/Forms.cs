﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CsvTools.Tests
{
  [TestClass]
  public class Forms
  {
    private readonly string m_ApplicationDirectory = FileSystemUtils.ExecutableDirectoryName() + @"\TestFiles";

    [TestMethod]
    public void FormLimitSize()
    {
      using (var frm = new FrmLimitSize())
      {
        frm.RecordLimit = 1000;
        frm.Show();
        frm.RecordLimit = 20;
        System.Threading.Thread.Sleep(200);
      }
    }

    [TestMethod]
    public void FormEditSettings()
    {
      var csvFile = new CsvFile
      {
        FileName = @"\TestFiles\BasicCSV.txt"
      };

      using (var frm = new FormEditSettings(csvFile))
      {
        frm.Show();
        System.Threading.Thread.Sleep(200);
        csvFile.CodePageId = 650001;
      }
    }

    [TestMethod]
    public void FormMain()
    {
      using (var frm = new FormMain(Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")))
      {
        frm.Show();
        System.Threading.Thread.Sleep(200);
      }
    }
  }
}