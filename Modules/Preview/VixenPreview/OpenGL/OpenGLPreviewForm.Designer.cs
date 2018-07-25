namespace VixenModules.Preview.VixenPreview.OpenGL
{
	partial class OpenGlPreviewForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.glControl = new OpenTK.GLControl();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusPixelsLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusPixels = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusFPS = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelDistance = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusDistance = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// glControl
			// 
			this.glControl.BackColor = System.Drawing.Color.Black;
			this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glControl.Location = new System.Drawing.Point(0, 0);
			this.glControl.Name = "glControl";
			this.glControl.Size = new System.Drawing.Size(284, 262);
			this.glControl.TabIndex = 0;
			this.glControl.VSync = false;
			this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
			this.glControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyDown);
			this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
			this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
			this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusPixelsLabel,
            this.toolStripStatusPixels,
            this.toolStripStatusLabelDistance,
            this.toolStripStatusDistance,
            this.toolStripStatusLabel1,
            this.toolStripStatusFPS});
			this.statusStrip.Location = new System.Drawing.Point(0, 240);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			this.statusStrip.Size = new System.Drawing.Size(284, 22);
			this.statusStrip.TabIndex = 2;
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
			this.toolStripStatusFPS.Size = new System.Drawing.Size(19, 17);
			this.toolStripStatusFPS.Text = "20";
			// 
			// toolStripStatusLabelDistance
			// 
			this.toolStripStatusLabelDistance.Name = "toolStripStatusLabelDistance";
			this.toolStripStatusLabelDistance.Size = new System.Drawing.Size(52, 17);
			this.toolStripStatusLabelDistance.Text = "Distance";
			// 
			// toolStripStatusDistance
			// 
			this.toolStripStatusDistance.Name = "toolStripStatusDistance";
			this.toolStripStatusDistance.Size = new System.Drawing.Size(13, 17);
			this.toolStripStatusDistance.Text = "0";
			// 
			// OpenGlPreviewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.glControl);
			this.Name = "OpenGlPreviewForm";
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreviewWindow_FormClosing);
			this.Move += new System.EventHandler(this.OpenGlPreviewForm_Move);
			this.Resize += new System.EventHandler(this.glControl_Resize);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenTK.GLControl glControl;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixelsLabel;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixels;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFPS;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDistance;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusDistance;
	}
}