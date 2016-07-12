namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_ToolPalette
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
			this.toolStripCurves = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportCurves = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportCurves = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkCurves = new System.Windows.Forms.CheckBox();
			this.listViewCurves = new System.Windows.Forms.ListView();
			this.toolStripGradients = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportGradients = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportGradients = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkGradients = new System.Windows.Forms.CheckBox();
			this.listViewGradients = new System.Windows.Forms.ListView();
			this.tabControlEX1 = new Dotnetrix.Controls.TabControlEX();
			this.tabPageEX1 = new Dotnetrix.Controls.TabPageEX();
			this.tabPageEX2 = new Dotnetrix.Controls.TabPageEX();
			this.tabPageEX3 = new Dotnetrix.Controls.TabPageEX();
			this.toolStripColors.SuspendLayout();
			this.toolStripCurves.SuspendLayout();
			this.toolStripGradients.SuspendLayout();
			this.tabControlEX1.SuspendLayout();
			this.tabPageEX1.SuspendLayout();
			this.tabPageEX2.SuspendLayout();
			this.tabPageEX3.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripColors
			// 
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
			this.toolStripColors.Size = new System.Drawing.Size(594, 25);
			this.toolStripColors.TabIndex = 3;
			this.toolStripColors.Text = "Colors";
			// 
			// toolStripButtonEditColor
			// 
			this.toolStripButtonEditColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditColor.Enabled = false;
			this.toolStripButtonEditColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditColor.Name = "toolStripButtonEditColor";
			this.toolStripButtonEditColor.Size = new System.Drawing.Size(63, 22);
			this.toolStripButtonEditColor.Text = "Edit Color";
			this.toolStripButtonEditColor.Click += new System.EventHandler(this.toolStripButtonEditColor_Click);
			// 
			// toolStripButtonNewColor
			// 
			this.toolStripButtonNewColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewColor.Name = "toolStripButtonNewColor";
			this.toolStripButtonNewColor.Size = new System.Drawing.Size(67, 22);
			this.toolStripButtonNewColor.Text = "New Color";
			this.toolStripButtonNewColor.Click += new System.EventHandler(this.toolStripButtonNewColor_Click);
			// 
			// toolStripButtonDeleteColor
			// 
			this.toolStripButtonDeleteColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteColor.Enabled = false;
			this.toolStripButtonDeleteColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteColor.Name = "toolStripButtonDeleteColor";
			this.toolStripButtonDeleteColor.Size = new System.Drawing.Size(76, 22);
			this.toolStripButtonDeleteColor.Text = "Delete Color";
			this.toolStripButtonDeleteColor.Click += new System.EventHandler(this.toolStripButtonDeleteColor_Click);
			// 
			// toolStripButtonExportColors
			// 
			this.toolStripButtonExportColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportColors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportColors.Name = "toolStripButtonExportColors";
			this.toolStripButtonExportColors.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonExportColors.Text = "Export";
			this.toolStripButtonExportColors.ToolTipText = "Export Favorite Colors";
			this.toolStripButtonExportColors.Click += new System.EventHandler(this.toolStripButtonExportColors_Click);
			// 
			// toolStripButtonImportColors
			// 
			this.toolStripButtonImportColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportColors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportColors.Name = "toolStripButtonImportColors";
			this.toolStripButtonImportColors.Size = new System.Drawing.Size(47, 22);
			this.toolStripButtonImportColors.Text = "Import";
			this.toolStripButtonImportColors.ToolTipText = "Import Favorite Colors";
			this.toolStripButtonImportColors.Click += new System.EventHandler(this.toolStripButtonImportColors_Click);
			// 
			// listViewColors
			// 
			this.listViewColors.AllowDrop = true;
			this.listViewColors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewColors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewColors.Location = new System.Drawing.Point(0, 25);
			this.listViewColors.Margin = new System.Windows.Forms.Padding(6);
			this.listViewColors.MultiSelect = false;
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(594, 368);
			this.listViewColors.TabIndex = 1;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			this.listViewColors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewColors_MouseDoubleClick);
			// 
			// toolStripCurves
			// 
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
			this.toolStripCurves.Size = new System.Drawing.Size(594, 25);
			this.toolStripCurves.TabIndex = 6;
			this.toolStripCurves.Text = "Curves";
			// 
			// toolStripButtonEditCurve
			// 
			this.toolStripButtonEditCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditCurve.Enabled = false;
			this.toolStripButtonEditCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditCurve.Name = "toolStripButtonEditCurve";
			this.toolStripButtonEditCurve.Size = new System.Drawing.Size(65, 22);
			this.toolStripButtonEditCurve.Text = "Edit Curve";
			this.toolStripButtonEditCurve.Click += new System.EventHandler(this.toolStripButtonEditCurve_Click);
			// 
			// toolStripButtonNewCurve
			// 
			this.toolStripButtonNewCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewCurve.Name = "toolStripButtonNewCurve";
			this.toolStripButtonNewCurve.Size = new System.Drawing.Size(69, 22);
			this.toolStripButtonNewCurve.Text = "New Curve";
			this.toolStripButtonNewCurve.Click += new System.EventHandler(this.toolStripButtonNewCurve_Click);
			// 
			// toolStripButtonDeleteCurve
			// 
			this.toolStripButtonDeleteCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteCurve.Enabled = false;
			this.toolStripButtonDeleteCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteCurve.Name = "toolStripButtonDeleteCurve";
			this.toolStripButtonDeleteCurve.Size = new System.Drawing.Size(78, 22);
			this.toolStripButtonDeleteCurve.Text = "Delete Curve";
			this.toolStripButtonDeleteCurve.Click += new System.EventHandler(this.toolStripButtonDeleteCurve_Click);
			// 
			// toolStripButtonExportCurves
			// 
			this.toolStripButtonExportCurves.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportCurves.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportCurves.Name = "toolStripButtonExportCurves";
			this.toolStripButtonExportCurves.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonExportCurves.Text = "Export";
			this.toolStripButtonExportCurves.ToolTipText = "Export Curve Library";
			this.toolStripButtonExportCurves.Click += new System.EventHandler(this.toolStripButtonExportCurves_Click);
			// 
			// toolStripButtonImportCurves
			// 
			this.toolStripButtonImportCurves.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportCurves.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportCurves.Name = "toolStripButtonImportCurves";
			this.toolStripButtonImportCurves.Size = new System.Drawing.Size(47, 22);
			this.toolStripButtonImportCurves.Text = "Import";
			this.toolStripButtonImportCurves.ToolTipText = "Import Curve Library";
			this.toolStripButtonImportCurves.Click += new System.EventHandler(this.toolStripButtonImportCurves_Click);
			// 
			// checkBoxLinkCurves
			// 
			this.checkBoxLinkCurves.AutoSize = true;
			this.checkBoxLinkCurves.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.checkBoxLinkCurves.Location = new System.Drawing.Point(0, 376);
			this.checkBoxLinkCurves.Margin = new System.Windows.Forms.Padding(6);
			this.checkBoxLinkCurves.Name = "checkBoxLinkCurves";
			this.checkBoxLinkCurves.Size = new System.Drawing.Size(594, 17);
			this.checkBoxLinkCurves.TabIndex = 4;
			this.checkBoxLinkCurves.Text = "Maintain Library Link";
			this.checkBoxLinkCurves.UseVisualStyleBackColor = true;
			// 
			// listViewCurves
			// 
			this.listViewCurves.AllowDrop = true;
			this.listViewCurves.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewCurves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewCurves.Location = new System.Drawing.Point(0, 25);
			this.listViewCurves.Margin = new System.Windows.Forms.Padding(6);
			this.listViewCurves.Name = "listViewCurves";
			this.listViewCurves.Size = new System.Drawing.Size(594, 351);
			this.listViewCurves.TabIndex = 0;
			this.listViewCurves.UseCompatibleStateImageBehavior = false;
			this.listViewCurves.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewCurves_ItemDrag);
			this.listViewCurves.SelectedIndexChanged += new System.EventHandler(this.listViewCurves_SelectedIndexChanged);
			this.listViewCurves.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragDrop);
			this.listViewCurves.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragEnter);
			this.listViewCurves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCurves_MouseDoubleClick);
			// 
			// toolStripGradients
			// 
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
			this.toolStripGradients.Size = new System.Drawing.Size(594, 25);
			this.toolStripGradients.TabIndex = 10;
			this.toolStripGradients.Text = "Color Gradients";
			// 
			// toolStripButtonEditGradient
			// 
			this.toolStripButtonEditGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonEditGradient.Enabled = false;
			this.toolStripButtonEditGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditGradient.Name = "toolStripButtonEditGradient";
			this.toolStripButtonEditGradient.Size = new System.Drawing.Size(79, 22);
			this.toolStripButtonEditGradient.Text = "Edit Gradient";
			this.toolStripButtonEditGradient.Click += new System.EventHandler(this.toolStripButtonEditGradient_Click);
			// 
			// toolStripButtonNewGradient
			// 
			this.toolStripButtonNewGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonNewGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewGradient.Name = "toolStripButtonNewGradient";
			this.toolStripButtonNewGradient.Size = new System.Drawing.Size(83, 22);
			this.toolStripButtonNewGradient.Text = "New Gradient";
			this.toolStripButtonNewGradient.Click += new System.EventHandler(this.toolStripButtonNewGradient_Click);
			// 
			// toolStripButtonDeleteGradient
			// 
			this.toolStripButtonDeleteGradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDeleteGradient.Enabled = false;
			this.toolStripButtonDeleteGradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteGradient.Name = "toolStripButtonDeleteGradient";
			this.toolStripButtonDeleteGradient.Size = new System.Drawing.Size(92, 22);
			this.toolStripButtonDeleteGradient.Text = "Delete Gradient";
			this.toolStripButtonDeleteGradient.Click += new System.EventHandler(this.toolStripButtonDeleteGradient_Click);
			// 
			// toolStripButtonExportGradients
			// 
			this.toolStripButtonExportGradients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonExportGradients.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonExportGradients.Name = "toolStripButtonExportGradients";
			this.toolStripButtonExportGradients.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonExportGradients.Text = "Export";
			this.toolStripButtonExportGradients.ToolTipText = "Export Gradient Library";
			this.toolStripButtonExportGradients.Click += new System.EventHandler(this.toolStripButtonExportGradients_Click);
			// 
			// toolStripButtonImportGradients
			// 
			this.toolStripButtonImportGradients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonImportGradients.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImportGradients.Name = "toolStripButtonImportGradients";
			this.toolStripButtonImportGradients.Size = new System.Drawing.Size(47, 22);
			this.toolStripButtonImportGradients.Text = "Import";
			this.toolStripButtonImportGradients.ToolTipText = "Import Gradient Library";
			this.toolStripButtonImportGradients.Click += new System.EventHandler(this.toolStripButtonImportGradients_Click);
			// 
			// checkBoxLinkGradients
			// 
			this.checkBoxLinkGradients.AutoSize = true;
			this.checkBoxLinkGradients.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.checkBoxLinkGradients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxLinkGradients.Location = new System.Drawing.Point(0, 376);
			this.checkBoxLinkGradients.Margin = new System.Windows.Forms.Padding(6);
			this.checkBoxLinkGradients.Name = "checkBoxLinkGradients";
			this.checkBoxLinkGradients.Size = new System.Drawing.Size(594, 17);
			this.checkBoxLinkGradients.TabIndex = 9;
			this.checkBoxLinkGradients.Text = "Maintain Library Link";
			this.checkBoxLinkGradients.UseVisualStyleBackColor = true;
			// 
			// listViewGradients
			// 
			this.listViewGradients.AllowDrop = true;
			this.listViewGradients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewGradients.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGradients.Location = new System.Drawing.Point(0, 25);
			this.listViewGradients.Margin = new System.Windows.Forms.Padding(6);
			this.listViewGradients.Name = "listViewGradients";
			this.listViewGradients.Size = new System.Drawing.Size(594, 351);
			this.listViewGradients.TabIndex = 0;
			this.listViewGradients.UseCompatibleStateImageBehavior = false;
			this.listViewGradients.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewGradient_ItemDrag);
			this.listViewGradients.SelectedIndexChanged += new System.EventHandler(this.listViewGradients_SelectedIndexChanged);
			this.listViewGradients.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragDrop);
			this.listViewGradients.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragEnter);
			this.listViewGradients.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewGradients_MouseDoubleClick);
			// 
			// tabControlEX1
			// 
			this.tabControlEX1.Controls.Add(this.tabPageEX1);
			this.tabControlEX1.Controls.Add(this.tabPageEX2);
			this.tabControlEX1.Controls.Add(this.tabPageEX3);
			this.tabControlEX1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEX1.ItemSize = new System.Drawing.Size(42, 18);
			this.tabControlEX1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEX1.Margin = new System.Windows.Forms.Padding(6);
			this.tabControlEX1.Name = "tabControlEX1";
			this.tabControlEX1.SelectedIndex = 1;
			this.tabControlEX1.Size = new System.Drawing.Size(602, 419);
			this.tabControlEX1.TabIndex = 7;
			this.tabControlEX1.UseVisualStyles = false;
			// 
			// tabPageEX1
			// 
			this.tabPageEX1.Controls.Add(this.listViewColors);
			this.tabPageEX1.Controls.Add(this.toolStripColors);
			this.tabPageEX1.Location = new System.Drawing.Point(4, 22);
			this.tabPageEX1.Margin = new System.Windows.Forms.Padding(6);
			this.tabPageEX1.Name = "tabPageEX1";
			this.tabPageEX1.Size = new System.Drawing.Size(594, 393);
			this.tabPageEX1.TabIndex = 0;
			this.tabPageEX1.Text = "Colors";
			this.tabPageEX1.UseVisualStyleBackColor = true;
			// 
			// tabPageEX2
			// 
			this.tabPageEX2.Controls.Add(this.listViewCurves);
			this.tabPageEX2.Controls.Add(this.toolStripCurves);
			this.tabPageEX2.Controls.Add(this.checkBoxLinkCurves);
			this.tabPageEX2.Location = new System.Drawing.Point(4, 22);
			this.tabPageEX2.Margin = new System.Windows.Forms.Padding(6);
			this.tabPageEX2.Name = "tabPageEX2";
			this.tabPageEX2.Size = new System.Drawing.Size(594, 393);
			this.tabPageEX2.TabIndex = 1;
			this.tabPageEX2.Text = "Curves";
			this.tabPageEX2.UseVisualStyleBackColor = true;
			// 
			// tabPageEX3
			// 
			this.tabPageEX3.Controls.Add(this.listViewGradients);
			this.tabPageEX3.Controls.Add(this.toolStripGradients);
			this.tabPageEX3.Controls.Add(this.checkBoxLinkGradients);
			this.tabPageEX3.Location = new System.Drawing.Point(4, 22);
			this.tabPageEX3.Margin = new System.Windows.Forms.Padding(6);
			this.tabPageEX3.Name = "tabPageEX3";
			this.tabPageEX3.Size = new System.Drawing.Size(594, 393);
			this.tabPageEX3.TabIndex = 2;
			this.tabPageEX3.Text = "Gradients";
			// 
			// Form_ToolPalette
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(602, 419);
			this.ControlBox = false;
			this.Controls.Add(this.tabControlEX1);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form_ToolPalette";
			this.Text = "Preset Libraries";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolPalette_FormClosing);
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.toolStripColors.ResumeLayout(false);
			this.toolStripColors.PerformLayout();
			this.toolStripCurves.ResumeLayout(false);
			this.toolStripCurves.PerformLayout();
			this.toolStripGradients.ResumeLayout(false);
			this.toolStripGradients.PerformLayout();
			this.tabControlEX1.ResumeLayout(false);
			this.tabPageEX1.ResumeLayout(false);
			this.tabPageEX1.PerformLayout();
			this.tabPageEX2.ResumeLayout(false);
			this.tabPageEX2.PerformLayout();
			this.tabPageEX3.ResumeLayout(false);
			this.tabPageEX3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.CheckBox checkBoxLinkCurves;
		private System.Windows.Forms.ListView listViewCurves;
		private System.Windows.Forms.CheckBox checkBoxLinkGradients;
		private System.Windows.Forms.ListView listViewGradients;
		private System.Windows.Forms.ToolStrip toolStripColors;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditColor;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewColor;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteColor;
		private System.Windows.Forms.ToolStrip toolStripGradients;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditGradient;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewGradient;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteGradient;
		private System.Windows.Forms.ToolStrip toolStripCurves;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteCurve;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportCurves;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportCurves;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportGradients;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportGradients;
		private System.Windows.Forms.ToolStripButton toolStripButtonExportColors;
		private System.Windows.Forms.ToolStripButton toolStripButtonImportColors;
		private Dotnetrix.Controls.TabControlEX tabControlEX1;
		private Dotnetrix.Controls.TabPageEX tabPageEX1;
		private Dotnetrix.Controls.TabPageEX tabPageEX2;
		private Dotnetrix.Controls.TabPageEX tabPageEX3;


	}
}