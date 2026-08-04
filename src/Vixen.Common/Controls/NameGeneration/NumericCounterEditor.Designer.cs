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
			this.editorLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDownStartNumber = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownEndNumber = new System.Windows.Forms.NumericUpDown();
			this.checkBoxEndless = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownStep = new System.Windows.Forms.NumericUpDown();
			this.editorLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).BeginInit();
			this.SuspendLayout();
			//
			// editorLayoutPanel
			//
			this.editorLayoutPanel.ColumnCount = 3;
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.editorLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.editorLayoutPanel.Controls.Add(this.numericUpDownStartNumber, 1, 0);
			this.editorLayoutPanel.Controls.Add(this.label2, 0, 1);
			this.editorLayoutPanel.Controls.Add(this.numericUpDownEndNumber, 1, 1);
			this.editorLayoutPanel.Controls.Add(this.checkBoxEndless, 2, 1);
			this.editorLayoutPanel.Controls.Add(this.label3, 0, 2);
			this.editorLayoutPanel.Controls.Add(this.numericUpDownStep, 1, 2);
			this.editorLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.editorLayoutPanel.Name = "editorLayoutPanel";
			this.editorLayoutPanel.Padding = new System.Windows.Forms.Padding(3);
			this.editorLayoutPanel.RowCount = 4;
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.editorLayoutPanel.Size = new System.Drawing.Size(221, 88);
			this.editorLayoutPanel.TabIndex = 0;
			//
			// label1
			//
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start Number:";
			//
			// numericUpDownStartNumber
			//
			this.numericUpDownStartNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
			// label2
			//
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "End Number:";
			//
			// numericUpDownEndNumber
			//
			this.numericUpDownEndNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
			this.checkBoxEndless.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.checkBoxEndless.AutoSize = true;
			this.checkBoxEndless.Name = "checkBoxEndless";
			this.checkBoxEndless.Size = new System.Drawing.Size(63, 17);
			this.checkBoxEndless.TabIndex = 4;
			this.checkBoxEndless.Text = "Endless";
			this.checkBoxEndless.UseVisualStyleBackColor = true;
			this.checkBoxEndless.CheckedChanged += new System.EventHandler(this.checkBoxEndless_CheckedChanged);
			//
			// label3
			//
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Step:";
			//
			// numericUpDownStep
			//
			this.numericUpDownStep.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
			this.Controls.Add(this.editorLayoutPanel);
			this.Name = "NumericCounterEditor";
			this.Size = new System.Drawing.Size(221, 88);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			this.editorLayoutPanel.ResumeLayout(false);
			this.editorLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel editorLayoutPanel;
		private System.Windows.Forms.NumericUpDown numericUpDownStartNumber;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownEndNumber;
		private System.Windows.Forms.CheckBox checkBoxEndless;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownStep;
	}
}
