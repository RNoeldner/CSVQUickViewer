using System.Diagnostics.CodeAnalysis;

namespace CsvTools
{
  partial class FillGuessSettingEdit
  {
    /// <summary>
    /// The components
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // <inheritdoc />
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
			this.fillGuessSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
			this.checkBoxNamedDates = new System.Windows.Forms.CheckBox();
			this.checkBoxDateParts = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.radioButtonEnabled = new System.Windows.Forms.RadioButton();
			this.radioButtonDisabled = new System.Windows.Forms.RadioButton();
			this.numericUpDownMin = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownSampleValues = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownChecked = new System.Windows.Forms.NumericUpDown();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trackBarCheckedRecords)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fillGuessSettingsBindingSource)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSampleValues)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownChecked)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBarCheckedRecords
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.trackBarCheckedRecords, 2);
			this.trackBarCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.fillGuessSettingsBindingSource, "CheckedRecords", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBarCheckedRecords.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBarCheckedRecords.LargeChange = 2000;
			this.trackBarCheckedRecords.Location = new System.Drawing.Point(2, 53);
			this.trackBarCheckedRecords.Margin = new System.Windows.Forms.Padding(2);
			this.trackBarCheckedRecords.Maximum = 50000;
			this.trackBarCheckedRecords.Name = "trackBarCheckedRecords";
			this.trackBarCheckedRecords.Size = new System.Drawing.Size(129, 20);
			this.trackBarCheckedRecords.SmallChange = 100;
			this.trackBarCheckedRecords.TabIndex = 3;
			this.trackBarCheckedRecords.TickFrequency = 2000;
			this.trackBarCheckedRecords.Value = 250;
			// 
			// fillGuessSettingsBindingSource
			// 
			this.fillGuessSettingsBindingSource.AllowNew = false;
			this.fillGuessSettingsBindingSource.DataSource = typeof(CsvTools.FillGuessSettings);
			// 
			// checkBoxDectectNumbers
			// 
			this.checkBoxDectectNumbers.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDectectNumbers, 3);
			this.checkBoxDectectNumbers.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxDectectNumbers.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectNumbers", true));
			this.checkBoxDectectNumbers.Location = new System.Drawing.Point(2, 77);
			this.checkBoxDectectNumbers.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDectectNumbers.Name = "checkBoxDectectNumbers";
			this.checkBoxDectectNumbers.Size = new System.Drawing.Size(65, 17);
			this.checkBoxDectectNumbers.TabIndex = 6;
			this.checkBoxDectectNumbers.Text = "Numeric";
			this.toolTip.SetToolTip(this.checkBoxDectectNumbers, "Numbers with leading 0 will not be regarded as numbers to prevent information los" +
        "s");
			this.checkBoxDectectNumbers.UseVisualStyleBackColor = true;
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(187, 212);
			this.label21.Margin = new System.Windows.Forms.Padding(2);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(442, 26);
			this.label21.TabIndex = 22;
			this.label21.Text = "Detect Boolean values like: Yes/No, True/False, 1/0.  You may add your own values" +
    " to the text boxes\r\n";
			// 
			// label22
			// 
			this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(187, 79);
			this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(210, 13);
			this.label22.TabIndex = 7;
			this.label22.Text = "Detect Numeric (Integer or Decimal) values";
			// 
			// checkBoxDectectPercentage
			// 
			this.checkBoxDectectPercentage.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDectectPercentage, 3);
			this.checkBoxDectectPercentage.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxDectectPercentage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectPercentage", true));
			this.checkBoxDectectPercentage.Location = new System.Drawing.Point(2, 191);
			this.checkBoxDectectPercentage.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDectectPercentage.Name = "checkBoxDectectPercentage";
			this.checkBoxDectectPercentage.Size = new System.Drawing.Size(81, 17);
			this.checkBoxDectectPercentage.TabIndex = 17;
			this.checkBoxDectectPercentage.Text = "Percentage";
			this.toolTip.SetToolTip(this.checkBoxDectectPercentage, "Detect Percentage and ");
			this.checkBoxDectectPercentage.UseVisualStyleBackColor = true;
			// 
			// checkBoxDetectDateTime
			// 
			this.checkBoxDetectDateTime.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDetectDateTime, 3);
			this.checkBoxDetectDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxDetectDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectDateTime", true));
			this.checkBoxDetectDateTime.Location = new System.Drawing.Point(2, 98);
			this.checkBoxDetectDateTime.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDetectDateTime.Name = "checkBoxDetectDateTime";
			this.checkBoxDetectDateTime.Size = new System.Drawing.Size(83, 17);
			this.checkBoxDetectDateTime.TabIndex = 8;
			this.checkBoxDetectDateTime.Text = "Date / Time";
			this.toolTip.SetToolTip(this.checkBoxDetectDateTime, "Detect dates and times on a variety of formats, to make sure the order of day and" +
        " month is determined correctly enough sample values should be present");
			this.checkBoxDetectDateTime.UseVisualStyleBackColor = true;
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(187, 98);
			this.label23.Margin = new System.Windows.Forms.Padding(2);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(215, 13);
			this.label23.TabIndex = 10;
			this.label23.Text = "Detect Date/Time values. in various formats";
			// 
			// label30
			// 
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(187, 191);
			this.label30.Margin = new System.Windows.Forms.Padding(2);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(297, 13);
			this.label30.TabIndex = 18;
			this.label30.Text = "Detect Percentages, stored as decimal value (divided by 100)";
			// 
			// checkBoxDetectBoolean
			// 
			this.checkBoxDetectBoolean.AutoSize = true;
			this.checkBoxDetectBoolean.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectBoolean", true));
			this.checkBoxDetectBoolean.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxDetectBoolean.Location = new System.Drawing.Point(2, 212);
			this.checkBoxDetectBoolean.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDetectBoolean.Name = "checkBoxDetectBoolean";
			this.checkBoxDetectBoolean.Size = new System.Drawing.Size(65, 17);
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
			this.textBoxTrue.Location = new System.Drawing.Point(71, 212);
			this.textBoxTrue.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxTrue.Name = "textBoxTrue";
			this.textBoxTrue.Size = new System.Drawing.Size(44, 20);
			this.textBoxTrue.TabIndex = 20;
			this.toolTip.SetToolTip(this.textBoxTrue, "Value(s) that should be regarded as TRUE, separated by ;");
			// 
			// textBoxFalse
			// 
			this.textBoxFalse.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.fillGuessSettingsBindingSource, "FalseValue", true));
			this.textBoxFalse.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxFalse.Location = new System.Drawing.Point(135, 212);
			this.textBoxFalse.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxFalse.Name = "textBoxFalse";
			this.textBoxFalse.Size = new System.Drawing.Size(48, 20);
			this.textBoxFalse.TabIndex = 21;
			this.toolTip.SetToolTip(this.textBoxFalse, "Value(s) that should be regarded as FALSE, separated by ;");
			// 
			// checkBoxSerialDateTime
			// 
			this.checkBoxSerialDateTime.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSerialDateTime, 3);
			this.checkBoxSerialDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "SerialDateTime", true));
			this.checkBoxSerialDateTime.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxSerialDateTime.Location = new System.Drawing.Point(2, 140);
			this.checkBoxSerialDateTime.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxSerialDateTime.Name = "checkBoxSerialDateTime";
			this.checkBoxSerialDateTime.Size = new System.Drawing.Size(129, 17);
			this.checkBoxSerialDateTime.TabIndex = 13;
			this.checkBoxSerialDateTime.Text = "Allow Serial DateTime";
			this.toolTip.SetToolTip(this.checkBoxSerialDateTime, "Excel stores dates as number of days after the December 31, 1899: \r\nJanuary 1, 19" +
        "00  is 1 or \r\nSaturday, 15. December 2018 13:40:10 is 43449.56956\r\n");
			this.checkBoxSerialDateTime.UseVisualStyleBackColor = true;
			// 
			// label32
			// 
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(187, 140);
			this.label32.Margin = new System.Windows.Forms.Padding(2);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(318, 13);
			this.label32.TabIndex = 14;
			this.label32.Text = "Allow serial Date Time formats, used in Excel and OLE Automation";
			// 
			// checkBoxDetectGUID
			// 
			this.checkBoxDetectGUID.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDetectGUID, 2);
			this.checkBoxDetectGUID.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "DetectGUID", true));
			this.checkBoxDetectGUID.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxDetectGUID.Location = new System.Drawing.Point(2, 242);
			this.checkBoxDetectGUID.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDetectGUID.Name = "checkBoxDetectGUID";
			this.checkBoxDetectGUID.Size = new System.Drawing.Size(53, 17);
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
			this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "IgnoreIdColumns", true));
			this.checkBox1.Location = new System.Drawing.Point(2, 263);
			this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(112, 17);
			this.checkBox1.TabIndex = 25;
			this.checkBox1.Text = "Ignore ID columns";
			this.toolTip.SetToolTip(this.checkBox1, "Ignore columns format detection based on the name of the column");
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(187, 263);
			this.label1.Margin = new System.Windows.Forms.Padding(2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(431, 26);
			this.label1.TabIndex = 26;
			this.label1.Text = "Columns names that end with Id, Ref or Text will be read as text even if seem to " +
    "contain a number";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(187, 242);
			this.label2.Margin = new System.Windows.Forms.Padding(2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(265, 13);
			this.label2.TabIndex = 24;
			this.label2.Text = "Detect GUIDs, GUID values cannot be filtered like text";
			// 
			// label19
			// 
			this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(187, 56);
			this.label19.Margin = new System.Windows.Forms.Padding(2);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(312, 13);
			this.label19.TabIndex = 5;
			this.label19.Text = "Number of records to check in order to get differnt sample values";
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(187, 23);
			this.label20.Margin = new System.Windows.Forms.Padding(2);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(420, 26);
			this.label20.TabIndex = 2;
			this.label20.Text = "Minimum and maximum number of samples to read before trying to determine the form" +
    "at. \r\nThe more values are read the better the detection but the slower the proce" +
    "ss.\r\n";
			// 
			// checkBoxNamedDates
			// 
			this.checkBoxNamedDates.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.checkBoxNamedDates, 3);
			this.checkBoxNamedDates.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "CheckNamedDates", true));
			this.checkBoxNamedDates.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxNamedDates.Location = new System.Drawing.Point(2, 119);
			this.checkBoxNamedDates.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxNamedDates.Name = "checkBoxNamedDates";
			this.checkBoxNamedDates.Size = new System.Drawing.Size(141, 17);
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
			this.checkBoxDateParts.Location = new System.Drawing.Point(2, 161);
			this.checkBoxDateParts.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDateParts.Name = "checkBoxDateParts";
			this.checkBoxDateParts.Size = new System.Drawing.Size(157, 17);
			this.checkBoxDateParts.TabIndex = 15;
			this.checkBoxDateParts.Text = "Include Time and Timezone";
			this.toolTip.SetToolTip(this.checkBoxDateParts, "Find columns that possible correspond to a date colum to combine date and time");
			this.checkBoxDateParts.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(187, 119);
			this.label4.Margin = new System.Windows.Forms.Padding(2);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(268, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "Check for named month or days  (this is a slow process)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(187, 161);
			this.label5.Margin = new System.Windows.Forms.Padding(2);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(423, 26);
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
			this.tableLayoutPanel1.Controls.Add(this.label2, 3, 10);
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
			this.tableLayoutPanel1.Controls.Add(this.radioButtonEnabled, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.radioButtonDisabled, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.numericUpDownMin, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.numericUpDownSampleValues, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.numericUpDownChecked, 2, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 12;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(632, 291);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// radioButtonEnabled
			// 
			this.radioButtonEnabled.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.radioButtonEnabled, 2);
			this.radioButtonEnabled.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.radioButtonEnabled.Location = new System.Drawing.Point(2, 2);
			this.radioButtonEnabled.Margin = new System.Windows.Forms.Padding(2);
			this.radioButtonEnabled.Name = "radioButtonEnabled";
			this.radioButtonEnabled.Size = new System.Drawing.Size(107, 17);
			this.radioButtonEnabled.TabIndex = 27;
			this.radioButtonEnabled.TabStop = true;
			this.radioButtonEnabled.Text = "Enable Detection";
			this.radioButtonEnabled.UseVisualStyleBackColor = true;
			// 
			// radioButtonDisabled
			// 
			this.radioButtonDisabled.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.radioButtonDisabled, 2);
			this.radioButtonDisabled.Location = new System.Drawing.Point(135, 2);
			this.radioButtonDisabled.Margin = new System.Windows.Forms.Padding(2);
			this.radioButtonDisabled.Name = "radioButtonDisabled";
			this.radioButtonDisabled.Size = new System.Drawing.Size(109, 17);
			this.radioButtonDisabled.TabIndex = 27;
			this.radioButtonDisabled.TabStop = true;
			this.radioButtonDisabled.Text = "Disbale Detection";
			this.radioButtonDisabled.UseVisualStyleBackColor = true;
			// 
			// numericUpDownMin
			// 
			this.numericUpDownMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.fillGuessSettingsBindingSource, "MinSamples", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownMin.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownMin.Location = new System.Drawing.Point(72, 24);
			this.numericUpDownMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownMin.Name = "numericUpDownMin";
			this.numericUpDownMin.Size = new System.Drawing.Size(43, 20);
			this.numericUpDownMin.TabIndex = 28;
			// 
			// numericUpDownSampleValues
			// 
			this.numericUpDownSampleValues.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.fillGuessSettingsBindingSource, "SampleValues", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownSampleValues.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownSampleValues.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownSampleValues.Location = new System.Drawing.Point(136, 24);
			this.numericUpDownSampleValues.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownSampleValues.Name = "numericUpDownSampleValues";
			this.numericUpDownSampleValues.Size = new System.Drawing.Size(46, 20);
			this.numericUpDownSampleValues.TabIndex = 29;
			// 
			// numericUpDownChecked
			// 
			this.numericUpDownChecked.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.fillGuessSettingsBindingSource, "CheckedRecords", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownChecked.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.fillGuessSettingsBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownChecked.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericUpDownChecked.Location = new System.Drawing.Point(136, 54);
			this.numericUpDownChecked.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
			this.numericUpDownChecked.Name = "numericUpDownChecked";
			this.numericUpDownChecked.Size = new System.Drawing.Size(46, 20);
			this.numericUpDownChecked.TabIndex = 30;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// FillGuessSettingEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MinimumSize = new System.Drawing.Size(632, 240);
			this.Name = "FillGuessSettingEdit";
			this.Size = new System.Drawing.Size(632, 299);
			((System.ComponentModel.ISupportInitialize)(this.trackBarCheckedRecords)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fillGuessSettingsBindingSource)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSampleValues)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownChecked)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

#endregion
    private System.Windows.Forms.TrackBar trackBarCheckedRecords;
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
    private System.Windows.Forms.CheckBox checkBoxNamedDates;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox checkBoxDateParts;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.RadioButton radioButtonEnabled;
    private System.Windows.Forms.RadioButton radioButtonDisabled;
    private System.Windows.Forms.NumericUpDown numericUpDownMin;
    private System.Windows.Forms.NumericUpDown numericUpDownSampleValues;
    private System.Windows.Forms.NumericUpDown numericUpDownChecked;
  }
}
