namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncMapMatrixEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LipSyncMapMatrixEditor));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.zoomTrackbar = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.blackCheckBox = new System.Windows.Forms.CheckBox();
            this.assignButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.phonemeLabel = new System.Windows.Forms.Label();
            this.nextPhonemeButton = new System.Windows.Forms.Button();
            this.prevPhonemeButton = new System.Windows.Forms.Button();
            this.phonemePicture = new System.Windows.Forms.PictureBox();
            this.lipSyncMapColorCtrl1 = new VixenModules.App.LipSyncApp.LipSyncMapColorCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackbar)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phonemePicture)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(495, 143);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 25);
            this.buttonOK.TabIndex = 16;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(494, 174);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 25);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(25, 145);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(47, 24);
            this.dataGridView1.TabIndex = 18;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Zoom";
            // 
            // zoomTrackbar
            // 
            this.zoomTrackbar.AutoSize = false;
            this.zoomTrackbar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.zoomTrackbar.Location = new System.Drawing.Point(259, 92);
            this.zoomTrackbar.Margin = new System.Windows.Forms.Padding(1);
            this.zoomTrackbar.Maximum = 25;
            this.zoomTrackbar.Minimum = -50;
            this.zoomTrackbar.Name = "zoomTrackbar";
            this.zoomTrackbar.Size = new System.Drawing.Size(90, 23);
            this.zoomTrackbar.TabIndex = 30;
            this.zoomTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zoomTrackbar.ValueChanged += new System.EventHandler(this.zoomTrackbar_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.blackCheckBox);
            this.groupBox3.Controls.Add(this.lipSyncMapColorCtrl1);
            this.groupBox3.Location = new System.Drawing.Point(383, 14);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox3.Size = new System.Drawing.Size(198, 91);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pen";
            // 
            // blackCheckBox
            // 
            this.blackCheckBox.AutoSize = true;
            this.blackCheckBox.Checked = true;
            this.blackCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.blackCheckBox.Location = new System.Drawing.Point(14, 70);
            this.blackCheckBox.Name = "blackCheckBox";
            this.blackCheckBox.Size = new System.Drawing.Size(123, 17);
            this.blackCheckBox.TabIndex = 1;
            this.blackCheckBox.Text = "Black is Transparent";
            this.blackCheckBox.UseVisualStyleBackColor = true;
            // 
            // assignButton
            // 
            this.assignButton.Location = new System.Drawing.Point(296, 28);
            this.assignButton.Name = "assignButton";
            this.assignButton.Size = new System.Drawing.Size(75, 23);
            this.assignButton.TabIndex = 32;
            this.assignButton.Text = "Assign";
            this.assignButton.UseVisualStyleBackColor = true;
            this.assignButton.Click += new System.EventHandler(this.assignButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(240, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Elements";
            // 
            // clearButton
            // 
            this.clearButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(500, 350);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 37;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // importButton
            // 
            this.importButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.importButton.Location = new System.Drawing.Point(500, 256);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 39;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.exportButton.Location = new System.Drawing.Point(500, 285);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 40;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(80, 26);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(140, 20);
            this.nameTextBox.TabIndex = 42;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Name";
            // 
            // phonemeLabel
            // 
            this.phonemeLabel.Location = new System.Drawing.Point(81, 59);
            this.phonemeLabel.Name = "phonemeLabel";
            this.phonemeLabel.Size = new System.Drawing.Size(47, 18);
            this.phonemeLabel.TabIndex = 46;
            this.phonemeLabel.Text = "Phoneme";
            this.phonemeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nextPhonemeButton
            // 
            this.nextPhonemeButton.Location = new System.Drawing.Point(134, 92);
            this.nextPhonemeButton.Name = "nextPhonemeButton";
            this.nextPhonemeButton.Size = new System.Drawing.Size(36, 23);
            this.nextPhonemeButton.TabIndex = 45;
            this.nextPhonemeButton.Text = ">";
            this.nextPhonemeButton.UseVisualStyleBackColor = true;
            this.nextPhonemeButton.Click += new System.EventHandler(this.nextPhonemeButton_Click);
            // 
            // prevPhonemeButton
            // 
            this.prevPhonemeButton.Location = new System.Drawing.Point(38, 92);
            this.prevPhonemeButton.Name = "prevPhonemeButton";
            this.prevPhonemeButton.Size = new System.Drawing.Size(36, 23);
            this.prevPhonemeButton.TabIndex = 44;
            this.prevPhonemeButton.Text = "<";
            this.prevPhonemeButton.UseVisualStyleBackColor = true;
            this.prevPhonemeButton.Click += new System.EventHandler(this.prevPhonemeButton_Click);
            // 
            // phonemePicture
            // 
            this.phonemePicture.Location = new System.Drawing.Point(80, 78);
            this.phonemePicture.Name = "phonemePicture";
            this.phonemePicture.Size = new System.Drawing.Size(48, 48);
            this.phonemePicture.TabIndex = 43;
            this.phonemePicture.TabStop = false;
            // 
            // lipSyncMapColorCtrl1
            // 
            this.lipSyncMapColorCtrl1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lipSyncMapColorCtrl1.HSVColor = ((Common.Controls.ColorManagement.ColorModels.HSV)(resources.GetObject("lipSyncMapColorCtrl1.HSVColor")));
            this.lipSyncMapColorCtrl1.Intensity = 0D;
            this.lipSyncMapColorCtrl1.Location = new System.Drawing.Point(4, 14);
            this.lipSyncMapColorCtrl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.lipSyncMapColorCtrl1.Name = "lipSyncMapColorCtrl1";
            this.lipSyncMapColorCtrl1.Size = new System.Drawing.Size(188, 56);
            this.lipSyncMapColorCtrl1.TabIndex = 0;
            // 
            // LipSyncMapMatrixEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(597, 403);
            this.Controls.Add(this.phonemeLabel);
            this.Controls.Add(this.nextPhonemeButton);
            this.Controls.Add(this.prevPhonemeButton);
            this.Controls.Add(this.phonemePicture);
            this.Controls.Add(this.zoomTrackbar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.assignButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MinimumSize = new System.Drawing.Size(605, 430);
            this.Name = "LipSyncMapMatrixEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LipSync Matrix Editor";
            this.Load += new System.EventHandler(this.LipSyncMapSetup_Load);
            this.Resize += new System.EventHandler(this.LipSyncBreakdownSetup_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackbar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phonemePicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox3;
        private LipSyncMapColorCtrl lipSyncMapColorCtrl1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar zoomTrackbar;
        private System.Windows.Forms.Button assignButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox blackCheckBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label phonemeLabel;
        private System.Windows.Forms.Button nextPhonemeButton;
        private System.Windows.Forms.Button prevPhonemeButton;
        private System.Windows.Forms.PictureBox phonemePicture;

    }
}