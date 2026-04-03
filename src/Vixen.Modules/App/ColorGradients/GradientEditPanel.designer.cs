namespace VixenModules.App.ColorGradients
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
			grpStops = new GroupBox();
			flowLayoutPanel1 = new FlowLayoutPanel();
			label4 = new Label();
			lblColorSelect = new Common.Controls.ColorManagement.ColorPicker.ColorLabel();
			label5 = new Label();
			vColorLoc = new Common.Controls.ControlsEx.ValueControls.ValueUpDown();
			btnDeleteColor = new Button();
			edit = new GradientEdit();
			grpStops.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// grpStops
			// 
			grpStops.Controls.Add(flowLayoutPanel1);
			resources.ApplyResources(grpStops, "grpStops");
			grpStops.Name = "grpStops";
			grpStops.TabStop = false;
			grpStops.Paint += groupBoxes_Paint;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Controls.Add(label4);
			flowLayoutPanel1.Controls.Add(lblColorSelect);
			flowLayoutPanel1.Controls.Add(label5);
			flowLayoutPanel1.Controls.Add(vColorLoc);
			flowLayoutPanel1.Controls.Add(btnDeleteColor);
			resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// label4
			// 
			resources.ApplyResources(label4, "label4");
			label4.Name = "label4";
			// 
			// lblColorSelect
			// 
			resources.ApplyResources(lblColorSelect, "lblColorSelect");
			lblColorSelect.Name = "lblColorSelect";
			lblColorSelect.ColorChanged += lblColorSelect_ColorChanged;
			lblColorSelect.Click += lblColorSelect_Click;
			// 
			// label5
			// 
			resources.ApplyResources(label5, "label5");
			label5.Name = "label5";
			// 
			// vColorLoc
			// 
			resources.ApplyResources(vColorLoc, "vColorLoc");
			vColorLoc.Name = "vColorLoc";
			vColorLoc.TrackerOrientation = Orientation.Vertical;
			vColorLoc.ValueChanged += vColorLoc_ValueChanged;
			// 
			// btnDeleteColor
			// 
			resources.ApplyResources(btnDeleteColor, "btnDeleteColor");
			btnDeleteColor.Name = "btnDeleteColor";
			btnDeleteColor.UseVisualStyleBackColor = false;
			btnDeleteColor.Click += btnDeleteColor_Click;
			// 
			// edit
			// 
			resources.ApplyResources(edit, "edit");
			edit.Name = "edit";
			edit.SelectionChanged += edit_GradientChanged;
			edit.GradientChanged += edit_GradientChanged;
			edit.KeyDown += edit_KeyDown;
			// 
			// GradientEditPanel
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(edit);
			Controls.Add(grpStops);
			Name = "GradientEditPanel";
			grpStops.ResumeLayout(false);
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpStops;
		private Common.Controls.ColorManagement.ColorPicker.ColorLabel lblColorSelect;
		private Common.Controls.ControlsEx.ValueControls.ValueUpDown vColorLoc;
		private System.Windows.Forms.Button btnDeleteColor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private GradientEdit edit;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
