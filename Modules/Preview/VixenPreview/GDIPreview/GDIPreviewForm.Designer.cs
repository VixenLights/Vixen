namespace VixenModules.Preview.VixenPreview
{
	partial class GDIPreviewForm
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
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusPixelsLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusPixels = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusFPS = new System.Windows.Forms.ToolStripStatusLabel();
			this.gdiControl = new VixenModules.Preview.VixenPreview.GDIControl();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusPixelsLabel,
            this.toolStripStatusPixels,
            this.toolStripStatusLabel1,
            this.toolStripStatusFPS});
			this.statusStrip.Location = new System.Drawing.Point(0, 349);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(768, 22);
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toolStripStatusPixelsLabel
			// 
			this.toolStripStatusPixelsLabel.Name = "toolStripStatusPixelsLabel";
			this.toolStripStatusPixelsLabel.Size = new System.Drawing.Size(42, 17);
			this.toolStripStatusPixelsLabel.Text = "Lights:";
			// 
			// toolStripStatusPixels
			// 
			this.toolStripStatusPixels.Name = "toolStripStatusPixels";
			this.toolStripStatusPixels.Size = new System.Drawing.Size(13, 17);
			this.toolStripStatusPixels.Text = "0";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(29, 17);
			this.toolStripStatusLabel1.Text = "FPS:";
			// 
			// toolStripStatusFPS
			// 
			this.toolStripStatusFPS.Name = "toolStripStatusFPS";
			this.toolStripStatusFPS.Size = new System.Drawing.Size(55, 17);
			this.toolStripStatusFPS.Text = "1,000,000";
			// 
			// gdiControl
			// 
			this.gdiControl.Background = null;
			this.gdiControl.BackgroundAlpha = 50;
			this.gdiControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gdiControl.FrameRate = ((long)(0));
			this.gdiControl.Location = new System.Drawing.Point(0, 0);
			this.gdiControl.Name = "gdiControl";
			this.gdiControl.Size = new System.Drawing.Size(768, 349);
			this.gdiControl.TabIndex = 2;
			// 
			// GDIPreviewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 371);
			this.Controls.Add(this.gdiControl);
			this.Controls.Add(this.statusStrip);
			this.Name = "GDIPreviewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Vixen Preview";
			this.Move += new System.EventHandler(this.GDIPreviewForm_Move);
			this.Resize += new System.EventHandler(this.GDIPreviewForm_Resize);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private GDIControl gdiControl;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixelsLabel;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixels;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFPS;

	}
}