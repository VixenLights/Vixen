namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class BeatMarkExportDialog
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
			this.radioAudacityFormat = new System.Windows.Forms.RadioButton();
			this.radioVixen3Format = new System.Windows.Forms.RadioButton();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioAudacityFormat
			// 
			this.radioAudacityFormat.AutoSize = true;
			this.radioAudacityFormat.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.radioAudacityFormat.Location = new System.Drawing.Point(57, 76);
			this.radioAudacityFormat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioAudacityFormat.Name = "radioAudacityFormat";
			this.radioAudacityFormat.Size = new System.Drawing.Size(145, 24);
			this.radioAudacityFormat.TabIndex = 1;
			this.radioAudacityFormat.TabStop = true;
			this.radioAudacityFormat.Text = "Audacity format";
			this.radioAudacityFormat.UseVisualStyleBackColor = true;
			// 
			// radioVixen3Format
			// 
			this.radioVixen3Format.AutoSize = true;
			this.radioVixen3Format.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.radioVixen3Format.Location = new System.Drawing.Point(57, 41);
			this.radioVixen3Format.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioVixen3Format.Name = "radioVixen3Format";
			this.radioVixen3Format.Size = new System.Drawing.Size(136, 24);
			this.radioVixen3Format.TabIndex = 0;
			this.radioVixen3Format.TabStop = true;
			this.radioVixen3Format.Text = "Vixen 3 format";
			this.radioVixen3Format.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonOK.Location = new System.Drawing.Point(20, 138);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(112, 35);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonCancel.Location = new System.Drawing.Point(141, 138);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(112, 35);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// BeatMarkExportDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(278, 194);
			this.ControlBox = false;
			this.Controls.Add(this.radioAudacityFormat);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.radioVixen3Format);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(300, 250);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(300, 250);
			this.Name = "BeatMarkExportDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export Beat Marks";
			this.Load += new System.EventHandler(this.BeatMarkExportDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioAudacityFormat;
		private System.Windows.Forms.RadioButton radioVixen3Format;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}