namespace VixenApplication
{
	partial class AboutVixen
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
			this.btnOK = new System.Windows.Forms.Button();
			this.textBoxLicense = new System.Windows.Forms.TextBox();
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.labelHeading = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(770, 708);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 35);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// textBoxLicense
			// 
			this.textBoxLicense.Location = new System.Drawing.Point(12, 259);
			this.textBoxLicense.Multiline = true;
			this.textBoxLicense.Name = "textBoxLicense";
			this.textBoxLicense.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLicense.Size = new System.Drawing.Size(858, 437);
			this.textBoxLicense.TabIndex = 4;
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(196, 197);
			this.pictureBoxIcon.TabIndex = 9;
			this.pictureBoxIcon.TabStop = false;
			// 
			// labelHeading
			// 
			this.labelHeading.AutoSize = true;
			this.labelHeading.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelHeading.Location = new System.Drawing.Point(243, 35);
			this.labelHeading.Name = "labelHeading";
			this.labelHeading.Size = new System.Drawing.Size(79, 32);
			this.labelHeading.TabIndex = 8;
			this.labelHeading.Text = "Vixen";
			// 
			// AboutVixen
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(882, 753);
			this.Controls.Add(this.pictureBoxIcon);
			this.Controls.Add(this.labelHeading);
			this.Controls.Add(this.textBoxLicense);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(900, 800);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(900, 800);
			this.Name = "AboutVixen";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Vixen";
			this.Load += new System.EventHandler(this.AboutVixen_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox textBoxLicense;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label labelHeading;
	}
}