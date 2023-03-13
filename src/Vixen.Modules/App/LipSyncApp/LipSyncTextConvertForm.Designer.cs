namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncTextConvertForm
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
			this.buttonConvert = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.markCollectionCombo = new System.Windows.Forms.ComboBox();
			this.alignCombo = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.startOffsetCombo = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.markCollectionRadio = new System.Windows.Forms.RadioButton();
			this.cursorRadio = new System.Windows.Forms.RadioButton();
			this.clipBoardRadio = new System.Windows.Forms.RadioButton();
			this.marksGroupBox = new System.Windows.Forms.GroupBox();
			this.markAlignmentLabel = new System.Windows.Forms.Label();
			this.markCollectionLabel = new System.Windows.Forms.Label();
			this.markStartOffsetLabel = new System.Windows.Forms.Label();
			this.checkBoxClearText = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.marksGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonConvert
			// 
			this.buttonConvert.BackColor = System.Drawing.Color.Transparent;
			this.buttonConvert.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonConvert.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonConvert.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonConvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonConvert.Location = new System.Drawing.Point(195, 369);
			this.buttonConvert.Name = "buttonConvert";
			this.buttonConvert.Size = new System.Drawing.Size(87, 27);
			this.buttonConvert.TabIndex = 0;
			this.buttonConvert.Text = "Convert";
			this.buttonConvert.UseVisualStyleBackColor = false;
			this.buttonConvert.Click += new System.EventHandler(this.convertButton_Click);
			this.buttonConvert.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonConvert.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// textBox
			// 
			this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.textBox.Location = new System.Drawing.Point(10, 298);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(271, 53);
			this.textBox.TabIndex = 2;
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// markCollectionCombo
			// 
			this.markCollectionCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.markCollectionCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.markCollectionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.markCollectionCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.markCollectionCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.markCollectionCombo.FormattingEnabled = true;
			this.markCollectionCombo.Location = new System.Drawing.Point(19, 27);
			this.markCollectionCombo.Name = "markCollectionCombo";
			this.markCollectionCombo.Size = new System.Drawing.Size(157, 24);
			this.markCollectionCombo.Sorted = true;
			this.markCollectionCombo.TabIndex = 4;
			this.markCollectionCombo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.markCollectionCombo.SelectedIndexChanged += new System.EventHandler(this.markCollectionCombo_SelectedIndexChanged);
			this.markCollectionCombo.EnabledChanged += new System.EventHandler(this.markCollectionCombo_EnabledChanged);
			// 
			// alignCombo
			// 
			this.alignCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.alignCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.alignCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.alignCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.alignCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.alignCombo.FormattingEnabled = true;
			this.alignCombo.Location = new System.Drawing.Point(19, 95);
			this.alignCombo.Name = "alignCombo";
			this.alignCombo.Size = new System.Drawing.Size(157, 24);
			this.alignCombo.TabIndex = 7;
			this.alignCombo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 278);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "Text to Convert";
			// 
			// startOffsetCombo
			// 
			this.startOffsetCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.startOffsetCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.startOffsetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.startOffsetCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.startOffsetCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.startOffsetCombo.FormattingEnabled = true;
			this.startOffsetCombo.Location = new System.Drawing.Point(19, 60);
			this.startOffsetCombo.Name = "startOffsetCombo";
			this.startOffsetCombo.Size = new System.Drawing.Size(157, 24);
			this.startOffsetCombo.Sorted = true;
			this.startOffsetCombo.TabIndex = 10;
			this.startOffsetCombo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.startOffsetCombo.DropDown += new System.EventHandler(this.startOffsetCombo_DropDown);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.markCollectionRadio);
			this.groupBox1.Controls.Add(this.cursorRadio);
			this.groupBox1.Controls.Add(this.clipBoardRadio);
			this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox1.Location = new System.Drawing.Point(87, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(194, 102);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Target / Spacing";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// markCollectionRadio
			// 
			this.markCollectionRadio.AutoSize = true;
			this.markCollectionRadio.Location = new System.Drawing.Point(7, 75);
			this.markCollectionRadio.Name = "markCollectionRadio";
			this.markCollectionRadio.Size = new System.Drawing.Size(129, 19);
			this.markCollectionRadio.TabIndex = 2;
			this.markCollectionRadio.TabStop = true;
			this.markCollectionRadio.Text = "Per Mark Collection";
			this.markCollectionRadio.UseVisualStyleBackColor = true;
			this.markCollectionRadio.CheckedChanged += new System.EventHandler(this.markCollectionRadio_CheckedChanged);
			// 
			// cursorRadio
			// 
			this.cursorRadio.AutoSize = true;
			this.cursorRadio.Location = new System.Drawing.Point(7, 22);
			this.cursorRadio.Name = "cursorRadio";
			this.cursorRadio.Size = new System.Drawing.Size(141, 19);
			this.cursorRadio.TabIndex = 1;
			this.cursorRadio.TabStop = true;
			this.cursorRadio.Text = "Cursor / Even Spacing";
			this.cursorRadio.UseVisualStyleBackColor = true;
			// 
			// clipBoardRadio
			// 
			this.clipBoardRadio.AutoSize = true;
			this.clipBoardRadio.Location = new System.Drawing.Point(7, 48);
			this.clipBoardRadio.Name = "clipBoardRadio";
			this.clipBoardRadio.Size = new System.Drawing.Size(158, 19);
			this.clipBoardRadio.TabIndex = 0;
			this.clipBoardRadio.TabStop = true;
			this.clipBoardRadio.Text = "Clipboard / Even Spacing";
			this.clipBoardRadio.UseVisualStyleBackColor = true;
			// 
			// marksGroupBox
			// 
			this.marksGroupBox.BackColor = System.Drawing.Color.Transparent;
			this.marksGroupBox.Controls.Add(this.markCollectionCombo);
			this.marksGroupBox.Controls.Add(this.startOffsetCombo);
			this.marksGroupBox.Controls.Add(this.alignCombo);
			this.marksGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.marksGroupBox.Location = new System.Drawing.Point(87, 128);
			this.marksGroupBox.Name = "marksGroupBox";
			this.marksGroupBox.Size = new System.Drawing.Size(194, 137);
			this.marksGroupBox.TabIndex = 12;
			this.marksGroupBox.TabStop = false;
			this.marksGroupBox.Text = "Marks";
			this.marksGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// markAlignmentLabel
			// 
			this.markAlignmentLabel.AutoSize = true;
			this.markAlignmentLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.markAlignmentLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.markAlignmentLabel.Location = new System.Drawing.Point(17, 226);
			this.markAlignmentLabel.Name = "markAlignmentLabel";
			this.markAlignmentLabel.Size = new System.Drawing.Size(63, 15);
			this.markAlignmentLabel.TabIndex = 6;
			this.markAlignmentLabel.Text = "Alignment";
			this.markAlignmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// markCollectionLabel
			// 
			this.markCollectionLabel.AutoSize = true;
			this.markCollectionLabel.Location = new System.Drawing.Point(17, 158);
			this.markCollectionLabel.Name = "markCollectionLabel";
			this.markCollectionLabel.Size = new System.Drawing.Size(61, 15);
			this.markCollectionLabel.TabIndex = 5;
			this.markCollectionLabel.Text = "Collection";
			// 
			// markStartOffsetLabel
			// 
			this.markStartOffsetLabel.AutoSize = true;
			this.markStartOffsetLabel.BackColor = System.Drawing.Color.Transparent;
			this.markStartOffsetLabel.Location = new System.Drawing.Point(17, 192);
			this.markStartOffsetLabel.Name = "markStartOffsetLabel";
			this.markStartOffsetLabel.Size = new System.Drawing.Size(66, 15);
			this.markStartOffsetLabel.TabIndex = 9;
			this.markStartOffsetLabel.Text = "Start Offset";
			// 
			// checkBoxClearText
			// 
			this.checkBoxClearText.AutoSize = true;
			this.checkBoxClearText.Location = new System.Drawing.Point(140, 278);
			this.checkBoxClearText.Name = "checkBoxClearText";
			this.checkBoxClearText.Size = new System.Drawing.Size(141, 19);
			this.checkBoxClearText.TabIndex = 13;
			this.checkBoxClearText.Text = "Clear after conversion";
			this.checkBoxClearText.UseVisualStyleBackColor = true;
			// 
			// LipSyncTextConvertForm
			// 
			this.AcceptButton = this.buttonConvert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(300, 403);
			this.Controls.Add(this.checkBoxClearText);
			this.Controls.Add(this.markAlignmentLabel);
			this.Controls.Add(this.marksGroupBox);
			this.Controls.Add(this.markCollectionLabel);
			this.Controls.Add(this.markStartOffsetLabel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.buttonConvert);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximumSize = new System.Drawing.Size(316, 437);
			this.MinimumSize = new System.Drawing.Size(316, 437);
			this.Name = "LipSyncTextConvertForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "LipSync Text Convert";
			this.Activated += new System.EventHandler(this.LipSyncTextConvertForm_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LipSyncTextConvertForm_FormClosed);
			this.Load += new System.EventHandler(this.LipSyncTextConvert_Load);
			this.Shown += new System.EventHandler(this.LipSyncTextConvertForm_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.marksGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.ComboBox markCollectionCombo;
        private System.Windows.Forms.ComboBox alignCombo;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox startOffsetCombo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton markCollectionRadio;
        private System.Windows.Forms.RadioButton cursorRadio;
        private System.Windows.Forms.RadioButton clipBoardRadio;
		private System.Windows.Forms.GroupBox marksGroupBox;
		private System.Windows.Forms.Label markCollectionLabel;
		private System.Windows.Forms.Label markStartOffsetLabel;
		private System.Windows.Forms.Label markAlignmentLabel;
		private System.Windows.Forms.CheckBox checkBoxClearText;
    }
}