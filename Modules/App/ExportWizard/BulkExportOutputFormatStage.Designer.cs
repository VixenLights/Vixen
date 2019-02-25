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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.resolutionComboBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.outputFormatComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lblChooseOutputFormat = new System.Windows.Forms.Label();
			this.btnOuputFolderSelect = new System.Windows.Forms.Button();
			this.grpSequence = new System.Windows.Forms.GroupBox();
			this.txtOutputFolder = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkRenameAudio = new System.Windows.Forms.CheckBox();
			this.chkIncludeAudio = new System.Windows.Forms.CheckBox();
			this.grpAudio = new System.Windows.Forms.GroupBox();
			this.txtAudioOutputFolder = new System.Windows.Forms.TextBox();
			this.lblAudioExportPath = new System.Windows.Forms.Label();
			this.btnAudioOutputFolder = new System.Windows.Forms.Button();
			this.grpFalcon = new System.Windows.Forms.GroupBox();
			this.txtFalconInfo = new System.Windows.Forms.TextBox();
			this.chkFppIncludeAudio = new System.Windows.Forms.CheckBox();
			this.chkBackupUniverseFile = new System.Windows.Forms.CheckBox();
			this.chkCreateUniverseFile = new System.Windows.Forms.CheckBox();
			this.txtFalconOutputFolder = new System.Windows.Forms.TextBox();
			this.btnFalconUniverseFolder = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1.SuspendLayout();
			this.grpSequence.SuspendLayout();
			this.grpAudio.SuspendLayout();
			this.grpFalcon.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.Controls.Add(this.resolutionComboBox);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.outputFormatComboBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox1.Location = new System.Drawing.Point(3, 23);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(498, 70);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Export Format";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// resolutionComboBox
			// 
			this.resolutionComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.resolutionComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.resolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.resolutionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.resolutionComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.resolutionComboBox.FormattingEnabled = true;
			this.resolutionComboBox.Items.AddRange(new object[] {
            "25",
            "50",
            "100"});
			this.resolutionComboBox.Location = new System.Drawing.Point(431, 24);
			this.resolutionComboBox.Name = "resolutionComboBox";
			this.resolutionComboBox.Size = new System.Drawing.Size(61, 24);
			this.resolutionComboBox.TabIndex = 2;
			this.resolutionComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.resolutionComboBox.SelectedIndexChanged += new System.EventHandler(this.resolutionComboBox_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label4.Location = new System.Drawing.Point(350, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 15);
			this.label4.TabIndex = 11;
			this.label4.Text = "Timing (ms):";
			// 
			// outputFormatComboBox
			// 
			this.outputFormatComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.outputFormatComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.outputFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.outputFormatComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.outputFormatComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.outputFormatComboBox.FormattingEnabled = true;
			this.outputFormatComboBox.Location = new System.Drawing.Point(66, 24);
			this.outputFormatComboBox.Name = "outputFormatComboBox";
			this.outputFormatComboBox.Size = new System.Drawing.Size(260, 24);
			this.outputFormatComboBox.TabIndex = 1;
			this.outputFormatComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.outputFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.outputFormatComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label3.Location = new System.Drawing.Point(12, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 15);
			this.label3.TabIndex = 9;
			this.label3.Text = "Format:";
			// 
			// lblChooseOutputFormat
			// 
			this.lblChooseOutputFormat.AutoSize = true;
			this.lblChooseOutputFormat.Location = new System.Drawing.Point(3, 0);
			this.lblChooseOutputFormat.Name = "lblChooseOutputFormat";
			this.lblChooseOutputFormat.Size = new System.Drawing.Size(279, 15);
			this.lblChooseOutputFormat.TabIndex = 15;
			this.lblChooseOutputFormat.Text = "Step 4:   Choose the Output Format and Destination";
			// 
			// btnOuputFolderSelect
			// 
			this.btnOuputFolderSelect.Location = new System.Drawing.Point(25, 37);
			this.btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			this.btnOuputFolderSelect.Size = new System.Drawing.Size(24, 23);
			this.btnOuputFolderSelect.TabIndex = 8;
			this.btnOuputFolderSelect.Text = "Output Folder";
			this.btnOuputFolderSelect.UseVisualStyleBackColor = true;
			this.btnOuputFolderSelect.Click += new System.EventHandler(this.btnOuputFolderSelect_Click);
			// 
			// grpSequence
			// 
			this.grpSequence.AutoSize = true;
			this.grpSequence.Controls.Add(this.txtOutputFolder);
			this.grpSequence.Controls.Add(this.label2);
			this.grpSequence.Controls.Add(this.btnOuputFolderSelect);
			this.grpSequence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpSequence.Location = new System.Drawing.Point(3, 285);
			this.grpSequence.Name = "grpSequence";
			this.grpSequence.Size = new System.Drawing.Size(499, 83);
			this.grpSequence.TabIndex = 15;
			this.grpSequence.TabStop = false;
			this.grpSequence.Text = "Sequence";
			this.grpSequence.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtOutputFolder
			// 
			this.txtOutputFolder.Location = new System.Drawing.Point(55, 38);
			this.txtOutputFolder.Name = "txtOutputFolder";
			this.txtOutputFolder.ReadOnly = true;
			this.txtOutputFolder.Size = new System.Drawing.Size(438, 23);
			this.txtOutputFolder.TabIndex = 9;
			this.txtOutputFolder.Leave += new System.EventHandler(this.txtOutputFolder_Leave);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 15);
			this.label2.TabIndex = 21;
			this.label2.Text = "Sequence Folder";
			// 
			// chkRenameAudio
			// 
			this.chkRenameAudio.AutoSize = true;
			this.chkRenameAudio.Location = new System.Drawing.Point(25, 47);
			this.chkRenameAudio.Name = "chkRenameAudio";
			this.chkRenameAudio.Size = new System.Drawing.Size(192, 19);
			this.chkRenameAudio.TabIndex = 11;
			this.chkRenameAudio.Text = "Rename file to match sequence";
			this.chkRenameAudio.UseVisualStyleBackColor = true;
			this.chkRenameAudio.CheckedChanged += new System.EventHandler(this.chkRenameAudio_CheckedChanged);
			// 
			// chkIncludeAudio
			// 
			this.chkIncludeAudio.AutoSize = true;
			this.chkIncludeAudio.Location = new System.Drawing.Point(24, 22);
			this.chkIncludeAudio.Name = "chkIncludeAudio";
			this.chkIncludeAudio.Size = new System.Drawing.Size(100, 19);
			this.chkIncludeAudio.TabIndex = 10;
			this.chkIncludeAudio.Text = "Include Audio";
			this.chkIncludeAudio.UseVisualStyleBackColor = true;
			this.chkIncludeAudio.CheckedChanged += new System.EventHandler(this.chkIncludeAudio_CheckedChanged);
			// 
			// grpAudio
			// 
			this.grpAudio.AutoSize = true;
			this.grpAudio.Controls.Add(this.txtAudioOutputFolder);
			this.grpAudio.Controls.Add(this.lblAudioExportPath);
			this.grpAudio.Controls.Add(this.btnAudioOutputFolder);
			this.grpAudio.Controls.Add(this.chkRenameAudio);
			this.grpAudio.Controls.Add(this.chkIncludeAudio);
			this.grpAudio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpAudio.Location = new System.Drawing.Point(3, 374);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new System.Drawing.Size(498, 133);
			this.grpAudio.TabIndex = 20;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			this.grpAudio.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtAudioOutputFolder
			// 
			this.txtAudioOutputFolder.Location = new System.Drawing.Point(55, 88);
			this.txtAudioOutputFolder.Name = "txtAudioOutputFolder";
			this.txtAudioOutputFolder.ReadOnly = true;
			this.txtAudioOutputFolder.Size = new System.Drawing.Size(437, 23);
			this.txtAudioOutputFolder.TabIndex = 12;
			this.txtAudioOutputFolder.Leave += new System.EventHandler(this.txtAudioOutputFolder_Leave);
			// 
			// lblAudioExportPath
			// 
			this.lblAudioExportPath.AutoSize = true;
			this.lblAudioExportPath.Location = new System.Drawing.Point(22, 69);
			this.lblAudioExportPath.Name = "lblAudioExportPath";
			this.lblAudioExportPath.Size = new System.Drawing.Size(75, 15);
			this.lblAudioExportPath.TabIndex = 20;
			this.lblAudioExportPath.Text = "Audio Folder";
			// 
			// btnAudioOutputFolder
			// 
			this.btnAudioOutputFolder.Location = new System.Drawing.Point(24, 87);
			this.btnAudioOutputFolder.Name = "btnAudioOutputFolder";
			this.btnAudioOutputFolder.Size = new System.Drawing.Size(24, 23);
			this.btnAudioOutputFolder.TabIndex = 11;
			this.btnAudioOutputFolder.Text = "Audio Output Folder";
			this.btnAudioOutputFolder.UseVisualStyleBackColor = true;
			this.btnAudioOutputFolder.Click += new System.EventHandler(this.btnAudioOutputFolder_Click);
			// 
			// grpFalcon
			// 
			this.grpFalcon.AutoSize = true;
			this.grpFalcon.Controls.Add(this.txtFalconInfo);
			this.grpFalcon.Controls.Add(this.chkFppIncludeAudio);
			this.grpFalcon.Controls.Add(this.chkBackupUniverseFile);
			this.grpFalcon.Controls.Add(this.chkCreateUniverseFile);
			this.grpFalcon.Controls.Add(this.txtFalconOutputFolder);
			this.grpFalcon.Controls.Add(this.btnFalconUniverseFolder);
			this.grpFalcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpFalcon.Location = new System.Drawing.Point(3, 99);
			this.grpFalcon.Name = "grpFalcon";
			this.grpFalcon.Size = new System.Drawing.Size(498, 180);
			this.grpFalcon.TabIndex = 23;
			this.grpFalcon.TabStop = false;
			this.grpFalcon.Text = "Falcon Pi Player 2.x";
			this.grpFalcon.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtFalconInfo
			// 
			this.txtFalconInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtFalconInfo.Cursor = System.Windows.Forms.Cursors.Default;
			this.txtFalconInfo.Location = new System.Drawing.Point(24, 76);
			this.txtFalconInfo.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
			this.txtFalconInfo.Multiline = true;
			this.txtFalconInfo.Name = "txtFalconInfo";
			this.txtFalconInfo.ReadOnly = true;
			this.txtFalconInfo.Size = new System.Drawing.Size(466, 45);
			this.txtFalconInfo.TabIndex = 27;
			this.txtFalconInfo.TabStop = false;
			this.txtFalconInfo.Text = "Output folder or FPP network location. To save directly to FPP, select the mapped" +
    " media folder or network path in the form of \\\\hostname\\fpp or \\\\ip\\fpp";
			// 
			// chkFppIncludeAudio
			// 
			this.chkFppIncludeAudio.AutoSize = true;
			this.chkFppIncludeAudio.Location = new System.Drawing.Point(25, 47);
			this.chkFppIncludeAudio.Name = "chkFppIncludeAudio";
			this.chkFppIncludeAudio.Size = new System.Drawing.Size(100, 19);
			this.chkFppIncludeAudio.TabIndex = 5;
			this.chkFppIncludeAudio.Text = "Include Audio";
			this.chkFppIncludeAudio.UseVisualStyleBackColor = true;
			this.chkFppIncludeAudio.CheckedChanged += new System.EventHandler(this.chkIncludeAudio_CheckedChanged);
			// 
			// chkBackupUniverseFile
			// 
			this.chkBackupUniverseFile.AutoSize = true;
			this.chkBackupUniverseFile.Location = new System.Drawing.Point(179, 22);
			this.chkBackupUniverseFile.Name = "chkBackupUniverseFile";
			this.chkBackupUniverseFile.Size = new System.Drawing.Size(134, 19);
			this.chkBackupUniverseFile.TabIndex = 4;
			this.chkBackupUniverseFile.Text = "Backup Universe File";
			this.chkBackupUniverseFile.UseVisualStyleBackColor = true;
			this.chkBackupUniverseFile.CheckedChanged += new System.EventHandler(this.chkBackupUniverseFile_CheckedChanged);
			// 
			// chkCreateUniverseFile
			// 
			this.chkCreateUniverseFile.AutoSize = true;
			this.chkCreateUniverseFile.Location = new System.Drawing.Point(25, 22);
			this.chkCreateUniverseFile.Name = "chkCreateUniverseFile";
			this.chkCreateUniverseFile.Size = new System.Drawing.Size(129, 19);
			this.chkCreateUniverseFile.TabIndex = 3;
			this.chkCreateUniverseFile.Text = "Create Universe File";
			this.chkCreateUniverseFile.UseVisualStyleBackColor = true;
			this.chkCreateUniverseFile.CheckedChanged += new System.EventHandler(this.chkCreateUniverseFile_CheckedChanged);
			// 
			// txtFalconOutputFolder
			// 
			this.txtFalconOutputFolder.Location = new System.Drawing.Point(54, 135);
			this.txtFalconOutputFolder.Name = "txtFalconOutputFolder";
			this.txtFalconOutputFolder.Size = new System.Drawing.Size(438, 23);
			this.txtFalconOutputFolder.TabIndex = 7;
			this.txtFalconOutputFolder.TextChanged += new System.EventHandler(this.txtFalconOutputFolder_TextChanged);
			this.txtFalconOutputFolder.Leave += new System.EventHandler(this.txtFalconOutputFolder_Leave);
			// 
			// btnFalconUniverseFolder
			// 
			this.btnFalconUniverseFolder.Location = new System.Drawing.Point(23, 135);
			this.btnFalconUniverseFolder.Name = "btnFalconUniverseFolder";
			this.btnFalconUniverseFolder.Size = new System.Drawing.Size(24, 23);
			this.btnFalconUniverseFolder.TabIndex = 6;
			this.btnFalconUniverseFolder.Text = "Output Folder";
			this.btnFalconUniverseFolder.UseVisualStyleBackColor = true;
			this.btnFalconUniverseFolder.Click += new System.EventHandler(this.btnFalconOutputFolder_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lblChooseOutputFormat, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.grpAudio, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.grpSequence, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.grpFalcon, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(509, 510);
			this.tableLayoutPanel1.TabIndex = 24;
			// 
			// BulkExportOutputFormatStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BulkExportOutputFormatStage";
			this.Size = new System.Drawing.Size(509, 510);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.grpSequence.ResumeLayout(false);
			this.grpSequence.PerformLayout();
			this.grpAudio.ResumeLayout(false);
			this.grpAudio.PerformLayout();
			this.grpFalcon.ResumeLayout(false);
			this.grpFalcon.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}
