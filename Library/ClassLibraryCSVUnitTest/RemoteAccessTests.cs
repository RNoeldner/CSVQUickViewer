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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvTools.Tests
{
  [TestClass()]
  public class RemoteAccessTests
  {
    public void CopyToTest()
    {
      // Covered by generic test
    }

    public void EqualsTest()
    {
      // Covered by generic test
    }

    [TestMethod()]
    public void RemoteAccessTest()
    {
      var testCase = new RemoteAccess();
      var hasFired = false;
      testCase.PropertyChanged += delegate (object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        hasFired = true;
      };
      testCase.EncryptedHostName = "Test";
      Assert.AreEqual("Test", testCase.EncryptedHostName);
      Assert.IsTrue(hasFired);
      hasFired = false;
      testCase.EncryptedHostName = "Test";
      Assert.IsFalse(hasFired);

      testCase.EncryptedUser = "Hello";
      Assert.IsTrue(hasFired);
      Assert.AreEqual("Hello", testCase.EncryptedUser);
      hasFired = false;
      testCase.EncryptedUser = "Hello";
      Assert.IsFalse(hasFired);

      testCase.EncryptedPassword = "World";
      Assert.IsTrue(hasFired);
      Assert.AreEqual("World", testCase.EncryptedPassword);
      hasFired = false;
      testCase.EncryptedPassword = "World";
      Assert.IsFalse(hasFired);
    }
  }
}