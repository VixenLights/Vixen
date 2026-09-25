namespace VixenModules.App.ExportWizard
{
	partial class BulkExportOutputFormatStage
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			groupBox1 = new System.Windows.Forms.GroupBox();
			resolutionComboBox = new System.Windows.Forms.ComboBox();
			label4 = new System.Windows.Forms.Label();
			outputFormatComboBox = new System.Windows.Forms.ComboBox();
			label3 = new System.Windows.Forms.Label();
			lblChooseOutputFormat = new System.Windows.Forms.Label();
			btnOuputFolderSelect = new System.Windows.Forms.Button();
			grpSequence = new System.Windows.Forms.GroupBox();
			txtOutputFolder = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			chkRenameAudio = new System.Windows.Forms.CheckBox();
			chkIncludeAudio = new System.Windows.Forms.CheckBox();
			grpAudio = new System.Windows.Forms.GroupBox();
			txtAudioOutputFolder = new System.Windows.Forms.TextBox();
			lblAudioExportPath = new System.Windows.Forms.Label();
			btnAudioOutputFolder = new System.Windows.Forms.Button();
			grpFalcon = new System.Windows.Forms.GroupBox();
			chkCompress = new System.Windows.Forms.CheckBox();
			txtFalconInfo = new System.Windows.Forms.TextBox();
			chkFppIncludeAudio = new System.Windows.Forms.CheckBox();
			chkBackupUniverseFile = new System.Windows.Forms.CheckBox();
			chkCreateUniverseFile = new System.Windows.Forms.CheckBox();
			rdoFilePath = new System.Windows.Forms.RadioButton();
			rdoDirectUpload = new System.Windows.Forms.RadioButton();
			lblFppHostAddress = new System.Windows.Forms.Label();
			txtFppHostAddress = new System.Windows.Forms.TextBox();
			txtFalconOutputFolder = new System.Windows.Forms.TextBox();
			btnFalconUniverseFolder = new System.Windows.Forms.Button();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			groupBox1.SuspendLayout();
			grpSequence.SuspendLayout();
			grpAudio.SuspendLayout();
			grpFalcon.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			groupBox1.AutoSize = true;
			groupBox1.Controls.Add(resolutionComboBox);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(outputFormatComboBox);
			groupBox1.Controls.Add(label3);
			groupBox1.Location = new System.Drawing.Point(3, 23);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(524, 70);
			groupBox1.TabIndex = 14;
			groupBox1.TabStop = false;
			groupBox1.Text = "Export Format";
			groupBox1.Paint += groupBoxes_Paint;
			// 
			// resolutionComboBox
			// 
			resolutionComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			resolutionComboBox.FormattingEnabled = true;
			resolutionComboBox.Items.AddRange(new object[] { "25", "50", "100" });
			resolutionComboBox.Location = new System.Drawing.Point(431, 24);
			resolutionComboBox.Name = "resolutionComboBox";
			resolutionComboBox.Size = new System.Drawing.Size(61, 24);
			resolutionComboBox.TabIndex = 2;
			resolutionComboBox.DrawItem += comboBox_DrawItem;
			resolutionComboBox.TextChanged += resolutionComboBox_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(350, 27);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(75, 15);
			label4.TabIndex = 11;
			label4.Text = "Timing (ms):";
			// 
			// outputFormatComboBox
			// 
			outputFormatComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			outputFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			outputFormatComboBox.FormattingEnabled = true;
			outputFormatComboBox.Location = new System.Drawing.Point(66, 24);
			outputFormatComboBox.Name = "outputFormatComboBox";
			outputFormatComboBox.Size = new System.Drawing.Size(260, 24);
			outputFormatComboBox.TabIndex = 1;
			outputFormatComboBox.DrawItem += comboBox_DrawItem;
			outputFormatComboBox.SelectedIndexChanged += outputFormatComboBox_SelectedIndexChanged;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 27);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(48, 15);
			label3.TabIndex = 9;
			label3.Text = "Format:";
			// 
			// lblChooseOutputFormat
			// 
			lblChooseOutputFormat.AutoSize = true;
			lblChooseOutputFormat.Location = new System.Drawing.Point(3, 0);
			lblChooseOutputFormat.Name = "lblChooseOutputFormat";
			lblChooseOutputFormat.Size = new System.Drawing.Size(279, 15);
			lblChooseOutputFormat.TabIndex = 15;
			lblChooseOutputFormat.Text = "Step 4:   Choose the Output Format and Destination";
			// 
			// btnOuputFolderSelect
			// 
			btnOuputFolderSelect.Location = new System.Drawing.Point(25, 37);
			btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			btnOuputFolderSelect.Size = new System.Drawing.Size(24, 23);
			btnOuputFolderSelect.TabIndex = 8;
			btnOuputFolderSelect.Text = "Output Folder";
			btnOuputFolderSelect.UseVisualStyleBackColor = true;
			btnOuputFolderSelect.Click += btnOuputFolderSelect_Click;
			// 
			// grpSequence
			// 
			grpSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			grpSequence.AutoSize = true;
			grpSequence.Controls.Add(txtOutputFolder);
			grpSequence.Controls.Add(label2);
			grpSequence.Controls.Add(btnOuputFolderSelect);
			grpSequence.Location = new System.Drawing.Point(3, 305);
			grpSequence.Name = "grpSequence";
			grpSequence.Size = new System.Drawing.Size(524, 83);
			grpSequence.TabIndex = 15;
			grpSequence.TabStop = false;
			grpSequence.Text = "Sequence";
			grpSequence.Paint += groupBoxes_Paint;
			// 
			// txtOutputFolder
			// 
			txtOutputFolder.Location = new System.Drawing.Point(55, 38);
			txtOutputFolder.Name = "txtOutputFolder";
			txtOutputFolder.ReadOnly = true;
			txtOutputFolder.Size = new System.Drawing.Size(438, 23);
			txtOutputFolder.TabIndex = 9;
			txtOutputFolder.Leave += txtOutputFolder_Leave;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(22, 19);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(94, 15);
			label2.TabIndex = 21;
			label2.Text = "Sequence Folder";
			// 
			// chkRenameAudio
			// 
			chkRenameAudio.AutoSize = true;
			chkRenameAudio.Location = new System.Drawing.Point(25, 47);
			chkRenameAudio.Name = "chkRenameAudio";
			chkRenameAudio.Size = new System.Drawing.Size(192, 19);
			chkRenameAudio.TabIndex = 11;
			chkRenameAudio.Text = "Rename file to match sequence";
			chkRenameAudio.UseVisualStyleBackColor = true;
			chkRenameAudio.CheckedChanged += chkRenameAudio_CheckedChanged;
			// 
			// chkIncludeAudio
			// 
			chkIncludeAudio.AutoSize = true;
			chkIncludeAudio.Location = new System.Drawing.Point(24, 22);
			chkIncludeAudio.Name = "chkIncludeAudio";
			chkIncludeAudio.Size = new System.Drawing.Size(100, 19);
			chkIncludeAudio.TabIndex = 10;
			chkIncludeAudio.Text = "Include Audio";
			chkIncludeAudio.UseVisualStyleBackColor = true;
			chkIncludeAudio.CheckedChanged += chkIncludeAudio_CheckedChanged;
			// 
			// grpAudio
			// 
			grpAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			grpAudio.AutoSize = true;
			grpAudio.Controls.Add(txtAudioOutputFolder);
			grpAudio.Controls.Add(lblAudioExportPath);
			grpAudio.Controls.Add(btnAudioOutputFolder);
			grpAudio.Controls.Add(chkRenameAudio);
			grpAudio.Controls.Add(chkIncludeAudio);
			grpAudio.Location = new System.Drawing.Point(3, 394);
			grpAudio.Name = "grpAudio";
			grpAudio.Size = new System.Drawing.Size(524, 133);
			grpAudio.TabIndex = 20;
			grpAudio.TabStop = false;
			grpAudio.Text = "Audio";
			grpAudio.Paint += groupBoxes_Paint;
			// 
			// txtAudioOutputFolder
			// 
			txtAudioOutputFolder.Location = new System.Drawing.Point(55, 88);
			txtAudioOutputFolder.Name = "txtAudioOutputFolder";
			txtAudioOutputFolder.ReadOnly = true;
			txtAudioOutputFolder.Size = new System.Drawing.Size(437, 23);
			txtAudioOutputFolder.TabIndex = 12;
			txtAudioOutputFolder.Leave += txtAudioOutputFolder_Leave;
			// 
			// lblAudioExportPath
			// 
			lblAudioExportPath.AutoSize = true;
			lblAudioExportPath.Location = new System.Drawing.Point(22, 69);
			lblAudioExportPath.Name = "lblAudioExportPath";
			lblAudioExportPath.Size = new System.Drawing.Size(75, 15);
			lblAudioExportPath.TabIndex = 20;
			lblAudioExportPath.Text = "Audio Folder";
			// 
			// btnAudioOutputFolder
			// 
			btnAudioOutputFolder.Location = new System.Drawing.Point(24, 87);
			btnAudioOutputFolder.Name = "btnAudioOutputFolder";
			btnAudioOutputFolder.Size = new System.Drawing.Size(24, 23);
			btnAudioOutputFolder.TabIndex = 11;
			btnAudioOutputFolder.Text = "Audio Output Folder";
			btnAudioOutputFolder.UseVisualStyleBackColor = true;
			btnAudioOutputFolder.Click += btnAudioOutputFolder_Click;
			// 
			// grpFalcon
			// 
			grpFalcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			grpFalcon.AutoSize = true;
			grpFalcon.Controls.Add(chkCompress);
			grpFalcon.Controls.Add(txtFalconInfo);
			grpFalcon.Controls.Add(chkFppIncludeAudio);
			grpFalcon.Controls.Add(chkBackupUniverseFile);
			grpFalcon.Controls.Add(chkCreateUniverseFile);
			grpFalcon.Controls.Add(rdoFilePath);
			grpFalcon.Controls.Add(rdoDirectUpload);
			grpFalcon.Controls.Add(lblFppHostAddress);
			grpFalcon.Controls.Add(txtFppHostAddress);
			grpFalcon.Controls.Add(txtFalconOutputFolder);
			grpFalcon.Controls.Add(btnFalconUniverseFolder);
			grpFalcon.Location = new System.Drawing.Point(3, 99);
			grpFalcon.Name = "grpFalcon";
			grpFalcon.Size = new System.Drawing.Size(524, 200);
			grpFalcon.TabIndex = 23;
			grpFalcon.TabStop = false;
			grpFalcon.Text = "Falcon Pi Player 2.x";
			grpFalcon.Paint += groupBoxes_Paint;
			// 
			// chkCompress
			// 
			chkCompress.AutoSize = true;
			chkCompress.Enabled = false;
			chkCompress.Location = new System.Drawing.Point(179, 47);
			chkCompress.Name = "chkCompress";
			chkCompress.Size = new System.Drawing.Size(134, 19);
			chkCompress.TabIndex = 28;
			chkCompress.Text = "Enable Compression";
			chkCompress.UseVisualStyleBackColor = true;
			chkCompress.CheckedChanged += chkCompress_CheckedChanged;
			// 
			// txtFalconInfo
			// 
			txtFalconInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			txtFalconInfo.Cursor = System.Windows.Forms.Cursors.Default;
			txtFalconInfo.Location = new System.Drawing.Point(24, 76);
			txtFalconInfo.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
			txtFalconInfo.Multiline = true;
			txtFalconInfo.Name = "txtFalconInfo";
			txtFalconInfo.ReadOnly = true;
			txtFalconInfo.Size = new System.Drawing.Size(466, 45);
			txtFalconInfo.TabIndex = 27;
			txtFalconInfo.TabStop = false;
			txtFalconInfo.Text = ("Output folder or FPP network location. To save directly to FPP, select the mapped" + " media folder or network path in the form of \\\\hostname\\fpp or \\\\ip\\fpp");
			// 
			// chkFppIncludeAudio
			// 
			chkFppIncludeAudio.AutoSize = true;
			chkFppIncludeAudio.Location = new System.Drawing.Point(25, 47);
			chkFppIncludeAudio.Name = "chkFppIncludeAudio";
			chkFppIncludeAudio.Size = new System.Drawing.Size(100, 19);
			chkFppIncludeAudio.TabIndex = 5;
			chkFppIncludeAudio.Text = "Include Audio";
			chkFppIncludeAudio.UseVisualStyleBackColor = true;
			chkFppIncludeAudio.CheckedChanged += chkIncludeAudio_CheckedChanged;
			// 
			// chkBackupUniverseFile
			// 
			chkBackupUniverseFile.AutoSize = true;
			chkBackupUniverseFile.Location = new System.Drawing.Point(179, 22);
			chkBackupUniverseFile.Name = "chkBackupUniverseFile";
			chkBackupUniverseFile.Size = new System.Drawing.Size(134, 19);
			chkBackupUniverseFile.TabIndex = 4;
			chkBackupUniverseFile.Text = "Backup Universe File";
			chkBackupUniverseFile.UseVisualStyleBackColor = true;
			chkBackupUniverseFile.CheckedChanged += chkBackupUniverseFile_CheckedChanged;
			// 
			// chkCreateUniverseFile
			// 
			chkCreateUniverseFile.AutoSize = true;
			chkCreateUniverseFile.Location = new System.Drawing.Point(25, 22);
			chkCreateUniverseFile.Name = "chkCreateUniverseFile";
			chkCreateUniverseFile.Size = new System.Drawing.Size(129, 19);
			chkCreateUniverseFile.TabIndex = 3;
			chkCreateUniverseFile.Text = "Create Universe File";
			chkCreateUniverseFile.UseVisualStyleBackColor = true;
			chkCreateUniverseFile.CheckedChanged += chkCreateUniverseFile_CheckedChanged;
			// 
			// rdoFilePath
			// 
			rdoFilePath.AutoSize = true;
			rdoFilePath.Checked = true;
			rdoFilePath.Location = new System.Drawing.Point(24, 130);
			rdoFilePath.Name = "rdoFilePath";
			rdoFilePath.Size = new System.Drawing.Size(70, 19);
			rdoFilePath.TabIndex = 20;
			rdoFilePath.TabStop = true;
			rdoFilePath.Text = "File Path";
			rdoFilePath.UseVisualStyleBackColor = true;
			rdoFilePath.CheckedChanged += rdoUploadMode_CheckedChanged;
			// 
			// rdoDirectUpload
			// 
			rdoDirectUpload.AutoSize = true;
			rdoDirectUpload.Location = new System.Drawing.Point(120, 130);
			rdoDirectUpload.Name = "rdoDirectUpload";
			rdoDirectUpload.Size = new System.Drawing.Size(97, 19);
			rdoDirectUpload.TabIndex = 21;
			rdoDirectUpload.Text = "Direct Upload";
			rdoDirectUpload.UseVisualStyleBackColor = true;
			rdoDirectUpload.CheckedChanged += rdoUploadMode_CheckedChanged;
			// 
			// lblFppHostAddress
			// 
			lblFppHostAddress.AutoSize = true;
			lblFppHostAddress.Location = new System.Drawing.Point(23, 159);
			lblFppHostAddress.Name = "lblFppHostAddress";
			lblFppHostAddress.Size = new System.Drawing.Size(79, 15);
			lblFppHostAddress.TabIndex = 22;
			lblFppHostAddress.Text = "FPP Host / IP:";
			lblFppHostAddress.Visible = false;
			// 
			// txtFppHostAddress
			// 
			txtFppHostAddress.Location = new System.Drawing.Point(120, 155);
			txtFppHostAddress.Name = "txtFppHostAddress";
			txtFppHostAddress.Size = new System.Drawing.Size(372, 23);
			txtFppHostAddress.TabIndex = 23;
			txtFppHostAddress.Visible = false;
			txtFppHostAddress.TextChanged += txtFppHostAddress_TextChanged;
			txtFppHostAddress.Leave += txtFppHostAddress_Leave;
			// 
			// txtFalconOutputFolder
			// 
			txtFalconOutputFolder.Location = new System.Drawing.Point(54, 155);
			txtFalconOutputFolder.Name = "txtFalconOutputFolder";
			txtFalconOutputFolder.Size = new System.Drawing.Size(438, 23);
			txtFalconOutputFolder.TabIndex = 7;
			txtFalconOutputFolder.TextChanged += txtFalconOutputFolder_TextChanged;
			txtFalconOutputFolder.Leave += txtFalconOutputFolder_Leave;
			// 
			// btnFalconUniverseFolder
			// 
			btnFalconUniverseFolder.Location = new System.Drawing.Point(23, 155);
			btnFalconUniverseFolder.Name = "btnFalconUniverseFolder";
			btnFalconUniverseFolder.Size = new System.Drawing.Size(24, 23);
			btnFalconUniverseFolder.TabIndex = 6;
			btnFalconUniverseFolder.Text = "Output Folder";
			btnFalconUniverseFolder.UseVisualStyleBackColor = true;
			btnFalconUniverseFolder.Click += btnFalconOutputFolder_Click;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(lblChooseOutputFormat, 0, 0);
			tableLayoutPanel1.Controls.Add(grpAudio, 0, 4);
			tableLayoutPanel1.Controls.Add(grpSequence, 0, 3);
			tableLayoutPanel1.Controls.Add(groupBox1, 0, 1);
			tableLayoutPanel1.Controls.Add(grpFalcon, 0, 2);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 5;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(530, 530);
			tableLayoutPanel1.TabIndex = 24;
			// 
			// BulkExportOutputFormatStage
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			Controls.Add(tableLayoutPanel1);
			Size = new System.Drawing.Size(530, 530);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			grpSequence.ResumeLayout(false);
			grpSequence.PerformLayout();
			grpAudio.ResumeLayout(false);
			grpAudio.PerformLayout();
			grpFalcon.ResumeLayout(false);
			grpFalcon.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox resolutionComboBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox outputFormatComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblChooseOutputFormat;
		private System.Windows.Forms.Button btnOuputFolderSelect;
		private System.Windows.Forms.GroupBox grpSequence;
		private System.Windows.Forms.CheckBox chkRenameAudio;
		private System.Windows.Forms.CheckBox chkIncludeAudio;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox grpAudio;
		private System.Windows.Forms.Label lblAudioExportPath;
		private System.Windows.Forms.Button btnAudioOutputFolder;
		private System.Windows.Forms.TextBox txtOutputFolder;
		private System.Windows.Forms.TextBox txtAudioOutputFolder;
		private System.Windows.Forms.GroupBox grpFalcon;
		private System.Windows.Forms.TextBox txtFalconOutputFolder;
		private System.Windows.Forms.Button btnFalconUniverseFolder;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox chkCreateUniverseFile;
		private System.Windows.Forms.CheckBox chkBackupUniverseFile;
		private System.Windows.Forms.CheckBox chkFppIncludeAudio;
		private System.Windows.Forms.TextBox txtFalconInfo;
		private System.Windows.Forms.CheckBox chkCompress;
		private System.Windows.Forms.RadioButton rdoFilePath;
		private System.Windows.Forms.RadioButton rdoDirectUpload;
		private System.Windows.Forms.Label lblFppHostAddress;
		private System.Windows.Forms.TextBox txtFppHostAddress;
	}
}
