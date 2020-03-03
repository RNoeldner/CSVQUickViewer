﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvTools.Tests
{
  [TestClass]
  public class ColumnCollectionTest
  {
    [TestMethod]
    public void AddIfNew()
    {
      var test = new ColumnCollection();
      Assert.AreEqual(0, test.Count);
      var item1 = test.AddIfNew(new Column("Test"));
      Assert.AreEqual(1, test.Count);
      var item2 = test.AddIfNew(new Column("Test"));
      Assert.AreEqual(1, test.Count);
      Assert.AreEqual(item1, item2);

      test.AddIfNew(new Column());

      var exception = false;
      try
      {
        test.AddIfNew(null);
      }
      catch (ArgumentException)
      {
        exception = true;
      }
      catch (Exception ex)
      {
        Assert.Fail("Wrong Exception Type: " + ex.GetType());
      }

      Assert.IsTrue(exception, "No Exception thrown");
    }

    [TestMethod]
    public void Get()
    {
      var test = new ColumnCollection();
      var item1 = test.AddIfNew(new Column("Test"));
      var item2 = test.AddIfNew(new Column("Test2"));
      Assert.AreEqual(item1, test.Get("Test"));
      Assert.AreEqual(item1, test.Get("TEST"));
      Assert.AreEqual(item2, test.Get("tEst2"));

      Assert.IsNull(test.Get("")); 
      Assert.IsNull(test.Get(null));
    }

    [TestMethod]
    public void Clone()
    {
      var test1 = new ColumnCollection();
      test1.AddIfNew(new Column("Test1"));
      test1.AddIfNew(new Column("Test2"));
      test1.AddIfNew(new Column("Test3"));

      var test2 = test1.Clone();

      Assert.IsTrue(test2.Equals(test1));
    }

    [TestMethod]
    public void CopyTo()
    {
      var test1 = new ColumnCollection();
      test1.AddIfNew(new Column("Test1"));
      test1.AddIfNew(new Column("Test2"));
      test1.AddIfNew(new Column("Test3"));
      var test2 = new ColumnCollection();
      test1.CopyTo(test2);

      Assert.IsTrue(test2.Equals(test1));
    }
  }
}
