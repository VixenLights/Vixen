namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_GradientLibrary
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
			this.toolStripGradients = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportGradients = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportGradients = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkGradients = new System.Windows.Forms.CheckBox();
			this.listViewGradients = new System.Windows.Forms.ListView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolStripGradients.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripGradients
			// 
			this.toolStripGradients.AutoSize = false;
			this.toolStripGradients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.toolStripGradients.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStripGradients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditGradient,
            this.toolStripButtonNewGradient,
            this.toolStripButtonDeleteGradient,
            this.toolStripButtonExportGradients,
            this.toolStripButtonImportGradients});
			this.toolStripGradients.Location = new System.Drawing.Point(0, 0);
			this.toolStripGradients.Name = "toolStripGradients";
			this.toolStripGradients.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripGradients.Size = new System.Drawing.Size(602, 27);
			this.toolStripGradients.TabIndex = 10;
			this.toolStripGradients.Text = "Color Gradients";
			// 
			// toolStripButtonEditGradient
			// 
			this.toolStripButtonEditGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditGradient.Enabled = false;
			this.toolStripButtonEditGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditGradient.Name = "toolStripButtonEditGradient";
			this.toolStripButtonEditGradient.Size = new System.Drawing.Size(79, 24);
			this.toolStripButtonEditGradient.Text = "Edit Gradient";
			this.toolStripButtonEditGradient.Click += new System.EventHandler(this.toolStripButtonEditGradient_Click);
			// 
			// toolStripButtonNewGradient
			// 
			this.toolStripButtonNewGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewGradient.Name = "toolStripButtonNewGradient";
			this.toolStripButtonNewGradient.Size = new System.Drawing.Size(83, 24);
			this.toolStripButtonNewGradient.Text = "New Gradient";
			this.toolStripButtonNewGradient.Click += new System.EventHandler(this.toolStripButtonNewGradient_Click);
			// 
			// toolStripButtonDeleteGradient
			// 
			this.toolStripButtonDeleteGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteGradient.Enabled = false;
			this.toolStripButtonDeleteGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteGradient.Name = "toolStripButtonDeleteGradient";
			this.toolStripButtonDeleteGradient.Size = new System.Drawing.Size(92, 24);
			this.toolStripButtonDeleteGradient.Text = "Delete Gradient";
			this.toolStripButtonDeleteGradient.Click += new System.EventHandler(this.toolStripButtonDeleteGradient_Click);
			// 
			// toolStripButtonExportGradients
			// 
			this.toolStripButtonExportGradients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportGradients.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportGradients.Name = "toolStripButtonExportGradients";
			this.toolStripButtonExportGradients.Size = new System.Drawing.Size(44, 24);
			this.toolStripButtonExportGradients.Text = "Export";
			this.toolStripButtonExportGradients.ToolTipText = "Export Gradient Library";
			this.toolStripButtonExportGradients.Click += new System.EventHandler(this.toolStripButtonExportGradients_Click);
			// 
			// toolStripButtonImportGradients
			// 
			this.toolStripButtonImportGradients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportGradients.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportGradients.Name = "toolStripButtonImportGradients";
			this.toolStripButtonImportGradients.Size = new System.Drawing.Size(47, 24);
			this.toolStripButtonImportGradients.Text = "Import";
			this.toolStripButtonImportGradients.ToolTipText = "Import Gradient Library";
			this.toolStripButtonImportGradients.Click += new System.EventHandler(this.toolStripButtonImportGradients_Click);
			// 
			// checkBoxLinkGradients
			// 
			this.checkBoxLinkGradients.AutoSize = true;
			this.checkBoxLinkGradients.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.checkBoxLinkGradients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxLinkGradients.Location = new System.Drawing.Point(0, 402);
			this.checkBoxLinkGradients.Margin = new System.Windows.Forms.Padding(6);
			this.checkBoxLinkGradients.Name = "checkBoxLinkGradients";
			this.checkBoxLinkGradients.Size = new System.Drawing.Size(602, 17);
			this.checkBoxLinkGradients.TabIndex = 9;
			this.checkBoxLinkGradients.Text = "Maintain Library Link";
			this.checkBoxLinkGradients.UseVisualStyleBackColor = true;
			// 
			// listViewGradients
			// 
			this.listViewGradients.AllowDrop = true;
			this.listViewGradients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewGradients.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGradients.Location = new System.Drawing.Point(0, 0);
			this.listViewGradients.Margin = new System.Windows.Forms.Padding(6);
			this.listViewGradients.Name = "listViewGradients";
			this.listViewGradients.Size = new System.Drawing.Size(602, 375);
			this.listViewGradients.TabIndex = 0;
			this.listViewGradients.UseCompatibleStateImageBehavior = false;
			this.listViewGradients.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewGradient_ItemDrag);
			this.listViewGradients.SelectedIndexChanged += new System.EventHandler(this.listViewGradients_SelectedIndexChanged);
			this.listViewGradients.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragDrop);
			this.listViewGradients.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragEnter);
			this.listViewGradients.DragLeave += new System.EventHandler(this.listViewGradients_DragLeave);
			this.listViewGradients.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewGradients_KeyDown);
			this.listViewGradients.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewGradients_KeyUp);
			this.listViewGradients.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewGradients_MouseDoubleClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listViewGradients);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(602, 375);
			this.panel1.TabIndex = 11;
			// 
			// Form_GradientLibrary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(602, 419);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.checkBoxLinkGradients);
			this.Controls.Add(this.toolStripGradients);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form_GradientLibrary";
			this.Text = "Gradient Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolPalette_FormClosing);
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.toolStripGradients.ResumeLayout(false);
			this.toolStripGradients.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxLinkGradients;
		private System.Windows.Forms.ListView listViewGradients;
		private System.Windows.Forms.ToolStrip toolStripGradients;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditGradient;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewGradient;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteGradient;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportGradients;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportGradients;
		private System.Windows.Forms.Panel panel1;


	}
}