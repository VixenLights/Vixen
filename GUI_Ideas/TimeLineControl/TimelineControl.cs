using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Timeline
{
	public partial class TimelineControl : UserControl
	{
		public TimelineControl()
		{
			InitializeComponent();

			this.DoubleBuffered = true;

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);
			VisibleTimeSpan = TimeSpan.FromSeconds(10);
			VisibleTimeStart = TimeSpan.FromSeconds(0);


			timelineGrid.Scroll += new ScrollEventHandler(OnGridScrolled);
		}

		#region Properties

		public TimeSpan TotalTime
		{
			get { return timelineGrid.TotalTime; }
			set
			{
				timelineGrid.TotalTime = value;
				timelineGrid.Invalidate();
			}
		}

		public TimeSpan VisibleTimeSpan
		{
			get { return timelineGrid.VisibleTimeSpan; }
			set
			{
				timelineGrid.VisibleTimeSpan = value;
				timelineGrid.Invalidate();
				timelineHeader.DisplayedTimeSpan = value;
				timelineHeader.Invalidate();
			}
		}

		public TimeSpan VisibleTimeStart
		{
			get { return timelineGrid.VisibleTimeStart; }
			set
			{
				timelineGrid.VisibleTimeStart = value;
				timelineGrid.Invalidate();
				timelineHeader.DisplayedTimeStart = value;
				timelineHeader.Invalidate();
			}
		}

		public TimeSpan VisibleTimeEnd
		{
			get { return timelineGrid.VisibleTimeEnd; }
			set { timelineGrid.VisibleTimeEnd = value; }
		}

		#endregion



		void OnGridScrolled(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				timelineHeader.DisplayedTimeStart = timelineGrid.VisibleTimeStart;
				timelineHeader.Invalidate();
			}
		}





		// only temporary: remove later on, once there are interfaces for everything in the control
		public TimelineGrid Grid
		{
			get { return this.timelineGrid; }
		}

		public TimelineRowList RowList
		{
			get { return this.timelineRowList; }
		}

		public TimelineHeader Header
		{
			get { return this.timelineHeader; }
		}
	}
}
