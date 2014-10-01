namespace VixenApplication
{
    partial class DataProfileForm
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
			this.comboBoxProfiles = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxLoadThisProfile = new System.Windows.Forms.ComboBox();
			this.radioButtonLoadThisProfile = new System.Windows.Forms.RadioButton();
			this.radioButtonAskMe = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.buttonSetDataFolder = new System.Windows.Forms.Button();
			this.textBoxDataFolder = new System.Windows.Forms.TextBox();
			this.textBoxProfileName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonDeleteProfile = new System.Windows.Forms.Button();
			this.buttonAddProfile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.folderBrowserDataFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.buttonZipWizard = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxProfiles
			// 
			this.comboBoxProfiles.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxProfiles.FormattingEnabled = true;
			this.comboBoxProfiles.Location = new System.Drawing.Point(59, 18);
			this.comboBoxProfiles.Name = "comboBoxProfiles";
			this.comboBoxProfiles.Size = new System.Drawing.Size(272, 21);
			this.comboBoxProfiles.TabIndex = 0;
			this.comboBoxProfiles.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboBoxProfiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxProfiles_SelectedIndexChanged);
			this.comboBoxProfiles.SelectionChangeCommitted += new System.EventHandler(this.comboBoxProfiles_SelectionChangeCommitted);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBoxLoadThisProfile);
			this.groupBox1.Controls.Add(this.radioButtonLoadThisProfile);
			this.groupBox1.Controls.Add(this.radioButtonAskMe);
			this.groupBox1.Location = new System.Drawing.Point(15, 139);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(400, 56);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Profile to Use when Loading Vixen";
			// 
			// comboBoxLoadThisProfile
			// 
			this.comboBoxLoadThisProfile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxLoadThisProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLoadThisProfile.FormattingEnabled = true;
			this.comboBoxLoadThisProfile.Location = new System.Drawing.Point(239, 22);
			this.comboBoxLoadThisProfile.Name = "comboBoxLoadThisProfile";
			this.comboBoxLoadThisProfile.Size = new System.Drawing.Size(148, 21);
			this.comboBoxLoadThisProfile.TabIndex = 2;
			this.comboBoxLoadThisProfile.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// radioButtonLoadThisProfile
			// 
			this.radioButtonLoadThisProfile.AutoSize = true;
			this.radioButtonLoadThisProfile.Location = new System.Drawing.Point(130, 23);
			this.radioButtonLoadThisProfile.Name = "radioButtonLoadThisProfile";
			this.radioButtonLoadThisProfile.Size = new System.Drawing.Size(107, 17);
			this.radioButtonLoadThisProfile.TabIndex = 1;
			this.radioButtonLoadThisProfile.TabStop = true;
			this.radioButtonLoadThisProfile.Text = "Load This Profile:";
			this.radioButtonLoadThisProfile.UseVisualStyleBackColor = true;
			// 
			// radioButtonAskMe
			// 
			this.radioButtonAskMe.AutoSize = true;
			this.radioButtonAskMe.Location = new System.Drawing.Point(7, 23);
			this.radioButtonAskMe.Name = "radioButtonAskMe";
			this.radioButtonAskMe.Size = new System.Drawing.Size(117, 17);
			this.radioButtonAskMe.TabIndex = 0;
			this.radioButtonAskMe.TabStop = true;
			this.radioButtonAskMe.Text = "Ask Me Every Time";
			this.radioButtonAskMe.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.buttonSetDataFolder);
			this.groupBox2.Controls.Add(this.textBoxDataFolder);
			this.groupBox2.Controls.Add(this.textBoxProfileName);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.buttonDeleteProfile);
			this.groupBox2.Controls.Add(this.buttonAddProfile);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.comboBoxProfiles);
			this.groupBox2.Location = new System.Drawing.Point(15, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(400, 121);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Profiles:";
			// 
			// buttonSetDataFolder
			// 
			this.buttonSetDataFolder.Location = new System.Drawing.Point(364, 87);
			this.buttonSetDataFolder.Name = "buttonSetDataFolder";
			this.buttonSetDataFolder.Size = new System.Drawing.Size(23, 23);
			this.buttonSetDataFolder.TabIndex = 9;
			this.buttonSetDataFolder.Text = "FOLDER";
			this.buttonSetDataFolder.UseVisualStyleBackColor = true;
			this.buttonSetDataFolder.Click += new System.EventHandler(this.buttonSetDataFolder_Click);
			// 
			// textBoxDataFolder
			// 
			this.textBoxDataFolder.Location = new System.Drawing.Point(100, 88);
			this.textBoxDataFolder.Name = "textBoxDataFolder";
			this.textBoxDataFolder.Size = new System.Drawing.Size(261, 20);
			this.textBoxDataFolder.TabIndex = 7;
			// 
			// textBoxProfileName
			// 
			this.textBoxProfileName.Location = new System.Drawing.Point(100, 62);
			this.textBoxProfileName.Name = "textBoxProfileName";
			this.textBoxProfileName.Size = new System.Drawing.Size(137, 20);
			this.textBoxProfileName.TabIndex = 6;
			this.textBoxProfileName.Leave += new System.EventHandler(this.textBoxProfileName_Leave);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Data Folder:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Profile Name:";
			// 
			// buttonDeleteProfile
			// 
			this.buttonDeleteProfile.Location = new System.Drawing.Point(363, 17);
			this.buttonDeleteProfile.Name = "buttonDeleteProfile";
			this.buttonDeleteProfile.Size = new System.Drawing.Size(23, 23);
			this.buttonDeleteProfile.TabIndex = 3;
			this.buttonDeleteProfile.Text = "-";
			this.buttonDeleteProfile.UseVisualStyleBackColor = true;
			this.buttonDeleteProfile.Click += new System.EventHandler(this.buttonDeleteProfile_Click);
			// 
			// buttonAddProfile
			// 
			this.buttonAddProfile.Location = new System.Drawing.Point(337, 17);
			this.buttonAddProfile.Name = "buttonAddProfile";
			this.buttonAddProfile.Size = new System.Drawing.Size(23, 23);
			this.buttonAddProfile.TabIndex = 2;
			this.buttonAddProfile.Text = "+";
			this.buttonAddProfile.UseVisualStyleBackColor = true;
			this.buttonAddProfile.Click += new System.EventHandler(this.buttonAddProfile_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Profile:";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(262, 206);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(343, 206);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonZipWizard
			// 
			this.buttonZipWizard.Location = new System.Drawing.Point(15, 206);
			this.buttonZipWizard.Name = "buttonZipWizard";
			this.buttonZipWizard.Size = new System.Drawing.Size(75, 23);
			this.buttonZipWizard.TabIndex = 8;
			this.buttonZipWizard.Text = "Zip Wizard";
			this.buttonZipWizard.UseVisualStyleBackColor = true;
			this.buttonZipWizard.Click += new System.EventHandler(this.buttonZipWizard_Click);
			// 
			// DataProfileForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(430, 240);
			this.ControlBox = false;
			this.Controls.Add(this.buttonZipWizard);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataProfileForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Data Profile";
			this.Load += new System.EventHandler(this.DataProfileForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxProfiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxLoadThisProfile;
        private System.Windows.Forms.RadioButton radioButtonLoadThisProfile;
        private System.Windows.Forms.RadioButton radioButtonAskMe;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonDeleteProfile;
        private System.Windows.Forms.Button buttonAddProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSetDataFolder;
        private System.Windows.Forms.TextBox textBoxDataFolder;
        private System.Windows.Forms.TextBox textBoxProfileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
      
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDataFolder;
		private System.Windows.Forms.Button buttonZipWizard;
    }
}