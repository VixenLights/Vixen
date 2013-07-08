namespace VixenModules.EffectEditor.WipeEditor {
	partial class WipeEditorControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			VixenModules.App.Curves.Curve curve1 = new VixenModules.App.Curves.Curve();
			VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
			this.groupBoxPulse = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.curveTypeEditorControlEachPulse = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.numericUpDownPulseLength = new System.Windows.Forms.NumericUpDown();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioWipeLeft = new System.Windows.Forms.RadioButton();
			this.radioWipeRight = new System.Windows.Forms.RadioButton();
			this.radioWipeDown = new System.Windows.Forms.RadioButton();
			this.radioWipeUp = new System.Windows.Forms.RadioButton();
			this.groupBoxPulse.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseLength)).BeginInit();
			this.groupBoxColor.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxPulse
			// 
			this.groupBoxPulse.Controls.Add(this.label3);
			this.groupBoxPulse.Controls.Add(this.label1);
			this.groupBoxPulse.Controls.Add(this.curveTypeEditorControlEachPulse);
			this.groupBoxPulse.Controls.Add(this.numericUpDownPulseLength);
			this.groupBoxPulse.Location = new System.Drawing.Point(3, 3);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Size = new System.Drawing.Size(268, 154);
			this.groupBoxPulse.TabIndex = 14;
			this.groupBoxPulse.TabStop = false;
			this.groupBoxPulse.Text = "Pulse";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 124);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Pulse Length (ms)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "Individual Pulses:";
			// 
			// curveTypeEditorControlEachPulse
			// 
			curve1.IsCurrentLibraryCurve = false;
			curve1.LibraryReferenceName = "";
			this.curveTypeEditorControlEachPulse.CurveValue = curve1;
			this.curveTypeEditorControlEachPulse.EffectParameterValues = new object[] {
        ((object)(curve1))};
			this.curveTypeEditorControlEachPulse.Location = new System.Drawing.Point(105, 23);
			this.curveTypeEditorControlEachPulse.Name = "curveTypeEditorControlEachPulse";
			this.curveTypeEditorControlEachPulse.Size = new System.Drawing.Size(150, 80);
			this.curveTypeEditorControlEachPulse.TabIndex = 14;
			this.curveTypeEditorControlEachPulse.TargetEffect = null;
			// 
			// numericUpDownPulseLength
			// 
			this.numericUpDownPulseLength.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDownPulseLength.Location = new System.Drawing.Point(162, 122);
			this.numericUpDownPulseLength.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.numericUpDownPulseLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownPulseLength.Name = "numericUpDownPulseLength";
			this.numericUpDownPulseLength.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownPulseLength.TabIndex = 12;
			this.numericUpDownPulseLength.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// groupBoxColor
			// 
			this.groupBoxColor.Controls.Add(this.colorGradientTypeEditorControlGradient);
			this.groupBoxColor.Location = new System.Drawing.Point(3, 163);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Size = new System.Drawing.Size(268, 98);
			this.groupBoxColor.TabIndex = 16;
			this.groupBoxColor.TabStop = false;
			this.groupBoxColor.Text = "Color Handling";
			// 
			// colorGradientTypeEditorControlGradient
			// 
			colorGradient1.Gammacorrected = false;
			colorGradient1.IsCurrentLibraryGradient = false;
			colorGradient1.LibraryReferenceName = "";
			colorGradient1.Title = null;
			this.colorGradientTypeEditorControlGradient.ColorGradientValue = colorGradient1;
			this.colorGradientTypeEditorControlGradient.EffectParameterValues = new object[] {
        ((object)(colorGradient1))};
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(6, 23);
			this.colorGradientTypeEditorControlGradient.Name = "colorGradientTypeEditorControlGradient";
			this.colorGradientTypeEditorControlGradient.Size = new System.Drawing.Size(249, 61);
			this.colorGradientTypeEditorControlGradient.TabIndex = 1;
			this.colorGradientTypeEditorControlGradient.TargetEffect = null;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioWipeLeft);
			this.groupBox1.Controls.Add(this.radioWipeRight);
			this.groupBox1.Controls.Add(this.radioWipeDown);
			this.groupBox1.Controls.Add(this.radioWipeUp);
			this.groupBox1.Location = new System.Drawing.Point(3, 267);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(267, 44);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Wipe Direction";
			// 
			// radioWipeLeft
			// 
			this.radioWipeLeft.AutoSize = true;
			this.radioWipeLeft.Location = new System.Drawing.Point(140, 19);
			this.radioWipeLeft.Name = "radioWipeLeft";
			this.radioWipeLeft.Size = new System.Drawing.Size(43, 17);
			this.radioWipeLeft.TabIndex = 3;
			this.radioWipeLeft.TabStop = true;
			this.radioWipeLeft.Text = "Left";
			this.radioWipeLeft.UseVisualStyleBackColor = true;
			// 
			// radioWipeRight
			// 
			this.radioWipeRight.AutoSize = true;
			this.radioWipeRight.Location = new System.Drawing.Point(204, 19);
			this.radioWipeRight.Name = "radioWipeRight";
			this.radioWipeRight.Size = new System.Drawing.Size(50, 17);
			this.radioWipeRight.TabIndex = 2;
			this.radioWipeRight.TabStop = true;
			this.radioWipeRight.Text = "Right";
			this.radioWipeRight.UseVisualStyleBackColor = true;
			// 
			// radioWipeDown
			// 
			this.radioWipeDown.AutoSize = true;
			this.radioWipeDown.Location = new System.Drawing.Point(66, 19);
			this.radioWipeDown.Name = "radioWipeDown";
			this.radioWipeDown.Size = new System.Drawing.Size(53, 17);
			this.radioWipeDown.TabIndex = 1;
			this.radioWipeDown.TabStop = true;
			this.radioWipeDown.Text = "Down";
			this.radioWipeDown.UseVisualStyleBackColor = true;
			// 
			// radioWipeUp
			// 
			this.radioWipeUp.AutoSize = true;
			this.radioWipeUp.Location = new System.Drawing.Point(12, 20);
			this.radioWipeUp.Name = "radioWipeUp";
			this.radioWipeUp.Size = new System.Drawing.Size(39, 17);
			this.radioWipeUp.TabIndex = 0;
			this.radioWipeUp.TabStop = true;
			this.radioWipeUp.Text = "Up";
			this.radioWipeUp.UseVisualStyleBackColor = true;
			// 
			// WipeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.groupBoxPulse);
			this.Name = "WipeEditorControl";
			this.Size = new System.Drawing.Size(277, 319);
			this.groupBoxPulse.ResumeLayout(false);
			this.groupBoxPulse.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseLength)).EndInit();
			this.groupBoxColor.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxPulse;
		private System.Windows.Forms.Label label1;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControlEachPulse;
		private System.Windows.Forms.GroupBox groupBoxColor;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl colorGradientTypeEditorControlGradient;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownPulseLength;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioWipeLeft;
		private System.Windows.Forms.RadioButton radioWipeRight;
		private System.Windows.Forms.RadioButton radioWipeDown;
		private System.Windows.Forms.RadioButton radioWipeUp;
	}
}
