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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.alignCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.startOffsetCombo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(12, 247);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(75, 23);
            this.convertButton.TabIndex = 0;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 157);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(218, 84);
            this.textBox.TabIndex = 2;
            // 
            // markCollectionCombo
            // 
            this.markCollectionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.markCollectionCombo.FormattingEnabled = true;
            this.markCollectionCombo.Location = new System.Drawing.Point(95, 20);
            this.markCollectionCombo.Name = "markCollectionCombo";
            this.markCollectionCombo.Size = new System.Drawing.Size(135, 21);
            this.markCollectionCombo.TabIndex = 4;
            this.markCollectionCombo.DropDown += new System.EventHandler(this.markCollectionCombo_DropDown);
            this.markCollectionCombo.SelectedIndexChanged += new System.EventHandler(this.markCollectionCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Mark Collection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Align";
            // 
            // alignCombo
            // 
            this.alignCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.alignCombo.FormattingEnabled = true;
            this.alignCombo.Items.AddRange(new object[] {
            "Phoneme",
            "Word",
            "Phrase"});
            this.alignCombo.Location = new System.Drawing.Point(95, 94);
            this.alignCombo.Name = "alignCombo";
            this.alignCombo.Size = new System.Drawing.Size(135, 21);
            this.alignCombo.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Text";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Start Offset";
            // 
            // startOffsetCombo
            // 
            this.startOffsetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startOffsetCombo.FormattingEnabled = true;
            this.startOffsetCombo.Location = new System.Drawing.Point(95, 57);
            this.startOffsetCombo.Name = "startOffsetCombo";
            this.startOffsetCombo.Size = new System.Drawing.Size(135, 21);
            this.startOffsetCombo.TabIndex = 10;
            this.startOffsetCombo.DropDown += new System.EventHandler(this.startOffsetCombo_DropDown);
            // 
            // LipSyncTextConvertForm
            // 
            this.AcceptButton = this.convertButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 282);
            this.Controls.Add(this.startOffsetCombo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.alignCombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.markCollectionCombo);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.convertButton);
            this.Name = "LipSyncTextConvertForm";
            this.Text = "LipSync Text Convert";
            this.Load += new System.EventHandler(this.LipSyncTextConvert_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.ComboBox markCollectionCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox alignCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox startOffsetCombo;
    }
}