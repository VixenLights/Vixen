namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class EffectTimeEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOk = new Button();
            label1 = new Label();
            label2 = new Label();
            btnCancel = new Button();
            txtStartTime = new TimeControl();
            txtDuration = new TimeControl();
            txtEndTime = new TimeControl();
            label3 = new Label();
            btnSetFullSequence = new Button();
            lblSetSequenceLength = new Label();
            SuspendLayout();
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(34, 197);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(87, 27);
            btnOk.TabIndex = 8;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 45);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 0;
            label1.Text = "Start";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 78);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 3;
            label2.Text = "Duration";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(129, 197);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(87, 27);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // txtStartTime
            // 
            txtStartTime.Location = new Point(82, 42);
            txtStartTime.Milliseconds = 0D;
            txtStartTime.Name = "txtStartTime";
            txtStartTime.Size = new Size(116, 23);
            txtStartTime.TabIndex = 1;
            txtStartTime.TimeSpan = TimeSpan.Parse("00:00:00");
            // 
            // txtDuration
            // 
            txtDuration.Location = new Point(82, 75);
            txtDuration.Milliseconds = 0D;
            txtDuration.Name = "txtDuration";
            txtDuration.Size = new Size(116, 23);
            txtDuration.TabIndex = 4;
            txtDuration.TimeSpan = TimeSpan.Parse("00:00:00");
            // 
            // txtEndTime
            // 
            txtEndTime.Location = new Point(82, 110);
            txtEndTime.Milliseconds = 0D;
            txtEndTime.Name = "txtEndTime";
            txtEndTime.Size = new Size(116, 23);
            txtEndTime.TabIndex = 6;
            txtEndTime.TimeSpan = TimeSpan.Parse("00:00:00");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(46, 112);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 5;
            label3.Text = "End";
            // 
            // btnSetFullSequence
            // 
            btnSetFullSequence.Location = new Point(177, 146);
            btnSetFullSequence.Name = "btnSetFullSequence";
            btnSetFullSequence.Size = new Size(39, 27);
            btnSetFullSequence.TabIndex = 8;
            btnSetFullSequence.Text = "Set";
            btnSetFullSequence.UseVisualStyleBackColor = false;
            btnSetFullSequence.Click += btnSetFullSequence_Click;
            // 
            // lblSetSequenceLength
            // 
            lblSetSequenceLength.AutoSize = true;
            lblSetSequenceLength.Location = new Point(20, 152);
            lblSetSequenceLength.Name = "lblSetSequenceLength";
            lblSetSequenceLength.Size = new Size(145, 15);
            lblSetSequenceLength.TabIndex = 7;
            lblSetSequenceLength.Text = "Match sequence start/end";
            // 
            // EffectTimeEditor
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            CancelButton = btnCancel;
            ClientSize = new Size(247, 236);
            Controls.Add(lblSetSequenceLength);
            Controls.Add(btnSetFullSequence);
            Controls.Add(txtEndTime);
            Controls.Add(label3);
            Controls.Add(txtDuration);
            Controls.Add(txtStartTime);
            Controls.Add(btnCancel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MaximumSize = new Size(263, 275);
            MinimizeBox = false;
            MinimumSize = new Size(263, 275);
            Name = "EffectTimeEditor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Effect Time Editor";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCancel;
		private TimeControl txtStartTime;
		private TimeControl txtDuration;
		private TimeControl txtEndTime;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnSetFullSequence;
		private System.Windows.Forms.Label lblSetSequenceLength;
	}
}