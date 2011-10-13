namespace VixenModules.EffectEditor.PulseEffectEditor
{
	partial class PulseEffectEditorControl
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
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.colorGradientTypeEditorControl = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.curveTypeEditorControl = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.SuspendLayout();
			// 
			// colorGradientTypeEditorControl
			// 
			this.colorGradientTypeEditorControl.ColorGradientValue = null;
			this.colorGradientTypeEditorControl.EffectParameterValues = new object[] {
        null};
			this.colorGradientTypeEditorControl.Location = new System.Drawing.Point(79, 3);
			this.colorGradientTypeEditorControl.Name = "colorGradientTypeEditorControl";
			this.colorGradientTypeEditorControl.Size = new System.Drawing.Size(70, 70);
			this.colorGradientTypeEditorControl.TabIndex = 0;
			// 
			// curveTypeEditorControl
			// 
			this.curveTypeEditorControl.CurveValue = null;
			this.curveTypeEditorControl.EffectParameterValues = new object[] {
        null};
			this.curveTypeEditorControl.Location = new System.Drawing.Point(3, 3);
			this.curveTypeEditorControl.Name = "curveTypeEditorControl";
			this.curveTypeEditorControl.Size = new System.Drawing.Size(70, 70);
			this.curveTypeEditorControl.TabIndex = 1;
			// 
			// PulseEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.curveTypeEditorControl);
			this.Controls.Add(this.colorGradientTypeEditorControl);
			this.Name = "PulseEffectEditorControl";
			this.Size = new System.Drawing.Size(152, 76);
			this.ResumeLayout(false);

		}

		#endregion

		private ColorGradientTypeEditor.ColorGradientTypeEditorControl colorGradientTypeEditorControl;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControl;
	}
}
