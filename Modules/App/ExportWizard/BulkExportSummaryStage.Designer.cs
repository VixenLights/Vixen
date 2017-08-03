namespace VixenModules.App.ExportWizard
{
	partial class BulkExportSummaryStage
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
			this.txtSummary = new System.Windows.Forms.TextBox();
			this.taskProgress = new System.Windows.Forms.ProgressBar();
			this.overallProgress = new System.Windows.Forms.ProgressBar();
			this.lblTaskProgress = new System.Windows.Forms.Label();
			this.lblOverallProgress = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtSummary
			// 
			this.txtSummary.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtSummary.Location = new System.Drawing.Point(3, 3);
			this.txtSummary.Multiline = true;
			this.txtSummary.Name = "txtSummary";
			this.txtSummary.ReadOnly = true;
			this.txtSummary.Size = new System.Drawing.Size(581, 130);
			this.txtSummary.TabIndex = 0;
			// 
			// taskProgress
			// 
			this.taskProgress.Location = new System.Drawing.Point(-1, 155);
			this.taskProgress.Name = "taskProgress";
			this.taskProgress.Size = new System.Drawing.Size(381, 23);
			this.taskProgress.TabIndex = 1;
			// 
			// overallProgress
			// 
			this.overallProgress.Location = new System.Drawing.Point(-1, 209);
			this.overallProgress.Name = "overallProgress";
			this.overallProgress.Size = new System.Drawing.Size(381, 22);
			this.overallProgress.TabIndex = 2;
			// 
			// lblTaskProgress
			// 
			this.lblTaskProgress.AutoSize = true;
			this.lblTaskProgress.Location = new System.Drawing.Point(0, 136);
			this.lblTaskProgress.Name = "lblTaskProgress";
			this.lblTaskProgress.Size = new System.Drawing.Size(39, 15);
			this.lblTaskProgress.TabIndex = 3;
			this.lblTaskProgress.Text = "Ready";
			// 
			// lblOverallProgress
			// 
			this.lblOverallProgress.AutoSize = true;
			this.lblOverallProgress.Location = new System.Drawing.Point(0, 191);
			this.lblOverallProgress.Name = "lblOverallProgress";
			this.lblOverallProgress.Size = new System.Drawing.Size(39, 15);
			this.lblOverallProgress.TabIndex = 5;
			this.lblOverallProgress.Text = "Ready";
			// 
			// BulkExportSummaryStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblOverallProgress);
			this.Controls.Add(this.lblTaskProgress);
			this.Controls.Add(this.overallProgress);
			this.Controls.Add(this.taskProgress);
			this.Controls.Add(this.txtSummary);
			this.Name = "BulkExportSummaryStage";
			this.Size = new System.Drawing.Size(616, 270);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtSummary;
		private System.Windows.Forms.ProgressBar taskProgress;
		private System.Windows.Forms.ProgressBar overallProgress;
		private System.Windows.Forms.Label lblTaskProgress;
		private System.Windows.Forms.Label lblOverallProgress;
	}
}
