namespace Common.Controls.NameGeneration
{
	partial class WordIteratorEditor
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
			this.textBoxWords = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Words:";
			// 
			// textBoxWords
			// 
			this.textBoxWords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxWords.Location = new System.Drawing.Point(75, 5);
			this.textBoxWords.Name = "textBoxWords";
			this.textBoxWords.Size = new System.Drawing.Size(110, 20);
			this.textBoxWords.TabIndex = 7;
			this.textBoxWords.TextChanged += new System.EventHandler(this.textBoxLetters_TextChanged);
			// 
			// WordIteratorEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxWords);
			this.Controls.Add(this.label1);
			this.Name = "WordIteratorEditor";
			this.Size = new System.Drawing.Size(188, 36);
			this.Load += new System.EventHandler(this.NumericCounterEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxWords;
	}
}
