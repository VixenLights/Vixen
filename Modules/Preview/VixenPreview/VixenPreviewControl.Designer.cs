namespace VixenModules.Preview.VixenPreview
{
	partial class VixenPreviewControl
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
			if (disposing)
			{
				if (_background != null)
					_background.Dispose();
				if (_alphaBackground != null)
					_alphaBackground.Dispose();
				//if (_blankAlphaBackground != null)
				//    _blankAlphaBackground.Dispose();
				//if (_backgroundBrush != null)
				//    _backgroundBrush.Dispose();
				_highlightedElements.Clear();
			}
			_background = null;
			_alphaBackground = null;
			//_blankAlphaBackground = null;
			_highlightedElements = null;
			_selectedDisplayItems = null;
			//_backgroundBrush = null;

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
            // VixenPreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "VixenPreviewControl";
            this.Size = new System.Drawing.Size(387, 295);
            this.Load += new System.EventHandler(this.VixenPreviewControl_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VixenPreviewControl_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseUp);
            this.Resize += new System.EventHandler(this.VixenPreviewControl_Resize);
            this.ResumeLayout(false);

		}

		#endregion
	}
}
