namespace TestTiming {
	partial class GenericStepperForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.labelCurrentStep = new System.Windows.Forms.Label();
			this.buttonNext = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownStep = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current step:";
			// 
			// labelCurrentStep
			// 
			this.labelCurrentStep.AutoSize = true;
			this.labelCurrentStep.Location = new System.Drawing.Point(94, 18);
			this.labelCurrentStep.Name = "labelCurrentStep";
			this.labelCurrentStep.Size = new System.Drawing.Size(13, 13);
			this.labelCurrentStep.TabIndex = 1;
			this.labelCurrentStep.Text = "0";
			// 
			// buttonNext
			// 
			this.buttonNext.Location = new System.Drawing.Point(137, 50);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(75, 23);
			this.buttonNext.TabIndex = 2;
			this.buttonNext.Text = "Step";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Step by";
			// 
			// numericUpDownStep
			// 
			this.numericUpDownStep.Location = new System.Drawing.Point(70, 53);
			this.numericUpDownStep.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownStep.Name = "numericUpDownStep";
			this.numericUpDownStep.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownStep.TabIndex = 4;
			this.numericUpDownStep.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// GenericStepperForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(234, 95);
			this.Controls.Add(this.numericUpDownStep);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonNext);
			this.Controls.Add(this.labelCurrentStep);
			this.Controls.Add(this.label1);
			this.Name = "GenericStepperForm";
			this.Text = "GenericStepperForm";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCurrentStep;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownStep;
	}
}