namespace Scheduler {
	partial class ScheduleDay {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);

			_backgroundBrush.Dispose();
			_hourPen.Dispose();
			_halfHourPen.Dispose();
			_timeLargeFont.Dispose();
			_timeSmallFont.Dispose();
			_dayViewHeaderFont.Dispose();
			_timeLinePen.Dispose();
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// vScrollBar
			// 
			this.vScrollBar.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.vScrollBar.Location = new System.Drawing.Point(311, 113);
			this.vScrollBar.Maximum = 47;
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 206);
			this.vScrollBar.TabIndex = 0;
			this.vScrollBar.Visible = false;
			this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
			// 
			// ScheduleDay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.vScrollBar);
			this.Name = "ScheduleDay";
			this.Size = new System.Drawing.Size(328, 319);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar;

	}
}
