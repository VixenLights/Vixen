namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_Marks
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Marks));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAddMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.numericUpDownStandardNudge = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownSuperNudge = new System.Windows.Forms.NumericUpDown();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listViewMarkCollections = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.boldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dottedSolidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStandardNudge)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSuperNudge)).BeginInit();
			this.panel1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditMarkCollection,
            this.toolStripButtonAddMarkCollection,
            this.toolStripButtonDeleteMarkCollection});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(256, 27);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEditMarkCollection
			// 
			this.toolStripButtonEditMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonEditMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditMarkCollection.Image")));
			this.toolStripButtonEditMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditMarkCollection.Name = "toolStripButtonEditMarkCollection";
			this.toolStripButtonEditMarkCollection.Size = new System.Drawing.Size(24, 24);
			this.toolStripButtonEditMarkCollection.Text = "toolStripButtonEditMarkCollections";
			this.toolStripButtonEditMarkCollection.ToolTipText = "Edit Mark Collections";
			this.toolStripButtonEditMarkCollection.Click += new System.EventHandler(this.toolStripButtonEditMarkCollection_Click);
			// 
			// toolStripButtonAddMarkCollection
			// 
			this.toolStripButtonAddMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAddMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddMarkCollection.Image")));
			this.toolStripButtonAddMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddMarkCollection.Name = "toolStripButtonAddMarkCollection";
			this.toolStripButtonAddMarkCollection.Size = new System.Drawing.Size(24, 24);
			this.toolStripButtonAddMarkCollection.ToolTipText = "New Mark Collection";
			this.toolStripButtonAddMarkCollection.Click += new System.EventHandler(this.toolStripButtonAddMarkCollection_Click);
			// 
			// toolStripButtonDeleteMarkCollection
			// 
			this.toolStripButtonDeleteMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDeleteMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteMarkCollection.Image")));
			this.toolStripButtonDeleteMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteMarkCollection.Name = "toolStripButtonDeleteMarkCollection";
			this.toolStripButtonDeleteMarkCollection.Size = new System.Drawing.Size(24, 24);
			this.toolStripButtonDeleteMarkCollection.Text = "toolStripButtonDeleteMarkCollection";
			this.toolStripButtonDeleteMarkCollection.ToolTipText = "Delete Mark Collection";
			this.toolStripButtonDeleteMarkCollection.Click += new System.EventHandler(this.toolStripButtonDeleteMarkCollection_Click);
			// 
			// numericUpDownStandardNudge
			// 
			this.numericUpDownStandardNudge.Location = new System.Drawing.Point(7, 20);
			this.numericUpDownStandardNudge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownStandardNudge.Name = "numericUpDownStandardNudge";
			this.numericUpDownStandardNudge.Size = new System.Drawing.Size(48, 23);
			this.numericUpDownStandardNudge.TabIndex = 1;
			this.toolTip1.SetToolTip(this.numericUpDownStandardNudge, "Standard Nudge in Miniseconds");
			this.numericUpDownStandardNudge.ValueChanged += new System.EventHandler(this.numericUpDownStandardNudge_ValueChanged);
			// 
			// numericUpDownSuperNudge
			// 
			this.numericUpDownSuperNudge.Location = new System.Drawing.Point(72, 20);
			this.numericUpDownSuperNudge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownSuperNudge.Name = "numericUpDownSuperNudge";
			this.numericUpDownSuperNudge.Size = new System.Drawing.Size(48, 23);
			this.numericUpDownSuperNudge.TabIndex = 3;
			this.toolTip1.SetToolTip(this.numericUpDownSuperNudge, "Super Nudge in Miliseconds");
			this.numericUpDownSuperNudge.ValueChanged += new System.EventHandler(this.numericUpDownSuperNudge_ValueChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.numericUpDownSuperNudge);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.numericUpDownStandardNudge);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 356);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(256, 46);
			this.panel1.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label3.Location = new System.Drawing.Point(3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(253, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Nudge";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label2.Location = new System.Drawing.Point(56, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(12, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "/";
			// 
			// listViewMarkCollections
			// 
			this.listViewMarkCollections.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewMarkCollections.CheckBoxes = true;
			this.listViewMarkCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewMarkCollections.ContextMenuStrip = this.contextMenuStrip1;
			this.listViewMarkCollections.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMarkCollections.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.listViewMarkCollections.FullRowSelect = true;
			this.listViewMarkCollections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMarkCollections.HideSelection = false;
			this.listViewMarkCollections.LabelEdit = true;
			this.listViewMarkCollections.Location = new System.Drawing.Point(0, 27);
			this.listViewMarkCollections.Name = "listViewMarkCollections";
			this.listViewMarkCollections.Size = new System.Drawing.Size(256, 329);
			this.listViewMarkCollections.TabIndex = 8;
			this.listViewMarkCollections.UseCompatibleStateImageBehavior = false;
			this.listViewMarkCollections.View = System.Windows.Forms.View.Details;
			this.listViewMarkCollections.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewMarkCollections_AfterLabelEdit);
			this.listViewMarkCollections.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewMarkCollections_ItemCheck);
			this.listViewMarkCollections.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.listViewMarkCollections_PreviewKeyDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Mark Collections";
			this.columnHeader1.Width = 999;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boldToolStripMenuItem,
            this.dottedSolidToolStripMenuItem,
            this.changeColorToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.ShowImageMargin = false;
			this.contextMenuStrip1.Size = new System.Drawing.Size(172, 104);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// boldToolStripMenuItem
			// 
			this.boldToolStripMenuItem.Name = "boldToolStripMenuItem";
			this.boldToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
			this.boldToolStripMenuItem.Text = "Normal/Bold Line";
			this.boldToolStripMenuItem.ToolTipText = "Toggles the Selected Mark Collection";
			this.boldToolStripMenuItem.Click += new System.EventHandler(this.boldToolStripMenuItem_Click);
			// 
			// dottedSolidToolStripMenuItem
			// 
			this.dottedSolidToolStripMenuItem.Name = "dottedSolidToolStripMenuItem";
			this.dottedSolidToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
			this.dottedSolidToolStripMenuItem.Text = "Dotted/Solid Line";
			this.dottedSolidToolStripMenuItem.ToolTipText = "Toggles the Selected Mark Collection";
			this.dottedSolidToolStripMenuItem.Click += new System.EventHandler(this.dottedSolidToolStripMenuItem_Click);
			// 
			// changeColorToolStripMenuItem
			// 
			this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
			this.changeColorToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
			this.changeColorToolStripMenuItem.Text = "Change Color";
			this.changeColorToolStripMenuItem.ToolTipText = "Change Mark Collection color";
			this.changeColorToolStripMenuItem.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
			// 
			// Form_Marks
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(256, 402);
			this.ControlBox = false;
			this.Controls.Add(this.listViewMarkCollections);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form_Marks";
			this.Text = "Marks";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Marks_Closing);
			this.Load += new System.EventHandler(this.Form_Marks_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStandardNudge)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSuperNudge)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditMarkCollection;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddMarkCollection;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteMarkCollection;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown numericUpDownSuperNudge;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownStandardNudge;
		private System.Windows.Forms.ListView listViewMarkCollections;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem boldToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dottedSolidToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem;
	}
}