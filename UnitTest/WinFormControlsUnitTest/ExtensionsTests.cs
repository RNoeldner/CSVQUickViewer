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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvTools.Tests
{
  [TestClass]
  public class ExtensionsTests
  {
    [TestMethod]
    public void WaitToCompleteTaskTestCompletedAlready()
    {
      var executed = false;

      var task = Task.Run<bool>(() => { executed = true; return true; });
      // give it time to finish task
      Thread.Sleep(50);
      Assert.IsTrue(executed);

      task.WaitToCompleteTaskUI(1);
      Assert.IsTrue(executed);
    }

    [TestMethod]
    public void WaitToCompleteTaskTestCompleteInTime()
    {
      var executed = false;
      Assert.IsFalse(executed);
      var task = Task.Run<bool>(() =>
      {
        Thread.Sleep(100);
        executed = true;
        return true;
      });
      Assert.IsFalse(executed);
      task.WaitToCompleteTaskUI(1);
      Assert.IsTrue(executed);
    }

    [TestMethod]
    public void WaitToCompleteTaskTestTimeout()
    {
      var task = Task.Run<bool>(() =>
      {
        Thread.Sleep(2000);
        return true;
      });

      try
      {
        task.WaitToCompleteTaskUI(1);
        Assert.Fail("Timeout did not occur");
      }
      catch (TimeoutException)
      {
      }
      catch (Exception ex)
      {
        Assert.Fail($"Wrong exception got {ex.GetType().Name} expected TimeoutException");
      }
    }

    [TestMethod]
    public void WaitToCompleteTaskCancel()
    {
      using (var cts = new CancellationTokenSource())
      {
        var task = Task.Run<bool>(() =>
        {
          Thread.Sleep(5000);
          return true;
        }, cts.Token);

        try
        {
          // Cancel Token after 200 ms in other thread
          var task2 = Task.Run(() =>
          {
            Thread.Sleep(200);
            cts.Cancel();
          }, cts.Token);
          task.WaitToCompleteTaskUI(2d, cts.Token);
          Assert.Fail("Timeout did not occur");
        }
        catch (AssertFailedException)
        {
          throw;
        }
        catch (OperationCanceledException)
        { }
        catch (Exception ex)
        {
          Assert.Fail($"Wrong exception got {ex.GetType().Name} expected OperationCanceledException : {ex.ExceptionMessages()}");
        }
      }
    }

    [TestMethod]
    public void WaitToCompleteWithException()
    {
      using (var cts = new CancellationTokenSource())
      {
        try
        {
          Task.Run<bool>(() =>
          {
            Thread.Sleep(100);
            var i = 0;
            if (i == 0)
              throw new ApplicationException("This is an error");
            else
              // Testing WaitToCompleteTask<T> I need a task that returns something
              return true;
          }).WaitToCompleteTaskUI(1.5d);

          Assert.Fail("no Exception did not occur");
        }
        catch (ApplicationException)
        {
          // all good
        }
        catch (Exception ex)
        {
          Assert.Fail($"Wrong exception got {ex.GetType().Name} expected ApplicationException");
        }
      }
    }

    [TestMethod]
    public void UpdateListViewColumnFormatTest()
    {
      using (var lv = new ListView())
      {
        var colFmt = new List<Column>();

        {
          var item = lv.Items.Add("Test");
          item.Selected = true;
        }
        lv.UpdateListViewColumnFormat(colFmt);
        Assert.AreEqual(0, lv.Items.Count);

        {
          lv.Items.Add("Test1");
          var item = lv.Items.Add("Test");
          item.Selected = true;
        }

        colFmt.Add(new Column { Name = "Test" });
        lv.UpdateListViewColumnFormat(colFmt);
      }
    }

    [TestMethod()]
    public void WriteBindingTest()
    {
      var obj = new DisplayItem<string>("15", "Text");
      using (var bindingSource = new BindingSource
      {
        DataSource = obj
      })
      {
        var bind = new Binding("Text", bindingSource, "ID", true);
        using (var textBoxBox = new TextBox())
        {
          textBoxBox.DataBindings.Add(bind);
          textBoxBox.Text = "12";

          Assert.AreEqual(bind, textBoxBox.GetTextBindng());
          textBoxBox.WriteBinding();
        }
      }
    }

    [TestMethod()]
    public void DeleteFileQuestionTest() => Assert.AreEqual(true, ".\\Test.hshsh".DeleteFileQuestion(false));

    [TestMethod()]
    public void GetProcessDisplayTest()
    {
      var setting = new CsvFile()
      {
        FileName = "Folder\\This is a long file name that should be cut and fit into 80 chars.txt",
        ShowProgress = true
      };
      using (var prc = setting.GetProcessDisplay(null, true, System.Threading.CancellationToken.None))
      {
        Assert.IsTrue(prc is IProcessDisplay, "GetProcessDisplay With Logger");
      }
      using (var prc = setting.GetProcessDisplay(null, false, System.Threading.CancellationToken.None))
      {
        Assert.IsTrue(prc is IProcessDisplay, "GetProcessDisplay Without Logger");
      }

      var setting2 = new CsvFile()
      {
        FileName = "Folder\\This is a long file name that should be cut and fit into 80 chars.txt",
        ShowProgress = false
      };

      using (var prc = setting2.GetProcessDisplay(null, false, System.Threading.CancellationToken.None))
      {
        Assert.IsTrue(prc is IProcessDisplay, "GetProcessDisplay without UI");
      }
    }

    [TestMethod()]
    public void LoadWindowStateTest()
    {
      using (var value = new FormProcessDisplay())
      {
        value.Show();
        var state = new WindowState(new System.Drawing.Rectangle(10, 10, 200, 200), FormWindowState.Normal)
        {
          CustomInt = 27,
          CustomText = "Test"
        };
        var result1 = -1;
        var result2 = "Hello";
        value.LoadWindowState(state, (val) => { result1 = val; }, (val) => { result2 = val; });
        Assert.AreEqual(state.CustomInt, result1);
        Assert.AreEqual(state.CustomText, result2);
      }
    }

    [TestMethod()]
    public void SafeBeginInvokeTest()
    {
    }

    [TestMethod()]
    public void SafeInvokeTest()
    {
    }

    [TestMethod()]
    public void SafeInvokeNoHandleNeededTest()
    {
    }

    [TestMethod()]
    public void StoreWindowStateTest()
    {
      using (var value = new FormProcessDisplay())
      {
        value.Show();
        var state1 = new WindowState(new System.Drawing.Rectangle(10, 10, value.Width, value.Height), FormWindowState.Normal)
        {
          CustomInt = 27,
          CustomText = "Test"
        };
        var result1 = -1;
        var result2 = "Hello";
        value.LoadWindowState(state1, (val) => { result1 = val; }, (val) => { result2 = val; });

        var state2 = value.StoreWindowState(result1, "World");
        Assert.AreEqual(state1.CustomInt, state2.CustomInt);
        Assert.AreEqual("World", state2.CustomText);
        Assert.AreEqual(state1.Left, state2.Left);
        Assert.AreEqual(state1.Width, state2.Width);
      }
    }

    [TestMethod()]
    public void UpdateListViewColumnFormatTest1()
    {
    }

    [TestMethod()]
    public void WriteFileWithInfoTest()
    {
    }
  }
}