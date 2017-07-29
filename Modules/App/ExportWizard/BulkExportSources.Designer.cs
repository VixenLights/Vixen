namespace VixenModules.App.ExportWizard
{
	partial class BulkExportSources
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
			this.lblChooseSequences = new System.Windows.Forms.Label();
			this.lstSequences = new System.Windows.Forms.ListView();
			this.selectSequencesDialog = new System.Windows.Forms.OpenFileDialog();
			this.btnSelectSequences = new System.Windows.Forms.Button();
			this.sequences = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// lblChooseSequences
			// 
			this.lblChooseSequences.AutoSize = true;
			this.lblChooseSequences.Location = new System.Drawing.Point(78, 36);
			this.lblChooseSequences.Name = "lblChooseSequences";
			this.lblChooseSequences.Size = new System.Drawing.Size(149, 15);
			this.lblChooseSequences.TabIndex = 0;
			this.lblChooseSequences.Text = "Select sequences to export.";
			// 
			// lstSequences
			// 
			this.lstSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sequences});
			this.lstSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstSequences.Location = new System.Drawing.Point(37, 63);
			this.lstSequences.Name = "lstSequences";
			this.lstSequences.Size = new System.Drawing.Size(280, 303);
			this.lstSequences.TabIndex = 1;
			this.lstSequences.UseCompatibleStateImageBehavior = false;
			this.lstSequences.View = System.Windows.Forms.View.Details;
			// 
			// selectSequencesDialog
			// 
			this.selectSequencesDialog.DefaultExt = "tim";
			this.selectSequencesDialog.Filter = "Sequence Files|*.tim";
			this.selectSequencesDialog.Multiselect = true;
			this.selectSequencesDialog.Title = "Select Sequences";
			this.selectSequencesDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
			// 
			// btnSelectSequences
			// 
			this.btnSelectSequences.Location = new System.Drawing.Point(37, 30);
			this.btnSelectSequences.Name = "btnSelectSequences";
			this.btnSelectSequences.Size = new System.Drawing.Size(34, 27);
			this.btnSelectSequences.TabIndex = 2;
			this.btnSelectSequences.Text = "button1";
			this.btnSelectSequences.UseVisualStyleBackColor = true;
			this.btnSelectSequences.Click += new System.EventHandler(this.btnSelectSequences_Click);
			// 
			// sequences
			// 
			this.sequences.Text = "Sequences";
			// 
			// BulkExportSources
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSelectSequences);
			this.Controls.Add(this.lstSequences);
			this.Controls.Add(this.lblChooseSequences);
			this.Name = "BulkExportSources";
			this.Size = new System.Drawing.Size(351, 389);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChooseSequences;
		private System.Windows.Forms.ListView lstSequences;
		private System.Windows.Forms.OpenFileDialog selectSequencesDialog;
		private System.Windows.Forms.Button btnSelectSequences;
		private System.Windows.Forms.ColumnHeader sequences;
	}
}
