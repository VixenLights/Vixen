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
		private int lastSplitterXPos;

		public TimelineControl()
		{
			InitializeComponent();

			this.DoubleBuffered = true;

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);
			VisibleTimeStart = TimeSpan.FromSeconds(0);

			splitContainer.Panel1MinSize = 100;
			splitContainer.Panel2MinSize = 100;
			lastSplitterXPos = splitContainer.SplitterDistance;

			Rows = new TimelineRowCollection();
			Rows.RowAdded += new EventHandler<RowAddedOrRemovedEventArgs>(TimelineRowAdded);
			Rows.RowRemoved += new EventHandler<RowAddedOrRemovedEventArgs>(TimelineRowRemoved);
			timelineGrid.Rows = Rows;
			timelineRowList.Rows = Rows;

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
				timelineHeader.VisibleTimeSpan = value;
				timelineGrid.Invalidate();
				timelineHeader.Invalidate();
			}
		}

		public TimeSpan VisibleTimeStart
		{
			get { return timelineGrid.VisibleTimeStart; }
			set
			{
				timelineGrid.VisibleTimeStart = value;
                timelineHeader.VisibleTimeStart = value;
				timelineGrid.Invalidate();
				timelineHeader.Invalidate();
			}
		}

		public TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
			set
			{
				VisibleTimeStart = value - VisibleTimeSpan;
			}
		}

		public TimelineRowCollection Rows { get; set; }


		#endregion

		#region Methods

		// Zoom in or out (ie. change the visible time span): give a scale < 1.0
		// and it zooms in, > 1.0 and it zooms out.
		public void Zoom(double scale)
		{
			if (scale <= 0.0)
				return;

			VisibleTimeSpan = TimeSpan.FromTicks((long)(VisibleTimeSpan.Ticks * scale));
			timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
		}

		#endregion

		#region Event Handlers

		private void OnGridScrolled(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
				timelineHeader.Invalidate();
			} else {
				timelineRowList.topOffset = timelineGrid.VerticalOffset;
				timelineRowList.Invalidate();
			}
		}

		private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			// figure out the splitter movement, and the relative change in size of the grid (new panel size / old panel size)
			double dx = e.SplitX - lastSplitterXPos;
			double gridScale = (double)splitContainer.Panel2.Width / (splitContainer.Panel2.Width + dx);

			// update the grid visible time, and record the last splitter position
			VisibleTimeSpan = TimeSpan.FromTicks((long)(VisibleTimeSpan.Ticks * gridScale));
			lastSplitterXPos = e.SplitX;

			// the row list will need to be redrawn, as it has changed width
			timelineRowList.Invalidate();
		}

		void TimelineRowAdded(object sender, RowAddedOrRemovedEventArgs e)
		{
			e.Row.ParentControl = timelineGrid;
		}

		void TimelineRowRemoved(object sender, RowAddedOrRemovedEventArgs e)
		{
			e.Row.ParentControl = null;
		}


		#endregion




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

		private void timelineGrid_Load(object sender, EventArgs e)
		{

		}

	}
}
