namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class AudacityImportDialog
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
			this.SuspendLayout();
			// 
			// radioVixen3Beats
			// 
			this.radioVixen3Beats.AutoSize = true;
			this.radioVixen3Beats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioVixen3Beats.Location = new System.Drawing.Point(31, 94);
			this.radioVixen3Beats.Name = "radioVixen3Beats";
			this.radioVixen3Beats.Size = new System.Drawing.Size(60, 17);
			this.radioVixen3Beats.TabIndex = 3;
			this.radioVixen3Beats.Text = "Vixen 3";
			this.radioVixen3Beats.UseVisualStyleBackColor = true;
			// 
			// radioAudacityBeats
			// 
			this.radioAudacityBeats.AutoSize = true;
			this.radioAudacityBeats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioAudacityBeats.Location = new System.Drawing.Point(31, 71);
			this.radioAudacityBeats.Name = "radioAudacityBeats";
			this.radioAudacityBeats.Size = new System.Drawing.Size(96, 17);
			this.radioAudacityBeats.TabIndex = 2;
			this.radioAudacityBeats.Text = "Audacity Beats";
			this.radioAudacityBeats.UseVisualStyleBackColor = true;
			// 
			// radioBeats
			// 
			this.radioBeats.AutoSize = true;
			this.radioBeats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioBeats.Location = new System.Drawing.Point(31, 48);
			this.radioBeats.Name = "radioBeats";
			this.radioBeats.Size = new System.Drawing.Size(150, 17);
			this.radioBeats.TabIndex = 1;
			this.radioBeats.Text = "Vamp Beat Tracker: Beats";
			this.radioBeats.UseVisualStyleBackColor = true;
			// 
			// radioBars
			// 
			this.radioBars.AutoSize = true;
			this.radioBars.Checked = true;
			this.radioBars.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioBars.Location = new System.Drawing.Point(31, 25);
			this.radioBars.Name = "radioBars";
			this.radioBars.Size = new System.Drawing.Size(144, 17);
			this.radioBars.TabIndex = 0;
			this.radioBars.TabStop = true;
			this.radioBars.Text = "Vamp Beat Tracker: Bars";
			this.radioBars.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.Transparent;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnCancel.Location = new System.Drawing.Point(119, 135);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnOk
			// 
			this.btnOk.BackColor = System.Drawing.Color.Transparent;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnOk.Location = new System.Drawing.Point(21, 135);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = false;
			this.btnOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// AudacityImportDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(216, 186);
			this.ControlBox = false;
			this.Controls.Add(this.radioVixen3Beats);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.radioAudacityBeats);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.radioBeats);
			this.Controls.Add(this.radioBars);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(232, 225);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(232, 225);
			this.Name = "AudacityImportDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Import Beat Selection";
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
	}
}