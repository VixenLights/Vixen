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
			this.grpStops = new System.Windows.Forms.GroupBox();
			this.lblColorSelect = new Common.Controls.ColorManagement.ColorPicker.ColorLabel();
			this.vColorLoc = new Common.Controls.ControlsEx.ValueControls.ValueUpDown();
			this.btnDeleteColor = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.edit = new VixenModules.App.ColorGradients.GradientEdit();
			this.grpStops.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpStops
			// 
			this.grpStops.Controls.Add(this.flowLayoutPanel1);
			resources.ApplyResources(this.grpStops, "grpStops");
			this.grpStops.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpStops.Name = "grpStops";
			this.grpStops.TabStop = false;
			this.grpStops.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
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
			this.vColorLoc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.vColorLoc.Name = "vColorLoc";
			this.vColorLoc.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.vColorLoc.ValueChanged += new Common.Controls.ControlsEx.ValueControls.ValueChangedEH(this.vColorLoc_ValueChanged);
			// 
			// btnDeleteColor
			// 
			resources.ApplyResources(this.btnDeleteColor, "btnDeleteColor");
			this.btnDeleteColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.btnDeleteColor.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.btnDeleteColor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btnDeleteColor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btnDeleteColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnDeleteColor.Name = "btnDeleteColor";
			this.btnDeleteColor.UseVisualStyleBackColor = false;
			this.btnDeleteColor.Click += new System.EventHandler(this.btnDeleteColor_Click);
			this.btnDeleteColor.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnDeleteColor.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label5.Name = "label5";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label4.Name = "label4";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.label4);
			this.flowLayoutPanel1.Controls.Add(this.lblColorSelect);
			this.flowLayoutPanel1.Controls.Add(this.label5);
			this.flowLayoutPanel1.Controls.Add(this.vColorLoc);
			this.flowLayoutPanel1.Controls.Add(this.btnDeleteColor);
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// edit
			// 
			this.edit.DiscreteColors = false;
			resources.ApplyResources(this.edit, "edit");
			this.edit.Name = "edit";
			this.edit.ReadOnly = false;
			this.edit.ValidDiscreteColors = null;
			this.edit.SelectionChanged += new System.EventHandler(this.edit_GradientChanged);
			this.edit.GradientChanged += new System.EventHandler(this.edit_GradientChanged);
			this.edit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edit_KeyDown);
			// 
			// GradientEditPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.Controls.Add(this.edit);
			this.Controls.Add(this.grpStops);
			this.Name = "GradientEditPanel";
			this.grpStops.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

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
