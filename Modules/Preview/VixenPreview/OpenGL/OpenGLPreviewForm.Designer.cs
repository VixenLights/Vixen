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
			// OpenGlPreviewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.glControl);
			this.Name = "OpenGlPreviewForm";
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreviewWindow_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenTK.GLControl glControl;
	}
}