namespace VixenModules.App.SimpleSchedule.Forms
{
    partial class ConfigureScheduledItems
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
            this.itemToExceuteGroupBox = new System.Windows.Forms.GroupBox();
            this.programLabel = new System.Windows.Forms.Label();
            this.sequenceLabel = new System.Windows.Forms.Label();
            this.selectProgramButton = new System.Windows.Forms.Button();
            this.selectSequenceButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.itemToExceuteGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // itemToExceuteGroupBox
            // 
            this.itemToExceuteGroupBox.Controls.Add(this.programLabel);
            this.itemToExceuteGroupBox.Controls.Add(this.sequenceLabel);
            this.itemToExceuteGroupBox.Controls.Add(this.selectProgramButton);
            this.itemToExceuteGroupBox.Controls.Add(this.selectSequenceButton);
            this.itemToExceuteGroupBox.Location = new System.Drawing.Point(12, 12);
            this.itemToExceuteGroupBox.Name = "itemToExceuteGroupBox";
            this.itemToExceuteGroupBox.Size = new System.Drawing.Size(267, 80);
            this.itemToExceuteGroupBox.TabIndex = 0;
            this.itemToExceuteGroupBox.TabStop = false;
            this.itemToExceuteGroupBox.Text = "Item to Execute";
            // 
            // programLabel
            // 
            this.programLabel.AutoSize = true;
            this.programLabel.Location = new System.Drawing.Point(113, 54);
            this.programLabel.Name = "programLabel";
            this.programLabel.Size = new System.Drawing.Size(0, 13);
            this.programLabel.TabIndex = 3;
            // 
            // sequenceLabel
            // 
            this.sequenceLabel.AutoSize = true;
            this.sequenceLabel.Location = new System.Drawing.Point(113, 25);
            this.sequenceLabel.Name = "sequenceLabel";
            this.sequenceLabel.Size = new System.Drawing.Size(0, 13);
            this.sequenceLabel.TabIndex = 2;
            // 
            // selectProgramButton
            // 
            this.selectProgramButton.Location = new System.Drawing.Point(7, 49);
            this.selectProgramButton.Name = "selectProgramButton";
            this.selectProgramButton.Size = new System.Drawing.Size(100, 23);
            this.selectProgramButton.TabIndex = 1;
            this.selectProgramButton.Text = "Select Program";
            this.selectProgramButton.UseVisualStyleBackColor = true;
            this.selectProgramButton.Click += new System.EventHandler(this.selectProgramButton_Click);
            // 
            // selectSequenceButton
            // 
            this.selectSequenceButton.Location = new System.Drawing.Point(7, 20);
            this.selectSequenceButton.Name = "selectSequenceButton";
            this.selectSequenceButton.Size = new System.Drawing.Size(100, 23);
            this.selectSequenceButton.TabIndex = 0;
            this.selectSequenceButton.Text = "Select Sequence";
            this.selectSequenceButton.UseVisualStyleBackColor = true;
            this.selectSequenceButton.Click += new System.EventHandler(this.selectSequenceButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(123, 98);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(204, 98);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Select Sequence";
            // 
            // ConfigureScheduledItems
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(290, 137);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.itemToExceuteGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureScheduledItems";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Schedule Item(s)";
            this.itemToExceuteGroupBox.ResumeLayout(false);
            this.itemToExceuteGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox itemToExceuteGroupBox;
        private System.Windows.Forms.Button selectProgramButton;
        private System.Windows.Forms.Button selectSequenceButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label programLabel;
        private System.Windows.Forms.Label sequenceLabel;
    }
}