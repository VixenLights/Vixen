namespace VixenModules.Preview.VixenPreview
{
    partial class VixenPreviewDisplay
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
			this.preview = new VixenModules.Preview.VixenPreview.VixenPreviewControl();
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
			// timerStatus
			// 
			this.timerStatus.Enabled = true;
			this.timerStatus.Interval = 1000;
			this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.preview.BackgroundAlpha = 255;
			this.preview.CurrentTool = VixenModules.Preview.VixenPreview.VixenPreviewControl.Tools.Select;
			this.preview.EditMode = false;
			this.preview.Location = new System.Drawing.Point(0, 0);
			this.preview.Name = "preview";
			this.preview.Paused = false;
			this.preview.ShowInfo = false;
			this.preview.Size = new System.Drawing.Size(878, 416);
			this.preview.TabIndex = 0;
			// 
			// VixenPreviewDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(878, 441);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.preview);
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "VixenPreviewDisplay";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenPreviewDisplay_FormClosing);
			this.Load += new System.EventHandler(this.VixenPreviewDisplay_Load);
			this.Move += new System.EventHandler(this.VixenPreviewDisplay_Move);
			this.Resize += new System.EventHandler(this.VixenPreviewDisplay_Resize);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private VixenPreviewControl preview;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLastRenderTime;
        private System.Windows.Forms.Timer timerStatus;
    }
}