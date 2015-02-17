namespace VixenModules.EffectEditor.SpinEffectEditor
{
	partial class SpinEffectEditorControl
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
			VixenModules.App.Curves.Curve curve1 = new VixenModules.App.Curves.Curve();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.radioButtonGradientAcrossItems = new System.Windows.Forms.RadioButton();
			this.radioButtonStaticColor = new System.Windows.Forms.RadioButton();
			this.label7 = new System.Windows.Forms.Label();
			this.radioButtonGradientIndividual = new System.Windows.Forms.RadioButton();
			this.radioButtonGradientOverWhole = new System.Windows.Forms.RadioButton();
			this.groupBoxSpeed = new System.Windows.Forms.GroupBox();
			this.radioButtonRevolutionTime = new System.Windows.Forms.RadioButton();
			this.radioButtonRevolutionFrequency = new System.Windows.Forms.RadioButton();
			this.radioButtonRevolutionCount = new System.Windows.Forms.RadioButton();
			this.numericUpDownRevolutionCount = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownRevolutionTime = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownRevolutionFrequency = new System.Windows.Forms.NumericUpDown();
			this.groupBoxPulse = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDownPulsePercentage = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownPulseTime = new System.Windows.Forms.NumericUpDown();
			this.radioButtonPulseEvenlyDistributed = new System.Windows.Forms.RadioButton();
			this.radioButtonPulsePercentage = new System.Windows.Forms.RadioButton();
			this.radioButtonPulseFixedTime = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxReverse = new System.Windows.Forms.CheckBox();
			this.groupByDepthOfEffect = new System.Windows.Forms.GroupBox();
			this.numericUpDownDepthOfEffect = new System.Windows.Forms.NumericUpDown();
			this.radioButtonApplyToAllElements = new System.Windows.Forms.RadioButton();
			this.radioButtonApplyToLevel = new System.Windows.Forms.RadioButton();
			this.levelTypeEditorControlDefaultLevel = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
			this.colorTypeEditorControlStaticColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.colorGradientTypeEditorControlGradient = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
			this.curveTypeEditorControlEachPulse = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
			this.groupBoxColor.SuspendLayout();
			this.groupBoxSpeed.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionFrequency)).BeginInit();
			this.groupBoxPulse.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulsePercentage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTime)).BeginInit();
			this.groupByDepthOfEffect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).BeginInit();
			this.SuspendLayout();
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
			this.groupBoxColor.Location = new System.Drawing.Point(460, 5);
			this.groupBoxColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxColor.Size = new System.Drawing.Size(442, 315);
			this.groupBoxColor.TabIndex = 3;
			this.groupBoxColor.TabStop = false;
			this.groupBoxColor.Text = "Color Handling";
			// 
			// radioButtonGradientAcrossItems
			// 
			this.radioButtonGradientAcrossItems.AutoSize = true;
			this.radioButtonGradientAcrossItems.Location = new System.Drawing.Point(9, 194);
			this.radioButtonGradientAcrossItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonGradientAcrossItems.Name = "radioButtonGradientAcrossItems";
			this.radioButtonGradientAcrossItems.Size = new System.Drawing.Size(419, 24);
			this.radioButtonGradientAcrossItems.TabIndex = 4;
			this.radioButtonGradientAcrossItems.TabStop = true;
			this.radioButtonGradientAcrossItems.Text = "The gradient is spread over the range of sub-elements.";
			this.radioButtonGradientAcrossItems.UseVisualStyleBackColor = true;
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
			this.label7.Location = new System.Drawing.Point(57, 255);
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
			this.radioButtonGradientIndividual.Size = new System.Drawing.Size(356, 24);
			this.radioButtonGradientIndividual.TabIndex = 3;
			this.radioButtonGradientIndividual.TabStop = true;
			this.radioButtonGradientIndividual.Text = "Each individual pulse uses the entire gradient.";
			this.radioButtonGradientIndividual.UseVisualStyleBackColor = true;
			// 
			// radioButtonGradientOverWhole
			// 
			this.radioButtonGradientOverWhole.AutoSize = true;
			this.radioButtonGradientOverWhole.Location = new System.Drawing.Point(9, 103);
			this.radioButtonGradientOverWhole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonGradientOverWhole.Name = "radioButtonGradientOverWhole";
			this.radioButtonGradientOverWhole.Size = new System.Drawing.Size(390, 44);
			this.radioButtonGradientOverWhole.TabIndex = 2;
			this.radioButtonGradientOverWhole.TabStop = true;
			this.radioButtonGradientOverWhole.Text = "The gradient is shown over the whole effect.\r\nAll elements display the same color" +
	" at a given time.";
			this.radioButtonGradientOverWhole.UseVisualStyleBackColor = true;
			// 
			// groupBoxSpeed
			// 
			this.groupBoxSpeed.Controls.Add(this.radioButtonRevolutionTime);
			this.groupBoxSpeed.Controls.Add(this.radioButtonRevolutionFrequency);
			this.groupBoxSpeed.Controls.Add(this.radioButtonRevolutionCount);
			this.groupBoxSpeed.Controls.Add(this.numericUpDownRevolutionCount);
			this.groupBoxSpeed.Controls.Add(this.numericUpDownRevolutionTime);
			this.groupBoxSpeed.Controls.Add(this.numericUpDownRevolutionFrequency);
			this.groupBoxSpeed.Location = new System.Drawing.Point(4, 5);
			this.groupBoxSpeed.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxSpeed.Name = "groupBoxSpeed";
			this.groupBoxSpeed.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxSpeed.Size = new System.Drawing.Size(447, 172);
			this.groupBoxSpeed.TabIndex = 0;
			this.groupBoxSpeed.TabStop = false;
			this.groupBoxSpeed.Text = "Spin Speed";
			// 
			// radioButtonRevolutionTime
			// 
			this.radioButtonRevolutionTime.AutoSize = true;
			this.radioButtonRevolutionTime.Location = new System.Drawing.Point(18, 117);
			this.radioButtonRevolutionTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonRevolutionTime.Name = "radioButtonRevolutionTime";
			this.radioButtonRevolutionTime.Size = new System.Drawing.Size(245, 24);
			this.radioButtonRevolutionTime.TabIndex = 4;
			this.radioButtonRevolutionTime.TabStop = true;
			this.radioButtonRevolutionTime.Text = "Fixed revolution duration (ms):";
			this.radioButtonRevolutionTime.UseVisualStyleBackColor = true;
			this.radioButtonRevolutionTime.CheckedChanged += new System.EventHandler(this.radioButtonRevolutionItem_CheckedChanged);
			// 
			// radioButtonRevolutionFrequency
			// 
			this.radioButtonRevolutionFrequency.AutoSize = true;
			this.radioButtonRevolutionFrequency.Location = new System.Drawing.Point(18, 77);
			this.radioButtonRevolutionFrequency.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonRevolutionFrequency.Name = "radioButtonRevolutionFrequency";
			this.radioButtonRevolutionFrequency.Size = new System.Drawing.Size(256, 24);
			this.radioButtonRevolutionFrequency.TabIndex = 2;
			this.radioButtonRevolutionFrequency.TabStop = true;
			this.radioButtonRevolutionFrequency.Text = "Fixed revolution frequency (Hz):";
			this.radioButtonRevolutionFrequency.UseVisualStyleBackColor = true;
			this.radioButtonRevolutionFrequency.CheckedChanged += new System.EventHandler(this.radioButtonRevolutionItem_CheckedChanged);
			// 
			// radioButtonRevolutionCount
			// 
			this.radioButtonRevolutionCount.AutoSize = true;
			this.radioButtonRevolutionCount.Location = new System.Drawing.Point(18, 37);
			this.radioButtonRevolutionCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonRevolutionCount.Name = "radioButtonRevolutionCount";
			this.radioButtonRevolutionCount.Size = new System.Drawing.Size(232, 24);
			this.radioButtonRevolutionCount.TabIndex = 0;
			this.radioButtonRevolutionCount.TabStop = true;
			this.radioButtonRevolutionCount.Text = "Fixed number of revolutions:";
			this.radioButtonRevolutionCount.UseVisualStyleBackColor = true;
			this.radioButtonRevolutionCount.CheckedChanged += new System.EventHandler(this.radioButtonRevolutionItem_CheckedChanged);
			// 
			// numericUpDownRevolutionCount
			// 
			this.numericUpDownRevolutionCount.DecimalPlaces = 2;
			this.numericUpDownRevolutionCount.Location = new System.Drawing.Point(296, 37);
			this.numericUpDownRevolutionCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownRevolutionCount.Maximum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.numericUpDownRevolutionCount.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			131072});
			this.numericUpDownRevolutionCount.Name = "numericUpDownRevolutionCount";
			this.numericUpDownRevolutionCount.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownRevolutionCount.TabIndex = 1;
			this.numericUpDownRevolutionCount.Value = new decimal(new int[] {
			3,
			0,
			0,
			0});
			this.numericUpDownRevolutionCount.ValueChanged += new System.EventHandler(this.numericUpDownAny_ValueChanged);
			// 
			// numericUpDownRevolutionTime
			// 
			this.numericUpDownRevolutionTime.Increment = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.numericUpDownRevolutionTime.Location = new System.Drawing.Point(296, 117);
			this.numericUpDownRevolutionTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownRevolutionTime.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.numericUpDownRevolutionTime.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownRevolutionTime.Name = "numericUpDownRevolutionTime";
			this.numericUpDownRevolutionTime.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownRevolutionTime.TabIndex = 5;
			this.numericUpDownRevolutionTime.Value = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.numericUpDownRevolutionTime.ValueChanged += new System.EventHandler(this.numericUpDownAny_ValueChanged);
			// 
			// numericUpDownRevolutionFrequency
			// 
			this.numericUpDownRevolutionFrequency.DecimalPlaces = 2;
			this.numericUpDownRevolutionFrequency.Location = new System.Drawing.Point(296, 77);
			this.numericUpDownRevolutionFrequency.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownRevolutionFrequency.Maximum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.numericUpDownRevolutionFrequency.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			131072});
			this.numericUpDownRevolutionFrequency.Name = "numericUpDownRevolutionFrequency";
			this.numericUpDownRevolutionFrequency.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownRevolutionFrequency.TabIndex = 3;
			this.numericUpDownRevolutionFrequency.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownRevolutionFrequency.ValueChanged += new System.EventHandler(this.numericUpDownAny_ValueChanged);
			// 
			// groupBoxPulse
			// 
			this.groupBoxPulse.Controls.Add(this.label1);
			this.groupBoxPulse.Controls.Add(this.curveTypeEditorControlEachPulse);
			this.groupBoxPulse.Controls.Add(this.numericUpDownPulsePercentage);
			this.groupBoxPulse.Controls.Add(this.numericUpDownPulseTime);
			this.groupBoxPulse.Controls.Add(this.radioButtonPulseEvenlyDistributed);
			this.groupBoxPulse.Controls.Add(this.radioButtonPulsePercentage);
			this.groupBoxPulse.Controls.Add(this.radioButtonPulseFixedTime);
			this.groupBoxPulse.Location = new System.Drawing.Point(4, 186);
			this.groupBoxPulse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxPulse.Name = "groupBoxPulse";
			this.groupBoxPulse.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxPulse.Size = new System.Drawing.Size(447, 283);
			this.groupBoxPulse.TabIndex = 1;
			this.groupBoxPulse.TabStop = false;
			this.groupBoxPulse.Text = "Pulse";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 195);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 20);
			this.label1.TabIndex = 15;
			this.label1.Text = "Individual Pulses:";
			// 
			// numericUpDownPulsePercentage
			// 
			this.numericUpDownPulsePercentage.Increment = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.numericUpDownPulsePercentage.Location = new System.Drawing.Point(296, 65);
			this.numericUpDownPulsePercentage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownPulsePercentage.Maximum = new decimal(new int[] {
			500,
			0,
			0,
			0});
			this.numericUpDownPulsePercentage.Name = "numericUpDownPulsePercentage";
			this.numericUpDownPulsePercentage.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownPulsePercentage.TabIndex = 2;
			this.numericUpDownPulsePercentage.Value = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.numericUpDownPulsePercentage.ValueChanged += new System.EventHandler(this.numericUpDownAny_ValueChanged);
			// 
			// numericUpDownPulseTime
			// 
			this.numericUpDownPulseTime.Increment = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.numericUpDownPulseTime.Location = new System.Drawing.Point(296, 105);
			this.numericUpDownPulseTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDownPulseTime.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.numericUpDownPulseTime.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownPulseTime.Name = "numericUpDownPulseTime";
			this.numericUpDownPulseTime.Size = new System.Drawing.Size(72, 26);
			this.numericUpDownPulseTime.TabIndex = 4;
			this.numericUpDownPulseTime.Value = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.numericUpDownPulseTime.ValueChanged += new System.EventHandler(this.numericUpDownAny_ValueChanged);
			// 
			// radioButtonPulseEvenlyDistributed
			// 
			this.radioButtonPulseEvenlyDistributed.AutoSize = true;
			this.radioButtonPulseEvenlyDistributed.Location = new System.Drawing.Point(24, 29);
			this.radioButtonPulseEvenlyDistributed.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonPulseEvenlyDistributed.Name = "radioButtonPulseEvenlyDistributed";
			this.radioButtonPulseEvenlyDistributed.Size = new System.Drawing.Size(401, 24);
			this.radioButtonPulseEvenlyDistributed.TabIndex = 0;
			this.radioButtonPulseEvenlyDistributed.TabStop = true;
			this.radioButtonPulseEvenlyDistributed.Text = "Evenly distributed pulses throughout each revolution";
			this.radioButtonPulseEvenlyDistributed.UseVisualStyleBackColor = true;
			this.radioButtonPulseEvenlyDistributed.CheckedChanged += new System.EventHandler(this.radioButtonPulseItem_CheckedChanged);
			// 
			// radioButtonPulsePercentage
			// 
			this.radioButtonPulsePercentage.AutoSize = true;
			this.radioButtonPulsePercentage.Location = new System.Drawing.Point(24, 65);
			this.radioButtonPulsePercentage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonPulsePercentage.Name = "radioButtonPulsePercentage";
			this.radioButtonPulsePercentage.Size = new System.Drawing.Size(249, 24);
			this.radioButtonPulsePercentage.TabIndex = 1;
			this.radioButtonPulsePercentage.TabStop = true;
			this.radioButtonPulsePercentage.Text = "Percentage of each revolution:";
			this.radioButtonPulsePercentage.UseVisualStyleBackColor = true;
			this.radioButtonPulsePercentage.CheckedChanged += new System.EventHandler(this.radioButtonPulseItem_CheckedChanged);
			// 
			// radioButtonPulseFixedTime
			// 
			this.radioButtonPulseFixedTime.AutoSize = true;
			this.radioButtonPulseFixedTime.Location = new System.Drawing.Point(24, 106);
			this.radioButtonPulseFixedTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonPulseFixedTime.Name = "radioButtonPulseFixedTime";
			this.radioButtonPulseFixedTime.Size = new System.Drawing.Size(249, 24);
			this.radioButtonPulseFixedTime.TabIndex = 3;
			this.radioButtonPulseFixedTime.TabStop = true;
			this.radioButtonPulseFixedTime.Text = "Fixed time for each pulse (ms):";
			this.radioButtonPulseFixedTime.UseVisualStyleBackColor = true;
			this.radioButtonPulseFixedTime.CheckedChanged += new System.EventHandler(this.radioButtonPulseItem_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(492, 349);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(239, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "Default level for all elements (%):";
			// 
			// checkBoxReverse
			// 
			this.checkBoxReverse.AutoSize = true;
			this.checkBoxReverse.Location = new System.Drawing.Point(496, 395);
			this.checkBoxReverse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkBoxReverse.Name = "checkBoxReverse";
			this.checkBoxReverse.Size = new System.Drawing.Size(139, 24);
			this.checkBoxReverse.TabIndex = 5;
			this.checkBoxReverse.Text = "Spin in reverse";
			this.checkBoxReverse.UseVisualStyleBackColor = true;
			// 
			// groupByDepthOfEffect
			// 
			this.groupByDepthOfEffect.Controls.Add(this.numericUpDownDepthOfEffect);
			this.groupByDepthOfEffect.Controls.Add(this.radioButtonApplyToAllElements);
			this.groupByDepthOfEffect.Controls.Add(this.radioButtonApplyToLevel);
			this.groupByDepthOfEffect.Location = new System.Drawing.Point(4, 478);
			this.groupByDepthOfEffect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupByDepthOfEffect.Name = "groupByDepthOfEffect";
			this.groupByDepthOfEffect.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupByDepthOfEffect.Size = new System.Drawing.Size(447, 114);
			this.groupByDepthOfEffect.TabIndex = 2;
			this.groupByDepthOfEffect.TabStop = false;
			this.groupByDepthOfEffect.Text = "Effect Applies To";
			// 
			// numericUpDownDepthOfEffect
			// 
			this.numericUpDownDepthOfEffect.Location = new System.Drawing.Point(345, 66);
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
			this.numericUpDownDepthOfEffect.TabIndex = 2;
			this.numericUpDownDepthOfEffect.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			// 
			// radioButtonApplyToAllElements
			// 
			this.radioButtonApplyToAllElements.AutoSize = true;
			this.radioButtonApplyToAllElements.Location = new System.Drawing.Point(24, 31);
			this.radioButtonApplyToAllElements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonApplyToAllElements.Name = "radioButtonApplyToAllElements";
			this.radioButtonApplyToAllElements.Size = new System.Drawing.Size(122, 24);
			this.radioButtonApplyToAllElements.TabIndex = 0;
			this.radioButtonApplyToAllElements.TabStop = true;
			this.radioButtonApplyToAllElements.Text = "All Elements";
			this.radioButtonApplyToAllElements.UseVisualStyleBackColor = true;
			this.radioButtonApplyToAllElements.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
			// 
			// radioButtonApplyToLevel
			// 
			this.radioButtonApplyToLevel.AutoSize = true;
			this.radioButtonApplyToLevel.Location = new System.Drawing.Point(24, 66);
			this.radioButtonApplyToLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonApplyToLevel.Name = "radioButtonApplyToLevel";
			this.radioButtonApplyToLevel.Size = new System.Drawing.Size(323, 24);
			this.radioButtonApplyToLevel.TabIndex = 1;
			this.radioButtonApplyToLevel.TabStop = true;
			this.radioButtonApplyToLevel.Text = "Only Groups/Elements nested this deep: ";
			this.radioButtonApplyToLevel.UseVisualStyleBackColor = true;
			this.radioButtonApplyToLevel.CheckedChanged += new System.EventHandler(this.radioButtonEffectAppliesTo_CheckedChanged);
			// 
			// levelTypeEditorControlDefaultLevel
			// 
			this.levelTypeEditorControlDefaultLevel.EffectParameterValues = new object[] {
		((object)(1D))};
			this.levelTypeEditorControlDefaultLevel.LevelValue = 1D;
			this.levelTypeEditorControlDefaultLevel.Location = new System.Drawing.Point(725, 331);
			this.levelTypeEditorControlDefaultLevel.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.levelTypeEditorControlDefaultLevel.Name = "levelTypeEditorControlDefaultLevel";
			this.levelTypeEditorControlDefaultLevel.Size = new System.Drawing.Size(135, 60);
			this.levelTypeEditorControlDefaultLevel.TabIndex = 4;
			this.levelTypeEditorControlDefaultLevel.TargetEffect = null;
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
			this.colorTypeEditorControlStaticColor.TabIndex = 1;
			this.colorTypeEditorControlStaticColor.TargetEffect = null;
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
			this.colorGradientTypeEditorControlGradient.Location = new System.Drawing.Point(141, 234);
			this.colorGradientTypeEditorControlGradient.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.colorGradientTypeEditorControlGradient.Name = "colorGradientTypeEditorControlGradient";
			this.colorGradientTypeEditorControlGradient.Size = new System.Drawing.Size(150, 62);
			this.colorGradientTypeEditorControlGradient.TabIndex = 5;
			this.colorGradientTypeEditorControlGradient.TargetEffect = null;
			// 
			// curveTypeEditorControlEachPulse
			// 
			curve1.IsCurrentLibraryCurve = false;
			curve1.LibraryReferenceName = "";
			this.curveTypeEditorControlEachPulse.CurveValue = curve1;
			this.curveTypeEditorControlEachPulse.EffectParameterValues = new object[] {
		((object)(curve1))};
			this.curveTypeEditorControlEachPulse.Location = new System.Drawing.Point(177, 145);
			this.curveTypeEditorControlEachPulse.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.curveTypeEditorControlEachPulse.Name = "curveTypeEditorControlEachPulse";
			this.curveTypeEditorControlEachPulse.Size = new System.Drawing.Size(225, 123);
			this.curveTypeEditorControlEachPulse.TabIndex = 5;
			this.curveTypeEditorControlEachPulse.TargetEffect = null;
			// 
			// SpinEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupByDepthOfEffect);
			this.Controls.Add(this.checkBoxReverse);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.levelTypeEditorControlDefaultLevel);
			this.Controls.Add(this.groupBoxColor);
			this.Controls.Add(this.groupBoxSpeed);
			this.Controls.Add(this.groupBoxPulse);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "SpinEffectEditorControl";
			this.Size = new System.Drawing.Size(909, 603);
			this.Load += new System.EventHandler(this.SpinEffectEditorControl_Load);
			this.groupBoxColor.ResumeLayout(false);
			this.groupBoxColor.PerformLayout();
			this.groupBoxSpeed.ResumeLayout(false);
			this.groupBoxSpeed.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevolutionFrequency)).EndInit();
			this.groupBoxPulse.ResumeLayout(false);
			this.groupBoxPulse.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulsePercentage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPulseTime)).EndInit();
			this.groupByDepthOfEffect.ResumeLayout(false);
			this.groupByDepthOfEffect.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepthOfEffect)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxColor;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControlStaticColor;
		private System.Windows.Forms.RadioButton radioButtonStaticColor;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton radioButtonGradientIndividual;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl colorGradientTypeEditorControlGradient;
		private System.Windows.Forms.RadioButton radioButtonGradientOverWhole;
		private System.Windows.Forms.GroupBox groupBoxSpeed;
		private System.Windows.Forms.NumericUpDown numericUpDownRevolutionCount;
		private System.Windows.Forms.NumericUpDown numericUpDownRevolutionTime;
		private System.Windows.Forms.NumericUpDown numericUpDownRevolutionFrequency;
		private System.Windows.Forms.GroupBox groupBoxPulse;
		private System.Windows.Forms.RadioButton radioButtonRevolutionCount;
		private System.Windows.Forms.RadioButton radioButtonRevolutionTime;
		private System.Windows.Forms.RadioButton radioButtonRevolutionFrequency;
		private System.Windows.Forms.NumericUpDown numericUpDownPulsePercentage;
		private System.Windows.Forms.NumericUpDown numericUpDownPulseTime;
		private System.Windows.Forms.RadioButton radioButtonPulseEvenlyDistributed;
		private System.Windows.Forms.RadioButton radioButtonPulsePercentage;
		private System.Windows.Forms.RadioButton radioButtonPulseFixedTime;
		private System.Windows.Forms.Label label1;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControlEachPulse;
		private System.Windows.Forms.RadioButton radioButtonGradientAcrossItems;
		private LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControlDefaultLevel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBoxReverse;
		private System.Windows.Forms.GroupBox groupByDepthOfEffect;
		private System.Windows.Forms.NumericUpDown numericUpDownDepthOfEffect;
		private System.Windows.Forms.RadioButton radioButtonApplyToAllElements;
		private System.Windows.Forms.RadioButton radioButtonApplyToLevel;
	}
}
