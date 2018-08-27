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
			this.chkFppIncludeAudio = new System.Windows.Forms.CheckBox();
			this.chkBackupUniverseFile = new System.Windows.Forms.CheckBox();
			this.chkCreateUniverseFile = new System.Windows.Forms.CheckBox();
			this.txtFalconOutputFolder = new System.Windows.Forms.TextBox();
			this.lblFppUniverse = new System.Windows.Forms.Label();
			this.btnFalconUniverseFolder = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.txtFalconInfo = new System.Windows.Forms.TextBox();
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
			this.groupBox1.Location = new System.Drawing.Point(6, 49);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.groupBox1.Size = new System.Drawing.Size(922, 135);
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
			this.resolutionComboBox.Location = new System.Drawing.Point(800, 51);
			this.resolutionComboBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.resolutionComboBox.Name = "resolutionComboBox";
			this.resolutionComboBox.Size = new System.Drawing.Size(110, 40);
			this.resolutionComboBox.TabIndex = 2;
			this.resolutionComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.resolutionComboBox.SelectedIndexChanged += new System.EventHandler(this.resolutionComboBox_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label4.Location = new System.Drawing.Point(650, 58);
			this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(146, 32);
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
			this.outputFormatComboBox.Location = new System.Drawing.Point(123, 51);
			this.outputFormatComboBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.outputFormatComboBox.Name = "outputFormatComboBox";
			this.outputFormatComboBox.Size = new System.Drawing.Size(479, 40);
			this.outputFormatComboBox.TabIndex = 1;
			this.outputFormatComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.outputFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.outputFormatComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label3.Location = new System.Drawing.Point(22, 58);
			this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 32);
			this.label3.TabIndex = 9;
			this.label3.Text = "Format:";
			// 
			// lblChooseOutputFormat
			// 
			this.lblChooseOutputFormat.AutoSize = true;
			this.lblChooseOutputFormat.Location = new System.Drawing.Point(6, 0);
			this.lblChooseOutputFormat.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblChooseOutputFormat.Name = "lblChooseOutputFormat";
			this.lblChooseOutputFormat.Size = new System.Drawing.Size(571, 32);
			this.lblChooseOutputFormat.TabIndex = 15;
			this.lblChooseOutputFormat.Text = "Step 4:   Choose the Output Format and Destination";
			// 
			// btnOuputFolderSelect
			// 
			this.btnOuputFolderSelect.Location = new System.Drawing.Point(46, 79);
			this.btnOuputFolderSelect.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			this.btnOuputFolderSelect.Size = new System.Drawing.Size(45, 49);
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
			this.grpSequence.Location = new System.Drawing.Point(6, 602);
			this.grpSequence.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpSequence.Name = "grpSequence";
			this.grpSequence.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpSequence.Size = new System.Drawing.Size(924, 172);
			this.grpSequence.TabIndex = 15;
			this.grpSequence.TabStop = false;
			this.grpSequence.Text = "Sequence";
			this.grpSequence.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtOutputFolder
			// 
			this.txtOutputFolder.Location = new System.Drawing.Point(102, 81);
			this.txtOutputFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.txtOutputFolder.Name = "txtOutputFolder";
			this.txtOutputFolder.ReadOnly = true;
			this.txtOutputFolder.Size = new System.Drawing.Size(810, 39);
			this.txtOutputFolder.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(41, 41);
			this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(194, 32);
			this.label2.TabIndex = 21;
			this.label2.Text = "Sequence Folder";
			// 
			// chkRenameAudio
			// 
			this.chkRenameAudio.AutoSize = true;
			this.chkRenameAudio.Location = new System.Drawing.Point(46, 100);
			this.chkRenameAudio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.chkRenameAudio.Name = "chkRenameAudio";
			this.chkRenameAudio.Size = new System.Drawing.Size(384, 36);
			this.chkRenameAudio.TabIndex = 11;
			this.chkRenameAudio.Text = "Rename file to match sequence";
			this.chkRenameAudio.UseVisualStyleBackColor = true;
			this.chkRenameAudio.CheckedChanged += new System.EventHandler(this.chkRenameAudio_CheckedChanged);
			// 
			// chkIncludeAudio
			// 
			this.chkIncludeAudio.AutoSize = true;
			this.chkIncludeAudio.Location = new System.Drawing.Point(45, 47);
			this.chkIncludeAudio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.chkIncludeAudio.Name = "chkIncludeAudio";
			this.chkIncludeAudio.Size = new System.Drawing.Size(195, 36);
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
			this.grpAudio.Location = new System.Drawing.Point(6, 786);
			this.grpAudio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpAudio.Size = new System.Drawing.Size(922, 279);
			this.grpAudio.TabIndex = 20;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			this.grpAudio.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtAudioOutputFolder
			// 
			this.txtAudioOutputFolder.Location = new System.Drawing.Point(102, 188);
			this.txtAudioOutputFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.txtAudioOutputFolder.Name = "txtAudioOutputFolder";
			this.txtAudioOutputFolder.ReadOnly = true;
			this.txtAudioOutputFolder.Size = new System.Drawing.Size(808, 39);
			this.txtAudioOutputFolder.TabIndex = 12;
			// 
			// lblAudioExportPath
			// 
			this.lblAudioExportPath.AutoSize = true;
			this.lblAudioExportPath.Location = new System.Drawing.Point(41, 147);
			this.lblAudioExportPath.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblAudioExportPath.Name = "lblAudioExportPath";
			this.lblAudioExportPath.Size = new System.Drawing.Size(152, 32);
			this.lblAudioExportPath.TabIndex = 20;
			this.lblAudioExportPath.Text = "Audio Folder";
			// 
			// btnAudioOutputFolder
			// 
			this.btnAudioOutputFolder.Location = new System.Drawing.Point(45, 186);
			this.btnAudioOutputFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAudioOutputFolder.Name = "btnAudioOutputFolder";
			this.btnAudioOutputFolder.Size = new System.Drawing.Size(45, 49);
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
			this.grpFalcon.Controls.Add(this.lblFppUniverse);
			this.grpFalcon.Controls.Add(this.btnFalconUniverseFolder);
			this.grpFalcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpFalcon.Location = new System.Drawing.Point(6, 196);
			this.grpFalcon.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpFalcon.Name = "grpFalcon";
			this.grpFalcon.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.grpFalcon.Size = new System.Drawing.Size(924, 394);
			this.grpFalcon.TabIndex = 23;
			this.grpFalcon.TabStop = false;
			this.grpFalcon.Text = "Falcon Pi Player";
			this.grpFalcon.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// chkFppIncludeAudio
			// 
			this.chkFppIncludeAudio.AutoSize = true;
			this.chkFppIncludeAudio.Location = new System.Drawing.Point(332, 47);
			this.chkFppIncludeAudio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.chkFppIncludeAudio.Name = "chkFppIncludeAudio";
			this.chkFppIncludeAudio.Size = new System.Drawing.Size(195, 36);
			this.chkFppIncludeAudio.TabIndex = 5;
			this.chkFppIncludeAudio.Text = "Include Audio";
			this.chkFppIncludeAudio.UseVisualStyleBackColor = true;
			this.chkFppIncludeAudio.CheckedChanged += new System.EventHandler(this.chkIncludeAudio_CheckedChanged);
			// 
			// chkBackupUniverseFile
			// 
			this.chkBackupUniverseFile.AutoSize = true;
			this.chkBackupUniverseFile.Location = new System.Drawing.Point(45, 100);
			this.chkBackupUniverseFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.chkBackupUniverseFile.Name = "chkBackupUniverseFile";
			this.chkBackupUniverseFile.Size = new System.Drawing.Size(267, 36);
			this.chkBackupUniverseFile.TabIndex = 4;
			this.chkBackupUniverseFile.Text = "Backup Universe File";
			this.chkBackupUniverseFile.UseVisualStyleBackColor = true;
			this.chkBackupUniverseFile.CheckedChanged += new System.EventHandler(this.chkBackupUniverseFile_CheckedChanged);
			// 
			// chkCreateUniverseFile
			// 
			this.chkCreateUniverseFile.AutoSize = true;
			this.chkCreateUniverseFile.Location = new System.Drawing.Point(45, 47);
			this.chkCreateUniverseFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.chkCreateUniverseFile.Name = "chkCreateUniverseFile";
			this.chkCreateUniverseFile.Size = new System.Drawing.Size(259, 36);
			this.chkCreateUniverseFile.TabIndex = 3;
			this.chkCreateUniverseFile.Text = "Create Universe File";
			this.chkCreateUniverseFile.UseVisualStyleBackColor = true;
			this.chkCreateUniverseFile.CheckedChanged += new System.EventHandler(this.chkCreateUniverseFile_CheckedChanged);
			// 
			// txtFalconOutputFolder
			// 
			this.txtFalconOutputFolder.Location = new System.Drawing.Point(102, 301);
			this.txtFalconOutputFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.txtFalconOutputFolder.Name = "txtFalconOutputFolder";
			this.txtFalconOutputFolder.Size = new System.Drawing.Size(810, 39);
			this.txtFalconOutputFolder.TabIndex = 7;
			this.txtFalconOutputFolder.TextChanged += new System.EventHandler(this.txtFalconOutputFolder_TextChanged);
			this.txtFalconOutputFolder.Leave += new System.EventHandler(this.txtFalconOutputFolder_Leave);
			// 
			// lblFppUniverse
			// 
			this.lblFppUniverse.AutoSize = true;
			this.lblFppUniverse.Location = new System.Drawing.Point(41, 94);
			this.lblFppUniverse.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblFppUniverse.Name = "lblFppUniverse";
			this.lblFppUniverse.Size = new System.Drawing.Size(225, 32);
			this.lblFppUniverse.TabIndex = 21;
			this.lblFppUniverse.Text = "Universe File Folder";
			// 
			// btnFalconUniverseFolder
			// 
			this.btnFalconUniverseFolder.Location = new System.Drawing.Point(45, 301);
			this.btnFalconUniverseFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnFalconUniverseFolder.Name = "btnFalconUniverseFolder";
			this.btnFalconUniverseFolder.Size = new System.Drawing.Size(45, 49);
			this.btnFalconUniverseFolder.TabIndex = 6;
			this.btnFalconUniverseFolder.Text = "Output Folder";
			this.btnFalconUniverseFolder.UseVisualStyleBackColor = true;
			this.btnFalconUniverseFolder.Click += new System.EventHandler(this.btnFalconOutputFolder_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lblChooseOutputFormat, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.grpAudio, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.grpSequence, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.grpFalcon, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(945, 996);
			this.tableLayoutPanel1.TabIndex = 24;
			// 
			// txtFalconInfo
			// 
			this.txtFalconInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtFalconInfo.Cursor = System.Windows.Forms.Cursors.Default;
			this.txtFalconInfo.Location = new System.Drawing.Point(45, 162);
			this.txtFalconInfo.Multiline = true;
			this.txtFalconInfo.Name = "txtFalconInfo";
			this.txtFalconInfo.ReadOnly = true;
			this.txtFalconInfo.Size = new System.Drawing.Size(865, 109);
			this.txtFalconInfo.TabIndex = 27;
			this.txtFalconInfo.TabStop = false;
			this.txtFalconInfo.Text = "Output folder or FPP network location. To save directly to FPP, select the mapped" +
    " media folder or network path in the form of \\\\hostname\\fpp\\media or \\\\ip\\fpp\\me" +
    "dia";
			// 
			// BulkExportOutputFormatStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.Name = "BulkExportOutputFormatStage";
			this.Size = new System.Drawing.Size(945, 996);
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
		private System.Windows.Forms.Label lblFppUniverse;
		private System.Windows.Forms.Button btnFalconUniverseFolder;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox chkCreateUniverseFile;
		private System.Windows.Forms.CheckBox chkBackupUniverseFile;
		private System.Windows.Forms.CheckBox chkFppIncludeAudio;
		private System.Windows.Forms.TextBox txtFalconInfo;
	}
}
