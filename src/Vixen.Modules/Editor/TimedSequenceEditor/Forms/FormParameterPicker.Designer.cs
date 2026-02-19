namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class FormParameterPicker
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
			flowLayoutPanel1 = new FlowLayoutPanel();
			SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			flowLayoutPanel1.BackColor = Color.FromArgb(68, 68, 68);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(1, 1);
			flowLayoutPanel1.Margin = new Padding(0);
			flowLayoutPanel1.MaximumSize = new Size(800, 800);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(724, 123);
			flowLayoutPanel1.TabIndex = 1;
			flowLayoutPanel1.Paint += flowLayoutPanel1_Paint;
			// 
			// FormParameterPicker
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BackColor = Color.FromArgb(221, 221, 221);
			ClientSize = new Size(726, 125);
			Controls.Add(flowLayoutPanel1);
			ForeColor = Color.FromArgb(221, 221, 221);
			FormBorderStyle = FormBorderStyle.None;
			Name = "FormParameterPicker";
			Padding = new Padding(1);
			Text = "FormParameterPicker";
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}