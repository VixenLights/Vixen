namespace Common.Controls.NameGeneration
{
	partial class LetterIteratorEditor
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
			this.textBoxLetters = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.editorLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// editorLayoutPanel
			//
			this.editorLayoutPanel.ColumnCount = 1;
			this.editorLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.editorLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.editorLayoutPanel.Controls.Add(this.textBoxLetters, 0, 1);
			this.editorLayoutPanel.Controls.Add(this.label2, 0, 2);
			this.editorLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.editorLayoutPanel.Name = "editorLayoutPanel";
			this.editorLayoutPanel.Padding = new System.Windows.Forms.Padding(4);
			this.editorLayoutPanel.RowCount = 4;
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.editorLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.editorLayoutPanel.Size = new System.Drawing.Size(208, 112);
			this.editorLayoutPanel.TabIndex = 0;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Enter a list of Letters to be use";
			//
			// textBoxLetters
			//
			this.textBoxLetters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxLetters.Name = "textBoxLetters";
			this.textBoxLetters.TabIndex = 7;
			this.textBoxLetters.TextChanged += new System.EventHandler(this.textBoxLetters_TextChanged);
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(198, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Example: LCR LR NS EW UL (Only one)";
			//
			// LetterIteratorEditor
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.editorLayoutPanel);
			this.Name = "LetterIteratorEditor";
			this.Size = new System.Drawing.Size(208, 112);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			this.editorLayoutPanel.ResumeLayout(false);
			this.editorLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel editorLayoutPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxLetters;
		private System.Windows.Forms.Label label2;
	}
}
