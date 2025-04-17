namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class CreateEvenMarksForm
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
			components = new System.ComponentModel.Container();
			labelStart = new Label();
			txtStartTime = new MaskedTextBox();
			labelEnd = new Label();
			txtEndTime = new MaskedTextBox();
			labelDivide = new Label();
			updownDivide = new NumericUpDown();
			btnOk = new Button();
			btnCancel = new Button();
			toolTip = new ToolTip(components);
			SuspendLayout();
			// 
			// labelStart
			// 
			labelStart.AutoSize = true;
			labelStart.Location = new Point(50, 14);
			labelStart.Name = "labelStart";
			labelStart.Size = new Size(60, 15);
			labelStart.TabIndex = 0;
			labelStart.Text = "Start Time";
			// 
			// txtStartTime
			// 
			txtStartTime.Location = new Point(121, 14);
			txtStartTime.Name = "txtStartTime";
			txtStartTime.Size = new Size(116, 23);
			txtStartTime.TabIndex = 1;
			txtStartTime.MaskInputRejected += txtStartTime_MaskInputRejected;
			txtStartTime.KeyDown += txtStartTime_KeyDown;
			txtStartTime.KeyUp += txtStartTime_KeyUp;
			// 
			// labelEnd
			// 
			labelEnd.AutoSize = true;
			labelEnd.Location = new Point(54, 44);
			labelEnd.Name = "labelEnd";
			labelEnd.Size = new Size(56, 15);
			labelEnd.TabIndex = 2;
			labelEnd.Text = "End Time";
			// 
			// txtEndTime
			// 
			txtEndTime.Location = new Point(121, 44);
			txtEndTime.Name = "txtEndTime";
			txtEndTime.Size = new Size(116, 23);
			txtEndTime.TabIndex = 3;
			txtEndTime.MaskInputRejected += txtEndTime_MaskInputRejected;
			txtEndTime.KeyDown += txtEndTime_KeyDown;
			txtEndTime.KeyUp += txtEndTime_KeyUp;
			// 
			// labelDivide
			// 
			labelDivide.AutoSize = false;
			labelDivide.Location = new Point(45, 70);
			labelDivide.Name = "labelDivide";
			labelDivide.Size = new Size(70, 30);
			labelDivide.TabIndex = 4;
			labelDivide.Text = "Number of Divisions";
			// 
			// updownDivide
			// 
			updownDivide.Location = new Point(121, 74);
			updownDivide.Name = "updownDivide";
			updownDivide.Minimum = 1;
			updownDivide.Size = new Size(116, 23);
			updownDivide.TabIndex = 5;
			// 
			// btnOk
			// 
			btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new Point(114, 130);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(87, 27);
			btnOk.TabIndex = 6;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(208, 130);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(87, 27);
			btnCancel.TabIndex = 7;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// CreateEvenMarksForm
			// 
			AcceptButton = btnOk;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			BackColor = Color.FromArgb(68, 68, 68);
			CancelButton = btnCancel;
			ClientSize = new Size(307, 180);
			Controls.Add(labelStart);
			Controls.Add(txtStartTime);
			Controls.Add(labelEnd);
			Controls.Add(txtEndTime);
			Controls.Add(labelDivide);
			Controls.Add(updownDivide);
			Controls.Add(btnOk);
			Controls.Add(btnCancel);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "CreateEvenMarksForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Create Evenly Divided Marks";
			Load += CreateEvenMarksForm_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label labelStart;
		private System.Windows.Forms.MaskedTextBox txtStartTime;
		private System.Windows.Forms.Label labelEnd;
		private System.Windows.Forms.MaskedTextBox txtEndTime;
		private System.Windows.Forms.Label labelDivide;
		private System.Windows.Forms.NumericUpDown updownDivide;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ToolTip toolTip;

	}
}