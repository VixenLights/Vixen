namespace Common.Controls
{
	partial class NumberDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumberDialog));
			button2 = new Button();
			button1 = new Button();
			labelPrompt = new Label();
			numericUpDownChooser = new NumericUpDown();
			((System.ComponentModel.ISupportInitialize)numericUpDownChooser).BeginInit();
			SuspendLayout();
			// 
			// button2
			// 
			button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button2.DialogResult = DialogResult.Cancel;
			button2.Location = new Point(258, 82);
			button2.Margin = new Padding(4, 3, 4, 3);
			button2.Name = "button2";
			button2.Size = new Size(93, 29);
			button2.TabIndex = 3;
			button2.Text = "Cancel";
			button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button1.DialogResult = DialogResult.OK;
			button1.Location = new Point(158, 82);
			button1.Margin = new Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new Size(93, 29);
			button1.TabIndex = 2;
			button1.Text = "OK";
			button1.UseVisualStyleBackColor = true;
			// 
			// labelPrompt
			// 
			labelPrompt.AutoSize = true;
			labelPrompt.Location = new Point(14, 31);
			labelPrompt.Margin = new Padding(4, 0, 4, 0);
			labelPrompt.Name = "labelPrompt";
			labelPrompt.Size = new Size(15, 15);
			labelPrompt.TabIndex = 4;
			labelPrompt.Text = "[]";
			// 
			// numericUpDownChooser
			// 
			numericUpDownChooser.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			numericUpDownChooser.Location = new Point(253, 29);
			numericUpDownChooser.Margin = new Padding(4, 3, 4, 3);
			numericUpDownChooser.Name = "numericUpDownChooser";
			numericUpDownChooser.Size = new Size(98, 23);
			numericUpDownChooser.TabIndex = 1;
			numericUpDownChooser.KeyDown += numericUpDownChooser_KeyDown;
			// 
			// NumberDialog
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(365, 125);
			Controls.Add(numericUpDownChooser);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(labelPrompt);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			KeyPreview = true;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "NumberDialog";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			((System.ComponentModel.ISupportInitialize)numericUpDownChooser).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label labelPrompt;
		private System.Windows.Forms.NumericUpDown numericUpDownChooser;
	}
}