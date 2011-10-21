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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Channels", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Groups", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Patches", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("test-channel");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("test-group");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("test-patch");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigChannels));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonAddChannel = new System.Windows.Forms.Button();
			this.groupBoxSelected = new System.Windows.Forms.GroupBox();
			this.radioButtonPatches = new System.Windows.Forms.RadioButton();
			this.labelParents = new System.Windows.Forms.Label();
			this.radioButtonGroups = new System.Windows.Forms.RadioButton();
			this.radioButtonChannels = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonRenameItem = new System.Windows.Forms.Button();
			this.listViewAddToNode = new System.Windows.Forms.ListView();
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonAddToGroup = new System.Windows.Forms.Button();
			this.buttonRemoveFromGroup = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listViewNodeContents = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.buttonCreateGroup = new System.Windows.Forms.Button();
			this.groupBoxProperties = new System.Windows.Forms.GroupBox();
			this.buttonRemoveProperty = new System.Windows.Forms.Button();
			this.buttonConfigureProperty = new System.Windows.Forms.Button();
			this.buttonAddProperty = new System.Windows.Forms.Button();
			this.listViewProperties = new System.Windows.Forms.ListView();
			this.buttonBulkRename = new System.Windows.Forms.Button();
			this.buttonDeleteItem = new System.Windows.Forms.Button();
			this.multiSelectTreeviewChannelsGroups = new CommonElements.MultiSelectTreeview();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxSelected.SuspendLayout();
			this.groupBoxProperties.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(682, 720);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 30);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonAddChannel
			// 
			this.buttonAddChannel.Location = new System.Drawing.Point(24, 641);
			this.buttonAddChannel.Name = "buttonAddChannel";
			this.buttonAddChannel.Size = new System.Drawing.Size(100, 30);
			this.buttonAddChannel.TabIndex = 9;
			this.buttonAddChannel.Text = "Add";
			this.buttonAddChannel.UseVisualStyleBackColor = true;
			this.buttonAddChannel.Click += new System.EventHandler(this.buttonAddChannel_Click);
			// 
			// groupBoxSelected
			// 
			this.groupBoxSelected.Controls.Add(this.radioButtonPatches);
			this.groupBoxSelected.Controls.Add(this.labelParents);
			this.groupBoxSelected.Controls.Add(this.radioButtonGroups);
			this.groupBoxSelected.Controls.Add(this.radioButtonChannels);
			this.groupBoxSelected.Controls.Add(this.label4);
			this.groupBoxSelected.Controls.Add(this.buttonRenameItem);
			this.groupBoxSelected.Controls.Add(this.listViewAddToNode);
			this.groupBoxSelected.Controls.Add(this.buttonAddToGroup);
			this.groupBoxSelected.Controls.Add(this.buttonRemoveFromGroup);
			this.groupBoxSelected.Controls.Add(this.label3);
			this.groupBoxSelected.Controls.Add(this.listViewNodeContents);
			this.groupBoxSelected.Controls.Add(this.label2);
			this.groupBoxSelected.Controls.Add(this.textBoxName);
			this.groupBoxSelected.Location = new System.Drawing.Point(250, 36);
			this.groupBoxSelected.Name = "groupBoxSelected";
			this.groupBoxSelected.Size = new System.Drawing.Size(522, 406);
			this.groupBoxSelected.TabIndex = 11;
			this.groupBoxSelected.TabStop = false;
			this.groupBoxSelected.Text = "Selected Item:";
			// 
			// radioButtonPatches
			// 
			this.radioButtonPatches.AutoSize = true;
			this.radioButtonPatches.Location = new System.Drawing.Point(444, 110);
			this.radioButtonPatches.Name = "radioButtonPatches";
			this.radioButtonPatches.Size = new System.Drawing.Size(64, 17);
			this.radioButtonPatches.TabIndex = 25;
			this.radioButtonPatches.Text = "Patches";
			this.radioButtonPatches.UseVisualStyleBackColor = true;
			this.radioButtonPatches.CheckedChanged += new System.EventHandler(this.radioButtonPatches_CheckedChanged);
			// 
			// labelParents
			// 
			this.labelParents.AutoEllipsis = true;
			this.labelParents.Location = new System.Drawing.Point(16, 63);
			this.labelParents.Name = "labelParents";
			this.labelParents.Size = new System.Drawing.Size(490, 16);
			this.labelParents.TabIndex = 24;
			this.labelParents.Text = "This item is in <x> groups:";
			// 
			// radioButtonGroups
			// 
			this.radioButtonGroups.AutoSize = true;
			this.radioButtonGroups.Location = new System.Drawing.Point(380, 110);
			this.radioButtonGroups.Name = "radioButtonGroups";
			this.radioButtonGroups.Size = new System.Drawing.Size(59, 17);
			this.radioButtonGroups.TabIndex = 23;
			this.radioButtonGroups.Text = "Groups";
			this.radioButtonGroups.UseVisualStyleBackColor = true;
			this.radioButtonGroups.CheckedChanged += new System.EventHandler(this.radioButtonGroups_CheckedChanged);
			// 
			// radioButtonChannels
			// 
			this.radioButtonChannels.AutoSize = true;
			this.radioButtonChannels.Checked = true;
			this.radioButtonChannels.Location = new System.Drawing.Point(306, 110);
			this.radioButtonChannels.Name = "radioButtonChannels";
			this.radioButtonChannels.Size = new System.Drawing.Size(69, 17);
			this.radioButtonChannels.TabIndex = 22;
			this.radioButtonChannels.TabStop = true;
			this.radioButtonChannels.Text = "Channels";
			this.radioButtonChannels.UseVisualStyleBackColor = true;
			this.radioButtonChannels.CheckedChanged += new System.EventHandler(this.radioButtonChannels_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(373, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 20;
			this.label4.Text = "Add to item:";
			// 
			// buttonRenameItem
			// 
			this.buttonRenameItem.Location = new System.Drawing.Point(368, 23);
			this.buttonRenameItem.Name = "buttonRenameItem";
			this.buttonRenameItem.Size = new System.Drawing.Size(100, 30);
			this.buttonRenameItem.TabIndex = 19;
			this.buttonRenameItem.Text = "Rename";
			this.buttonRenameItem.UseVisualStyleBackColor = true;
			this.buttonRenameItem.Click += new System.EventHandler(this.buttonRenameItem_Click);
			// 
			// listViewAddToNode
			// 
			this.listViewAddToNode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
			this.listViewAddToNode.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewAddToNode.HideSelection = false;
			this.listViewAddToNode.Location = new System.Drawing.Point(306, 133);
			this.listViewAddToNode.Name = "listViewAddToNode";
			this.listViewAddToNode.Size = new System.Drawing.Size(200, 253);
			this.listViewAddToNode.TabIndex = 18;
			this.listViewAddToNode.UseCompatibleStateImageBehavior = false;
			this.listViewAddToNode.View = System.Windows.Forms.View.Details;
			this.listViewAddToNode.SelectedIndexChanged += new System.EventHandler(this.listViewAddToNode_SelectedIndexChanged);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 178;
			// 
			// buttonAddToGroup
			// 
			this.buttonAddToGroup.Location = new System.Drawing.Point(231, 170);
			this.buttonAddToGroup.Name = "buttonAddToGroup";
			this.buttonAddToGroup.Size = new System.Drawing.Size(60, 60);
			this.buttonAddToGroup.TabIndex = 17;
			this.buttonAddToGroup.Text = "<----";
			this.buttonAddToGroup.UseVisualStyleBackColor = true;
			this.buttonAddToGroup.Click += new System.EventHandler(this.buttonAddToGroup_Click);
			// 
			// buttonRemoveFromGroup
			// 
			this.buttonRemoveFromGroup.Location = new System.Drawing.Point(231, 266);
			this.buttonRemoveFromGroup.Name = "buttonRemoveFromGroup";
			this.buttonRemoveFromGroup.Size = new System.Drawing.Size(60, 60);
			this.buttonRemoveFromGroup.TabIndex = 16;
			this.buttonRemoveFromGroup.Text = "---->";
			this.buttonRemoveFromGroup.UseVisualStyleBackColor = true;
			this.buttonRemoveFromGroup.Click += new System.EventHandler(this.buttonRemoveFromGroup_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(67, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 13);
			this.label3.TabIndex = 15;
			this.label3.Text = "This item contains:";
			// 
			// listViewNodeContents
			// 
			this.listViewNodeContents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			listViewGroup1.Header = "Channels";
			listViewGroup1.Name = "listViewGroup1";
			listViewGroup2.Header = "Groups";
			listViewGroup2.Name = "listViewGroup2";
			listViewGroup3.Header = "Patches";
			listViewGroup3.Name = "listViewGroup3";
			this.listViewNodeContents.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.listViewNodeContents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewNodeContents.HideSelection = false;
			listViewItem1.Group = listViewGroup1;
			listViewItem2.Group = listViewGroup2;
			listViewItem3.Group = listViewGroup3;
			this.listViewNodeContents.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
			this.listViewNodeContents.Location = new System.Drawing.Point(16, 133);
			this.listViewNodeContents.Name = "listViewNodeContents";
			this.listViewNodeContents.Size = new System.Drawing.Size(200, 253);
			this.listViewNodeContents.TabIndex = 14;
			this.listViewNodeContents.UseCompatibleStateImageBehavior = false;
			this.listViewNodeContents.View = System.Windows.Forms.View.Details;
			this.listViewNodeContents.SelectedIndexChanged += new System.EventHandler(this.listViewNodeContents_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "";
			this.columnHeader1.Width = 178;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(19, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(63, 29);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(292, 20);
			this.textBoxName.TabIndex = 11;
			// 
			// buttonCreateGroup
			// 
			this.buttonCreateGroup.Location = new System.Drawing.Point(24, 677);
			this.buttonCreateGroup.Name = "buttonCreateGroup";
			this.buttonCreateGroup.Size = new System.Drawing.Size(206, 30);
			this.buttonCreateGroup.TabIndex = 14;
			this.buttonCreateGroup.Text = "Create Group from Selected";
			this.buttonCreateGroup.UseVisualStyleBackColor = true;
			this.buttonCreateGroup.Click += new System.EventHandler(this.buttonCreateGroup_Click);
			// 
			// groupBoxProperties
			// 
			this.groupBoxProperties.Controls.Add(this.buttonRemoveProperty);
			this.groupBoxProperties.Controls.Add(this.buttonConfigureProperty);
			this.groupBoxProperties.Controls.Add(this.buttonAddProperty);
			this.groupBoxProperties.Controls.Add(this.listViewProperties);
			this.groupBoxProperties.Location = new System.Drawing.Point(250, 448);
			this.groupBoxProperties.Name = "groupBoxProperties";
			this.groupBoxProperties.Size = new System.Drawing.Size(410, 302);
			this.groupBoxProperties.TabIndex = 15;
			this.groupBoxProperties.TabStop = false;
			this.groupBoxProperties.Text = "Item Properties:";
			// 
			// buttonRemoveProperty
			// 
			this.buttonRemoveProperty.Location = new System.Drawing.Point(289, 100);
			this.buttonRemoveProperty.Name = "buttonRemoveProperty";
			this.buttonRemoveProperty.Size = new System.Drawing.Size(100, 30);
			this.buttonRemoveProperty.TabIndex = 19;
			this.buttonRemoveProperty.Text = "Remove";
			this.buttonRemoveProperty.UseVisualStyleBackColor = true;
			this.buttonRemoveProperty.Click += new System.EventHandler(this.buttonRemoveProperty_Click);
			// 
			// buttonConfigureProperty
			// 
			this.buttonConfigureProperty.Location = new System.Drawing.Point(289, 62);
			this.buttonConfigureProperty.Name = "buttonConfigureProperty";
			this.buttonConfigureProperty.Size = new System.Drawing.Size(100, 30);
			this.buttonConfigureProperty.TabIndex = 18;
			this.buttonConfigureProperty.Text = "Configure";
			this.buttonConfigureProperty.UseVisualStyleBackColor = true;
			this.buttonConfigureProperty.Click += new System.EventHandler(this.buttonConfigureProperty_Click);
			// 
			// buttonAddProperty
			// 
			this.buttonAddProperty.Location = new System.Drawing.Point(289, 24);
			this.buttonAddProperty.Name = "buttonAddProperty";
			this.buttonAddProperty.Size = new System.Drawing.Size(100, 30);
			this.buttonAddProperty.TabIndex = 17;
			this.buttonAddProperty.Text = "Add";
			this.buttonAddProperty.UseVisualStyleBackColor = true;
			this.buttonAddProperty.Click += new System.EventHandler(this.buttonAddProperty_Click);
			// 
			// listViewProperties
			// 
			this.listViewProperties.Location = new System.Drawing.Point(16, 24);
			this.listViewProperties.Name = "listViewProperties";
			this.listViewProperties.Size = new System.Drawing.Size(257, 261);
			this.listViewProperties.TabIndex = 15;
			this.listViewProperties.UseCompatibleStateImageBehavior = false;
			this.listViewProperties.SelectedIndexChanged += new System.EventHandler(this.listViewProperties_SelectedIndexChanged);
			this.listViewProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewProperties_MouseDoubleClick);
			// 
			// buttonBulkRename
			// 
			this.buttonBulkRename.Location = new System.Drawing.Point(24, 713);
			this.buttonBulkRename.Name = "buttonBulkRename";
			this.buttonBulkRename.Size = new System.Drawing.Size(206, 30);
			this.buttonBulkRename.TabIndex = 16;
			this.buttonBulkRename.Text = "Bulk Rename Selected";
			this.buttonBulkRename.UseVisualStyleBackColor = true;
			this.buttonBulkRename.Click += new System.EventHandler(this.buttonBulkRename_Click);
			// 
			// buttonDeleteItem
			// 
			this.buttonDeleteItem.Location = new System.Drawing.Point(130, 641);
			this.buttonDeleteItem.Name = "buttonDeleteItem";
			this.buttonDeleteItem.Size = new System.Drawing.Size(100, 30);
			this.buttonDeleteItem.TabIndex = 22;
			this.buttonDeleteItem.Text = "Delete";
			this.buttonDeleteItem.UseVisualStyleBackColor = true;
			this.buttonDeleteItem.Click += new System.EventHandler(this.buttonDeleteItem_Click);
			// 
			// multiSelectTreeviewChannelsGroups
			// 
			this.multiSelectTreeviewChannelsGroups.AllowDrop = true;
			this.multiSelectTreeviewChannelsGroups.CustomDragCursor = null;
			this.multiSelectTreeviewChannelsGroups.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.multiSelectTreeviewChannelsGroups.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.multiSelectTreeviewChannelsGroups.DragMode = System.Windows.Forms.DragDropEffects.Move;
			this.multiSelectTreeviewChannelsGroups.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.multiSelectTreeviewChannelsGroups.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.multiSelectTreeviewChannelsGroups.HideSelection = false;
			this.multiSelectTreeviewChannelsGroups.Location = new System.Drawing.Point(12, 36);
			this.multiSelectTreeviewChannelsGroups.Name = "multiSelectTreeviewChannelsGroups";
			this.multiSelectTreeviewChannelsGroups.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("multiSelectTreeviewChannelsGroups.SelectedNodes")));
			this.multiSelectTreeviewChannelsGroups.Size = new System.Drawing.Size(230, 599);
			this.multiSelectTreeviewChannelsGroups.TabIndex = 12;
			this.multiSelectTreeviewChannelsGroups.UsingCustomDragCursor = false;
			this.multiSelectTreeviewChannelsGroups.MouseDown += new System.Windows.Forms.MouseEventHandler(this.multiSelectTreeviewChannelsGroups_MouseDown);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Channels && Groups:";
			// 
			// ConfigChannels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 762);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonDeleteItem);
			this.Controls.Add(this.buttonBulkRename);
			this.Controls.Add(this.groupBoxProperties);
			this.Controls.Add(this.buttonCreateGroup);
			this.Controls.Add(this.multiSelectTreeviewChannelsGroups);
			this.Controls.Add(this.groupBoxSelected);
			this.Controls.Add(this.buttonAddChannel);
			this.Controls.Add(this.buttonOk);
			this.Name = "ConfigChannels";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Channel & Group Configuration";
			this.Load += new System.EventHandler(this.ConfigChannels_Load);
			this.groupBoxSelected.ResumeLayout(false);
			this.groupBoxSelected.PerformLayout();
			this.groupBoxProperties.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonAddChannel;
		private System.Windows.Forms.GroupBox groupBoxSelected;
		private CommonElements.MultiSelectTreeview multiSelectTreeviewChannelsGroups;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonRenameItem;
		private System.Windows.Forms.ListView listViewAddToNode;
		private System.Windows.Forms.Button buttonAddToGroup;
		private System.Windows.Forms.Button buttonRemoveFromGroup;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListView listViewNodeContents;
		private System.Windows.Forms.Button buttonCreateGroup;
		private System.Windows.Forms.GroupBox groupBoxProperties;
		private System.Windows.Forms.Button buttonRemoveProperty;
		private System.Windows.Forms.Button buttonConfigureProperty;
		private System.Windows.Forms.Button buttonAddProperty;
		private System.Windows.Forms.ListView listViewProperties;
		private System.Windows.Forms.Button buttonBulkRename;
		private System.Windows.Forms.RadioButton radioButtonGroups;
		private System.Windows.Forms.RadioButton radioButtonChannels;
		private System.Windows.Forms.Button buttonDeleteItem;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelParents;
		private System.Windows.Forms.RadioButton radioButtonPatches;
	}
}