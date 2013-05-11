namespace VixenModules.App.Scheduler {
	partial class DayPanel {
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
			_timeLinePen.Dispose();
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// DayPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Name = "DayPanel";
			this.Load += new System.EventHandler(this.DayPanel_Load);
			this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DayPanel_MouseDoubleClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DayPanel_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
