using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace CsvTools.Tests
{
  [TestClass]
	public class ProcessDisplayTimeTest
	{
		[TestMethod]
		public void CancellationOutside()
		{
			var cts = new CancellationTokenSource();
			var test = new ProcessDisplayTime(cts.Token);
			Assert.IsFalse(test.CancellationToken.IsCancellationRequested);
			cts.Cancel();
			Assert.IsTrue(cts.IsCancellationRequested);
			Assert.IsTrue(test.CancellationToken.IsCancellationRequested);
		}

		[TestMethod]
		[Timeout(10000)]
		public void MeasureTimeToCompletion()
		{
			var test = new ProcessDisplayTime(UnitTestInitializeCsv.Token) { Maximum = 100 };

			for (long counter = 1; counter <= 20; counter++)
			{
				test.SetProcess(counter.ToString(), counter, true);
				Thread.Sleep(100);
			}

			Assert.AreEqual(20, test.TimeToCompletion.Percent);
			// 20 * 100ms = 2s for 100 we need 10s as 2s are passed it should be 8s
			var est = test.TimeToCompletion.EstimatedTimeRemaining.TotalSeconds;
			Assert.IsTrue(est > 7 && est < 12, $"{est}s {test.TimeToCompletion.EstimatedTimeRemainingDisplay}");
		}

		[TestMethod]
		public void Properties()
		{
			var test = new ProcessDisplayTime(UnitTestInitializeCsv.Token) { Maximum = 5 };
			Assert.AreEqual(5, test.Maximum);
			test.Maximum = 100;
			Assert.AreEqual(100, test.Maximum);

			test.Title = "Hello";
			Assert.AreEqual("Hello", test.Title);
			test.Title = "";
			Assert.AreEqual("", test.Title);
		}
	}
}