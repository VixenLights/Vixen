namespace VixenApplication
{
	partial class ConfigChannels
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("test-channel");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("test-group");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("test-patch");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("something else");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigChannels));
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxSelectedNode = new System.Windows.Forms.GroupBox();
			this.groupBoxAddPatch = new System.Windows.Forms.GroupBox();
			this.buttonAddPatch = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDownPatchOutputSelect = new System.Windows.Forms.NumericUpDown();
			this.comboBoxPatchControllerSelect = new System.Windows.Forms.ComboBox();
			this.groupBoxPatches = new System.Windows.Forms.GroupBox();
			this.checkBoxDisableOutputs = new System.Windows.Forms.CheckBox();
			this.buttonRemovePatch = new System.Windows.Forms.Button();
			this.listViewPatches = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBoxProperties = new System.Windows.Forms.GroupBox();
			this.buttonConfigureProperty = new System.Windows.Forms.Button();
			this.buttonRemoveProperty = new System.Windows.Forms.Button();
			this.buttonAddProperty = new System.Windows.Forms.Button();
			this.listViewProperties = new System.Windows.Forms.ListView();
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelParents = new System.Windows.Forms.Label();
			this.buttonRenameItem = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.treeIconsImageList = new System.Windows.Forms.ImageList(this.components);
			this.groupBoxOperations = new System.Windows.Forms.GroupBox();
			this.buttonDeleteNode = new System.Windows.Forms.Button();
			this.buttonBulkRename = new System.Windows.Forms.Button();
			this.buttonCreateGroup = new System.Windows.Forms.Button();
			this.buttonAddNode = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.multiSelectTreeviewChannelsGroups = new CommonElements.MultiSelectTreeview();
			this.contextMenuStripTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.nodePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pastePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.addNewNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createGroupWithNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripDragging = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.moveHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxSelectedNode.SuspendLayout();
			this.groupBoxAddPatch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPatchOutputSelect)).BeginInit();
			this.groupBoxPatches.SuspendLayout();
			this.groupBoxProperties.SuspendLayout();
			this.groupBoxOperations.SuspendLayout();
			this.contextMenuStripTreeView.SuspendLayout();
			this.contextMenuStripDragging.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(509, 645);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// groupBoxSelectedNode
			// 
			this.groupBoxSelectedNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedNode.Controls.Add(this.groupBoxAddPatch);
			this.groupBoxSelectedNode.Controls.Add(this.groupBoxPatches);
			this.groupBoxSelectedNode.Controls.Add(this.groupBoxProperties);
			this.groupBoxSelectedNode.Controls.Add(this.labelParents);
			this.groupBoxSelectedNode.Controls.Add(this.buttonRenameItem);
			this.groupBoxSelectedNode.Controls.Add(this.label2);
			this.groupBoxSelectedNode.Controls.Add(this.textBoxName);
			this.groupBoxSelectedNode.Location = new System.Drawing.Point(318, 137);
			this.groupBoxSelectedNode.Name = "groupBoxSelectedNode";
			this.groupBoxSelectedNode.Size = new System.Drawing.Size(281, 499);
			this.groupBoxSelectedNode.TabIndex = 11;
			this.groupBoxSelectedNode.TabStop = false;
			this.groupBoxSelectedNode.Text = "Selected Node";
			// 
			// groupBoxAddPatch
			// 
			this.groupBoxAddPatch.Controls.Add(this.buttonAddPatch);
			this.groupBoxAddPatch.Controls.Add(this.label3);
			this.groupBoxAddPatch.Controls.Add(this.label1);
			this.groupBoxAddPatch.Controls.Add(this.numericUpDownPatchOutputSelect);
			this.groupBoxAddPatch.Controls.Add(this.comboBoxPatchControllerSelect);
			this.groupBoxAddPatch.Location = new System.Drawing.Point(6, 253);
			this.groupBoxAddPatch.Name = "groupBoxAddPatch";
			this.groupBoxAddPatch.Size = new System.Drawing.Size(269, 87);
			this.groupBoxAddPatch.TabIndex = 32;
			this.groupBoxAddPatch.TabStop = false;
			this.groupBoxAddPatch.Text = "Add Patch";
			// 
			// buttonAddPatch
			// 
			this.buttonAddPatch.Location = new System.Drawing.Point(150, 49);
			this.buttonAddPatch.Name = "buttonAddPatch";
			this.buttonAddPatch.Size = new System.Drawing.Size(80, 25);
			this.buttonAddPatch.TabIndex = 26;
			this.buttonAddPatch.Text = "Add Patch";
			this.buttonAddPatch.UseVisualStyleBackColor = true;
			this.buttonAddPatch.Click += new System.EventHandler(this.buttonAddPatch_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 13);
			this.label3.TabIndex = 25;
			this.label3.Text = "Output:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 24;
			this.label1.Text = "Controller:";
			// 
			// numericUpDownPatchOutputSelect
			// 
			this.numericUpDownPatchOutputSelect.Location = new System.Drawing.Point(66, 52);
			this.numericUpDownPatchOutputSelect.Name = "numericUpDownPatchOutputSelect";
			this.numericUpDownPatchOutputSelect.Size = new System.Drawing.Size(55, 20);
			this.numericUpDownPatchOutputSelect.TabIndex = 23;
			// 
			// comboBoxPatchControllerSelect
			// 
			this.comboBoxPatchControllerSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPatchControllerSelect.FormattingEnabled = true;
			this.comboBoxPatchControllerSelect.Location = new System.Drawing.Point(66, 19);
			this.comboBoxPatchControllerSelect.Name = "comboBoxPatchControllerSelect";
			this.comboBoxPatchControllerSelect.Size = new System.Drawing.Size(184, 21);
			this.comboBoxPatchControllerSelect.TabIndex = 22;
			this.comboBoxPatchControllerSelect.SelectedIndexChanged += new System.EventHandler(this.comboBoxPatchControllerSelect_SelectedIndexChanged);
			// 
			// groupBoxPatches
			// 
			this.groupBoxPatches.Controls.Add(this.checkBoxDisableOutputs);
			this.groupBoxPatches.Controls.Add(this.buttonRemovePatch);
			this.groupBoxPatches.Controls.Add(this.listViewPatches);
			this.groupBoxPatches.Location = new System.Drawing.Point(6, 81);
			this.groupBoxPatches.Name = "groupBoxPatches";
			this.groupBoxPatches.Size = new System.Drawing.Size(269, 166);
			this.groupBoxPatches.TabIndex = 27;
			this.groupBoxPatches.TabStop = false;
			this.groupBoxPatches.Text = "Channel Patches";
			// 
			// checkBoxDisableOutputs
			// 
			this.checkBoxDisableOutputs.AutoSize = true;
			this.checkBoxDisableOutputs.Location = new System.Drawing.Point(16, 25);
			this.checkBoxDisableOutputs.Name = "checkBoxDisableOutputs";
			this.checkBoxDisableOutputs.Size = new System.Drawing.Size(153, 17);
			this.checkBoxDisableOutputs.TabIndex = 30;
			this.checkBoxDisableOutputs.Text = "Disable all channel outputs";
			this.checkBoxDisableOutputs.UseVisualStyleBackColor = true;
			this.checkBoxDisableOutputs.CheckedChanged += new System.EventHandler(this.checkBoxDisableOutputs_CheckedChanged);
			// 
			// buttonRemovePatch
			// 
			this.buttonRemovePatch.Location = new System.Drawing.Point(16, 130);
			this.buttonRemovePatch.Name = "buttonRemovePatch";
			this.buttonRemovePatch.Size = new System.Drawing.Size(100, 25);
			this.buttonRemovePatch.TabIndex = 18;
			this.buttonRemovePatch.Text = "Remove Patch";
			this.buttonRemovePatch.UseVisualStyleBackColor = true;
			this.buttonRemovePatch.Click += new System.EventHandler(this.buttonRemovePatch_Click);
			// 
			// listViewPatches
			// 
			this.listViewPatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewPatches.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewPatches.HideSelection = false;
			this.listViewPatches.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
			this.listViewPatches.Location = new System.Drawing.Point(16, 50);
			this.listViewPatches.Name = "listViewPatches";
			this.listViewPatches.Size = new System.Drawing.Size(234, 74);
			this.listViewPatches.TabIndex = 17;
			this.listViewPatches.UseCompatibleStateImageBehavior = false;
			this.listViewPatches.View = System.Windows.Forms.View.Details;
			this.listViewPatches.SelectedIndexChanged += new System.EventHandler(this.listViewPatches_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "";
			this.columnHeader1.Width = 178;
			// 
			// groupBoxProperties
			// 
			this.groupBoxProperties.Controls.Add(this.buttonConfigureProperty);
			this.groupBoxProperties.Controls.Add(this.buttonRemoveProperty);
			this.groupBoxProperties.Controls.Add(this.buttonAddProperty);
			this.groupBoxProperties.Controls.Add(this.listViewProperties);
			this.groupBoxProperties.Location = new System.Drawing.Point(6, 346);
			this.groupBoxProperties.Name = "groupBoxProperties";
			this.groupBoxProperties.Size = new System.Drawing.Size(269, 144);
			this.groupBoxProperties.TabIndex = 26;
			this.groupBoxProperties.TabStop = false;
			this.groupBoxProperties.Text = "Properties";
			// 
			// buttonConfigureProperty
			// 
			this.buttonConfigureProperty.Location = new System.Drawing.Point(98, 105);
			this.buttonConfigureProperty.Name = "buttonConfigureProperty";
			this.buttonConfigureProperty.Size = new System.Drawing.Size(70, 25);
			this.buttonConfigureProperty.TabIndex = 20;
			this.buttonConfigureProperty.Text = "Configure";
			this.buttonConfigureProperty.UseVisualStyleBackColor = true;
			this.buttonConfigureProperty.Click += new System.EventHandler(this.buttonConfigureProperty_Click);
			// 
			// buttonRemoveProperty
			// 
			this.buttonRemoveProperty.Location = new System.Drawing.Point(180, 105);
			this.buttonRemoveProperty.Name = "buttonRemoveProperty";
			this.buttonRemoveProperty.Size = new System.Drawing.Size(70, 25);
			this.buttonRemoveProperty.TabIndex = 19;
			this.buttonRemoveProperty.Text = "Remove";
			this.buttonRemoveProperty.UseVisualStyleBackColor = true;
			this.buttonRemoveProperty.Click += new System.EventHandler(this.buttonRemoveProperty_Click);
			// 
			// buttonAddProperty
			// 
			this.buttonAddProperty.Location = new System.Drawing.Point(16, 105);
			this.buttonAddProperty.Name = "buttonAddProperty";
			this.buttonAddProperty.Size = new System.Drawing.Size(70, 25);
			this.buttonAddProperty.TabIndex = 17;
			this.buttonAddProperty.Text = "Add";
			this.buttonAddProperty.UseVisualStyleBackColor = true;
			this.buttonAddProperty.Click += new System.EventHandler(this.buttonAddProperty_Click);
			// 
			// listViewProperties
			// 
			this.listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
			this.listViewProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewProperties.HideSelection = false;
			this.listViewProperties.Location = new System.Drawing.Point(16, 24);
			this.listViewProperties.Name = "listViewProperties";
			this.listViewProperties.Size = new System.Drawing.Size(234, 75);
			this.listViewProperties.TabIndex = 15;
			this.listViewProperties.UseCompatibleStateImageBehavior = false;
			this.listViewProperties.View = System.Windows.Forms.View.Details;
			this.listViewProperties.SelectedIndexChanged += new System.EventHandler(this.listViewProperties_SelectedIndexChanged);
			this.listViewProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewProperties_MouseDoubleClick);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 200;
			// 
			// labelParents
			// 
			this.labelParents.AutoEllipsis = true;
			this.labelParents.Location = new System.Drawing.Point(7, 48);
			this.labelParents.Name = "labelParents";
			this.labelParents.Size = new System.Drawing.Size(268, 28);
			this.labelParents.TabIndex = 24;
			this.labelParents.Text = "This node is in <x> groups:\r\nasdfasdfasdfasdfasdfasdfasdfasdfqwerqwerqwerqwer\r\n23" +
				"4123412341234123412341234";
			// 
			// buttonRenameItem
			// 
			this.buttonRenameItem.Location = new System.Drawing.Point(201, 18);
			this.buttonRenameItem.Name = "buttonRenameItem";
			this.buttonRenameItem.Size = new System.Drawing.Size(70, 25);
			this.buttonRenameItem.TabIndex = 19;
			this.buttonRenameItem.Text = "Rename";
			this.buttonRenameItem.UseVisualStyleBackColor = true;
			this.buttonRenameItem.Click += new System.EventHandler(this.buttonRenameItem_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(47, 21);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(148, 20);
			this.textBoxName.TabIndex = 11;
			// 
			// treeIconsImageList
			// 
			this.treeIconsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeIconsImageList.ImageStream")));
			this.treeIconsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.treeIconsImageList.Images.SetKeyName(0, "Group");
			this.treeIconsImageList.Images.SetKeyName(1, "GreyBall");
			this.treeIconsImageList.Images.SetKeyName(2, "RedBall");
			this.treeIconsImageList.Images.SetKeyName(3, "GreenBall");
			this.treeIconsImageList.Images.SetKeyName(4, "YellowBall");
			this.treeIconsImageList.Images.SetKeyName(5, "BlueBall");
			this.treeIconsImageList.Images.SetKeyName(6, "WhiteBall");
			// 
			// groupBoxOperations
			// 
			this.groupBoxOperations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOperations.Controls.Add(this.buttonDeleteNode);
			this.groupBoxOperations.Controls.Add(this.buttonBulkRename);
			this.groupBoxOperations.Controls.Add(this.buttonCreateGroup);
			this.groupBoxOperations.Controls.Add(this.buttonAddNode);
			this.groupBoxOperations.Location = new System.Drawing.Point(318, 12);
			this.groupBoxOperations.Name = "groupBoxOperations";
			this.groupBoxOperations.Size = new System.Drawing.Size(281, 119);
			this.groupBoxOperations.TabIndex = 23;
			this.groupBoxOperations.TabStop = false;
			this.groupBoxOperations.Text = "Operations";
			// 
			// buttonDeleteNode
			// 
			this.buttonDeleteNode.Location = new System.Drawing.Point(146, 21);
			this.buttonDeleteNode.Name = "buttonDeleteNode";
			this.buttonDeleteNode.Size = new System.Drawing.Size(120, 25);
			this.buttonDeleteNode.TabIndex = 26;
			this.buttonDeleteNode.Text = "Delete Node";
			this.buttonDeleteNode.UseVisualStyleBackColor = true;
			this.buttonDeleteNode.Click += new System.EventHandler(this.buttonDeleteNode_Click);
			// 
			// buttonBulkRename
			// 
			this.buttonBulkRename.Location = new System.Drawing.Point(16, 83);
			this.buttonBulkRename.Name = "buttonBulkRename";
			this.buttonBulkRename.Size = new System.Drawing.Size(250, 25);
			this.buttonBulkRename.TabIndex = 25;
			this.buttonBulkRename.Text = "Bulk Rename Selected Nodes";
			this.buttonBulkRename.UseVisualStyleBackColor = true;
			this.buttonBulkRename.Click += new System.EventHandler(this.buttonBulkRename_Click);
			// 
			// buttonCreateGroup
			// 
			this.buttonCreateGroup.Location = new System.Drawing.Point(16, 52);
			this.buttonCreateGroup.Name = "buttonCreateGroup";
			this.buttonCreateGroup.Size = new System.Drawing.Size(250, 25);
			this.buttonCreateGroup.TabIndex = 24;
			this.buttonCreateGroup.Text = "Create Group from Selected Nodes";
			this.buttonCreateGroup.UseVisualStyleBackColor = true;
			this.buttonCreateGroup.Click += new System.EventHandler(this.buttonCreateGroup_Click);
			// 
			// buttonAddNode
			// 
			this.buttonAddNode.Location = new System.Drawing.Point(16, 21);
			this.buttonAddNode.Name = "buttonAddNode";
			this.buttonAddNode.Size = new System.Drawing.Size(120, 25);
			this.buttonAddNode.TabIndex = 23;
			this.buttonAddNode.Text = "Add Node";
			this.buttonAddNode.UseVisualStyleBackColor = true;
			this.buttonAddNode.Click += new System.EventHandler(this.buttonAddNode_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 643);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(244, 13);
			this.label4.TabIndex = 24;
			this.label4.Text = "Click and drag to move nodes in the configuration.";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 660);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(299, 13);
			this.label5.TabIndex = 25;
			this.label5.Text = "Hold down CTRL while dragging to copy nodes to destination.";
			// 
			// multiSelectTreeviewChannelsGroups
			// 
			this.multiSelectTreeviewChannelsGroups.AllowDrop = true;
			this.multiSelectTreeviewChannelsGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.multiSelectTreeviewChannelsGroups.ContextMenuStrip = this.contextMenuStripTreeView;
			this.multiSelectTreeviewChannelsGroups.Cursor = System.Windows.Forms.Cursors.Default;
			this.multiSelectTreeviewChannelsGroups.CustomDragCursor = null;
			this.multiSelectTreeviewChannelsGroups.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
			this.multiSelectTreeviewChannelsGroups.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.multiSelectTreeviewChannelsGroups.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.multiSelectTreeviewChannelsGroups.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.multiSelectTreeviewChannelsGroups.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.multiSelectTreeviewChannelsGroups.HideSelection = false;
			this.multiSelectTreeviewChannelsGroups.ImageIndex = 0;
			this.multiSelectTreeviewChannelsGroups.ImageList = this.treeIconsImageList;
			this.multiSelectTreeviewChannelsGroups.Location = new System.Drawing.Point(12, 12);
			this.multiSelectTreeviewChannelsGroups.Name = "multiSelectTreeviewChannelsGroups";
			this.multiSelectTreeviewChannelsGroups.SelectedImageIndex = 0;
			this.multiSelectTreeviewChannelsGroups.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("multiSelectTreeviewChannelsGroups.SelectedNodes")));
			this.multiSelectTreeviewChannelsGroups.Size = new System.Drawing.Size(300, 624);
			this.multiSelectTreeviewChannelsGroups.TabIndex = 12;
			this.multiSelectTreeviewChannelsGroups.UsingCustomDragCursor = false;
			this.multiSelectTreeviewChannelsGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.multiSelectTreeviewChannelsGroups_AfterSelect);
			// 
			// contextMenuStripTreeView
			// 
			this.contextMenuStripTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutNodesToolStripMenuItem,
            this.copyNodesToolStripMenuItem,
            this.pasteNodesToolStripMenuItem,
            this.toolStripSeparator1,
            this.nodePropertiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.addNewNodeToolStripMenuItem,
            this.deleteNodesToolStripMenuItem,
            this.createGroupWithNodesToolStripMenuItem,
            this.renameNodesToolStripMenuItem});
			this.contextMenuStripTreeView.Name = "contextMenuStripTreeView";
			this.contextMenuStripTreeView.Size = new System.Drawing.Size(205, 192);
			this.contextMenuStripTreeView.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTreeView_Opening);
			// 
			// cutNodesToolStripMenuItem
			// 
			this.cutNodesToolStripMenuItem.Name = "cutNodesToolStripMenuItem";
			this.cutNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.cutNodesToolStripMenuItem.Text = "Cut nodes";
			this.cutNodesToolStripMenuItem.Click += new System.EventHandler(this.cutNodesToolStripMenuItem_Click);
			// 
			// copyNodesToolStripMenuItem
			// 
			this.copyNodesToolStripMenuItem.Name = "copyNodesToolStripMenuItem";
			this.copyNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.copyNodesToolStripMenuItem.Text = "Copy nodes";
			this.copyNodesToolStripMenuItem.Click += new System.EventHandler(this.copyNodesToolStripMenuItem_Click);
			// 
			// pasteNodesToolStripMenuItem
			// 
			this.pasteNodesToolStripMenuItem.Name = "pasteNodesToolStripMenuItem";
			this.pasteNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.pasteNodesToolStripMenuItem.Text = "Paste nodes";
			this.pasteNodesToolStripMenuItem.Click += new System.EventHandler(this.pasteNodesToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
			// 
			// nodePropertiesToolStripMenuItem
			// 
			this.nodePropertiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPropertiesToolStripMenuItem,
            this.pastePropertiesToolStripMenuItem});
			this.nodePropertiesToolStripMenuItem.Name = "nodePropertiesToolStripMenuItem";
			this.nodePropertiesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.nodePropertiesToolStripMenuItem.Text = "Node properties";
			// 
			// copyPropertiesToolStripMenuItem
			// 
			this.copyPropertiesToolStripMenuItem.Name = "copyPropertiesToolStripMenuItem";
			this.copyPropertiesToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.copyPropertiesToolStripMenuItem.Text = "Copy properties";
			this.copyPropertiesToolStripMenuItem.Click += new System.EventHandler(this.copyPropertiesToolStripMenuItem_Click);
			// 
			// pastePropertiesToolStripMenuItem
			// 
			this.pastePropertiesToolStripMenuItem.Name = "pastePropertiesToolStripMenuItem";
			this.pastePropertiesToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.pastePropertiesToolStripMenuItem.Text = "Paste properties";
			this.pastePropertiesToolStripMenuItem.Click += new System.EventHandler(this.pastePropertiesToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
			// 
			// addNewNodeToolStripMenuItem
			// 
			this.addNewNodeToolStripMenuItem.Name = "addNewNodeToolStripMenuItem";
			this.addNewNodeToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.addNewNodeToolStripMenuItem.Text = "Add new node";
			this.addNewNodeToolStripMenuItem.Click += new System.EventHandler(this.addNewNodeToolStripMenuItem_Click);
			// 
			// deleteNodesToolStripMenuItem
			// 
			this.deleteNodesToolStripMenuItem.Name = "deleteNodesToolStripMenuItem";
			this.deleteNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.deleteNodesToolStripMenuItem.Text = "Delete nodes";
			this.deleteNodesToolStripMenuItem.Click += new System.EventHandler(this.deleteNodesToolStripMenuItem_Click);
			// 
			// createGroupWithNodesToolStripMenuItem
			// 
			this.createGroupWithNodesToolStripMenuItem.Name = "createGroupWithNodesToolStripMenuItem";
			this.createGroupWithNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.createGroupWithNodesToolStripMenuItem.Text = "Create group with nodes";
			this.createGroupWithNodesToolStripMenuItem.Click += new System.EventHandler(this.createGroupWithNodesToolStripMenuItem_Click);
			// 
			// renameNodesToolStripMenuItem
			// 
			this.renameNodesToolStripMenuItem.Name = "renameNodesToolStripMenuItem";
			this.renameNodesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.renameNodesToolStripMenuItem.Text = "Rename nodes";
			this.renameNodesToolStripMenuItem.Click += new System.EventHandler(this.renameNodesToolStripMenuItem_Click);
			// 
			// contextMenuStripDragging
			// 
			this.contextMenuStripDragging.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveHereToolStripMenuItem,
            this.copyHereToolStripMenuItem});
			this.contextMenuStripDragging.Name = "contextMenuStripProperties";
			this.contextMenuStripDragging.Size = new System.Drawing.Size(131, 48);
			// 
			// moveHereToolStripMenuItem
			// 
			this.moveHereToolStripMenuItem.Name = "moveHereToolStripMenuItem";
			this.moveHereToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			this.moveHereToolStripMenuItem.Text = "Move here";
			// 
			// copyHereToolStripMenuItem
			// 
			this.copyHereToolStripMenuItem.Name = "copyHereToolStripMenuItem";
			this.copyHereToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			this.copyHereToolStripMenuItem.Text = "Copy here";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(413, 645);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 26;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// ConfigChannels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(611, 682);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBoxOperations);
			this.Controls.Add(this.multiSelectTreeviewChannelsGroups);
			this.Controls.Add(this.groupBoxSelectedNode);
			this.Controls.Add(this.buttonOk);
			this.DoubleBuffered = true;
			this.Name = "ConfigChannels";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Channel & Group Configuration";
			this.Load += new System.EventHandler(this.ConfigChannels_Load);
			this.groupBoxSelectedNode.ResumeLayout(false);
			this.groupBoxSelectedNode.PerformLayout();
			this.groupBoxAddPatch.ResumeLayout(false);
			this.groupBoxAddPatch.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPatchOutputSelect)).EndInit();
			this.groupBoxPatches.ResumeLayout(false);
			this.groupBoxPatches.PerformLayout();
			this.groupBoxProperties.ResumeLayout(false);
			this.groupBoxOperations.ResumeLayout(false);
			this.contextMenuStripTreeView.ResumeLayout(false);
			this.contextMenuStripDragging.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBoxSelectedNode;
		private CommonElements.MultiSelectTreeview multiSelectTreeviewChannelsGroups;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonRenameItem;
		private System.Windows.Forms.Label labelParents;
		private System.Windows.Forms.ImageList treeIconsImageList;
		private System.Windows.Forms.GroupBox groupBoxPatches;
		private System.Windows.Forms.Button buttonRemovePatch;
		private System.Windows.Forms.ListView listViewPatches;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.GroupBox groupBoxProperties;
		private System.Windows.Forms.Button buttonRemoveProperty;
		private System.Windows.Forms.Button buttonAddProperty;
		private System.Windows.Forms.ListView listViewProperties;
		private System.Windows.Forms.CheckBox checkBoxDisableOutputs;
		private System.Windows.Forms.Button buttonConfigureProperty;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.GroupBox groupBoxAddPatch;
		private System.Windows.Forms.Button buttonAddPatch;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownPatchOutputSelect;
		private System.Windows.Forms.ComboBox comboBoxPatchControllerSelect;
		private System.Windows.Forms.GroupBox groupBoxOperations;
		private System.Windows.Forms.Button buttonDeleteNode;
		private System.Windows.Forms.Button buttonBulkRename;
		private System.Windows.Forms.Button buttonCreateGroup;
		private System.Windows.Forms.Button buttonAddNode;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeView;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripDragging;
		private System.Windows.Forms.ToolStripMenuItem cutNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem nodePropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyPropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pastePropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem addNewNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createGroupWithNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveHereToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyHereToolStripMenuItem;
		private System.Windows.Forms.Button buttonCancel;
	}
}