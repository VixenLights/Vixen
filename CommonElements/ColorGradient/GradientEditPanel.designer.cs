namespace CommonElements.ColorManagement.Gradients
{
	partial class GradientEditPanel
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
			this.grpStops = new System.Windows.Forms.GroupBox();
			this.lblColorSelect = new CommonElements.ColorManagement.ColorPicker.ColorLabel();
			this.vColorLoc = new CommonElements.ControlsEx.ValueControls.ValueUpDown();
			this.btnDeleteColor = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.edit = new CommonElements.ColorManagement.Gradients.GradientEdit();
			this.buttonLoadPreset = new System.Windows.Forms.Button();
			this.buttonSaveNewPreset = new System.Windows.Forms.Button();
			this.grpStops.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpStops
			// 
			resources.ApplyResources(this.grpStops, "grpStops");
			this.grpStops.Controls.Add(this.lblColorSelect);
			this.grpStops.Controls.Add(this.vColorLoc);
			this.grpStops.Controls.Add(this.btnDeleteColor);
			this.grpStops.Controls.Add(this.label5);
			this.grpStops.Controls.Add(this.label4);
			this.grpStops.Name = "grpStops";
			this.grpStops.TabStop = false;
			// 
			// lblColorSelect
			// 
			resources.ApplyResources(this.lblColorSelect, "lblColorSelect");
			this.lblColorSelect.Name = "lblColorSelect";
			this.lblColorSelect.ColorChanged += new System.EventHandler(this.lblColorSelect_ColorChanged);
			this.lblColorSelect.Click += new System.EventHandler(this.lblColorSelect_Click);
			// 
			// vColorLoc
			// 
			resources.ApplyResources(this.vColorLoc, "vColorLoc");
			this.vColorLoc.Name = "vColorLoc";
			this.vColorLoc.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.vColorLoc.ValueChanged += new CommonElements.ControlsEx.ValueControls.ValueChangedEH(this.vColorLoc_ValueChanged);
			// 
			// btnDeleteColor
			// 
			resources.ApplyResources(this.btnDeleteColor, "btnDeleteColor");
			this.btnDeleteColor.Name = "btnDeleteColor";
			this.btnDeleteColor.UseVisualStyleBackColor = true;
			this.btnDeleteColor.Click += new System.EventHandler(this.btnDeleteColor_Click);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// edit
			// 
			resources.ApplyResources(this.edit, "edit");
			this.edit.Name = "edit";
			this.edit.SelectionChanged += new System.EventHandler(this.edit_GradientChanged);
			this.edit.GradientChanged += new System.EventHandler(this.edit_GradientChanged);
			// 
			// buttonLoadPreset
			// 
			resources.ApplyResources(this.buttonLoadPreset, "buttonLoadPreset");
			this.buttonLoadPreset.Name = "buttonLoadPreset";
			this.buttonLoadPreset.UseVisualStyleBackColor = true;
			this.buttonLoadPreset.Click += new System.EventHandler(this.buttonLoadPreset_Click);
			// 
			// buttonSaveNewPreset
			// 
			resources.ApplyResources(this.buttonSaveNewPreset, "buttonSaveNewPreset");
			this.buttonSaveNewPreset.Name = "buttonSaveNewPreset";
			this.buttonSaveNewPreset.UseVisualStyleBackColor = true;
			this.buttonSaveNewPreset.Click += new System.EventHandler(this.buttonSaveNewPreset_Click);
			// 
			// GradientEditPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonSaveNewPreset);
			this.Controls.Add(this.buttonLoadPreset);
			this.Controls.Add(this.grpStops);
			this.Controls.Add(this.edit);
			this.MinimumSize = new System.Drawing.Size(357, 120);
			this.Name = "GradientEditPanel";
			this.grpStops.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpStops;
		private CommonElements.ColorManagement.ColorPicker.ColorLabel lblColorSelect;
		private ControlsEx.ValueControls.ValueUpDown vColorLoc;
		private System.Windows.Forms.Button btnDeleteColor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private GradientEdit edit;
		private System.Windows.Forms.Button buttonLoadPreset;
		private System.Windows.Forms.Button buttonSaveNewPreset;

	}
}
