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
			this.lipSyncMapColorCtrl1 = new VixenModules.App.LipSyncApp.LipSyncMapColorCtrl();
			this.buttonAssign = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonClear = new System.Windows.Forms.Button();
			this.buttonImport = new System.Windows.Forms.Button();
			this.buttonExport = new System.Windows.Forms.Button();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.phonemeLabel = new System.Windows.Forms.Label();
			this.nextPhonemeButton = new System.Windows.Forms.Button();
			this.prevPhonemeButton = new System.Windows.Forms.Button();
			this.phonemePicture = new System.Windows.Forms.PictureBox();
			this.tableLayoutGridArea = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutImportExportButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutOkCancel = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackbar)).BeginInit();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.phonemePicture)).BeginInit();
			this.tableLayoutGridArea.SuspendLayout();
			this.flowLayoutImportExportButtons.SuspendLayout();
			this.flowLayoutOkCancel.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonOK.Location = new System.Drawing.Point(2, 3);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 29);
			this.buttonOK.TabIndex = 16;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonCancel.Location = new System.Drawing.Point(92, 3);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 29);
			this.buttonCancel.TabIndex = 15;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(2, 3);
			this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(563, 248);
			this.dataGridView1.TabIndex = 18;
			this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
			this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label5.Location = new System.Drawing.Point(258, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(39, 15);
			this.label5.TabIndex = 31;
			this.label5.Text = "Zoom";
			// 
			// zoomTrackbar
			// 
			this.zoomTrackbar.AutoSize = false;
			this.zoomTrackbar.BackColor = System.Drawing.SystemColors.Control;
			this.zoomTrackbar.Location = new System.Drawing.Point(302, 106);
			this.zoomTrackbar.Margin = new System.Windows.Forms.Padding(1);
			this.zoomTrackbar.Maximum = 25;
			this.zoomTrackbar.Minimum = -50;
			this.zoomTrackbar.Name = "zoomTrackbar";
			this.zoomTrackbar.Size = new System.Drawing.Size(105, 27);
			this.zoomTrackbar.TabIndex = 30;
			this.zoomTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.zoomTrackbar.ValueChanged += new System.EventHandler(this.zoomTrackbar_ValueChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.blackCheckBox);
			this.groupBox3.Controls.Add(this.lipSyncMapColorCtrl1);
			this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox3.Location = new System.Drawing.Point(447, 16);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox3.Size = new System.Drawing.Size(231, 105);
			this.groupBox3.TabIndex = 29;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Pen";
			this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// blackCheckBox
			// 
			this.blackCheckBox.AutoSize = true;
			this.blackCheckBox.Checked = true;
			this.blackCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.blackCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.blackCheckBox.Location = new System.Drawing.Point(16, 81);
			this.blackCheckBox.Name = "blackCheckBox";
			this.blackCheckBox.Size = new System.Drawing.Size(131, 19);
			this.blackCheckBox.TabIndex = 1;
			this.blackCheckBox.Text = "Black is Transparent";
			this.blackCheckBox.UseVisualStyleBackColor = true;
			// 
			// lipSyncMapColorCtrl1
			// 
			this.lipSyncMapColorCtrl1.BackColor = System.Drawing.SystemColors.Control;
			this.lipSyncMapColorCtrl1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.lipSyncMapColorCtrl1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lipSyncMapColorCtrl1.HSVColor = ((Common.Controls.ColorManagement.ColorModels.HSV)(resources.GetObject("lipSyncMapColorCtrl1.HSVColor")));
			this.lipSyncMapColorCtrl1.Intensity = 1D;
			this.lipSyncMapColorCtrl1.Location = new System.Drawing.Point(5, 16);
			this.lipSyncMapColorCtrl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.lipSyncMapColorCtrl1.Name = "lipSyncMapColorCtrl1";
			this.lipSyncMapColorCtrl1.Size = new System.Drawing.Size(219, 65);
			this.lipSyncMapColorCtrl1.TabIndex = 0;
			// 
			// buttonAssign
			// 
			this.buttonAssign.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonAssign.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAssign.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonAssign.Location = new System.Drawing.Point(345, 26);
			this.buttonAssign.Name = "buttonAssign";
			this.buttonAssign.Size = new System.Drawing.Size(87, 27);
			this.buttonAssign.TabIndex = 32;
			this.buttonAssign.Text = "Assign";
			this.buttonAssign.UseVisualStyleBackColor = true;
			this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
			this.buttonAssign.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAssign.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label6.Location = new System.Drawing.Point(280, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(55, 15);
			this.label6.TabIndex = 33;
			this.label6.Text = "Elements";
			// 
			// buttonClear
			// 
			this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonClear.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonClear.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonClear.Location = new System.Drawing.Point(3, 69);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(87, 27);
			this.buttonClear.TabIndex = 37;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			this.buttonClear.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonClear.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonImport
			// 
			this.buttonImport.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonImport.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonImport.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonImport.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonImport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonImport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonImport.Location = new System.Drawing.Point(3, 3);
			this.buttonImport.Name = "buttonImport";
			this.buttonImport.Size = new System.Drawing.Size(87, 27);
			this.buttonImport.TabIndex = 39;
			this.buttonImport.Text = "Import";
			this.buttonImport.UseVisualStyleBackColor = true;
			this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
			this.buttonImport.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonImport.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonExport
			// 
			this.buttonExport.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonExport.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonExport.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonExport.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonExport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonExport.Location = new System.Drawing.Point(3, 36);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(87, 27);
			this.buttonExport.TabIndex = 40;
			this.buttonExport.Text = "Export";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			this.buttonExport.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonExport.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// nameTextBox
			// 
			this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nameTextBox.Location = new System.Drawing.Point(93, 28);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(163, 23);
			this.nameTextBox.TabIndex = 42;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label1.Location = new System.Drawing.Point(28, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 15);
			this.label1.TabIndex = 41;
			this.label1.Text = "Name";
			// 
			// phonemeLabel
			// 
			this.phonemeLabel.AutoSize = true;
			this.phonemeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.phonemeLabel.Location = new System.Drawing.Point(94, 68);
			this.phonemeLabel.Name = "phonemeLabel";
			this.phonemeLabel.Size = new System.Drawing.Size(58, 15);
			this.phonemeLabel.TabIndex = 46;
			this.phonemeLabel.Text = "Phoneme";
			this.phonemeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// nextPhonemeButton
			// 
			this.nextPhonemeButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.nextPhonemeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.nextPhonemeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nextPhonemeButton.Location = new System.Drawing.Point(156, 106);
			this.nextPhonemeButton.Name = "nextPhonemeButton";
			this.nextPhonemeButton.Size = new System.Drawing.Size(42, 27);
			this.nextPhonemeButton.TabIndex = 45;
			this.nextPhonemeButton.Text = ">";
			this.nextPhonemeButton.UseVisualStyleBackColor = true;
			this.nextPhonemeButton.Click += new System.EventHandler(this.nextPhonemeButton_Click);
			// 
			// prevPhonemeButton
			// 
			this.prevPhonemeButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.prevPhonemeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.prevPhonemeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.prevPhonemeButton.Location = new System.Drawing.Point(44, 106);
			this.prevPhonemeButton.Name = "prevPhonemeButton";
			this.prevPhonemeButton.Size = new System.Drawing.Size(42, 27);
			this.prevPhonemeButton.TabIndex = 44;
			this.prevPhonemeButton.Text = "<";
			this.prevPhonemeButton.UseVisualStyleBackColor = true;
			this.prevPhonemeButton.Click += new System.EventHandler(this.prevPhonemeButton_Click);
			// 
			// phonemePicture
			// 
			this.phonemePicture.Location = new System.Drawing.Point(93, 90);
			this.phonemePicture.Name = "phonemePicture";
			this.phonemePicture.Size = new System.Drawing.Size(56, 55);
			this.phonemePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.phonemePicture.TabIndex = 43;
			this.phonemePicture.TabStop = false;
			// 
			// tableLayoutGridArea
			// 
			this.tableLayoutGridArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutGridArea.ColumnCount = 2;
			this.tableLayoutGridArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutGridArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutGridArea.Controls.Add(this.flowLayoutImportExportButtons, 1, 0);
			this.tableLayoutGridArea.Controls.Add(this.dataGridView1, 0, 0);
			this.tableLayoutGridArea.Controls.Add(this.flowLayoutOkCancel, 0, 1);
			this.tableLayoutGridArea.Location = new System.Drawing.Point(12, 161);
			this.tableLayoutGridArea.Name = "tableLayoutGridArea";
			this.tableLayoutGridArea.RowCount = 2;
			this.tableLayoutGridArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutGridArea.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutGridArea.Size = new System.Drawing.Size(666, 304);
			this.tableLayoutGridArea.TabIndex = 47;
			// 
			// flowLayoutImportExportButtons
			// 
			this.flowLayoutImportExportButtons.AutoSize = true;
			this.flowLayoutImportExportButtons.Controls.Add(this.buttonImport);
			this.flowLayoutImportExportButtons.Controls.Add(this.buttonExport);
			this.flowLayoutImportExportButtons.Controls.Add(this.buttonClear);
			this.flowLayoutImportExportButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutImportExportButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutImportExportButtons.Location = new System.Drawing.Point(570, 3);
			this.flowLayoutImportExportButtons.Name = "flowLayoutImportExportButtons";
			this.flowLayoutImportExportButtons.Size = new System.Drawing.Size(93, 248);
			this.flowLayoutImportExportButtons.TabIndex = 0;
			// 
			// flowLayoutOkCancel
			// 
			this.flowLayoutOkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutOkCancel.AutoSize = true;
			this.tableLayoutGridArea.SetColumnSpan(this.flowLayoutOkCancel, 2);
			this.flowLayoutOkCancel.Controls.Add(this.buttonCancel);
			this.flowLayoutOkCancel.Controls.Add(this.buttonOK);
			this.flowLayoutOkCancel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutOkCancel.Location = new System.Drawing.Point(482, 266);
			this.flowLayoutOkCancel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
			this.flowLayoutOkCancel.Name = "flowLayoutOkCancel";
			this.flowLayoutOkCancel.Size = new System.Drawing.Size(181, 35);
			this.flowLayoutOkCancel.TabIndex = 19;
			// 
			// LipSyncMapMatrixEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(696, 481);
			this.Controls.Add(this.tableLayoutGridArea);
			this.Controls.Add(this.phonemeLabel);
			this.Controls.Add(this.nextPhonemeButton);
			this.Controls.Add(this.prevPhonemeButton);
			this.Controls.Add(this.phonemePicture);
			this.Controls.Add(this.zoomTrackbar);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nameTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.buttonAssign);
			this.Controls.Add(this.groupBox3);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.MinimumSize = new System.Drawing.Size(698, 476);
			this.Name = "LipSyncMapMatrixEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "LipSync Matrix Editor";
			this.Load += new System.EventHandler(this.LipSyncMapSetup_Load);
			this.Resize += new System.EventHandler(this.LipSyncBreakdownSetup_Resize);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackbar)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.phonemePicture)).EndInit();
			this.tableLayoutGridArea.ResumeLayout(false);
			this.tableLayoutGridArea.PerformLayout();
			this.flowLayoutImportExportButtons.ResumeLayout(false);
			this.flowLayoutOkCancel.ResumeLayout(false);
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
        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox blackCheckBox;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label phonemeLabel;
        private System.Windows.Forms.Button nextPhonemeButton;
        private System.Windows.Forms.Button prevPhonemeButton;
        private System.Windows.Forms.PictureBox phonemePicture;
		private System.Windows.Forms.TableLayoutPanel tableLayoutGridArea;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutImportExportButtons;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutOkCancel;
	}
}