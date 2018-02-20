﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CsvTools.Tests
{
  [TestClass]
  public class CsvHelperTest
  {
    private readonly string m_ApplicationDirectory = FileSystemUtils.ExecutableDirectoryName() + @"\TestFiles";

    [TestMethod]
    public void GuessCodePage()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")
      };
      CsvHelper.GuessCodePage(setting);
      Assert.AreEqual(1200, setting.CodePageId);

      var setting2 = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "UnicodeUTF16BE.txt")
      };
      CsvHelper.GuessCodePage(setting2);
      Assert.AreEqual(1201, setting2.CodePageId);

      var setting3 = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "Test.csv")
      };
      CsvHelper.GuessCodePage(setting3);
      Assert.AreEqual(65001, setting3.CodePageId);
    }

    [TestMethod]
    public void GuessHasHeader()
    {
      Assert.IsTrue(CsvHelper.GuessHasHeader(new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")
      }, new DummyProcessDisplay()), "BasicCSV.txt");

      Assert.IsFalse(CsvHelper.GuessHasHeader(new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "txTranscripts.txt")
      }, new DummyProcessDisplay()), "txTranscripts.txt");
    }

    [TestMethod]
    public void GuessStartRow()
    {
      Assert.AreEqual(0, CsvHelper.GuessStartRow(new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")
      }), "BasicCSV.txt");
    }

    [TestMethod]
    public void GuessDelimiter()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")
      };
      Assert.AreEqual(",", CsvHelper.GuessDelimiter(setting));

      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "LateStartRow.txt"))
      {
        SkipRows = 10,
        CodePageId = 20127
      };
      test.FileFormat.FieldQualifier = "\"";
      Assert.AreEqual("|", CsvHelper.GuessDelimiter(test));
    }

    [TestMethod]
    public void HasUsedQualifier()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt")
      };

      CsvHelper.RefreshCsvFile(setting, new DummyProcessDisplay());
      Assert.AreEqual(1200, setting.CodePageId);
      Assert.AreEqual(",", setting.FileFormat.FieldDelimiter);
    }

    [TestMethod]
    public void FillGuessColumnFormat_InvalidateColumnHeader()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt");
      setting.HasFieldHeader = true;
      // setting.TreatTextNullAsNull = true;

      using (var test = setting.GetFileReader())
      {
        test.Open(CancellationToken.None, false);
        var list = new List<string>();
        CsvHelper.InvalidateColumnHeader(setting);
      }
    }

    [TestMethod]
    public void GetColumnHeader_FileEmpty()
    {
      var headers = CsvHelper.GetColumnHeader(new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "CSVTestEmpty.txt"),
        HasFieldHeader = true
      }, false, null).ToArray();
      Assert.AreEqual(0, headers.Length);
    }

    [TestMethod]
    public void GetColumnHeader_Headings()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt");
      setting.FileFormat.FieldDelimiter = ",";
      setting.HasFieldHeader = true;
      var headers = CsvHelper.GetColumnHeader(setting, false, null).ToArray();
      Assert.AreEqual(6, headers.Length);
      Assert.AreEqual("ID", headers[0]);
      Assert.AreEqual("IsNativeLang", headers[5]);
    }

    [TestMethod]
    public void GetColumnHeader_NoHeaders()
    {
      var setting = new CsvFile
      {
        FileName = Path.Combine(m_ApplicationDirectory, "UnicodeUTF8.txt"),
        HasFieldHeader = false
      };

      Assert.AreEqual("Column1", CsvHelper.GetColumnHeader(setting, false, null).First());
    }

    [TestMethod]
    public void GuessDelimiterComma()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "AlternateTextQualifiers.txt"))
      {
        CodePageId = -1
      };
      test.FileFormat.EscapeCharacter = "\\";

      Assert.AreEqual(",", CsvHelper.GuessDelimiter(test));
    }

    [TestMethod]
    public void GuessDelimiterPipe()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "DifferentColumnDelimiter.txt"))
      {
        CodePageId = -1
      };
      test.FileFormat.EscapeCharacter = "";
      Assert.AreEqual("|", CsvHelper.GuessDelimiter(test));
    }

    [TestMethod]
    public void GuessDelimiterQualifier()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "TextQualifiers.txt"));
      test.CodePageId = -1;
      test.FileFormat.EscapeCharacter = "";
      Assert.AreEqual(",", CsvHelper.GuessDelimiter(test));
    }

    [TestMethod]
    public void GuessDelimiterTAB()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "txTranscripts.txt"));
      test.CodePageId = -1;
      test.FileFormat.EscapeCharacter = "\\";
      Assert.AreEqual("TAB", CsvHelper.GuessDelimiter(test));
    }

    [TestMethod]
    public void GuessStartRow0()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "TextQualifiers.txt"));
      test.CodePageId = -1;
      test.FileFormat.FieldDelimiter = ",";
      test.FileFormat.FieldQualifier = "\"";
      Assert.AreEqual(0, CsvHelper.GuessStartRow(test));
    }

    [TestMethod]
    public void GuessStartRow12()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "SkippingEmptyRowsWithDelimiter.txt"));
      test.CodePageId = -1;
      test.FileFormat.FieldDelimiter = ",";
      test.FileFormat.FieldQualifier = "\"";
      Assert.AreEqual(12, CsvHelper.GuessStartRow(test));
    }

    [TestMethod]
    public void HasUsedQualifier_False()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt");
      setting.HasFieldHeader = true;

      Assert.IsFalse(CsvHelper.HasUsedQualifier(setting, CancellationToken.None));
    }

    [TestMethod]
    public void HasUsedQualifier_True()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "AlternateTextQualifiers.txt");
      Assert.IsTrue(CsvHelper.HasUsedQualifier(setting, CancellationToken.None));
    }

    [TestMethod]
    public void NewCsvFileGuessAll_Headings()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "BasicCSV.txt");
      using (var display = new DummyProcessDisplay())
      {
        setting.RefreshCsvFile(display);
      }

      Assert.AreEqual(0, setting.SkipRows);
      Assert.AreEqual(",", setting.FileFormat.FieldDelimiter);
      Assert.AreEqual(1200, setting.CodePageId); //UTF16_LE
    }

    [TestMethod]
    public void NewCsvFileGuessAll_TestEmpty()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "CSVTestEmpty.txt");
      using (var display = new DummyProcessDisplay())
      {
        setting.RefreshCsvFile(display);
      }

      Assert.AreEqual(0, setting.SkipRows);
    }

    [TestMethod]
    public void TestGuessStartRow()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "LateStartRow.txt"));
      test.CodePageId = 20127;
      test.FileFormat.FieldDelimiter = "|";
      test.FileFormat.FieldQualifier = "\"";
      var rows = CsvHelper.GuessStartRow(test);
      Assert.AreEqual(10, rows);
    }

    [TestMethod]
    public void GetEmptyColumnHeaderTest()
    {
      var setting = new CsvFile();
      setting.FileFormat.FieldDelimiter = ",";
      setting.FileName = Path.Combine(m_ApplicationDirectory, "EmptyColumns.txt");
      setting.HasFieldHeader = false;
      using (var disp = new DummyProcessDisplay())
      {
        Assert.IsTrue(CsvHelper.GetEmptyColumnHeader(setting, disp).IsEmpty());
        setting.HasFieldHeader = true;
        var res = CsvHelper.GetEmptyColumnHeader(setting, disp);

        Assert.IsFalse(res.IsEmpty());
        Assert.AreEqual("ID", res[0]);
      }
    }

    [TestMethod]
    public void GetColumnIndexTest()
    {
      var setting = new CsvFile();
      setting.FileName = Path.Combine(m_ApplicationDirectory, "EmptyColumns.txt");
      setting.FileFormat.FieldDelimiter = ",";
      setting.HasFieldHeader = true;
      Assert.AreEqual(0, CsvHelper.GetColumnIndex(setting, "ID"));
      Assert.AreEqual(2, CsvHelper.GetColumnIndex(setting, "ExamDate"));
    }

    [TestMethod]
    public void GuessNotADelimitedFileTest()
    {
      ICsvFile test = new CsvFile(Path.Combine(m_ApplicationDirectory, "EmptyColumns.txt"));
      test.CodePageId = 65001;
      Assert.IsFalse(CsvHelper.GuessNotADelimitedFile(test));

      ICsvFile test2 = new CsvFile(Path.Combine(m_ApplicationDirectory, "RowWithoutColumnDelimiter.txt"));
      test2.CodePageId = 65001;
      Assert.IsTrue(CsvHelper.GuessNotADelimitedFile(test2));
    }

    [TestMethod]
    public void GuessNewlineTest()
    {
      var Test = new CsvFile(Path.Combine(m_ApplicationDirectory, "TestFile.txt"));
      Test.CodePageId = 65001;
      Test.FileFormat.FieldQualifier = "\"";
      Assert.AreEqual("LF", CsvHelper.GuessNewline(Test));
    }
  }
}