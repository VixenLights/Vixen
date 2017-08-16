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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtOutputFolder = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkRenameAudio = new System.Windows.Forms.CheckBox();
			this.chkIncludeAudio = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtAudioOutputFolder = new System.Windows.Forms.TextBox();
			this.lblAudioExportPath = new System.Windows.Forms.Label();
			this.btnAudioOutputFolder = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
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
			this.groupBox1.Location = new System.Drawing.Point(6, 44);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(533, 70);
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
			this.resolutionComboBox.TabIndex = 12;
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
			this.outputFormatComboBox.TabIndex = 10;
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
			this.lblChooseOutputFormat.Location = new System.Drawing.Point(3, 15);
			this.lblChooseOutputFormat.Name = "lblChooseOutputFormat";
			this.lblChooseOutputFormat.Size = new System.Drawing.Size(279, 15);
			this.lblChooseOutputFormat.TabIndex = 15;
			this.lblChooseOutputFormat.Text = "Step 4:   Choose the Output Format and Destination";
			// 
			// btnOuputFolderSelect
			// 
			this.btnOuputFolderSelect.Location = new System.Drawing.Point(24, 49);
			this.btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			this.btnOuputFolderSelect.Size = new System.Drawing.Size(24, 23);
			this.btnOuputFolderSelect.TabIndex = 16;
			this.btnOuputFolderSelect.Text = "Output Folder";
			this.btnOuputFolderSelect.UseVisualStyleBackColor = true;
			this.btnOuputFolderSelect.Click += new System.EventHandler(this.btnOuputFolderSelect_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.AutoSize = true;
			this.groupBox2.Controls.Add(this.txtOutputFolder);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.btnOuputFolderSelect);
			this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox2.Location = new System.Drawing.Point(6, 120);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(533, 96);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sequence";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtOutputFolder
			// 
			this.txtOutputFolder.Location = new System.Drawing.Point(54, 51);
			this.txtOutputFolder.Name = "txtOutputFolder";
			this.txtOutputFolder.ReadOnly = true;
			this.txtOutputFolder.Size = new System.Drawing.Size(438, 23);
			this.txtOutputFolder.TabIndex = 22;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 15);
			this.label2.TabIndex = 21;
			this.label2.Text = "Sequence Folder";
			// 
			// chkRenameAudio
			// 
			this.chkRenameAudio.AutoSize = true;
			this.chkRenameAudio.Location = new System.Drawing.Point(24, 47);
			this.chkRenameAudio.Name = "chkRenameAudio";
			this.chkRenameAudio.Size = new System.Drawing.Size(192, 19);
			this.chkRenameAudio.TabIndex = 19;
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
			this.chkIncludeAudio.TabIndex = 18;
			this.chkIncludeAudio.Text = "Include Audio";
			this.chkIncludeAudio.UseVisualStyleBackColor = true;
			this.chkIncludeAudio.CheckedChanged += new System.EventHandler(this.chkIncludeAudio_CheckedChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.AutoSize = true;
			this.groupBox3.Controls.Add(this.txtAudioOutputFolder);
			this.groupBox3.Controls.Add(this.lblAudioExportPath);
			this.groupBox3.Controls.Add(this.btnAudioOutputFolder);
			this.groupBox3.Controls.Add(this.chkRenameAudio);
			this.groupBox3.Controls.Add(this.chkIncludeAudio);
			this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox3.Location = new System.Drawing.Point(6, 222);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(533, 149);
			this.groupBox3.TabIndex = 20;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Audio";
			this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtAudioOutputFolder
			// 
			this.txtAudioOutputFolder.Location = new System.Drawing.Point(55, 102);
			this.txtAudioOutputFolder.Name = "txtAudioOutputFolder";
			this.txtAudioOutputFolder.ReadOnly = true;
			this.txtAudioOutputFolder.Size = new System.Drawing.Size(437, 23);
			this.txtAudioOutputFolder.TabIndex = 21;
			// 
			// lblAudioExportPath
			// 
			this.lblAudioExportPath.AutoSize = true;
			this.lblAudioExportPath.Location = new System.Drawing.Point(22, 83);
			this.lblAudioExportPath.Name = "lblAudioExportPath";
			this.lblAudioExportPath.Size = new System.Drawing.Size(75, 15);
			this.lblAudioExportPath.TabIndex = 20;
			this.lblAudioExportPath.Text = "Audio Folder";
			// 
			// btnAudioOutputFolder
			// 
			this.btnAudioOutputFolder.Location = new System.Drawing.Point(24, 101);
			this.btnAudioOutputFolder.Name = "btnAudioOutputFolder";
			this.btnAudioOutputFolder.Size = new System.Drawing.Size(24, 23);
			this.btnAudioOutputFolder.TabIndex = 16;
			this.btnAudioOutputFolder.Text = "Audio Output Folder";
			this.btnAudioOutputFolder.UseVisualStyleBackColor = true;
			this.btnAudioOutputFolder.Click += new System.EventHandler(this.btnAudioOutputFolder_Click);
			// 
			// BulkExportOutputFormatStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.lblChooseOutputFormat);
			this.Controls.Add(this.groupBox1);
			this.Name = "BulkExportOutputFormatStage";
			this.Size = new System.Drawing.Size(552, 393);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox chkRenameAudio;
		private System.Windows.Forms.CheckBox chkIncludeAudio;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label lblAudioExportPath;
		private System.Windows.Forms.Button btnAudioOutputFolder;
		private System.Windows.Forms.TextBox txtOutputFolder;
		private System.Windows.Forms.TextBox txtAudioOutputFolder;
	}
}
