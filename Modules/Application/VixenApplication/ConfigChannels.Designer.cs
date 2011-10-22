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
			System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("test-channel");
			System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("test-group");
			System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("test-patch");
			System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("something else");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigChannels));
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxSelectedNode = new System.Windows.Forms.GroupBox();
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
			this.multiSelectTreeviewChannelsGroups = new CommonElements.MultiSelectTreeview();
			this.groupBoxOperations = new System.Windows.Forms.GroupBox();
			this.comboBoxPatchControllerSelect = new System.Windows.Forms.ComboBox();
			this.numericUpDownPatchOutputSelect = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonAddPatch = new System.Windows.Forms.Button();
			this.groupBoxAddPatch = new System.Windows.Forms.GroupBox();
			this.buttonDeleteNode = new System.Windows.Forms.Button();
			this.buttonBulkRename = new System.Windows.Forms.Button();
			this.buttonCreateGroup = new System.Windows.Forms.Button();
			this.buttonAddNode = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBoxSelectedNode.SuspendLayout();
			this.groupBoxPatches.SuspendLayout();
			this.groupBoxProperties.SuspendLayout();
			this.groupBoxOperations.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPatchOutputSelect)).BeginInit();
			this.groupBoxAddPatch.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(509, 693);
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
			this.groupBoxSelectedNode.Location = new System.Drawing.Point(318, 145);
			this.groupBoxSelectedNode.Name = "groupBoxSelectedNode";
			this.groupBoxSelectedNode.Size = new System.Drawing.Size(281, 536);
			this.groupBoxSelectedNode.TabIndex = 11;
			this.groupBoxSelectedNode.TabStop = false;
			this.groupBoxSelectedNode.Text = "Selected Node";
			// 
			// groupBoxPatches
			// 
			this.groupBoxPatches.Controls.Add(this.checkBoxDisableOutputs);
			this.groupBoxPatches.Controls.Add(this.buttonRemovePatch);
			this.groupBoxPatches.Controls.Add(this.listViewPatches);
			this.groupBoxPatches.Location = new System.Drawing.Point(6, 102);
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
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12});
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
			this.groupBoxProperties.Location = new System.Drawing.Point(6, 387);
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
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 200;
			// 
			// labelParents
			// 
			this.labelParents.AutoEllipsis = true;
			this.labelParents.Location = new System.Drawing.Point(7, 63);
			this.labelParents.Name = "labelParents";
			this.labelParents.Size = new System.Drawing.Size(268, 28);
			this.labelParents.TabIndex = 24;
			this.labelParents.Text = "This node is in <x> groups:\r\nasdfasdfasdfasdfasdfasdfasdfasdfqwerqwerqwerqwer\r\n23" +
				"4123412341234123412341234";
			// 
			// buttonRenameItem
			// 
			this.buttonRenameItem.Location = new System.Drawing.Point(201, 26);
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
			this.label2.Location = new System.Drawing.Point(6, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(43, 29);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(152, 20);
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
			// multiSelectTreeviewChannelsGroups
			// 
			this.multiSelectTreeviewChannelsGroups.AllowDrop = true;
			this.multiSelectTreeviewChannelsGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
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
			this.multiSelectTreeviewChannelsGroups.Size = new System.Drawing.Size(300, 669);
			this.multiSelectTreeviewChannelsGroups.TabIndex = 12;
			this.multiSelectTreeviewChannelsGroups.UsingCustomDragCursor = false;
			this.multiSelectTreeviewChannelsGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.multiSelectTreeviewChannelsGroups_AfterSelect);
			// 
			// groupBoxOperations
			// 
			this.groupBoxOperations.Controls.Add(this.buttonDeleteNode);
			this.groupBoxOperations.Controls.Add(this.buttonBulkRename);
			this.groupBoxOperations.Controls.Add(this.buttonCreateGroup);
			this.groupBoxOperations.Controls.Add(this.buttonAddNode);
			this.groupBoxOperations.Location = new System.Drawing.Point(318, 12);
			this.groupBoxOperations.Name = "groupBoxOperations";
			this.groupBoxOperations.Size = new System.Drawing.Size(281, 127);
			this.groupBoxOperations.TabIndex = 23;
			this.groupBoxOperations.TabStop = false;
			this.groupBoxOperations.Text = "Operations";
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
			// numericUpDownPatchOutputSelect
			// 
			this.numericUpDownPatchOutputSelect.Location = new System.Drawing.Point(66, 52);
			this.numericUpDownPatchOutputSelect.Name = "numericUpDownPatchOutputSelect";
			this.numericUpDownPatchOutputSelect.Size = new System.Drawing.Size(55, 20);
			this.numericUpDownPatchOutputSelect.TabIndex = 23;
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
			// groupBoxAddPatch
			// 
			this.groupBoxAddPatch.Controls.Add(this.buttonAddPatch);
			this.groupBoxAddPatch.Controls.Add(this.label3);
			this.groupBoxAddPatch.Controls.Add(this.label1);
			this.groupBoxAddPatch.Controls.Add(this.numericUpDownPatchOutputSelect);
			this.groupBoxAddPatch.Controls.Add(this.comboBoxPatchControllerSelect);
			this.groupBoxAddPatch.Location = new System.Drawing.Point(6, 284);
			this.groupBoxAddPatch.Name = "groupBoxAddPatch";
			this.groupBoxAddPatch.Size = new System.Drawing.Size(269, 87);
			this.groupBoxAddPatch.TabIndex = 32;
			this.groupBoxAddPatch.TabStop = false;
			this.groupBoxAddPatch.Text = "Add Patch";
			// 
			// buttonDeleteNode
			// 
			this.buttonDeleteNode.Location = new System.Drawing.Point(146, 24);
			this.buttonDeleteNode.Name = "buttonDeleteNode";
			this.buttonDeleteNode.Size = new System.Drawing.Size(120, 25);
			this.buttonDeleteNode.TabIndex = 26;
			this.buttonDeleteNode.Text = "Delete Node";
			this.buttonDeleteNode.UseVisualStyleBackColor = true;
			// 
			// buttonBulkRename
			// 
			this.buttonBulkRename.Location = new System.Drawing.Point(16, 86);
			this.buttonBulkRename.Name = "buttonBulkRename";
			this.buttonBulkRename.Size = new System.Drawing.Size(250, 25);
			this.buttonBulkRename.TabIndex = 25;
			this.buttonBulkRename.Text = "Bulk Rename Selected Nodes";
			this.buttonBulkRename.UseVisualStyleBackColor = true;
			// 
			// buttonCreateGroup
			// 
			this.buttonCreateGroup.Location = new System.Drawing.Point(16, 55);
			this.buttonCreateGroup.Name = "buttonCreateGroup";
			this.buttonCreateGroup.Size = new System.Drawing.Size(250, 25);
			this.buttonCreateGroup.TabIndex = 24;
			this.buttonCreateGroup.Text = "Create Group from Selected Nodes";
			this.buttonCreateGroup.UseVisualStyleBackColor = true;
			// 
			// buttonAddNode
			// 
			this.buttonAddNode.Location = new System.Drawing.Point(16, 24);
			this.buttonAddNode.Name = "buttonAddNode";
			this.buttonAddNode.Size = new System.Drawing.Size(120, 25);
			this.buttonAddNode.TabIndex = 23;
			this.buttonAddNode.Text = "Add Node";
			this.buttonAddNode.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 688);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(244, 13);
			this.label4.TabIndex = 24;
			this.label4.Text = "Click and drag to move nodes in the configuration.";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 705);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(299, 13);
			this.label5.TabIndex = 25;
			this.label5.Text = "Hold down CTRL while dragging to copy nodes to destination.";
			// 
			// ConfigChannels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(611, 730);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBoxOperations);
			this.Controls.Add(this.multiSelectTreeviewChannelsGroups);
			this.Controls.Add(this.groupBoxSelectedNode);
			this.Controls.Add(this.buttonOk);
			this.Name = "ConfigChannels";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Channel & Group Configuration";
			this.Load += new System.EventHandler(this.ConfigChannels_Load);
			this.groupBoxSelectedNode.ResumeLayout(false);
			this.groupBoxSelectedNode.PerformLayout();
			this.groupBoxPatches.ResumeLayout(false);
			this.groupBoxPatches.PerformLayout();
			this.groupBoxProperties.ResumeLayout(false);
			this.groupBoxOperations.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPatchOutputSelect)).EndInit();
			this.groupBoxAddPatch.ResumeLayout(false);
			this.groupBoxAddPatch.PerformLayout();
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
	}
}