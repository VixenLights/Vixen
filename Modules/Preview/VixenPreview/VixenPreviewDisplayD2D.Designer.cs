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
			this.timerStatus = new System.Windows.Forms.Timer(this.components);
			this.previewWinform1 = new VixenModules.Preview.VixenPreview.Direct2D.PreviewWinform();
			this.SuspendLayout();
			// 
			// previewWinform1
			// 
			this.previewWinform1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewWinform1.Location = new System.Drawing.Point(0, 0);
			this.previewWinform1.Name = "previewWinform1";
			this.previewWinform1.Scene = null;
			this.previewWinform1.Size = new System.Drawing.Size(878, 442);
			this.previewWinform1.TabIndex = 0;
			// 
			// VixenPreviewDisplayD2D
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(878, 442);
			this.Controls.Add(this.previewWinform1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimumSize = new System.Drawing.Size(400, 200);
			this.Name = "VixenPreviewDisplayD2D";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenPreviewDisplay_FormClosing);
			this.Move += new System.EventHandler(this.VixenPreviewDisplay_Move);
			this.Resize += new System.EventHandler(this.VixenPreviewDisplay_Resize);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Timer timerStatus;
		private Direct2D.PreviewWinform previewWinform1;
	    }
}