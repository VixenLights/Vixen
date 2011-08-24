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
	public partial class TimelineControl : TimelineControlBase
	{

		public TimelineControl()
		{
			InitializeComponent();

			this.DoubleBuffered = true;

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);

			// Splitter
			splitContainer.Panel1MinSize = 100;
			splitContainer.Panel2MinSize = 100;
			splitContainer.FixedPanel = FixedPanel.Panel1;

			timelineGrid.Scroll += new ScrollEventHandler(OnGridScrolled);
		}


		#region Properties

		public TimeSpan TotalTime
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.TotalTime;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.TotalTime = value;
			}
		}


		public override TimeSpan VisibleTimeStart
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VisibleTimeStart;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null && timelineHeader != null)
					timelineGrid.VisibleTimeStart = timelineHeader.VisibleTimeStart = value;
			}
		}

		public override TimeSpan TimePerPixel
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.TimePerPixel;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.TimePerPixel = value;
				if (timelineHeader != null)
					timelineHeader.TimePerPixel = value;
			}
		}

		public override TimeSpan VisibleTimeSpan
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VisibleTimeSpan;
				else
					return TimeSpan.Zero;
			}
		}


		#endregion

		#region Methods

		// Zoom in or out (ie. change the visible time span): give a scale < 1.0
		// and it zooms in, > 1.0 and it zooms out.
		public void Zoom(double scale)
		{
			if (scale <= 0.0)
				return;

			TimePerPixel = TimePerPixel.Scale(scale);
		}

		private void AddRowToControls(TimelineRow row, TimelineRowLabel label)
		{
			timelineGrid.Rows.Add(row);
			timelineRowList.AddRowLabel(label);
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

		#endregion
	
	}
}
