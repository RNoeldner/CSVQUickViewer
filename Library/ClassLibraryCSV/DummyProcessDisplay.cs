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
using System.Threading;

namespace CsvTools
{
  public class DummyProcessDisplay : IProcessDisplay
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private readonly CancellationTokenSource m_CancellationTokenSource;

    public DummyProcessDisplay() : this(CancellationToken.None)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DummyProcessDisplay" /> class.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public DummyProcessDisplay(CancellationToken cancellationToken)
    {
      if (cancellationToken == CancellationToken.None || cancellationToken.IsCancellationRequested)
        m_CancellationTokenSource = new CancellationTokenSource();
      else
        m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    /// <summary>
    ///   Event handler called as progress should be displayed
    /// </summary>
    public virtual event EventHandler<ProgressEventArgs> Progress;

    /// <summary>
    ///   Gets or sets the cancellation token.
    /// </summary>
    /// <value>
    ///   The cancellation token.
    /// </value>
    public CancellationToken CancellationToken => m_CancellationTokenSource.Token;

    public bool LogAsDebug { get; set; } = true;
    /// <summary>
    ///   Gets or sets the maximum value for the Progress
    /// </summary>
    /// <value>
    ///   The maximum value.
    /// </value>
    public virtual long Maximum { get; set; }

    public virtual string Title { get; set; }
    public static void Show()
    {
    }

    /// <summary>
    ///   To be called if the process should be closed, this will cancel any processing
    /// </summary>
    public virtual void Cancel()
    {
      if (!m_CancellationTokenSource.IsCancellationRequested)
        m_CancellationTokenSource.Cancel();
    }

    /// <summary>
    ///   Sets the process.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="value">The value.</param>
    public virtual void SetProcess(string text, long value, bool log = true)
    {
      if (log)
      {
        if (LogAsDebug)
          Log.Debug(text);
        else
          Log.Info(text);
      }
      Progress?.Invoke(this, new ProgressEventArgs(text, value, log));
    }

    /// <summary>
    ///   Sets the process.
    /// </summary>    
    public void SetProcess(string text) => SetProcess(text ?? string.Empty, -1, true);

    public void SetProcess(string text, long value) => SetProcess(text ?? string.Empty, value, true);

    /// <summary>
    ///   Set the progress used by Events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void SetProcess(object sender, ProgressEventArgs e)
    {
      if (e == null) return;
      SetProcess(e.Text ?? string.Empty, e.Value, e.Log);
    }
    #region IDisposable Support

    private bool m_DisposedValue; // To detect redundant calls

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      Cancel();
      if (m_DisposedValue) return;
      if (disposing)
      {
        m_CancellationTokenSource.Cancel();
        m_CancellationTokenSource.Dispose();
      }

      m_DisposedValue = true;
    }

    #endregion IDisposable Support
  }
}