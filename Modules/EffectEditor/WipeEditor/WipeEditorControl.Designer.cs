namespace VixenModules.EffectEditor.WipeEditor
{
	partial class WipeEditorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			VixenModules.App.Curves.Curve curve1 = new VixenModules.App.Curves.Curve();
			VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
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
			this.radioWipeOut = new System.Windows.Forms.RadioButton();
			this.radioWipeIn = new System.Windows.Forms.RadioButton();
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
			this.groupBoxPulse.Location = new System.Drawing.Point(4, 5);
			this.groupBoxPulse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxPulse.Size = new System.Drawing.Size(402, 343);
			this.groupBoxPulse.TabIndex = 0;
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
			this.groupBox2.Location = new System.Drawing.Point(20, 146);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox2.Size = new System.Drawing.Size(363, 188);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Pulse Length";
			// 
			// radioNumPasses
			// 
			this.radioNumPasses.AutoSize = true;
			this.radioNumPasses.Location = new System.Drawing.Point(12, 86);
			this.radioNumPasses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioNumPasses.Name = "radioNumPasses";
			this.radioNumPasses.Size = new System.Drawing.Size(156, 24);
			this.radioNumPasses.TabIndex = 2;
			this.radioNumPasses.Text = "Number of Wipes";
			this.toolTipWipe.SetToolTip(this.radioNumPasses, "Number of wipes across the effect time.");
			this.radioNumPasses.UseVisualStyleBackColor = true;
			this.radioNumPasses.CheckedChanged += new System.EventHandler(this.radioNumPasses_CheckedChanged);
			// 
			// radioPulseLength
			// 
			this.radioPulseLength.AutoSize = true;
			this.radioPulseLength.Checked = true;
			this.radioPulseLength.Location = new System.Drawing.Point(12, 29);
			this.radioPulseLength.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioPulseLength.Name = "radioPulseLength";
			this.radioPulseLength.Size = new System.Drawing.Size(162, 24);
			this.radioPulseLength.TabIndex = 0;
			this.radioPulseLength.TabStop = true;
			this.radioPulseLength.Text = "Pulse Length (ms)";
			this.toolTipWipe.SetToolTip(this.radioPulseLength, "Specific pulse width. May result in partial wipe at end of the effect.");
			this.radioPulseLength.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 126);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 20);
			this.label2.TabIndex = 21;
			this.label2.Text = "Pulse Width Percent";
			this.toolTipWipe.SetToolTip(this.label2, "Pulse length as a percent of the wipe segment.");
			// 
			// numericUpDownPulseWidth
			// 
			this.numericUpDownPulseWidth.Enabled = false;
			this.numericUpDownPulseWidth.Location = new System.Drawing.Point(184, 123);
			this.numericUpDownPulseWidth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownPulseWidth.Name = "numericUpDownPulseWidth";
			this.numericUpDownPulseWidth.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownPulseWidth.TabIndex = 4;
			// 
			// numericUpDownNumPasses
			// 
			this.numericUpDownNumPasses.Enabled = false;
			this.numericUpDownNumPasses.Location = new System.Drawing.Point(184, 82);
			this.numericUpDownNumPasses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownNumPasses.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownNumPasses.Name = "numericUpDownNumPasses";
			this.numericUpDownNumPasses.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownNumPasses.TabIndex = 3;
			this.numericUpDownNumPasses.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			// 
			// numericUpDownPulseLength
			// 
			this.numericUpDownPulseLength.Increment = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.numericUpDownPulseLength.Location = new System.Drawing.Point(184, 25);
			this.numericUpDownPulseLength.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
			this.numericUpDownPulseLength.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownPulseLength.TabIndex = 1;
			this.numericUpDownPulseLength.Value = new decimal(new int[] {
			10,
			0,
			0,
			0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 69);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 20);
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
			this.curveTypeEditorControlEachPulse.Location = new System.Drawing.Point(168, 14);
			this.curveTypeEditorControlEachPulse.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.curveTypeEditorControlEachPulse.Name = "curveTypeEditorControlEachPulse";
			this.curveTypeEditorControlEachPulse.Size = new System.Drawing.Size(225, 123);
			this.curveTypeEditorControlEachPulse.TabIndex = 0;
			this.curveTypeEditorControlEachPulse.TargetEffect = null;
			// 
			// groupBoxColor
			// 
			this.groupBoxColor.Controls.Add(this.colorGradientTypeEditorControlGradient);
			this.groupBoxColor.Location = new System.Drawing.Point(4, 357);
			this.groupBoxColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxColor.Size = new System.Drawing.Size(402, 151);
			this.groupBoxColor.TabIndex = 1;
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
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(9, 29);
			this.colorGradientTypeEditorControlGradient.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.colorGradientTypeEditorControlGradient.Name = "colorGradientTypeEditorControlGradient";
			this.colorGradientTypeEditorControlGradient.Size = new System.Drawing.Size(374, 94);
			this.colorGradientTypeEditorControlGradient.TabIndex = 1;
			this.colorGradientTypeEditorControlGradient.TargetEffect = null;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioWipeOut);
			this.groupBox1.Controls.Add(this.radioWipeIn);
			this.groupBox1.Controls.Add(this.radioWipeLeft);
			this.groupBox1.Controls.Add(this.radioWipeRight);
			this.groupBox1.Controls.Add(this.radioWipeDown);
			this.groupBox1.Controls.Add(this.radioWipeUp);
			this.groupBox1.Location = new System.Drawing.Point(4, 517);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Size = new System.Drawing.Size(400, 131);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Wipe Direction";
			this.toolTipWipe.SetToolTip(this.groupBox1, "Direction of wipe across the elements configured locations.");
			// 
			// radioWipeOut
			// 
			this.radioWipeOut.AutoSize = true;
			this.radioWipeOut.Location = new System.Drawing.Point(210, 66);
			this.radioWipeOut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeOut.Name = "radioWipeOut";
			this.radioWipeOut.Size = new System.Drawing.Size(136, 24);
			this.radioWipeOut.TabIndex = 5;
			this.radioWipeOut.TabStop = true;
			this.radioWipeOut.Text = "Burst Outward";
			this.radioWipeOut.UseVisualStyleBackColor = true;
			// 
			// radioWipeIn
			// 
			this.radioWipeIn.AutoSize = true;
			this.radioWipeIn.Location = new System.Drawing.Point(18, 66);
			this.radioWipeIn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeIn.Name = "radioWipeIn";
			this.radioWipeIn.Size = new System.Drawing.Size(124, 24);
			this.radioWipeIn.TabIndex = 4;
			this.radioWipeIn.TabStop = true;
			this.radioWipeIn.Text = "Burst Inward";
			this.radioWipeIn.UseVisualStyleBackColor = true;
			// 
			// radioWipeLeft
			// 
			this.radioWipeLeft.AutoSize = true;
			this.radioWipeLeft.Location = new System.Drawing.Point(210, 29);
			this.radioWipeLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeLeft.Name = "radioWipeLeft";
			this.radioWipeLeft.Size = new System.Drawing.Size(62, 24);
			this.radioWipeLeft.TabIndex = 2;
			this.radioWipeLeft.TabStop = true;
			this.radioWipeLeft.Text = "Left";
			this.radioWipeLeft.UseVisualStyleBackColor = true;
			// 
			// radioWipeRight
			// 
			this.radioWipeRight.AutoSize = true;
			this.radioWipeRight.Location = new System.Drawing.Point(306, 29);
			this.radioWipeRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeRight.Name = "radioWipeRight";
			this.radioWipeRight.Size = new System.Drawing.Size(72, 24);
			this.radioWipeRight.TabIndex = 3;
			this.radioWipeRight.TabStop = true;
			this.radioWipeRight.Text = "Right";
			this.radioWipeRight.UseVisualStyleBackColor = true;
			// 
			// radioWipeDown
			// 
			this.radioWipeDown.AutoSize = true;
			this.radioWipeDown.Location = new System.Drawing.Point(99, 29);
			this.radioWipeDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeDown.Name = "radioWipeDown";
			this.radioWipeDown.Size = new System.Drawing.Size(75, 24);
			this.radioWipeDown.TabIndex = 1;
			this.radioWipeDown.TabStop = true;
			this.radioWipeDown.Text = "Down";
			this.radioWipeDown.UseVisualStyleBackColor = true;
			// 
			// radioWipeUp
			// 
			this.radioWipeUp.AutoSize = true;
			this.radioWipeUp.Location = new System.Drawing.Point(18, 31);
			this.radioWipeUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioWipeUp.Name = "radioWipeUp";
			this.radioWipeUp.Size = new System.Drawing.Size(55, 24);
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
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.groupBoxPulse);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "WipeEditorControl";
			this.Size = new System.Drawing.Size(416, 678);
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
		private System.Windows.Forms.RadioButton radioWipeOut;
		private System.Windows.Forms.RadioButton radioWipeIn;
	}
}
