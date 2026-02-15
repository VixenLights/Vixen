namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class EffectParameterPickerControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			labelParameterName = new Label();
			pictureParameterImage = new PictureBox();
			flowLayoutPanel1 = new FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)pictureParameterImage).BeginInit();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// labelParameterName
			// 
			labelParameterName.Location = new Point(4, 72);
			labelParameterName.Margin = new Padding(4, 0, 4, 0);
			labelParameterName.Name = "labelParameterName";
			labelParameterName.Size = new Size(60, 39);
			labelParameterName.TabIndex = 1;
			labelParameterName.Text = "Gradients";
			labelParameterName.TextAlign = ContentAlignment.MiddleCenter;
			labelParameterName.Click += labelParameterName_Click;
			// 
			// pictureParameterImage
			// 
			pictureParameterImage.Location = new Point(4, 4);
			pictureParameterImage.Margin = new Padding(4);
			pictureParameterImage.Name = "pictureParameterImage";
			pictureParameterImage.Size = new Size(64, 64);
			pictureParameterImage.TabIndex = 2;
			pictureParameterImage.TabStop = false;
			pictureParameterImage.Click += pictureParameterImage_Click;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Controls.Add(pictureParameterImage);
			flowLayoutPanel1.Controls.Add(labelParameterName);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(0, 0);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(75, 115);
			flowLayoutPanel1.TabIndex = 3;
			// 
			// EffectParameterPickerControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(flowLayoutPanel1);
			Margin = new Padding(4);
			Name = "EffectParameterPickerControl";
			Size = new Size(75, 115);
			((System.ComponentModel.ISupportInitialize)pictureParameterImage).EndInit();
			flowLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelParameterName;
		private System.Windows.Forms.PictureBox pictureParameterImage;
		private FlowLayoutPanel flowLayoutPanel1;
	}
}
