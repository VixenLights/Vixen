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
			this.listViewColors = new System.Windows.Forms.ListView();
			this.buttonDeleteColor = new System.Windows.Forms.Button();
			this.buttonNewColor = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabColors = new System.Windows.Forms.TabPage();
			this.buttonEditColor = new System.Windows.Forms.Button();
			this.tabCurves = new System.Windows.Forms.TabPage();
			this.checkBoxLinkCurves = new System.Windows.Forms.CheckBox();
			this.buttonEditCurve = new System.Windows.Forms.Button();
			this.buttonDeleteCurve = new System.Windows.Forms.Button();
			this.buttonNewCurve = new System.Windows.Forms.Button();
			this.listViewCurves = new System.Windows.Forms.ListView();
			this.tabGradients = new System.Windows.Forms.TabPage();
			this.checkBoxLinkGradients = new System.Windows.Forms.CheckBox();
			this.comboBoxGradientHandling = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonEditGradient = new System.Windows.Forms.Button();
			this.buttonDeleteGradient = new System.Windows.Forms.Button();
			this.buttonNewGradient = new System.Windows.Forms.Button();
			this.listViewGradients = new System.Windows.Forms.ListView();
			this.tabHelp = new System.Windows.Forms.TabPage();
			this.linkToolPaletteDocumentation = new System.Windows.Forms.LinkLabel();
			this.labelHelp = new System.Windows.Forms.Label();
			this.tabEffects = new System.Windows.Forms.TabPage();
			this.listViewEffects = new System.Windows.Forms.ListView();
			this.tabControl1.SuspendLayout();
			this.tabColors.SuspendLayout();
			this.tabCurves.SuspendLayout();
			this.tabGradients.SuspendLayout();
			this.tabHelp.SuspendLayout();
			this.tabEffects.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewColors
			// 
			this.listViewColors.AllowDrop = true;
			this.listViewColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewColors.Location = new System.Drawing.Point(3, 3);
			this.listViewColors.MultiSelect = false;
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(870, 224);
			this.listViewColors.TabIndex = 1;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			this.listViewColors.DoubleClick += new System.EventHandler(this.listViewColors_DoubleClick);
			// 
			// buttonDeleteColor
			// 
			this.buttonDeleteColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDeleteColor.Enabled = false;
			this.buttonDeleteColor.Location = new System.Drawing.Point(782, 233);
			this.buttonDeleteColor.Name = "buttonDeleteColor";
			this.buttonDeleteColor.Size = new System.Drawing.Size(91, 23);
			this.buttonDeleteColor.TabIndex = 1;
			this.buttonDeleteColor.Text = "Delete Color";
			this.buttonDeleteColor.UseVisualStyleBackColor = true;
			this.buttonDeleteColor.Click += new System.EventHandler(this.buttonDeleteColor_Click);
			// 
			// buttonNewColor
			// 
			this.buttonNewColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewColor.Location = new System.Drawing.Point(3, 233);
			this.buttonNewColor.Name = "buttonNewColor";
			this.buttonNewColor.Size = new System.Drawing.Size(75, 23);
			this.buttonNewColor.TabIndex = 0;
			this.buttonNewColor.Text = "New Color";
			this.buttonNewColor.UseVisualStyleBackColor = true;
			this.buttonNewColor.Click += new System.EventHandler(this.buttonNewColor_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabColors);
			this.tabControl1.Controls.Add(this.tabCurves);
			this.tabControl1.Controls.Add(this.tabGradients);
			this.tabControl1.Controls.Add(this.tabEffects);
			this.tabControl1.Controls.Add(this.tabHelp);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(884, 288);
			this.tabControl1.TabIndex = 4;
			// 
			// tabColors
			// 
			this.tabColors.Controls.Add(this.buttonEditColor);
			this.tabColors.Controls.Add(this.buttonDeleteColor);
			this.tabColors.Controls.Add(this.listViewColors);
			this.tabColors.Controls.Add(this.buttonNewColor);
			this.tabColors.Location = new System.Drawing.Point(4, 4);
			this.tabColors.Name = "tabColors";
			this.tabColors.Padding = new System.Windows.Forms.Padding(3);
			this.tabColors.Size = new System.Drawing.Size(876, 262);
			this.tabColors.TabIndex = 0;
			this.tabColors.Text = "Colors";
			this.tabColors.UseVisualStyleBackColor = true;
			// 
			// buttonEditColor
			// 
			this.buttonEditColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditColor.Enabled = false;
			this.buttonEditColor.Location = new System.Drawing.Point(84, 233);
			this.buttonEditColor.Name = "buttonEditColor";
			this.buttonEditColor.Size = new System.Drawing.Size(75, 23);
			this.buttonEditColor.TabIndex = 2;
			this.buttonEditColor.Text = "Edit Color";
			this.buttonEditColor.UseVisualStyleBackColor = true;
			this.buttonEditColor.Click += new System.EventHandler(this.buttonEditColor_Click);
			// 
			// tabCurves
			// 
			this.tabCurves.Controls.Add(this.checkBoxLinkCurves);
			this.tabCurves.Controls.Add(this.buttonEditCurve);
			this.tabCurves.Controls.Add(this.buttonDeleteCurve);
			this.tabCurves.Controls.Add(this.buttonNewCurve);
			this.tabCurves.Controls.Add(this.listViewCurves);
			this.tabCurves.Location = new System.Drawing.Point(4, 4);
			this.tabCurves.Name = "tabCurves";
			this.tabCurves.Size = new System.Drawing.Size(876, 262);
			this.tabCurves.TabIndex = 2;
			this.tabCurves.Text = "Curves";
			this.tabCurves.UseVisualStyleBackColor = true;
			// 
			// checkBoxLinkCurves
			// 
			this.checkBoxLinkCurves.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxLinkCurves.AutoSize = true;
			this.checkBoxLinkCurves.Location = new System.Drawing.Point(165, 237);
			this.checkBoxLinkCurves.Name = "checkBoxLinkCurves";
			this.checkBoxLinkCurves.Size = new System.Drawing.Size(123, 17);
			this.checkBoxLinkCurves.TabIndex = 4;
			this.checkBoxLinkCurves.Text = "Maintain Library Link";
			this.checkBoxLinkCurves.UseVisualStyleBackColor = true;
			// 
			// buttonEditCurve
			// 
			this.buttonEditCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditCurve.Enabled = false;
			this.buttonEditCurve.Location = new System.Drawing.Point(84, 233);
			this.buttonEditCurve.Name = "buttonEditCurve";
			this.buttonEditCurve.Size = new System.Drawing.Size(75, 23);
			this.buttonEditCurve.TabIndex = 3;
			this.buttonEditCurve.Text = "Edit Curve";
			this.buttonEditCurve.UseVisualStyleBackColor = true;
			this.buttonEditCurve.Click += new System.EventHandler(this.buttonEditCurve_Click);
			// 
			// buttonDeleteCurve
			// 
			this.buttonDeleteCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDeleteCurve.Enabled = false;
			this.buttonDeleteCurve.Location = new System.Drawing.Point(782, 233);
			this.buttonDeleteCurve.Name = "buttonDeleteCurve";
			this.buttonDeleteCurve.Size = new System.Drawing.Size(91, 23);
			this.buttonDeleteCurve.TabIndex = 2;
			this.buttonDeleteCurve.Text = "Delete Curve";
			this.buttonDeleteCurve.UseVisualStyleBackColor = true;
			this.buttonDeleteCurve.Click += new System.EventHandler(this.buttonDeleteCurve_Click);
			// 
			// buttonNewCurve
			// 
			this.buttonNewCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewCurve.Location = new System.Drawing.Point(3, 233);
			this.buttonNewCurve.Name = "buttonNewCurve";
			this.buttonNewCurve.Size = new System.Drawing.Size(75, 23);
			this.buttonNewCurve.TabIndex = 1;
			this.buttonNewCurve.Text = "New Curve";
			this.buttonNewCurve.UseVisualStyleBackColor = true;
			this.buttonNewCurve.Click += new System.EventHandler(this.buttonNewCurve_Click);
			// 
			// listViewCurves
			// 
			this.listViewCurves.AllowDrop = true;
			this.listViewCurves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewCurves.Location = new System.Drawing.Point(3, 3);
			this.listViewCurves.Name = "listViewCurves";
			this.listViewCurves.Size = new System.Drawing.Size(870, 224);
			this.listViewCurves.TabIndex = 0;
			this.listViewCurves.UseCompatibleStateImageBehavior = false;
			this.listViewCurves.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewCurves_ItemDrag);
			this.listViewCurves.SelectedIndexChanged += new System.EventHandler(this.listViewCurves_SelectedIndexChanged);
			this.listViewCurves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCurves_MouseDoubleClick);
			// 
			// tabGradients
			// 
			this.tabGradients.Controls.Add(this.checkBoxLinkGradients);
			this.tabGradients.Controls.Add(this.comboBoxGradientHandling);
			this.tabGradients.Controls.Add(this.label1);
			this.tabGradients.Controls.Add(this.buttonEditGradient);
			this.tabGradients.Controls.Add(this.buttonDeleteGradient);
			this.tabGradients.Controls.Add(this.buttonNewGradient);
			this.tabGradients.Controls.Add(this.listViewGradients);
			this.tabGradients.Location = new System.Drawing.Point(4, 4);
			this.tabGradients.Name = "tabGradients";
			this.tabGradients.Padding = new System.Windows.Forms.Padding(3);
			this.tabGradients.Size = new System.Drawing.Size(876, 262);
			this.tabGradients.TabIndex = 1;
			this.tabGradients.Text = "Gradients";
			this.tabGradients.UseVisualStyleBackColor = true;
			// 
			// checkBoxLinkGradients
			// 
			this.checkBoxLinkGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxLinkGradients.AutoSize = true;
			this.checkBoxLinkGradients.Location = new System.Drawing.Point(191, 237);
			this.checkBoxLinkGradients.Name = "checkBoxLinkGradients";
			this.checkBoxLinkGradients.Size = new System.Drawing.Size(123, 17);
			this.checkBoxLinkGradients.TabIndex = 9;
			this.checkBoxLinkGradients.Text = "Maintain Library Link";
			this.checkBoxLinkGradients.UseVisualStyleBackColor = true;
			// 
			// comboBoxGradientHandling
			// 
			this.comboBoxGradientHandling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxGradientHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxGradientHandling.FormattingEnabled = true;
			this.comboBoxGradientHandling.Items.AddRange(new object[] {
            "The gradient is shown over the whole effect.",
            "Each pulse uses the entire gradient.",
            "The gradient is spread over the sub-elements."});
			this.comboBoxGradientHandling.Location = new System.Drawing.Point(418, 235);
			this.comboBoxGradientHandling.Name = "comboBoxGradientHandling";
			this.comboBoxGradientHandling.Size = new System.Drawing.Size(246, 21);
			this.comboBoxGradientHandling.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(320, 238);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Gradient Handling";
			// 
			// buttonEditGradient
			// 
			this.buttonEditGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditGradient.Enabled = false;
			this.buttonEditGradient.Location = new System.Drawing.Point(97, 233);
			this.buttonEditGradient.Name = "buttonEditGradient";
			this.buttonEditGradient.Size = new System.Drawing.Size(88, 23);
			this.buttonEditGradient.TabIndex = 6;
			this.buttonEditGradient.Text = "Edit Gradient";
			this.buttonEditGradient.UseVisualStyleBackColor = true;
			this.buttonEditGradient.Click += new System.EventHandler(this.buttonEditGradient_Click);
			// 
			// buttonDeleteGradient
			// 
			this.buttonDeleteGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDeleteGradient.Enabled = false;
			this.buttonDeleteGradient.Location = new System.Drawing.Point(780, 233);
			this.buttonDeleteGradient.Name = "buttonDeleteGradient";
			this.buttonDeleteGradient.Size = new System.Drawing.Size(93, 23);
			this.buttonDeleteGradient.TabIndex = 5;
			this.buttonDeleteGradient.Text = "Delete Gradient";
			this.buttonDeleteGradient.UseVisualStyleBackColor = true;
			this.buttonDeleteGradient.Click += new System.EventHandler(this.buttonDeleteGradient_Click);
			// 
			// buttonNewGradient
			// 
			this.buttonNewGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewGradient.Location = new System.Drawing.Point(3, 233);
			this.buttonNewGradient.Name = "buttonNewGradient";
			this.buttonNewGradient.Size = new System.Drawing.Size(88, 23);
			this.buttonNewGradient.TabIndex = 4;
			this.buttonNewGradient.Text = "New Gradient";
			this.buttonNewGradient.UseVisualStyleBackColor = true;
			this.buttonNewGradient.Click += new System.EventHandler(this.buttonNewGradient_Click);
			// 
			// listViewGradients
			// 
			this.listViewGradients.AllowDrop = true;
			this.listViewGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewGradients.Location = new System.Drawing.Point(3, 3);
			this.listViewGradients.Name = "listViewGradients";
			this.listViewGradients.Size = new System.Drawing.Size(870, 224);
			this.listViewGradients.TabIndex = 0;
			this.listViewGradients.UseCompatibleStateImageBehavior = false;
			this.listViewGradients.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewGradient_ItemDrag);
			this.listViewGradients.SelectedIndexChanged += new System.EventHandler(this.listViewGradients_SelectedIndexChanged);
			this.listViewGradients.DoubleClick += new System.EventHandler(this.listViewGradients_DoubleClick);
			// 
			// tabHelp
			// 
			this.tabHelp.Controls.Add(this.linkToolPaletteDocumentation);
			this.tabHelp.Controls.Add(this.labelHelp);
			this.tabHelp.Location = new System.Drawing.Point(4, 4);
			this.tabHelp.Name = "tabHelp";
			this.tabHelp.Size = new System.Drawing.Size(876, 262);
			this.tabHelp.TabIndex = 4;
			this.tabHelp.Text = "Help";
			this.tabHelp.UseVisualStyleBackColor = true;
			// 
			// linkToolPaletteDocumentation
			// 
			this.linkToolPaletteDocumentation.AutoSize = true;
			this.linkToolPaletteDocumentation.Location = new System.Drawing.Point(8, 5);
			this.linkToolPaletteDocumentation.Name = "linkToolPaletteDocumentation";
			this.linkToolPaletteDocumentation.Size = new System.Drawing.Size(167, 13);
			this.linkToolPaletteDocumentation.TabIndex = 1;
			this.linkToolPaletteDocumentation.TabStop = true;
			this.linkToolPaletteDocumentation.Text = "Tool Palette Documentation Page";
			this.linkToolPaletteDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkToolPaletteDocumentation_LinkClicked);
			// 
			// labelHelp
			// 
			this.labelHelp.AutoSize = true;
			this.labelHelp.Location = new System.Drawing.Point(8, 33);
			this.labelHelp.Name = "labelHelp";
			this.labelHelp.Size = new System.Drawing.Size(58, 13);
			this.labelHelp.TabIndex = 0;
			this.labelHelp.Text = "Help Label";
			// 
			// tabEffects
			// 
			this.tabEffects.Controls.Add(this.listViewEffects);
			this.tabEffects.Location = new System.Drawing.Point(4, 4);
			this.tabEffects.Name = "tabEffects";
			this.tabEffects.Size = new System.Drawing.Size(876, 262);
			this.tabEffects.TabIndex = 5;
			this.tabEffects.Text = "Effects";
			this.tabEffects.UseVisualStyleBackColor = true;
			// 
			// listViewEffects
			// 
			this.listViewEffects.AllowDrop = true;
			this.listViewEffects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewEffects.Location = new System.Drawing.Point(0, 0);
			this.listViewEffects.Name = "listViewEffects";
			this.listViewEffects.Size = new System.Drawing.Size(876, 262);
			this.listViewEffects.TabIndex = 0;
			this.listViewEffects.UseCompatibleStateImageBehavior = false;
			this.listViewEffects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewEffects_ItemDrag);
			// 
			// Form_ToolPalette
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 288);
			this.ControlBox = false;
			this.Controls.Add(this.tabControl1);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form_ToolPalette";
			this.Text = "Tool Palette";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolPalette_FormClosing);
			this.Load += new System.EventHandler(this.ColorPalette_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabColors.ResumeLayout(false);
			this.tabCurves.ResumeLayout(false);
			this.tabCurves.PerformLayout();
			this.tabGradients.ResumeLayout(false);
			this.tabGradients.PerformLayout();
			this.tabHelp.ResumeLayout(false);
			this.tabHelp.PerformLayout();
			this.tabEffects.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.Button buttonDeleteColor;
		private System.Windows.Forms.Button buttonNewColor;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabColors;
		private System.Windows.Forms.TabPage tabGradients;
		private System.Windows.Forms.TabPage tabCurves;
		private System.Windows.Forms.ListView listViewCurves;
		private System.Windows.Forms.ListView listViewGradients;
		private System.Windows.Forms.Button buttonEditCurve;
		private System.Windows.Forms.Button buttonDeleteCurve;
		private System.Windows.Forms.Button buttonNewCurve;
		private System.Windows.Forms.Button buttonEditGradient;
		private System.Windows.Forms.Button buttonDeleteGradient;
		private System.Windows.Forms.Button buttonNewGradient;
		private System.Windows.Forms.ComboBox comboBoxGradientHandling;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabHelp;
		private System.Windows.Forms.Label labelHelp;
		private System.Windows.Forms.LinkLabel linkToolPaletteDocumentation;
		private System.Windows.Forms.CheckBox checkBoxLinkCurves;
		private System.Windows.Forms.CheckBox checkBoxLinkGradients;
		private System.Windows.Forms.Button buttonEditColor;
		private System.Windows.Forms.TabPage tabEffects;
		private System.Windows.Forms.ListView listViewEffects;

	}
}