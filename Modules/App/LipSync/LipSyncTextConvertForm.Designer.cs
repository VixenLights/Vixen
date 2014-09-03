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
            this.convertButton = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.markCollectionCombo = new System.Windows.Forms.ComboBox();
            this.markCollectionLabel = new System.Windows.Forms.Label();
            this.markAlignmentLabel = new System.Windows.Forms.Label();
            this.alignCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.markStartOffsetLabel = new System.Windows.Forms.Label();
            this.startOffsetCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.markCollectionRadio = new System.Windows.Forms.RadioButton();
            this.cursorRadio = new System.Windows.Forms.RadioButton();
            this.clipBoardRadio = new System.Windows.Forms.RadioButton();
            this.marksGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.marksGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(9, 293);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(75, 23);
            this.convertButton.TabIndex = 0;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(9, 258);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(229, 20);
            this.textBox.TabIndex = 2;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // markCollectionCombo
            // 
            this.markCollectionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.markCollectionCombo.FormattingEnabled = true;
            this.markCollectionCombo.Location = new System.Drawing.Point(83, 20);
            this.markCollectionCombo.Name = "markCollectionCombo";
            this.markCollectionCombo.Size = new System.Drawing.Size(135, 21);
            this.markCollectionCombo.Sorted = true;
            this.markCollectionCombo.TabIndex = 4;
            this.markCollectionCombo.SelectedIndexChanged += new System.EventHandler(this.markCollectionCombo_SelectedIndexChanged);
            // 
            // markCollectionLabel
            // 
            this.markCollectionLabel.AutoSize = true;
            this.markCollectionLabel.Location = new System.Drawing.Point(6, 23);
            this.markCollectionLabel.Name = "markCollectionLabel";
            this.markCollectionLabel.Size = new System.Drawing.Size(53, 13);
            this.markCollectionLabel.TabIndex = 5;
            this.markCollectionLabel.Text = "Collection";
            // 
            // markAlignmentLabel
            // 
            this.markAlignmentLabel.AutoSize = true;
            this.markAlignmentLabel.Location = new System.Drawing.Point(6, 83);
            this.markAlignmentLabel.Name = "markAlignmentLabel";
            this.markAlignmentLabel.Size = new System.Drawing.Size(53, 13);
            this.markAlignmentLabel.TabIndex = 6;
            this.markAlignmentLabel.Text = "Alignment";
            // 
            // alignCombo
            // 
            this.alignCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.alignCombo.FormattingEnabled = true;
            this.alignCombo.Location = new System.Drawing.Point(83, 79);
            this.alignCombo.Name = "alignCombo";
            this.alignCombo.Size = new System.Drawing.Size(135, 21);
            this.alignCombo.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Text to Convert";
            // 
            // markStartOffsetLabel
            // 
            this.markStartOffsetLabel.AutoSize = true;
            this.markStartOffsetLabel.Location = new System.Drawing.Point(6, 52);
            this.markStartOffsetLabel.Name = "markStartOffsetLabel";
            this.markStartOffsetLabel.Size = new System.Drawing.Size(60, 13);
            this.markStartOffsetLabel.TabIndex = 9;
            this.markStartOffsetLabel.Text = "Start Offset";
            // 
            // startOffsetCombo
            // 
            this.startOffsetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startOffsetCombo.FormattingEnabled = true;
            this.startOffsetCombo.Location = new System.Drawing.Point(83, 49);
            this.startOffsetCombo.Name = "startOffsetCombo";
            this.startOffsetCombo.Size = new System.Drawing.Size(135, 21);
            this.startOffsetCombo.Sorted = true;
            this.startOffsetCombo.TabIndex = 10;
            this.startOffsetCombo.DropDown += new System.EventHandler(this.startOffsetCombo_DropDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.markCollectionRadio);
            this.groupBox1.Controls.Add(this.cursorRadio);
            this.groupBox1.Controls.Add(this.clipBoardRadio);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 88);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target / Spacing";
            // 
            // markCollectionRadio
            // 
            this.markCollectionRadio.AutoSize = true;
            this.markCollectionRadio.Location = new System.Drawing.Point(6, 65);
            this.markCollectionRadio.Name = "markCollectionRadio";
            this.markCollectionRadio.Size = new System.Drawing.Size(117, 17);
            this.markCollectionRadio.TabIndex = 2;
            this.markCollectionRadio.TabStop = true;
            this.markCollectionRadio.Text = "Per Mark Collection";
            this.markCollectionRadio.UseVisualStyleBackColor = true;
            this.markCollectionRadio.CheckedChanged += new System.EventHandler(this.markCollectionRadio_CheckedChanged);
            // 
            // cursorRadio
            // 
            this.cursorRadio.AutoSize = true;
            this.cursorRadio.Location = new System.Drawing.Point(6, 19);
            this.cursorRadio.Name = "cursorRadio";
            this.cursorRadio.Size = new System.Drawing.Size(133, 17);
            this.cursorRadio.TabIndex = 1;
            this.cursorRadio.TabStop = true;
            this.cursorRadio.Text = "Cursor / Even Spacing";
            this.cursorRadio.UseVisualStyleBackColor = true;
            // 
            // clipBoardRadio
            // 
            this.clipBoardRadio.AutoSize = true;
            this.clipBoardRadio.Location = new System.Drawing.Point(6, 42);
            this.clipBoardRadio.Name = "clipBoardRadio";
            this.clipBoardRadio.Size = new System.Drawing.Size(147, 17);
            this.clipBoardRadio.TabIndex = 0;
            this.clipBoardRadio.TabStop = true;
            this.clipBoardRadio.Text = "Clipboard / Even Spacing";
            this.clipBoardRadio.UseVisualStyleBackColor = true;
            // 
            // marksGroupBox
            // 
            this.marksGroupBox.Controls.Add(this.markCollectionLabel);
            this.marksGroupBox.Controls.Add(this.markCollectionCombo);
            this.marksGroupBox.Controls.Add(this.startOffsetCombo);
            this.marksGroupBox.Controls.Add(this.alignCombo);
            this.marksGroupBox.Controls.Add(this.markStartOffsetLabel);
            this.marksGroupBox.Controls.Add(this.markAlignmentLabel);
            this.marksGroupBox.Location = new System.Drawing.Point(12, 111);
            this.marksGroupBox.Name = "marksGroupBox";
            this.marksGroupBox.Size = new System.Drawing.Size(229, 119);
            this.marksGroupBox.TabIndex = 12;
            this.marksGroupBox.TabStop = false;
            this.marksGroupBox.Text = "Marks";
            // 
            // LipSyncTextConvertForm
            // 
            this.AcceptButton = this.convertButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 333);
            this.Controls.Add(this.marksGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.convertButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LipSyncTextConvertForm";
            this.Text = "LipSync Text Convert";
            this.Activated += new System.EventHandler(this.LipSyncTextConvertForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LipSyncTextConvertForm_FormClosed);
            this.Load += new System.EventHandler(this.LipSyncTextConvert_Load);
            this.Shown += new System.EventHandler(this.LipSyncTextConvertForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.marksGroupBox.ResumeLayout(false);
            this.marksGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.ComboBox markCollectionCombo;
        private System.Windows.Forms.Label markCollectionLabel;
        private System.Windows.Forms.Label markAlignmentLabel;
        private System.Windows.Forms.ComboBox alignCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label markStartOffsetLabel;
        private System.Windows.Forms.ComboBox startOffsetCombo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton markCollectionRadio;
        private System.Windows.Forms.RadioButton cursorRadio;
        private System.Windows.Forms.RadioButton clipBoardRadio;
        private System.Windows.Forms.GroupBox marksGroupBox;
    }
}