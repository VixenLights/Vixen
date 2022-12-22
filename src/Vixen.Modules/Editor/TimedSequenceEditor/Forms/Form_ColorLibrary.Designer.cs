namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_ColorLibrary
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
			this.toolStripColors = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportColors = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportColors = new System.Windows.Forms.ToolStripButton();
			this.listViewColors = new System.Windows.Forms.ListView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolStripColors.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripColors
			// 
			this.toolStripColors.AutoSize = false;
			this.toolStripColors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.toolStripColors.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStripColors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditColor,
            this.toolStripButtonNewColor,
            this.toolStripButtonDeleteColor,
            this.toolStripButtonExportColors,
            this.toolStripButtonImportColors});
			this.toolStripColors.Location = new System.Drawing.Point(0, 0);
			this.toolStripColors.Name = "toolStripColors";
			this.toolStripColors.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripColors.Size = new System.Drawing.Size(602, 27);
			this.toolStripColors.TabIndex = 3;
			this.toolStripColors.Text = "Colors";
			// 
			// toolStripButtonEditColor
			// 
			this.toolStripButtonEditColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditColor.Enabled = false;
			this.toolStripButtonEditColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditColor.Name = "toolStripButtonEditColor";
			this.toolStripButtonEditColor.Size = new System.Drawing.Size(63, 24);
			this.toolStripButtonEditColor.Text = "Edit Color";
			this.toolStripButtonEditColor.Click += new System.EventHandler(this.toolStripButtonEditColor_Click);
			// 
			// toolStripButtonNewColor
			// 
			this.toolStripButtonNewColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewColor.Name = "toolStripButtonNewColor";
			this.toolStripButtonNewColor.Size = new System.Drawing.Size(67, 24);
			this.toolStripButtonNewColor.Text = "New Color";
			this.toolStripButtonNewColor.Click += new System.EventHandler(this.toolStripButtonNewColor_Click);
			// 
			// toolStripButtonDeleteColor
			// 
			this.toolStripButtonDeleteColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteColor.Enabled = false;
			this.toolStripButtonDeleteColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteColor.Name = "toolStripButtonDeleteColor";
			this.toolStripButtonDeleteColor.Size = new System.Drawing.Size(76, 24);
			this.toolStripButtonDeleteColor.Text = "Delete Color";
			this.toolStripButtonDeleteColor.Click += new System.EventHandler(this.toolStripButtonDeleteColor_Click);
			// 
			// toolStripButtonExportColors
			// 
			this.toolStripButtonExportColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportColors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportColors.Name = "toolStripButtonExportColors";
			this.toolStripButtonExportColors.Size = new System.Drawing.Size(44, 24);
			this.toolStripButtonExportColors.Text = "Export";
			this.toolStripButtonExportColors.ToolTipText = "Export Favorite Colors";
			this.toolStripButtonExportColors.Click += new System.EventHandler(this.toolStripButtonExportColors_Click);
			// 
			// toolStripButtonImportColors
			// 
			this.toolStripButtonImportColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportColors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportColors.Name = "toolStripButtonImportColors";
			this.toolStripButtonImportColors.Size = new System.Drawing.Size(47, 24);
			this.toolStripButtonImportColors.Text = "Import";
			this.toolStripButtonImportColors.ToolTipText = "Import Favorite Colors";
			this.toolStripButtonImportColors.Click += new System.EventHandler(this.toolStripButtonImportColors_Click);
			// 
			// listViewColors
			// 
			this.listViewColors.AllowDrop = true;
			this.listViewColors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewColors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewColors.LabelWrap = false;
			this.listViewColors.Location = new System.Drawing.Point(0, 0);
			this.listViewColors.Margin = new System.Windows.Forms.Padding(6);
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(602, 392);
			this.listViewColors.TabIndex = 1;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			this.listViewColors.DragLeave += new System.EventHandler(this.listViewColors_DragLeave);
			this.listViewColors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewColors_MouseDoubleClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listViewColors);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(602, 392);
			this.panel1.TabIndex = 4;
			// 
			// Form_ColorLibrary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(602, 419);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStripColors);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form_ColorLibrary";
			this.Text = "Color Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ColorLibrary_FormClosing);
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.toolStripColors.ResumeLayout(false);
			this.toolStripColors.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.ToolStrip toolStripColors;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditColor;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewColor;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteColor;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportColors;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportColors;
		private System.Windows.Forms.Panel panel1;


	}
}