namespace VixenModules.SequenceType.Vixen2x
{
	partial class Vixen2xSequenceImporterChannelMapper
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
			if (disposing)
{
    if (components != null)
    {
        components.Dispose();
    }
    if (treeview != null)
    {
        treeview.Dispose();
        treeview = null;
    }
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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("group1", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("group2", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("group3", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("qwerqewr");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "asdfasdf"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "sub1", System.Drawing.SystemColors.Info, System.Drawing.SystemColors.HotTrack, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "sub2")}, -1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("jjjjjjjjjjjjjj");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("hhhhhhhhhhhhhh");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vixen2xSequenceImporterChannelMapper));
			this.listViewMapping = new System.Windows.Forms.ListView();
			this.v2Channel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.v2ChannelOutput = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.v2ChannelName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.v2ChannelColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.destinationElement = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.destinationColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.multiSelectTreeview1 = new Common.Controls.MultiSelectTreeview();
			this.destinationColorButton = new System.Windows.Forms.Button();
			this.mappingNameLabel = new System.Windows.Forms.Label();
			this.mappingNameTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listViewMapping
			// 
			this.listViewMapping.AllowDrop = true;
			this.listViewMapping.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listViewMapping.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.v2Channel,
            this.v2ChannelOutput,
            this.v2ChannelName,
            this.v2ChannelColor,
            this.destinationElement,
            this.destinationColor});
			this.listViewMapping.FullRowSelect = true;
			this.listViewMapping.GridLines = true;
			listViewGroup1.Header = "group1";
			listViewGroup1.Name = "listViewGroup1";
			listViewGroup2.Header = "group2";
			listViewGroup2.Name = "listViewGroup2";
			listViewGroup3.Header = "group3";
			listViewGroup3.Name = "listViewGroup3";
			this.listViewMapping.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.listViewMapping.HideSelection = false;
			this.listViewMapping.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
			this.listViewMapping.Location = new System.Drawing.Point(12, 12);
			this.listViewMapping.Name = "listViewMapping";
			this.listViewMapping.ShowGroups = false;
			this.listViewMapping.Size = new System.Drawing.Size(628, 462);
			this.listViewMapping.TabIndex = 0;
			this.listViewMapping.UseCompatibleStateImageBehavior = false;
			this.listViewMapping.View = System.Windows.Forms.View.Details;
			this.listViewMapping.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewMapping_DragDrop);
			this.listViewMapping.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewMapping_DragEnter);
			// 
			// v2Channel
			// 
			this.v2Channel.Text = "V2 Channel";
			this.v2Channel.Width = 68;
			// 
			// v2ChannelOutput
			// 
			this.v2ChannelOutput.Text = "V2 Output";
			this.v2ChannelOutput.Width = 68;
			// 
			// v2ChannelName
			// 
			this.v2ChannelName.Text = "V2 Name";
			this.v2ChannelName.Width = 130;
			// 
			// v2ChannelColor
			// 
			this.v2ChannelColor.Text = "V2 Color";
			this.v2ChannelColor.Width = 70;
			// 
			// destinationElement
			// 
			this.destinationElement.Text = "Destination Element";
			this.destinationElement.Width = 150;
			// 
			// destinationColor
			// 
			this.destinationColor.Text = "Import Color";
			this.destinationColor.Width = 120;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(704, 489);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(800, 489);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(90, 25);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// multiSelectTreeview1
			// 
			this.multiSelectTreeview1.AllowDrop = true;
			this.multiSelectTreeview1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.multiSelectTreeview1.CustomDragCursor = null;
			this.multiSelectTreeview1.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
			this.multiSelectTreeview1.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.multiSelectTreeview1.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.multiSelectTreeview1.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.multiSelectTreeview1.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.multiSelectTreeview1.Location = new System.Drawing.Point(646, 12);
			this.multiSelectTreeview1.Name = "multiSelectTreeview1";
			this.multiSelectTreeview1.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("multiSelectTreeview1.SelectedNodes")));
			this.multiSelectTreeview1.Size = new System.Drawing.Size(244, 462);
			this.multiSelectTreeview1.TabIndex = 5;
			this.multiSelectTreeview1.UsingCustomDragCursor = false;
			this.multiSelectTreeview1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.multiSelectTreeview1_ItemDrag);
			this.multiSelectTreeview1.DragEnter += new System.Windows.Forms.DragEventHandler(this.multiSelectTreeview1_DragEnter);
			// 
			// destinationColorButton
			// 
			this.destinationColorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.destinationColorButton.Location = new System.Drawing.Point(550, 489);
			this.destinationColorButton.Name = "destinationColorButton";
			this.destinationColorButton.Size = new System.Drawing.Size(90, 25);
			this.destinationColorButton.TabIndex = 6;
			this.destinationColorButton.Text = "Set Import Color";
			this.destinationColorButton.UseVisualStyleBackColor = true;
			this.destinationColorButton.Click += new System.EventHandler(this.destinationColorButton_Click);
			// 
			// mappingNameLabel
			// 
			this.mappingNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mappingNameLabel.AutoSize = true;
			this.mappingNameLabel.Location = new System.Drawing.Point(12, 495);
			this.mappingNameLabel.Name = "mappingNameLabel";
			this.mappingNameLabel.Size = new System.Drawing.Size(82, 13);
			this.mappingNameLabel.TabIndex = 7;
			this.mappingNameLabel.Text = "Mapping Name:";
			// 
			// mappingNameTextBox
			// 
			this.mappingNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mappingNameTextBox.Location = new System.Drawing.Point(100, 492);
			this.mappingNameTextBox.Name = "mappingNameTextBox";
			this.mappingNameTextBox.Size = new System.Drawing.Size(298, 20);
			this.mappingNameTextBox.TabIndex = 8;
			// 
			// Vixen2xSequenceImporterChannelMapper
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(896, 526);
			this.Controls.Add(this.mappingNameTextBox);
			this.Controls.Add(this.mappingNameLabel);
			this.Controls.Add(this.destinationColorButton);
			this.Controls.Add(this.multiSelectTreeview1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.listViewMapping);
			this.DoubleBuffered = true;
			this.Name = "Vixen2xSequenceImporterChannelMapper";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Vixen 2.x Channel Mapping";
			this.Load += new System.EventHandler(this.Vixen2xSequenceImporterChannelMapper_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewMapping;
		private System.Windows.Forms.ColumnHeader v2ChannelName;
        private System.Windows.Forms.ColumnHeader v2ChannelColor;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColumnHeader v2Channel;
        private System.Windows.Forms.ColumnHeader v2ChannelOutput;
		private System.Windows.Forms.ColumnHeader destinationElement;
        private System.Windows.Forms.Button buttonOK;
		private Common.Controls.MultiSelectTreeview multiSelectTreeview1;
        private System.Windows.Forms.ColumnHeader destinationColor;
        private System.Windows.Forms.Button destinationColorButton;
		private System.Windows.Forms.Label mappingNameLabel;
		private System.Windows.Forms.TextBox mappingNameTextBox;
	}
}