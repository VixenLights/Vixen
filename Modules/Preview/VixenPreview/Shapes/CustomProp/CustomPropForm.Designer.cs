namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	partial class CustomPropForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomPropForm));
			this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Rename = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_AddMultiple = new System.Windows.Forms.ToolStripMenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.numGridHeight = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.toolStripMenuItem_Add = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuChannels = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.changeChannelColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnUpdateChannelCount = new System.Windows.Forms.Button();
			this.numGridWidth = new System.Windows.Forms.NumericUpDown();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.chkMaintainAspect = new System.Windows.Forms.CheckBox();
			this.btnLoadBackgroundImage = new System.Windows.Forms.Button();
			this.txtBackgroundImage = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.trkImageOpacity = new System.Windows.Forms.TrackBar();
			this.treeViewChannels = new Common.Controls.MultiSelectTreeview();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gridPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.numGridHeight)).BeginInit();
			this.contextMenuChannels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numGridWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkImageOpacity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripMenuItem_Remove
			// 
			this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
			this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(195, 22);
			this.toolStripMenuItem_Remove.Text = "Remove Channel";
			this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
			// 
			// toolStripMenuItem_Rename
			// 
			this.toolStripMenuItem_Rename.Name = "toolStripMenuItem_Rename";
			this.toolStripMenuItem_Rename.Size = new System.Drawing.Size(195, 22);
			this.toolStripMenuItem_Rename.Text = "Rename Channel";
			this.toolStripMenuItem_Rename.Click += new System.EventHandler(this.toolStripMenuItem_Rename_Click);
			// 
			// toolStripMenuItem_AddMultiple
			// 
			this.toolStripMenuItem_AddMultiple.Name = "toolStripMenuItem_AddMultiple";
			this.toolStripMenuItem_AddMultiple.Size = new System.Drawing.Size(195, 22);
			this.toolStripMenuItem_AddMultiple.Text = "Add Multiple Channels";
			this.toolStripMenuItem_AddMultiple.Click += new System.EventHandler(this.toolStripMenuItem_AddMultiple_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Prop Name";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(123, 160);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(111, 27);
			this.btnSave.TabIndex = 9;
			this.btnSave.Text = "Save and Exit";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// numGridHeight
			// 
			this.numGridHeight.Location = new System.Drawing.Point(83, 35);
			this.numGridHeight.Name = "numGridHeight";
			this.numGridHeight.Size = new System.Drawing.Size(66, 20);
			this.numGridHeight.TabIndex = 7;
			this.numGridHeight.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numGridHeight.ValueChanged += new System.EventHandler(this.numGridHeight_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label4.Location = new System.Drawing.Point(0, 190);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Channels";
			this.label4.Click += new System.EventHandler(this.label4_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Grid Height";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// toolStripMenuItem_Add
			// 
			this.toolStripMenuItem_Add.Name = "toolStripMenuItem_Add";
			this.toolStripMenuItem_Add.Size = new System.Drawing.Size(195, 22);
			this.toolStripMenuItem_Add.Text = "Add Channel";
			this.toolStripMenuItem_Add.Click += new System.EventHandler(this.toolStripMenuItem_Add_Click);
			// 
			// contextMenuChannels
			// 
			this.contextMenuChannels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Add,
            this.toolStripMenuItem_AddMultiple,
            this.toolStripSeparator2,
            this.changeChannelColorToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem_Remove,
            this.toolStripMenuItem_Rename});
			this.contextMenuChannels.Name = "contextMenuChannels";
			this.contextMenuChannels.Size = new System.Drawing.Size(196, 126);
			this.contextMenuChannels.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuChannels_Opening);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
			this.toolStripSeparator2.Click += new System.EventHandler(this.toolStripSeparator2_Click);
			// 
			// changeChannelColorToolStripMenuItem
			// 
			this.changeChannelColorToolStripMenuItem.Name = "changeChannelColorToolStripMenuItem";
			this.changeChannelColorToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.changeChannelColorToolStripMenuItem.Text = "Change Channel Color";
			this.changeChannelColorToolStripMenuItem.Click += new System.EventHandler(this.changeChannelColorToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			this.toolStripSeparator1.Click += new System.EventHandler(this.toolStripSeparator1_Click);
			// 
			// btnUpdateChannelCount
			// 
			this.btnUpdateChannelCount.Location = new System.Drawing.Point(20, 160);
			this.btnUpdateChannelCount.Name = "btnUpdateChannelCount";
			this.btnUpdateChannelCount.Size = new System.Drawing.Size(97, 27);
			this.btnUpdateChannelCount.TabIndex = 2;
			this.btnUpdateChannelCount.Text = "Resize";
			this.btnUpdateChannelCount.UseVisualStyleBackColor = true;
			this.btnUpdateChannelCount.Click += new System.EventHandler(this.btnUpdateChannelCount_Click);
			// 
			// numGridWidth
			// 
			this.numGridWidth.Location = new System.Drawing.Point(83, 61);
			this.numGridWidth.Name = "numGridWidth";
			this.numGridWidth.Size = new System.Drawing.Size(66, 20);
			this.numGridWidth.TabIndex = 5;
			this.numGridWidth.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numGridWidth.ValueChanged += new System.EventHandler(this.numGridWidth_ValueChanged);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.chkMaintainAspect);
			this.splitContainer2.Panel1.Controls.Add(this.btnLoadBackgroundImage);
			this.splitContainer2.Panel1.Controls.Add(this.txtBackgroundImage);
			this.splitContainer2.Panel1.Controls.Add(this.textBox1);
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			this.splitContainer2.Panel1.Controls.Add(this.btnSave);
			this.splitContainer2.Panel1.Controls.Add(this.numGridHeight);
			this.splitContainer2.Panel1.Controls.Add(this.label4);
			this.splitContainer2.Panel1.Controls.Add(this.label3);
			this.splitContainer2.Panel1.Controls.Add(this.btnUpdateChannelCount);
			this.splitContainer2.Panel1.Controls.Add(this.numGridWidth);
			this.splitContainer2.Panel1.Controls.Add(this.label2);
			this.splitContainer2.Panel1.Controls.Add(this.trkImageOpacity);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.treeViewChannels);
			this.splitContainer2.Size = new System.Drawing.Size(246, 500);
			this.splitContainer2.SplitterDistance = 203;
			this.splitContainer2.TabIndex = 0;
			this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
			// 
			// chkMaintainAspect
			// 
			this.chkMaintainAspect.AutoSize = true;
			this.chkMaintainAspect.Checked = true;
			this.chkMaintainAspect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkMaintainAspect.Location = new System.Drawing.Point(67, 126);
			this.chkMaintainAspect.Name = "chkMaintainAspect";
			this.chkMaintainAspect.Size = new System.Drawing.Size(121, 17);
			this.chkMaintainAspect.TabIndex = 15;
			this.chkMaintainAspect.Text = "Retain Aspect Ratio";
			this.chkMaintainAspect.UseVisualStyleBackColor = true;
			// 
			// btnLoadBackgroundImage
			// 
			this.btnLoadBackgroundImage.Location = new System.Drawing.Point(208, 87);
			this.btnLoadBackgroundImage.Name = "btnLoadBackgroundImage";
			this.btnLoadBackgroundImage.Size = new System.Drawing.Size(26, 20);
			this.btnLoadBackgroundImage.TabIndex = 13;
			this.btnLoadBackgroundImage.Text = "...";
			this.btnLoadBackgroundImage.UseVisualStyleBackColor = true;
			this.btnLoadBackgroundImage.Click += new System.EventHandler(this.btnLoadBackgroundImage_Click);
			// 
			// txtBackgroundImage
			// 
			this.txtBackgroundImage.Location = new System.Drawing.Point(20, 88);
			this.txtBackgroundImage.Name = "txtBackgroundImage";
			this.txtBackgroundImage.ReadOnly = true;
			this.txtBackgroundImage.Size = new System.Drawing.Size(190, 20);
			this.txtBackgroundImage.TabIndex = 12;
			this.txtBackgroundImage.TextChanged += new System.EventHandler(this.txtBackgroundImage_TextChanged);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(83, 9);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(151, 20);
			this.textBox1.TabIndex = 11;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Grid Width";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// trkImageOpacity
			// 
			this.trkImageOpacity.Location = new System.Drawing.Point(20, 109);
			this.trkImageOpacity.Maximum = 100;
			this.trkImageOpacity.Name = "trkImageOpacity";
			this.trkImageOpacity.Size = new System.Drawing.Size(214, 45);
			this.trkImageOpacity.SmallChange = 5;
			this.trkImageOpacity.TabIndex = 14;
			this.trkImageOpacity.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trkImageOpacity.Value = 100;
			this.trkImageOpacity.Scroll += new System.EventHandler(this.trkImageOpacity_Scroll);
			this.trkImageOpacity.ValueChanged += new System.EventHandler(this.trkImageOpacity_ValueChanged);
			// 
			// treeViewChannels
			// 
			this.treeViewChannels.AllowDrop = true;
			this.treeViewChannels.CustomDragCursor = null;
			this.treeViewChannels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewChannels.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
			this.treeViewChannels.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.treeViewChannels.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.treeViewChannels.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.treeViewChannels.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.treeViewChannels.Location = new System.Drawing.Point(0, 0);
			this.treeViewChannels.Name = "treeViewChannels";
			this.treeViewChannels.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeViewChannels.SelectedNodes")));
			this.treeViewChannels.Size = new System.Drawing.Size(246, 293);
			this.treeViewChannels.TabIndex = 0;
			this.treeViewChannels.UsingCustomDragCursor = false;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Black;
			this.splitContainer1.Panel1.Controls.Add(this.gridPanel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(914, 500);
			this.splitContainer1.SplitterDistance = 664;
			this.splitContainer1.TabIndex = 1;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// gridPanel
			// 
			this.gridPanel.BackColor = System.Drawing.Color.Black;
			this.gridPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridPanel.Location = new System.Drawing.Point(0, 0);
			this.gridPanel.Name = "gridPanel";
			this.gridPanel.Size = new System.Drawing.Size(664, 500);
			this.gridPanel.TabIndex = 1;
			// 
			// CustomPropForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(914, 500);
			this.Controls.Add(this.splitContainer1);
			this.Name = "CustomPropForm";
			this.Text = "Custom Prop Editor";
			this.Load += new System.EventHandler(this.CustomPropForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.numGridHeight)).EndInit();
			this.contextMenuChannels.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numGridWidth)).EndInit();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trkImageOpacity)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Remove;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Rename;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_AddMultiple;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.NumericUpDown numGridHeight;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Add;
		private System.Windows.Forms.ContextMenuStrip contextMenuChannels;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Button btnUpdateChannelCount;
		private System.Windows.Forms.NumericUpDown numGridWidth;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem changeChannelColorToolStripMenuItem;
		private System.Windows.Forms.Button btnLoadBackgroundImage;
		private System.Windows.Forms.TextBox txtBackgroundImage;
		private System.Windows.Forms.TrackBar trkImageOpacity;
		private System.Windows.Forms.CheckBox chkMaintainAspect;
		private System.Windows.Forms.Panel gridPanel;
		private Common.Controls.MultiSelectTreeview treeViewChannels;
	}
}

