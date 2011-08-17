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
            this.tc = new Timeline.TimelineControl();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonElemAt
            // 
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
            this.buttonReset.Location = new System.Drawing.Point(145, 596);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 1;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // tc
            // 
            this.tc.AutoScroll = true;
            this.tc.AutoScrollOffset = new System.Drawing.Point(453, 0);
            this.tc.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tc.GridlineInterval = System.TimeSpan.Parse("00:00:01");
            this.tc.Location = new System.Drawing.Point(13, 12);
            this.tc.MaximumTime = System.TimeSpan.Parse("00:00:00");
            this.tc.Name = "tc";
            this.tc.RowSeparatorColor = System.Drawing.Color.Black;
            this.tc.Size = new System.Drawing.Size(906, 438);
            this.tc.TabIndex = 2;
            this.tc.VisibleTimeSpan = System.TimeSpan.Parse("00:00:10");
            this.tc.VisibleTimeStart = System.TimeSpan.Parse("00:00:04.9999875");
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(395, 533);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(524, 86);
            this.textBoxLog.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(931, 641);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.tc);
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
        private TimelineControl tc;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}

