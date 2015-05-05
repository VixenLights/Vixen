namespace VixenApplication
{
	partial class DataZipForm
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxMedia = new System.Windows.Forms.CheckBox();
			this.checkBoxLogs = new System.Windows.Forms.CheckBox();
			this.checkBoxTemplate = new System.Windows.Forms.CheckBox();
			this.checkBoxModule = new System.Windows.Forms.CheckBox();
			this.checkBoxSystem = new System.Windows.Forms.CheckBox();
			this.checkBoxProgram = new System.Windows.Forms.CheckBox();
			this.checkBoxSequence = new System.Windows.Forms.CheckBox();
			this.textBoxFileName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonSetSaveFolder = new System.Windows.Forms.Button();
			this.textBoxSaveFolder = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxProfiles = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonStartCancel = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.folderBrowserSaveFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.checkBoxUserSettings = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.textBoxFileName);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.buttonSetSaveFolder);
			this.groupBox1.Controls.Add(this.textBoxSaveFolder);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboBoxProfiles);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 250);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxUserSettings);
			this.groupBox2.Controls.Add(this.checkBoxMedia);
			this.groupBox2.Controls.Add(this.checkBoxLogs);
			this.groupBox2.Controls.Add(this.checkBoxTemplate);
			this.groupBox2.Controls.Add(this.checkBoxModule);
			this.groupBox2.Controls.Add(this.checkBoxSystem);
			this.groupBox2.Controls.Add(this.checkBoxProgram);
			this.groupBox2.Controls.Add(this.checkBoxSequence);
			this.groupBox2.Location = new System.Drawing.Point(10, 19);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(264, 90);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Profile artifacts to include";
			// 
			// checkBoxMedia
			// 
			this.checkBoxMedia.AutoSize = true;
			this.checkBoxMedia.Checked = true;
			this.checkBoxMedia.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMedia.Location = new System.Drawing.Point(7, 65);
			this.checkBoxMedia.Name = "checkBoxMedia";
			this.checkBoxMedia.Size = new System.Drawing.Size(55, 17);
			this.checkBoxMedia.TabIndex = 19;
			this.checkBoxMedia.Text = "Media";
			this.checkBoxMedia.UseVisualStyleBackColor = true;
			// 
			// checkBoxLogs
			// 
			this.checkBoxLogs.AutoSize = true;
			this.checkBoxLogs.Checked = true;
			this.checkBoxLogs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxLogs.Location = new System.Drawing.Point(169, 42);
			this.checkBoxLogs.Name = "checkBoxLogs";
			this.checkBoxLogs.Size = new System.Drawing.Size(49, 17);
			this.checkBoxLogs.TabIndex = 13;
			this.checkBoxLogs.Text = "Logs";
			this.checkBoxLogs.UseVisualStyleBackColor = true;
			// 
			// checkBoxTemplate
			// 
			this.checkBoxTemplate.AutoSize = true;
			this.checkBoxTemplate.Checked = true;
			this.checkBoxTemplate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxTemplate.Location = new System.Drawing.Point(90, 42);
			this.checkBoxTemplate.Name = "checkBoxTemplate";
			this.checkBoxTemplate.Size = new System.Drawing.Size(70, 17);
			this.checkBoxTemplate.TabIndex = 18;
			this.checkBoxTemplate.Text = "Template";
			this.checkBoxTemplate.UseVisualStyleBackColor = true;
			// 
			// checkBoxModule
			// 
			this.checkBoxModule.AutoSize = true;
			this.checkBoxModule.Checked = true;
			this.checkBoxModule.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxModule.Location = new System.Drawing.Point(6, 19);
			this.checkBoxModule.Name = "checkBoxModule";
			this.checkBoxModule.Size = new System.Drawing.Size(61, 17);
			this.checkBoxModule.TabIndex = 14;
			this.checkBoxModule.Text = "Module";
			this.checkBoxModule.UseVisualStyleBackColor = true;
			// 
			// checkBoxSystem
			// 
			this.checkBoxSystem.AutoSize = true;
			this.checkBoxSystem.Checked = true;
			this.checkBoxSystem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxSystem.Location = new System.Drawing.Point(7, 42);
			this.checkBoxSystem.Name = "checkBoxSystem";
			this.checkBoxSystem.Size = new System.Drawing.Size(60, 17);
			this.checkBoxSystem.TabIndex = 17;
			this.checkBoxSystem.Text = "System";
			this.checkBoxSystem.UseVisualStyleBackColor = true;
			// 
			// checkBoxProgram
			// 
			this.checkBoxProgram.AutoSize = true;
			this.checkBoxProgram.Checked = true;
			this.checkBoxProgram.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxProgram.Location = new System.Drawing.Point(90, 19);
			this.checkBoxProgram.Name = "checkBoxProgram";
			this.checkBoxProgram.Size = new System.Drawing.Size(65, 17);
			this.checkBoxProgram.TabIndex = 15;
			this.checkBoxProgram.Text = "Program";
			this.checkBoxProgram.UseVisualStyleBackColor = true;
			// 
			// checkBoxSequence
			// 
			this.checkBoxSequence.AutoSize = true;
			this.checkBoxSequence.Checked = true;
			this.checkBoxSequence.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxSequence.Location = new System.Drawing.Point(169, 19);
			this.checkBoxSequence.Name = "checkBoxSequence";
			this.checkBoxSequence.Size = new System.Drawing.Size(75, 17);
			this.checkBoxSequence.TabIndex = 16;
			this.checkBoxSequence.Text = "Sequence";
			this.checkBoxSequence.UseVisualStyleBackColor = true;
			// 
			// textBoxFileName
			// 
			this.textBoxFileName.Location = new System.Drawing.Point(9, 216);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.Size = new System.Drawing.Size(233, 20);
			this.textBoxFileName.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 200);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(176, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Archive file name, without extension";
			// 
			// buttonSetSaveFolder
			// 
			this.buttonSetSaveFolder.Location = new System.Drawing.Point(248, 173);
			this.buttonSetSaveFolder.Name = "buttonSetSaveFolder";
			this.buttonSetSaveFolder.Size = new System.Drawing.Size(23, 23);
			this.buttonSetSaveFolder.TabIndex = 7;
			this.buttonSetSaveFolder.Text = "F";
			this.buttonSetSaveFolder.UseVisualStyleBackColor = true;
			this.buttonSetSaveFolder.Click += new System.EventHandler(this.buttonSetSaveFolder_Click);
			// 
			// textBoxSaveFolder
			// 
			this.textBoxSaveFolder.Location = new System.Drawing.Point(9, 173);
			this.textBoxSaveFolder.Name = "textBoxSaveFolder";
			this.textBoxSaveFolder.Size = new System.Drawing.Size(233, 20);
			this.textBoxSaveFolder.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 157);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Folder to save archive to";
			// 
			// comboBoxProfiles
			// 
			this.comboBoxProfiles.FormattingEnabled = true;
			this.comboBoxProfiles.Location = new System.Drawing.Point(9, 128);
			this.comboBoxProfiles.Name = "comboBoxProfiles";
			this.comboBoxProfiles.Size = new System.Drawing.Size(261, 21);
			this.comboBoxProfiles.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Profile to archive";
			// 
			// buttonStartCancel
			// 
			this.buttonStartCancel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.buttonStartCancel.Location = new System.Drawing.Point(131, 284);
			this.buttonStartCancel.Name = "buttonStartCancel";
			this.buttonStartCancel.Size = new System.Drawing.Size(80, 23);
			this.buttonStartCancel.TabIndex = 10;
			this.buttonStartCancel.Text = "Start";
			this.buttonStartCancel.UseVisualStyleBackColor = true;
			this.buttonStartCancel.Click += new System.EventHandler(this.buttonStartCancel_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(217, 284);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 322);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(303, 22);
			this.statusStrip1.TabIndex = 11;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripProgressBar
			// 
			this.toolStripProgressBar.Name = "toolStripProgressBar";
			this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.toolStripProgressBar.Visible = false;
			// 
			// checkBoxUserSettings
			// 
			this.checkBoxUserSettings.AutoSize = true;
			this.checkBoxUserSettings.Checked = true;
			this.checkBoxUserSettings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxUserSettings.Location = new System.Drawing.Point(90, 65);
			this.checkBoxUserSettings.Name = "checkBoxUserSettings";
			this.checkBoxUserSettings.Size = new System.Drawing.Size(89, 17);
			this.checkBoxUserSettings.TabIndex = 20;
			this.checkBoxUserSettings.Text = "User Settings";
			this.checkBoxUserSettings.UseVisualStyleBackColor = true;
			// 
			// DataZipForm
			// 
			this.AcceptButton = this.buttonStartCancel;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(303, 344);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonStartCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataZipForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Profile Archive Wizard";
			this.Load += new System.EventHandler(this.DataZipForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonStartCancel;
		private System.Windows.Forms.TextBox textBoxFileName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonSetSaveFolder;
		private System.Windows.Forms.TextBox textBoxSaveFolder;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxProfiles;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserSaveFolder;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxLogs;
		private System.Windows.Forms.CheckBox checkBoxTemplate;
		private System.Windows.Forms.CheckBox checkBoxModule;
		private System.Windows.Forms.CheckBox checkBoxSystem;
		private System.Windows.Forms.CheckBox checkBoxProgram;
		private System.Windows.Forms.CheckBox checkBoxSequence;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
		private System.Windows.Forms.CheckBox checkBoxMedia;
		private System.Windows.Forms.CheckBox checkBoxUserSettings;
	}
}