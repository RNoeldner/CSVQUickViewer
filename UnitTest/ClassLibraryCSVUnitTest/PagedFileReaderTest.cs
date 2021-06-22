﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvTools.Tests
{

  [TestClass()]
  public class PagedFileReaderTest
  {
    [TestMethod()]
    public async Task PagedFileReaderOpen()
    {
      using (var process = new CustomProcessDisplay(UnitTestInitializeCsv.Token))
      using (var reader = new CsvFileReader(UnitTestHelper.ReaderGetAllFormats(), process))
      {
        var test = new PagedFileReader(reader, 20, UnitTestInitializeCsv.Token);
        await test.OpenAsync(true, true, true, true);
        Assert.AreEqual(20, test.Count);

        // get the value in row 1 for the property TZ
        dynamic testValue = test[0];
        Assert.AreEqual("CET", testValue.TZ);

        test.Close();
      }
    }

    [TestMethod()]
    public async Task PagedFileReaderNextPage()
    {
      using (var process = new CustomProcessDisplay(UnitTestInitializeCsv.Token))
      using (var reader = new CsvFileReader(UnitTestHelper.ReaderGetAllFormats(), process))
      {
        var test = new PagedFileReader(reader, 5, UnitTestInitializeCsv.Token);
        await test.OpenAsync(true, true, true, true);
        Assert.AreEqual(5, test.Count);

        bool collectionChangedCalled = false;
        test.CollectionChanged += (o, s) => { collectionChangedCalled = true; };
        await test.MoveToNextPageAsync();
        Assert.IsTrue(collectionChangedCalled);
        Assert.AreEqual(5, test.Count);
      }
    }
  }
}
