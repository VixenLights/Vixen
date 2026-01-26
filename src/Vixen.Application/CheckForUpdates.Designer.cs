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
			btnOK = new Button();
			pictureBoxIcon = new PictureBox();
			labelHeading = new Label();
			labelCurrentVersion = new Label();
			textBoxReleaseNotes = new TextBox();
			lblChangeLog = new Label();
			buttonDownload = new Button();
			tableLayoutPanel1 = new TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// btnOK
			// 
			btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Location = new Point(677, 430);
			btnOK.Margin = new Padding(4, 4, 8, 4);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(88, 26);
			btnOK.TabIndex = 1;
			btnOK.Text = "OK";
			btnOK.UseVisualStyleBackColor = true;
			// 
			// pictureBoxIcon
			// 
			pictureBoxIcon.Dock = DockStyle.Fill;
			pictureBoxIcon.Location = new Point(3, 2);
			pictureBoxIcon.Margin = new Padding(3, 2, 3, 2);
			pictureBoxIcon.Name = "pictureBoxIcon";
			tableLayoutPanel1.SetRowSpan(pictureBoxIcon, 3);
			pictureBoxIcon.Size = new Size(172, 172);
			pictureBoxIcon.SizeMode = PictureBoxSizeMode.AutoSize;
			pictureBoxIcon.TabIndex = 7;
			pictureBoxIcon.TabStop = false;
			// 
			// labelHeading
			// 
			labelHeading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			labelHeading.AutoSize = true;
			labelHeading.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			labelHeading.Location = new Point(181, 0);
			labelHeading.Name = "labelHeading";
			labelHeading.Size = new Size(589, 25);
			labelHeading.TabIndex = 6;
			labelHeading.Text = "A new version of Vixen is available.";
			labelHeading.TextAlign = ContentAlignment.MiddleCenter;
			labelHeading.Visible = false;
			// 
			// labelCurrentVersion
			// 
			labelCurrentVersion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			labelCurrentVersion.AutoSize = true;
			labelCurrentVersion.Location = new Point(181, 25);
			labelCurrentVersion.Name = "labelCurrentVersion";
			labelCurrentVersion.Size = new Size(589, 15);
			labelCurrentVersion.TabIndex = 5;
			labelCurrentVersion.Text = "label1";
			labelCurrentVersion.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// textBoxReleaseNotes
			// 
			tableLayoutPanel1.SetColumnSpan(textBoxReleaseNotes, 2);
			textBoxReleaseNotes.Dock = DockStyle.Fill;
			textBoxReleaseNotes.Location = new Point(8, 203);
			textBoxReleaseNotes.Margin = new Padding(8, 2, 8, 10);
			textBoxReleaseNotes.MaximumSize = new Size(1000, 225);
			textBoxReleaseNotes.Multiline = true;
			textBoxReleaseNotes.Name = "textBoxReleaseNotes";
			textBoxReleaseNotes.ReadOnly = true;
			textBoxReleaseNotes.ScrollBars = ScrollBars.Both;
			textBoxReleaseNotes.Size = new Size(757, 213);
			textBoxReleaseNotes.TabIndex = 4;
			textBoxReleaseNotes.Visible = false;
			textBoxReleaseNotes.WordWrap = false;
			// 
			// lblChangeLog
			// 
			lblChangeLog.AutoSize = true;
			tableLayoutPanel1.SetColumnSpan(lblChangeLog, 2);
			lblChangeLog.Location = new Point(3, 181);
			lblChangeLog.Margin = new Padding(3, 5, 3, 5);
			lblChangeLog.Name = "lblChangeLog";
			lblChangeLog.Size = new Size(287, 15);
			lblChangeLog.TabIndex = 8;
			lblChangeLog.Text = "Change Log between installed and available Versions:";
			lblChangeLog.Visible = false;
			// 
			// buttonDownload
			// 
			buttonDownload.Anchor = AnchorStyles.None;
			buttonDownload.AutoSize = true;
			buttonDownload.Location = new Point(402, 95);
			buttonDownload.Name = "buttonDownload";
			buttonDownload.Size = new Size(146, 25);
			buttonDownload.TabIndex = 9;
			buttonDownload.Text = "Download";
			buttonDownload.Visible = false;
			buttonDownload.Click += buttonDownload_Click;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.Controls.Add(pictureBoxIcon, 0, 0);
			tableLayoutPanel1.Controls.Add(btnOK, 1, 5);
			tableLayoutPanel1.Controls.Add(textBoxReleaseNotes, 0, 4);
			tableLayoutPanel1.Controls.Add(lblChangeLog, 0, 3);
			tableLayoutPanel1.Controls.Add(buttonDownload, 1, 2);
			tableLayoutPanel1.Controls.Add(labelHeading, 1, 0);
			tableLayoutPanel1.Controls.Add(labelCurrentVersion, 1, 1);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 6;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(773, 466);
			tableLayoutPanel1.TabIndex = 10;
			// 
			// CheckForUpdates
			// 
			AcceptButton = btnOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(773, 466);
			Controls.Add(tableLayoutPanel1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Margin = new Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "CheckForUpdates";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Load += CheckForUpdates_Load;
			((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox textBoxReleaseNotes;
		private System.Windows.Forms.Label labelCurrentVersion;
		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label lblChangeLog;
		private System.Windows.Forms.Button buttonDownload;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}