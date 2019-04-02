﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace CsvTools.Tests
{
  [TestClass]
  public class CsvDataReaderPGPFilesTest
  {
    [TestMethod]
    public void ReadGZip()
    {
      var setting = new CsvFile
      {
        HasFieldHeader = true,
        AlternateQuoting = true
      };
      setting.FileName = "BasicCSV.txt.gz";
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ExamDate",
        DataType = DataType.DateTime,
        DateFormat = @"dd/MM/yyyy"
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ID",
        DataType = DataType.Integer
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "IsNativeLang",
        DataType = DataType.Boolean
      });
      using (var processDisplay = new DummyProcessDisplay()) using (var test = new CsvFileReader(setting, processDisplay))
      {
        test.Open();                
        int row = 0;
        while (test.Read())
          row++;
        Assert.AreEqual(row, test.RecordNumber);
        Assert.AreEqual(7, row);
      }
    }

    [TestMethod]
    public void ReadPGP()

    {
      var setting = new CsvFile
      {
        HasFieldHeader = true,
        AlternateQuoting = true
      };
      PGPKeyStorageTestHelper.SetApplicationSetting();
      setting.GetEncryptedPassphraseFunction = setting.DummyEncryptedPassphaseFunction;
      setting.FileName = "BasicCSV.pgp";
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ExamDate",
        DataType = DataType.DateTime,
        DateFormat = @"dd/MM/yyyy"
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ID",
        DataType = DataType.Integer
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "IsNativeLang",
        DataType = DataType.Boolean
      });
      using (var processDisplay = new DummyProcessDisplay()) using (var test = new CsvFileReader(setting, processDisplay))
      {
        test.Open();        
        int row = 0;
        while (test.Read())
          row++;
        Assert.AreEqual(row, test.RecordNumber);
        Assert.AreEqual(7, row);
      }
    }

    [TestMethod]
    public void ReadGPG()
    {
      var setting = new CsvFile
      {
        HasFieldHeader = true,
        AlternateQuoting = true
      };
      PGPKeyStorageTestHelper.SetApplicationSetting();
      setting.GetEncryptedPassphraseFunction = setting.DummyEncryptedPassphaseFunction;
      setting.FileName = "BasicCSV.pgp";
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ExamDate",
        DataType = DataType.DateTime,
        DateFormat = @"dd/MM/yyyy"
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "ID",
        DataType = DataType.Integer
      });
      setting.ColumnCollection.AddIfNew(new Column
      {
        Name = "IsNativeLang",
        DataType = DataType.Boolean
      });
      using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, processDisplay))
      {
        test.Open();        
        int row = 0;
        while (test.Read())
          row++;
        Assert.AreEqual(row, test.RecordNumber);
        Assert.AreEqual(7, row);
      }
    }
  }
}