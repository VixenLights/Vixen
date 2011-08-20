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


			//timelineGrid.ParentControl = this;
			//timelineRowList.ParentControl = this;
			//timelineHeader.ParentControl = this;
			//this.Controls.Add(timelineGrid);
			//this.Controls.Add(timelineRowList);
			//this.Controls.Add(timelineHeader);
			//timelineGrid.Parent = this;
			//timelineRowList.Parent = this;
			//timelineHeader.Parent = this;

			DoubleBuffered = true;

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);
			VisibleTimeStart = TimeSpan.FromSeconds(0);
			VisibleTimeSpan = TimeSpan.FromSeconds(10);
			MajorGridlineColor = Color.FromArgb(180, 180, 180);
			RowSeparatorColor = Color.Black;
			MajorGridlineInterval = TimeSpan.FromSeconds(1);

			splitContainer.Panel1MinSize = 100;
			splitContainer.Panel2MinSize = 100;
			lastSplitterXPos = splitContainer.SplitterDistance;

			//Rows = new TimelineRowCollection();
			//Rows.RowAdded += new EventHandler<RowAddedOrRemovedEventArgs>(TimelineRowAdded);
			//Rows.RowRemoved += new EventHandler<RowAddedOrRemovedEventArgs>(TimelineRowRemoved);
			//timelineGrid.Rows = Rows;
			//timelineRowList.Rows = Rows;

			timelineGrid.Scroll += new ScrollEventHandler(OnGridScrolled);
		}



		#region Properties

		public TimeSpan TotalTime
		{
			get { return timelineGrid.TotalTime; }
			set { timelineGrid.TotalTime = value; }
		}

		public TimeSpan VisibleTimeSpan
		{
			get { return timelineGrid.VisibleTimeSpan; }
			set
			{
				timelineGrid.VisibleTimeSpan = timelineHeader.VisibleTimeSpan = value;
				timelineGrid.Refresh();
				timelineHeader.Refresh();
			}
		}

		public TimeSpan VisibleTimeStart
		{
			get { return timelineGrid.VisibleTimeStart; }
			set { timelineGrid.VisibleTimeStart = timelineHeader.VisibleTimeStart = value; }
			//timelineGrid.Invalidate();
			//timelineHeader.Invalidate();
		}

		public TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
			set	{ VisibleTimeStart = value - VisibleTimeSpan; }
		}

		public TimeSpan MajorGridlineInterval
		{
			get { return timelineGrid.MajorGridlineInterval; }
			set { timelineGrid.MajorGridlineInterval = timelineHeader.MajorTickInterval = value; }
		}

		public Color MajorGridlineColor
		{
			get { return timelineGrid.MajorGridlineColor; }
			set { timelineGrid.MajorGridlineColor = value; }
		}

		public Color RowSeparatorColor
		{
			get { return timelineGrid.RowSeparatorColor; }
			set { timelineGrid.RowSeparatorColor = value; }
		}

		public Color SnapPointColor
		{
			get { return timelineGrid.SnapPointColor; }
			set { timelineGrid.SnapPointColor = value; }
		}

		//public List<TimelineRow> Rows
		//{
		//    get
		//    {
		//        List<TimelineRow> rows = new List<TimelineRow>();
		//        foreach (TimelineRow r in timelineGrid.Controls) {
		//            rows.Add(r);
		//        }
		//        return rows;
		//    }
		//}



		#endregion

		#region Methods

		public void AddRow(string name, int height = 100)
		{
			TimelineRow row = new TimelineRow();
			row.Name = name;
			row.Height = height;
			timelineGrid.Controls.Add(row);
		}

		// Zoom in or out (ie. change the visible time span): give a scale < 1.0
		// and it zooms in, > 1.0 and it zooms out.
		public void Zoom(double scale)
		{
			if (scale <= 0.0)
				return;

			VisibleTimeSpan = TimeSpan.FromTicks((long)(VisibleTimeSpan.Ticks * scale));
		}

		#endregion

		#region Event Handlers

		private void OnGridScrolled(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
			} else {
				timelineRowList.TopOffset = timelineGrid.VerticalOffset;
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

		//void TimelineRowAdded(object sender, RowAddedOrRemovedEventArgs e)
		//{
		//    e.Row.ParentControl = timelineGrid;
		//}

		//void TimelineRowRemoved(object sender, RowAddedOrRemovedEventArgs e)
		//{
		//    e.Row.ParentControl = null;
		//}


		#endregion




		//// only temporary: remove later on, once there are interfaces for everything in the control
		//public TimelineGrid Grid
		//{
		//    get { return this.timelineGrid; }
		//}

		//public TimelineRowList RowList
		//{
		//    get { return this.timelineRowList; }
		//}

		//public TimelineHeader Header
		//{
		//    get { return this.timelineHeader; }
		//}


	}
}
