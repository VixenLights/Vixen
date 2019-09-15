namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	partial class SequencePackageImportFinishedStage
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblFinished = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblFinished
			// 
			this.lblFinished.AutoSize = true;
			this.lblFinished.Location = new System.Drawing.Point(26, 38);
			this.lblFinished.Name = "lblFinished";
			this.lblFinished.Size = new System.Drawing.Size(145, 15);
			this.lblFinished.TabIndex = 0;
			this.lblFinished.Text = "Package Import Complete";
			// 
			// SequencePackageImportFinishedStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblFinished);
			this.Name = "SequencePackageImportFinishedStage";
			this.Size = new System.Drawing.Size(363, 317);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFinished;
	}
}
