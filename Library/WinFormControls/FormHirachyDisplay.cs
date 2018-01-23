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

using CsvToolLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
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
    private readonly DataRow[] m_DataRow;
    private readonly DataTable m_DataTable;
    private readonly Timer m_TimerChange = new Timer();

    /// <summary>
    ///   Required designer variable.
    /// </summary>
    private IContainer components;

    private ComboBox m_ComboBoxDisplay1;

    private ComboBox m_ComboBoxDisplay2;

    private ComboBox m_ComboBoxID;

    private ComboBox m_ComboBoxParentID;

    private FlowLayoutPanel m_FlowLayoutPanel1;
    private TableLayoutPanel m_TableLayoutPanel1;
    private TextBox m_TextBoxValue;
    private IEnumerable<TreeData> m_TreeData = new List<TreeData>();
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
      Icon = Resources.SubFormIcon;
      m_TimerChange.Elapsed += FilterValueChangedElapsed;
      m_TimerChange.Interval = 1000;
      m_TimerChange.AutoReset = false;
    }

    /// <summary>
    ///   Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) components?.Dispose();
      base.Dispose(disposing);
    }

    /// <summary>
    ///   Adds the tree data node with child's.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <param name="rootNode">The root node.</param>
    private void AddTreeDataNodeWithChilds(TreeData root, TreeNode rootNode)
    {
      Contract.Requires(root != null);
      root.Visited = true;
      var treeNode = new TreeNode(root.NodeTitle) { Tag = root };
      if (rootNode == null)
        m_TreeView.Nodes.Add(treeNode);
      else
        rootNode.Nodes.Add(treeNode);
      if (root.Children.Count > 0)
        treeNode.Nodes.AddRange(BuildSubNodes(root));
    }

    /// <summary>
    ///   Builds the sub nodes.
    /// </summary>
    /// <param name="parent">The parent ID.</param>
    /// <returns></returns>
    private TreeNode[] BuildSubNodes(TreeData parent)
    {
      Contract.Requires(parent != null);
      var treeNodes = new List<TreeNode>();
      foreach (var child in parent.Children)
        if (child.Visited)
        {
          var treeNode = new TreeNode("Cycle -> " + child.Title) { Tag = child };
          treeNodes.Add(treeNode);
        }
        else
        {
          child.Visited = true;
          var treeNode = new TreeNode(child.NodeTitle, BuildSubNodes(child)) { Tag = child };
          treeNodes.Add(treeNode);
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
        BuildTreeData();
        ShowTree();
      }
      catch (Exception exc)
      {
        MessageBox.Show(this, exc.ExceptionMessages(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Close();
      }
      finally
      {
        Cursor.Current = oldCursor;
      }
    }

    /// <summary>
    ///   Builds the tree data.
    /// </summary>
    private void BuildTreeData()
    {
      Contract.Requires(m_DataTable != null);

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
      var rootDataParentNotFound = new TreeData
      {
        ID = "{M}",
        Title = "Parent not found"
      };
      treeDataDictionary.Add(rootDataParentFound.ID, rootDataParentFound);

      foreach (var dataRow in m_DataRow)
      {
        var id = dataRow[dataColumnID.Ordinal].ToString();
        if (string.IsNullOrEmpty(id)) continue;
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
        if (!string.IsNullOrEmpty(child.ParentID) && !treeDataDictionary.ContainsKey(child.ParentID))
          additionalRootNodes.Add(child.ParentID);

      if (additionalRootNodes.Count > 0)
      {
        treeDataDictionary.Add(rootDataParentNotFound.ID, rootDataParentNotFound);

        // Create new entries
        foreach (var parentID in additionalRootNodes)
        {
          var childData = new TreeData
          {
            ParentID = rootDataParentNotFound.ID,
            ID = parentID,
            Title = $"{m_ComboBoxID.SelectedItem} - {parentID}"
          };
          treeDataDictionary.Add(parentID, childData);
        }
      }

      foreach (var child in treeDataDictionary.Values)
        if (string.IsNullOrEmpty(child.ParentID) && child.ID != rootDataParentFound.ID &&
            child.ID != rootDataParentNotFound.ID)
          child.ParentID = rootDataParentFound.ID;

      // Fill m_Children for the new nodes
      foreach (var child in treeDataDictionary.Values)
        if (!string.IsNullOrEmpty(child.ParentID))
          treeDataDictionary[child.ParentID].Children.Add(child);
      m_TreeData = treeDataDictionary.Values;
    }

    private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      m_TreeView.SuspendLayout();
      m_TreeView.BeginUpdate();
      m_TreeView.CollapseAll();
      m_TreeView.EndUpdate();
      m_TreeView.ResumeLayout();
    }

    /// <summary>
    ///   Handles the SelectionChangeCommitted event of the comboBox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void comboBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (!(sender is ComboBox cb))
        return;
      var newValue = cb.SelectedItem.ToString();
      if (m_ComboBoxParentID == cb && m_ComboBoxID.SelectedItem != null &&
          m_ComboBoxID.SelectedItem.ToString() == newValue)
        m_ComboBoxID.SelectedIndex = -1;
      if (m_ComboBoxID == cb && m_ComboBoxParentID.SelectedItem != null &&
          m_ComboBoxParentID.SelectedItem.ToString() == newValue) m_ComboBoxParentID.SelectedIndex = -1;

      if (m_ComboBoxID.SelectedItem != null &&
          m_ComboBoxParentID.SelectedItem != null)
        BuildTree();
    }

    private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      m_TreeView.SuspendLayout();
      m_TreeView.BeginUpdate();
      m_TreeView.ExpandAll();
      m_TreeView.EndUpdate();
      m_TreeView.ResumeLayout();
    }

    private void FilterValueChangedElapsed(object sender, ElapsedEventArgs e)
    {
      // go to UI Main thread
      m_TextBoxValue.Invoke((MethodInvoker)delegate { Search(m_TextBoxValue.Text, m_TreeView.Nodes); });
    }

    /// <summary>
    ///   Handles the Load event of the HirachyDisplay control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void HirachyDisplay_Load(object sender, EventArgs e)
    {
      foreach (DataColumn col in m_DataTable.Columns)
      {
        m_ComboBoxID.Items.Add(col.ColumnName);
        m_ComboBoxDisplay1.Items.Add(col.ColumnName);
        m_ComboBoxDisplay2.Items.Add(col.ColumnName);
        m_ComboBoxParentID.Items.Add(col.ColumnName);
      }
    }

    /// <summary>
    ///   Required method for Designer support - do not modify the contents of this method with the
    ///   code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new Container();
      m_TableLayoutPanel1 = new TableLayoutPanel();
      m_ComboBoxID = new ComboBox();
      m_ComboBoxParentID = new ComboBox();
      m_FlowLayoutPanel1 = new FlowLayoutPanel();
      m_ComboBoxDisplay1 = new ComboBox();
      m_ComboBoxDisplay2 = new ComboBox();
      m_TreeView = new MultiselectTreeView();
      m_TextBoxValue = new TextBox();
      var contextMenuStrip = new ContextMenuStrip(components);
      var expandAllToolStripMenuItem = new ToolStripMenuItem();
      var closeAllToolStripMenuItem = new ToolStripMenuItem();
      var label1 = new Label();
      var label2 = new Label();
      var label3 = new Label();
      var label4 = new Label();
      contextMenuStrip.SuspendLayout();
      m_TableLayoutPanel1.SuspendLayout();
      m_FlowLayoutPanel1.SuspendLayout();
      SuspendLayout();
      //
      // contextMenuStrip
      //
      contextMenuStrip.Items.AddRange(new ToolStripItem[]
      {
        expandAllToolStripMenuItem,
        closeAllToolStripMenuItem
      });
      contextMenuStrip.Name = "contextMenuStrip";
      contextMenuStrip.Size = new Size(130, 48);
      //
      // expandAllToolStripMenuItem
      //
      expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
      expandAllToolStripMenuItem.Size = new Size(129, 22);
      expandAllToolStripMenuItem.Text = "Expand All";
      expandAllToolStripMenuItem.Click += expandAllToolStripMenuItem_Click;
      //
      // closeAllToolStripMenuItem
      //
      closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
      closeAllToolStripMenuItem.Size = new Size(129, 22);
      closeAllToolStripMenuItem.Text = "Close All";
      closeAllToolStripMenuItem.Click += closeAllToolStripMenuItem_Click;
      //
      // label1
      //
      label1.Anchor = AnchorStyles.Right;
      label1.AutoSize = true;
      label1.Location = new Point(49, 5);
      label1.Name = "label1";
      label1.Size = new Size(18, 13);
      label1.TabIndex = 3;
      label1.Text = "ID";
      //
      // label2
      //
      label2.Anchor = AnchorStyles.Right;
      label2.AutoSize = true;
      label2.Location = new Point(26, 29);
      label2.Name = "label2";
      label2.Size = new Size(41, 13);
      label2.TabIndex = 5;
      label2.Text = "Display";
      //
      // label3
      //
      label3.Anchor = AnchorStyles.Right;
      label3.AutoSize = true;
      label3.Location = new Point(15, 53);
      label3.Name = "label3";
      label3.Size = new Size(52, 13);
      label3.TabIndex = 7;
      label3.Text = "Parent ID";
      //
      // label4
      //
      label4.Anchor = AnchorStyles.Right;
      label4.AutoSize = true;
      label4.Location = new Point(40, 77);
      label4.Name = "label4";
      label4.Size = new Size(27, 13);
      label4.TabIndex = 5;
      label4.Text = "Find";
      //
      // tableLayoutPanel1
      //
      m_TableLayoutPanel1.ColumnCount = 2;
      m_TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
      m_TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
      m_TableLayoutPanel1.Controls.Add(label1, 0, 0);
      m_TableLayoutPanel1.Controls.Add(label2, 0, 1);
      m_TableLayoutPanel1.Controls.Add(label3, 0, 2);
      m_TableLayoutPanel1.Controls.Add(m_ComboBoxID, 1, 0);
      m_TableLayoutPanel1.Controls.Add(m_ComboBoxParentID, 1, 2);
      m_TableLayoutPanel1.Controls.Add(m_FlowLayoutPanel1, 1, 1);
      m_TableLayoutPanel1.Controls.Add(m_TreeView, 0, 4);
      m_TableLayoutPanel1.Controls.Add(m_TextBoxValue, 1, 3);
      m_TableLayoutPanel1.Controls.Add(label4, 0, 3);
      m_TableLayoutPanel1.Dock = DockStyle.Fill;
      m_TableLayoutPanel1.Location = new Point(0, 0);
      m_TableLayoutPanel1.Name = "m_TableLayoutPanel1";
      m_TableLayoutPanel1.RowCount = 5;
      m_TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
      m_TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
      m_TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
      m_TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
      m_TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      m_TableLayoutPanel1.Size = new Size(376, 443);
      m_TableLayoutPanel1.TabIndex = 10;
      //
      // comboBoxID
      //
      m_ComboBoxID.Dock = DockStyle.Top;
      m_ComboBoxID.DropDownStyle = ComboBoxStyle.DropDownList;
      m_ComboBoxID.FormattingEnabled = true;
      m_ComboBoxID.Location = new Point(73, 3);
      m_ComboBoxID.Name = "m_ComboBoxID";
      m_ComboBoxID.Size = new Size(300, 21);
      m_ComboBoxID.TabIndex = 10;
      m_ComboBoxID.SelectionChangeCommitted += comboBox_SelectionChangeCommitted;
      //
      // comboBoxParentID
      //
      m_ComboBoxParentID.Dock = DockStyle.Top;
      m_ComboBoxParentID.DropDownStyle = ComboBoxStyle.DropDownList;
      m_ComboBoxParentID.FormattingEnabled = true;
      m_ComboBoxParentID.Location = new Point(73, 51);
      m_ComboBoxParentID.Name = "m_ComboBoxParentID";
      m_ComboBoxParentID.Size = new Size(300, 21);
      m_ComboBoxParentID.TabIndex = 12;
      m_ComboBoxParentID.SelectionChangeCommitted += comboBox_SelectionChangeCommitted;
      //
      // flowLayoutPanel1
      //
      m_FlowLayoutPanel1.Controls.Add(m_ComboBoxDisplay1);
      m_FlowLayoutPanel1.Controls.Add(m_ComboBoxDisplay2);
      m_FlowLayoutPanel1.Dock = DockStyle.Fill;
      m_FlowLayoutPanel1.Location = new Point(70, 24);
      m_FlowLayoutPanel1.Margin = new Padding(0);
      m_FlowLayoutPanel1.Name = "m_FlowLayoutPanel1";
      m_FlowLayoutPanel1.Size = new Size(306, 24);
      m_FlowLayoutPanel1.TabIndex = 14;
      //
      // comboBoxDisplay1
      //
      m_ComboBoxDisplay1.FormattingEnabled = true;
      m_ComboBoxDisplay1.Location = new Point(3, 3);
      m_ComboBoxDisplay1.Name = "m_ComboBoxDisplay1";
      m_ComboBoxDisplay1.Size = new Size(148, 21);
      m_ComboBoxDisplay1.TabIndex = 11;
      m_ComboBoxDisplay1.SelectedIndexChanged += comboBox_SelectionChangeCommitted;
      //
      // comboBoxDisplay2
      //
      m_ComboBoxDisplay2.FormattingEnabled = true;
      m_ComboBoxDisplay2.Location = new Point(157, 3);
      m_ComboBoxDisplay2.Name = "m_ComboBoxDisplay2";
      m_ComboBoxDisplay2.Size = new Size(137, 21);
      m_ComboBoxDisplay2.TabIndex = 11;
      m_ComboBoxDisplay2.SelectedIndexChanged += comboBox_SelectionChangeCommitted;
      //
      // treeView
      //
      m_TableLayoutPanel1.SetColumnSpan(m_TreeView, 2);
      m_TreeView.ContextMenuStrip = contextMenuStrip;
      m_TreeView.Dock = DockStyle.Fill;
      m_TreeView.Location = new Point(3, 99);
      m_TreeView.Name = "m_TreeView";
      m_TreeView.Size = new Size(370, 341);
      m_TreeView.TabIndex = 9;
      //
      // textBoxValue
      //
      m_TextBoxValue.Location = new Point(73, 75);
      m_TextBoxValue.Name = "m_TextBoxValue";
      m_TextBoxValue.Size = new Size(148, 20);
      m_TextBoxValue.TabIndex = 15;
      m_TextBoxValue.TextChanged += textBox1_TextChanged;
      //
      // FormHierachyDisplay
      //
      AutoScaleDimensions = new SizeF(6F, 13F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(376, 443);
      Controls.Add(m_TableLayoutPanel1);
      FormBorderStyle = FormBorderStyle.SizableToolWindow;
      MinimumSize = new Size(384, 245);
      Name = "FormHierachyDisplay";
      Text = "Hierarchy";
      Load += HirachyDisplay_Load;
      contextMenuStrip.ResumeLayout(false);
      m_TableLayoutPanel1.ResumeLayout(false);
      m_TableLayoutPanel1.PerformLayout();
      m_FlowLayoutPanel1.ResumeLayout(false);
      ResumeLayout(false);
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
        if (MarkInCycle(child, visitedEntries))
          break;
      return false;
    }

    private void Search(string text, TreeNodeCollection nodes)
    {
      if (nodes == null)
        return;
      if (nodes.Count == 0)
        return;
      foreach (TreeNode node in nodes)
        if (node.Text.Contains(text))
        {
          m_TreeView.Select();
          node.EnsureVisible();
          m_TreeView.SelectedNode = node;
          return;
        }

      foreach (TreeNode node in nodes)
        Search(text, node.Nodes);
    }

    /// <summary>
    ///   Shows the tree.
    /// </summary>
    private void ShowTree()
    {
      m_TreeView.BeginUpdate();
      try
      {
        m_TreeView.Nodes.Clear();
        if (m_TreeData == null)
          return;

        foreach (var treeData in m_TreeData)
          treeData.Visited = false;

        foreach (var treeData in m_TreeData)
          if (string.IsNullOrEmpty(treeData.ParentID))
            AddTreeDataNodeWithChilds(treeData, null);

        var hasCycles = false;
        foreach (var treeData in m_TreeData)
          if (!treeData.Visited)
          {
            hasCycles = true;
            break;
          }

        if (!hasCycles) return;
        {
          var rootNode = new TreeNode("Cycles in Hierarchy");
          m_TreeView.Nodes.Add(rootNode);

          foreach (var treeData in m_TreeData)
            if (!treeData.Visited)
              MarkInCycle(treeData, new HashSet<TreeData>());

          foreach (var root in m_TreeData)
            if (!root.Visited && root.InCycle)
              AddTreeDataNodeWithChilds(root, rootNode);
          // rootNode.ExpandAll();
        }
      }
      finally
      {
        m_TreeView.EndUpdate();
      }
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      m_TimerChange.Stop();
      m_TimerChange.Start();
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
          if (m_StoreIndirect < 0) m_StoreIndirect = GetInDirectCildren(this);
          return m_StoreIndirect;
        }
      }

      public string NodeTitle
      {
        get
        {
          if (DirectCildren <= 0) return Title;
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
          if (child.Children.Count > 0)
            returnVal += GetInDirectCildren(child);
        return returnVal;
      }
    }
  }
}