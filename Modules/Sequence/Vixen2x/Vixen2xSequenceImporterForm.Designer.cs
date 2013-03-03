namespace VixenModules.SequenceType.Vixen2x
{
	partial class Vixen2xSequenceImporterForm
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
			this.lblStatusLine = new System.Windows.Forms.Label();
			this.pbImport = new System.Windows.Forms.ProgressBar();
			this.dialogOpen = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// lblStatusLine
			// 
			this.lblStatusLine.AutoSize = true;
			this.lblStatusLine.Location = new System.Drawing.Point(10, 9);
			this.lblStatusLine.Name = "lblStatusLine";
			this.lblStatusLine.Size = new System.Drawing.Size(61, 13);
			this.lblStatusLine.TabIndex = 2;
			this.lblStatusLine.Text = "Status here";
			// 
			// pbImport
			// 
			this.pbImport.Location = new System.Drawing.Point(13, 29);
			this.pbImport.Name = "pbImport";
			this.pbImport.Size = new System.Drawing.Size(549, 23);
			this.pbImport.TabIndex = 3;
			// 
			// Vixen2xSequenceImporterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(574, 68);
			this.ControlBox = false;
			this.Controls.Add(this.pbImport);
			this.Controls.Add(this.lblStatusLine);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "Vixen2xSequenceImporterForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Importing";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblStatusLine;
		private System.Windows.Forms.ProgressBar pbImport;
		private System.Windows.Forms.OpenFileDialog dialogOpen;
	}
}