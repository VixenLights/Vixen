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
			this.labelNudge = new System.Windows.Forms.Label();
			this.listViewMarkCollections = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemBold = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDottedSolid = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemChangeColor = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemNudgeSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStandardNudge)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSuperNudge)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
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
			this.toolStrip1.Size = new System.Drawing.Size(192, 25);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEditMarkCollection
			// 
			this.toolStripButtonEditMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditMarkCollection.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonEditMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditMarkCollection.Name = "toolStripButtonEditMarkCollection";
			this.toolStripButtonEditMarkCollection.Size = new System.Drawing.Size(31, 22);
			this.toolStripButtonEditMarkCollection.Text = "Edit";
			this.toolStripButtonEditMarkCollection.ToolTipText = "Open Mark Manager";
			this.toolStripButtonEditMarkCollection.Click += new System.EventHandler(this.toolStripButtonEditMarkCollection_Click);
			// 
			// toolStripButtonAddMarkCollection
			// 
			this.toolStripButtonAddMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonAddMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddMarkCollection.Name = "toolStripButtonAddMarkCollection";
			this.toolStripButtonAddMarkCollection.Size = new System.Drawing.Size(35, 22);
			this.toolStripButtonAddMarkCollection.Text = "New";
			this.toolStripButtonAddMarkCollection.ToolTipText = "New Collection";
			this.toolStripButtonAddMarkCollection.Click += new System.EventHandler(this.toolStripButtonAddMarkCollection_Click);
			// 
			// toolStripButtonDeleteMarkCollection
			// 
			this.toolStripButtonDeleteMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteMarkCollection.Name = "toolStripButtonDeleteMarkCollection";
			this.toolStripButtonDeleteMarkCollection.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonDeleteMarkCollection.Text = "Delete";
			this.toolStripButtonDeleteMarkCollection.ToolTipText = "Delete Selected Collection(s)";
			this.toolStripButtonDeleteMarkCollection.Click += new System.EventHandler(this.toolStripButtonDeleteMarkCollection_Click);
			// 
			// numericUpDownStandardNudge
			// 
			this.numericUpDownStandardNudge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDownStandardNudge.AutoSize = true;
			this.numericUpDownStandardNudge.Location = new System.Drawing.Point(54, 6);
			this.numericUpDownStandardNudge.Margin = new System.Windows.Forms.Padding(2);
			this.numericUpDownStandardNudge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownStandardNudge.Name = "numericUpDownStandardNudge";
			this.numericUpDownStandardNudge.Size = new System.Drawing.Size(41, 20);
			this.numericUpDownStandardNudge.TabIndex = 1;
			this.toolTip1.SetToolTip(this.numericUpDownStandardNudge, "Standard Nudge in Milliseconds");
			this.numericUpDownStandardNudge.ValueChanged += new System.EventHandler(this.numericUpDownStandardNudge_ValueChanged);
			// 
			// numericUpDownSuperNudge
			// 
			this.numericUpDownSuperNudge.AutoSize = true;
			this.numericUpDownSuperNudge.Location = new System.Drawing.Point(105, 6);
			this.numericUpDownSuperNudge.Margin = new System.Windows.Forms.Padding(2);
			this.numericUpDownSuperNudge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownSuperNudge.Name = "numericUpDownSuperNudge";
			this.numericUpDownSuperNudge.Size = new System.Drawing.Size(41, 20);
			this.numericUpDownSuperNudge.TabIndex = 3;
			this.toolTip1.SetToolTip(this.numericUpDownSuperNudge, "Super Nudge in Milliseconds");
			this.numericUpDownSuperNudge.ValueChanged += new System.EventHandler(this.numericUpDownSuperNudge_ValueChanged);
			// 
			// labelNudge
			// 
			this.labelNudge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelNudge.AutoSize = true;
			this.labelNudge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.labelNudge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.labelNudge.Location = new System.Drawing.Point(5, 8);
			this.labelNudge.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelNudge.Name = "labelNudge";
			this.labelNudge.Size = new System.Drawing.Size(39, 13);
			this.labelNudge.TabIndex = 4;
			this.labelNudge.Text = "Nudge";
			// 
			// listViewMarkCollections
			// 
			this.listViewMarkCollections.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewMarkCollections.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
			this.listViewMarkCollections.Location = new System.Drawing.Point(0, 25);
			this.listViewMarkCollections.Margin = new System.Windows.Forms.Padding(2);
			this.listViewMarkCollections.Name = "listViewMarkCollections";
			this.listViewMarkCollections.Size = new System.Drawing.Size(192, 331);
			this.listViewMarkCollections.TabIndex = 8;
			this.listViewMarkCollections.UseCompatibleStateImageBehavior = false;
			this.listViewMarkCollections.View = System.Windows.Forms.View.Details;
			this.listViewMarkCollections.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewMarkCollections_AfterLabelEdit);
			this.listViewMarkCollections.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewMarkCollections_ItemCheck);
			this.listViewMarkCollections.SelectedIndexChanged += new System.EventHandler(this.listViewMarkCollections_SelectedIndexChanged);
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
            this.toolStripMenuItemBold,
            this.toolStripMenuItemDottedSolid,
            this.ToolStripMenuItemChangeColor,
            this.toolStripSeparator1,
            this.toolStripMenuItemNudgeSettings});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(169, 98);
			// 
			// toolStripMenuItemBold
			// 
			this.toolStripMenuItemBold.Enabled = false;
			this.toolStripMenuItemBold.Name = "toolStripMenuItemBold";
			this.toolStripMenuItemBold.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItemBold.Text = "Normal/Bold Line";
			this.toolStripMenuItemBold.ToolTipText = "Toggles the Selected Mark Collection";
			this.toolStripMenuItemBold.Click += new System.EventHandler(this.boldToolStripMenuItem_Click);
			// 
			// toolStripMenuItemDottedSolid
			// 
			this.toolStripMenuItemDottedSolid.Enabled = false;
			this.toolStripMenuItemDottedSolid.Name = "toolStripMenuItemDottedSolid";
			this.toolStripMenuItemDottedSolid.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItemDottedSolid.Text = "Dotted/Solid Line";
			this.toolStripMenuItemDottedSolid.ToolTipText = "Toggles the Selected Mark Collection";
			this.toolStripMenuItemDottedSolid.Click += new System.EventHandler(this.dottedSolidToolStripMenuItem_Click);
			// 
			// ToolStripMenuItemChangeColor
			// 
			this.ToolStripMenuItemChangeColor.Enabled = false;
			this.ToolStripMenuItemChangeColor.Name = "ToolStripMenuItemChangeColor";
			this.ToolStripMenuItemChangeColor.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItemChangeColor.Text = "Change Color";
			this.ToolStripMenuItemChangeColor.ToolTipText = "Change Mark Collection color";
			this.ToolStripMenuItemChangeColor.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
			// 
			// toolStripMenuItemNudgeSettings
			// 
			this.toolStripMenuItemNudgeSettings.Name = "toolStripMenuItemNudgeSettings";
			this.toolStripMenuItemNudgeSettings.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItemNudgeSettings.Text = "Nudge Settings";
			this.toolStripMenuItemNudgeSettings.Click += new System.EventHandler(this.toolStripMenuItemNudgeSettings_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.numericUpDownSuperNudge);
			this.panel1.Controls.Add(this.labelNudge);
			this.panel1.Controls.Add(this.numericUpDownStandardNudge);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 321);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(192, 35);
			this.panel1.TabIndex = 9;
			this.panel1.Visible = false;
			// 
			// Form_Marks
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(192, 356);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.listViewMarkCollections);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Form_Marks";
			this.Text = "Mark Collections";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Marks_Closing);
			this.Load += new System.EventHandler(this.Form_Marks_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStandardNudge)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSuperNudge)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditMarkCollection;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddMarkCollection;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteMarkCollection;
		private System.Windows.Forms.NumericUpDown numericUpDownSuperNudge;
		private System.Windows.Forms.NumericUpDown numericUpDownStandardNudge;
		private System.Windows.Forms.ListView listViewMarkCollections;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label labelNudge;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBold;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDottedSolid;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemChangeColor;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNudgeSettings;
		private System.Windows.Forms.Panel panel1;
	}
}