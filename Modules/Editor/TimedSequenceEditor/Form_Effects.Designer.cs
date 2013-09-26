namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_Effects
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
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Basic Lighting");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Advanced Lighting");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Device Action");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Effects));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.toolStripEffects = new System.Windows.Forms.ToolStrip();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.treeEffects = new System.Windows.Forms.TreeView();
			this.effectTreeImages = new System.Windows.Forms.ImageList(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(256, 402);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.toolStripEffects);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(248, 376);
			this.tabPage1.TabIndex = 3;
			this.tabPage1.Text = "Standard";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// toolStripEffects
			// 
			this.toolStripEffects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripEffects.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripEffects.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.toolStripEffects.Location = new System.Drawing.Point(0, 0);
			this.toolStripEffects.Name = "toolStripEffects";
			this.toolStripEffects.Size = new System.Drawing.Size(248, 376);
			this.toolStripEffects.TabIndex = 1;
			this.toolStripEffects.Text = "toolStrip1";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.treeEffects);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(248, 376);
			this.tabPage2.TabIndex = 4;
			this.tabPage2.Text = "Effect Tree";
			this.tabPage2.UseVisualStyleBackColor = true;
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
			treeNode4.ImageKey = "bullet_arrow_Right.png";
			treeNode4.Name = "treeBasic";
			treeNode4.Text = "Basic Lighting";
			treeNode5.ImageKey = "bullet_arrow_Right.png";
			treeNode5.Name = "treeAdvanced";
			treeNode5.Text = "Advanced Lighting";
			treeNode6.ImageKey = "bullet_arrow_Right.png";
			treeNode6.Name = "treeAction";
			treeNode6.SelectedImageKey = "(default)";
			treeNode6.StateImageKey = "(none)";
			treeNode6.Text = "Device Action";
			this.treeEffects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
			this.treeEffects.SelectedImageIndex = 0;
			this.treeEffects.ShowLines = false;
			this.treeEffects.ShowPlusMinus = false;
			this.treeEffects.ShowRootLines = false;
			this.treeEffects.Size = new System.Drawing.Size(248, 376);
			this.treeEffects.TabIndex = 0;
			this.treeEffects.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeEffects_AfterCollapse);
			this.treeEffects.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeEffects_AfterExpand);
			this.treeEffects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeEffects_MouseDown);
			// 
			// effectTreeImages
			// 
			this.effectTreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("effectTreeImages.ImageStream")));
			this.effectTreeImages.TransparentColor = System.Drawing.Color.Transparent;
			this.effectTreeImages.Images.SetKeyName(0, "bullet_arrow_Right.png");
			this.effectTreeImages.Images.SetKeyName(1, "bullet_arrow_down.png");
			this.effectTreeImages.Images.SetKeyName(2, "blank.png");
			// 
			// Form_Effects
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(256, 402);
			this.ControlBox = false;
			this.Controls.Add(this.tabControl1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form_Effects";
			this.Text = "Effects";
			this.Load += new System.EventHandler(this.Form_Effects_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ToolStrip toolStripEffects;
		private System.Windows.Forms.ImageList effectTreeImages;
		private System.Windows.Forms.TreeView treeEffects;
	}
}