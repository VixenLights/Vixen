namespace VixenModules.EffectEditor.AlternatingEditor
{
	partial class AlternatingEffectEditorControl
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
            VixenModules.App.Curves.Curve curve1 = new VixenModules.App.Curves.Curve();
            VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
            VixenModules.App.Curves.Curve curve2 = new VixenModules.App.Curves.Curve();
            VixenModules.App.ColorGradients.ColorGradient colorGradient2 = new VixenModules.App.ColorGradients.ColorGradient();
            this.levelTypeEditorControl1 = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
            this.colorTypeEditorControl1 = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.curveTypeEditorControl1 = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
            this.radioGradient1 = new System.Windows.Forms.RadioButton();
            this.gradient1 = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
            this.radioStatic1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.curveTypeEditorControl2 = new VixenModules.EffectEditor.CurveTypeEditor.CurveTypeEditorControl();
            this.radioGradient2 = new System.Windows.Forms.RadioButton();
            this.radioStatic2 = new System.Windows.Forms.RadioButton();
            this.gradient2 = new VixenModules.EffectEditor.ColorGradientTypeEditor.ColorGradientTypeEditorControl();
            this.levelTypeEditorControl2 = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
            this.colorTypeEditorControl2 = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.trackBarInterval = new System.Windows.Forms.TrackBar();
            this.numChangeInterval = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numGroup = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChangeInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // levelTypeEditorControl1
            // 
            this.levelTypeEditorControl1.EffectParameterValues = new object[] {
        ((object)(1D))};
            this.levelTypeEditorControl1.LevelValue = 1D;
            this.levelTypeEditorControl1.Location = new System.Drawing.Point(147, 20);
            this.levelTypeEditorControl1.Name = "levelTypeEditorControl1";
            this.levelTypeEditorControl1.Size = new System.Drawing.Size(90, 39);
            this.levelTypeEditorControl1.TabIndex = 18;
            this.levelTypeEditorControl1.TargetEffect = null;
            // 
            // colorTypeEditorControl1
            // 
            this.colorTypeEditorControl1.ColorValue = System.Drawing.Color.Empty;
            this.colorTypeEditorControl1.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
            this.colorTypeEditorControl1.Location = new System.Drawing.Point(101, 19);
            this.colorTypeEditorControl1.Name = "colorTypeEditorControl1";
            this.colorTypeEditorControl1.Size = new System.Drawing.Size(40, 40);
            this.colorTypeEditorControl1.TabIndex = 17;
            this.colorTypeEditorControl1.TargetEffect = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.curveTypeEditorControl1);
            this.groupBox1.Controls.Add(this.radioGradient1);
            this.groupBox1.Controls.Add(this.gradient1);
            this.groupBox1.Controls.Add(this.radioStatic1);
            this.groupBox1.Controls.Add(this.levelTypeEditorControl1);
            this.groupBox1.Controls.Add(this.colorTypeEditorControl1);
            this.groupBox1.Location = new System.Drawing.Point(3, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 202);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color 1";
            // 
            // curveTypeEditorControl1
            // 
            curve1.IsCurrentLibraryCurve = false;
            curve1.LibraryReferenceName = "";
            this.curveTypeEditorControl1.CurveValue = curve1;
            this.curveTypeEditorControl1.EffectParameterValues = new object[] {
        ((object)(curve1))};
            this.curveTypeEditorControl1.Location = new System.Drawing.Point(101, 110);
            this.curveTypeEditorControl1.Name = "curveTypeEditorControl1";
            this.curveTypeEditorControl1.Size = new System.Drawing.Size(150, 80);
            this.curveTypeEditorControl1.TabIndex = 25;
            this.curveTypeEditorControl1.TargetEffect = null;
            // 
            // radioGradient1
            // 
            this.radioGradient1.AutoSize = true;
            this.radioGradient1.Location = new System.Drawing.Point(23, 76);
            this.radioGradient1.Name = "radioGradient1";
            this.radioGradient1.Size = new System.Drawing.Size(65, 17);
            this.radioGradient1.TabIndex = 24;
            this.radioGradient1.Text = "Gradient";
            this.radioGradient1.UseVisualStyleBackColor = true;
            this.radioGradient1.CheckedChanged += new System.EventHandler(this.radio1_CheckedChanged);
            // 
            // gradient1
            // 
            colorGradient1.Gammacorrected = false;
            colorGradient1.IsCurrentLibraryGradient = false;
            colorGradient1.LibraryReferenceName = "";
            colorGradient1.Title = null;
            this.gradient1.ColorGradientValue = colorGradient1;
            this.gradient1.EffectParameterValues = new object[] {
        ((object)(colorGradient1))};
            this.gradient1.Location = new System.Drawing.Point(101, 64);
            this.gradient1.Name = "gradient1";
            this.gradient1.Size = new System.Drawing.Size(100, 40);
            this.gradient1.TabIndex = 23;
            this.gradient1.TargetEffect = null;
            // 
            // radioStatic1
            // 
            this.radioStatic1.AutoSize = true;
            this.radioStatic1.Checked = true;
            this.radioStatic1.Location = new System.Drawing.Point(23, 33);
            this.radioStatic1.Name = "radioStatic1";
            this.radioStatic1.Size = new System.Drawing.Size(52, 17);
            this.radioStatic1.TabIndex = 22;
            this.radioStatic1.TabStop = true;
            this.radioStatic1.Text = "Static";
            this.radioStatic1.UseVisualStyleBackColor = true;
            this.radioStatic1.CheckedChanged += new System.EventHandler(this.radio1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.curveTypeEditorControl2);
            this.groupBox2.Controls.Add(this.radioGradient2);
            this.groupBox2.Controls.Add(this.radioStatic2);
            this.groupBox2.Controls.Add(this.gradient2);
            this.groupBox2.Controls.Add(this.levelTypeEditorControl2);
            this.groupBox2.Controls.Add(this.colorTypeEditorControl2);
            this.groupBox2.Location = new System.Drawing.Point(296, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 202);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Color 2";
            // 
            // curveTypeEditorControl2
            // 
            curve2.IsCurrentLibraryCurve = false;
            curve2.LibraryReferenceName = "";
            this.curveTypeEditorControl2.CurveValue = curve2;
            this.curveTypeEditorControl2.EffectParameterValues = new object[] {
        ((object)(curve2))};
            this.curveTypeEditorControl2.Location = new System.Drawing.Point(101, 110);
            this.curveTypeEditorControl2.Name = "curveTypeEditorControl2";
            this.curveTypeEditorControl2.Size = new System.Drawing.Size(150, 80);
            this.curveTypeEditorControl2.TabIndex = 23;
            this.curveTypeEditorControl2.TargetEffect = null;
            // 
            // radioGradient2
            // 
            this.radioGradient2.AutoSize = true;
            this.radioGradient2.Location = new System.Drawing.Point(23, 76);
            this.radioGradient2.Name = "radioGradient2";
            this.radioGradient2.Size = new System.Drawing.Size(65, 17);
            this.radioGradient2.TabIndex = 22;
            this.radioGradient2.Text = "Gradient";
            this.radioGradient2.UseVisualStyleBackColor = true;
            this.radioGradient2.CheckedChanged += new System.EventHandler(this.radio2_CheckedChanged);
            // 
            // radioStatic2
            // 
            this.radioStatic2.AutoSize = true;
            this.radioStatic2.Checked = true;
            this.radioStatic2.Location = new System.Drawing.Point(23, 33);
            this.radioStatic2.Name = "radioStatic2";
            this.radioStatic2.Size = new System.Drawing.Size(52, 17);
            this.radioStatic2.TabIndex = 21;
            this.radioStatic2.TabStop = true;
            this.radioStatic2.Text = "Static";
            this.radioStatic2.UseVisualStyleBackColor = true;
            this.radioStatic2.CheckedChanged += new System.EventHandler(this.radio2_CheckedChanged);
            // 
            // gradient2
            // 
            colorGradient2.Gammacorrected = false;
            colorGradient2.IsCurrentLibraryGradient = false;
            colorGradient2.LibraryReferenceName = "";
            colorGradient2.Title = null;
            this.gradient2.ColorGradientValue = colorGradient2;
            this.gradient2.EffectParameterValues = new object[] {
        ((object)(colorGradient2))};
            this.gradient2.Location = new System.Drawing.Point(101, 64);
            this.gradient2.Name = "gradient2";
            this.gradient2.Size = new System.Drawing.Size(100, 40);
            this.gradient2.TabIndex = 20;
            this.gradient2.TargetEffect = null;
            // 
            // levelTypeEditorControl2
            // 
            this.levelTypeEditorControl2.EffectParameterValues = new object[] {
        ((object)(1D))};
            this.levelTypeEditorControl2.LevelValue = 1D;
            this.levelTypeEditorControl2.Location = new System.Drawing.Point(147, 20);
            this.levelTypeEditorControl2.Name = "levelTypeEditorControl2";
            this.levelTypeEditorControl2.Size = new System.Drawing.Size(90, 39);
            this.levelTypeEditorControl2.TabIndex = 18;
            this.levelTypeEditorControl2.TargetEffect = null;
            // 
            // colorTypeEditorControl2
            // 
            this.colorTypeEditorControl2.ColorValue = System.Drawing.Color.Empty;
            this.colorTypeEditorControl2.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
            this.colorTypeEditorControl2.Location = new System.Drawing.Point(101, 19);
            this.colorTypeEditorControl2.Name = "colorTypeEditorControl2";
            this.colorTypeEditorControl2.Size = new System.Drawing.Size(40, 40);
            this.colorTypeEditorControl2.TabIndex = 17;
            this.colorTypeEditorControl2.TargetEffect = null;
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(20, 222);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkEnabled.Size = new System.Drawing.Size(120, 17);
            this.chkEnabled.TabIndex = 22;
            this.chkEnabled.Text = "Make Display Static";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // trackBarInterval
            // 
            this.trackBarInterval.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBarInterval.Location = new System.Drawing.Point(3, 16);
            this.trackBarInterval.Maximum = 5000;
            this.trackBarInterval.Minimum = 1;
            this.trackBarInterval.Name = "trackBarInterval";
            this.trackBarInterval.Size = new System.Drawing.Size(285, 45);
            this.trackBarInterval.TabIndex = 24;
            this.trackBarInterval.Value = 100;
            this.trackBarInterval.ValueChanged += new System.EventHandler(this.trackBarInterval_ValueChanged);
            // 
            // numChangeInterval
            // 
            this.numChangeInterval.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.numChangeInterval.Location = new System.Drawing.Point(3, 49);
            this.numChangeInterval.Name = "numChangeInterval";
            this.numChangeInterval.Size = new System.Drawing.Size(285, 20);
            this.numChangeInterval.TabIndex = 25;
            this.numChangeInterval.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numChangeInterval);
            this.groupBox3.Controls.Add(this.trackBarInterval);
            this.groupBox3.Location = new System.Drawing.Point(146, 222);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 72);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Change Interval";
            // 
            // numGroup
            // 
            this.numGroup.Location = new System.Drawing.Point(17, 271);
            this.numGroup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numGroup.Name = "numGroup";
            this.numGroup.Size = new System.Drawing.Size(65, 20);
            this.numGroup.TabIndex = 29;
            this.numGroup.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Group Element Effects";
            // 
            // AlternatingEffectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numGroup);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "AlternatingEffectEditorControl";
            this.Size = new System.Drawing.Size(564, 301);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChangeInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControl1;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControl1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControl2;
		private System.Windows.Forms.CheckBox chkEnabled;
		private System.Windows.Forms.TrackBar trackBarInterval;
		private System.Windows.Forms.NumericUpDown numChangeInterval;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioGradient1;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl gradient1;
		private System.Windows.Forms.RadioButton radioStatic1;
		private System.Windows.Forms.RadioButton radioGradient2;
		private System.Windows.Forms.RadioButton radioStatic2;
		private ColorGradientTypeEditor.ColorGradientTypeEditorControl gradient2;
		private System.Windows.Forms.NumericUpDown numGroup;
		private System.Windows.Forms.Label label1;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControl2;
		private CurveTypeEditor.CurveTypeEditorControl curveTypeEditorControl1;
		private LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControl2;
	}
}
