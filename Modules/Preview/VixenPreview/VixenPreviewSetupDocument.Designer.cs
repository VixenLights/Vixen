namespace VixenModules.Preview.VixenPreview
{
    partial class VixenPreviewSetupDocument
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
			this.timerRender = new System.Windows.Forms.Timer(this.components);
			this.previewControl = new VixenModules.Preview.VixenPreview.VixenPreviewControl();
			this.SuspendLayout();
			// 
			// timerRender
			// 
			this.timerRender.Enabled = true;
			this.timerRender.Interval = 10;
			this.timerRender.Tick += new System.EventHandler(this.timerRender_Tick);
			// 
			// previewControl
			// 
			this.previewControl.BackgroundAlpha = 255;
			this.previewControl.CurrentTool = VixenModules.Preview.VixenPreview.VixenPreviewControl.Tools.Select;
			this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewControl.EditMode = false;
			this.previewControl.Location = new System.Drawing.Point(0, 0);
			this.previewControl.Name = "previewControl";
			this.previewControl.Paused = false;
			this.previewControl.ShowInfo = false;
			this.previewControl.Size = new System.Drawing.Size(758, 394);
			this.previewControl.TabIndex = 0;
			this.previewControl.ZoomLevel = 1D;
			// 
			// VixenPreviewSetupDocument
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(758, 394);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.Controls.Add(this.previewControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "VixenPreviewSetupDocument";
			this.Text = "Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenPreviewSetupDocument_FormClosing);
			this.Load += new System.EventHandler(this.VixenPreviewSetupDocument_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private VixenPreviewControl previewControl;
		private System.Windows.Forms.Timer timerRender;

    }
}