﻿namespace CsvTools
{
  sealed partial class FormMain
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      if (m_CancellationTokenSource != null)
        m_CancellationTokenSource.Dispose();
      Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
      Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.fileSystemWatcher = new System.IO.FileSystemWatcher();
      this.textBoxProgress = new CsvTools.LoggerDisplay();
      this.textPanel = new System.Windows.Forms.Panel();
      this.buttonCloseText = new System.Windows.Forms.Button();
      this.csvTextDisplay = new CsvTools.CsvTextDisplay();
      this.detailControl = new CsvTools.DetailControl();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
      this.textPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // fileSystemWatcher
      // 
      this.fileSystemWatcher.EnableRaisingEvents = true;
      this.fileSystemWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.Size | System.IO.NotifyFilters.LastWrite)));
      this.fileSystemWatcher.SynchronizingObject = this;
      this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.FileSystemWatcher_Changed);
      // 
      // textBoxProgress
      // 
      this.textBoxProgress.BackColor = System.Drawing.SystemColors.Window;
      this.textBoxProgress.CausesValidation = false;
      this.textBoxProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxProgress.Location = new System.Drawing.Point(3, 3);
      this.textBoxProgress.Name = "textBoxProgress";
      this.textBoxProgress.ReadOnly = true;
      this.textBoxProgress.Size = new System.Drawing.Size(133, 160);
      this.textBoxProgress.TabIndex = 2;
      this.textBoxProgress.Text = "";
      this.textBoxProgress.Threshold = ((log4net.Core.Level)(resources.GetObject("textBoxProgress.Threshold")));
      // 
      // textPanel
      // 
      this.textPanel.Controls.Add(this.buttonCloseText);
      this.textPanel.Controls.Add(this.textBoxProgress);
      this.textPanel.Controls.Add(this.csvTextDisplay);
      this.textPanel.Location = new System.Drawing.Point(9, 40);
      this.textPanel.Margin = new System.Windows.Forms.Padding(0);
      this.textPanel.Name = "textPanel";
      this.textPanel.Size = new System.Drawing.Size(415, 180);
      this.textPanel.TabIndex = 4;
      this.textPanel.Visible = false;
      // 
      // buttonCloseText
      // 
      this.buttonCloseText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCloseText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonCloseText.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.buttonCloseText.Location = new System.Drawing.Point(335, 138);
      this.buttonCloseText.Name = "buttonCloseText";
      this.buttonCloseText.Size = new System.Drawing.Size(58, 23);
      this.buttonCloseText.TabIndex = 4;
      this.buttonCloseText.Text = "&Close";
      this.buttonCloseText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonCloseText.UseVisualStyleBackColor = true;
      this.buttonCloseText.Click += new System.EventHandler(this.ShowGrid);
      // 
      // csvTextDisplay
      // 
      this.csvTextDisplay.Location = new System.Drawing.Point(142, 3);
      this.csvTextDisplay.Name = "csvTextDisplay";
      this.csvTextDisplay.Size = new System.Drawing.Size(197, 177);
      this.csvTextDisplay.TabIndex = 5;
      // 
      // detailControl
      // 
      dataGridViewCellStyle3.BackColor = System.Drawing.Color.Gainsboro;
      this.detailControl.AlternatingRowDefaultCellSyle = dataGridViewCellStyle3;
      this.detailControl.DataTable = null;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.detailControl.DefaultCellStyle = dataGridViewCellStyle4;
      this.detailControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailControl.Location = new System.Drawing.Point(0, 0);
      this.detailControl.Name = "detailControl";
      this.detailControl.Size = new System.Drawing.Size(592, 368);
      this.detailControl.TabIndex = 1;
      this.detailControl.ButtonShowSource += new System.EventHandler(this.DetailControl_ButtonShowSource);
      this.detailControl.OnSettingsClick += new System.EventHandler(this.ShowSettings);
      // 
      // FormMain
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(592, 368);
      this.Controls.Add(this.detailControl);
      this.Controls.Add(this.textPanel);
      this.HelpButton = true;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MinimumSize = new System.Drawing.Size(600, 140);
      this.Name = "FormMain";
      this.Activated += new System.EventHandler(this.Display_Activated);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Display_FormClosing);
      this.Shown += new System.EventHandler(this.Display_Shown);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DataGridView_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DataGridView_DragEnter);
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
      this.textPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.IO.FileSystemWatcher fileSystemWatcher;
    private DetailControl detailControl;
    private LoggerDisplay textBoxProgress;
    
    private System.Windows.Forms.Panel textPanel;
    private System.Windows.Forms.Button buttonCloseText;
    private CsvTextDisplay csvTextDisplay;    



  }
}

