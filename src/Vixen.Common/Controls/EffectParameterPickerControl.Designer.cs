﻿namespace Common.Controls
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
			this.labelParameterName = new System.Windows.Forms.Label();
			this.pictureParameterImage = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureParameterImage)).BeginInit();
			this.SuspendLayout();
			// 
			// labelParameterName
			// 
			this.labelParameterName.Location = new System.Drawing.Point(-3, 72);
			this.labelParameterName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelParameterName.Name = "labelParameterName";
			this.labelParameterName.Size = new System.Drawing.Size(106, 42);
			this.labelParameterName.TabIndex = 1;
			this.labelParameterName.Text = "Parameter Name";
			this.labelParameterName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelParameterName.Click += new System.EventHandler(this.labelParameterName_Click);
			// 
			// pictureParameterImage
			// 
			this.pictureParameterImage.Location = new System.Drawing.Point(16, 4);
			this.pictureParameterImage.Margin = new System.Windows.Forms.Padding(4);
			this.pictureParameterImage.Name = "pictureParameterImage";
			this.pictureParameterImage.Size = new System.Drawing.Size(64, 64);
			this.pictureParameterImage.TabIndex = 2;
			this.pictureParameterImage.TabStop = false;
			this.pictureParameterImage.Click += new System.EventHandler(this.pictureParameterImage_Click);
			// 
			// EffectParameterPickerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureParameterImage);
			this.Controls.Add(this.labelParameterName);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "EffectParameterPickerControl";
			this.Size = new System.Drawing.Size(102, 123);
			((System.ComponentModel.ISupportInitialize)(this.pictureParameterImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelParameterName;
		private System.Windows.Forms.PictureBox pictureParameterImage;

	}
}
