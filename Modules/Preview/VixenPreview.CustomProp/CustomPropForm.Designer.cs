namespace VixenModules.Preview.VixenPreview.CustomProp
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
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btnUpdateChannelCount = new System.Windows.Forms.Button();
			this.numGridWidth = new System.Windows.Forms.NumericUpDown();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.label2 = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnOpen = new System.Windows.Forms.Button();
			this.changeChannelColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.numGridHeight)).BeginInit();
			this.contextMenuChannels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numGridWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
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
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(159, 87);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 76);
			this.btnSave.TabIndex = 9;
			this.btnSave.Text = "Save";
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
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Grid Height";
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
			this.contextMenuChannels.Size = new System.Drawing.Size(196, 148);
			this.contextMenuChannels.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuChannels_Opening);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// listBox1
			// 
			this.listBox1.ContextMenuStrip = this.contextMenuChannels;
			this.listBox1.DisplayMember = "ID_Text";
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(246, 293);
			this.listBox1.TabIndex = 9;
			this.listBox1.ValueMember = "Text";
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// btnUpdateChannelCount
			// 
			this.btnUpdateChannelCount.Location = new System.Drawing.Point(20, 87);
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
			this.numGridWidth.ValueChanged += new System.EventHandler(this.numGridHeight_ValueChanged);
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
			this.splitContainer2.Panel1.Controls.Add(this.btnOpen);
			this.splitContainer2.Panel1.Controls.Add(this.textBox1);
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			this.splitContainer2.Panel1.Controls.Add(this.btnSave);
			this.splitContainer2.Panel1.Controls.Add(this.numGridHeight);
			this.splitContainer2.Panel1.Controls.Add(this.label4);
			this.splitContainer2.Panel1.Controls.Add(this.label3);
			this.splitContainer2.Panel1.Controls.Add(this.btnUpdateChannelCount);
			this.splitContainer2.Panel1.Controls.Add(this.numGridWidth);
			this.splitContainer2.Panel1.Controls.Add(this.label2);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.listBox1);
			this.splitContainer2.Size = new System.Drawing.Size(246, 500);
			this.splitContainer2.SplitterDistance = 203;
			this.splitContainer2.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Grid Width";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(914, 500);
			this.splitContainer1.SplitterDistance = 664;
			this.splitContainer1.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(664, 500);
			this.panel1.TabIndex = 1;
			this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(83, 9);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(151, 20);
			this.textBox1.TabIndex = 11;
			// 
			// btnOpen
			// 
			this.btnOpen.Location = new System.Drawing.Point(20, 134);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(97, 29);
			this.btnOpen.TabIndex = 12;
			this.btnOpen.Text = "Open / Import";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// changeChannelColorToolStripMenuItem
			// 
			this.changeChannelColorToolStripMenuItem.Name = "changeChannelColorToolStripMenuItem";
			this.changeChannelColorToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.changeChannelColorToolStripMenuItem.Text = "Change Channel Color";
			this.changeChannelColorToolStripMenuItem.Click += new System.EventHandler(this.changeChannelColorToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
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
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
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
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btnUpdateChannelCount;
		private System.Windows.Forms.NumericUpDown numGridWidth;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem changeChannelColorToolStripMenuItem;
	}
}

