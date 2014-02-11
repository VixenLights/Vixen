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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioVixen3Format = new System.Windows.Forms.RadioButton();
			this.radioAudacityFormat = new System.Windows.Forms.RadioButton();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioAudacityFormat);
			this.groupBox1.Controls.Add(this.radioVixen3Format);
			this.groupBox1.Location = new System.Drawing.Point(13, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(156, 71);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Export Beat Marks";
			// 
			// radioVixen3Format
			// 
			this.radioVixen3Format.AutoSize = true;
			this.radioVixen3Format.Location = new System.Drawing.Point(7, 20);
			this.radioVixen3Format.Name = "radioVixen3Format";
			this.radioVixen3Format.Size = new System.Drawing.Size(92, 17);
			this.radioVixen3Format.TabIndex = 0;
			this.radioVixen3Format.TabStop = true;
			this.radioVixen3Format.Text = "Vixen 3 format";
			this.radioVixen3Format.UseVisualStyleBackColor = true;
			// 
			// radioAudacityFormat
			// 
			this.radioAudacityFormat.AutoSize = true;
			this.radioAudacityFormat.Location = new System.Drawing.Point(7, 43);
			this.radioAudacityFormat.Name = "radioAudacityFormat";
			this.radioAudacityFormat.Size = new System.Drawing.Size(98, 17);
			this.radioAudacityFormat.TabIndex = 1;
			this.radioAudacityFormat.TabStop = true;
			this.radioAudacityFormat.Text = "Audacity format";
			this.radioAudacityFormat.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(13, 90);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(94, 90);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// BeatMarkExportDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(182, 122);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BeatMarkExportDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export Beat Marks";
			this.Load += new System.EventHandler(this.BeatMarkExportDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioAudacityFormat;
		private System.Windows.Forms.RadioButton radioVixen3Format;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}