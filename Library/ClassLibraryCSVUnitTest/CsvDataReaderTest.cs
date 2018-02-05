﻿using CsvTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;

namespace CvsTool.Tests
{
  [TestClass]
  public class CsvDataReader_UnitTest
  {
    private readonly string m_ApplicationDirectory = FileSystemUtils.ExecutableDirectoryName() + @"\TestFiles";
    private readonly CsvFile m_ValidSetting = new CsvFile();

    [TestInitialize]
    public void Init()
    {
      m_ValidSetting.FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt");
      var cf = m_ValidSetting.ColumnAdd(new Column
      {
        DataType = DataType.DateTime,
        Name = "ExamDate"
      });

      Assert.IsNotNull(cf);
      cf.DateFormat = @"dd/MM/yyyy";
      m_ValidSetting.FileFormat.FieldDelimiter = ",";
      m_ValidSetting.FileFormat.CommentLine = "#";
      m_ValidSetting.ColumnAdd(new Column { Name = "Score", DataType = DataType.Integer });
      m_ValidSetting.ColumnAdd(new Column { Name = "Proficiency", DataType = DataType.Numeric });
      m_ValidSetting.ColumnAdd(new Column { Name = "IsNativeLang", DataType = DataType.Boolean });
    }

    [TestMethod]
    public void TestGetDataTypeName()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.AreEqual("String", test.GetDataTypeName(0));
      }
    }

    [TestMethod]
    public void TestWarningsRecordWithMapping()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        var dataTable = new DataTable
        {
          TableName = "DataTable",
          Locale = CultureInfo.InvariantCulture
        };

        dataTable.Columns.Add(test.GetName(0), test.GetFieldType(0));

        var recordNumberColumn = dataTable.Columns.Add(test.RecordNumberFieldName, typeof(int));
        recordNumberColumn.AllowDBNull = true;

        var lineNumberColumn = dataTable.Columns.Add(test.EndLineNumberFieldName, typeof(int));
        lineNumberColumn.AllowDBNull = true;

        int[] columnMapping = { 0 };
        var warningsList = new RowErrorCollection();
        test.CopyRowToTable(dataTable, warningsList, columnMapping, recordNumberColumn, lineNumberColumn, null);
        var dataRow = dataTable.NewRow();
        test.Read();

        //warningsList.Add(-1, "Test1");
        //warningsList.Add(0, "Test2");
        //test.AssignNumbersAndWarnings(dataRow, columnMapping, recordNumberColumn, lineNumberColumn, null, warningsList);

        //Assert.AreEqual("Test1", dataRow.RowError);
        //Assert.AreEqual("Test2", dataRow.GetColumnError(0));
      }
    }

    [TestMethod]
    public void CopyRowToTableNullWarningList()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        try
        {
          test.CopyRowToTable(new DataTable(), null, null, null, null, null);
        }
        catch (ArgumentNullException)
        {
          return;
        }
        catch (NullReferenceException)
        {
          return;
        }

        Assert.Fail("Expected Exception");
      }
    }

    [TestMethod]
    public void CopyRowToTableNullDataTable()
    {
      using (var test = m_ValidSetting.GetFileReader())
      {
        test.Open(CancellationToken.None, false);
        try
        {
          test.CopyRowToTable(null, new RowErrorCollection(), null, null, null, null);
        }
        catch (ArgumentNullException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch
        {
          Assert.Fail();
        }
      }
    }

    [TestMethod]
    public void TestWarningsRecordNoMapping()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        var dataTable = new DataTable
        {
          TableName = "DataTable",
          Locale = CultureInfo.InvariantCulture
        };

        dataTable.Columns.Add(test.GetName(0), test.GetFieldType(0));

        var recordNumberColumn = dataTable.Columns.Add(test.RecordNumberFieldName, typeof(int));
        recordNumberColumn.AllowDBNull = true;

        var lineNumberColumn = dataTable.Columns.Add(test.EndLineNumberFieldName, typeof(int));
        lineNumberColumn.AllowDBNull = true;

        var dataRow = dataTable.NewRow();
        test.Read();

        var warningsList = new Dictionary<int, string>
        {
          {-1, "Test1"},
          {0, "Test2"}
        };

        //test.AssignNumbersAndWarnings(dataRow, null, recordNumberColumn, lineNumberColumn, null, warningsList);
        //Assert.AreEqual("Test1", dataRow.RowError);
        //Assert.AreEqual("Test2", dataRow.GetColumnError(0));
      }
    }

    [TestMethod]
    public void GetPart()
    {
      var partToEnd = new Column
      {
        DataType = DataType.TextPart,
        PartSplitter = '-',
        Part = 2,
        PartToEnd = true
      };
      var justPart = new Column
      {
        DataType = DataType.TextPart,
        PartSplitter = '-',
        Part = 2,
        PartToEnd = false
      };

      using (var test = new CsvFileReader(m_ValidSetting))
      {
        var inputValue = "17-Hello-World";
        var value = test.GetPart(inputValue, partToEnd);
        Assert.AreEqual("Hello-World", value);

        var value2 = test.GetPart(inputValue, justPart);
        Assert.AreEqual("Hello", value2);
      }
    }

    [TestMethod]
    public void GetInteger32And64()
    {
      var column = new Column
      {
        DataType = DataType.Integer,
        GroupSeparator = ",",
        DecimalSeparator = "."
      };
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        var inputValue = "17";

        var value32 = test.GetInt32Null(inputValue, column);
        Assert.AreEqual(17, value32.Value);

        var value64 = test.GetInt64Null(inputValue, column);
        Assert.AreEqual(17, value64.Value);

        value32 = test.GetInt32Null(null, column);
        Assert.IsFalse(value32.HasValue);

        value64 = test.GetInt64Null(null, column);
        Assert.IsFalse(value64.HasValue);
      }
    }

    [TestMethod]
    public void TestBatchFinishedNotifcation()
    {
      var finished = false;
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.ReadFinished += delegate { finished = true; };
        test.Open(CancellationToken.None, false);

        while (test.Read())
        {
        }
      }

      Assert.IsTrue(finished, "ReadFinished");
    }

    [TestMethod]
    public void TestReadFinishedNotifcation()
    {
      var finished = false;
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.ReadFinished += delegate { finished = true; };
        test.Open(CancellationToken.None, false);
        while (test.Read())
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

      Assert.IsNotNull(target.GetColumn("Score"));
      var cf = target.GetColumn("Score");
      Assert.AreEqual(cf.Name, "Score");

      // Remove the one filed
      target.Column.Remove(target.GetColumn("Score"));
      Assert.IsNull(target.GetColumn("Score"));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void GetDateTimeTest()
    {
      var csvFile = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "TestFile.txt"),
        CodePageId = 65001,
        FileFormat = { FieldDelimiter = "tab" }
      };

      csvFile.ColumnAdd(new Column { Name = "Title", DataType = DataType.DateTime });

      using (var test = new CsvFileReader(csvFile))
      {
        test.Open(CancellationToken.None, false);
        test.Read();
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    public void CsvDataReader_WriteToDataTable()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);

        var res = test.WriteToDataTable(m_ValidSetting, 0, null, CancellationToken.None);
        Assert.AreEqual(7, res.Rows.Count);
        Assert.AreEqual(
          6 + (m_ValidSetting.DisplayStartLineNo ? 1 : 0) + (m_ValidSetting.DisplayEndLineNo ? 1 : 0) +
          (m_ValidSetting.DisplayRecordNo ? 1 : 0), res.Columns.Count);
      }
    }

    [TestMethod]
    public void CsvDataReader_WriteToDataTable_DisplayRecordNo()
    {
      var newCsvFile = (CsvFile)m_ValidSetting.Clone();
      newCsvFile.DisplayRecordNo = true;
      using (var test = new CsvFileReader(newCsvFile))
      {
        test.Open(CancellationToken.None, false);
        var res = test.WriteToDataTable(newCsvFile, 0, null, CancellationToken.None);
        Assert.AreEqual(7, res.Rows.Count);
        Assert.AreEqual(
          6 + (newCsvFile.DisplayStartLineNo ? 1 : 0) + (newCsvFile.DisplayEndLineNo ? 1 : 0) +
          (newCsvFile.DisplayRecordNo ? 1 : 0), res.Columns.Count);
      }
    }

    [TestMethod]
    public void CsvDataReader_WriteToDataTable2()
    {
      var wl = new RowErrorCollection();
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        var res = test.WriteToDataTable(m_ValidSetting, 2, wl, CancellationToken.None);
        Assert.AreEqual(2, res.Rows.Count);
        Assert.AreEqual(
          6 + (m_ValidSetting.DisplayStartLineNo ? 1 : 0) + (m_ValidSetting.DisplayEndLineNo ? 1 : 0) +
          (m_ValidSetting.DisplayRecordNo ? 1 : 0), res.Columns.Count);
      }
    }

    [TestMethod]
    public void CsvDataReader_ImportFileEmptyNullNotExisting()
    {
      var setting = new CsvFile();
      try
      {
        setting.FileName = string.Empty;
        var test = new CsvFileReader(setting);

        Assert.Fail("Exception expected");
      }
      catch (ArgumentException)
      {
      }
      catch (ApplicationException)
      {
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      try
      {
        var test = new CsvFileReader(setting);
        Assert.Fail("Exception expected");
      }
      catch (ArgumentException)
      {
      }
      catch (ApplicationException)
      {
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      try
      {
        setting.FileName = @"b;dslkfg;sldfkgjs;ldfkgj;sldfkg.sdfgsfd";
        var test = new CsvFileReader(setting);

        Assert.Fail("Exception expected");
      }
      catch (ArgumentException)
      {
      }
      catch (FileNotFoundException)
      {
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }
    }

    [TestMethod]
    public void CsvDataReader_Properties()
    {
      var test = new CsvFileReader(m_ValidSetting);
      test.Open(CancellationToken.None, false);

      Assert.AreEqual(0, test.Depth, "Depth");
      Assert.AreEqual(6, test.FieldCount, "FieldCount");
      Assert.AreEqual(0U, test.RecordNumber, "RecordNumber");
      Assert.AreEqual(-1, test.RecordsAffected, "RecordsAffected");

      Assert.IsFalse(test.EndOfFile, "EndOfFile");
      Assert.IsFalse(test.IsClosed, "IsClosed");
    }

    [TestMethod]
    public void CsvDataReader_GetName()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.AreEqual("ID", test.GetName(0));
        Assert.AreEqual("LangCodeID", test.GetName(1));
        Assert.AreEqual("ExamDate", test.GetName(2));
        Assert.AreEqual("Score", test.GetName(3));
        Assert.AreEqual("Proficiency", test.GetName(4));
        Assert.AreEqual("IsNativeLang", test.GetName(5));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetOrdinal()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
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
    public void CsvDataReader_UseIndexer()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual("1", test["ID"]);
        Assert.AreEqual("German", test[1]);
        Assert.AreEqual(new DateTime(2010, 01, 20), test["ExamDate"]);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(DBNull.Value, test["Proficiency"]);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetValueNull()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.AreEqual(DBNull.Value, test.GetValue(4));
      }
    }

#if COMInterface
    [TestMethod]
    public void CsvDataReader_GetValueADONull()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.AreEqual(DBNull.Value, test.GetValueADO(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void CsvDataReader_GetValueADONoRead()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.AreEqual(DBNull.Value, test.GetValueADO(0));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetValueADO()
    {
      using (CsvFileReader test = new CsvFileReader())
      {
        test.Open(false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual("1", test.GetValueADO(0));
        Assert.AreEqual("German", test.GetValueADO(1));
        Assert.AreEqual(new DateTime(2010, 01, 20), test.GetValueADO(2));
      }
    }

#endif

    [TestMethod]
    public void CsvDataReader_GetBoolean()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.GetBoolean(5));
        Assert.IsTrue(test.Read());
        Assert.IsFalse(test.GetBoolean(5));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetBooleanError()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetBoolean(1);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetDateTime()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        // 20/01/2010
        Assert.AreEqual(new DateTime(2010, 01, 20), test.GetDateTime(2));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDateTimeError()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetInt32()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(276, test.GetInt32(3));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetInt32Error()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetInt32(1);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetDecimal()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(0.94m, test.GetDecimal(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDecimalError()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetDecimal(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReaderGetInt32Null()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        test.GetInt32(4);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public void CsvDataReaderGetBytes()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        test.GetBytes(0, 0, null, 0, 0);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public void CsvDataReaderGetData()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        test.GetData(0);
      }
    }

    [TestMethod]
    public void CsvDataReaderGetFloat()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(Convert.ToSingle(0.94), test.GetFloat(4));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReaderGetFloatError()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetFloat(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReaderGetGuid()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetGuid(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDateTimeNull()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        test.GetDateTime(2);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDateTimeWrongType()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.GetDateTime(1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDecimalFormatException()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        test.GetDecimal(4);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetByte()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetByte(0));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetByteFrormat()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetByte(1));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetDouble()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetDouble(0));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void CsvDataReader_GetDoubleFrormat()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetDouble(1));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetInt16()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetInt16(0));
      }
    }

    [TestMethod]
    public void CsvDataReader_InitWarnings()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldQualifier = "XX";
      setting.FileFormat.FieldDelimiter = ",,";
      var warningList = new RowErrorCollection();
      using (var test = new CsvFileReader(setting))
      {
        test.Warning += warningList.Add;
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(warningList.Display.Contains("Only the first character of 'XX' is be used for quoting."));
        Assert.IsTrue(warningList.Display.Contains("Only the first character of ',,' is used as delimiter."));
      }
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldDelimiterCR()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = "\r";
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldQualifierCR()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldQualifier = "Carriage return";
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldQualifierLF()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldQualifier = "Line feed";
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    [Ignore]
    public void CsvDataReader_GuessCodePage()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = true,
        CodePageId = 0
      };
      setting.FileFormat.FieldDelimiter = ",";
      using (var test = new CsvFileReader(setting))
      {
        test.Open(CancellationToken.None, false);
      }

      Assert.AreEqual(setting.CurrentEncoding.WindowsCodePage, 1252);
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldDelimiterLF()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = "\n";
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldDelimiterSpace()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = " ";
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_InitErrorFieldQualifierIsFieldDelimiter()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldQualifier = "\"";
      setting.FileFormat.FieldDelimiter = setting.FileFormat.FieldQualifier;
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(setting))
        {
          test.Open(CancellationToken.None, false);
        }
      }
      catch (ArgumentException)
      {
        Exception = true;
      }
      catch (ApplicationException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_GetInt16Format()
    {
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(m_ValidSetting))
        {
          test.Open(CancellationToken.None, false);
          Assert.IsTrue(test.Read());
          Assert.AreEqual(1, test.GetInt16(1));
        }
      }
      catch (FormatException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_GetInt64()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual(1, test.GetInt64(0));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetInt64Error()
    {
      var Exception = false;
      try
      {
        using (var test = new CsvFileReader(m_ValidSetting))
        {
          test.Open(CancellationToken.None, false);
          Assert.IsTrue(test.Read());
          Assert.AreEqual(1, test.GetInt64(1));
        }
      }
      catch (FormatException)
      {
        Exception = true;
      }
      catch (Exception)
      {
        Assert.Fail("Wrong Exception Type");
      }

      Assert.IsTrue(Exception, "No Exception thrown");
    }

    [TestMethod]
    public void CsvDataReader_GetChar()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual('G', test.GetChar(1));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetStringColumnNotExisting()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        var Exception = false;
        test.Open(CancellationToken.None, false);
        test.Read();
        try
        {
          test.GetString(666);
        }
        catch (ArgumentException)
        {
          Exception = true;
        }
        catch (IndexOutOfRangeException)
        {
          Exception = true;
        }
        catch (NullReferenceException)
        {
          Exception = true;
        }
        catch (Exception)
        {
          Assert.Fail("Wrong Exception Type");
        }

        Assert.IsTrue(Exception, "No Exception thrown");
      }
    }

    [TestMethod]
    public void CsvDataReader_GetString()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.AreEqual("German", test.GetString(1));
        Assert.AreEqual("German", test.GetValue(1));
      }
    }

    public void DataReader_ResetPositionToFirstDataRow()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.ResetPositionToFirstDataRow();
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    [TestMethod]
    public void CsvDataReader_IsDBNull()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsFalse(test.IsDBNull(4));
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.IsDBNull(4));
        test.Close();
      }
    }

    [TestMethod]
    public void CsvDataReader_TreatNullTextTrue()
    {
      //m_ValidSetting.TreatTextNullAsNull = true;
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());

        Assert.AreEqual(DBNull.Value, test["LangCodeID"]);
      }
    }

    [TestMethod]
    public void CsvDataReader_TreatNullTextFalse()
    {
      m_ValidSetting.TreatTextAsNull = null;
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());

        Assert.AreEqual("NULL", test["LangCodeID"]);
      }
    }

    [TestMethod]
    public void CsvDataReader_GetValues()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        var values = new object[test.FieldCount];
        Assert.AreEqual(6, test.GetValues(values));
      }
    }

    [TestMethod]
    public void CsvDataReader_GetChars()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        char[] buffer = { '0', '0', '0', '0' };
        test.GetChars(1, 0, buffer, 0, 4);
        Assert.AreEqual('G', buffer[0], "G");
        Assert.AreEqual('e', buffer[1], "E");
        Assert.AreEqual('r', buffer[2], "R");
        Assert.AreEqual('m', buffer[3], "M");
      }
    }

    [TestMethod]
    public void CsvDataReader_GetSchemaTable()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        var dt = test.GetSchemaTable();
        Assert.IsInstanceOfType(dt, typeof(DataTable));
        Assert.AreEqual(6, dt.Rows.Count);
      }
    }

    [TestMethod]
    public void CsvDataReader_ReadAfterEnd()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsTrue(test.Read());
        Assert.IsFalse(test.Read());
        Assert.IsFalse(test.Read());
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    [TestMethod]
    public void CsvDataReader_ReadAfterClose()
    {
      using (var test = new CsvFileReader(m_ValidSetting))
      {
        test.Open(CancellationToken.None, false);
        Assert.IsTrue(test.Read());
        test.Close();
        Assert.IsFalse(test.Read());
      }
    }

    //[TestMethod]
    //public void CsvDataReader_OpenDetails()
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
    public void CsvDataReader_OpenDetailSkipRows()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = ",";
      using (var test = new CsvFileReader(setting))
      {
        test.Open(CancellationToken.None, true);
        Assert.AreEqual(1, test.GetColumn(0).Size);
        Assert.AreEqual(7, test.GetColumn(1).Size, "LangCodeID");
        Assert.AreEqual(10, test.GetColumn(2).Size, "ExamDate");
        Assert.AreEqual(3, test.GetColumn(3).Size, "Score");
        Assert.AreEqual(5, test.GetColumn(4).Size, "Proficiency");
        Assert.AreEqual(1, test.GetColumn(5).Size, "IsNativeLang");
      }
    }

    [TestMethod]
    public void CsvDataReader_NoHeader()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt"),
        HasFieldHeader = false,
        SkipRows = 1
      };
      setting.FileFormat.FieldDelimiter = ",";
      using (var test = new CsvFileReader(setting))
      {
        test.Open(CancellationToken.None, true);
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