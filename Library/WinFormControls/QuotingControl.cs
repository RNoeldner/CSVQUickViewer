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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;

namespace CsvTools
{
  /// <summary>
  ///   A Control to edit the quoting and visualize the result
  /// </summary>
  public class QuotingControl : UserControl
  {
    private CheckBox m_CheckBoxAlternateQuoting;
    private ComboBox m_ComboBoxTrim;

    /// <summary>
    ///   The components
    /// </summary>
    private IContainer components;

    private ErrorProvider m_ErrorProvider;
    private BindingSource m_FileFormatBindingSource;
    private BindingSource m_FileSettingBindingSource;
    private Label m_Label1;
    private Label m_Label2;
    private Label m_Label3;
    private Label m_Label5;
    private Label m_Label8;
    private Label m_LabelEscapeCharacter;
    private Label m_LabelInfoQuoting;
    private Label m_LabelQuote;
    private Label m_LabelQuotePlaceholer;
    private Label m_LabelTrim;
    private CsvFile m_CsvFile;
    private bool m_IsWriteSetting;
    private CSVRichTextBox m_RichTextBox;
    private CSVRichTextBox m_RichTextBox00;
    private CSVRichTextBox m_RichTextBox01;
    private CSVRichTextBox m_RichTextBox02;
    private CSVRichTextBox m_RichTextBox10;
    private CSVRichTextBox m_RichTextBox11;
    private CSVRichTextBox m_RichTextBox12;
    private TableLayoutPanel m_TableLayoutPanel1;
    private TextBox m_TextBoxEscape;
    private TextBox m_TextBoxQuote;
    private TextBox m_TextBoxQuotePlaceHolder;
    private ToolTip m_ToolTip;

    /// <summary>
    ///   CTOR of QuotingControl
    /// </summary>
    public QuotingControl()
    {
      InitializeComponent();
      m_ComboBoxTrim.Items.Add(new DisplayItem<int>(0, "None"));
      m_ComboBoxTrim.Items.Add(new DisplayItem<int>(1, "Unquoted"));
      m_ComboBoxTrim.Items.Add(new DisplayItem<int>(3, "All"));
    }

    /// <summary>
    ///   The CSV File
    /// </summary>
    public CsvFile CsvFile
    {
      get => m_CsvFile;

      set
      {
        m_CsvFile = value;
        if (m_CsvFile == null) return;
        m_FileSettingBindingSource.DataSource = m_CsvFile;
        m_FileFormatBindingSource.DataSource = m_CsvFile.FileFormat;
        m_CsvFile.FileFormat.PropertyChanged += m_CsvFile_PropertyChanged;
        SetCboTrim(m_CsvFile.TrimmingOption);
        SetDelimiter();
      }
    }

    /// <summary>
    ///   In case of a Write only setting things will be hidden
    /// </summary>
    public bool IsWriteSetting
    {
      get => m_IsWriteSetting;

      set
      {
        Contract.Assume(m_LabelInfoQuoting != null);
        Contract.Assume(m_ComboBoxTrim != null);
        Contract.Assume(m_CheckBoxAlternateQuoting != null);
        m_IsWriteSetting = value;
        m_LabelInfoQuoting.Visible = !value;
        m_ComboBoxTrim.Enabled = !value;
        m_CheckBoxAlternateQuoting.Visible = !value;
      }
    }

    #region

    /// <summary>
    ///   Initializes the component.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.m_LabelQuote = new System.Windows.Forms.Label();
      this.m_LabelQuotePlaceholer = new System.Windows.Forms.Label();
      this.m_TextBoxEscape = new System.Windows.Forms.TextBox();
      this.m_FileFormatBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_LabelEscapeCharacter = new System.Windows.Forms.Label();
      this.m_LabelTrim = new System.Windows.Forms.Label();
      this.m_TextBoxQuote = new System.Windows.Forms.TextBox();
      this.m_TextBoxQuotePlaceHolder = new System.Windows.Forms.TextBox();
      this.m_CheckBoxAlternateQuoting = new System.Windows.Forms.CheckBox();
      this.m_FileSettingBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_ToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.m_ComboBoxTrim = new System.Windows.Forms.ComboBox();
      this.m_TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.m_Label2 = new System.Windows.Forms.Label();
      this.m_RichTextBox02 = new CsvTools.CSVRichTextBox();
      this.m_RichTextBox01 = new CsvTools.CSVRichTextBox();
      this.m_RichTextBox00 = new CsvTools.CSVRichTextBox();
      this.m_RichTextBox10 = new CsvTools.CSVRichTextBox();
      this.m_RichTextBox11 = new CsvTools.CSVRichTextBox();
      this.m_RichTextBox12 = new CsvTools.CSVRichTextBox();
      this.m_Label1 = new System.Windows.Forms.Label();
      this.m_Label3 = new System.Windows.Forms.Label();
      this.m_LabelInfoQuoting = new System.Windows.Forms.Label();
      this.m_Label8 = new System.Windows.Forms.Label();
      this.m_ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.m_Label5 = new System.Windows.Forms.Label();
      this.m_RichTextBox = new CsvTools.CSVRichTextBox();
      ((System.ComponentModel.ISupportInitialize)(this.m_FileFormatBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_FileSettingBindingSource)).BeginInit();
      this.m_TableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider)).BeginInit();
      this.SuspendLayout();
      //
      // m_LabelQuote
      //
      this.m_LabelQuote.AutoSize = true;
      this.m_LabelQuote.Location = new System.Drawing.Point(77, 3);
      this.m_LabelQuote.Name = "m_LabelQuote";
      this.m_LabelQuote.Size = new System.Drawing.Size(39, 13);
      this.m_LabelQuote.TabIndex = 21;
      this.m_LabelQuote.Text = "Quote:";
      //
      // m_LabelQuotePlaceholer
      //
      this.m_LabelQuotePlaceholer.AutoSize = true;
      this.m_LabelQuotePlaceholer.Location = new System.Drawing.Point(50, 51);
      this.m_LabelQuotePlaceholer.Name = "m_LabelQuotePlaceholer";
      this.m_LabelQuotePlaceholer.Size = new System.Drawing.Size(66, 13);
      this.m_LabelQuotePlaceholer.TabIndex = 22;
      this.m_LabelQuotePlaceholer.Text = "Placeholder:";
      //
      // m_TextBoxEscape
      //
      this.m_TextBoxEscape.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_FileFormatBindingSource, "EscapeCharacter", true));
      this.m_TextBoxEscape.Location = new System.Drawing.Point(122, 24);
      this.m_TextBoxEscape.Name = "m_TextBoxEscape";
      this.m_TextBoxEscape.Size = new System.Drawing.Size(45, 20);
      this.m_TextBoxEscape.TabIndex = 34;
      this.m_ToolTip.SetToolTip(this.m_TextBoxEscape, "Character to escape an embedded quote, if left empty embedded quotes need to be d" +
        "uplicated");
      //
      // m_FileFormatBindingSource
      //
      this.m_FileFormatBindingSource.AllowNew = false;
      this.m_FileFormatBindingSource.DataSource = typeof(CsvTools.FileFormat);
      //
      // m_LabelEscapeCharacter
      //
      this.m_LabelEscapeCharacter.AutoSize = true;
      this.m_LabelEscapeCharacter.Location = new System.Drawing.Point(21, 27);
      this.m_LabelEscapeCharacter.Name = "m_LabelEscapeCharacter";
      this.m_LabelEscapeCharacter.Size = new System.Drawing.Size(95, 13);
      this.m_LabelEscapeCharacter.TabIndex = 33;
      this.m_LabelEscapeCharacter.Text = "Escape Character:";
      //
      // m_LabelTrim
      //
      this.m_LabelTrim.AutoSize = true;
      this.m_LabelTrim.Location = new System.Drawing.Point(30, 77);
      this.m_LabelTrim.Name = "m_LabelTrim";
      this.m_LabelTrim.Size = new System.Drawing.Size(86, 13);
      this.m_LabelTrim.TabIndex = 89;
      this.m_LabelTrim.Text = "Trimming Option:";
      //
      // m_TextBoxQuote
      //
      this.m_TextBoxQuote.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_FileFormatBindingSource, "FieldQualifier", true));
      this.m_TextBoxQuote.Location = new System.Drawing.Point(122, 1);
      this.m_TextBoxQuote.Name = "m_TextBoxQuote";
      this.m_TextBoxQuote.Size = new System.Drawing.Size(45, 20);
      this.m_TextBoxQuote.TabIndex = 19;
      this.m_ToolTip.SetToolTip(this.m_TextBoxQuote, "Columns may be delimited with a quoting character; the quotes are removed by the " +
        "reading applications.");
      //
      // m_TextBoxQuotePlaceHolder
      //
      this.m_TextBoxQuotePlaceHolder.AutoCompleteCustomSource.AddRange(new string[] {
            "{q}",
            "&quot;"});
      this.m_TextBoxQuotePlaceHolder.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.m_TextBoxQuotePlaceHolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_FileFormatBindingSource, "QuotePlaceholder", true));
      this.m_TextBoxQuotePlaceHolder.Location = new System.Drawing.Point(122, 48);
      this.m_TextBoxQuotePlaceHolder.Name = "m_TextBoxQuotePlaceHolder";
      this.m_TextBoxQuotePlaceHolder.Size = new System.Drawing.Size(45, 20);
      this.m_TextBoxQuotePlaceHolder.TabIndex = 20;
      this.m_ToolTip.SetToolTip(this.m_TextBoxQuotePlaceHolder, "If this placeholder is part of the text it will be replaced with the quoting char" +
        "acter");
      //
      // m_CheckBoxAlternateQuoting
      //
      this.m_CheckBoxAlternateQuoting.AutoSize = true;
      this.m_CheckBoxAlternateQuoting.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.m_FileSettingBindingSource, "AlternateQuoting", true));
      this.m_CheckBoxAlternateQuoting.Location = new System.Drawing.Point(236, 4);
      this.m_CheckBoxAlternateQuoting.Name = "m_CheckBoxAlternateQuoting";
      this.m_CheckBoxAlternateQuoting.Size = new System.Drawing.Size(148, 17);
      this.m_CheckBoxAlternateQuoting.TabIndex = 18;
      this.m_CheckBoxAlternateQuoting.Text = "Context Sensitive Quoting";
      this.m_ToolTip.SetToolTip(this.m_CheckBoxAlternateQuoting, "A quote is only regarded as closing quote if it is followed by linefeed or delimi" +
        "ter");
      this.m_CheckBoxAlternateQuoting.UseVisualStyleBackColor = true;
      //
      // m_FileSettingBindingSource
      //
      this.m_FileSettingBindingSource.AllowNew = false;
      this.m_FileSettingBindingSource.DataSource = typeof(CsvTools.CsvFile);
      //
      // m_ComboBoxTrim
      //
      this.m_ComboBoxTrim.DisplayMember = "Display";
      this.m_ComboBoxTrim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_ComboBoxTrim.Location = new System.Drawing.Point(122, 74);
      this.m_ComboBoxTrim.Name = "m_ComboBoxTrim";
      this.m_ComboBoxTrim.Size = new System.Drawing.Size(105, 21);
      this.m_ComboBoxTrim.TabIndex = 90;
      this.m_ToolTip.SetToolTip(this.m_ComboBoxTrim, "None will not remove whitespaces; Unquoted will remove whitespaces if the column " +
        "was not quoted; All will remove whitespaces even if the column was quoted");
      this.m_ComboBoxTrim.ValueMember = "ID";
      //
      // m_TableLayoutPanel1
      //
      this.m_TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window;
      this.m_TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
      this.m_TableLayoutPanel1.ColumnCount = 3;
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 17F));
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.88889F));
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.11111F));
      this.m_TableLayoutPanel1.Controls.Add(this.m_Label2, 0, 0);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox02, 1, 2);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox01, 1, 1);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox00, 1, 0);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox10, 2, 0);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox11, 2, 1);
      this.m_TableLayoutPanel1.Controls.Add(this.m_RichTextBox12, 2, 2);
      this.m_TableLayoutPanel1.Controls.Add(this.m_Label1, 0, 1);
      this.m_TableLayoutPanel1.Controls.Add(this.m_Label3, 0, 2);
      this.m_TableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_TableLayoutPanel1.Location = new System.Drawing.Point(313, 101);
      this.m_TableLayoutPanel1.Name = "m_TableLayoutPanel1";
      this.m_TableLayoutPanel1.RowCount = 3;
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.Size = new System.Drawing.Size(327, 83);
      this.m_TableLayoutPanel1.TabIndex = 92;
      //
      // m_Label2
      //
      this.m_Label2.AutoSize = true;
      this.m_Label2.ForeColor = System.Drawing.Color.Teal;
      this.m_Label2.Location = new System.Drawing.Point(5, 2);
      this.m_Label2.Name = "m_Label2";
      this.m_Label2.Size = new System.Drawing.Size(11, 18);
      this.m_Label2.TabIndex = 102;
      this.m_Label2.Text = "1\r\n";
      //
      // m_RichTextBox02
      //
      this.m_RichTextBox02.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox02.Delimiter = ',';
      this.m_RichTextBox02.DisplaySpace = true;
      this.m_RichTextBox02.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox02.Escape = '\\';
      this.m_RichTextBox02.Location = new System.Drawing.Point(21, 42);
      this.m_RichTextBox02.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox02.Name = "m_RichTextBox02";
      this.m_RichTextBox02.Quote = '\"';
      this.m_RichTextBox02.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox02.Size = new System.Drawing.Size(117, 39);
      this.m_RichTextBox02.TabIndex = 101;
      this.m_RichTextBox02.Text = "Example ";
      this.m_RichTextBox02.WordWrap = false;
      //
      // m_RichTextBox01
      //
      this.m_RichTextBox01.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox01.Delimiter = ',';
      this.m_RichTextBox01.DisplaySpace = true;
      this.m_RichTextBox01.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox01.Escape = '\\';
      this.m_RichTextBox01.Location = new System.Drawing.Point(21, 22);
      this.m_RichTextBox01.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox01.Name = "m_RichTextBox01";
      this.m_RichTextBox01.Quote = '\"';
      this.m_RichTextBox01.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox01.Size = new System.Drawing.Size(117, 18);
      this.m_RichTextBox01.TabIndex = 100;
      this.m_RichTextBox01.Text = " a Trimming ";
      this.m_RichTextBox01.WordWrap = false;
      //
      // m_RichTextBox00
      //
      this.m_RichTextBox00.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox00.Delimiter = ',';
      this.m_RichTextBox00.DisplaySpace = true;
      this.m_RichTextBox00.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox00.Escape = '\\';
      this.m_RichTextBox00.Location = new System.Drawing.Point(21, 2);
      this.m_RichTextBox00.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox00.Name = "m_RichTextBox00";
      this.m_RichTextBox00.Quote = '\"';
      this.m_RichTextBox00.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox00.Size = new System.Drawing.Size(117, 18);
      this.m_RichTextBox00.TabIndex = 99;
      this.m_RichTextBox00.Text = "This is ";
      this.m_RichTextBox00.WordWrap = false;
      //
      // m_RichTextBox10
      //
      this.m_RichTextBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox10.Delimiter = ',';
      this.m_RichTextBox10.DisplaySpace = true;
      this.m_RichTextBox10.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox10.Escape = '\\';
      this.m_RichTextBox10.Location = new System.Drawing.Point(140, 2);
      this.m_RichTextBox10.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox10.Name = "m_RichTextBox10";
      this.m_RichTextBox10.Quote = '\"';
      this.m_RichTextBox10.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox10.Size = new System.Drawing.Size(185, 18);
      this.m_RichTextBox10.TabIndex = 98;
      this.m_RichTextBox10.Text = "Column with:, Delimiter";
      this.m_RichTextBox10.WordWrap = false;
      //
      // m_RichTextBox11
      //
      this.m_RichTextBox11.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox11.Delimiter = ',';
      this.m_RichTextBox11.DisplaySpace = true;
      this.m_RichTextBox11.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox11.Escape = '\\';
      this.m_RichTextBox11.Location = new System.Drawing.Point(140, 22);
      this.m_RichTextBox11.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox11.Name = "m_RichTextBox11";
      this.m_RichTextBox11.Quote = '\"';
      this.m_RichTextBox11.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox11.Size = new System.Drawing.Size(185, 18);
      this.m_RichTextBox11.TabIndex = 99;
      this.m_RichTextBox11.Text = "Column with:\" Quote";
      this.m_RichTextBox11.WordWrap = false;
      //
      // m_RichTextBox12
      //
      this.m_RichTextBox12.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox12.Delimiter = ',';
      this.m_RichTextBox12.DisplaySpace = true;
      this.m_RichTextBox12.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_RichTextBox12.Escape = '\\';
      this.m_RichTextBox12.Location = new System.Drawing.Point(140, 42);
      this.m_RichTextBox12.Margin = new System.Windows.Forms.Padding(0);
      this.m_RichTextBox12.Name = "m_RichTextBox12";
      this.m_RichTextBox12.Quote = '\"';
      this.m_RichTextBox12.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox12.Size = new System.Drawing.Size(185, 39);
      this.m_RichTextBox12.TabIndex = 99;
      this.m_RichTextBox12.Text = "Column with \nLinefeed";
      this.m_RichTextBox12.WordWrap = false;
      //
      // m_Label1
      //
      this.m_Label1.AutoSize = true;
      this.m_Label1.ForeColor = System.Drawing.Color.Teal;
      this.m_Label1.Location = new System.Drawing.Point(5, 22);
      this.m_Label1.Name = "m_Label1";
      this.m_Label1.Size = new System.Drawing.Size(11, 18);
      this.m_Label1.TabIndex = 103;
      this.m_Label1.Text = "2";
      //
      // m_Label3
      //
      this.m_Label3.AutoSize = true;
      this.m_Label3.ForeColor = System.Drawing.Color.Teal;
      this.m_Label3.Location = new System.Drawing.Point(5, 42);
      this.m_Label3.Name = "m_Label3";
      this.m_Label3.Size = new System.Drawing.Size(11, 18);
      this.m_Label3.TabIndex = 103;
      this.m_Label3.Text = "3";
      //
      // m_LabelInfoQuoting
      //
      this.m_LabelInfoQuoting.AutoSize = true;
      this.m_LabelInfoQuoting.BackColor = System.Drawing.SystemColors.Info;
      this.m_LabelInfoQuoting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_LabelInfoQuoting.ForeColor = System.Drawing.SystemColors.InfoText;
      this.m_LabelInfoQuoting.Location = new System.Drawing.Point(233, 77);
      this.m_LabelInfoQuoting.Name = "m_LabelInfoQuoting";
      this.m_LabelInfoQuoting.Size = new System.Drawing.Size(225, 15);
      this.m_LabelInfoQuoting.TabIndex = 93;
      this.m_LabelInfoQuoting.Text = "Not possible to have leading or trailing spaces";
      this.m_LabelInfoQuoting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      //
      // m_Label8
      //
      this.m_Label8.AutoSize = true;
      this.m_Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_Label8.ForeColor = System.Drawing.Color.Teal;
      this.m_Label8.Location = new System.Drawing.Point(-4, 101);
      this.m_Label8.Name = "m_Label8";
      this.m_Label8.Size = new System.Drawing.Size(18, 80);
      this.m_Label8.TabIndex = 95;
      this.m_Label8.Text = "1\r\n2\r\n3\r\n4";
      //
      // m_ErrorProvider
      //
      this.m_ErrorProvider.ContainerControl = this;
      //
      // m_Label5
      //
      this.m_Label5.AutoSize = true;
      this.m_Label5.Location = new System.Drawing.Point(15, 188);
      this.m_Label5.Name = "m_Label5";
      this.m_Label5.Size = new System.Drawing.Size(336, 13);
      this.m_Label5.TabIndex = 98;
      this.m_Label5.Text = "Tab visualized as »   Linefeed visualized as ¶    Space visualized as ●";
      //
      // m_RichTextBox
      //
      this.m_RichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_RichTextBox.Delimiter = ';';
      this.m_RichTextBox.DisplaySpace = true;
      this.m_RichTextBox.Escape = '>';
      this.m_RichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_RichTextBox.Location = new System.Drawing.Point(18, 101);
      this.m_RichTextBox.Name = "m_RichTextBox";
      this.m_RichTextBox.Quote = '\"';
      this.m_RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.m_RichTextBox.Size = new System.Drawing.Size(294, 83);
      this.m_RichTextBox.TabIndex = 97;
      this.m_RichTextBox.Text = "\"This is \";Column with:, Delimiter\n a Trimming ;Column with \"\" Quote\nExample ;\"Co" +
    "lumn with \nLinefeed\"";
      this.m_RichTextBox.WordWrap = false;
      //
      // QuotingControl
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_Label5);
      this.Controls.Add(this.m_RichTextBox);
      this.Controls.Add(this.m_Label8);
      this.Controls.Add(this.m_LabelInfoQuoting);
      this.Controls.Add(this.m_TableLayoutPanel1);
      this.Controls.Add(this.m_ComboBoxTrim);
      this.Controls.Add(this.m_LabelTrim);
      this.Controls.Add(this.m_LabelEscapeCharacter);
      this.Controls.Add(this.m_TextBoxEscape);
      this.Controls.Add(this.m_TextBoxQuote);
      this.Controls.Add(this.m_LabelQuote);
      this.Controls.Add(this.m_LabelQuotePlaceholer);
      this.Controls.Add(this.m_TextBoxQuotePlaceHolder);
      this.Controls.Add(this.m_CheckBoxAlternateQuoting);
      this.Name = "QuotingControl";
      this.Size = new System.Drawing.Size(640, 210);
      ((System.ComponentModel.ISupportInitialize)(this.m_FileFormatBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_FileSettingBindingSource)).EndInit();
      this.m_TableLayoutPanel1.ResumeLayout(false);
      this.m_TableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion Vom Komponenten-Designer generierter Code

    /// <summary>
    ///   Dispose
    /// </summary>
    /// <param name="disposing">
    ///   true to release both managed and unmanaged resources; false to release only unmanaged
    ///   resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) components?.Dispose();
      base.Dispose(disposing);
    }

    private void comboBoxTrim_SelectedIndexChanged(object sender, EventArgs e)
    {
      Contract.Requires(m_ComboBoxTrim != null);
      if (m_ComboBoxTrim.SelectedItem == null) return;
      m_RichTextBox00.Clear();
      m_RichTextBox01.Clear();
      m_RichTextBox02.Clear();
      switch (((DisplayItem<int>)m_ComboBoxTrim.SelectedItem).ID)
      {
        case 1:
          m_CsvFile.TrimmingOption = TrimmingOption.Unquoted;
          m_RichTextBox00.SelectionColor = Color.Black;
          m_RichTextBox00.Text = "This is ";
          m_RichTextBox00.Select(m_RichTextBox00.TextLength - 1, 1);
          m_RichTextBox00.SelectionBackColor = Color.Yellow;

          m_RichTextBox01.Text = " a Trimming ";
          m_RichTextBox01.Select(0, 1);
          m_RichTextBox01.SelectionBackColor = Color.Yellow;
          m_RichTextBox01.SelectionColor = Color.Orange;
          m_RichTextBox01.Select(m_RichTextBox01.TextLength - 1, 1);
          m_RichTextBox01.SelectionBackColor = Color.Yellow;
          m_RichTextBox02.Text = "Example";
          m_LabelInfoQuoting.Text = "Import of leading or training spaces possible, but the field has to be quoted";
          break;

        case 3:
          m_CsvFile.TrimmingOption = TrimmingOption.All;
          m_RichTextBox00.Text = "This is";
          m_RichTextBox01.Text = "a Trimming";
          m_RichTextBox02.Text = "Example";
          m_LabelInfoQuoting.Text = "Not possible to have leading nor trailing spaces";
          break;

        default:
          m_CsvFile.TrimmingOption = TrimmingOption.None;
          m_RichTextBox00.Text = "This is ";
          m_RichTextBox00.Select(m_RichTextBox00.TextLength - 1, 1);
          m_RichTextBox00.SelectionBackColor = Color.Yellow;

          m_RichTextBox01.Text = " a Trimming ";
          m_RichTextBox01.Select(0, 1);
          m_RichTextBox01.SelectionBackColor = Color.Yellow;
          m_RichTextBox01.SelectionColor = Color.Orange;
          m_RichTextBox01.Select(m_RichTextBox01.TextLength - 1, 1);
          m_RichTextBox01.SelectionBackColor = Color.Yellow;

          m_RichTextBox02.Text = "Example ";
          m_RichTextBox02.Select(m_RichTextBox02.TextLength - 1, 1);
          m_RichTextBox02.SelectionBackColor = Color.Yellow;

          m_LabelInfoQuoting.Text = "Leading or training spaces will stay as they are";
          break;
      }
    }

    private void label2_Click(object sender, EventArgs e)
    {
    }

    private void m_CsvFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(FileFormat.FieldDelimiter)) SetDelimiter();
    }

    private void SetCboTrim(TrimmingOption trim)
    {
      foreach (var ite in m_ComboBoxTrim.Items)
      {
        var item = (DisplayItem<int>)ite;
        if (item.ID == 0 && trim == TrimmingOption.None)
        {
          m_ComboBoxTrim.SelectedItem = ite;
          break;
        }

        if (item.ID == 1 && trim == TrimmingOption.Unquoted)
        {
          m_ComboBoxTrim.SelectedItem = ite;
          break;
        }

        if (item.ID != 3 || trim != TrimmingOption.All) continue;
        m_ComboBoxTrim.SelectedItem = ite;
        break;
      }
    }

    private void SetDelimiter()
    {
      try
      {
        var delimiter = m_CsvFile.FileFormat.FieldDelimiterChar;
        m_RichTextBox.Delimiter = delimiter;
        SetToolTipPlaceholder(null, null);
      }
      catch
      {
        // ignore
      }
    }

    private void SetToolTipPlaceholder(object sender, EventArgs e)
    {
      this.SafeInvoke(() =>
      {
        m_ErrorProvider.SetError(m_TableLayoutPanel1, "");
        m_ErrorProvider.SetError(m_TextBoxQuote, "");

        if (string.IsNullOrEmpty(m_TextBoxQuote.Text))
          m_ErrorProvider.SetError(m_TableLayoutPanel1, "Without quoting, the delimiter can not be part of a field");
        var quote = FileFormat.GetChar(m_TextBoxQuote.Text).ToString();

        if (quote != "\0" && quote != "'" && quote != "\"")
          m_ErrorProvider.SetError(m_TextBoxQuote, "Unusual Quoting character");

        if (m_RichTextBox.Delimiter == quote[0])
          m_ErrorProvider.SetError(m_TextBoxQuote, "Delimiter and Quote have to be different");

        if (quote == "\0")
          quote = null;

        if (quote == null)
        {
          m_RichTextBox10.Text = "<Not possible>";
          m_RichTextBox10.SelectAll();
          m_RichTextBox10.SelectionColor = Color.Red;

          m_RichTextBox12.Text = "<Not possible>";
          m_RichTextBox12.SelectAll();
          m_RichTextBox12.SelectionColor = Color.Red;
        }
        else
        {
          m_RichTextBox10.Text = "Column with:" + m_RichTextBox.Delimiter + " Delimiter";
          m_RichTextBox12.Text = "Column with \nLinefeed";
        }

        m_RichTextBox11.Text = "Column with:" + quote + " Quote";
        // richTextBox11.Quote = quote[0];

        var newToolTip = m_IsWriteSetting
      ? "Start the column with a quote, if a quote is part of the text the quote is replaced with a placeholder."
      : "If the placeholder is part of the text it will be replaced with the quoting character.";

        var sampleText = quote + "This is " + quote + m_RichTextBox.Delimiter + quote + "Column with:" +
                         m_RichTextBox.Delimiter + " Delimiter" + quote + "\r\n" +
                         quote + " a Trimming " + quote + m_RichTextBox.Delimiter + quote + "Column with:{*} Quote" +
                         quote + "\r\n" +
                         "Example " + m_RichTextBox.Delimiter + quote + "Column with \r\nLinefeed" + quote;

        if (!string.IsNullOrEmpty(m_TextBoxQuotePlaceHolder.Text) && !string.IsNullOrEmpty(quote))
        {
          newToolTip += m_IsWriteSetting
            ? $"\r\nhello {quote} world ->{quote}hello {m_TextBoxQuotePlaceHolder.Text} world{quote}"
            : $"\r\n{quote}hello {m_TextBoxQuotePlaceHolder.Text} world{quote} -> hello {quote} world";

          sampleText = sampleText.Replace("{*}", m_TextBoxQuotePlaceHolder.Text);
        }

        if (m_CheckBoxAlternateQuoting.Checked)
        {
          sampleText = sampleText.Replace("{*}", quote);
        }
        else
        {
          sampleText = string.IsNullOrEmpty(m_TextBoxEscape.Text) ? sampleText.Replace("{*}", quote + quote) : sampleText.Replace("{*}", m_TextBoxEscape.Text + quote);
        }

        m_RichTextBox.Text = sampleText;

        m_RichTextBox.Quote = quote?[0] ?? '\0';

        m_ToolTip.SetToolTip(m_TextBoxQuotePlaceHolder, newToolTip);
      });
    }
  }
}