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
			labelParameterName.Anchor = AnchorStyles.Top;
			labelParameterName.AutoSize = true;
			labelParameterName.Location = new Point(4, 72);
			labelParameterName.Margin = new Padding(4, 0, 4, 5);
			labelParameterName.MaximumSize = new Size(64, 0);
			labelParameterName.MinimumSize = new Size(64, 15);
			labelParameterName.Name = "labelParameterName";
			labelParameterName.Size = new Size(64, 15);
			labelParameterName.TabIndex = 1;
			labelParameterName.Text = "Gradients";
			labelParameterName.TextAlign = ContentAlignment.MiddleCenter;
			labelParameterName.Click += labelParameterName_Click;
			// 
			// pictureParameterImage
			// 
			pictureParameterImage.Anchor = AnchorStyles.Top;
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
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.Controls.Add(pictureParameterImage);
			flowLayoutPanel1.Controls.Add(labelParameterName);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(0, 0);
			flowLayoutPanel1.Margin = new Padding(0);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(72, 92);
			flowLayoutPanel1.TabIndex = 3;
			flowLayoutPanel1.WrapContents = false;
			// 
			// EffectParameterPickerControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Controls.Add(flowLayoutPanel1);
			Margin = new Padding(4);
			Name = "EffectParameterPickerControl";
			Size = new Size(72, 92);
			((System.ComponentModel.ISupportInitialize)pictureParameterImage).EndInit();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelParameterName;
		private System.Windows.Forms.PictureBox pictureParameterImage;
		private FlowLayoutPanel flowLayoutPanel1;
	}
}
