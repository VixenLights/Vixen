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
			VixenModules.App.Curves.Curve curve7 = new VixenModules.App.Curves.Curve();
			VixenModules.App.Curves.Curve curve8 = new VixenModules.App.Curves.Curve();
			VixenModules.App.ColorGradients.ColorGradient colorGradient4 = new VixenModules.App.ColorGradients.ColorGradient();
			this.label1 = new System.Windows.Forms.Label();
			this.curveTypeEditorControlEachPulse = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownPulseTimeOverlap = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.curveTypeEditorControlChaseMovement = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.levelTypeEditorControlDefaultLevel = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
			this.groupBoxPulse = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.radioButtonGradientAcrossItems = new System.Windows.Forms.RadioButton();
			this.radioButtonStaticColor = new System.Windows.Forms.RadioButton();
			this.colorTypeEditorControlStaticColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.radioButtonGradientIndividual = new System.Windows.Forms.RadioButton();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.radioButtonGradientOverWhole = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBoxApplyToChildren = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeOverlap)).BeginInit();
			this.groupBoxPulse.SuspendLayout();
			this.groupBoxColor.SuspendLayout();
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
			// curveTypeEditorControlEachPulse
			// 
			curve7.IsCurrentLibraryCurve = false;
			curve7.LibraryReferenceName = "";
			this.curveTypeEditorControlEachPulse.CurveValue = curve7;
			this.curveTypeEditorControlEachPulse.EffectParameterValues = new object[] {
        ((object)(curve7))};
			this.curveTypeEditorControlEachPulse.Location = new System.Drawing.Point(105, 23);
			this.curveTypeEditorControlEachPulse.Name = "curveTypeEditorControlEachPulse";
			this.curveTypeEditorControlEachPulse.Size = new System.Drawing.Size(150, 80);
			this.curveTypeEditorControlEachPulse.TabIndex = 14;
			this.curveTypeEditorControlEachPulse.TargetEffect = null;
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
			// curveTypeEditorControlChaseMovement
			// 
			curve8.IsCurrentLibraryCurve = false;
			curve8.LibraryReferenceName = "";
			this.curveTypeEditorControlChaseMovement.CurveValue = curve8;
			this.curveTypeEditorControlChaseMovement.EffectParameterValues = new object[] {
        ((object)(curve8))};
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
			// groupBoxPulse
			// 
			this.groupBoxPulse.Controls.Add(this.label3);
			this.groupBoxPulse.Controls.Add(this.label1);
			this.groupBoxPulse.Controls.Add(this.curveTypeEditorControlEachPulse);
			this.groupBoxPulse.Controls.Add(this.numericUpDownPulseTimeOverlap);
			this.groupBoxPulse.Location = new System.Drawing.Point(3, 89);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Size = new System.Drawing.Size(268, 164);
			this.groupBoxPulse.TabIndex = 13;
			this.groupBoxPulse.TabStop = false;
			this.groupBoxPulse.Text = "Pulse";
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
			// radioButtonGradientAcrossItems
			// 
			this.radioButtonGradientAcrossItems.AutoSize = true;
			this.radioButtonGradientAcrossItems.Location = new System.Drawing.Point(6, 126);
			this.radioButtonGradientAcrossItems.Name = "radioButtonGradientAcrossItems";
			this.radioButtonGradientAcrossItems.Size = new System.Drawing.Size(283, 17);
			this.radioButtonGradientAcrossItems.TabIndex = 9;
			this.radioButtonGradientAcrossItems.TabStop = true;
			this.radioButtonGradientAcrossItems.Text = "The gradient is spread over the range of sub-channels.";
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
			colorGradient4.Gammacorrected = false;
			colorGradient4.IsCurrentLibraryGradient = false;
			colorGradient4.LibraryReferenceName = "";
			colorGradient4.Title = null;
			this.colorGradientTypeEditorControlGradient.ColorGradientValue = colorGradient4;
			this.colorGradientTypeEditorControlGradient.EffectParameterValues = new object[] {
        ((object)(colorGradient4))};
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
			this.radioButtonGradientOverWhole.Size = new System.Drawing.Size(264, 30);
			this.radioButtonGradientOverWhole.TabIndex = 0;
			this.radioButtonGradientOverWhole.TabStop = true;
			this.radioButtonGradientOverWhole.Text = "The gradient is shown over the whole effect.\r\nAll channels display the same color" +
    " at a given time.";
			this.radioButtonGradientOverWhole.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(286, 236);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(109, 13);
			this.label5.TabIndex = 21;
			this.label5.Text = "(non-active channels)";
			// 
			// comboBoxApplyToChildren
			// 
			this.comboBoxApplyToChildren.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxApplyToChildren.FormattingEnabled = true;
			this.comboBoxApplyToChildren.Location = new System.Drawing.Point(108, 259);
			this.comboBoxApplyToChildren.Name = "comboBoxApplyToChildren";
			this.comboBoxApplyToChildren.Size = new System.Drawing.Size(163, 21);
			this.comboBoxApplyToChildren.TabIndex = 22;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(22, 262);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 13);
			this.label6.TabIndex = 23;
			this.label6.Text = "Apply chase to:";
			// 
			// ChaseEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboBoxApplyToChildren);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.curveTypeEditorControlChaseMovement);
			this.Controls.Add(this.levelTypeEditorControlDefaultLevel);
			this.Controls.Add(this.groupBoxPulse);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.label5);
			this.Name = "ChaseEffectEditorControl";
			this.Size = new System.Drawing.Size(579, 292);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeOverlap)).EndInit();
			this.groupBoxPulse.ResumeLayout(false);
			this.groupBoxPulse.PerformLayout();
			this.groupBoxColor.ResumeLayout(false);
			this.groupBoxColor.PerformLayout();
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
        private System.Windows.Forms.ComboBox comboBoxApplyToChildren;
        private System.Windows.Forms.Label label6;
	}
}
