namespace VixenModules.Preview.VixenPreview
{
	partial class VixenPreviewDisplayD2D
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
			this.components = new System.ComponentModel.Container();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLastRenderTime = new System.Windows.Forms.ToolStripStatusLabel();
			this.timerStatus = new System.Windows.Forms.Timer(this.components);
			this.direct2DControlWinForm1 = new Common.Controls.Direct2D.Direct2DControlWinForm();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLastRenderTime});
			this.statusStrip1.Location = new System.Drawing.Point(0, 419);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(878, 22);
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(90, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatus1";
			// 
			// toolStripStatusLastRenderTime
			// 
			this.toolStripStatusLastRenderTime.Name = "toolStripStatusLastRenderTime";
			this.toolStripStatusLastRenderTime.Size = new System.Drawing.Size(169, 17);
			this.toolStripStatusLastRenderTime.Text = "toolStripStatusLastRenderTime";
			 
			// 
			// direct2DControlWinForm1
			// 
			this.direct2DControlWinForm1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.direct2DControlWinForm1.Location = new System.Drawing.Point(0, 0);
			this.direct2DControlWinForm1.Name = "direct2DControlWinForm1";
			this.direct2DControlWinForm1.Points = null;
			this.direct2DControlWinForm1.Size = new System.Drawing.Size(878, 419);
			this.direct2DControlWinForm1.TabIndex = 5;
			// 
			// VixenPreviewDisplayD2D
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(878, 441);
			this.Controls.Add(this.direct2DControlWinForm1);
			this.Controls.Add(this.statusStrip1);
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "VixenPreviewDisplayD2D";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenPreviewDisplay_FormClosing);
		 
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLastRenderTime;
        private System.Windows.Forms.Timer timerStatus;
		internal Common.Controls.Direct2D.Direct2DControlWinForm direct2DControlWinForm1;
    }
}