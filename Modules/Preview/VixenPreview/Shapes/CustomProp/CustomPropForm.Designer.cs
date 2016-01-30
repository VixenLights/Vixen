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
			this.label1 = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.chkMaintainAspect = new System.Windows.Forms.CheckBox();
			this.btnLoadBackgroundImage = new System.Windows.Forms.Button();
			this.txtBackgroundImage = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.trkImageOpacity = new System.Windows.Forms.TrackBar();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.treeViewChannels = new Common.Controls.MultiSelectTreeview();
			this.contextMenuStripChannels = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addMultipleNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gridPanel = new System.Windows.Forms.Panel();
			this.contextMenuStripPixels = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.removeSelectedItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkImageOpacity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.contextMenuStripChannels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.contextMenuStripPixels.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Prop Name";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(141, 89);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(93, 27);
			this.btnSave.TabIndex = 9;
			this.btnSave.Text = "Save and Exit";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label4.Location = new System.Drawing.Point(0, 119);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Channels";
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
			this.splitContainer2.Panel1.Controls.Add(this.label4);
			this.splitContainer2.Panel1.Controls.Add(this.trkImageOpacity);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(246, 500);
			this.splitContainer2.SplitterDistance = 132;
			this.splitContainer2.TabIndex = 0;
			// 
			// chkMaintainAspect
			// 
			this.chkMaintainAspect.AutoSize = true;
			this.chkMaintainAspect.Checked = true;
			this.chkMaintainAspect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkMaintainAspect.Location = new System.Drawing.Point(20, 95);
			this.chkMaintainAspect.Name = "chkMaintainAspect";
			this.chkMaintainAspect.Size = new System.Drawing.Size(121, 17);
			this.chkMaintainAspect.TabIndex = 15;
			this.chkMaintainAspect.Text = "Retain Aspect Ratio";
			this.chkMaintainAspect.UseVisualStyleBackColor = true;
			this.chkMaintainAspect.CheckedChanged += new System.EventHandler(this.chkMaintainAspect_CheckedChanged);
			// 
			// btnLoadBackgroundImage
			// 
			this.btnLoadBackgroundImage.Location = new System.Drawing.Point(208, 35);
			this.btnLoadBackgroundImage.Name = "btnLoadBackgroundImage";
			this.btnLoadBackgroundImage.Size = new System.Drawing.Size(26, 20);
			this.btnLoadBackgroundImage.TabIndex = 13;
			this.btnLoadBackgroundImage.Text = "...";
			this.btnLoadBackgroundImage.UseVisualStyleBackColor = true;
			this.btnLoadBackgroundImage.Click += new System.EventHandler(this.btnLoadBackgroundImage_Click);
			// 
			// txtBackgroundImage
			// 
			this.txtBackgroundImage.Location = new System.Drawing.Point(20, 35);
			this.txtBackgroundImage.Name = "txtBackgroundImage";
			this.txtBackgroundImage.ReadOnly = true;
			this.txtBackgroundImage.Size = new System.Drawing.Size(190, 20);
			this.txtBackgroundImage.TabIndex = 12;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(83, 9);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(151, 20);
			this.textBox1.TabIndex = 11;
			// 
			// trkImageOpacity
			// 
			this.trkImageOpacity.Location = new System.Drawing.Point(20, 61);
			this.trkImageOpacity.Maximum = 100;
			this.trkImageOpacity.Name = "trkImageOpacity";
			this.trkImageOpacity.Size = new System.Drawing.Size(214, 45);
			this.trkImageOpacity.SmallChange = 5;
			this.trkImageOpacity.TabIndex = 14;
			this.trkImageOpacity.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trkImageOpacity.Value = 100;
			this.trkImageOpacity.Scroll += new System.EventHandler(this.trkImageOpacity_ValueChanged);
			this.trkImageOpacity.ValueChanged += new System.EventHandler(this.trkImageOpacity_ValueChanged);
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.treeViewChannels);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer3.Size = new System.Drawing.Size(246, 364);
			this.splitContainer3.SplitterDistance = 222;
			this.splitContainer3.TabIndex = 1;
			// 
			// treeViewChannels
			// 
			this.treeViewChannels.AllowDrop = true;
			this.treeViewChannels.ContextMenuStrip = this.contextMenuStripChannels;
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
			this.treeViewChannels.Size = new System.Drawing.Size(246, 222);
			this.treeViewChannels.TabIndex = 0;
			this.treeViewChannels.UsingCustomDragCursor = false;
			this.treeViewChannels.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewChannels_AfterSelect);
			// 
			// contextMenuStripChannels
			// 
			this.contextMenuStripChannels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNodeToolStripMenuItem,
            this.addMultipleNodesToolStripMenuItem,
            this.removeNodesToolStripMenuItem});
			this.contextMenuStripChannels.Name = "contextMenuStripChannels";
			this.contextMenuStripChannels.Size = new System.Drawing.Size(181, 70);
			this.contextMenuStripChannels.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripChannels_Opening);
			// 
			// addNodeToolStripMenuItem
			// 
			this.addNodeToolStripMenuItem.Name = "addNodeToolStripMenuItem";
			this.addNodeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.addNodeToolStripMenuItem.Text = "Add Node";
			this.addNodeToolStripMenuItem.Click += new System.EventHandler(this.addNodeToolStripMenuItem_Click);
			// 
			// addMultipleNodesToolStripMenuItem
			// 
			this.addMultipleNodesToolStripMenuItem.Name = "addMultipleNodesToolStripMenuItem";
			this.addMultipleNodesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.addMultipleNodesToolStripMenuItem.Text = "Add Multiple Nodes";
			this.addMultipleNodesToolStripMenuItem.Click += new System.EventHandler(this.addMultipleNodesToolStripMenuItem_Click);
			// 
			// removeNodesToolStripMenuItem
			// 
			this.removeNodesToolStripMenuItem.Name = "removeNodesToolStripMenuItem";
			this.removeNodesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.removeNodesToolStripMenuItem.Text = "Remove Node(s)";
			this.removeNodesToolStripMenuItem.Click += new System.EventHandler(this.removeNodesToolStripMenuItem_Click);
			// 
			// propertyGrid
			// 
			this.propertyGrid.BackColor = System.Drawing.SystemColors.Control;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(246, 138);
			this.propertyGrid.TabIndex = 3;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
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
			// 
			// gridPanel
			// 
			this.gridPanel.BackColor = System.Drawing.Color.Transparent;
			this.gridPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.gridPanel.ContextMenuStrip = this.contextMenuStripPixels;
			this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridPanel.Location = new System.Drawing.Point(0, 0);
			this.gridPanel.Name = "gridPanel";
			this.gridPanel.Size = new System.Drawing.Size(664, 500);
			this.gridPanel.TabIndex = 1;
			this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPanel_Paint);
			this.gridPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseDown);
			this.gridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseMove);
			this.gridPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseUp);
			// 
			// contextMenuStripPixels
			// 
			this.contextMenuStripPixels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedItemsToolStripMenuItem});
			this.contextMenuStripPixels.Name = "contextMenuStripPixels";
			this.contextMenuStripPixels.Size = new System.Drawing.Size(197, 26);
			this.contextMenuStripPixels.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripPixels_Opening);
			// 
			// removeSelectedItemsToolStripMenuItem
			// 
			this.removeSelectedItemsToolStripMenuItem.Name = "removeSelectedItemsToolStripMenuItem";
			this.removeSelectedItemsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			this.removeSelectedItemsToolStripMenuItem.Text = "Remove Selected Items";
			this.removeSelectedItemsToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedItemsToolStripMenuItem_Click);
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
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trkImageOpacity)).EndInit();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.contextMenuStripChannels.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.contextMenuStripPixels.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnLoadBackgroundImage;
		private System.Windows.Forms.TextBox txtBackgroundImage;
		private System.Windows.Forms.TrackBar trkImageOpacity;
		private System.Windows.Forms.CheckBox chkMaintainAspect;
		private System.Windows.Forms.Panel gridPanel;
		private Common.Controls.MultiSelectTreeview treeViewChannels;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripPixels;
		private System.Windows.Forms.ToolStripMenuItem removeSelectedItemsToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripChannels;
		private System.Windows.Forms.ToolStripMenuItem addNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addMultipleNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeNodesToolStripMenuItem;
	}
}

