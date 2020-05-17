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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvTools.Tests
{
  [TestClass]
  public class CsvDataReaderUnitTest
  {
    private readonly CsvFile m_ValidSetting = new CsvFile
    {
      FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
      FileFormat = {FieldDelimiter = ",", CommentLine = "#"}
    };

    [TestInitialize]
    public void Init()
    {
      m_ValidSetting.ColumnCollection.AddIfNew(new Column("Score", DataType.Integer));
      m_ValidSetting.ColumnCollection.AddIfNew(new Column("Proficiency", DataType.Numeric));
      m_ValidSetting.ColumnCollection.AddIfNew(new Column("IsNativeLang", DataType.Boolean));
      var cf = m_ValidSetting.ColumnCollection.AddIfNew(new Column("ExamDate", DataType.DateTime));
      cf.ValueFormat.DateFormat = @"dd/MM/yyyy";
    }

    [TestMethod]
    public async Task AllFormatsPipeReaderAsync()
    {
      var setting =
        new CsvFile(UnitTestInitialize.GetTestPath("AllFormatsPipe.txt"))
        {
          HasFieldHeader = true,
          FileFormat = {FieldDelimiter = "|", FieldQualifier = "\""},
          SkipEmptyLines = false
        };

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.AreEqual(10, test.FieldCount);
        await test.ReadAsync();
        Assert.AreEqual(2, test.StartLineNumber);
        Assert.AreEqual(3, test.EndLineNumber);
        Assert.AreEqual(1, test.RecordNumber);
        Assert.AreEqual("-22477", test.GetString(1));
        await test.ReadAsync();
        Assert.AreEqual(3, test.StartLineNumber);
        Assert.AreEqual(4, test.EndLineNumber);
        Assert.AreEqual(2, test.RecordNumber);
        Assert.AreEqual("22435", test.GetString(1));
        for (var line = 3; line < 25; line++)
          await test.ReadAsync();
        Assert.AreEqual("-21928", test.GetString(1));
        Assert.IsTrue(test.GetString(4).EndsWith("twpapulfffy"));
        Assert.AreEqual(25, test.StartLineNumber);
        Assert.AreEqual(27, test.EndLineNumber);
        Assert.AreEqual(24, test.RecordNumber);

        for (var line = 25; line < 47; line++)
          await test.ReadAsync();
        Assert.AreEqual(49, test.EndLineNumber);
        Assert.AreEqual("4390", test.GetString(1));

        Assert.AreEqual(46, test.RecordNumber);
      }
    }

    [TestMethod]
    public async Task IssueReaderAsync()
    {
      var basIssues = new CsvFile
      {
        TreatLFAsSpace = true,
        TryToSolveMoreColumns = true,
        AllowRowCombining = true,
        FileName = UnitTestInitialize.GetTestPath("BadIssues.csv"),
        FileFormat = {FieldDelimiter = "TAB", FieldQualifier = string.Empty}
      };
      basIssues.ColumnCollection.AddIfNew(new Column("effectiveDate", "yyyy/MM/dd", "-"));
      basIssues.ColumnCollection.AddIfNew(new Column("timestamp", "yyyy/MM/ddTHH:mm:ss", "-"));

      basIssues.ColumnCollection.AddIfNew(new Column("version", DataType.Integer));
      basIssues.ColumnCollection.AddIfNew(new Column("retrainingRequired", DataType.Boolean));

      basIssues.ColumnCollection.AddIfNew(new Column("classroomTraining", DataType.Boolean));
      basIssues.ColumnCollection.AddIfNew(new Column("webLink", DataType.TextToHtml));

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(basIssues, TimeZoneInfo.Local.Id, processDisplay))
      {
        var warningList = new RowErrorCollection(test);
        await test.OpenAsync();
        warningList.HandleIgnoredColumns(test);
        // need 22 columns
        Assert.AreEqual(22, test.GetSchemaTable().Rows.Count());

        // This should work
        await test.ReadAsync();
        Assert.AreEqual(0, warningList.CountRows);

        Assert.AreEqual("Eagle_sop020517", test.GetValue(0));
        Assert.AreEqual("de-DE", test.GetValue(2));

        // There are more columns
        await test.ReadAsync();
        Assert.AreEqual(1, warningList.CountRows);
        Assert.AreEqual("Eagle_SRD-0137699", test.GetValue(0));
        Assert.AreEqual("de-DE", test.GetValue(2));
        Assert.AreEqual(3, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("Eagle_600.364", test.GetValue(0));
        Assert.AreEqual(4, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("Eagle_spt029698", test.GetValue(0));
        Assert.AreEqual(5, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("Eagle_SRD-0137698", test.GetValue(0));
        Assert.AreEqual(2, warningList.CountRows);
        Assert.AreEqual(6, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("Eagle_SRD-0138074", test.GetValue(0));
        Assert.AreEqual(7, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("Eagle_SRD-0125563", test.GetValue(0));
        Assert.AreEqual(8, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("doc_1004040002982", test.GetValue(0));
        Assert.AreEqual(3, warningList.CountRows);
        Assert.AreEqual(9, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("doc_1004040002913", test.GetValue(0));
        Assert.AreEqual(10, test.StartLineNumber, "StartLineNumber");
        Assert.AreEqual(5, warningList.CountRows);

        await test.ReadAsync();
        Assert.AreEqual("doc_1003001000427", test.GetValue(0));
        Assert.AreEqual(12, test.StartLineNumber);

        await test.ReadAsync();
        Assert.AreEqual("doc_1008017000611", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1004040000268", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1008011000554", test.GetValue(0));
        await test.ReadAsync();
        Assert.AreEqual("doc_1003001000936", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1200000124471", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1200000134529", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1004040003504", test.GetValue(0));

        await test.ReadAsync();
        Assert.AreEqual("doc_1200000016068", test.GetValue(0));

        await test.ReadAsync();
      }
    }

    [TestMethod]
    public async Task TestGetDataTypeNameAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.AreEqual("String", test.GetDataTypeName(0));
      }
    }

    [TestMethod]
    public async Task TestWarningsRecordNoMappingAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        var dataTable = new DataTable
        {
          TableName = "DataTable",
          Locale = CultureInfo.InvariantCulture
        };

        dataTable.Columns.Add(test.GetName(0), test.GetFieldType(0));

        var recordNumberColumn = dataTable.Columns.Add(BaseFileReader.cRecordNumberFieldName, typeof(int));
        recordNumberColumn.AllowDBNull = true;

        var lineNumberColumn = dataTable.Columns.Add(BaseFileReader.cEndLineNumberFieldName, typeof(int));
        lineNumberColumn.AllowDBNull = true;

        _ = dataTable.NewRow();
        await test.ReadAsync();
        _ = new Dictionary<int, string>
        {
          {-1, "Test1"},
          {0, "Test2"}
        };

        //test.AssignNumbersAndWarnings(dataRow, null, recordNumberColumn, lineNumberColumn, null, warningsList);
        //Assert.AreEqual("Test1", dataRow.RowError);
        //Assert.AreEqual("Test2", dataRow.GetColumnError(0));
      }
    }

    //[TestMethod]
    //public async Task GetPart()
    //{
    //  var partToEnd = new Column
    //  {
    //    DataType = DataType.TextPart,
    //    PartSplitter = '-',
    //    Part = 2,
    //    PartToEnd = true
    //  };
    //  var justPart = new Column
    //  {
    //    DataType = DataType.TextPart,
    //    PartSplitter = '-',
    //    Part = 2,
    //    PartToEnd = false
    //  };

    // using (var test = new CsvFileReader(m_ValidSetting, null)) { var inputValue =
    // "17-Hello-World"; var value = test.GetPart(inputValue, partToEnd);
    // Assert.AreEqual("Hello-World", value);

    //    var value2 = test.GetPart(inputValue, justPart);
    //    Assert.AreEqual("Hello", value2);
    //  }
    //}

    [TestMethod]
    public void GetInteger32And64()
    {
      var column = new Column
      {
        ValueFormat = {DataType = DataType.Integer, GroupSeparator = ",", DecimalSeparator = ","}
      };

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        var inputValue = "17";

        var value32 = test.GetInt32Null(inputValue, column);
        Assert.IsTrue(value32.HasValue);
        Assert.AreEqual(17, value32.Value);

        var value64 = test.GetInt64Null(inputValue, column);
        Assert.IsTrue(value64.HasValue);
        Assert.AreEqual(17, value64.Value);

        value32 = test.GetInt32Null(null, column);
        Assert.IsFalse(value32.HasValue);

        value64 = test.GetInt64Null(null, column);
        Assert.IsFalse(value64.HasValue);
      }
    }

    [TestMethod]
    public async Task TestBatchFinishedNotifcationAsync()
    {
      var finished = false;
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        test.ReadFinished += delegate { finished = true; };
        await test.OpenAsync();

        while (await test.ReadAsync())
        {
        }
      }

      Assert.IsTrue(finished, "ReadFinished");
    }

    [TestMethod]
    public async Task TestReadFinishedNotificationAsync()
    {
      var finished = false;
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        test.ReadFinished += delegate { finished = true; };
        await test.OpenAsync();
        while (await test.ReadAsync())
        {
        }
      }

      Assert.IsTrue(finished);
    }

    [TestMethod]
    public void ColumnFormat()
    {
      var target = new CsvFile();
      m_ValidSetting.CopyTo(target);

      Assert.IsNotNull(target.ColumnCollection.Get("Score"));
      var cf = target.ColumnCollection.Get("Score");
      Assert.AreEqual(cf.Name, "Score");

      // Remove the one filed
      target.ColumnCollection.Remove(target.ColumnCollection.Get("Score"));
      Assert.IsNull(target.ColumnCollection.Get("Score"));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task GetDateTimeTestAsync()
    {
      var csvFile = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("TestFile.txt"),
        CodePageId = 65001,
        FileFormat = {FieldDelimiter = "tab"}
      };

      csvFile.ColumnCollection.AddIfNew(new Column("Title", DataType.DateTime));

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(csvFile, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        await test.ReadAsync();
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    public void CsvDataReaderImportFileEmptyNullNotExisting()
    {
      var setting = new CsvFile();

      try
      {
        setting.FileName = string.Empty;
        using (new CsvFileReader(setting, TimeZoneInfo.Local.Id, null))
        {
        }

        Assert.Fail("Exception expected");
      }
      catch (ArgumentException)
      {
      }
      catch (FileReaderException)
      {
      }
      catch (Exception ex)
      {
        Assert.Fail($"Wrong Exception Type {ex.GetType()}, Empty Filename");
      }

      try
      {
        setting.FileName = @"b;dslkfg;sldfkgjs;ldfkgj;sldfkg.sdfgsfd";
        using (new CsvFileReader(setting, TimeZoneInfo.Local.Id, null))
        {
        }

        Assert.Fail("Exception expected");
      }
      catch (ArgumentException)
      {
      }
      catch (FileNotFoundException)
      {
      }
      catch (Exception ex)
      {
        Assert.Fail($"Wrong Exception Type {ex.GetType()}, Invalid Filename");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderRecordNumberEmptyLinesAsync()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSVEmptyLine.txt"),
        HasFieldHeader = true
      };

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        var row = 0;
        while (await test.ReadAsync())
          row++;
        Assert.AreEqual(row, test.RecordNumber);
        Assert.AreEqual(2, row);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderRecordNumberEmptyLinesSkipEmptyLinesAsync()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSVEmptyLine.txt"),
        HasFieldHeader = true,
        SkipEmptyLines = false,
        ConsecutiveEmptyRows = 3
      };
      /*
       * ID,LangCode,ExamDate,Score,Proficiency,IsNativeLang
1
2 00001,German,20/01/2010,276,0.94,Y
3 ,,,,,
4 ,,,,,
5 00001,English,22/01/2012,190,,N
6 ,,,,,
7 ,,,,,
8 ,,,,,
9 ,,,,,
10
*/

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        var row = 0;
        while (await test.ReadAsync())
          row++;
        Assert.AreEqual(row, test.RecordNumber, "Compare with RecordNumber");
        Assert.AreEqual(7, row, "Read");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderPropertiesAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();

        Assert.AreEqual(0, test.Depth, "Depth");
        Assert.AreEqual(6, test.FieldCount, "FieldCount");
        Assert.AreEqual(0U, test.RecordNumber, "RecordNumber");
        Assert.AreEqual(-1, test.RecordsAffected, "RecordsAffected");

        Assert.IsFalse(test.EndOfFile, "EndOfFile");
        Assert.IsFalse(test.IsClosed, "IsClosed");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetNameAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.AreEqual("ID", test.GetName(0));
        Assert.AreEqual("LangCodeID", test.GetName(1));
        Assert.AreEqual("ExamDate", test.GetName(2));
        Assert.AreEqual("Score", test.GetName(3));
        Assert.AreEqual("Proficiency", test.GetName(4));
        Assert.AreEqual("IsNativeLang", test.GetName(5));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetOrdinalAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.AreEqual(0, test.GetOrdinal("ID"));
        Assert.AreEqual(1, test.GetOrdinal("LangCodeID"));
        Assert.AreEqual(2, test.GetOrdinal("ExamDate"));
        Assert.AreEqual(3, test.GetOrdinal("Score"));
        Assert.AreEqual(4, test.GetOrdinal("Proficiency"));
        Assert.AreEqual(5, test.GetOrdinal("IsNativeLang"));
        Assert.AreEqual(-1, test.GetOrdinal("Not Existing"));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderUseIndexerAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual("1", test["ID"]);
        Assert.AreEqual("German", test[1]);
        Assert.AreEqual(new DateTime(2010, 01, 20), test["ExamDate"]);
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(DBNull.Value, test["Proficiency"]);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetValueNullAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(DBNull.Value, test.GetValue(4));
      }
    }

#if COMInterface
    [TestMethod]
    public async Task CsvDataReader_GetValueADONull()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(DBNull.Value, test.GetValueADO(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task CsvDataReader_GetValueADONoRead()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.AreEqual(DBNull.Value, test.GetValueADO(0));
      }
    }

    [TestMethod]
    public async Task CsvDataReader_GetValueADO()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual("1", test.GetValueADO(0));
        Assert.AreEqual("German", test.GetValueADO(1));
        Assert.AreEqual(new DateTime(2010, 01, 20), test.GetValueADO(2));
      }
    }

#endif

    [TestMethod]
    public async Task CsvDataReaderGetBooleanAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(test.GetBoolean(5));
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsFalse(test.GetBoolean(5));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetBooleanErrorAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetBoolean(1);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetDateTimeAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        // 20/01/2010
        Assert.AreEqual(new DateTime(2010, 01, 20), test.GetDateTime(2));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDateTimeErrorAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetInt32Async()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(276, test.GetInt32(3));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetInt32ErrorAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetInt32(1);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetDecimalAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(0.94m, test.GetDecimal(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDecimalErrorAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetDecimal(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetInt32NullAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        test.GetInt32(4);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public async Task CsvDataReaderGetBytesAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        test.GetBytes(0, 0, null, 0, 0);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public async Task CsvDataReaderGetDataAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        test.GetData(0);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetFloatAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(Convert.ToSingle(0.94), test.GetFloat(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetFloatErrorAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetFloat(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetGuidAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetGuid(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDateTimeNullAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        test.GetDateTime(2);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDateTimeWrongTypeAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDecimalFormatException()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        test.GetDecimal(4);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetByte()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetByte(0));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetByteFrormat()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetByte(1));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetDouble()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetDouble(0));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public async Task CsvDataReaderGetDoubleFrormat()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetDouble(1));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetInt16()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetInt16(0));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderInitWarnings()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldQualifier = "XX";
      setting.FileFormat.FieldDelimiter = ",,";

      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        var warningList = new RowErrorCollection(test);
        await test.OpenAsync();
        warningList.HandleIgnoredColumns(test);

        Assert.IsTrue(warningList.Display.Contains("Only the first character of 'XX' is be used for quoting."));
        Assert.IsTrue(warningList.Display.Contains("Only the first character of ',,' is used as delimiter."));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldDelimiterCr()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldDelimiter = "\r"}
      };
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldQualifierCr()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldQualifier = "Carriage return"}
      };
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldQualifierLF()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldQualifier = "Line feed"}
      };
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderGuessCodePage()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = true,
        CodePageId = 0
      };
      setting.FileFormat.FieldDelimiter = ",";
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
      }

      Assert.AreEqual(1200, setting.CurrentEncoding.WindowsCodePage); // UTF-16 little endian
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldDelimiterLF()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldDelimiter = "\n"}
      };
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldDelimiterSpace()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldDelimiter = " "}
      };
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderInitErrorFieldQualifierIsFieldDelimiter()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldQualifier = "\""}
      };
      setting.FileFormat.FieldDelimiter = setting.FileFormat.FieldQualifier;
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
        }
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (FileReaderException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderGetInt16Format()
    {
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
          Assert.IsTrue(await test.ReadAsync());
          Assert.AreEqual(1, test.GetInt16(1));
        }
      }
      catch (FormatException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderGetInt64()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual(1, test.GetInt64(0));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetInt64Error()
    {
      var exception = false;
      try
      {
        using (var processDisplay = new DummyProcessDisplay())
        using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
        {
          await test.OpenAsync();
          Assert.IsTrue(await test.ReadAsync());
          Assert.AreEqual(1, test.GetInt64(1));
        }
      }
      catch (FormatException)
      {
        exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public async Task CsvDataReaderGetChar()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual('G', test.GetChar(1));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetStringColumnNotExisting()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        var exception = false;
        await test.OpenAsync();
        await test.ReadAsync();
        try
        {
          test.GetString(666);
        }
        catch (IndexOutOfRangeException)
        {
          exception = true;
        }
        catch (InvalidOperationException)
        {
          exception = true;
        }
        catch (Exception)
        {
          Assert.Fail("Wrong Exception Type");
        }

        Assert.IsTrue(exception, "No Exception thrown");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetString()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.AreEqual("German", test.GetString(1));
        Assert.AreEqual("German", test.GetValue(1));
      }
    }

    public async Task DataReaderResetPositionToFirstDataRow()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.ResetPositionToFirstDataRowAsync();
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    [TestMethod]
    public async Task CsvDataReaderIsDBNull()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsFalse(test.IsDBNull(4));
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(test.IsDBNull(4));
        test.Close();
      }
    }

    [TestMethod]
    public async Task CsvDataReaderTreatNullTextTrue()
    {
      //m_ValidSetting.TreatTextNullAsNull = true;
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());

        Assert.AreEqual(DBNull.Value, test["LangCodeID"]);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderTreatNullTextFalse()
    {
      m_ValidSetting.TreatTextAsNull = null;
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());

        Assert.AreEqual("NULL", test["LangCodeID"]);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetValues()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        var values = new object[test.FieldCount];
        Assert.AreEqual(6, test.GetValues(values));
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetChars()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        char[] buffer = {'0', '0', '0', '0'};
        test.GetChars(1, 0, buffer, 0, 4);
        Assert.AreEqual('G', buffer[0], "G");
        Assert.AreEqual('e', buffer[1], "E");
        Assert.AreEqual('r', buffer[2], "R");
        Assert.AreEqual('m', buffer[3], "M");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderGetSchemaTable()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        var dt = test.GetSchemaTable();
        Assert.IsInstanceOfType(dt, typeof(DataTable));
        Assert.AreEqual(6, dt.Rows.Count);
      }
    }

    [TestMethod]
    public async Task CsvDataReaderReadAfterEndAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        /*
1,German,20/01/2010,276,0.94,Y
2,English,22/01/2012,190,,N
3,German,,150,0.5,N
4,German,01/04/2010,166,0.678,N
5,NULL,05/03/2001,251,0.92,Y
6,French,13/12/2000,399,0.67,N
7,Dutch,01/11/2001,234,0.89,n
         */
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsTrue(await test.ReadAsync());
        Assert.IsFalse(await test.ReadAsync());
        Assert.IsFalse(await test.ReadAsync());
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    [TestMethod]
    public async Task CsvDataReaderReadAfterCloseAsync()
    {
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(m_ValidSetting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.IsTrue(await test.ReadAsync());
        test.Close();
        Assert.IsFalse(await test.ReadAsync());
      }
    }

    //[TestMethod]
    //public async Task CsvDataReader_OpenDetails()
    //{
    //  using (CsvFileReader test = new CsvFileReader())
    //  {
    //    test.Open(true);
    //    Assert.AreEqual(1, m_ValidSetting.Column[0].Size);
    //    Assert.AreEqual(7, m_ValidSetting.Column[1].Size, "LangCodeID");
    //    Assert.AreEqual(10, m_ValidSetting.Column[2].Size, "ExamDate");
    //    Assert.AreEqual(3, m_ValidSetting.Column[3].Size, "Score");
    //    Assert.AreEqual(5, m_ValidSetting.Column[4].Size, "Proficiency");
    //    Assert.AreEqual(1, m_ValidSetting.Column[5].Size, "IsNativeLang");
    //  }
    //}

    [TestMethod]
    public async Task CsvDataReaderOpenDetailSkipRowsAsync()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = ",";
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        await test.ReadAsync();
        Assert.AreEqual(1, test.GetColumn(0).Size, "ID: 1");
        Assert.AreEqual(6, test.GetColumn(1).Size, "LangCodeID: German");
        Assert.AreEqual(10, test.GetColumn(2).Size, "ExamDate");
        Assert.AreEqual(3, test.GetColumn(3).Size, "Score");
        Assert.AreEqual(4, test.GetColumn(4).Size, "Proficiency: 0.94");
        Assert.AreEqual(1, test.GetColumn(5).Size, "IsNativeLang ");
      }
    }

    [TestMethod]
    public async Task CsvDataReaderNoHeader()
    {
      var setting = new CsvFile
      {
        FileName = UnitTestInitialize.GetTestPath("BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1,
        FileFormat = {FieldDelimiter = ","}
      };
      using (var processDisplay = new DummyProcessDisplay())
      using (var test = new CsvFileReader(setting, TimeZoneInfo.Local.Id, processDisplay))
      {
        await test.OpenAsync();
        Assert.AreEqual("Column1", test.GetName(0));
        Assert.AreEqual("Column2", test.GetName(1));
        Assert.AreEqual("Column3", test.GetName(2));
        Assert.AreEqual("Column4", test.GetName(3));
        Assert.AreEqual("Column5", test.GetName(4));
        Assert.AreEqual("Column6", test.GetName(5));
      }
    }
  }
}