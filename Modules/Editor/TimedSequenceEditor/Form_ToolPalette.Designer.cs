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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabColors = new System.Windows.Forms.TabPage();
			this.toolStripColors = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteColor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportColors = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportColors = new System.Windows.Forms.ToolStripButton();
			this.listViewColors = new System.Windows.Forms.ListView();
			this.tabCurves = new System.Windows.Forms.TabPage();
			this.toolStripCurves = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportCurves = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportCurves = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkCurves = new System.Windows.Forms.CheckBox();
			this.listViewCurves = new System.Windows.Forms.ListView();
			this.tabGradients = new System.Windows.Forms.TabPage();
			this.toolStripGradients = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteGradient = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonExportGradients = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImportGradients = new System.Windows.Forms.ToolStripButton();
			this.checkBoxLinkGradients = new System.Windows.Forms.CheckBox();
			this.comboBoxGradientHandling = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listViewGradients = new System.Windows.Forms.ListView();
			this.tabControl1.SuspendLayout();
			this.tabColors.SuspendLayout();
			this.toolStripColors.SuspendLayout();
			this.tabCurves.SuspendLayout();
			this.toolStripCurves.SuspendLayout();
			this.tabGradients.SuspendLayout();
			this.toolStripGradients.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabColors);
			this.tabControl1.Controls.Add(this.tabCurves);
			this.tabControl1.Controls.Add(this.tabGradients);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(526, 390);
			this.tabControl1.TabIndex = 6;
			// 
			// tabColors
			// 
			this.tabColors.Controls.Add(this.toolStripColors);
			this.tabColors.Controls.Add(this.listViewColors);
			this.tabColors.Location = new System.Drawing.Point(4, 22);
			this.tabColors.Name = "tabColors";
			this.tabColors.Size = new System.Drawing.Size(518, 364);
			this.tabColors.TabIndex = 0;
			this.tabColors.Text = "Colors";
			this.tabColors.UseVisualStyleBackColor = true;
			// 
			// toolStripColors
			// 
			this.toolStripColors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditColor,
            this.toolStripButtonNewColor,
            this.toolStripButtonDeleteColor,
            this.toolStripButtonExportColors,
            this.toolStripButtonImportColors});
			this.toolStripColors.Location = new System.Drawing.Point(0, 0);
			this.toolStripColors.Name = "toolStripColors";
			this.toolStripColors.Size = new System.Drawing.Size(518, 25);
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
			this.listViewColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewColors.Location = new System.Drawing.Point(0, 28);
			this.listViewColors.MultiSelect = false;
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(518, 336);
			this.listViewColors.TabIndex = 1;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			this.listViewColors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewColors_MouseDoubleClick);
			// 
			// tabCurves
			// 
			this.tabCurves.Controls.Add(this.toolStripCurves);
			this.tabCurves.Controls.Add(this.checkBoxLinkCurves);
			this.tabCurves.Controls.Add(this.listViewCurves);
			this.tabCurves.Location = new System.Drawing.Point(4, 22);
			this.tabCurves.Name = "tabCurves";
			this.tabCurves.Size = new System.Drawing.Size(518, 364);
			this.tabCurves.TabIndex = 2;
			this.tabCurves.Text = "Curves";
			this.tabCurves.UseVisualStyleBackColor = true;
			// 
			// toolStripCurves
			// 
			this.toolStripCurves.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditCurve,
            this.toolStripButtonNewCurve,
            this.toolStripButtonDeleteCurve,
            this.toolStripButtonExportCurves,
            this.toolStripButtonImportCurves});
			this.toolStripCurves.Location = new System.Drawing.Point(0, 0);
			this.toolStripCurves.Name = "toolStripCurves";
			this.toolStripCurves.Size = new System.Drawing.Size(518, 25);
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
			this.checkBoxLinkCurves.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxLinkCurves.AutoSize = true;
			this.checkBoxLinkCurves.Location = new System.Drawing.Point(3, 344);
			this.checkBoxLinkCurves.Name = "checkBoxLinkCurves";
			this.checkBoxLinkCurves.Size = new System.Drawing.Size(123, 17);
			this.checkBoxLinkCurves.TabIndex = 4;
			this.checkBoxLinkCurves.Text = "Maintain Library Link";
			this.checkBoxLinkCurves.UseVisualStyleBackColor = true;
			// 
			// listViewCurves
			// 
			this.listViewCurves.AllowDrop = true;
			this.listViewCurves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewCurves.Location = new System.Drawing.Point(0, 28);
			this.listViewCurves.Name = "listViewCurves";
			this.listViewCurves.Size = new System.Drawing.Size(518, 310);
			this.listViewCurves.TabIndex = 0;
			this.listViewCurves.UseCompatibleStateImageBehavior = false;
			this.listViewCurves.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewCurves_ItemDrag);
			this.listViewCurves.SelectedIndexChanged += new System.EventHandler(this.listViewCurves_SelectedIndexChanged);
			this.listViewCurves.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragDrop);
			this.listViewCurves.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewCurves_DragEnter);
			this.listViewCurves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCurves_MouseDoubleClick);
			// 
			// tabGradients
			// 
			this.tabGradients.Controls.Add(this.toolStripGradients);
			this.tabGradients.Controls.Add(this.checkBoxLinkGradients);
			this.tabGradients.Controls.Add(this.comboBoxGradientHandling);
			this.tabGradients.Controls.Add(this.label1);
			this.tabGradients.Controls.Add(this.listViewGradients);
			this.tabGradients.Location = new System.Drawing.Point(4, 22);
			this.tabGradients.Name = "tabGradients";
			this.tabGradients.Size = new System.Drawing.Size(518, 364);
			this.tabGradients.TabIndex = 1;
			this.tabGradients.Text = "Gradients";
			this.tabGradients.UseVisualStyleBackColor = true;
			// 
			// toolStripGradients
			// 
			this.toolStripGradients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditGradient,
            this.toolStripButtonNewGradient,
            this.toolStripButtonDeleteGradient,
            this.toolStripButtonExportGradients,
            this.toolStripButtonImportGradients});
			this.toolStripGradients.Location = new System.Drawing.Point(0, 0);
			this.toolStripGradients.Name = "toolStripGradients";
			this.toolStripGradients.Size = new System.Drawing.Size(518, 25);
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
			this.checkBoxLinkGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxLinkGradients.AutoSize = true;
			this.checkBoxLinkGradients.Location = new System.Drawing.Point(3, 344);
			this.checkBoxLinkGradients.Name = "checkBoxLinkGradients";
			this.checkBoxLinkGradients.Size = new System.Drawing.Size(123, 17);
			this.checkBoxLinkGradients.TabIndex = 9;
			this.checkBoxLinkGradients.Text = "Maintain Library Link";
			this.checkBoxLinkGradients.UseVisualStyleBackColor = true;
			// 
			// comboBoxGradientHandling
			// 
			this.comboBoxGradientHandling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxGradientHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxGradientHandling.FormattingEnabled = true;
			this.comboBoxGradientHandling.Items.AddRange(new object[] {
            "The gradient is shown over the whole effect.",
            "Each pulse uses the entire gradient.",
            "The gradient is spread over the sub-elements."});
			this.comboBoxGradientHandling.Location = new System.Drawing.Point(3, 317);
			this.comboBoxGradientHandling.Name = "comboBoxGradientHandling";
			this.comboBoxGradientHandling.Size = new System.Drawing.Size(512, 21);
			this.comboBoxGradientHandling.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 301);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Gradient Handling";
			// 
			// listViewGradients
			// 
			this.listViewGradients.AllowDrop = true;
			this.listViewGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewGradients.Location = new System.Drawing.Point(0, 28);
			this.listViewGradients.Name = "listViewGradients";
			this.listViewGradients.Size = new System.Drawing.Size(518, 270);
			this.listViewGradients.TabIndex = 0;
			this.listViewGradients.UseCompatibleStateImageBehavior = false;
			this.listViewGradients.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewGradient_ItemDrag);
			this.listViewGradients.SelectedIndexChanged += new System.EventHandler(this.listViewGradients_SelectedIndexChanged);
			this.listViewGradients.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragDrop);
			this.listViewGradients.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewGradients_DragEnter);
			this.listViewGradients.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewGradients_MouseDoubleClick);
			// 
			// Form_ToolPalette
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(526, 390);
			this.ControlBox = false;
			this.Controls.Add(this.tabControl1);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form_ToolPalette";
			this.Text = "Preset Libraries";
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabColors.ResumeLayout(false);
			this.tabColors.PerformLayout();
			this.toolStripColors.ResumeLayout(false);
			this.toolStripColors.PerformLayout();
			this.tabCurves.ResumeLayout(false);
			this.tabCurves.PerformLayout();
			this.toolStripCurves.ResumeLayout(false);
			this.toolStripCurves.PerformLayout();
			this.tabGradients.ResumeLayout(false);
			this.tabGradients.PerformLayout();
			this.toolStripGradients.ResumeLayout(false);
			this.toolStripGradients.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabColors;
		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.TabPage tabCurves;
		private System.Windows.Forms.CheckBox checkBoxLinkCurves;
		private System.Windows.Forms.ListView listViewCurves;
		private System.Windows.Forms.TabPage tabGradients;
		private System.Windows.Forms.CheckBox checkBoxLinkGradients;
		private System.Windows.Forms.ComboBox comboBoxGradientHandling;
		private System.Windows.Forms.Label label1;
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


	}
}