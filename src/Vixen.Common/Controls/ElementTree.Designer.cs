using System.Windows.Forms;
namespace Common.Controls
{
	partial class ElementTree
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
			if (disposing && (components != null)) {
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElementTree));
			treeIconsImageList = new ImageList(components);
			contextMenuStripTreeView = new ContextMenuStrip(components);
			cutNodesToolStripMenuItem = new ToolStripMenuItem();
			copyNodesToolStripMenuItem = new ToolStripMenuItem();
			pasteNodesToolStripMenuItem = new ToolStripMenuItem();
			pasteAsNewToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			nodePropertiesToolStripMenuItem = new ToolStripMenuItem();
			copyPropertiesToolStripMenuItem = new ToolStripMenuItem();
			pastePropertiesToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			addNewNodeToolStripMenuItem = new ToolStripMenuItem();
			addMultipleNewNodesToolStripMenuItem = new ToolStripMenuItem();
			deleteNodesToolStripMenuItem = new ToolStripMenuItem();
			createGroupWithNodesToolStripMenuItem = new ToolStripMenuItem();
			renameNodesToolStripMenuItem = new ToolStripMenuItem();
			patternRenameToolStripMenuItem = new ToolStripMenuItem();
			collapseAllToolStripMenuItem = new ToolStripMenuItem();
			reverseElementsToolStripMenuItem = new ToolStripMenuItem();
			sortToolStripMenuItem = new ToolStripMenuItem();
			exportWireDiagramToolStripMenuItem = new ToolStripMenuItem();
			frontWireDiagramToolStripMenuItem = new ToolStripMenuItem();
			backWireDiagramToolStripMenuItem = new ToolStripMenuItem();
			exportElementTreeToolStripMenuItem = new ToolStripMenuItem();
			contextMenuStripDragging = new ContextMenuStrip(components);
			moveHereToolStripMenuItem = new ToolStripMenuItem();
			copyHereToolStripMenuItem = new ToolStripMenuItem();
			treeview = new MultiSelectTreeview();
			refreshToolStripMenuItem = new ToolStripMenuItem();
			contextMenuStripTreeView.SuspendLayout();
			contextMenuStripDragging.SuspendLayout();
			SuspendLayout();
			// 
			// treeIconsImageList
			// 
			treeIconsImageList.ColorDepth = ColorDepth.Depth32Bit;
			treeIconsImageList.ImageStream = (ImageListStreamer)resources.GetObject("treeIconsImageList.ImageStream");
			treeIconsImageList.TransparentColor = Color.Transparent;
			treeIconsImageList.Images.SetKeyName(0, "Group");
			treeIconsImageList.Images.SetKeyName(1, "GreyBall");
			treeIconsImageList.Images.SetKeyName(2, "RedBall");
			treeIconsImageList.Images.SetKeyName(3, "GreenBall");
			treeIconsImageList.Images.SetKeyName(4, "YellowBall");
			treeIconsImageList.Images.SetKeyName(5, "BlueBall");
			treeIconsImageList.Images.SetKeyName(6, "WhiteBall");
			// 
			// contextMenuStripTreeView
			// 
			contextMenuStripTreeView.ImageScalingSize = new Size(24, 24);
			contextMenuStripTreeView.Items.AddRange(new ToolStripItem[] { cutNodesToolStripMenuItem, copyNodesToolStripMenuItem, pasteNodesToolStripMenuItem, pasteAsNewToolStripMenuItem, toolStripSeparator1, nodePropertiesToolStripMenuItem, toolStripSeparator2, addNewNodeToolStripMenuItem, addMultipleNewNodesToolStripMenuItem, deleteNodesToolStripMenuItem, createGroupWithNodesToolStripMenuItem, renameNodesToolStripMenuItem, patternRenameToolStripMenuItem, collapseAllToolStripMenuItem, reverseElementsToolStripMenuItem, sortToolStripMenuItem, exportWireDiagramToolStripMenuItem, exportElementTreeToolStripMenuItem, refreshToolStripMenuItem });
			contextMenuStripTreeView.Name = "contextMenuStripTreeView";
			contextMenuStripTreeView.Size = new Size(217, 412);
			contextMenuStripTreeView.Opening += contextMenuStripTreeView_Opening;
			// 
			// cutNodesToolStripMenuItem
			// 
			cutNodesToolStripMenuItem.Name = "cutNodesToolStripMenuItem";
			cutNodesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
			cutNodesToolStripMenuItem.Size = new Size(216, 22);
			cutNodesToolStripMenuItem.Text = "Cut";
			cutNodesToolStripMenuItem.Click += cutNodesToolStripMenuItem_Click;
			// 
			// copyNodesToolStripMenuItem
			// 
			copyNodesToolStripMenuItem.Name = "copyNodesToolStripMenuItem";
			copyNodesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
			copyNodesToolStripMenuItem.Size = new Size(216, 22);
			copyNodesToolStripMenuItem.Text = "Copy";
			copyNodesToolStripMenuItem.Click += copyNodesToolStripMenuItem_Click;
			// 
			// pasteNodesToolStripMenuItem
			// 
			pasteNodesToolStripMenuItem.Name = "pasteNodesToolStripMenuItem";
			pasteNodesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
			pasteNodesToolStripMenuItem.Size = new Size(216, 22);
			pasteNodesToolStripMenuItem.Text = "Paste";
			pasteNodesToolStripMenuItem.Click += pasteNodesToolStripMenuItem_Click;
			// 
			// pasteAsNewToolStripMenuItem
			// 
			pasteAsNewToolStripMenuItem.Enabled = false;
			pasteAsNewToolStripMenuItem.Name = "pasteAsNewToolStripMenuItem";
			pasteAsNewToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.V;
			pasteAsNewToolStripMenuItem.Size = new Size(216, 22);
			pasteAsNewToolStripMenuItem.Text = "Paste as New";
			pasteAsNewToolStripMenuItem.Click += pasteNodesAsNewToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(213, 6);
			// 
			// nodePropertiesToolStripMenuItem
			// 
			nodePropertiesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copyPropertiesToolStripMenuItem, pastePropertiesToolStripMenuItem });
			nodePropertiesToolStripMenuItem.Name = "nodePropertiesToolStripMenuItem";
			nodePropertiesToolStripMenuItem.Size = new Size(216, 22);
			nodePropertiesToolStripMenuItem.Text = "Item properties";
			// 
			// copyPropertiesToolStripMenuItem
			// 
			copyPropertiesToolStripMenuItem.Name = "copyPropertiesToolStripMenuItem";
			copyPropertiesToolStripMenuItem.Size = new Size(158, 22);
			copyPropertiesToolStripMenuItem.Text = "Copy properties";
			copyPropertiesToolStripMenuItem.Click += copyPropertiesToolStripMenuItem_Click;
			// 
			// pastePropertiesToolStripMenuItem
			// 
			pastePropertiesToolStripMenuItem.Name = "pastePropertiesToolStripMenuItem";
			pastePropertiesToolStripMenuItem.Size = new Size(158, 22);
			pastePropertiesToolStripMenuItem.Text = "Paste properties";
			pastePropertiesToolStripMenuItem.Click += pastePropertiesToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(213, 6);
			// 
			// addNewNodeToolStripMenuItem
			// 
			addNewNodeToolStripMenuItem.Name = "addNewNodeToolStripMenuItem";
			addNewNodeToolStripMenuItem.Size = new Size(216, 22);
			addNewNodeToolStripMenuItem.Text = "&Add";
			addNewNodeToolStripMenuItem.Click += addNewNodeToolStripMenuItem_Click;
			// 
			// addMultipleNewNodesToolStripMenuItem
			// 
			addMultipleNewNodesToolStripMenuItem.Name = "addMultipleNewNodesToolStripMenuItem";
			addMultipleNewNodesToolStripMenuItem.Size = new Size(216, 22);
			addMultipleNewNodesToolStripMenuItem.Text = "Add &Multiple";
			addMultipleNewNodesToolStripMenuItem.Click += addMultipleNewNodesToolStripMenuItem_Click;
			// 
			// deleteNodesToolStripMenuItem
			// 
			deleteNodesToolStripMenuItem.Name = "deleteNodesToolStripMenuItem";
			deleteNodesToolStripMenuItem.Size = new Size(216, 22);
			deleteNodesToolStripMenuItem.Text = "&Delete";
			deleteNodesToolStripMenuItem.Click += deleteNodesToolStripMenuItem_Click;
			// 
			// createGroupWithNodesToolStripMenuItem
			// 
			createGroupWithNodesToolStripMenuItem.Name = "createGroupWithNodesToolStripMenuItem";
			createGroupWithNodesToolStripMenuItem.Size = new Size(216, 22);
			createGroupWithNodesToolStripMenuItem.Text = "Create &Group";
			createGroupWithNodesToolStripMenuItem.Click += createGroupWithNodesToolStripMenuItem_Click;
			// 
			// renameNodesToolStripMenuItem
			// 
			renameNodesToolStripMenuItem.Name = "renameNodesToolStripMenuItem";
			renameNodesToolStripMenuItem.Size = new Size(216, 22);
			renameNodesToolStripMenuItem.Text = "Re&name";
			renameNodesToolStripMenuItem.Click += renameNodesToolStripMenuItem_Click;
			// 
			// patternRenameToolStripMenuItem
			// 
			patternRenameToolStripMenuItem.Name = "patternRenameToolStripMenuItem";
			patternRenameToolStripMenuItem.Size = new Size(216, 22);
			patternRenameToolStripMenuItem.Text = "Find/Replace";
			patternRenameToolStripMenuItem.Click += patternRenameToolStripMenuItem_Click;
			// 
			// collapseAllToolStripMenuItem
			// 
			collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
			collapseAllToolStripMenuItem.Size = new Size(216, 22);
			collapseAllToolStripMenuItem.Text = "&Collapse All";
			collapseAllToolStripMenuItem.Click += collapseAllToolStripMenuItem_Click;
			// 
			// reverseElementsToolStripMenuItem
			// 
			reverseElementsToolStripMenuItem.Name = "reverseElementsToolStripMenuItem";
			reverseElementsToolStripMenuItem.Size = new Size(216, 22);
			reverseElementsToolStripMenuItem.Text = "Reverse Elements";
			reverseElementsToolStripMenuItem.Click += reverseElementsToolStripMenuItem_Click;
			// 
			// sortToolStripMenuItem
			// 
			sortToolStripMenuItem.Name = "sortToolStripMenuItem";
			sortToolStripMenuItem.Size = new Size(216, 22);
			sortToolStripMenuItem.Text = "&Sort";
			sortToolStripMenuItem.Click += sortToolStripMenuItem_Click;
			// 
			// exportWireDiagramToolStripMenuItem
			// 
			exportWireDiagramToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { frontWireDiagramToolStripMenuItem, backWireDiagramToolStripMenuItem });
			exportWireDiagramToolStripMenuItem.Name = "exportWireDiagramToolStripMenuItem";
			exportWireDiagramToolStripMenuItem.Size = new Size(216, 22);
			exportWireDiagramToolStripMenuItem.Text = "Export Wire Diagram";
			// 
			// frontWireDiagramToolStripMenuItem
			// 
			frontWireDiagramToolStripMenuItem.Name = "frontWireDiagramToolStripMenuItem";
			frontWireDiagramToolStripMenuItem.Size = new Size(177, 22);
			frontWireDiagramToolStripMenuItem.Text = "Front Wire Diagram";
			frontWireDiagramToolStripMenuItem.Click += frontWireDiagramToolStripMenuItem_Click;
			// 
			// backWireDiagramToolStripMenuItem
			// 
			backWireDiagramToolStripMenuItem.Name = "backWireDiagramToolStripMenuItem";
			backWireDiagramToolStripMenuItem.Size = new Size(177, 22);
			backWireDiagramToolStripMenuItem.Text = "Back Wire Diagram";
			backWireDiagramToolStripMenuItem.Click += backWireDiagramToolStripMenuItem_Click;
			// 
			// exportElementTreeToolStripMenuItem
			// 
			exportElementTreeToolStripMenuItem.Name = "exportElementTreeToolStripMenuItem";
			exportElementTreeToolStripMenuItem.Size = new Size(216, 22);
			exportElementTreeToolStripMenuItem.Text = "Export Element Tree";
			exportElementTreeToolStripMenuItem.ToolTipText = "Export the element tree for importing into other profiles.";
			exportElementTreeToolStripMenuItem.Click += ExportElementTreeToolStripMenuItem_Click;
			// 
			// contextMenuStripDragging
			// 
			contextMenuStripDragging.ImageScalingSize = new Size(24, 24);
			contextMenuStripDragging.Items.AddRange(new ToolStripItem[] { moveHereToolStripMenuItem, copyHereToolStripMenuItem });
			contextMenuStripDragging.Name = "contextMenuStripProperties";
			contextMenuStripDragging.Size = new Size(133, 48);
			// 
			// moveHereToolStripMenuItem
			// 
			moveHereToolStripMenuItem.Name = "moveHereToolStripMenuItem";
			moveHereToolStripMenuItem.Size = new Size(132, 22);
			moveHereToolStripMenuItem.Text = "Move Here";
			// 
			// copyHereToolStripMenuItem
			// 
			copyHereToolStripMenuItem.Name = "copyHereToolStripMenuItem";
			copyHereToolStripMenuItem.Size = new Size(132, 22);
			copyHereToolStripMenuItem.Text = "Copy Here";
			// 
			// treeview
			// 
			treeview.AllowDrop = true;
			treeview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			treeview.ContextMenuStrip = contextMenuStripTreeView;
			treeview.CustomDragCursor = null;
			treeview.DragDefaultMode = DragDropEffects.Move;
			treeview.DragDestinationNodeBackColor = SystemColors.Highlight;
			treeview.DragDestinationNodeForeColor = SystemColors.HighlightText;
			treeview.DragSourceNodeBackColor = SystemColors.ControlLight;
			treeview.DragSourceNodeForeColor = SystemColors.ControlText;
			treeview.HideSelection = false;
			treeview.ImageIndex = 0;
			treeview.ImageList = treeIconsImageList;
			treeview.Location = new Point(0, 0);
			treeview.Margin = new Padding(4, 3, 4, 3);
			treeview.Name = "treeview";
			treeview.SelectedImageIndex = 0;
			treeview.SelectedNodes = (List<TreeNode>)resources.GetObject("treeview.SelectedNodes");
			treeview.Size = new Size(233, 461);
			treeview.TabIndex = 13;
			treeview.UsingCustomDragCursor = false;
			treeview.KeyDown += treeview_KeyDown;
			// 
			// refreshToolStripMenuItem
			// 
			refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			refreshToolStripMenuItem.Size = new Size(216, 22);
			refreshToolStripMenuItem.Text = "Refresh";
			refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
			// 
			// ElementTree
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(treeview);
			Margin = new Padding(4, 3, 4, 3);
			Name = "ElementTree";
			Size = new Size(233, 462);
			Load += ElementTree_Load;
			contextMenuStripTreeView.ResumeLayout(false);
			contextMenuStripDragging.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MultiSelectTreeview treeview;
		private System.Windows.Forms.ImageList treeIconsImageList;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeView;
		private System.Windows.Forms.ToolStripMenuItem cutNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem nodePropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyPropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pastePropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem addNewNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addMultipleNewNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createGroupWithNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameNodesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripDragging;
		private System.Windows.Forms.ToolStripMenuItem moveHereToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyHereToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteAsNewToolStripMenuItem;
		private ToolStripMenuItem collapseAllToolStripMenuItem;
		private ToolStripMenuItem reverseElementsToolStripMenuItem;
		private ToolStripMenuItem patternRenameToolStripMenuItem;
		private ToolStripMenuItem sortToolStripMenuItem;
		private ToolStripMenuItem exportWireDiagramToolStripMenuItem;
		private ToolStripMenuItem exportElementTreeToolStripMenuItem;
		private ToolStripMenuItem frontWireDiagramToolStripMenuItem;
		private ToolStripMenuItem backWireDiagramToolStripMenuItem;
		private ToolStripMenuItem refreshToolStripMenuItem;
	}
}
