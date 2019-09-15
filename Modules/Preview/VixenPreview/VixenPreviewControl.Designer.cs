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
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
			// 
			// VixenPreviewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "VixenPreviewControl";
			this.Size = new System.Drawing.Size(290, 240);
			this.Load += new System.EventHandler(this.VixenPreviewControl_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.VixenPreviewControl_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VixenPreviewControl_MouseUp);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.VixenPreviewControl_PreviewKeyDown);
			this.Resize += new System.EventHandler(this.VixenPreviewControl_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
	}
}
