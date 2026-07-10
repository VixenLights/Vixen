namespace Common.Controls.NameGeneration
{
	partial class NumericCounterEditor
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
			this.numericUpDownStartNumber = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownEndNumber = new System.Windows.Forms.NumericUpDown();
			this.checkBoxEndless = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownStep = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownStartNumber
			// 
			this.numericUpDownStartNumber.Location = new System.Drawing.Point(85, 6);
			this.numericUpDownStartNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericUpDownStartNumber.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
			this.numericUpDownStartNumber.Name = "numericUpDownStartNumber";
			this.numericUpDownStartNumber.Size = new System.Drawing.Size(58, 20);
			this.numericUpDownStartNumber.TabIndex = 0;
			this.numericUpDownStartNumber.ValueChanged += new System.EventHandler(this.numericUpDownStartNumber_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start Number:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "End Number:";
			// 
			// numericUpDownEndNumber
			// 
			this.numericUpDownEndNumber.Location = new System.Drawing.Point(85, 31);
			this.numericUpDownEndNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericUpDownEndNumber.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
			this.numericUpDownEndNumber.Name = "numericUpDownEndNumber";
			this.numericUpDownEndNumber.Size = new System.Drawing.Size(58, 20);
			this.numericUpDownEndNumber.TabIndex = 2;
			this.numericUpDownEndNumber.ValueChanged += new System.EventHandler(this.numericUpDownEndNumber_ValueChanged);
			// 
			// checkBoxEndless
			// 
			this.checkBoxEndless.AutoSize = true;
			this.checkBoxEndless.Location = new System.Drawing.Point(153, 32);
			this.checkBoxEndless.Name = "checkBoxEndless";
			this.checkBoxEndless.Size = new System.Drawing.Size(63, 17);
			this.checkBoxEndless.TabIndex = 4;
			this.checkBoxEndless.Text = "Endless";
			this.checkBoxEndless.UseVisualStyleBackColor = true;
			this.checkBoxEndless.CheckedChanged += new System.EventHandler(this.checkBoxEndless_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Step:";
			// 
			// numericUpDownStep
			// 
			this.numericUpDownStep.Location = new System.Drawing.Point(85, 56);
			this.numericUpDownStep.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericUpDownStep.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
			this.numericUpDownStep.Name = "numericUpDownStep";
			this.numericUpDownStep.Size = new System.Drawing.Size(58, 20);
			this.numericUpDownStep.TabIndex = 5;
			this.numericUpDownStep.ValueChanged += new System.EventHandler(this.numericUpDownStep_ValueChanged);
			// 
			// NumericCounterEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownStep);
			this.Controls.Add(this.checkBoxEndless);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numericUpDownEndNumber);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDownStartNumber);
			this.Name = "NumericCounterEditor";
			this.Size = new System.Drawing.Size(221, 88);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownStartNumber;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownEndNumber;
		private System.Windows.Forms.CheckBox checkBoxEndless;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownStep;
	}
}
