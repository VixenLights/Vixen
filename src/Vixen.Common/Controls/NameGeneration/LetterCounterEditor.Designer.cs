namespace Common.Controls.NameGeneration
{
	partial class LetterCounterEditor
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
			this.textBoxStartLetter = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownSteps = new System.Windows.Forms.NumericUpDown();
			this.editorLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSteps)).BeginInit();
			this.SuspendLayout();
			//
			// editorLayoutPanel
			//
			this.editorLayoutPanel.ColumnCount = 2;
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.editorLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.editorLayoutPanel.Controls.Add(this.textBoxStartLetter, 1, 0);
			this.editorLayoutPanel.Controls.Add(this.label2, 0, 1);
			this.editorLayoutPanel.Controls.Add(this.numericUpDownSteps, 1, 1);
			this.editorLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.editorLayoutPanel.Name = "editorLayoutPanel";
			this.editorLayoutPanel.Padding = new System.Windows.Forms.Padding(4);
			this.editorLayoutPanel.RowCount = 2;
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.Size = new System.Drawing.Size(200, 97);
			this.editorLayoutPanel.TabIndex = 0;
			//
			// label1
			//
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start Letter:";
			//
			// textBoxStartLetter
			//
			this.textBoxStartLetter.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.textBoxStartLetter.MaxLength = 1;
			this.textBoxStartLetter.Name = "textBoxStartLetter";
			this.textBoxStartLetter.Size = new System.Drawing.Size(64, 26);
			this.textBoxStartLetter.TabIndex = 0;
			this.textBoxStartLetter.TextChanged += new System.EventHandler(this.textBoxStartLetter_TextChanged);
			//
			// label2
			//
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Steps:";
			//
			// numericUpDownSteps
			//
			this.numericUpDownSteps.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.numericUpDownSteps.Maximum = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.numericUpDownSteps.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownSteps.Name = "numericUpDownSteps";
			this.numericUpDownSteps.Size = new System.Drawing.Size(66, 26);
			this.numericUpDownSteps.TabIndex = 1;
			this.numericUpDownSteps.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownSteps.ValueChanged += new System.EventHandler(this.numericUpDownSteps_ValueChanged);
			//
			// LetterCounterEditor
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.editorLayoutPanel);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "LetterCounterEditor";
			this.Size = new System.Drawing.Size(200, 97);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			this.editorLayoutPanel.ResumeLayout(false);
			this.editorLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSteps)).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel editorLayoutPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownSteps;
		private System.Windows.Forms.TextBox textBoxStartLetter;
	}
}
