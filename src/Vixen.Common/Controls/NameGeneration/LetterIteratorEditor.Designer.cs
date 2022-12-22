﻿namespace Common.Controls.NameGeneration
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxLetters = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Enter a list of Letters to be use";
			// 
			// textBoxLetters
			// 
			this.textBoxLetters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLetters.Location = new System.Drawing.Point(10, 44);
			this.textBoxLetters.Name = "textBoxLetters";
			this.textBoxLetters.Size = new System.Drawing.Size(184, 20);
			this.textBoxLetters.TabIndex = 7;
			this.textBoxLetters.TextChanged += new System.EventHandler(this.textBoxLetters_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(198, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Example: LCR LR NS EW UL (Only one)";
			// 
			// LetterIteratorEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxLetters);
			this.Controls.Add(this.label1);
			this.Name = "LetterIteratorEditor";
			this.Size = new System.Drawing.Size(208, 112);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxLetters;
		private System.Windows.Forms.Label label2;
	}
}
