namespace VixenModules.EffectEditor.TwinkleEffectEditor
{
    using VixenModules.EffectEditor.LevelTypeEditor;

    partial class TwinkleEffectEditorControl
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
            VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
            this.groupBoxLevels = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLevelVariation = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.levelTypeEditorControlMaxValue = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
            this.levelTypeEditorControlMinValue = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownCoverage = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownPulseTimeVariation = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownAveragePulseTime = new System.Windows.Forms.NumericUpDown();
            this.groupBoxColor = new System.Windows.Forms.GroupBox();
            this.radioButtonGradientAcrossItems = new System.Windows.Forms.RadioButton();
            this.colorTypeEditorControlStaticColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
            this.radioButtonStaticColor = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonGradientIndividual = new System.Windows.Forms.RadioButton();
            this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
            this.radioButtonGradientOverWhole = new System.Windows.Forms.RadioButton();
            this.groupBoxElements = new System.Windows.Forms.GroupBox();
            this.numericUpDownDepthOfEffect = new System.Windows.Forms.NumericUpDown();
            this.radioButtonSynchronizedElements = new System.Windows.Forms.RadioButton();
            this.radioButtonIndividualElements = new System.Windows.Forms.RadioButton();
            this.radioButtonApplyToLevel = new System.Windows.Forms.RadioButton();
            this.groupBoxLevels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevelVariation)).BeginInit();
            this.groupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoverage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeVariation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAveragePulseTime)).BeginInit();
            this.groupBoxColor.SuspendLayout();
            this.groupBoxElements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxLevels
            // 
            this.groupBoxLevels.Controls.Add(this.label5);
            this.groupBoxLevels.Controls.Add(this.numericUpDownLevelVariation);
            this.groupBoxLevels.Controls.Add(this.label2);
            this.groupBoxLevels.Controls.Add(this.label1);
            this.groupBoxLevels.Controls.Add(this.levelTypeEditorControlMaxValue);
            this.groupBoxLevels.Controls.Add(this.levelTypeEditorControlMinValue);
            this.groupBoxLevels.Location = new System.Drawing.Point(4, 226);
            this.groupBoxLevels.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxLevels.Name = "groupBoxLevels";
            this.groupBoxLevels.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxLevels.Size = new System.Drawing.Size(315, 223);
            this.groupBoxLevels.TabIndex = 1;
            this.groupBoxLevels.TabStop = false;
            this.groupBoxLevels.Text = "Levels";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 169);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "Level variation (%):";
            // 
            // numericUpDownLevelVariation
            // 
            this.numericUpDownLevelVariation.Location = new System.Drawing.Point(182, 166);
            this.numericUpDownLevelVariation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownLevelVariation.Name = "numericUpDownLevelVariation";
            this.numericUpDownLevelVariation.Size = new System.Drawing.Size(88, 26);
            this.numericUpDownLevelVariation.TabIndex = 2;
            this.numericUpDownLevelVariation.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 111);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Maximum Level:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Minimum Level:";
            // 
            // levelTypeEditorControlMaxValue
            // 
            this.levelTypeEditorControlMaxValue.EffectParameterValues = new object[] {
        ((object)(1D))};
            this.levelTypeEditorControlMaxValue.LevelValue = 1D;
            this.levelTypeEditorControlMaxValue.Location = new System.Drawing.Point(168, 91);
            this.levelTypeEditorControlMaxValue.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.levelTypeEditorControlMaxValue.Name = "levelTypeEditorControlMaxValue";
            this.levelTypeEditorControlMaxValue.Size = new System.Drawing.Size(135, 60);
            this.levelTypeEditorControlMaxValue.TabIndex = 1;
            this.levelTypeEditorControlMaxValue.TargetEffect = null;
            // 
            // levelTypeEditorControlMinValue
            // 
            this.levelTypeEditorControlMinValue.EffectParameterValues = new object[] {
        ((object)(1D))};
            this.levelTypeEditorControlMinValue.LevelValue = 1D;
            this.levelTypeEditorControlMinValue.Location = new System.Drawing.Point(168, 26);
            this.levelTypeEditorControlMinValue.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.levelTypeEditorControlMinValue.Name = "levelTypeEditorControlMinValue";
            this.levelTypeEditorControlMinValue.Size = new System.Drawing.Size(135, 60);
            this.levelTypeEditorControlMinValue.TabIndex = 0;
            this.levelTypeEditorControlMinValue.TargetEffect = null;
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Controls.Add(this.label6);
            this.groupBoxDetails.Controls.Add(this.numericUpDownCoverage);
            this.groupBoxDetails.Controls.Add(this.label4);
            this.groupBoxDetails.Controls.Add(this.numericUpDownPulseTimeVariation);
            this.groupBoxDetails.Controls.Add(this.label3);
            this.groupBoxDetails.Controls.Add(this.numericUpDownAveragePulseTime);
            this.groupBoxDetails.Location = new System.Drawing.Point(4, 5);
            this.groupBoxDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxDetails.Size = new System.Drawing.Size(315, 212);
            this.groupBoxDetails.TabIndex = 0;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Twinkle Details";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(94, 40);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "Coverage (%):";
            // 
            // numericUpDownCoverage
            // 
            this.numericUpDownCoverage.Location = new System.Drawing.Point(213, 37);
            this.numericUpDownCoverage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownCoverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCoverage.Name = "numericUpDownCoverage";
            this.numericUpDownCoverage.Size = new System.Drawing.Size(72, 26);
            this.numericUpDownCoverage.TabIndex = 0;
            this.numericUpDownCoverage.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 152);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Pulse time variation (%):";
            // 
            // numericUpDownPulseTimeVariation
            // 
            this.numericUpDownPulseTimeVariation.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPulseTimeVariation.Location = new System.Drawing.Point(213, 148);
            this.numericUpDownPulseTimeVariation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownPulseTimeVariation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownPulseTimeVariation.Name = "numericUpDownPulseTimeVariation";
            this.numericUpDownPulseTimeVariation.Size = new System.Drawing.Size(72, 26);
            this.numericUpDownPulseTimeVariation.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Average pulse time (ms):";
            // 
            // numericUpDownAveragePulseTime
            // 
            this.numericUpDownAveragePulseTime.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownAveragePulseTime.Location = new System.Drawing.Point(213, 92);
            this.numericUpDownAveragePulseTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownAveragePulseTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownAveragePulseTime.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownAveragePulseTime.Name = "numericUpDownAveragePulseTime";
            this.numericUpDownAveragePulseTime.Size = new System.Drawing.Size(72, 26);
            this.numericUpDownAveragePulseTime.TabIndex = 1;
            this.numericUpDownAveragePulseTime.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
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
            this.groupBoxColor.Location = new System.Drawing.Point(328, 11);
            this.groupBoxColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxColor.Name = "groupBoxColor";
            this.groupBoxColor.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxColor.Size = new System.Drawing.Size(417, 322);
            this.groupBoxColor.TabIndex = 2;
            this.groupBoxColor.TabStop = false;
            this.groupBoxColor.Text = "Color Handling";
            // 
            // radioButtonGradientAcrossItems
            // 
            this.radioButtonGradientAcrossItems.AutoSize = true;
            this.radioButtonGradientAcrossItems.Location = new System.Drawing.Point(9, 194);
            this.radioButtonGradientAcrossItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonGradientAcrossItems.Name = "radioButtonGradientAcrossItems";
            this.radioButtonGradientAcrossItems.Size = new System.Drawing.Size(356, 24);
            this.radioButtonGradientAcrossItems.TabIndex = 0;
            this.radioButtonGradientAcrossItems.TabStop = true;
            this.radioButtonGradientAcrossItems.Text = "The gradient is spread over the sub-elements.";
            this.radioButtonGradientAcrossItems.UseVisualStyleBackColor = true;
            // 
            // colorTypeEditorControlStaticColor
            // 
            this.colorTypeEditorControlStaticColor.ColorValue = System.Drawing.Color.Empty;
            this.colorTypeEditorControlStaticColor.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
            this.colorTypeEditorControlStaticColor.Location = new System.Drawing.Point(141, 31);
            this.colorTypeEditorControlStaticColor.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.colorTypeEditorControlStaticColor.Name = "colorTypeEditorControlStaticColor";
            this.colorTypeEditorControlStaticColor.Size = new System.Drawing.Size(60, 62);
            this.colorTypeEditorControlStaticColor.TabIndex = 8;
            this.colorTypeEditorControlStaticColor.TargetEffect = null;
            // 
            // radioButtonStaticColor
            // 
            this.radioButtonStaticColor.AutoSize = true;
            this.radioButtonStaticColor.Location = new System.Drawing.Point(9, 49);
            this.radioButtonStaticColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonStaticColor.Name = "radioButtonStaticColor";
            this.radioButtonStaticColor.Size = new System.Drawing.Size(120, 24);
            this.radioButtonStaticColor.TabIndex = 0;
            this.radioButtonStaticColor.TabStop = true;
            this.radioButtonStaticColor.Text = "Static Color:";
            this.radioButtonStaticColor.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 262);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Gradient:";
            // 
            // radioButtonGradientIndividual
            // 
            this.radioButtonGradientIndividual.AutoSize = true;
            this.radioButtonGradientIndividual.Location = new System.Drawing.Point(9, 158);
            this.radioButtonGradientIndividual.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonGradientIndividual.Name = "radioButtonGradientIndividual";
            this.radioButtonGradientIndividual.Size = new System.Drawing.Size(366, 24);
            this.radioButtonGradientIndividual.TabIndex = 0;
            this.radioButtonGradientIndividual.TabStop = true;
            this.radioButtonGradientIndividual.Text = "Each individual twinkle uses the entire gradient.";
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
            this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(118, 240);
            this.colorGradientTypeEditorControlGradient.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.colorGradientTypeEditorControlGradient.Name = "colorGradientTypeEditorControlGradient";
            this.colorGradientTypeEditorControlGradient.Size = new System.Drawing.Size(150, 62);
            this.colorGradientTypeEditorControlGradient.TabIndex = 1;
            this.colorGradientTypeEditorControlGradient.TargetEffect = null;
            // 
            // radioButtonGradientOverWhole
            // 
            this.radioButtonGradientOverWhole.AutoSize = true;
            this.radioButtonGradientOverWhole.Location = new System.Drawing.Point(9, 103);
            this.radioButtonGradientOverWhole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonGradientOverWhole.Name = "radioButtonGradientOverWhole";
            this.radioButtonGradientOverWhole.Size = new System.Drawing.Size(390, 44);
            this.radioButtonGradientOverWhole.TabIndex = 0;
            this.radioButtonGradientOverWhole.TabStop = true;
            this.radioButtonGradientOverWhole.Text = "The gradient is shown over the whole effect.\r\nAll elements display the same color" +
    " at a given time.";
            this.radioButtonGradientOverWhole.UseVisualStyleBackColor = true;
            // 
            // groupBoxElements
            // 
            this.groupBoxElements.Controls.Add(this.numericUpDownDepthOfEffect);
            this.groupBoxElements.Controls.Add(this.radioButtonSynchronizedElements);
            this.groupBoxElements.Controls.Add(this.radioButtonIndividualElements);
            this.groupBoxElements.Controls.Add(this.radioButtonApplyToLevel);
            this.groupBoxElements.Location = new System.Drawing.Point(328, 342);
            this.groupBoxElements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxElements.Name = "groupBoxElements";
            this.groupBoxElements.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxElements.Size = new System.Drawing.Size(417, 138);
            this.groupBoxElements.TabIndex = 3;
            this.groupBoxElements.TabStop = false;
            this.groupBoxElements.Text = "Element Handling";
            // 
            // numericUpDownDepthOfEffect
            // 
            this.numericUpDownDepthOfEffect.Location = new System.Drawing.Point(350, 65);
            this.numericUpDownDepthOfEffect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numericUpDownDepthOfEffect.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownDepthOfEffect.TabIndex = 1;
            this.numericUpDownDepthOfEffect.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonSynchronizedElements
            // 
            this.radioButtonSynchronizedElements.AutoSize = true;
            this.radioButtonSynchronizedElements.Location = new System.Drawing.Point(9, 100);
            this.radioButtonSynchronizedElements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonSynchronizedElements.Name = "radioButtonSynchronizedElements";
            this.radioButtonSynchronizedElements.Size = new System.Drawing.Size(351, 24);
            this.radioButtonSynchronizedElements.TabIndex = 0;
            this.radioButtonSynchronizedElements.TabStop = true;
            this.radioButtonSynchronizedElements.Text = "Twinkle all Elements together (synchronized).";
            this.radioButtonSynchronizedElements.UseVisualStyleBackColor = true;
            this.radioButtonSynchronizedElements.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
            // 
            // radioButtonIndividualElements
            // 
            this.radioButtonIndividualElements.AutoSize = true;
            this.radioButtonIndividualElements.Location = new System.Drawing.Point(9, 29);
            this.radioButtonIndividualElements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonIndividualElements.Name = "radioButtonIndividualElements";
            this.radioButtonIndividualElements.Size = new System.Drawing.Size(279, 24);
            this.radioButtonIndividualElements.TabIndex = 0;
            this.radioButtonIndividualElements.TabStop = true;
            this.radioButtonIndividualElements.Text = "Twinkle all Elements independently";
            this.radioButtonIndividualElements.UseVisualStyleBackColor = true;
            this.radioButtonIndividualElements.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
            // 
            // radioButtonApplyToLevel
            // 
            this.radioButtonApplyToLevel.AutoSize = true;
            this.radioButtonApplyToLevel.Location = new System.Drawing.Point(9, 65);
            this.radioButtonApplyToLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonApplyToLevel.Name = "radioButtonApplyToLevel";
            this.radioButtonApplyToLevel.Size = new System.Drawing.Size(344, 24);
            this.radioButtonApplyToLevel.TabIndex = 0;
            this.radioButtonApplyToLevel.TabStop = true;
            this.radioButtonApplyToLevel.Text = "Twinkle Groups/Elements nested this deep: ";
            this.radioButtonApplyToLevel.UseVisualStyleBackColor = true;
            this.radioButtonApplyToLevel.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
            // 
            // TwinkleEffectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxElements);
            this.Controls.Add(this.groupBoxColor);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.groupBoxLevels);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TwinkleEffectEditorControl";
            this.Size = new System.Drawing.Size(756, 486);
            this.groupBoxLevels.ResumeLayout(false);
            this.groupBoxLevels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevelVariation)).EndInit();
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoverage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeVariation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAveragePulseTime)).EndInit();
            this.groupBoxColor.ResumeLayout(false);
            this.groupBoxColor.PerformLayout();
            this.groupBoxElements.ResumeLayout(false);
            this.groupBoxElements.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxLevels;
		private LevelTypeEditorControl levelTypeEditorControlMaxValue;
		private LevelTypeEditorControl levelTypeEditorControlMinValue;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBoxDetails;
		private System.Windows.Forms.GroupBox groupBoxColor;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownPulseTimeVariation;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownAveragePulseTime;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownLevelVariation;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDownCoverage;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton radioButtonGradientIndividual;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl colorGradientTypeEditorControlGradient;
		private System.Windows.Forms.RadioButton radioButtonGradientOverWhole;
		private System.Windows.Forms.GroupBox groupBoxElements;
		private System.Windows.Forms.RadioButton radioButtonSynchronizedElements;
		private System.Windows.Forms.RadioButton radioButtonIndividualElements;
		private System.Windows.Forms.RadioButton radioButtonStaticColor;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControlStaticColor;
		private System.Windows.Forms.RadioButton radioButtonGradientAcrossItems;
		private System.Windows.Forms.NumericUpDown numericUpDownDepthOfEffect;
		private System.Windows.Forms.RadioButton radioButtonApplyToLevel;
	}
}
