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
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.gdiControl = new VixenModules.Preview.VixenPreview.GDIControl();
			this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
			this.statusStrip.Location = new System.Drawing.Point(0, 349);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(768, 22);
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(112, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel";
			// 
			// gdiControl
			// 
			this.gdiControl.Background = null;
			this.gdiControl.BackgroundAlpha = 50;
			this.gdiControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gdiControl.Location = new System.Drawing.Point(0, 0);
			this.gdiControl.Name = "gdiControl";
			this.gdiControl.Size = new System.Drawing.Size(768, 349);
			this.gdiControl.TabIndex = 2;
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
			// 
			// GDIPreviewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 371);
			this.Controls.Add(this.gdiControl);
			this.Controls.Add(this.statusStrip);
			this.Name = "GDIPreviewForm";
			this.Text = "Vixen Preview";
			this.Load += new System.EventHandler(this.GDIPreviewForm_Load);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private GDIControl gdiControl;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;

	}
}