﻿namespace CsvTools
{
  partial class TimeZoneSelector
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
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.comboBoxTimeZoneID = new System.Windows.Forms.ComboBox();
      this.buttonLocalTZ = new System.Windows.Forms.Button();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 2;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel.Controls.Add(this.comboBoxTimeZoneID, 0, 0);
      this.tableLayoutPanel.Controls.Add(this.buttonLocalTZ, 1, 0);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel.Size = new System.Drawing.Size(686, 40);
      this.tableLayoutPanel.TabIndex = 30;
      // 
      // comboBoxTimeZoneID
      // 
      this.comboBoxTimeZoneID.Dock = System.Windows.Forms.DockStyle.Top;
      this.comboBoxTimeZoneID.FormattingEnabled = true;
      this.comboBoxTimeZoneID.Location = new System.Drawing.Point(0, 4);
      this.comboBoxTimeZoneID.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
      this.comboBoxTimeZoneID.Name = "comboBoxTimeZoneID";
      this.comboBoxTimeZoneID.Size = new System.Drawing.Size(580, 28);
      this.comboBoxTimeZoneID.TabIndex = 28;
      this.comboBoxTimeZoneID.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTimeZoneID_SelectedIndexChanged);
      // 
      // buttonLocalTZ
      // 
      this.buttonLocalTZ.AutoSize = true;
      this.buttonLocalTZ.Location = new System.Drawing.Point(582, 2);
      this.buttonLocalTZ.Margin = new System.Windows.Forms.Padding(2);
      this.buttonLocalTZ.Name = "buttonLocalTZ";
      this.buttonLocalTZ.Size = new System.Drawing.Size(102, 34);
      this.buttonLocalTZ.TabIndex = 29;
      this.buttonLocalTZ.Text = "Local";
      this.buttonLocalTZ.UseVisualStyleBackColor = true;
      this.buttonLocalTZ.Click += new System.EventHandler(this.ButtonLocalTZ_Click);
      // 
      // TimeZoneSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel);
      this.Name = "TimeZoneSelector";
      this.Size = new System.Drawing.Size(686, 40);
      this.Load += new System.EventHandler(this.TimeZoneSelector_Load);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button buttonLocalTZ;
    private System.Windows.Forms.ComboBox comboBoxTimeZoneID;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
  }
}
