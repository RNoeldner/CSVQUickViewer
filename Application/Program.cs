/*
 * Copyright (C) 2014 Raphael Nöldner : http://csvquickviewer.com/
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

namespace CsvTools
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Threading;
  using System.Windows.Forms;

#if NETCOREAPP3_1
  using System.Text;
#endif

  internal static class Program
  {
    static Program()
    {
      try
      {
#if NETCOREAPP3_1
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#else
        CosturaUtility.Initialize();
#endif
      }
      catch (Exception ex)
      {
        UnhandledException(ex);
      }
    }

    /// <summary>
    ///   Handles the ThreadException event of the Application control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///   The <see cref="ThreadExceptionEventArgs" /> instance containing the event data.
    /// </param>
    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) =>
      UnhandledException(e.Exception);

    /// <summary>
    ///   Handles the UnhandledException event of the CurrentDomain control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///   The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.
    /// </param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
      UnhandledException((Exception) e.ExceptionObject);

    /// <summary>
    ///   The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
      Application.ThreadException += Application_ThreadException;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      var fileName = string.Empty;
#if NETCOREAPP3_1
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // read the command line parameter
      if (args.Length == 1)
        fileName = args[0];

      // in case we have multiple arguments assume the path was split at space
      else if (args.Length > 1)
        fileName = args.Join(" ");

      var m_ViewSettings = ViewSettingHelper.LoadViewSettings();

      if (string.IsNullOrEmpty(fileName) || !FileSystemUtils.FileExists(fileName))
      {

        var strFilter = "Common types|*.csv;*.txt;*.tab;*.log;*.tsv;*.dat;*.json;*.gz;*.zip|"
                      + "Delimited files (*.csv;*.txt;*.tab;*.tsv;*.dat;*.log)|*.csv;*.txt;*.tab;*.tsv;*.dat;*.log|";

        if (m_ViewSettings.StoreSettingsByFile)
          strFilter += "Setting files (*" + CsvFile.cCsvSettingExtension + ")|*" + CsvFile.cCsvSettingExtension + "|";

        strFilter +=   "Json files (*.json)|*.json|"
                     + "Compressed files (*.gz;*.zip)|*.gz;*.zip|"
                     + "All files (*.*)|*.*";

        fileName = WindowsAPICodePackWrapper.Open(".", "File to Display", strFilter, null);
      }
      FunctionalDI.SignalBackground = Application.DoEvents;
      var frm = new FormMain(m_ViewSettings);
      frm.Show();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      frm.LoadCsvFile(fileName);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

      Application.Run(frm);      
    }

    /// <summary>
    ///   Handle's any not yet handled exception
    /// </summary>
    /// <param name="ex">The exception.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void UnhandledException(Exception ex)
    {
      // Most likely disposing something which is still being used by a different thread its very
      // hard to track down as the stackframe is not useful, in 99% its updating progress or UI
      if (ex is ObjectDisposedException && ex.Message == "Safe handle has been closed")
      {
        // Logger.Warning(ex, "UnhandledException of type ObjectDisposedException is ignored");
        return;
      }

      Logger.Error(ex, "Not handled Exception");
      var message = $"{ex.GetType()}\n\n{ex.ExceptionMessages()}\nStack Trace:\n{ex.CsvToolsStackTrace()}";
#if DEBUG
      System.Diagnostics.Debug.Assert(false, @"Not handled Exception", message);
#else
      if (MessageBox.Show(message, @"Not handled Exception", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop)
          == DialogResult.Abort)
        Application.Exit();
#endif
    }
  }
}