using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
			set {timelineGrid.VisibleTimeSpan = timelineHeader.VisibleTimeSpan = value; }
		}

		public TimeSpan VisibleTimeStart
		{
			get { return timelineGrid.VisibleTimeStart; }
			set { timelineGrid.VisibleTimeStart = timelineHeader.VisibleTimeStart = value; }
		}

		public TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
			set { VisibleTimeStart = value - VisibleTimeSpan; }
		}

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

		private void AddRowToControls(TimelineRow row, TimelineRowLabel label)
		{
			timelineGrid.Rows.Add(row);
			timelineRowList.Controls.Add(label);
		}

		// adds a given row to the control, optionally as a child of the given parent
		public void AddRow(TimelineRow row, TimelineRow parent = null)
		{
			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);
		}

		// adds a new, empty row with a default label with the given name, as a child of the (optional) given parent
		public TimelineRow AddRow(string name, TimelineRow parent = null, int height = 50)
		{
			TimelineRow row = new TimelineRow();

			row.Name = name;
			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}

		// adds a new, empty row with the given label, as a child of the (optional) given parent
		public TimelineRow AddRow(TimelineRowLabel label, TimelineRow parent = null, int height = 50)
		{
			TimelineRow row = new TimelineRow(label);

			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}


		public bool AddSnapTime(TimeSpan time, int level)
		{
			if (timelineGrid.SnapPoints.ContainsKey(time))
				return false;

			timelineGrid.SnapPoints[time] = level;
			return true;
		}

		public bool RemoveSnapTime(TimeSpan time)
		{
			return timelineGrid.SnapPoints.Remove(time);
		}



		#endregion

		#region Event Handlers

		private void OnGridScrolled(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
			} else {
				timelineRowList.TopOffset = timelineGrid.VerticalOffset;
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
			timelineRowList.Refresh();
		}

		#endregion
	}
}
