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
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.nameLabel = new System.Windows.Forms.Label();
			this.phonemeLabel = new System.Windows.Forms.Label();
			this.nextPhonemeButton = new System.Windows.Forms.Button();
			this.prevPhonemeButton = new System.Windows.Forms.Button();
			this.phonemeIcon = new System.Windows.Forms.PictureBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.flowLayoutImportExportButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.openButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.clearButton = new System.Windows.Forms.Button();
			this.tableLayoutGridArea = new System.Windows.Forms.TableLayoutPanel();
			this.renderedPicture = new System.Windows.Forms.PictureBox();
			this.notesLabel = new System.Windows.Forms.Label();
			this.notesTextBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.phonemeIcon)).BeginInit();
			this.flowLayoutImportExportButtons.SuspendLayout();
			this.tableLayoutGridArea.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.renderedPicture)).BeginInit();
			this.SuspendLayout();
			// 
			// nameTextBox
			// 
			this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nameTextBox.Location = new System.Drawing.Point(59, 28);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(177, 23);
			this.nameTextBox.TabIndex = 42;
			// 
			// nameLabel
			// 
			this.nameLabel.AutoSize = true;
			this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nameLabel.Location = new System.Drawing.Point(14, 30);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(39, 15);
			this.nameLabel.TabIndex = 41;
			this.nameLabel.Text = "Name";
			// 
			// phonemeLabel
			// 
			this.phonemeLabel.AutoSize = true;
			this.phonemeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.phonemeLabel.Location = new System.Drawing.Point(105, 62);
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
			this.nextPhonemeButton.Location = new System.Drawing.Point(167, 100);
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
			this.prevPhonemeButton.Location = new System.Drawing.Point(55, 100);
			this.prevPhonemeButton.Name = "prevPhonemeButton";
			this.prevPhonemeButton.Size = new System.Drawing.Size(42, 27);
			this.prevPhonemeButton.TabIndex = 44;
			this.prevPhonemeButton.Text = "<";
			this.prevPhonemeButton.UseVisualStyleBackColor = true;
			this.prevPhonemeButton.Click += new System.EventHandler(this.prevPhonemeButton_Click);
			// 
			// phonemeIcon
			// 
			this.phonemeIcon.Location = new System.Drawing.Point(104, 84);
			this.phonemeIcon.Name = "phonemeIcon";
			this.phonemeIcon.Size = new System.Drawing.Size(56, 55);
			this.phonemeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.phonemeIcon.TabIndex = 43;
			this.phonemeIcon.TabStop = false;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonCancel.Location = new System.Drawing.Point(149, 342);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 29);
			this.buttonCancel.TabIndex = 15;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonOK.Location = new System.Drawing.Point(52, 342);
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
			// flowLayoutImportExportButtons
			// 
			this.flowLayoutImportExportButtons.AutoSize = true;
			this.flowLayoutImportExportButtons.Controls.Add(this.openButton);
			this.flowLayoutImportExportButtons.Controls.Add(this.editButton);
			this.flowLayoutImportExportButtons.Controls.Add(this.clearButton);
			this.flowLayoutImportExportButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutImportExportButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutImportExportButtons.Location = new System.Drawing.Point(105, 3);
			this.flowLayoutImportExportButtons.Name = "flowLayoutImportExportButtons";
			this.flowLayoutImportExportButtons.Size = new System.Drawing.Size(54, 95);
			this.flowLayoutImportExportButtons.TabIndex = 0;
			// 
			// openButton
			// 
			this.openButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.openButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.openButton.Location = new System.Drawing.Point(3, 3);
			this.openButton.Name = "openButton";
			this.openButton.Size = new System.Drawing.Size(48, 25);
			this.openButton.TabIndex = 48;
			this.openButton.Text = "File";
			this.openButton.UseVisualStyleBackColor = true;
			this.openButton.Click += new System.EventHandler(this.openButton_Click);
			// 
			// editButton
			// 
			this.editButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.editButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.editButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.editButton.Location = new System.Drawing.Point(3, 34);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(48, 25);
			this.editButton.TabIndex = 49;
			this.editButton.Text = "Edit";
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.editButton_Click);
			// 
			// clearButton
			// 
			this.clearButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.clearButton.Location = new System.Drawing.Point(3, 65);
			this.clearButton.Name = "clearButton";
			this.clearButton.Size = new System.Drawing.Size(48, 25);
			this.clearButton.TabIndex = 50;
			this.clearButton.Text = "Clear";
			this.clearButton.UseVisualStyleBackColor = true;
			this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
			// 
			// tableLayoutGridArea
			// 
			this.tableLayoutGridArea.ColumnCount = 2;
			this.tableLayoutGridArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutGridArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutGridArea.Controls.Add(this.flowLayoutImportExportButtons, 1, 0);
			this.tableLayoutGridArea.Controls.Add(this.renderedPicture, 0, 0);
			this.tableLayoutGridArea.Location = new System.Drawing.Point(50, 149);
			this.tableLayoutGridArea.Name = "tableLayoutGridArea";
			this.tableLayoutGridArea.RowCount = 1;
			this.tableLayoutGridArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutGridArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 101F));
			this.tableLayoutGridArea.Size = new System.Drawing.Size(162, 101);
			this.tableLayoutGridArea.TabIndex = 47;
			// 
			// renderedPicture
			// 
			this.renderedPicture.BackColor = System.Drawing.Color.Transparent;
			this.renderedPicture.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("renderedPicture.BackgroundImage")));
			this.renderedPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.renderedPicture.Location = new System.Drawing.Point(3, 3);
			this.renderedPicture.Name = "renderedPicture";
			this.renderedPicture.Size = new System.Drawing.Size(96, 95);
			this.renderedPicture.TabIndex = 20;
			this.renderedPicture.TabStop = false;
			this.renderedPicture.DoubleClick += new System.EventHandler(this.renderedPicture_DoubleClick);
			// 
			// notesLabel
			// 
			this.notesLabel.AutoSize = true;
			this.notesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.notesLabel.Location = new System.Drawing.Point(14, 272);
			this.notesLabel.Name = "notesLabel";
			this.notesLabel.Size = new System.Drawing.Size(41, 15);
			this.notesLabel.TabIndex = 48;
			this.notesLabel.Text = "Notes:";
			// 
			// notesTextBox
			// 
			this.notesTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.notesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.notesTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.notesTextBox.Location = new System.Drawing.Point(59, 270);
			this.notesTextBox.Multiline = true;
			this.notesTextBox.Name = "notesTextBox";
			this.notesTextBox.Size = new System.Drawing.Size(177, 55);
			this.notesTextBox.TabIndex = 49;
			// 
			// LipSyncMapMatrixEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(258, 384);
			this.Controls.Add(this.notesTextBox);
			this.Controls.Add(this.notesLabel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.tableLayoutGridArea);
			this.Controls.Add(this.phonemeLabel);
			this.Controls.Add(this.nextPhonemeButton);
			this.Controls.Add(this.prevPhonemeButton);
			this.Controls.Add(this.phonemeIcon);
			this.Controls.Add(this.nameTextBox);
			this.Controls.Add(this.nameLabel);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.MinimumSize = new System.Drawing.Size(239, 422);
			this.Name = "LipSyncMapMatrixEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Map";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResizeEnd += new System.EventHandler(this.OnResizeEnd);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			((System.ComponentModel.ISupportInitialize)(this.phonemeIcon)).EndInit();
			this.flowLayoutImportExportButtons.ResumeLayout(false);
			this.tableLayoutGridArea.ResumeLayout(false);
			this.tableLayoutGridArea.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.renderedPicture)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label phonemeLabel;
        private System.Windows.Forms.Button nextPhonemeButton;
        private System.Windows.Forms.Button prevPhonemeButton;
        private System.Windows.Forms.PictureBox phonemeIcon;
        private System.Windows.Forms.TableLayoutPanel tableLayoutGridArea;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutImportExportButtons;
        private System.Windows.Forms.PictureBox renderedPicture;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label notesLabel;
        private System.Windows.Forms.TextBox notesTextBox;
    }
}










