namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_PresetEffects
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("User Preset Effects");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PresetEffects));
			this.treeEffects = new System.Windows.Forms.TreeView();
			this.effectTreeImages = new System.Windows.Forms.ImageList(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeEffects
			// 
			this.treeEffects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeEffects.FullRowSelect = true;
			this.treeEffects.ImageIndex = 0;
			this.treeEffects.ImageList = this.effectTreeImages;
			this.treeEffects.ItemHeight = 22;
			this.treeEffects.Location = new System.Drawing.Point(0, 0);
			this.treeEffects.Name = "treeEffects";
			treeNode1.ImageKey = "bullet_arrow_Right.png";
			treeNode1.Name = "treePreset";
			treeNode1.Text = "User Preset Effects";
			this.treeEffects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.treeEffects.SelectedImageIndex = 0;
			this.treeEffects.ShowLines = false;
			this.treeEffects.ShowPlusMinus = false;
			this.treeEffects.ShowRootLines = false;
			this.treeEffects.Size = new System.Drawing.Size(202, 442);
			this.treeEffects.TabIndex = 0;
			this.treeEffects.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeEffects_AfterCollapse);
			this.treeEffects.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeEffects_AfterExpand);
			this.treeEffects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeEffects_AfterSelect);
			this.treeEffects.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeEffects_MouseClick);
			this.treeEffects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeEffects_MouseDown);
			this.treeEffects.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeEffects_MouseMove);
			// 
			// effectTreeImages
			// 
			this.effectTreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("effectTreeImages.ImageStream")));
			this.effectTreeImages.TransparentColor = System.Drawing.Color.Transparent;
			this.effectTreeImages.Images.SetKeyName(0, "bullet_arrow_Right.png");
			this.effectTreeImages.Images.SetKeyName(1, "bullet_arrow_down.png");
			this.effectTreeImages.Images.SetKeyName(2, "blank.png");
			// 
			// toolStrip1
			// 
			this.toolStrip1.Enabled = false;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonExport,
            this.toolStripButtonImport});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(202, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip";
			this.toolStrip1.Visible = false;
			// 
			// toolStripButtonExport
			// 
			this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExport.Image")));
			this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExport.Name = "toolStripButtonExport";
			this.toolStripButtonExport.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonExport.Text = "Export";
			this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
			// 
			// toolStripButtonImport
			// 
			this.toolStripButtonImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImport.Image")));
			this.toolStripButtonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImport.Name = "toolStripButtonImport";
			this.toolStripButtonImport.Size = new System.Drawing.Size(47, 22);
			this.toolStripButtonImport.Text = "Import";
			this.toolStripButtonImport.Click += new System.EventHandler(this.toolStripButtonImport_Click);
			// 
			// Form_PresetEffects
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(202, 442);
			this.ControlBox = false;
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.treeEffects);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form_PresetEffects";
			this.Text = "User Preset Effects";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_PresetEffects_FormClosing);
			this.Load += new System.EventHandler(this.Form_PresetEffects_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList effectTreeImages;
		private System.Windows.Forms.TreeView treeEffects;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonExport;
		private System.Windows.Forms.ToolStripButton toolStripButtonImport;
	}
}