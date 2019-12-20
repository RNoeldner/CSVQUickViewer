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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pri.LongPath;

namespace CsvTools.Tests
{
  [TestClass]
  public class JsonFileReaderTest
  {
    private readonly string m_ApplicationDirectory = FileSystemUtils.ExecutableDirectoryName() + @"\TestFiles";

    [TestMethod]
    public void NotSupported()
    {
      var setting = new CsvFile(Path.Combine(m_ApplicationDirectory, "Emp.json"))
      {
        JsonFormat = true
      };

      using (var dpd = new DummyProcessDisplay())
      using (var jfr = new JsonFileReader(setting, dpd))
      {
        jfr.Open();
        jfr.Read();

        try
        {
          jfr.GetByte(1);
          Assert.Fail("GetByte - No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"GetByte - Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          var buffer = new byte[200];
          jfr.GetBytes(1, 0, buffer,0,100);
          Assert.Fail("GetBytes - No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"GetBytes - Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          var buffer = new byte[200];
          jfr.GetBytes(1, 0, buffer, 0, 100);
          Assert.Fail("No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          jfr.GetChar(1);
          Assert.Fail("GetChar - No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"GetChar - Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          var buffer = new char[200];
          jfr.GetChars(1, 0, buffer, 0, 100);
          Assert.Fail("GetChars - No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"GetChars - Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          var buffer = new byte[200];
          jfr.GetBytes(1, 0, buffer, 0, 100);
          Assert.Fail("No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"Wrong type of exception  {ex.GetType().Name}");
        }

        try
        {
          jfr.GetData(2);
          Assert.Fail("GetData - No Exception");
        }
        catch (NotImplementedException)
        {
        }
        catch (Exception ex)
        {
          Assert.Fail($"GetData - Wrong type of exception  {ex.GetType().Name}");
        }

      }

    }

    [TestMethod]
    public void ReadJSonEmp()
    {
      var setting = new CsvFile(Path.Combine(m_ApplicationDirectory, "Emp.json"))
      {
        JsonFormat = true
      };

      using (var dpd = new DummyProcessDisplay())
      using (var jfr = new JsonFileReader(setting, dpd))
      {
        jfr.Open();
        Assert.AreEqual(110, jfr.FieldCount);
        jfr.Read();
        Assert.AreEqual("String", jfr.GetDataTypeName(0));
        Assert.AreEqual("43357099", jfr.GetValue(0));
        Assert.AreEqual("T454898", jfr.GetValue(1));
        Assert.AreEqual(new DateTime(2012 ,02 , 06), jfr.GetValue(jfr.GetOrdinal("LastHireDate")));
        Assert.IsTrue(jfr.IsDBNull(jfr.GetOrdinal("ASSIGN_2ND_DEPT")));
        Assert.AreEqual((Int16) 0, jfr.GetInt16(jfr.GetOrdinal("Approvals")));
        Assert.AreEqual(0, jfr.GetInt32(jfr.GetOrdinal("Approvals")));
        Assert.AreEqual(0l, jfr.GetInt64(jfr.GetOrdinal("Approvals")));
        Assert.AreEqual(0f, jfr.GetFloat(jfr.GetOrdinal("Approvals")));
        Assert.AreEqual((decimal)0, jfr.GetDecimal(jfr.GetOrdinal("Approvals")));
        Assert.AreEqual((double)0, jfr.GetDouble(jfr.GetOrdinal("Approvals")));




        Assert.IsTrue(jfr.GetBoolean(2));
        Assert.AreEqual(1.000, jfr.GetValue(jfr.GetOrdinal("FTE")));
        while (jfr.Read())
        {
        }
        Assert.AreEqual(2782, jfr.RecordNumber);
      }
    }

    [TestMethod]
    public void ReadJSon1()
    {
      var setting = new CsvFile(Path.Combine(m_ApplicationDirectory, "Jason1.json"))
      {
        JsonFormat = true
      };

      using (var dpd = new DummyProcessDisplay())
      using (var jfr = new JsonFileReader(setting, dpd))
      {
        jfr.Open();
        Assert.AreEqual(20, jfr.FieldCount);
        while (jfr.Read())
        {
        }
        Assert.AreEqual(2, jfr.RecordNumber);
      }
    }

    [TestMethod]
    public void ReadJSon2()
    {
      var setting = new CsvFile(Path.Combine(m_ApplicationDirectory, "Jason2.json"))
      {
        JsonFormat = true
      };
      using (var dpd = new DummyProcessDisplay())
      using (var jfr = new JsonFileReader(setting, dpd))
      {
        jfr.Open();
        Assert.AreEqual(7, jfr.FieldCount);
        while (jfr.Read())
        {
        }
        Assert.AreEqual(29, jfr.RecordNumber);
      }
    }

    [TestMethod]
    public void ReadJSon3()
    {
      var setting = new CsvFile(Path.Combine(m_ApplicationDirectory, "Jason3.json"))
      {
        JsonFormat = true
      };

      using (var dpd = new DummyProcessDisplay())
      using (var jfr = new JsonFileReader(setting, dpd))
      {
        jfr.Open();
        Assert.AreEqual(2, jfr.FieldCount);
        while (jfr.Read())
        {
        }
        Assert.AreEqual(3, jfr.RecordNumber);
      }
    }
  }
}