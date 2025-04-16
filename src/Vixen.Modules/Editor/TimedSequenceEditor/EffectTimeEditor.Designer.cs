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
			this.components = new System.ComponentModel.Container();
			this.btnOk = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtStartTime = new TimeControl();
			this.txtDuration = new TimeControl();
			this.txtEndTime = new TimeControl();
			this.label3 = new System.Windows.Forms.Label();
			this.btnSetFullSequence = new System.Windows.Forms.Button();
			this.lblSetSequenceLength = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(34, 197);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(87, 27);
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(41, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Start";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "Duration";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(129, 197);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			// 
			// txtStartTime
			// 
			this.txtStartTime.Location = new System.Drawing.Point(82, 42);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.Size = new System.Drawing.Size(116, 23);
			this.txtStartTime.TabIndex = 1;
			// 
			// txtDuration
			// 
			this.txtDuration.Location = new System.Drawing.Point(82, 75);
			this.txtDuration.Name = "txtDuration";
			this.txtDuration.Size = new System.Drawing.Size(116, 23);
			this.txtDuration.TabIndex = 4;
			// 
			// txtEndTime
			// 
			this.txtEndTime.Location = new System.Drawing.Point(82, 110);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.Size = new System.Drawing.Size(116, 23);
			this.txtEndTime.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(46, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(27, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "End";
			// 
			// btnSetFullSequence
			// 
			this.btnSetFullSequence.Location = new System.Drawing.Point(177, 146);
			this.btnSetFullSequence.Name = "btnSetFullSequence";
			this.btnSetFullSequence.Size = new System.Drawing.Size(39, 27);
			this.btnSetFullSequence.TabIndex = 8;
			this.btnSetFullSequence.Text = "Set";
			this.btnSetFullSequence.UseVisualStyleBackColor = false;
			this.btnSetFullSequence.Click += new System.EventHandler(this.btnSetFullSequence_Click);
			// 
			// lblSetSequenceLength
			// 
			this.lblSetSequenceLength.AutoSize = true;
			this.lblSetSequenceLength.Location = new System.Drawing.Point(20, 152);
			this.lblSetSequenceLength.Name = "lblSetSequenceLength";
			this.lblSetSequenceLength.Size = new System.Drawing.Size(145, 15);
			this.lblSetSequenceLength.TabIndex = 7;
			this.lblSetSequenceLength.Text = "Match sequence start/end";
			// 
			// EffectTimeEditor
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(247, 236);
			this.Controls.Add(this.lblSetSequenceLength);
			this.Controls.Add(this.btnSetFullSequence);
			this.Controls.Add(this.txtEndTime);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtDuration);
			this.Controls.Add(this.txtStartTime);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(263, 275);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(263, 275);
			this.Name = "EffectTimeEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Effect Time Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

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