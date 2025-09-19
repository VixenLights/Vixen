namespace TimedSequenceEditor
{
	partial class SetSequenceLength
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
			label1 = new Label();
			timeControl = new TimeControl();
			buttonOK = new Button();
			buttonCancel = new Button();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(28, 23);
			label1.Name = "label1";
			label1.Size = new Size(152, 15);
			label1.TabIndex = 0;
			label1.Text = "Enter new sequence length:";
			// 
			// timeControl
			// 
			timeControl.Location = new Point(28, 46);
			timeControl.Name = "Time Control";
			timeControl.Size = new Size(152, 23);
			timeControl.TabIndex = 1;
			// 
			// buttonOK
			// 
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new Point(81, 98);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(75, 23);
			buttonOK.TabIndex = 2;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			buttonCancel.Location = new Point(162, 98);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(75, 23);
			buttonCancel.TabIndex = 3;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// SetSequenceLength
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = buttonCancel;
			ClientSize = new Size(270, 136);
			MaximizeBox = false;
			MinimizeBox = false;
			Controls.Add(label1);
			Controls.Add(timeControl);
			Controls.Add(buttonOK);
			Controls.Add(buttonCancel);
			Name = "SetSequenceLength";
			Text = "Sequence Length";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			ResumeLayout(false);
			PerformLayout();
		}
		#endregion

		private Label label1;
		private TimeControl timeControl;
		private Button buttonOK;
		private Button buttonCancel;
	}
}