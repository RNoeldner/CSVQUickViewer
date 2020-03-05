﻿namespace CsvTools
{
  partial class FillGuessSettingEdit
  {
    /// <summary>
    /// The components
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>Dispose
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.trackBarCheckedRecords = new System.Windows.Forms.TrackBar();
      this.textBoxCheckedRecords = new System.Windows.Forms.TextBox();
      this.textBoxSampleValues = new System.Windows.Forms.TextBox();
      this.checkBoxDectectNumbers = new System.Windows.Forms.CheckBox();
      this.label21 = new System.Windows.Forms.Label();
      this.label22 = new System.Windows.Forms.Label();
      this.checkBoxDectectPercentage = new System.Windows.Forms.CheckBox();
      this.checkBoxDetectDateTime = new System.Windows.Forms.CheckBox();
      this.label23 = new System.Windows.Forms.Label();
      this.label30 = new System.Windows.Forms.Label();
      this.checkBoxDetectBoolean = new System.Windows.Forms.CheckBox();
      this.textBoxTrue = new System.Windows.Forms.TextBox();
      this.textBoxFalse = new System.Windows.Forms.TextBox();
      this.checkBoxSerialDateTime = new System.Windows.Forms.CheckBox();
      this.label32 = new System.Windows.Forms.Label();
      this.checkBoxDetectGUID = new System.Windows.Forms.CheckBox();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.label20 = new System.Windows.Forms.Label();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.textBoxMinSamples = new System.Windows.Forms.TextBox();
      this.checkBoxNamedDates = new System.Windows.Forms.CheckBox();
      this.checkBoxDateParts = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.radioButtonEnabled = new System.Windows.Forms.RadioButton();
      this.radioButtonDisabled = new System.Windows.Forms.RadioButton();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.fillGuessSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.trackBarCheckedRecords)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.fillGuessSettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // trackBarCheckedRecords
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.trackBarCheckedRecords, 2);
      this.trackBarCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.fillGuessSettingsBindingSource, "CheckedRecords", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.trackBarCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.trackBarCheckedRecords.LargeChange = 2000;
      this.trackBarCheckedRecords.Location = new System.Drawing.Point(3, 74);
      this.trackBarCheckedRecords.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.trackBarCheckedRecords.Maximum = 50000;
      this.trackBarCheckedRecords.Name = "trackBarCheckedRecords";
      this.trackBarCheckedRecords.Size = new System.Drawing.Size(204, 28);
      this.trackBarCheckedRecords.SmallChange = 100;
      this.trackBarCheckedRecords.TabIndex = 3;
      this.trackBarCheckedRecords.TickFrequency = 2000;
      this.trackBarCheckedRecords.Value = 250;
      // 
      // textBoxCheckedRecords
      // 
      this.textBoxCheckedRecords.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.textBoxCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "CheckedRecords", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxCheckedRecords.Location = new System.Drawing.Point(213, 75);
      this.textBoxCheckedRecords.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.textBoxCheckedRecords.Name = "textBoxCheckedRecords";
      this.textBoxCheckedRecords.Size = new System.Drawing.Size(63, 25);
      this.textBoxCheckedRecords.TabIndex = 4;
      this.toolTip.SetToolTip(this.textBoxCheckedRecords, "The more records are read the higher is the chance to get a good variety of sampl" +
        "e values, especially for sparsely populated columns");
      // 
      // textBoxSampleValues
      // 
      this.textBoxSampleValues.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "SampleValues", true));
      this.textBoxSampleValues.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxSampleValues.Location = new System.Drawing.Point(213, 30);
      this.textBoxSampleValues.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.textBoxSampleValues.Name = "textBoxSampleValues";
      this.textBoxSampleValues.Size = new System.Drawing.Size(63, 25);
      this.textBoxSampleValues.TabIndex = 1;
      this.toolTip.SetToolTip(this.textBoxSampleValues, "As the amount of sample is found the detection process will start");
      this.textBoxSampleValues.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxSampleValues_Validating);
      // 
      // checkBoxDectectNumbers
      // 
      this.checkBoxDectectNumbers.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDectectNumbers, 3);
      this.checkBoxDectectNumbers.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDectectNumbers.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DectectNumbers", true));
      this.checkBoxDectectNumbers.Location = new System.Drawing.Point(3, 106);
      this.checkBoxDectectNumbers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDectectNumbers.Name = "checkBoxDectectNumbers";
      this.checkBoxDectectNumbers.Size = new System.Drawing.Size(93, 24);
      this.checkBoxDectectNumbers.TabIndex = 6;
      this.checkBoxDectectNumbers.Text = "Numeric";
      this.toolTip.SetToolTip(this.checkBoxDectectNumbers, "Numbers with leading 0 will not be regarded as numbers to prevent information los" +
        "s");
      this.checkBoxDectectNumbers.UseVisualStyleBackColor = true;
      // 
      // label21
      // 
      this.label21.AutoSize = true;
      this.label21.Location = new System.Drawing.Point(282, 290);
      this.label21.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label21.Name = "label21";
      this.label21.Size = new System.Drawing.Size(639, 40);
      this.label21.TabIndex = 22;
      this.label21.Text = "Detect Boolean values like: Yes/No, True/False, 1/0.  You may add your own values" +
    " to the text boxes\r\n";
      // 
      // label22
      // 
      this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label22.AutoSize = true;
      this.label22.Location = new System.Drawing.Point(282, 108);
      this.label22.Name = "label22";
      this.label22.Size = new System.Drawing.Size(312, 20);
      this.label22.TabIndex = 7;
      this.label22.Text = "Detect Numeric (Integer or Decimal) values";
      // 
      // checkBoxDectectPercentage
      // 
      this.checkBoxDectectPercentage.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDectectPercentage, 3);
      this.checkBoxDectectPercentage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DectectPercentage", true));
      this.checkBoxDectectPercentage.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDectectPercentage.Location = new System.Drawing.Point(3, 262);
      this.checkBoxDectectPercentage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDectectPercentage.Name = "checkBoxDectectPercentage";
      this.checkBoxDectectPercentage.Size = new System.Drawing.Size(117, 24);
      this.checkBoxDectectPercentage.TabIndex = 17;
      this.checkBoxDectectPercentage.Text = "Percentage";
      this.toolTip.SetToolTip(this.checkBoxDectectPercentage, "Detect Percentage and ");
      this.checkBoxDectectPercentage.UseVisualStyleBackColor = true;
      // 
      // checkBoxDetectDateTime
      // 
      this.checkBoxDetectDateTime.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDetectDateTime, 3);
      this.checkBoxDetectDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectDateTime", true));
      this.checkBoxDetectDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDetectDateTime.Location = new System.Drawing.Point(3, 134);
      this.checkBoxDetectDateTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDetectDateTime.Name = "checkBoxDetectDateTime";
      this.checkBoxDetectDateTime.Size = new System.Drawing.Size(116, 24);
      this.checkBoxDetectDateTime.TabIndex = 8;
      this.checkBoxDetectDateTime.Text = "Date / Time";
      this.toolTip.SetToolTip(this.checkBoxDetectDateTime, "Detect dates and times on a variety of formats, to make sure the order of day and" +
        " month is determined correctly enough sample values should be present");
      this.checkBoxDetectDateTime.UseVisualStyleBackColor = true;
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(282, 134);
      this.label23.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(315, 20);
      this.label23.TabIndex = 10;
      this.label23.Text = "Detect Date/Time values. in various formats";
      // 
      // label30
      // 
      this.label30.AutoSize = true;
      this.label30.Location = new System.Drawing.Point(282, 262);
      this.label30.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label30.Name = "label30";
      this.label30.Size = new System.Drawing.Size(438, 20);
      this.label30.TabIndex = 18;
      this.label30.Text = "Detect Percentages, stored as decimal value (divided by 100)";
      // 
      // checkBoxDetectBoolean
      // 
      this.checkBoxDetectBoolean.AutoSize = true;
      this.checkBoxDetectBoolean.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectBoolean", true));
      this.checkBoxDetectBoolean.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDetectBoolean.Location = new System.Drawing.Point(3, 290);
      this.checkBoxDetectBoolean.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDetectBoolean.Name = "checkBoxDetectBoolean";
      this.checkBoxDetectBoolean.Size = new System.Drawing.Size(94, 24);
      this.checkBoxDetectBoolean.TabIndex = 19;
      this.checkBoxDetectBoolean.Text = "Boolean";
      this.toolTip.SetToolTip(this.checkBoxDetectBoolean, "Detect Boolean values, the minimum number of samples does not need to be checked " +
        "to allow detection");
      this.checkBoxDetectBoolean.UseVisualStyleBackColor = true;
      // 
      // textBoxTrue
      // 
      this.textBoxTrue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "TrueValue", true));
      this.textBoxTrue.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxTrue.Location = new System.Drawing.Point(103, 290);
      this.textBoxTrue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.textBoxTrue.Name = "textBoxTrue";
      this.textBoxTrue.Size = new System.Drawing.Size(63, 25);
      this.textBoxTrue.TabIndex = 20;
      this.toolTip.SetToolTip(this.textBoxTrue, "Value(s) that should be regarded as TRUE, separated by ;");
      // 
      // textBoxFalse
      // 
      this.textBoxFalse.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "FalseValue", true));
      this.textBoxFalse.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxFalse.Location = new System.Drawing.Point(213, 290);
      this.textBoxFalse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.textBoxFalse.Name = "textBoxFalse";
      this.textBoxFalse.Size = new System.Drawing.Size(63, 25);
      this.textBoxFalse.TabIndex = 21;
      this.toolTip.SetToolTip(this.textBoxFalse, "Value(s) that should be regarded as FALSE, separated by ;");
      // 
      // checkBoxSerialDateTime
      // 
      this.checkBoxSerialDateTime.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSerialDateTime, 3);
      this.checkBoxSerialDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "SerialDateTime", true));
      this.checkBoxSerialDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxSerialDateTime.Location = new System.Drawing.Point(3, 190);
      this.checkBoxSerialDateTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxSerialDateTime.Name = "checkBoxSerialDateTime";
      this.checkBoxSerialDateTime.Size = new System.Drawing.Size(189, 24);
      this.checkBoxSerialDateTime.TabIndex = 13;
      this.checkBoxSerialDateTime.Text = "Allow Serial DateTime";
      this.toolTip.SetToolTip(this.checkBoxSerialDateTime, "Excel stores dates as number of days after the December 31, 1899: \r\nJanuary 1, 19" +
        "00  is 1 or \r\nSaturday, 15. December 2018 13:40:10 is 43449.56956\r\n");
      this.checkBoxSerialDateTime.UseVisualStyleBackColor = true;
      // 
      // label32
      // 
      this.label32.AutoSize = true;
      this.label32.Location = new System.Drawing.Point(282, 190);
      this.label32.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label32.Name = "label32";
      this.label32.Size = new System.Drawing.Size(476, 20);
      this.label32.TabIndex = 14;
      this.label32.Text = "Allow serial Date Time formats, used in Excel and OLE Automation";
      // 
      // checkBoxDetectGUID
      // 
      this.checkBoxDetectGUID.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDetectGUID, 2);
      this.checkBoxDetectGUID.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectGUID", true));
      this.checkBoxDetectGUID.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDetectGUID.Location = new System.Drawing.Point(3, 334);
      this.checkBoxDetectGUID.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDetectGUID.Name = "checkBoxDetectGUID";
      this.checkBoxDetectGUID.Size = new System.Drawing.Size(77, 24);
      this.checkBoxDetectGUID.TabIndex = 23;
      this.checkBoxDetectGUID.Text = "GUID";
      this.toolTip.SetToolTip(this.checkBoxDetectGUID, "Detect Globally Unique Identifiers sometimes named UUID or universally unique ide" +
        "ntifier");
      this.checkBoxDetectGUID.UseVisualStyleBackColor = true;
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBox1, 3);
      this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "IgnoreIdColums", true));
      this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBox1.Location = new System.Drawing.Point(3, 362);
      this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(165, 24);
      this.checkBox1.TabIndex = 25;
      this.checkBox1.Text = "Ignore ID columns";
      this.toolTip.SetToolTip(this.checkBox1, "Ignore columns format detection based on the name of the column");
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(282, 362);
      this.label1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(639, 40);
      this.label1.TabIndex = 26;
      this.label1.Text = "Columns names that end with Id, Ref or Text will be read as text even if seem to " +
    "contain a number";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(282, 334);
      this.label2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(394, 20);
      this.label2.TabIndex = 24;
      this.label2.Text = "Detect GUIDs, GUID values cannot be filtered like text";
      // 
      // label19
      // 
      this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(282, 78);
      this.label19.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(464, 20);
      this.label19.TabIndex = 5;
      this.label19.Text = "Number of records to check in order to get differnt sample values";
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Location = new System.Drawing.Point(282, 30);
      this.label20.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(637, 40);
      this.label20.TabIndex = 2;
      this.label20.Text = "Minimum and maximum number of samples to read before trying to determine the form" +
    "at. \r\nThe more values are read the better the detection but the slower the proce" +
    "ss.\r\n";
      // 
      // textBoxMinSamples
      // 
      this.textBoxMinSamples.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "MinSamples", true));
      this.textBoxMinSamples.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBoxMinSamples.Location = new System.Drawing.Point(103, 30);
      this.textBoxMinSamples.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.textBoxMinSamples.Name = "textBoxMinSamples";
      this.textBoxMinSamples.Size = new System.Drawing.Size(63, 25);
      this.textBoxMinSamples.TabIndex = 9;
      this.toolTip.SetToolTip(this.textBoxMinSamples, "A higher the number of samples ensures the guessed format is correct, columns tha" +
        "t not contain a variety of values might not provide may samples\r\n");
      this.textBoxMinSamples.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxMinSamples_Validating);
      // 
      // checkBoxNamedDates
      // 
      this.checkBoxNamedDates.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxNamedDates, 3);
      this.checkBoxNamedDates.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "CheckNamedDates", true));
      this.checkBoxNamedDates.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxNamedDates.Location = new System.Drawing.Point(3, 162);
      this.checkBoxNamedDates.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxNamedDates.Name = "checkBoxNamedDates";
      this.checkBoxNamedDates.Size = new System.Drawing.Size(206, 24);
      this.checkBoxNamedDates.TabIndex = 11;
      this.checkBoxNamedDates.Text = "Named Month and Days";
      this.toolTip.SetToolTip(this.checkBoxNamedDates, "Detect dates with names days or month, e.G. Monday, 3. May 2017");
      this.checkBoxNamedDates.UseVisualStyleBackColor = true;
      // 
      // checkBoxDateParts
      // 
      this.checkBoxDateParts.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDateParts, 3);
      this.checkBoxDateParts.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DateParts", true));
      this.checkBoxDateParts.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkBoxDateParts.Location = new System.Drawing.Point(3, 218);
      this.checkBoxDateParts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.checkBoxDateParts.Name = "checkBoxDateParts";
      this.checkBoxDateParts.Size = new System.Drawing.Size(229, 24);
      this.checkBoxDateParts.TabIndex = 15;
      this.checkBoxDateParts.Text = "Include Time and Timezone";
      this.toolTip.SetToolTip(this.checkBoxDateParts, "Find columns that possible correspond to a date colum to combine date and time");
      this.checkBoxDateParts.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(282, 162);
      this.label4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(400, 20);
      this.label4.TabIndex = 12;
      this.label4.Text = "Check for named month or days  (this is a slow process)";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(282, 218);
      this.label5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(631, 40);
      this.label5.TabIndex = 16;
      this.label5.Text = "Find associated Time and Time Zone for date columns and combine the information t" +
    "o a date with time\r\n";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 3, 11);
      this.tableLayoutPanel1.Controls.Add(this.textBoxSampleValues, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 3, 10);
      this.tableLayoutPanel1.Controls.Add(this.textBoxMinSamples, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label21, 3, 9);
      this.tableLayoutPanel1.Controls.Add(this.textBoxFalse, 2, 9);
      this.tableLayoutPanel1.Controls.Add(this.label30, 3, 8);
      this.tableLayoutPanel1.Controls.Add(this.label5, 3, 7);
      this.tableLayoutPanel1.Controls.Add(this.label32, 3, 6);
      this.tableLayoutPanel1.Controls.Add(this.label4, 3, 5);
      this.tableLayoutPanel1.Controls.Add(this.label23, 3, 4);
      this.tableLayoutPanel1.Controls.Add(this.label22, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this.label19, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this.label20, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 11);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDetectGUID, 0, 10);
      this.tableLayoutPanel1.Controls.Add(this.textBoxTrue, 1, 9);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDetectBoolean, 0, 9);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDectectPercentage, 0, 8);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDateParts, 0, 7);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxSerialDateTime, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxNamedDates, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDetectDateTime, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxDectectNumbers, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.trackBarCheckedRecords, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.textBoxCheckedRecords, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.radioButtonEnabled, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.radioButtonDisabled, 2, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 12;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(947, 404);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // radioButtonEnabled
      // 
      this.radioButtonEnabled.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.radioButtonEnabled, 2);
      this.radioButtonEnabled.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.radioButtonEnabled.Location = new System.Drawing.Point(3, 2);
      this.radioButtonEnabled.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.radioButtonEnabled.Name = "radioButtonEnabled";
      this.radioButtonEnabled.Size = new System.Drawing.Size(157, 24);
      this.radioButtonEnabled.TabIndex = 27;
      this.radioButtonEnabled.TabStop = true;
      this.radioButtonEnabled.Text = "Enable Detection";
      this.radioButtonEnabled.UseVisualStyleBackColor = true;
      // 
      // radioButtonDisabled
      // 
      this.radioButtonDisabled.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.radioButtonDisabled, 2);
      this.radioButtonDisabled.Location = new System.Drawing.Point(213, 2);
      this.radioButtonDisabled.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.radioButtonDisabled.Name = "radioButtonDisabled";
      this.radioButtonDisabled.Size = new System.Drawing.Size(160, 24);
      this.radioButtonDisabled.TabIndex = 27;
      this.radioButtonDisabled.TabStop = true;
      this.radioButtonDisabled.Text = "Disbale Detection";
      this.radioButtonDisabled.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // fillGuessSettingsBindingSource
      // 
      this.fillGuessSettingsBindingSource.AllowNew = false;
      this.fillGuessSettingsBindingSource.DataSource = typeof(CsvTools.FillGuessSettings);
      // 
      // FillGuessSettingEdit
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.MinimumSize = new System.Drawing.Size(947, 370);
      this.Name = "FillGuessSettingEdit";
      this.Size = new System.Drawing.Size(947, 416);
      ((System.ComponentModel.ISupportInitialize)(this.trackBarCheckedRecords)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.fillGuessSettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

#endregion
    private System.Windows.Forms.TrackBar trackBarCheckedRecords;
    private System.Windows.Forms.TextBox textBoxSampleValues;
    private System.Windows.Forms.TextBox textBoxCheckedRecords;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.Label label20;
    private System.Windows.Forms.CheckBox checkBoxDectectNumbers;
    private System.Windows.Forms.Label label22;
    private System.Windows.Forms.CheckBox checkBoxDetectGUID;
    private System.Windows.Forms.CheckBox checkBoxDectectPercentage;
    private System.Windows.Forms.CheckBox checkBoxDetectDateTime;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.Label label30;
    private System.Windows.Forms.CheckBox checkBoxSerialDateTime;
    private System.Windows.Forms.Label label32;
    private System.Windows.Forms.CheckBox checkBoxDetectBoolean;
    private System.Windows.Forms.TextBox textBoxTrue;
    private System.Windows.Forms.TextBox textBoxFalse;
    private System.Windows.Forms.BindingSource fillGuessSettingsBindingSource;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label21;
    private System.Windows.Forms.TextBox textBoxMinSamples;
    private System.Windows.Forms.CheckBox checkBoxNamedDates;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox checkBoxDateParts;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.RadioButton radioButtonEnabled;
    private System.Windows.Forms.RadioButton radioButtonDisabled;
  }
}
