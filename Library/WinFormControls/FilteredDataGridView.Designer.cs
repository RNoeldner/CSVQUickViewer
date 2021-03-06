
namespace CsvTools
{
  using System.Diagnostics.CodeAnalysis;
  partial class FilteredDataGridView
  {

    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilteredDataGridView));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.contextMenuStripCell = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemCopyError = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripFilter = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemApplyFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFilterAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFilterThisValue = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFilterRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemShowAllColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemHideAllColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFilled = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemLoadCol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSaveCol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripTextBoxColFilter = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemHideThisColumn = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCF = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorCF = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemRemoveOne = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFilterRemoveAllFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSortAscending = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSortDescending = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSortRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFreeze = new System.Windows.Forms.ToolStripMenuItem();
			this.timerColumsFilterChecked = new System.Windows.Forms.Timer(this.components);
			this.timerColumsFilterText = new System.Windows.Forms.Timer(this.components);
			this.toolStripMenuItemColumnVisibility = new CsvTools.ToolStripCheckedListBox();
			this.contextMenuStripCell.SuspendLayout();
			this.contextMenuStripFilter.SuspendLayout();
			this.contextMenuStripColumns.SuspendLayout();
			this.contextMenuStripHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(110, 6);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(265, 6);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(265, 6);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(167, 6);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(320, 6);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(320, 6);
			// 
			// contextMenuStripCell
			// 
			this.contextMenuStripCell.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripCell.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopyError,
            this.toolStripMenuItemCopy,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.toolStripMenuItemFilterThisValue,
            this.toolStripMenuItemFilterRemove,
            this.toolStripSeparator7,
            this.toolStripMenuItem1});
			this.contextMenuStripCell.Name = "contextMenuStripDropDownCopy";
			this.contextMenuStripCell.Size = new System.Drawing.Size(324, 196);
			// 
			// toolStripMenuItemCopyError
			// 
			this.toolStripMenuItemCopyError.Name = "toolStripMenuItemCopyError";
			this.toolStripMenuItemCopyError.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopyError.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItemCopyError.Text = "Copy";
			this.toolStripMenuItemCopyError.Click += new System.EventHandler(this.ToolStripMenuItemCopyError_Click);
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
			this.toolStripMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItemCopy.Text = "Copy (without error information)";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.ToolStripMenuItemCopy_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.DropDown = this.contextMenuStripFilter;
			this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItem2.Text = "Filter";
			// 
			// contextMenuStripFilter
			// 
			this.contextMenuStripFilter.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator4,
            this.toolStripMenuItemApplyFilter});
			this.contextMenuStripFilter.Name = "contextMenuStripFilter";
			this.contextMenuStripFilter.OwnerItem = this.toolStripMenuItem2;
			this.contextMenuStripFilter.Size = new System.Drawing.Size(114, 40);
			this.contextMenuStripFilter.Text = "contextMenuStripFilter";
			// 
			// toolStripMenuItemApplyFilter
			// 
			this.toolStripMenuItemApplyFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemApplyFilter.Image")));
			this.toolStripMenuItemApplyFilter.Name = "toolStripMenuItemApplyFilter";
			this.toolStripMenuItemApplyFilter.Size = new System.Drawing.Size(113, 30);
			this.toolStripMenuItemApplyFilter.Text = "&Apply";
			this.toolStripMenuItemApplyFilter.Click += new System.EventHandler(this.ToolStripMenuItemApply_Click);
			// 
			// toolStripMenuItemFilterAdd
			// 
			this.toolStripMenuItemFilterAdd.DropDown = this.contextMenuStripFilter;
			this.toolStripMenuItemFilterAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemFilterAdd.Image")));
			this.toolStripMenuItemFilterAdd.Name = "toolStripMenuItemFilterAdd";
			this.toolStripMenuItemFilterAdd.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemFilterAdd.Text = "Filter";
			// 
			// toolStripMenuItemFilterThisValue
			// 
			this.toolStripMenuItemFilterThisValue.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemFilterThisValue.Image")));
			this.toolStripMenuItemFilterThisValue.Name = "toolStripMenuItemFilterThisValue";
			this.toolStripMenuItemFilterThisValue.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItemFilterThisValue.Text = "Filter for this value";
			this.toolStripMenuItemFilterThisValue.Click += new System.EventHandler(this.ToolStripMenuItemFilterValue_Click);
			// 
			// toolStripMenuItemFilterRemove
			// 
			this.toolStripMenuItemFilterRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemFilterRemove.Image")));
			this.toolStripMenuItemFilterRemove.Name = "toolStripMenuItemFilterRemove";
			this.toolStripMenuItemFilterRemove.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItemFilterRemove.Text = "Remove all Filter";
			this.toolStripMenuItemFilterRemove.Click += new System.EventHandler(this.ToolStripMenuItemFilterRemoveAll_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDown = this.contextMenuStripColumns;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(323, 30);
			this.toolStripMenuItem1.Text = "Columns";
			// 
			// contextMenuStripColumns
			// 
			this.contextMenuStripColumns.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripColumns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemShowAllColumns,
            this.toolStripMenuItemHideAllColumns,
            this.toolStripMenuItemFilled,
            this.toolStripSeparator5,
            this.toolStripMenuItemLoadCol,
            this.toolStripMenuItemSaveCol,
            this.toolStripSeparator8,
            this.toolStripTextBoxColFilter,
            this.toolStripSeparator9,
            this.toolStripMenuItemColumnVisibility});
			this.contextMenuStripColumns.Name = "contextMenuStripColumns";
			this.contextMenuStripColumns.OwnerItem = this.toolStripMenuItemColumns;
			this.contextMenuStripColumns.ShowImageMargin = false;
			this.contextMenuStripColumns.Size = new System.Drawing.Size(179, 185);
			// 
			// toolStripMenuItemShowAllColumns
			// 
			this.toolStripMenuItemShowAllColumns.Name = "toolStripMenuItemShowAllColumns";
			this.toolStripMenuItemShowAllColumns.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemShowAllColumns.Text = "Show All Columns";
			this.toolStripMenuItemShowAllColumns.Click += new System.EventHandler(this.ToolStripMenuItemShowAllColumns_Click);
			// 
			// toolStripMenuItemHideAllColumns
			// 
			this.toolStripMenuItemHideAllColumns.Name = "toolStripMenuItemHideAllColumns";
			this.toolStripMenuItemHideAllColumns.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemHideAllColumns.Text = "Hide Other Columns";
			this.toolStripMenuItemHideAllColumns.Click += new System.EventHandler(this.ToolStripMenuItemHideAllColumns_Click);
			// 
			// toolStripMenuItemFilled
			// 
			this.toolStripMenuItemFilled.Name = "toolStripMenuItemFilled";
			this.toolStripMenuItemFilled.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemFilled.Text = "Hide Empty Columns";
			this.toolStripMenuItemFilled.Click += new System.EventHandler(this.ToolStripMenuItemFilled_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(175, 6);
			// 
			// toolStripMenuItemLoadCol
			// 
			this.toolStripMenuItemLoadCol.Name = "toolStripMenuItemLoadCol";
			this.toolStripMenuItemLoadCol.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemLoadCol.Text = "Load Columns and Filter";
			this.toolStripMenuItemLoadCol.ToolTipText = "Load column configuarion from file";
			this.toolStripMenuItemLoadCol.Click += new System.EventHandler(this.ToolStripMenuItemLoadCol_Click);
			// 
			// toolStripMenuItemSaveCol
			// 
			this.toolStripMenuItemSaveCol.Name = "toolStripMenuItemSaveCol";
			this.toolStripMenuItemSaveCol.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemSaveCol.Text = "Save Columns and Filter";
			this.toolStripMenuItemSaveCol.ToolTipText = "Save column configuarion to file";
			this.toolStripMenuItemSaveCol.Click += new System.EventHandler(this.ToolStripMenuItemSaveCol_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(175, 6);
			// 
			// toolStripTextBoxColFilter
			// 
			this.toolStripTextBoxColFilter.BackColor = System.Drawing.SystemColors.Info;
			this.toolStripTextBoxColFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.toolStripTextBoxColFilter.Name = "toolStripTextBoxColFilter";
			this.toolStripTextBoxColFilter.Size = new System.Drawing.Size(100, 23);
			this.toolStripTextBoxColFilter.ToolTipText = "Show columns that contain the input";
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(175, 6);
			// 
			// toolStripMenuItemColumns
			// 
			this.toolStripMenuItemColumns.DropDown = this.contextMenuStripColumns;
			this.toolStripMenuItemColumns.Name = "toolStripMenuItemColumns";
			this.toolStripMenuItemColumns.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemColumns.Text = "Columns";
			// 
			// contextMenuStripHeader
			// 
			this.contextMenuStripHeader.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemHideThisColumn,
            this.toolStripMenuItemFilterAdd,
            this.toolStripMenuItemColumns,
            this.toolStripMenuItemCF,
            this.toolStripSeparatorCF,
            this.toolStripMenuItemRemoveOne,
            this.toolStripMenuItemFilterRemoveAllFilter,
            this.toolStripSeparator2,
            this.toolStripMenuItemSortAscending,
            this.toolStripMenuItemSortDescending,
            this.toolStripMenuItemSortRemove,
            this.toolStripSeparator3,
            this.toolStripMenuItemFreeze});
			this.contextMenuStripHeader.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.contextMenuStripHeader.Name = "contextMenuStripHeader";
			this.contextMenuStripHeader.Size = new System.Drawing.Size(269, 322);
			// 
			// toolStripMenuItemHideThisColumn
			// 
			this.toolStripMenuItemHideThisColumn.Name = "toolStripMenuItemHideThisColumn";
			this.toolStripMenuItemHideThisColumn.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemHideThisColumn.Text = "Hide Column";
			this.toolStripMenuItemHideThisColumn.Click += new System.EventHandler(this.ToolStripMenuItemHideThisColumn_Click);
			// 
			// toolStripMenuItemCF
			// 
			this.toolStripMenuItemCF.Name = "toolStripMenuItemCF";
			this.toolStripMenuItemCF.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemCF.Text = "Change Format";
			this.toolStripMenuItemCF.Visible = false;
			this.toolStripMenuItemCF.Click += new System.EventHandler(this.ToolStripMenuItemCF_Click);
			// 
			// toolStripSeparatorCF
			// 
			this.toolStripSeparatorCF.Name = "toolStripSeparatorCF";
			this.toolStripSeparatorCF.Size = new System.Drawing.Size(265, 6);
			this.toolStripSeparatorCF.Visible = false;
			// 
			// toolStripMenuItemRemoveOne
			// 
			this.toolStripMenuItemRemoveOne.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemRemoveOne.Image")));
			this.toolStripMenuItemRemoveOne.Name = "toolStripMenuItemRemoveOne";
			this.toolStripMenuItemRemoveOne.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.toolStripMenuItemRemoveOne.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemRemoveOne.Text = "Remove Filter";
			this.toolStripMenuItemRemoveOne.Click += new System.EventHandler(this.ToolStripMenuItemFilterRemoveOne_Click);
			// 
			// toolStripMenuItemFilterRemoveAllFilter
			// 
			this.toolStripMenuItemFilterRemoveAllFilter.Enabled = false;
			this.toolStripMenuItemFilterRemoveAllFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemFilterRemoveAllFilter.Image")));
			this.toolStripMenuItemFilterRemoveAllFilter.Name = "toolStripMenuItemFilterRemoveAllFilter";
			this.toolStripMenuItemFilterRemoveAllFilter.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemFilterRemoveAllFilter.Text = "Remove all Filter";
			this.toolStripMenuItemFilterRemoveAllFilter.Click += new System.EventHandler(this.ToolStripMenuItemFilterRemoveAll_Click);
			// 
			// toolStripMenuItemSortAscending
			// 
			this.toolStripMenuItemSortAscending.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemSortAscending.Image")));
			this.toolStripMenuItemSortAscending.Name = "toolStripMenuItemSortAscending";
			this.toolStripMenuItemSortAscending.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemSortAscending.Tag = "Sort ascending by \'{0}\'";
			this.toolStripMenuItemSortAscending.Text = "Sort ascending by \'Column name\'";
			this.toolStripMenuItemSortAscending.Click += new System.EventHandler(this.ToolStripMenuItemSortAscending_Click);
			// 
			// toolStripMenuItemSortDescending
			// 
			this.toolStripMenuItemSortDescending.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemSortDescending.Image")));
			this.toolStripMenuItemSortDescending.Name = "toolStripMenuItemSortDescending";
			this.toolStripMenuItemSortDescending.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemSortDescending.Tag = "Sort descending by \'{0}\'";
			this.toolStripMenuItemSortDescending.Text = "Sort descending by \'Column name\'";
			this.toolStripMenuItemSortDescending.Click += new System.EventHandler(this.ToolStripMenuItemSortDescending_Click);
			// 
			// toolStripMenuItemSortRemove
			// 
			this.toolStripMenuItemSortRemove.Name = "toolStripMenuItemSortRemove";
			this.toolStripMenuItemSortRemove.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemSortRemove.Text = "Unsort";
			this.toolStripMenuItemSortRemove.Click += new System.EventHandler(this.ToolStripMenuItemSortRemove_Click);
			// 
			// toolStripMenuItemFreeze
			// 
			this.toolStripMenuItemFreeze.Name = "toolStripMenuItemFreeze";
			this.toolStripMenuItemFreeze.Size = new System.Drawing.Size(268, 30);
			this.toolStripMenuItemFreeze.Text = "Freeze";
			this.toolStripMenuItemFreeze.Click += new System.EventHandler(this.ToolStripMenuItemFreeze_Click);
			// 
			// timerColumsFilterChecked
			// 
			this.timerColumsFilterChecked.Interval = 500;
			this.timerColumsFilterChecked.Tick += new System.EventHandler(this.TimerColumnsFilter_Tick);
			// 
			// timerColumsFilterText
			// 
			this.timerColumsFilterText.Interval = 400;
			this.timerColumsFilterText.Tick += new System.EventHandler(this.TimerColumsFilterText_Tick);
			// 
			// toolStripMenuItemColumnVisibility
			// 
			this.toolStripMenuItemColumnVisibility.BackColor = System.Drawing.SystemColors.Window;
			this.toolStripMenuItemColumnVisibility.Name = "toolStripMenuItemColumnVisibility";
			this.toolStripMenuItemColumnVisibility.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
			this.toolStripMenuItemColumnVisibility.Size = new System.Drawing.Size(39, 25);
			// 
			// FilteredDataGridView
			// 
			this.AutoGenerateColumns = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.RowTemplate.Height = 33;
			this.contextMenuStripCell.ResumeLayout(false);
			this.contextMenuStripFilter.ResumeLayout(false);
			this.contextMenuStripColumns.ResumeLayout(false);
			this.contextMenuStripColumns.PerformLayout();
			this.contextMenuStripHeader.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);

    }

    private System.Windows.Forms.ContextMenuStrip contextMenuStripCell;
    private System.Windows.Forms.ContextMenuStrip contextMenuStripColumns;
    private System.Windows.Forms.ContextMenuStrip contextMenuStripFilter;
    private System.Windows.Forms.ContextMenuStrip contextMenuStripHeader;
    private System.Windows.Forms.Timer timerColumsFilterChecked;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemApplyFilter;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCF;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemColumns;
    private CsvTools.ToolStripCheckedListBox toolStripMenuItemColumnVisibility;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyError;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFilled;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFilterAdd;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFilterRemove;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFilterRemoveAllFilter;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFilterThisValue;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFreeze;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHideAllColumns;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHideThisColumn;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadCol;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveOne;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveCol;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowAllColumns;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSortAscending;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSortDescending;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSortRemove;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparatorCF;
    private System.Windows.Forms.ToolStripTextBox toolStripTextBoxColFilter;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
    private System.Windows.Forms.Timer timerColumsFilterText;
  }
}
