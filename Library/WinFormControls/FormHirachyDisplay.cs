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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace CsvTools
{
  /// <summary>
  ///   Windows Form to Display the hierarchy
  /// </summary>
  public class FormHierachyDisplay : Form
  {
    private readonly CancellationTokenSource m_CancellationTokenSource =
      new CancellationTokenSource();

    private FormProcessDisplay m_BuildProcess;
    private readonly DataRow[] m_DataRow;
    private readonly DataTable m_DataTable;
    private readonly Timer m_TimerSearch = new Timer();
    private readonly Timer m_TimerDisplay = new Timer();

    /// <summary>
    ///   Required designer variable.
    /// </summary>
    private IContainer components;

    private ComboBox m_ComboBoxID;

    private ComboBox m_ComboBoxParentID;
    private TableLayoutPanel m_TableLayoutPanel1;
    private TextBox m_TextBoxValue;
    private IEnumerable<TreeData> m_TreeData = new List<TreeData>();
    private ComboBox m_ComboBoxDisplay2;
    private ComboBox m_ComboBoxDisplay1;
    private MultiselectTreeView m_TreeView;

    /// <summary>
    ///   Initializes a new instance of the <see cref="FormHierachyDisplay" /> class.
    /// </summary>
    /// <param name="dataTable">The data table.</param>
    /// <param name="dataRows">The filter.</param>
    public FormHierachyDisplay(DataTable dataTable, DataRow[] dataRows)
    {
      Contract.Requires(dataTable != null);
      Contract.Requires(dataRows != null);
      m_DataTable = dataTable;
      m_DataRow = dataRows;
      InitializeComponent();
      Icon = CsvToolLib.Resources.SubFormIcon;
      m_TimerSearch.Elapsed += FilterValueChangedElapsed;
      m_TimerSearch.Interval = 500;
      m_TimerSearch.AutoReset = false;

      m_TimerDisplay.Elapsed += TimerDisplayElapsed;
      m_TimerDisplay.Interval = 1000;
      m_TimerDisplay.AutoReset = false;
    }

    private void TimerDisplayElapsed(object sender, ElapsedEventArgs e) => this.SafeBeginInvoke(() =>
    {
      if (m_ComboBoxID.SelectedItem != null &&
         m_ComboBoxParentID.SelectedItem != null)
      {
        BuildTree();
      };
    });

    private bool m_DisposedValue; // To detect redundant calls

    /// <summary>
    ///   Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (m_DisposedValue)
        return;
      if (disposing)
      {
        if (components != null)
          components.Dispose();
        m_TimerDisplay.Dispose();
        m_TimerSearch.Dispose();
        if (m_BuildProcess != null)
          m_BuildProcess.Dispose();

        m_CancellationTokenSource.Dispose();
      }
      base.Dispose(disposing);
      m_DisposedValue = true;
    }

    /// <summary>
    ///   Adds the tree data node with child's.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <param name="rootNode">The root node.</param>
    private void AddTreeDataNodeWithChilds(TreeData root, TreeNode rootNode, IProcessDisplay process)
    {
      Contract.Requires(root != null);
      root.Visited = true;
      var treeNode = new TreeNode(root.NodeTitle) { Tag = root };
      if (rootNode == null)
        m_TreeView.Nodes.Add(treeNode);
      else
        rootNode.Nodes.Add(treeNode);
      if (root.Children.Count > 0)
        treeNode.Nodes.AddRange(BuildSubNodes(root, process));
    }

    /// <summary>
    ///   Builds the sub nodes.
    /// </summary>
    /// <param name="parent">The parent ID.</param>
    /// <returns></returns>
    private TreeNode[] BuildSubNodes(TreeData parent, IProcessDisplay process)
    {
      Contract.Requires(parent != null);
      var treeNodes = new List<TreeNode>();
      foreach (var child in parent.Children)
      {
        process.CancellationToken.ThrowIfCancellationRequested();
        Extensions.ProcessUIElements();
        if (child.Visited)
        {
          var treeNode = new TreeNode("Cycle -> " + child.Title) { Tag = child };
          treeNodes.Add(treeNode);
        }
        else
        {
          child.Visited = true;
          var treeNode = new TreeNode(child.NodeTitle, BuildSubNodes(child, process)) { Tag = child };
          treeNodes.Add(treeNode);
        }
      }

      return treeNodes.ToArray();
    }

    /// <summary>
    ///   Builds the tree.
    /// </summary>
    private void BuildTree()
    {
      var oldCursor = Cursor.Current == Cursors.WaitCursor ? Cursors.WaitCursor : Cursors.Default;
      Cursor.Current = Cursors.WaitCursor;
      try
      {
        m_BuildProcess = new FormProcessDisplay("Building Tree", false, m_CancellationTokenSource.Token);
        m_BuildProcess.Show(this);
        m_BuildProcess.Maximum = m_DataRow.GetLength(0) * 2;
        BuildTreeData(m_BuildProcess);

        m_BuildProcess.Maximum = 0;
        ShowTree(m_BuildProcess);
      }
      catch (Exception)
      {
      }
      finally
      {
        m_BuildProcess.Dispose();
        m_BuildProcess = null;
        Cursor.Current = oldCursor;
      }
    }

    /// <summary>
    ///   Builds the tree data.
    /// </summary>
    private void BuildTreeData(IProcessDisplay process)
    {
      Contract.Requires(m_DataTable != null);
      var intervalAction = new IntervalAction();

      var dataColumnParent = m_DataTable.Columns[m_ComboBoxParentID.SelectedItem.ToString()];
      var dataColumnID = m_DataTable.Columns[m_ComboBoxID.SelectedItem.ToString()];
      var dataColumnDisplay1 = m_ComboBoxDisplay1.SelectedItem != null
        ? m_DataTable.Columns[m_ComboBoxDisplay1.SelectedItem.ToString()]
        : null;
      var dataColumnDisplay2 = m_ComboBoxDisplay2.SelectedItem != null
        ? m_DataTable.Columns[m_ComboBoxDisplay2.SelectedItem.ToString()]
        : null;

      // Using a dictionary here to speed up lookups
      var treeDataDictionary = new Dictionary<string, TreeData>();
      var rootDataParentFound = new TreeData
      {
        ID = "{R}",
        Title = "Parent found / No Parent"
      };

      treeDataDictionary.Add(rootDataParentFound.ID, rootDataParentFound);

      var counter = 0;
      foreach (var dataRow in m_DataRow)
      {
        process.CancellationToken.ThrowIfCancellationRequested();
        counter++;
        intervalAction.Invoke(delegate
        {
          process.SetProcess($"Parent found {counter}/{process.Maximum} ", counter);
        });
        var id = dataRow[dataColumnID.Ordinal].ToString();
        if (string.IsNullOrEmpty(id))
          continue;
        var treeData = new TreeData
        {
          ID = id,
          Title = dataColumnDisplay1 != null
            ? dataColumnDisplay2 != null
              ? dataRow[dataColumnDisplay1.Ordinal] + " - " + dataRow[dataColumnDisplay2.Ordinal]
              : dataRow[
                dataColumnDisplay1.Ordinal].ToString()
            : id,
          ParentID = dataRow[dataColumnParent.Ordinal].ToString()
        };
        if (dataColumnDisplay1 != null)
          treeData.Tag = dataRow[dataColumnDisplay1.Ordinal].ToString();
        // Store the display
        if (!treeDataDictionary.ContainsKey(id))
          treeDataDictionary.Add(id, treeData);
      }

      // Generate a list of missing parents
      var additionalRootNodes = new HashSet<string>();
      foreach (var child in treeDataDictionary.Values)
      {
        if (!string.IsNullOrEmpty(child.ParentID) && !treeDataDictionary.ContainsKey(child.ParentID))
          additionalRootNodes.Add(child.ParentID);
      }

      var rootDataParentNotFound = new TreeData
      {
        ID = "{M}",
        Title = "Parent not found"
      };

      if (additionalRootNodes.Count > 0)
      {
        treeDataDictionary.Add(rootDataParentNotFound.ID, rootDataParentNotFound);
        counter = 0;
        process.Maximum = additionalRootNodes.Count;
        // Create new entries
        foreach (var parentID in additionalRootNodes)
        {
          process.CancellationToken.ThrowIfCancellationRequested();
          counter++;
          intervalAction.Invoke(delegate
          {
            process.SetProcess($"Parent not found (Step 1) {counter}/{process.Maximum} ", counter);
          });
          var childData = new TreeData
          {
            ParentID = rootDataParentNotFound.ID,
            ID = parentID,
            Title = $"{m_ComboBoxID.SelectedItem} - {parentID}"
          };
          treeDataDictionary.Add(parentID, childData);
        }
      }

      process.Maximum = treeDataDictionary.Values.Count;
      counter = 0;
      foreach (var child in treeDataDictionary.Values)
      {
        process.CancellationToken.ThrowIfCancellationRequested();
        counter++;
        intervalAction.Invoke(delegate
        {
          process.SetProcess($"Parent not found (Step 2) {counter}/{process.Maximum} ", counter);
        });
        if (string.IsNullOrEmpty(child.ParentID) && child.ID != rootDataParentFound.ID &&
            child.ID != rootDataParentNotFound.ID)
        {
          child.ParentID = rootDataParentFound.ID;
        }
      }

      process.Maximum = treeDataDictionary.Values.Count;
      counter = 0;
      // Fill m_Children for the new nodes
      foreach (var child in treeDataDictionary.Values)
      {
        process.CancellationToken.ThrowIfCancellationRequested();
        counter++;
        intervalAction.Invoke(delegate
        {
          process.SetProcess($"Set children {counter}/{process.Maximum} ", counter);
        });
        if (!string.IsNullOrEmpty(child.ParentID))
          treeDataDictionary[child.ParentID].Children.Add(child);
      }

      m_TreeData = treeDataDictionary.Values;
    }

    private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        m_TreeView.SuspendLayout();
        m_TreeView.BeginUpdate();
        m_TreeView.CollapseAll();
        m_TreeView.EndUpdate();
        m_TreeView.ResumeLayout();
      }
      catch (Exception ex)
      {
        this.ShowError(ex);
      }
    }

    /// <summary>
    ///   Handles the SelectionChangeCommitted event of the comboBox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (!(sender is ComboBox cb))
        return;
      var newValue = cb.SelectedItem.ToString();
      if (m_ComboBoxParentID == cb && m_ComboBoxID.SelectedItem != null &&
          m_ComboBoxID.SelectedItem.ToString() == newValue)
      {
        m_ComboBoxID.SelectedIndex = -1;
      }

      if (m_ComboBoxID == cb && m_ComboBoxParentID.SelectedItem != null &&
          m_ComboBoxParentID.SelectedItem.ToString() == newValue)
      {
        m_ComboBoxParentID.SelectedIndex = -1;
      }
      m_TimerDisplay.Stop();
      m_TimerDisplay.Start();
    }

    private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        m_TreeView.SuspendLayout();
        m_TreeView.BeginUpdate();
        m_TreeView.ExpandAll();
        m_TreeView.EndUpdate();
        m_TreeView.ResumeLayout();
      }
      catch (Exception ex)
      {
        this.ShowError(ex);
      }
    }

    private void FilterValueChangedElapsed(object sender, ElapsedEventArgs e) =>
      // go to UI Main thread
      m_TextBoxValue.Invoke((MethodInvoker)delegate
      {
        try
        {
          using (var proc = new FormProcessDisplay("Searching", false, m_CancellationTokenSource.Token))
          {
            proc.Show(this);
            Search(m_TextBoxValue.Text, m_TreeView.Nodes, proc.CancellationToken);
          }
        }
        catch (Exception ex)
        {
          this.ShowError(ex);
        }
      });

    /// <summary>
    ///   Handles the Load event of the HirachyDisplay control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void HirachyDisplay_Load(object sender, EventArgs e)
    {
      try
      {
        foreach (DataColumn col in m_DataTable.Columns)
        {
          m_ComboBoxID.Items.Add(col.ColumnName);
          m_ComboBoxDisplay1.Items.Add(col.ColumnName);
          m_ComboBoxDisplay2.Items.Add(col.ColumnName);
          m_ComboBoxParentID.Items.Add(col.ColumnName);
        }
      }
      catch (Exception ex)
      {
        this.ShowError(ex);
      }
    }

    /// <summary>
    ///   Required method for Designer support - do not modify the contents of this method with the
    ///   code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label label1;
      System.Windows.Forms.Label label2;
      System.Windows.Forms.Label label3;
      System.Windows.Forms.ContextMenuStrip contextMenuStrip;
      System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
      System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
      System.Windows.Forms.Label label4;
      this.m_TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.m_ComboBoxID = new System.Windows.Forms.ComboBox();
      this.m_ComboBoxParentID = new System.Windows.Forms.ComboBox();
      this.m_TreeView = new CsvTools.MultiselectTreeView();
      this.m_TextBoxValue = new System.Windows.Forms.TextBox();
      this.m_ComboBoxDisplay2 = new System.Windows.Forms.ComboBox();
      this.m_ComboBoxDisplay1 = new System.Windows.Forms.ComboBox();
      label1 = new System.Windows.Forms.Label();
      label2 = new System.Windows.Forms.Label();
      label3 = new System.Windows.Forms.Label();
      contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      label4 = new System.Windows.Forms.Label();
      contextMenuStrip.SuspendLayout();
      this.m_TableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(63, 9);
      label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(26, 20);
      label1.TabIndex = 3;
      label1.Text = "ID";
      // 
      // label2
      // 
      label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      label2.AutoSize = true;
      label2.Location = new System.Drawing.Point(29, 47);
      label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      label2.Name = "label2";
      label2.Size = new System.Drawing.Size(60, 20);
      label2.TabIndex = 5;
      label2.Text = "Display";
      // 
      // label3
      // 
      label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
      label3.AutoSize = true;
      label3.Location = new System.Drawing.Point(12, 85);
      label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      label3.Name = "label3";
      label3.Size = new System.Drawing.Size(77, 20);
      label3.TabIndex = 7;
      label3.Text = "Parent ID";
      // 
      // contextMenuStrip
      // 
      contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
      contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            expandAllToolStripMenuItem,
            closeAllToolStripMenuItem});
      contextMenuStrip.Name = "contextMenuStrip";
      contextMenuStrip.Size = new System.Drawing.Size(168, 68);
      // 
      // expandAllToolStripMenuItem
      // 
      expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
      expandAllToolStripMenuItem.Size = new System.Drawing.Size(167, 32);
      expandAllToolStripMenuItem.Text = "Expand All";
      expandAllToolStripMenuItem.Click += new System.EventHandler(this.ExpandAllToolStripMenuItem_Click);
      // 
      // closeAllToolStripMenuItem
      // 
      closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
      closeAllToolStripMenuItem.Size = new System.Drawing.Size(167, 32);
      closeAllToolStripMenuItem.Text = "Close All";
      closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
      // 
      // label4
      // 
      label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
      label4.AutoSize = true;
      label4.Location = new System.Drawing.Point(49, 122);
      label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      label4.Name = "label4";
      label4.Size = new System.Drawing.Size(40, 20);
      label4.TabIndex = 5;
      label4.Text = "Find";
      // 
      // m_TableLayoutPanel1
      // 
      this.m_TableLayoutPanel1.ColumnCount = 3;
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_TableLayoutPanel1.Controls.Add(label1, 0, 0);
      this.m_TableLayoutPanel1.Controls.Add(label2, 0, 1);
      this.m_TableLayoutPanel1.Controls.Add(label3, 0, 2);
      this.m_TableLayoutPanel1.Controls.Add(this.m_ComboBoxID, 1, 0);
      this.m_TableLayoutPanel1.Controls.Add(this.m_ComboBoxParentID, 1, 2);
      this.m_TableLayoutPanel1.Controls.Add(this.m_TreeView, 0, 4);
      this.m_TableLayoutPanel1.Controls.Add(this.m_TextBoxValue, 1, 3);
      this.m_TableLayoutPanel1.Controls.Add(label4, 0, 3);
      this.m_TableLayoutPanel1.Controls.Add(this.m_ComboBoxDisplay2, 2, 1);
      this.m_TableLayoutPanel1.Controls.Add(this.m_ComboBoxDisplay1, 1, 1);
      this.m_TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.m_TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_TableLayoutPanel1.Name = "m_TableLayoutPanel1";
      this.m_TableLayoutPanel1.RowCount = 5;
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.m_TableLayoutPanel1.Size = new System.Drawing.Size(698, 568);
      this.m_TableLayoutPanel1.TabIndex = 10;
      // 
      // m_ComboBoxID
      // 
      this.m_TableLayoutPanel1.SetColumnSpan(this.m_ComboBoxID, 2);
      this.m_ComboBoxID.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_ComboBoxID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_ComboBoxID.FormattingEnabled = true;
      this.m_ComboBoxID.Location = new System.Drawing.Point(97, 5);
      this.m_ComboBoxID.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_ComboBoxID.Name = "m_ComboBoxID";
      this.m_ComboBoxID.Size = new System.Drawing.Size(597, 28);
      this.m_ComboBoxID.TabIndex = 0;
      this.m_ComboBoxID.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectionChangeCommitted);
      // 
      // m_ComboBoxParentID
      // 
      this.m_TableLayoutPanel1.SetColumnSpan(this.m_ComboBoxParentID, 2);
      this.m_ComboBoxParentID.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_ComboBoxParentID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_ComboBoxParentID.FormattingEnabled = true;
      this.m_ComboBoxParentID.Location = new System.Drawing.Point(97, 81);
      this.m_ComboBoxParentID.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_ComboBoxParentID.Name = "m_ComboBoxParentID";
      this.m_ComboBoxParentID.Size = new System.Drawing.Size(597, 28);
      this.m_ComboBoxParentID.TabIndex = 1;
      this.m_ComboBoxParentID.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectionChangeCommitted);
      // 
      // m_TreeView
      // 
      this.m_TableLayoutPanel1.SetColumnSpan(this.m_TreeView, 3);
      this.m_TreeView.ContextMenuStrip = contextMenuStrip;
      this.m_TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_TreeView.Location = new System.Drawing.Point(4, 155);
      this.m_TreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_TreeView.Name = "m_TreeView";
      this.m_TreeView.Size = new System.Drawing.Size(690, 408);
      this.m_TreeView.TabIndex = 9;
      // 
      // m_TextBoxValue
      // 
      this.m_TextBoxValue.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_TextBoxValue.Location = new System.Drawing.Point(97, 119);
      this.m_TextBoxValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_TextBoxValue.Name = "m_TextBoxValue";
      this.m_TextBoxValue.Size = new System.Drawing.Size(294, 26);
      this.m_TextBoxValue.TabIndex = 2;
      this.m_TextBoxValue.TextChanged += new System.EventHandler(this.TimerSearchRestart);
      // 
      // m_ComboBoxDisplay2
      // 
      this.m_ComboBoxDisplay2.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_ComboBoxDisplay2.FormattingEnabled = true;
      this.m_ComboBoxDisplay2.Location = new System.Drawing.Point(399, 43);
      this.m_ComboBoxDisplay2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_ComboBoxDisplay2.Name = "m_ComboBoxDisplay2";
      this.m_ComboBoxDisplay2.Size = new System.Drawing.Size(295, 28);
      this.m_ComboBoxDisplay2.TabIndex = 15;
      this.m_ComboBoxDisplay2.SelectedIndexChanged += new System.EventHandler(this.TimeDisplayRestart);
      // 
      // m_ComboBoxDisplay1
      // 
      this.m_ComboBoxDisplay1.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_ComboBoxDisplay1.FormattingEnabled = true;
      this.m_ComboBoxDisplay1.Location = new System.Drawing.Point(97, 43);
      this.m_ComboBoxDisplay1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.m_ComboBoxDisplay1.Name = "m_ComboBoxDisplay1";
      this.m_ComboBoxDisplay1.Size = new System.Drawing.Size(294, 28);
      this.m_ComboBoxDisplay1.TabIndex = 16;
      this.m_ComboBoxDisplay1.SelectedIndexChanged += new System.EventHandler(this.TimeDisplayRestart);
      // 
      // FormHierachyDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(698, 568);
      this.Controls.Add(this.m_TableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MinimumSize = new System.Drawing.Size(499, 278);
      this.Name = "FormHierachyDisplay";
      this.Text = "Hierarchy";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormHierachyDisplay_FormClosed);
      this.Load += new System.EventHandler(this.HirachyDisplay_Load);
      contextMenuStrip.ResumeLayout(false);
      this.m_TableLayoutPanel1.ResumeLayout(false);
      this.m_TableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    private bool MarkInCycle(TreeData treeData, ICollection<TreeData> visitedEntries)
    {
      Contract.Requires(treeData != null);
      Contract.Requires(visitedEntries != null);
      if (visitedEntries.Contains(treeData))
      {
        treeData.InCycle = true;
        return true;
      }

      visitedEntries.Add(treeData);
      foreach (var child in treeData.Children)
      {
        if (MarkInCycle(child, visitedEntries))
          break;
      }

      return false;
    }

    private void Search(string text, TreeNodeCollection nodes, System.Threading.CancellationToken token)
    {
      if (nodes == null)
        return;
      if (nodes.Count == 0)
        return;
      token.ThrowIfCancellationRequested();
      foreach (TreeNode node in nodes)
      {
        if (node.Text.Contains(text))
        {
          m_TreeView.Select();
          node.EnsureVisible();
          m_TreeView.SelectedNode = node;
          return;
        }
      }

      foreach (TreeNode node in nodes)
        Search(text, node.Nodes, token);
    }

    /// <summary>
    ///   Shows the tree.
    /// </summary>
    private void ShowTree(IProcessDisplay process)
    {
      m_TreeView.BeginUpdate();
      try
      {
        m_TreeView.Nodes.Clear();
        if (m_TreeData == null)
          return;

        foreach (var treeData in m_TreeData)
          treeData.Visited = false;
        process.SetProcess("Adding Tree with children");
        foreach (var treeData in m_TreeData)
        {
          process.CancellationToken.ThrowIfCancellationRequested();
          Extensions.ProcessUIElements();
          if (string.IsNullOrEmpty(treeData.ParentID))
            AddTreeDataNodeWithChilds(treeData, null, process);
        }

        process.SetProcess("Finding Cycles");
        var hasCycles = false;
        foreach (var treeData in m_TreeData)
        {
          process.CancellationToken.ThrowIfCancellationRequested();
          Extensions.ProcessUIElements();
          if (!treeData.Visited)
          {
            hasCycles = true;
            break;
          }
        }

        if (!hasCycles)
          return;

        process.SetProcess("Adding Cycles");
        var rootNode = new TreeNode("Cycles in Hierarchy");
        m_TreeView.Nodes.Add(rootNode);

        foreach (var treeData in m_TreeData)
        {
          process.CancellationToken.ThrowIfCancellationRequested();
          Extensions.ProcessUIElements();
          if (!treeData.Visited)
            MarkInCycle(treeData, new HashSet<TreeData>());
        }

        foreach (var root in m_TreeData)
        {
          process.CancellationToken.ThrowIfCancellationRequested();
          Extensions.ProcessUIElements();
          if (!root.Visited && root.InCycle)
            AddTreeDataNodeWithChilds(root, rootNode, process);
        }
      }
      catch
      {
        m_TreeView.Nodes.Clear();
      }
      finally
      {
        m_TreeView.EndUpdate();
      }
    }

    private void TimerSearchRestart(object sender, EventArgs e)
    {
      m_TimerSearch.Stop();
      m_TimerSearch.Start();
    }

    [DebuggerDisplay("TreeData {ID} {ParentID} {Visited} Children:{Children.Count}")]
    internal class TreeData
    {
      public readonly ICollection<TreeData> Children = new List<TreeData>();

      public string ID;

      public bool InCycle;

      private int m_StoreIndirect = -1;

      public string ParentID;

      public string Tag;
      public string Title;
      public bool Visited;

      public int DirectCildren => Children.Count;

      public int InDirectCildren
      {
        get
        {
          if (m_StoreIndirect < 0)
            m_StoreIndirect = GetInDirectCildren(this);
          return m_StoreIndirect;
        }
      }

      public string NodeTitle
      {
        get
        {
          if (DirectCildren <= 0)
            return Title;
          return DirectCildren == InDirectCildren
            ? $"{Title} - Direct {DirectCildren}"
            : $"{Title} - Direct {DirectCildren} - Indirect {InDirectCildren}";
        }
      }

      private static int GetInDirectCildren(TreeData root)
      {
        if (root == null || root.InCycle)
          return 0;

        var returnVal = root.Children.Count;
        foreach (var child in root.Children)
        {
          if (child.Children.Count > 0)
            returnVal += GetInDirectCildren(child);
        }

        return returnVal;
      }
    }

    private void FormHierachyDisplay_FormClosed(object sender, FormClosedEventArgs e) => Dispose(true);

    private void Refresh_Click(object sender, EventArgs e) => m_TimerDisplay.Start();

    private void TimeDisplayRestart(object sender, EventArgs e)
    {
      m_TimerDisplay.Stop();
      if (m_BuildProcess != null && !m_BuildProcess.CancellationToken.IsCancellationRequested)
        m_BuildProcess.Cancel();
      m_TimerDisplay.Start();
    }
  }
}