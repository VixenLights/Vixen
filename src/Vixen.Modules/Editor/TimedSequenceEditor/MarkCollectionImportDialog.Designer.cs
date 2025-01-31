namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class MarkCollectionImportDialog
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
            this.radioVixen3Beats = new System.Windows.Forms.RadioButton();
            this.radioAudacityBeats = new System.Windows.Forms.RadioButton();
            this.radioBeats = new System.Windows.Forms.RadioButton();
            this.radioBars = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.radioXTiming = new System.Windows.Forms.RadioButton();
            this.radioPapagayo = new System.Windows.Forms.RadioButton();
            this.radioTimingTrackBrowser = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radioVixen3Beats
            // 
            this.radioVixen3Beats.AutoSize = true;
			radioVixen3Beats.Location = new Point(12, 93);
            this.radioVixen3Beats.Name = "radioVixen3Beats";
            this.radioVixen3Beats.Size = new System.Drawing.Size(63, 19);
            this.radioVixen3Beats.TabIndex = 3;
            this.radioVixen3Beats.Text = "Vixen 3";
            this.radioVixen3Beats.UseVisualStyleBackColor = true;
            // 
            // radioAudacityBeats
            // 
            this.radioAudacityBeats.AutoSize = true;
			radioAudacityBeats.Location = new Point(12, 67);
            this.radioAudacityBeats.Name = "radioAudacityBeats";
            this.radioAudacityBeats.Size = new System.Drawing.Size(103, 19);
            this.radioAudacityBeats.TabIndex = 2;
            this.radioAudacityBeats.Text = "Audacity Beats";
            this.radioAudacityBeats.UseVisualStyleBackColor = true;
            // 
            // radioBeats
            // 
            this.radioBeats.AutoSize = true;
			radioBeats.Location = new Point(12, 40);
            this.radioBeats.Name = "radioBeats";
            this.radioBeats.Size = new System.Drawing.Size(155, 19);
            this.radioBeats.TabIndex = 1;
            this.radioBeats.Text = "Vamp Beat Tracker: Beats";
            this.radioBeats.UseVisualStyleBackColor = true;
            // 
            // radioBars
            // 
            this.radioBars.AutoSize = true;
            this.radioBars.Checked = true;
			radioBars.Location = new Point(12, 14);
            this.radioBars.Name = "radioBars";
            this.radioBars.Size = new System.Drawing.Size(149, 19);
            this.radioBars.TabIndex = 0;
            this.radioBars.TabStop = true;
            this.radioBars.Text = "Vamp Beat Tracker: Bars";
            this.radioBars.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Location = new Point(105, 205);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnOk.Location = new Point(12, 205);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(87, 27);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // radioXTiming
            // 
            this.radioXTiming.AutoSize = true;
			radioXTiming.Location = new Point(12, 118);
            this.radioXTiming.Name = "radioXTiming";
            this.radioXTiming.Size = new System.Drawing.Size(68, 19);
            this.radioXTiming.TabIndex = 4;
            this.radioXTiming.Text = "xTiming";
            this.radioXTiming.UseVisualStyleBackColor = true;
            // 
            // radioPapagayo
            // 
            this.radioPapagayo.AutoSize = true;
			radioPapagayo.Location = new Point(12, 143);
            this.radioPapagayo.Name = "radioPapagayo";
            this.radioPapagayo.Size = new System.Drawing.Size(143, 19);
            this.radioPapagayo.TabIndex = 5;
            this.radioPapagayo.Text = "Papagayo Voice Tracks";
            this.radioPapagayo.UseVisualStyleBackColor = true;
            // 
            // radioTimingTrackBrowser
            // 
            this.radioTimingTrackBrowser.AutoSize = true;
			radioTimingTrackBrowser.Location = new Point(12, 168);
            this.radioTimingTrackBrowser.Name = "radioTimingTrackBrowser";
            this.radioTimingTrackBrowser.Size = new System.Drawing.Size(169, 19);
            this.radioTimingTrackBrowser.TabIndex = 6;
            this.radioTimingTrackBrowser.Text = "Singing Faces Lyric Browser";
            this.radioTimingTrackBrowser.UseVisualStyleBackColor = true;
            // 
            // MarkCollectionImportDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
			ClientSize = new Size(207, 244);
            this.Controls.Add(this.radioTimingTrackBrowser);
            this.Controls.Add(this.radioPapagayo);
            this.Controls.Add(this.radioXTiming);
            this.Controls.Add(this.radioVixen3Beats);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.radioAudacityBeats);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.radioBeats);
            this.Controls.Add(this.radioBars);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
            this.MinimizeBox = false;
			MinimumSize = new Size(150, 283);
            this.Name = "MarkCollectionImportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Type Selection";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioBeats;
		private System.Windows.Forms.RadioButton radioBars;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.RadioButton radioAudacityBeats;
		private System.Windows.Forms.RadioButton radioVixen3Beats;
		private System.Windows.Forms.RadioButton radioXTiming;
		private System.Windows.Forms.RadioButton radioPapagayo;
        private System.Windows.Forms.RadioButton radioTimingTrackBrowser;
    }
}