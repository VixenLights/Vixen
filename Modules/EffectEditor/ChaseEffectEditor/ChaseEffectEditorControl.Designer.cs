namespace VixenModules.EffectEditor.ChaseEffectEditor
{
	partial class ChaseEffectEditorControl
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
			this.components = new System.ComponentModel.Container();
			VixenModules.App.Curves.Curve curve1 = new VixenModules.App.Curves.Curve();
			VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
			VixenModules.App.Curves.Curve curve2 = new VixenModules.App.Curves.Curve();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownPulseTimeOverlap = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBoxPulse = new System.Windows.Forms.GroupBox();
			this.chkExtendPulseToStart = new System.Windows.Forms.CheckBox();
			this.chkExtendPulseToEnd = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.curveTypeEditorControlEachPulse = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.radioButtonGradientAcrossItems = new System.Windows.Forms.RadioButton();
			this.radioButtonStaticColor = new System.Windows.Forms.RadioButton();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.colorTypeEditorControlStaticColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.label7 = new System.Windows.Forms.Label();
			this.radioButtonGradientIndividual = new System.Windows.Forms.RadioButton();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.radioButtonGradientOverWhole = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioButtonApplyToAllElements = new System.Windows.Forms.RadioButton();
			this.radioButtonApplyToLevel = new System.Windows.Forms.RadioButton();
			this.groupByDepthOfEffect = new System.Windows.Forms.GroupBox();
			this.numericUpDownDepthOfEffect = new System.Windows.Forms.NumericUpDown();
			this.curveTypeEditorControlChaseMovement = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.levelTypeEditorControlDefaultLevel = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeOverlap)).BeginInit();
			this.groupBoxPulse.SuspendLayout();
			this.groupBoxColor.SuspendLayout();
			this.groupByDepthOfEffect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).BeginInit();
			this.SuspendLayout();
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(10, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 13);
			this.label4.TabIndex = 20;
			this.label4.Text = "Chase movement:";
			// 
			// numericUpDownPulseTimeOverlap
			// 
			this.numericUpDownPulseTimeOverlap.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDownPulseTimeOverlap.Location = new System.Drawing.Point(162, 122);
			this.numericUpDownPulseTimeOverlap.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.numericUpDownPulseTimeOverlap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownPulseTimeOverlap.Name = "numericUpDownPulseTimeOverlap";
			this.numericUpDownPulseTimeOverlap.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownPulseTimeOverlap.TabIndex = 12;
			this.numericUpDownPulseTimeOverlap.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(301, 221);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 13);
			this.label2.TabIndex = 18;
			this.label2.Text = "Default level:";
			// 
			// groupBoxPulse
			// 
			this.groupBoxPulse.Controls.Add(this.chkExtendPulseToStart);
			this.groupBoxPulse.Controls.Add(this.chkExtendPulseToEnd);
			this.groupBoxPulse.Controls.Add(this.label3);
			this.groupBoxPulse.Controls.Add(this.label1);
			this.groupBoxPulse.Controls.Add(this.curveTypeEditorControlEachPulse);
			this.groupBoxPulse.Controls.Add(this.numericUpDownPulseTimeOverlap);
			this.groupBoxPulse.Location = new System.Drawing.Point(3, 89);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Size = new System.Drawing.Size(268, 193);
			this.groupBoxPulse.TabIndex = 13;
			this.groupBoxPulse.TabStop = false;
			this.groupBoxPulse.Text = "Pulse";
			// 
			// chkExtendPulseToStart
			// 
			this.chkExtendPulseToStart.AutoSize = true;
			this.chkExtendPulseToStart.Location = new System.Drawing.Point(13, 170);
			this.chkExtendPulseToStart.Name = "chkExtendPulseToStart";
			this.chkExtendPulseToStart.Size = new System.Drawing.Size(92, 17);
			this.chkExtendPulseToStart.TabIndex = 18;
			this.chkExtendPulseToStart.Text = "Pulse Start Fill";
			this.toolTip1.SetToolTip(this.chkExtendPulseToStart, "This allows the beginning value of each pulse to be extended to the start of the " +
        "effect.");
			this.chkExtendPulseToStart.UseVisualStyleBackColor = true;
			// 
			// chkExtendPulseToEnd
			// 
			this.chkExtendPulseToEnd.AutoSize = true;
			this.chkExtendPulseToEnd.Location = new System.Drawing.Point(13, 147);
			this.chkExtendPulseToEnd.Name = "chkExtendPulseToEnd";
			this.chkExtendPulseToEnd.Size = new System.Drawing.Size(89, 17);
			this.chkExtendPulseToEnd.TabIndex = 17;
			this.chkExtendPulseToEnd.Text = "Pulse End Fill";
			this.toolTip1.SetToolTip(this.chkExtendPulseToEnd, "This allows the last value of each pulse to be extended to the end of the effect." +
        "");
			this.chkExtendPulseToEnd.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 124);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(146, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Overlap between pulses (ms):";
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
			// radioButtonGradientAcrossItems
			// 
			this.radioButtonGradientAcrossItems.AutoSize = true;
			this.radioButtonGradientAcrossItems.Location = new System.Drawing.Point(6, 126);
			this.radioButtonGradientAcrossItems.Name = "radioButtonGradientAcrossItems";
			this.radioButtonGradientAcrossItems.Size = new System.Drawing.Size(282, 17);
			this.radioButtonGradientAcrossItems.TabIndex = 9;
			this.radioButtonGradientAcrossItems.TabStop = true;
			this.radioButtonGradientAcrossItems.Text = "The gradient is spread over the range of sub-elements.";
			this.radioButtonGradientAcrossItems.UseVisualStyleBackColor = true;
			// 
			// radioButtonStaticColor
			// 
			this.radioButtonStaticColor.AutoSize = true;
			this.radioButtonStaticColor.Location = new System.Drawing.Point(6, 32);
			this.radioButtonStaticColor.Name = "radioButtonStaticColor";
			this.radioButtonStaticColor.Size = new System.Drawing.Size(82, 17);
			this.radioButtonStaticColor.TabIndex = 7;
			this.radioButtonStaticColor.TabStop = true;
			this.radioButtonStaticColor.Text = "Static Color:";
			this.radioButtonStaticColor.UseVisualStyleBackColor = true;
			// 
			// groupBoxColor
			// 
			this.groupBoxColor.Controls.Add(this.radioButtonGradientAcrossItems);
			this.groupBoxColor.Controls.Add(this.colorTypeEditorControlStaticColor);
			this.groupBoxColor.Controls.Add(this.radioButtonStaticColor);
			this.groupBoxColor.Controls.Add(this.label7);
			this.groupBoxColor.Controls.Add(this.radioButtonGradientIndividual);
			this.groupBoxColor.Controls.Add(this.colorGradientTypeEditorControlGradient);
			this.groupBoxColor.Controls.Add(this.radioButtonGradientOverWhole);
			this.groupBoxColor.Location = new System.Drawing.Point(277, 3);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Size = new System.Drawing.Size(295, 205);
			this.groupBoxColor.TabIndex = 15;
			this.groupBoxColor.TabStop = false;
			this.groupBoxColor.Text = "Color Handling";
			// 
			// colorTypeEditorControlStaticColor
			// 
			this.colorTypeEditorControlStaticColor.ColorValue = System.Drawing.Color.Empty;
			this.colorTypeEditorControlStaticColor.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
			this.colorTypeEditorControlStaticColor.Location = new System.Drawing.Point(94, 20);
			this.colorTypeEditorControlStaticColor.Name = "colorTypeEditorControlStaticColor";
			this.colorTypeEditorControlStaticColor.Size = new System.Drawing.Size(40, 40);
			this.colorTypeEditorControlStaticColor.TabIndex = 8;
			this.colorTypeEditorControlStaticColor.TargetEffect = null;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(38, 166);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Gradient:";
			// 
			// radioButtonGradientIndividual
			// 
			this.radioButtonGradientIndividual.AutoSize = true;
			this.radioButtonGradientIndividual.Location = new System.Drawing.Point(6, 103);
			this.radioButtonGradientIndividual.Name = "radioButtonGradientIndividual";
			this.radioButtonGradientIndividual.Size = new System.Drawing.Size(241, 17);
			this.radioButtonGradientIndividual.TabIndex = 2;
			this.radioButtonGradientIndividual.TabStop = true;
			this.radioButtonGradientIndividual.Text = "Each individual pulse uses the entire gradient.";
			this.radioButtonGradientIndividual.UseVisualStyleBackColor = true;
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
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(94, 152);
			this.colorGradientTypeEditorControlGradient.Name = "colorGradientTypeEditorControlGradient";
			this.colorGradientTypeEditorControlGradient.Size = new System.Drawing.Size(100, 40);
			this.colorGradientTypeEditorControlGradient.TabIndex = 1;
			this.colorGradientTypeEditorControlGradient.TargetEffect = null;
			// 
			// radioButtonGradientOverWhole
			// 
			this.radioButtonGradientOverWhole.AutoSize = true;
			this.radioButtonGradientOverWhole.Location = new System.Drawing.Point(6, 67);
			this.radioButtonGradientOverWhole.Name = "radioButtonGradientOverWhole";
			this.radioButtonGradientOverWhole.Size = new System.Drawing.Size(263, 30);
			this.radioButtonGradientOverWhole.TabIndex = 0;
			this.radioButtonGradientOverWhole.TabStop = true;
			this.radioButtonGradientOverWhole.Text = "The gradient is shown over the whole effect.\r\nAll elements display the same color" +
    " at a given time.";
			this.radioButtonGradientOverWhole.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(286, 236);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(108, 13);
			this.label5.TabIndex = 21;
			this.label5.Text = "(non-active elements)";
			// 
			// radioButtonApplyToAllElements
			// 
			this.radioButtonApplyToAllElements.AutoSize = true;
			this.radioButtonApplyToAllElements.Location = new System.Drawing.Point(6, 21);
			this.radioButtonApplyToAllElements.Name = "radioButtonApplyToAllElements";
			this.radioButtonApplyToAllElements.Size = new System.Drawing.Size(82, 17);
			this.radioButtonApplyToAllElements.TabIndex = 24;
			this.radioButtonApplyToAllElements.TabStop = true;
			this.radioButtonApplyToAllElements.Text = "All Elements";
			this.radioButtonApplyToAllElements.UseVisualStyleBackColor = true;
			this.radioButtonApplyToAllElements.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
			// 
			// radioButtonApplyToLevel
			// 
			this.radioButtonApplyToLevel.AutoSize = true;
			this.radioButtonApplyToLevel.Location = new System.Drawing.Point(6, 43);
			this.radioButtonApplyToLevel.Name = "radioButtonApplyToLevel";
			this.radioButtonApplyToLevel.Size = new System.Drawing.Size(218, 17);
			this.radioButtonApplyToLevel.TabIndex = 25;
			this.radioButtonApplyToLevel.TabStop = true;
			this.radioButtonApplyToLevel.Text = "Only Groups/Elements nested this deep: ";
			this.radioButtonApplyToLevel.UseVisualStyleBackColor = true;
			this.radioButtonApplyToLevel.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
			// 
			// groupByDepthOfEffect
			// 
			this.groupByDepthOfEffect.Controls.Add(this.numericUpDownDepthOfEffect);
			this.groupByDepthOfEffect.Controls.Add(this.radioButtonApplyToAllElements);
			this.groupByDepthOfEffect.Controls.Add(this.radioButtonApplyToLevel);
			this.groupByDepthOfEffect.Location = new System.Drawing.Point(3, 288);
			this.groupByDepthOfEffect.Name = "groupByDepthOfEffect";
			this.groupByDepthOfEffect.Size = new System.Drawing.Size(268, 74);
			this.groupByDepthOfEffect.TabIndex = 26;
			this.groupByDepthOfEffect.TabStop = false;
			this.groupByDepthOfEffect.Text = "Effect Applies To";
			// 
			// numericUpDownDepthOfEffect
			// 
			this.numericUpDownDepthOfEffect.Location = new System.Drawing.Point(220, 43);
			this.numericUpDownDepthOfEffect.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericUpDownDepthOfEffect.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownDepthOfEffect.Name = "numericUpDownDepthOfEffect";
			this.numericUpDownDepthOfEffect.Size = new System.Drawing.Size(42, 20);
			this.numericUpDownDepthOfEffect.TabIndex = 26;
			this.numericUpDownDepthOfEffect.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// curveTypeEditorControlChaseMovement
			// 
			curve2.IsCurrentLibraryCurve = false;
			curve2.LibraryReferenceName = "";
			this.curveTypeEditorControlChaseMovement.CurveValue = curve2;
			this.curveTypeEditorControlChaseMovement.EffectParameterValues = new object[] {
        ((object)(curve2))};
			this.curveTypeEditorControlChaseMovement.Location = new System.Drawing.Point(108, 3);
			this.curveTypeEditorControlChaseMovement.Name = "curveTypeEditorControlChaseMovement";
			this.curveTypeEditorControlChaseMovement.Size = new System.Drawing.Size(150, 80);
			this.curveTypeEditorControlChaseMovement.TabIndex = 17;
			this.curveTypeEditorControlChaseMovement.TargetEffect = null;
			// 
			// levelTypeEditorControlDefaultLevel
			// 
			this.levelTypeEditorControlDefaultLevel.EffectParameterValues = new object[] {
        ((object)(1D))};
			this.levelTypeEditorControlDefaultLevel.LevelValue = 1D;
			this.levelTypeEditorControlDefaultLevel.Location = new System.Drawing.Point(399, 214);
			this.levelTypeEditorControlDefaultLevel.Name = "levelTypeEditorControlDefaultLevel";
			this.levelTypeEditorControlDefaultLevel.Size = new System.Drawing.Size(90, 39);
			this.levelTypeEditorControlDefaultLevel.TabIndex = 16;
			this.levelTypeEditorControlDefaultLevel.TargetEffect = null;
			// 
			// ChaseEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupByDepthOfEffect);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.curveTypeEditorControlChaseMovement);
			this.Controls.Add(this.levelTypeEditorControlDefaultLevel);
			this.Controls.Add(this.groupBoxPulse);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.label5);
			this.Name = "ChaseEffectEditorControl";
			this.Size = new System.Drawing.Size(579, 372);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeOverlap)).EndInit();
			this.groupBoxPulse.ResumeLayout(false);
			this.groupBoxPulse.PerformLayout();
			this.groupBoxColor.ResumeLayout(false);
			this.groupBoxColor.PerformLayout();
			this.groupByDepthOfEffect.ResumeLayout(false);
			this.groupByDepthOfEffect.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControlEachPulse;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownPulseTimeOverlap;
		private System.Windows.Forms.Label label2;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControlChaseMovement;
		private LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControlDefaultLevel;
		private System.Windows.Forms.GroupBox groupBoxPulse;
		private System.Windows.Forms.RadioButton radioButtonGradientAcrossItems;
		private System.Windows.Forms.RadioButton radioButtonStaticColor;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControlStaticColor;
		private System.Windows.Forms.GroupBox groupBoxColor;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton radioButtonGradientIndividual;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl colorGradientTypeEditorControlGradient;
		private System.Windows.Forms.RadioButton radioButtonGradientOverWhole;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton radioButtonApplyToAllElements;
		private System.Windows.Forms.RadioButton radioButtonApplyToLevel;
		private System.Windows.Forms.GroupBox groupByDepthOfEffect;
		private System.Windows.Forms.NumericUpDown numericUpDownDepthOfEffect;
		private System.Windows.Forms.CheckBox chkExtendPulseToEnd;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox chkExtendPulseToStart;
	}
}
