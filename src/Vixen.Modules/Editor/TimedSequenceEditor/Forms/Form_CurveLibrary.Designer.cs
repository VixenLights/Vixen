namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_CurveLibrary
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
			this.toolStripCurves = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportCurves = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportCurves = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkCurves = new System.Windows.Forms.CheckBox();
			this.listViewCurves = new System.Windows.Forms.ListView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolStripCurves.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripCurves
			// 
			this.toolStripCurves.AutoSize = false;
			this.toolStripCurves.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.toolStripCurves.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStripCurves.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditCurve,
            this.toolStripButtonNewCurve,
            this.toolStripButtonDeleteCurve,
            this.toolStripButtonExportCurves,
            this.toolStripButtonImportCurves});
			this.toolStripCurves.Location = new System.Drawing.Point(0, 0);
			this.toolStripCurves.Name = "toolStripCurves";
			this.toolStripCurves.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripCurves.Size = new System.Drawing.Size(602, 27);
			this.toolStripCurves.TabIndex = 6;
			this.toolStripCurves.Text = "Curves";
			// 
			// toolStripButtonEditCurve
			// 
			this.toolStripButtonEditCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditCurve.Enabled = false;
			this.toolStripButtonEditCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditCurve.Name = "toolStripButtonEditCurve";
			this.toolStripButtonEditCurve.Size = new System.Drawing.Size(65, 24);
			this.toolStripButtonEditCurve.Text = "Edit Curve";
			this.toolStripButtonEditCurve.Click += new System.EventHandler(this.toolStripButtonEditCurve_Click);
			// 
			// toolStripButtonNewCurve
			// 
			this.toolStripButtonNewCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewCurve.Name = "toolStripButtonNewCurve";
			this.toolStripButtonNewCurve.Size = new System.Drawing.Size(69, 24);
			this.toolStripButtonNewCurve.Text = "New Curve";
			this.toolStripButtonNewCurve.Click += new System.EventHandler(this.toolStripButtonNewCurve_Click);
			// 
			// toolStripButtonDeleteCurve
			// 
			this.toolStripButtonDeleteCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteCurve.Enabled = false;
			this.toolStripButtonDeleteCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteCurve.Name = "toolStripButtonDeleteCurve";
			this.toolStripButtonDeleteCurve.Size = new System.Drawing.Size(78, 24);
			this.toolStripButtonDeleteCurve.Text = "Delete Curve";
			this.toolStripButtonDeleteCurve.Click += new System.EventHandler(this.toolStripButtonDeleteCurve_Click);
			// 
			// toolStripButtonExportCurves
			// 
			this.toolStripButtonExportCurves.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportCurves.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportCurves.Name = "toolStripButtonExportCurves";
			this.toolStripButtonExportCurves.Size = new System.Drawing.Size(44, 24);
			this.toolStripButtonExportCurves.Text = "Export";
			this.toolStripButtonExportCurves.ToolTipText = "Export Curve Library";
			this.toolStripButtonExportCurves.Click += new System.EventHandler(this.toolStripButtonExportCurves_Click);
			// 
			// toolStripButtonImportCurves
			// 
			this.toolStripButtonImportCurves.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportCurves.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportCurves.Name = "toolStripButtonImportCurves";
			this.toolStripButtonImportCurves.Size = new System.Drawing.Size(47, 24);
			this.toolStripButtonImportCurves.Text = "Import";
			this.toolStripButtonImportCurves.ToolTipText = "Import Curve Library";
			this.toolStripButtonImportCurves.Click += new System.EventHandler(this.toolStripButtonImportCurves_Click);
			// 
			// checkBoxLinkCurves
			// 
			this.checkBoxLinkCurves.AutoSize = true;
			this.checkBoxLinkCurves.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.checkBoxLinkCurves.Location = new System.Drawing.Point(0, 402);
			this.checkBoxLinkCurves.Margin = new System.Windows.Forms.Padding(6);
			this.checkBoxLinkCurves.Name = "checkBoxLinkCurves";
			this.checkBoxLinkCurves.Size = new System.Drawing.Size(602, 17);
			this.checkBoxLinkCurves.TabIndex = 4;
			this.checkBoxLinkCurves.Text = "Maintain Library Link";
			this.checkBoxLinkCurves.UseVisualStyleBackColor = true;
			// 
			// listViewCurves
			// 
			this.listViewCurves.AllowDrop = true;
			this.listViewCurves.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewCurves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewCurves.Location = new System.Drawing.Point(0, 0);
			this.listViewCurves.Margin = new System.Windows.Forms.Padding(6);
			this.listViewCurves.Name = "listViewCurves";
			this.listViewCurves.Size = new System.Drawing.Size(602, 375);
			this.listViewCurves.TabIndex = 0;
			this.listViewCurves.UseCompatibleStateImageBehavior = false;
			this.listViewCurves.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewCurves_ItemDrag);
			this.listViewCurves.SelectedIndexChanged += new System.EventHandler(this.listViewCurves_SelectedIndexChanged);
			this.listViewCurves.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragDrop);
			this.listViewCurves.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragEnter);
			this.listViewCurves.DragLeave += new System.EventHandler(this.listViewCurves_DragLeave);
			this.listViewCurves.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewCurves_KeyDown);
			this.listViewCurves.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewCurves_KeyUp);
			this.listViewCurves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCurves_MouseDoubleClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listViewCurves);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(602, 375);
			this.panel1.TabIndex = 7;
			// 
			// Form_CurveLibrary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(602, 419);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStripCurves);
			this.Controls.Add(this.checkBoxLinkCurves);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form_CurveLibrary";
			this.Text = "Curve Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolPalette_FormClosing);
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.toolStripCurves.ResumeLayout(false);
			this.toolStripCurves.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxLinkCurves;
		private System.Windows.Forms.ToolStrip toolStripCurves;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportCurves;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportCurves;
		private System.Windows.Forms.ListView listViewCurves;
		private System.Windows.Forms.Panel panel1;


	}
}