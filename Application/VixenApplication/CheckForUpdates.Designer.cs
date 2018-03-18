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
			this.lblChangeLog = new System.Windows.Forms.Label();
			this.linkLabelVixenDownLoadPage = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(631, 420);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(88, 26);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBoxIcon.Location = new System.Drawing.Point(3, 2);
			this.pictureBoxIcon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.tableLayoutPanel1.SetRowSpan(this.pictureBoxIcon, 3);
			this.pictureBoxIcon.Size = new System.Drawing.Size(172, 172);
			this.pictureBoxIcon.TabIndex = 7;
			this.pictureBoxIcon.TabStop = false;
			// 
			// labelHeading
			// 
			this.labelHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHeading.AutoSize = true;
			this.labelHeading.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelHeading.Location = new System.Drawing.Point(181, 0);
			this.labelHeading.Name = "labelHeading";
			this.labelHeading.Size = new System.Drawing.Size(539, 30);
			this.labelHeading.TabIndex = 6;
			this.labelHeading.Text = "A new version of Vixen is available!";
			this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelHeading.Visible = false;
			// 
			// labelCurrentVersion
			// 
			this.labelCurrentVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCurrentVersion.AutoSize = true;
			this.labelCurrentVersion.Location = new System.Drawing.Point(181, 30);
			this.labelCurrentVersion.Name = "labelCurrentVersion";
			this.labelCurrentVersion.Size = new System.Drawing.Size(539, 30);
			this.labelCurrentVersion.TabIndex = 5;
			this.labelCurrentVersion.Text = "label1";
			this.labelCurrentVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxReleaseNotes
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.textBoxReleaseNotes, 2);
			this.textBoxReleaseNotes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxReleaseNotes.Location = new System.Drawing.Point(8, 203);
			this.textBoxReleaseNotes.Margin = new System.Windows.Forms.Padding(8, 2, 8, 10);
			this.textBoxReleaseNotes.Multiline = true;
			this.textBoxReleaseNotes.Name = "textBoxReleaseNotes";
			this.textBoxReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxReleaseNotes.Size = new System.Drawing.Size(707, 203);
			this.textBoxReleaseNotes.TabIndex = 4;
			this.textBoxReleaseNotes.Visible = false;
			this.textBoxReleaseNotes.WordWrap = false;
			// 
			// lblChangeLog
			// 
			this.lblChangeLog.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.lblChangeLog, 2);
			this.lblChangeLog.Location = new System.Drawing.Point(3, 181);
			this.lblChangeLog.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.lblChangeLog.Name = "lblChangeLog";
			this.lblChangeLog.Size = new System.Drawing.Size(288, 15);
			this.lblChangeLog.TabIndex = 8;
			this.lblChangeLog.Text = "Change Log between installed and available Versions:";
			this.lblChangeLog.Visible = false;
			// 
			// linkLabelVixenDownLoadPage
			// 
			this.linkLabelVixenDownLoadPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabelVixenDownLoadPage.AutoSize = true;
			this.linkLabelVixenDownLoadPage.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.linkLabelVixenDownLoadPage.Location = new System.Drawing.Point(181, 60);
			this.linkLabelVixenDownLoadPage.Name = "linkLabelVixenDownLoadPage";
			this.linkLabelVixenDownLoadPage.Size = new System.Drawing.Size(539, 116);
			this.linkLabelVixenDownLoadPage.TabIndex = 9;
			this.linkLabelVixenDownLoadPage.TabStop = true;
			this.linkLabelVixenDownLoadPage.Text = "linkLabel1";
			this.linkLabelVixenDownLoadPage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.linkLabelVixenDownLoadPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVixenDownLoadPage_LinkClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.pictureBoxIcon, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnOK, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.textBoxReleaseNotes, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.lblChangeLog, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.linkLabelVixenDownLoadPage, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.labelHeading, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelCurrentVersion, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(723, 436);
			this.tableLayoutPanel1.TabIndex = 10;
			// 
			// CheckForUpdates
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(723, 436);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 600);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(700, 0);
			this.Name = "CheckForUpdates";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Load += new System.EventHandler(this.CheckForUpdates_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox textBoxReleaseNotes;
		private System.Windows.Forms.Label labelCurrentVersion;
		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label lblChangeLog;
		private System.Windows.Forms.LinkLabel linkLabelVixenDownLoadPage;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}