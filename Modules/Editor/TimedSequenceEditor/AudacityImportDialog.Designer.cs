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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioAudacityBeats = new System.Windows.Forms.RadioButton();
			this.radioBeats = new System.Windows.Forms.RadioButton();
			this.radioBars = new System.Windows.Forms.RadioButton();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.radioVixen3Beats = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioVixen3Beats);
			this.groupBox1.Controls.Add(this.radioAudacityBeats);
			this.groupBox1.Controls.Add(this.radioBeats);
			this.groupBox1.Controls.Add(this.radioBars);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(182, 117);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Beat Type";
			// 
			// radioAudacityBeats
			// 
			this.radioAudacityBeats.AutoSize = true;
			this.radioAudacityBeats.Location = new System.Drawing.Point(25, 66);
			this.radioAudacityBeats.Name = "radioAudacityBeats";
			this.radioAudacityBeats.Size = new System.Drawing.Size(96, 17);
			this.radioAudacityBeats.TabIndex = 2;
			this.radioAudacityBeats.Text = "Audacity Beats";
			this.radioAudacityBeats.UseVisualStyleBackColor = true;
			// 
			// radioBeats
			// 
			this.radioBeats.AutoSize = true;
			this.radioBeats.Location = new System.Drawing.Point(25, 43);
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
			this.radioBars.Location = new System.Drawing.Point(25, 20);
			this.radioBars.Name = "radioBars";
			this.radioBars.Size = new System.Drawing.Size(144, 17);
			this.radioBars.TabIndex = 0;
			this.radioBars.TabStop = true;
			this.radioBars.Text = "Vamp Beat Tracker: Bars";
			this.radioBars.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(119, 135);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(12, 135);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// radioVixen3Beats
			// 
			this.radioVixen3Beats.AutoSize = true;
			this.radioVixen3Beats.Location = new System.Drawing.Point(25, 89);
			this.radioVixen3Beats.Name = "radioVixen3Beats";
			this.radioVixen3Beats.Size = new System.Drawing.Size(60, 17);
			this.radioVixen3Beats.TabIndex = 3;
			this.radioVixen3Beats.Text = "Vixen 3";
			this.radioVixen3Beats.UseVisualStyleBackColor = true;
			// 
			// AudacityImportDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(206, 165);
			this.ControlBox = false;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AudacityImportDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Beat Selection";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioBeats;
		private System.Windows.Forms.RadioButton radioBars;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.RadioButton radioAudacityBeats;
		private System.Windows.Forms.RadioButton radioVixen3Beats;
	}
}