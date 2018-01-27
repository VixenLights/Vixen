namespace VixenApplication
{
	partial class CheckForUpdates
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
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.labelHeading = new System.Windows.Forms.Label();
			this.labelCurrentVersion = new System.Windows.Forms.Label();
			this.textBoxReleaseNotes = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.linkLabelVixenDownLoadPage = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(769, 708);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 35);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(20, 17);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(196, 197);
			this.pictureBoxIcon.TabIndex = 7;
			this.pictureBoxIcon.TabStop = false;
			// 
			// labelHeading
			// 
			this.labelHeading.AutoSize = true;
			this.labelHeading.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelHeading.Location = new System.Drawing.Point(251, 40);
			this.labelHeading.Name = "labelHeading";
			this.labelHeading.Size = new System.Drawing.Size(420, 32);
			this.labelHeading.TabIndex = 6;
			this.labelHeading.Text = "A new version of Vixen is available!";
			// 
			// labelCurrentVersion
			// 
			this.labelCurrentVersion.AutoSize = true;
			this.labelCurrentVersion.Location = new System.Drawing.Point(253, 139);
			this.labelCurrentVersion.Name = "labelCurrentVersion";
			this.labelCurrentVersion.Size = new System.Drawing.Size(50, 20);
			this.labelCurrentVersion.TabIndex = 5;
			this.labelCurrentVersion.Text = "label1";
			// 
			// textBoxReleaseNotes
			// 
			this.textBoxReleaseNotes.Location = new System.Drawing.Point(12, 274);
			this.textBoxReleaseNotes.Multiline = true;
			this.textBoxReleaseNotes.Name = "textBoxReleaseNotes";
			this.textBoxReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxReleaseNotes.Size = new System.Drawing.Size(858, 422);
			this.textBoxReleaseNotes.TabIndex = 4;
			this.textBoxReleaseNotes.WordWrap = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 242);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(363, 20);
			this.label2.TabIndex = 8;
			this.label2.Text = "Change Log between installed and available Versions:";
			// 
			// linkLabelVixenDownLoadPage
			// 
			this.linkLabelVixenDownLoadPage.AutoSize = true;
			this.linkLabelVixenDownLoadPage.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.linkLabelVixenDownLoadPage.Location = new System.Drawing.Point(255, 166);
			this.linkLabelVixenDownLoadPage.Name = "linkLabelVixenDownLoadPage";
			this.linkLabelVixenDownLoadPage.Size = new System.Drawing.Size(76, 20);
			this.linkLabelVixenDownLoadPage.TabIndex = 9;
			this.linkLabelVixenDownLoadPage.TabStop = true;
			this.linkLabelVixenDownLoadPage.Text = "linkLabel1";
			this.linkLabelVixenDownLoadPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVixenDownLoadPage_LinkClicked);
			// 
			// CheckForUpdates
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(882, 753);
			this.Controls.Add(this.linkLabelVixenDownLoadPage);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pictureBoxIcon);
			this.Controls.Add(this.labelHeading);
			this.Controls.Add(this.labelCurrentVersion);
			this.Controls.Add(this.textBoxReleaseNotes);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(900, 800);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(900, 800);
			this.Name = "CheckForUpdates";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Load += new System.EventHandler(this.CheckForUpdates_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox textBoxReleaseNotes;
		private System.Windows.Forms.Label labelCurrentVersion;
		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabelVixenDownLoadPage;
	}
}