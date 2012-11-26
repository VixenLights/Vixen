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
			VixenModules.App.ColorGradients.ColorGradient colorGradient2 = new VixenModules.App.ColorGradients.ColorGradient();
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
			this.colorTypeEditorControlStaticColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.radioButtonStaticColor = new System.Windows.Forms.RadioButton();
			this.label7 = new System.Windows.Forms.Label();
			this.radioButtonGradientIndividual = new System.Windows.Forms.RadioButton();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.radioButtonGradientOverWhole = new System.Windows.Forms.RadioButton();
			this.groupBoxChannels = new System.Windows.Forms.GroupBox();
			this.radioButtonSynchronizedChannels = new System.Windows.Forms.RadioButton();
			this.radioButtonIndividualChannels = new System.Windows.Forms.RadioButton();
			this.radioButtonGradientAcrossItems = new System.Windows.Forms.RadioButton();
			this.groupBoxLevels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevelVariation)).BeginInit();
			this.groupBoxDetails.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoverage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTimeVariation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownAveragePulseTime)).BeginInit();
			this.groupBoxColor.SuspendLayout();
			this.groupBoxChannels.SuspendLayout();
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
			this.groupBoxLevels.Location = new System.Drawing.Point(3, 147);
			this.groupBoxLevels.Name = "groupBoxLevels";
			this.groupBoxLevels.Size = new System.Drawing.Size(210, 145);
			this.groupBoxLevels.TabIndex = 0;
			this.groupBoxLevels.TabStop = false;
			this.groupBoxLevels.Text = "Levels";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 110);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Level variation (%):";
			// 
			// numericUpDownLevelVariation
			// 
			this.numericUpDownLevelVariation.Location = new System.Drawing.Point(121, 108);
			this.numericUpDownLevelVariation.Name = "numericUpDownLevelVariation";
			this.numericUpDownLevelVariation.Size = new System.Drawing.Size(59, 20);
			this.numericUpDownLevelVariation.TabIndex = 4;
			this.numericUpDownLevelVariation.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(26, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Maximum Level:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Minimum Level:";
			// 
			// levelTypeEditorControlMaxValue
			// 
			this.levelTypeEditorControlMaxValue.EffectParameterValues = new object[] {
        ((object)(1D))};
			this.levelTypeEditorControlMaxValue.LevelValue = 1D;
			this.levelTypeEditorControlMaxValue.Location = new System.Drawing.Point(112, 59);
			this.levelTypeEditorControlMaxValue.Name = "levelTypeEditorControlMaxValue";
			this.levelTypeEditorControlMaxValue.Size = new System.Drawing.Size(90, 39);
			this.levelTypeEditorControlMaxValue.TabIndex = 1;
			this.levelTypeEditorControlMaxValue.TargetEffect = null;
			// 
			// levelTypeEditorControlMinValue
			// 
			this.levelTypeEditorControlMinValue.EffectParameterValues = new object[] {
        ((object)(1D))};
			this.levelTypeEditorControlMinValue.LevelValue = 1D;
			this.levelTypeEditorControlMinValue.Location = new System.Drawing.Point(112, 17);
			this.levelTypeEditorControlMinValue.Name = "levelTypeEditorControlMinValue";
			this.levelTypeEditorControlMinValue.Size = new System.Drawing.Size(90, 39);
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
			this.groupBoxDetails.Location = new System.Drawing.Point(3, 3);
			this.groupBoxDetails.Name = "groupBoxDetails";
			this.groupBoxDetails.Size = new System.Drawing.Size(210, 138);
			this.groupBoxDetails.TabIndex = 1;
			this.groupBoxDetails.TabStop = false;
			this.groupBoxDetails.Text = "Twinkle Details";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(63, 26);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(73, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Coverage (%):";
			// 
			// numericUpDownCoverage
			// 
			this.numericUpDownCoverage.Location = new System.Drawing.Point(142, 24);
			this.numericUpDownCoverage.Name = "numericUpDownCoverage";
			this.numericUpDownCoverage.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownCoverage.TabIndex = 4;
			this.numericUpDownCoverage.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(118, 13);
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
			this.numericUpDownPulseTimeVariation.Location = new System.Drawing.Point(142, 96);
			this.numericUpDownPulseTimeVariation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownPulseTimeVariation.Name = "numericUpDownPulseTimeVariation";
			this.numericUpDownPulseTimeVariation.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownPulseTimeVariation.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(122, 13);
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
			this.numericUpDownAveragePulseTime.Location = new System.Drawing.Point(142, 60);
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
			this.numericUpDownAveragePulseTime.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownAveragePulseTime.TabIndex = 0;
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
			this.groupBoxColor.Location = new System.Drawing.Point(219, 7);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Size = new System.Drawing.Size(278, 209);
			this.groupBoxColor.TabIndex = 2;
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
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(23, 170);
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
			this.radioButtonGradientIndividual.Size = new System.Drawing.Size(249, 17);
			this.radioButtonGradientIndividual.TabIndex = 2;
			this.radioButtonGradientIndividual.TabStop = true;
			this.radioButtonGradientIndividual.Text = "Each individual twinkle uses the entire gradient.";
			this.radioButtonGradientIndividual.UseVisualStyleBackColor = true;
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
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(79, 156);
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
			// groupBoxChannels
			// 
			this.groupBoxChannels.Controls.Add(this.radioButtonSynchronizedChannels);
			this.groupBoxChannels.Controls.Add(this.radioButtonIndividualChannels);
			this.groupBoxChannels.Location = new System.Drawing.Point(219, 222);
			this.groupBoxChannels.Name = "groupBoxChannels";
			this.groupBoxChannels.Size = new System.Drawing.Size(278, 70);
			this.groupBoxChannels.TabIndex = 3;
			this.groupBoxChannels.TabStop = false;
			this.groupBoxChannels.Text = "Channel Handling";
			// 
			// radioButtonSynchronizedChannels
			// 
			this.radioButtonSynchronizedChannels.AutoSize = true;
			this.radioButtonSynchronizedChannels.Location = new System.Drawing.Point(6, 42);
			this.radioButtonSynchronizedChannels.Name = "radioButtonSynchronizedChannels";
			this.radioButtonSynchronizedChannels.Size = new System.Drawing.Size(237, 17);
			this.radioButtonSynchronizedChannels.TabIndex = 1;
			this.radioButtonSynchronizedChannels.TabStop = true;
			this.radioButtonSynchronizedChannels.Text = "Twinkle all channels together (synchronized).";
			this.radioButtonSynchronizedChannels.UseVisualStyleBackColor = true;
			// 
			// radioButtonIndividualChannels
			// 
			this.radioButtonIndividualChannels.AutoSize = true;
			this.radioButtonIndividualChannels.Location = new System.Drawing.Point(6, 19);
			this.radioButtonIndividualChannels.Name = "radioButtonIndividualChannels";
			this.radioButtonIndividualChannels.Size = new System.Drawing.Size(202, 17);
			this.radioButtonIndividualChannels.TabIndex = 0;
			this.radioButtonIndividualChannels.TabStop = true;
			this.radioButtonIndividualChannels.Text = "Twinkle each channel independently.";
			this.radioButtonIndividualChannels.UseVisualStyleBackColor = true;
			// 
			// radioButtonGradientAcrossItems
			// 
			this.radioButtonGradientAcrossItems.AutoSize = true;
			this.radioButtonGradientAcrossItems.Location = new System.Drawing.Point(6, 126);
			this.radioButtonGradientAcrossItems.Name = "radioButtonGradientAcrossItems";
			this.radioButtonGradientAcrossItems.Size = new System.Drawing.Size(241, 17);
			this.radioButtonGradientAcrossItems.TabIndex = 9;
			this.radioButtonGradientAcrossItems.TabStop = true;
			this.radioButtonGradientAcrossItems.Text = "The gradient is spread over the sub-channels.";
			this.radioButtonGradientAcrossItems.UseVisualStyleBackColor = true;
			// 
			// TwinkleEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxChannels);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.groupBoxDetails);
			this.Controls.Add(this.groupBoxLevels);
			this.Name = "TwinkleEffectEditorControl";
			this.Size = new System.Drawing.Size(504, 300);
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
			this.groupBoxChannels.ResumeLayout(false);
			this.groupBoxChannels.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBoxChannels;
		private System.Windows.Forms.RadioButton radioButtonSynchronizedChannels;
		private System.Windows.Forms.RadioButton radioButtonIndividualChannels;
		private System.Windows.Forms.RadioButton radioButtonStaticColor;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControlStaticColor;
		private System.Windows.Forms.RadioButton radioButtonGradientAcrossItems;
	}
}
