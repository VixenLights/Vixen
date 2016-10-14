namespace VixenModules.Preview.VixenPreview
{
	partial class GDIControl
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
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
					components = null;
				}
				if (_fastPixel != null)
				{
					_fastPixel.Dispose();
					_fastPixel = null;
				}
				if (_backgroundAlphaImage != null)
				{
					_backgroundAlphaImage.Dispose();
					_backgroundAlphaImage = null;
				}
				if (_background != null)
				{
					_background.Dispose();
					_background = null;
				}
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
			this.SuspendLayout();
			// 
			// GDIControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.Name = "GDIControl";
			this.Size = new System.Drawing.Size(626, 379);
			this.Resize += new System.EventHandler(this.GDIControl_Resize);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
