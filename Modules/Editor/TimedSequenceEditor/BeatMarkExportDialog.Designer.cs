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
			this.radioAudacityFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioAudacityFormat.Location = new System.Drawing.Point(44, 57);
			this.radioAudacityFormat.Name = "radioAudacityFormat";
			this.radioAudacityFormat.Size = new System.Drawing.Size(111, 19);
			this.radioAudacityFormat.TabIndex = 1;
			this.radioAudacityFormat.TabStop = true;
			this.radioAudacityFormat.Text = "Audacity format";
			this.radioAudacityFormat.UseVisualStyleBackColor = true;
			// 
			// radioVixen3Format
			// 
			this.radioVixen3Format.AutoSize = true;
			this.radioVixen3Format.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.radioVixen3Format.Location = new System.Drawing.Point(44, 31);
			this.radioVixen3Format.Name = "radioVixen3Format";
			this.radioVixen3Format.Size = new System.Drawing.Size(101, 19);
			this.radioVixen3Format.TabIndex = 0;
			this.radioVixen3Format.TabStop = true;
			this.radioVixen3Format.Text = "Vixen 3 format";
			this.radioVixen3Format.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.BackColor = System.Drawing.Color.Transparent;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonOK.Location = new System.Drawing.Point(15, 104);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = false;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonCancel.Location = new System.Drawing.Point(110, 104);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// BeatMarkExportDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(220, 159);
			this.ControlBox = false;
			this.Controls.Add(this.radioAudacityFormat);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.radioVixen3Format);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(236, 197);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(236, 197);
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