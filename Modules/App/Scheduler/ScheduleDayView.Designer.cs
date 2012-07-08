namespace VixenModules.App.Scheduler {
	partial class ScheduleDayView {
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

			//_backgroundBrush.Dispose();
			//_hourPen.Dispose();
			//_halfHourPen.Dispose();
			//_timeLargeFont.Dispose();
			//_timeSmallFont.Dispose();
			//_dayViewHeaderFont.Dispose();
			//_timeLinePen.Dispose();
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dayPanel = new VixenModules.App.Scheduler.DayPanel();
			this.headerPanel = new VixenModules.App.Scheduler.HeaderPanel();
			this.SuspendLayout();
			// 
			// dayPanel
			// 
			this.dayPanel.AutoScroll = true;
			this.dayPanel.AutoScrollMinSize = new System.Drawing.Size(150, 960);
			this.dayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dayPanel.Location = new System.Drawing.Point(0, 30);
			this.dayPanel.Name = "dayPanel";
			this.dayPanel.Size = new System.Drawing.Size(328, 289);
			this.dayPanel.TabIndex = 3;
			// 
			// headerPanel
			// 
			this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerPanel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
			this.headerPanel.Location = new System.Drawing.Point(0, 0);
			this.headerPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(328, 30);
			this.headerPanel.TabIndex = 0;
			// 
			// ScheduleDayView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.dayPanel);
			this.Controls.Add(this.headerPanel);
			this.Name = "ScheduleDayView";
			this.Size = new System.Drawing.Size(328, 319);
			this.ResumeLayout(false);

		}

		#endregion

		private HeaderPanel headerPanel;
		private DayPanel dayPanel;



	}
}
