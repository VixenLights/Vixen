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
			this.components = new System.ComponentModel.Container();
			VixenModules.App.Curves.Curve curve2 = new VixenModules.App.Curves.Curve();
			VixenModules.App.ColorGradients.ColorGradient colorGradient2 = new VixenModules.App.ColorGradients.ColorGradient();
			this.groupBoxPulse = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioNumPasses = new System.Windows.Forms.RadioButton();
			this.radioPulseLength = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownPulseWidth = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownNumPasses = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownPulseLength = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.curveTypeEditorControlEachPulse = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioWipeLeft = new System.Windows.Forms.RadioButton();
			this.radioWipeRight = new System.Windows.Forms.RadioButton();
			this.radioWipeDown = new System.Windows.Forms.RadioButton();
			this.radioWipeUp = new System.Windows.Forms.RadioButton();
			this.toolTipWipe = new System.Windows.Forms.ToolTip(this.components);
			this.groupBoxPulse.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumPasses)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseLength)).BeginInit();
			this.groupBoxColor.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxPulse
			// 
			this.groupBoxPulse.Controls.Add(this.groupBox2);
			this.groupBoxPulse.Controls.Add(this.label1);
			this.groupBoxPulse.Controls.Add(this.curveTypeEditorControlEachPulse);
			this.groupBoxPulse.Location = new System.Drawing.Point(3, 3);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Size = new System.Drawing.Size(268, 223);
			this.groupBoxPulse.TabIndex = 14;
			this.groupBoxPulse.TabStop = false;
			this.groupBoxPulse.Text = "Pulse";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioNumPasses);
			this.groupBox2.Controls.Add(this.radioPulseLength);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.numericUpDownPulseWidth);
			this.groupBox2.Controls.Add(this.numericUpDownNumPasses);
			this.groupBox2.Controls.Add(this.numericUpDownPulseLength);
			this.groupBox2.Location = new System.Drawing.Point(13, 95);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(242, 122);
			this.groupBox2.TabIndex = 17;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Pulse Length";
			// 
			// radioNumPasses
			// 
			this.radioNumPasses.AutoSize = true;
			this.radioNumPasses.Location = new System.Drawing.Point(8, 56);
			this.radioNumPasses.Name = "radioNumPasses";
			this.radioNumPasses.Size = new System.Drawing.Size(107, 17);
			this.radioNumPasses.TabIndex = 23;
			this.radioNumPasses.Text = "Number of Wipes";
			this.toolTipWipe.SetToolTip(this.radioNumPasses, "Number of wipes across the effect time.");
			this.radioNumPasses.UseVisualStyleBackColor = true;
			this.radioNumPasses.CheckedChanged += new System.EventHandler(this.radioNumPasses_CheckedChanged);
			// 
			// radioPulseLength
			// 
			this.radioPulseLength.AutoSize = true;
			this.radioPulseLength.Checked = true;
			this.radioPulseLength.Location = new System.Drawing.Point(8, 19);
			this.radioPulseLength.Name = "radioPulseLength";
			this.radioPulseLength.Size = new System.Drawing.Size(109, 17);
			this.radioPulseLength.TabIndex = 22;
			this.radioPulseLength.TabStop = true;
			this.radioPulseLength.Text = "Pulse Length (ms)";
			this.toolTipWipe.SetToolTip(this.radioPulseLength, "Specific pulse width. May result in partial wipe at end of the effect.");
			this.radioPulseLength.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 13);
			this.label2.TabIndex = 21;
			this.label2.Text = "Pulse Width Percent";
			this.toolTipWipe.SetToolTip(this.label2, "Pulse length as a percent of the wipe segment.");
			// 
			// numericUpDownPulseWidth
			// 
			this.numericUpDownPulseWidth.Enabled = false;
			this.numericUpDownPulseWidth.Location = new System.Drawing.Point(123, 80);
			this.numericUpDownPulseWidth.Name = "numericUpDownPulseWidth";
			this.numericUpDownPulseWidth.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownPulseWidth.TabIndex = 20;
			// 
			// numericUpDownNumPasses
			// 
			this.numericUpDownNumPasses.Enabled = false;
			this.numericUpDownNumPasses.Location = new System.Drawing.Point(123, 53);
			this.numericUpDownNumPasses.Name = "numericUpDownNumPasses";
			this.numericUpDownNumPasses.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownNumPasses.TabIndex = 19;
			// 
			// numericUpDownPulseLength
			// 
			this.numericUpDownPulseLength.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDownPulseLength.Location = new System.Drawing.Point(123, 16);
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "Individual Pulses:";
			// 
			// curveTypeEditorControlEachPulse
			// 
			curve2.IsCurrentLibraryCurve = false;
			curve2.LibraryReferenceName = "";
			this.curveTypeEditorControlEachPulse.CurveValue = curve2;
			this.curveTypeEditorControlEachPulse.EffectParameterValues = new object[] {
        ((object)(curve2))};
			this.curveTypeEditorControlEachPulse.Location = new System.Drawing.Point(112, 9);
			this.curveTypeEditorControlEachPulse.Name = "curveTypeEditorControlEachPulse";
			this.curveTypeEditorControlEachPulse.Size = new System.Drawing.Size(150, 80);
			this.curveTypeEditorControlEachPulse.TabIndex = 14;
			this.curveTypeEditorControlEachPulse.TargetEffect = null;
			// 
			// groupBoxColor
			// 
			this.groupBoxColor.Controls.Add(this.colorGradientTypeEditorControlGradient);
			this.groupBoxColor.Location = new System.Drawing.Point(3, 232);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Size = new System.Drawing.Size(268, 98);
			this.groupBoxColor.TabIndex = 16;
			this.groupBoxColor.TabStop = false;
			this.groupBoxColor.Text = "Color Handling";
			// 
			// colorGradientTypeEditorControlGradient
			// 
			colorGradient2.Gammacorrected = false;
			colorGradient2.IsCurrentLibraryGradient = false;
			colorGradient2.LibraryReferenceName = "";
			colorGradient2.Title = null;
			this.colorGradientTypeEditorControlGradient.ColorGradientValue = colorGradient2;
			this.colorGradientTypeEditorControlGradient.EffectParameterValues = new object[] {
        ((object)(colorGradient2))};
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(6, 19);
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
			this.groupBox1.Location = new System.Drawing.Point(3, 336);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(267, 44);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Wipe Direction";
			this.toolTipWipe.SetToolTip(this.groupBox1, "Direction of wipe across the elements configured locations.");
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
			// toolTipWipe
			// 
			this.toolTipWipe.ToolTipTitle = "Wipe Configuration";
			// 
			// WipeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.groupBoxPulse);
			this.Name = "WipeEditorControl";
			this.Size = new System.Drawing.Size(277, 394);
			this.groupBoxPulse.ResumeLayout(false);
			this.groupBoxPulse.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumPasses)).EndInit();
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
		private System.Windows.Forms.NumericUpDown numericUpDownPulseLength;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioWipeLeft;
		private System.Windows.Forms.RadioButton radioWipeRight;
		private System.Windows.Forms.RadioButton radioWipeDown;
		private System.Windows.Forms.RadioButton radioWipeUp;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioNumPasses;
		private System.Windows.Forms.ToolTip toolTipWipe;
		private System.Windows.Forms.RadioButton radioPulseLength;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownPulseWidth;
		private System.Windows.Forms.NumericUpDown numericUpDownNumPasses;
	}
}
