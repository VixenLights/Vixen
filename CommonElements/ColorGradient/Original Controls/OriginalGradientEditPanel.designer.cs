namespace CommonElements.ColorManagement.Gradients
{
	partial class OriginalGradientEditPanel
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientEditPanel));
			this.cmbGammaCorrect = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.grpStops = new System.Windows.Forms.GroupBox();
			this.vColorLoc = new ControlsEx.ValueControls.ValueUpDown();
			this.vAlphaLoc = new ControlsEx.ValueControls.ValueUpDown();
			this.vOpacity = new ControlsEx.ValueControls.ValueUpDown();
			this.btnDeleteColor = new System.Windows.Forms.Button();
			this.btnDeleteAlpha = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblColorSelect = new CommonElements.ColorPicker.ColorLabel();
			this.edit = new GradientEdit();
			this.grpStops.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbGammaCorrect
			// 
			this.cmbGammaCorrect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cmbGammaCorrect, "cmbGammaCorrect");
			this.cmbGammaCorrect.FormattingEnabled = true;
			this.cmbGammaCorrect.Items.AddRange(new object[] {
            resources.GetString("cmbGammaCorrect.Items"),
            resources.GetString("cmbGammaCorrect.Items1")});
			this.cmbGammaCorrect.Name = "cmbGammaCorrect";
			this.cmbGammaCorrect.SelectedIndexChanged += new System.EventHandler(this.cmbGammaCorrect_SelectedIndexChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// grpStops
			// 
			resources.ApplyResources(this.grpStops, "grpStops");
			this.grpStops.Controls.Add(this.lblColorSelect);
			this.grpStops.Controls.Add(this.vColorLoc);
			this.grpStops.Controls.Add(this.vAlphaLoc);
			this.grpStops.Controls.Add(this.vOpacity);
			this.grpStops.Controls.Add(this.btnDeleteColor);
			this.grpStops.Controls.Add(this.btnDeleteAlpha);
			this.grpStops.Controls.Add(this.label5);
			this.grpStops.Controls.Add(this.label3);
			this.grpStops.Controls.Add(this.label4);
			this.grpStops.Controls.Add(this.label2);
			this.grpStops.Name = "grpStops";
			this.grpStops.TabStop = false;
			// 
			// vColorLoc
			// 
			resources.ApplyResources(this.vColorLoc, "vColorLoc");
			this.vColorLoc.Name = "vColorLoc";
			this.vColorLoc.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.vColorLoc.ValueChanged += new ControlsEx.ValueControls.ValueChangedEH(this.vColorLoc_ValueChanged);
			// 
			// vAlphaLoc
			// 
			resources.ApplyResources(this.vAlphaLoc, "vAlphaLoc");
			this.vAlphaLoc.Name = "vAlphaLoc";
			this.vAlphaLoc.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.vAlphaLoc.ValueChanged += new ControlsEx.ValueControls.ValueChangedEH(this.vAlphaLoc_ValueChanged);
			// 
			// vOpacity
			// 
			resources.ApplyResources(this.vOpacity, "vOpacity");
			this.vOpacity.Name = "vOpacity";
			this.vOpacity.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.vOpacity.ValueChanged += new ControlsEx.ValueControls.ValueChangedEH(this.vOpacity_ValueChanged);
			// 
			// btnDeleteColor
			// 
			resources.ApplyResources(this.btnDeleteColor, "btnDeleteColor");
			this.btnDeleteColor.Name = "btnDeleteColor";
			this.btnDeleteColor.UseVisualStyleBackColor = true;
			this.btnDeleteColor.Click += new System.EventHandler(this.btnDeleteColor_Click);
			// 
			// btnDeleteAlpha
			// 
			resources.ApplyResources(this.btnDeleteAlpha, "btnDeleteAlpha");
			this.btnDeleteAlpha.Name = "btnDeleteAlpha";
			this.btnDeleteAlpha.UseVisualStyleBackColor = true;
			this.btnDeleteAlpha.Click += new System.EventHandler(this.btnDeleteAlpha_Click);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// lblColorSelect
			// 
			resources.ApplyResources(this.lblColorSelect, "lblColorSelect");
			this.lblColorSelect.Name = "lblColorSelect";
			this.lblColorSelect.ColorChanged += new System.EventHandler(this.lblColorSelect_ColorChanged);
			this.lblColorSelect.Click += new System.EventHandler(this.lblColorSelect_Click);
			// 
			// edit
			// 
			resources.ApplyResources(this.edit, "edit");
			this.edit.Name = "edit";
			this.edit.SelectionChanged += new System.EventHandler(this.edit_GradientChanged);
			this.edit.GradientChanged += new System.EventHandler(this.edit_GradientChanged);
			// 
			// GradientEditPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cmbGammaCorrect);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.grpStops);
			this.Controls.Add(this.edit);
			this.MinimumSize = new System.Drawing.Size(357, 154);
			this.Name = "GradientEditPanel";
			this.grpStops.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbGammaCorrect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox grpStops;
		private CommonElements.ColorPicker.ColorLabel lblColorSelect;
		private ControlsEx.ValueControls.ValueUpDown vColorLoc;
		private ControlsEx.ValueControls.ValueUpDown vAlphaLoc;
		private ControlsEx.ValueControls.ValueUpDown vOpacity;
		private System.Windows.Forms.Button btnDeleteColor;
		private System.Windows.Forms.Button btnDeleteAlpha;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private GradientEdit edit;

	}
}
