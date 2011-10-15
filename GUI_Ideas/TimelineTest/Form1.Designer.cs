namespace Timeline
{
    partial class Form1
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
			this.buttonElemAt = new System.Windows.Forms.Button();
			this.buttonReset = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.buttonZoomIn = new System.Windows.Forms.Button();
			this.buttonZoomOut = new System.Windows.Forms.Button();
			this.buttonGenericDebug = new System.Windows.Forms.Button();
			this.buttonGenericDebug2 = new System.Windows.Forms.Button();
			this.timelineControl1 = new CommonElements.Timeline.TimelineControl();
			this.buttonAlignLeft = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonElemAt
			// 
			this.buttonElemAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonElemAt.Location = new System.Drawing.Point(13, 597);
			this.buttonElemAt.Name = "buttonElemAt";
			this.buttonElemAt.Size = new System.Drawing.Size(115, 23);
			this.buttonElemAt.TabIndex = 0;
			this.buttonElemAt.Text = "Elements at 0:02";
			this.buttonElemAt.UseVisualStyleBackColor = true;
			this.buttonElemAt.Click += new System.EventHandler(this.buttonElemAt_Click);
			// 
			// buttonReset
			// 
			this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonReset.Location = new System.Drawing.Point(145, 596);
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Size = new System.Drawing.Size(75, 23);
			this.buttonReset.TabIndex = 1;
			this.buttonReset.Text = "Reset";
			this.buttonReset.UseVisualStyleBackColor = true;
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(395, 472);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(524, 147);
			this.textBoxLog.TabIndex = 3;
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonZoomIn.Location = new System.Drawing.Point(12, 554);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(75, 23);
			this.buttonZoomIn.TabIndex = 4;
			this.buttonZoomIn.Text = "Zoom In";
			this.buttonZoomIn.UseVisualStyleBackColor = true;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonZoomOut.Location = new System.Drawing.Point(104, 554);
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(75, 23);
			this.buttonZoomOut.TabIndex = 5;
			this.buttonZoomOut.Text = "Zoom Out";
			this.buttonZoomOut.UseVisualStyleBackColor = true;
			this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
			// 
			// buttonGenericDebug
			// 
			this.buttonGenericDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonGenericDebug.Location = new System.Drawing.Point(234, 533);
			this.buttonGenericDebug.Name = "buttonGenericDebug";
			this.buttonGenericDebug.Size = new System.Drawing.Size(119, 44);
			this.buttonGenericDebug.TabIndex = 6;
			this.buttonGenericDebug.Text = "Generic Debug";
			this.buttonGenericDebug.UseVisualStyleBackColor = true;
			this.buttonGenericDebug.Click += new System.EventHandler(this.buttonGenericDebug_Click);
			// 
			// buttonGenericDebug2
			// 
			this.buttonGenericDebug2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonGenericDebug2.Location = new System.Drawing.Point(234, 483);
			this.buttonGenericDebug2.Name = "buttonGenericDebug2";
			this.buttonGenericDebug2.Size = new System.Drawing.Size(119, 44);
			this.buttonGenericDebug2.TabIndex = 7;
			this.buttonGenericDebug2.Text = "Generic Debug 2";
			this.buttonGenericDebug2.UseVisualStyleBackColor = true;
			this.buttonGenericDebug2.Click += new System.EventHandler(this.buttonGenericDebug2_Click);
			// 
			// timelineControl1
			// 
			this.timelineControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineControl1.AutoSize = true;
			this.timelineControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.timelineControl1.Location = new System.Drawing.Point(12, 12);
			this.timelineControl1.Name = "timelineControl1";
			this.timelineControl1.Size = new System.Drawing.Size(907, 444);
			this.timelineControl1.TabIndex = 8;
			//this.timelineControl1.TimePerPixel = System.TimeSpan.Parse("00:00:00.0100000");
			this.timelineControl1.TotalTime = System.TimeSpan.Parse("00:02:00");
			this.timelineControl1.VerticalOffset = 0;
			//this.timelineControl1.VisibleTimeEnd = System.TimeSpan.Parse("00:00:06.1600000");
			//this.timelineControl1.VisibleTimeStart = System.TimeSpan.Parse("00:00:00");
			// 
			// buttonAlignLeft
			// 
			this.buttonAlignLeft.Location = new System.Drawing.Point(13, 521);
			this.buttonAlignLeft.Name = "buttonAlignLeft";
			this.buttonAlignLeft.Size = new System.Drawing.Size(41, 23);
			this.buttonAlignLeft.TabIndex = 9;
			this.buttonAlignLeft.Text = "| <--";
			this.buttonAlignLeft.UseVisualStyleBackColor = true;
			this.buttonAlignLeft.Click += new System.EventHandler(this.buttonAlignLeft_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(931, 641);
			this.Controls.Add(this.buttonAlignLeft);
			this.Controls.Add(this.timelineControl1);
			this.Controls.Add(this.buttonGenericDebug2);
			this.Controls.Add(this.buttonGenericDebug);
			this.Controls.Add(this.buttonZoomOut);
			this.Controls.Add(this.buttonZoomIn);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.buttonReset);
			this.Controls.Add(this.buttonElemAt);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonElemAt;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Button buttonZoomIn;
		private System.Windows.Forms.Button buttonZoomOut;
		private System.Windows.Forms.Button buttonGenericDebug;
		private System.Windows.Forms.Button buttonGenericDebug2;
		private CommonElements.Timeline.TimelineControl timelineControl1;
		private System.Windows.Forms.Button buttonAlignLeft;
    }
}

