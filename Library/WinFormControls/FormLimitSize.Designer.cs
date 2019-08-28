﻿namespace CsvTools
{
  partial class FrmLimitSize
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label label2;
      System.Windows.Forms.Label label4;
      this.labelCount2 = new System.Windows.Forms.Label();
      this.labelCount3 = new System.Windows.Forms.Label();
      this.trackBarLimit = new System.Windows.Forms.TrackBar();
      this.label5 = new System.Windows.Forms.Label();
      this.labelCount1 = new System.Windows.Forms.Label();
      this.labelCount4 = new System.Windows.Forms.Label();
      this.timer = new System.Windows.Forms.Timer(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label = new System.Windows.Forms.Label();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonOK = new System.Windows.Forms.Button();
      label2 = new System.Windows.Forms.Label();
      label4 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.trackBarLimit)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label2
      // 
      label2.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(label2, 6);
      label2.Location = new System.Drawing.Point(3, 24);
      label2.Name = "label2";
      label2.Size = new System.Drawing.Size(478, 24);
      label2.TabIndex = 2;
      label2.Text = "Please move the slider to restrict the number of records to view.\r\n";
      label2.UseCompatibleTextRendering = true;
      // 
      // label4
      // 
      label4.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(label4, 6);
      label4.Location = new System.Drawing.Point(3, 0);
      label4.Name = "label4";
      label4.Size = new System.Drawing.Size(327, 24);
      label4.TabIndex = 1;
      label4.Text = "Display high amounts of data might be slow.";
      label4.UseCompatibleTextRendering = true;
      // 
      // labelCount2
      // 
      this.labelCount2.AutoSize = true;
      this.labelCount2.Location = new System.Drawing.Point(112, 102);
      this.labelCount2.Name = "labelCount2";
      this.labelCount2.Size = new System.Drawing.Size(58, 20);
      this.labelCount2.TabIndex = 5;
      this.labelCount2.Text = "20.000";
      this.labelCount2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // labelCount3
      // 
      this.labelCount3.AutoSize = true;
      this.labelCount3.Location = new System.Drawing.Point(225, 102);
      this.labelCount3.Name = "labelCount3";
      this.labelCount3.Size = new System.Drawing.Size(58, 20);
      this.labelCount3.TabIndex = 6;
      this.labelCount3.Text = "50.000";
      this.labelCount3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // trackBarLimit
      // 
      this.trackBarLimit.AutoSize = false;
      this.tableLayoutPanel1.SetColumnSpan(this.trackBarLimit, 5);
      this.trackBarLimit.Dock = System.Windows.Forms.DockStyle.Top;
      this.trackBarLimit.LargeChange = 1;
      this.trackBarLimit.Location = new System.Drawing.Point(3, 51);
      this.trackBarLimit.Maximum = 5;
      this.trackBarLimit.Minimum = 1;
      this.trackBarLimit.Name = "trackBarLimit";
      this.trackBarLimit.Size = new System.Drawing.Size(485, 48);
      this.trackBarLimit.TabIndex = 2;
      this.trackBarLimit.Value = 3;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label5, 2);
      this.label5.Location = new System.Drawing.Point(463, 102);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(26, 20);
      this.label5.TabIndex = 7;
      this.label5.Text = "All";
      // 
      // labelCount1
      // 
      this.labelCount1.AutoSize = true;
      this.labelCount1.Location = new System.Drawing.Point(3, 102);
      this.labelCount1.Name = "labelCount1";
      this.labelCount1.Size = new System.Drawing.Size(58, 20);
      this.labelCount1.TabIndex = 4;
      this.labelCount1.Text = "10.000";
      this.labelCount1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // labelCount4
      // 
      this.labelCount4.AutoSize = true;
      this.labelCount4.Location = new System.Drawing.Point(341, 102);
      this.labelCount4.Name = "labelCount4";
      this.labelCount4.Size = new System.Drawing.Size(67, 20);
      this.labelCount4.TabIndex = 10;
      this.labelCount4.Text = "100.000";
      this.labelCount4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // timer
      // 
      this.timer.Enabled = true;
      this.timer.Interval = 500;
      this.timer.Tick += new System.EventHandler(this.Timer_Tick);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 6;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.1831F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.00469F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.59155F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.76191F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.476191F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(label4, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label5, 4, 3);
      this.tableLayoutPanel1.Controls.Add(this.labelCount4, 3, 3);
      this.tableLayoutPanel1.Controls.Add(label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.labelCount1, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.labelCount3, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.trackBarLimit, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.labelCount2, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.label, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 5, 4);
      this.tableLayoutPanel1.Controls.Add(this.buttonOK, 3, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(602, 178);
      this.tableLayoutPanel1.TabIndex = 12;
      // 
      // label
      // 
      this.label.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label, 3);
      this.label.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
      this.label.Location = new System.Drawing.Point(3, 148);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(154, 20);
      this.label.TabIndex = 11;
      this.label.Text = "Default in 5 seconds";
      // 
      // buttonCancel
      // 
      this.buttonCancel.AutoSize = true;
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Location = new System.Drawing.Point(494, 141);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(102, 34);
      this.buttonCancel.TabIndex = 8;
      this.buttonCancel.Text = "&Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // buttonOK
      // 
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.buttonOK, 2);
      this.buttonOK.Location = new System.Drawing.Point(401, 141);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(87, 34);
      this.buttonOK.TabIndex = 9;
      this.buttonOK.Text = "&OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
      // 
      // FrmLimitSize
      // 
      this.AcceptButton = this.buttonOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(602, 178);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "FrmLimitSize";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Limit Records";
      this.TopMost = true;
      ((System.ComponentModel.ISupportInitialize)(this.trackBarLimit)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TrackBar trackBarLimit;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label labelCount2;
    private System.Windows.Forms.Label labelCount3;
    private System.Windows.Forms.Label labelCount1;
    private System.Windows.Forms.Label labelCount4;
    private System.Windows.Forms.Timer timer;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonOK;
  }
}