namespace VixenModules.Analysis.BeatsAndBars
{
	partial class BeatsAndBarsProgress
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.generateLabel = new System.Windows.Forms.Label();
			this.percentLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.progressBar1.Location = new System.Drawing.Point(14, 43);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(218, 27);
			this.progressBar1.TabIndex = 0;
			this.progressBar1.UseWaitCursor = true;
			// 
			// generateLabel
			// 
			this.generateLabel.AutoSize = true;
			this.generateLabel.Location = new System.Drawing.Point(10, 10);
			this.generateLabel.Name = "generateLabel";
			this.generateLabel.Size = new System.Drawing.Size(103, 15);
			this.generateLabel.TabIndex = 1;
			this.generateLabel.Text = "Generating Marks:";
			this.generateLabel.UseWaitCursor = true;
			// 
			// percentLabel
			// 
			this.percentLabel.AutoSize = true;
			this.percentLabel.Location = new System.Drawing.Point(239, 54);
			this.percentLabel.Name = "percentLabel";
			this.percentLabel.Size = new System.Drawing.Size(23, 15);
			this.percentLabel.TabIndex = 2;
			this.percentLabel.Text = "0%";
			this.percentLabel.UseWaitCursor = true;
			// 
			// BeatsAndBarsProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(275, 93);
			this.ControlBox = false;
			this.Controls.Add(this.percentLabel);
			this.Controls.Add(this.generateLabel);
			this.Controls.Add(this.progressBar1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "BeatsAndBarsProgress";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Beats and Bars";
			this.UseWaitCursor = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label generateLabel;
		private System.Windows.Forms.Label percentLabel;
	}
}