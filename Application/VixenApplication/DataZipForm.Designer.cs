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
			this.checkBoxApplication = new System.Windows.Forms.CheckBox();
			this.checkBoxTemplate = new System.Windows.Forms.CheckBox();
			this.checkBoxModule = new System.Windows.Forms.CheckBox();
			this.checkBoxSystem = new System.Windows.Forms.CheckBox();
			this.checkBoxProgram = new System.Windows.Forms.CheckBox();
			this.checkBoxSequence = new System.Windows.Forms.CheckBox();
			this.radioButtonUsersChoice = new System.Windows.Forms.RadioButton();
			this.radioButtonZipEverything = new System.Windows.Forms.RadioButton();
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
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.radioButtonUsersChoice);
			this.groupBox1.Controls.Add(this.radioButtonZipEverything);
			this.groupBox1.Controls.Add(this.textBoxFileName);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.buttonSetSaveFolder);
			this.groupBox1.Controls.Add(this.textBoxSaveFolder);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboBoxProfiles);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 245);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxApplication);
			this.groupBox2.Controls.Add(this.checkBoxTemplate);
			this.groupBox2.Controls.Add(this.checkBoxModule);
			this.groupBox2.Controls.Add(this.checkBoxSystem);
			this.groupBox2.Controls.Add(this.checkBoxProgram);
			this.groupBox2.Controls.Add(this.checkBoxSequence);
			this.groupBox2.Location = new System.Drawing.Point(6, 42);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(264, 66);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Data to zip";
			// 
			// checkBoxApplication
			// 
			this.checkBoxApplication.AutoSize = true;
			this.checkBoxApplication.Location = new System.Drawing.Point(6, 19);
			this.checkBoxApplication.Name = "checkBoxApplication";
			this.checkBoxApplication.Size = new System.Drawing.Size(78, 17);
			this.checkBoxApplication.TabIndex = 13;
			this.checkBoxApplication.Text = "Application";
			this.checkBoxApplication.UseVisualStyleBackColor = true;
			// 
			// checkBoxTemplate
			// 
			this.checkBoxTemplate.AutoSize = true;
			this.checkBoxTemplate.Location = new System.Drawing.Point(169, 42);
			this.checkBoxTemplate.Name = "checkBoxTemplate";
			this.checkBoxTemplate.Size = new System.Drawing.Size(70, 17);
			this.checkBoxTemplate.TabIndex = 18;
			this.checkBoxTemplate.Text = "Template";
			this.checkBoxTemplate.UseVisualStyleBackColor = true;
			// 
			// checkBoxModule
			// 
			this.checkBoxModule.AutoSize = true;
			this.checkBoxModule.Location = new System.Drawing.Point(90, 19);
			this.checkBoxModule.Name = "checkBoxModule";
			this.checkBoxModule.Size = new System.Drawing.Size(61, 17);
			this.checkBoxModule.TabIndex = 14;
			this.checkBoxModule.Text = "Module";
			this.checkBoxModule.UseVisualStyleBackColor = true;
			// 
			// checkBoxSystem
			// 
			this.checkBoxSystem.AutoSize = true;
			this.checkBoxSystem.Location = new System.Drawing.Point(90, 42);
			this.checkBoxSystem.Name = "checkBoxSystem";
			this.checkBoxSystem.Size = new System.Drawing.Size(60, 17);
			this.checkBoxSystem.TabIndex = 17;
			this.checkBoxSystem.Text = "System";
			this.checkBoxSystem.UseVisualStyleBackColor = true;
			// 
			// checkBoxProgram
			// 
			this.checkBoxProgram.AutoSize = true;
			this.checkBoxProgram.Location = new System.Drawing.Point(169, 19);
			this.checkBoxProgram.Name = "checkBoxProgram";
			this.checkBoxProgram.Size = new System.Drawing.Size(65, 17);
			this.checkBoxProgram.TabIndex = 15;
			this.checkBoxProgram.Text = "Program";
			this.checkBoxProgram.UseVisualStyleBackColor = true;
			// 
			// checkBoxSequence
			// 
			this.checkBoxSequence.AutoSize = true;
			this.checkBoxSequence.Location = new System.Drawing.Point(6, 42);
			this.checkBoxSequence.Name = "checkBoxSequence";
			this.checkBoxSequence.Size = new System.Drawing.Size(75, 17);
			this.checkBoxSequence.TabIndex = 16;
			this.checkBoxSequence.Text = "Sequence";
			this.checkBoxSequence.UseVisualStyleBackColor = true;
			// 
			// radioButtonUsersChoice
			// 
			this.radioButtonUsersChoice.AutoSize = true;
			this.radioButtonUsersChoice.Location = new System.Drawing.Point(175, 19);
			this.radioButtonUsersChoice.Name = "radioButtonUsersChoice";
			this.radioButtonUsersChoice.Size = new System.Drawing.Size(95, 17);
			this.radioButtonUsersChoice.TabIndex = 12;
			this.radioButtonUsersChoice.TabStop = true;
			this.radioButtonUsersChoice.Text = "Let me choose";
			this.radioButtonUsersChoice.UseVisualStyleBackColor = true;
			this.radioButtonUsersChoice.Click += new System.EventHandler(this.radioButtonUsersChoice_Click);
			// 
			// radioButtonZipEverything
			// 
			this.radioButtonZipEverything.AutoSize = true;
			this.radioButtonZipEverything.Location = new System.Drawing.Point(6, 19);
			this.radioButtonZipEverything.Name = "radioButtonZipEverything";
			this.radioButtonZipEverything.Size = new System.Drawing.Size(93, 17);
			this.radioButtonZipEverything.TabIndex = 11;
			this.radioButtonZipEverything.TabStop = true;
			this.radioButtonZipEverything.Text = "Zip Everything";
			this.radioButtonZipEverything.UseVisualStyleBackColor = true;
			this.radioButtonZipEverything.Click += new System.EventHandler(this.radioButtonZipEverything_Click);
			// 
			// textBoxFileName
			// 
			this.textBoxFileName.Location = new System.Drawing.Point(6, 216);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.Size = new System.Drawing.Size(233, 20);
			this.textBoxFileName.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 199);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(172, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Zip file name, without .zip extention";
			// 
			// buttonSetSaveFolder
			// 
			this.buttonSetSaveFolder.Location = new System.Drawing.Point(245, 172);
			this.buttonSetSaveFolder.Name = "buttonSetSaveFolder";
			this.buttonSetSaveFolder.Size = new System.Drawing.Size(23, 23);
			this.buttonSetSaveFolder.TabIndex = 7;
			this.buttonSetSaveFolder.Text = "F";
			this.buttonSetSaveFolder.UseVisualStyleBackColor = true;
			this.buttonSetSaveFolder.Click += new System.EventHandler(this.buttonSetSaveFolder_Click);
			// 
			// textBoxSaveFolder
			// 
			this.textBoxSaveFolder.Location = new System.Drawing.Point(6, 172);
			this.textBoxSaveFolder.Name = "textBoxSaveFolder";
			this.textBoxSaveFolder.Size = new System.Drawing.Size(233, 20);
			this.textBoxSaveFolder.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 155);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Folder to save zip file in";
			// 
			// comboBoxProfiles
			// 
			this.comboBoxProfiles.FormattingEnabled = true;
			this.comboBoxProfiles.Location = new System.Drawing.Point(6, 127);
			this.comboBoxProfiles.Name = "comboBoxProfiles";
			this.comboBoxProfiles.Size = new System.Drawing.Size(261, 21);
			this.comboBoxProfiles.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 111);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Profile to zip";
			// 
			// buttonStartCancel
			// 
			this.buttonStartCancel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.buttonStartCancel.Location = new System.Drawing.Point(131, 263);
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
			this.buttonClose.Location = new System.Drawing.Point(217, 263);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
			this.statusStrip1.Location = new System.Drawing.Point(0, 300);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(303, 22);
			this.statusStrip1.TabIndex = 11;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(122, 17);
			this.toolStripStatusLabel.Text = "Waiting for user input";
			// 
			// toolStripProgressBar
			// 
			this.toolStripProgressBar.Name = "toolStripProgressBar";
			this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.toolStripProgressBar.Visible = false;
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			// 
			// DataZipForm
			// 
			this.AcceptButton = this.buttonStartCancel;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(303, 322);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonStartCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataZipForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Data Zip Wizard";
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
		private System.Windows.Forms.CheckBox checkBoxApplication;
		private System.Windows.Forms.CheckBox checkBoxTemplate;
		private System.Windows.Forms.CheckBox checkBoxModule;
		private System.Windows.Forms.CheckBox checkBoxSystem;
		private System.Windows.Forms.CheckBox checkBoxProgram;
		private System.Windows.Forms.CheckBox checkBoxSequence;
		private System.Windows.Forms.RadioButton radioButtonUsersChoice;
		private System.Windows.Forms.RadioButton radioButtonZipEverything;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
	}
}