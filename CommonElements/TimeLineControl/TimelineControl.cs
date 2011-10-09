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

namespace CommonElements.Timeline
{
	public partial class TimelineControl : TimelineControlBase, IEnumerable<TimelineRow>
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

			timelineGrid.Scroll += GridScrolledHandler;
			timelineGrid.VerticalOffsetChanged += GridScrollVerticalHandler;
			timelineGrid.VisibleTimeStartChanged += GridScrollHorizontalHandler;
			timelineHeader.Click += HeaderClickedHandler;
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
				if (timelineGrid != null)
					timelineGrid.VisibleTimeStart = value;
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

		public int VerticalOffset
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VerticalOffset;
				else
					return 0;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.VerticalOffset = value;
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

			if (VisibleTimeSpan.Scale(scale) > TotalTime) {
				TimePerPixel = TimeSpan.FromTicks(TotalTime.Ticks / timelineGrid.Width);
			} else {
				TimePerPixel = TimePerPixel.Scale(scale);
				if (VisibleTimeEnd > TotalTime)
					VisibleTimeEnd = TotalTime;
			}
		}

		private void AddRowToControls(TimelineRow row, TimelineRowLabel label)
		{
			timelineGrid.Rows.Add(row);
			timelineRowList.AddRowLabel(label);
		}

		private void RemoveRowFromControls(TimelineRow row)
		{
			timelineGrid.Rows.Remove(row);
			timelineRowList.RemoveRowLabel(row.RowLabel);
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
			return timelineGrid.AddSnapPoint(time, level);
		}

		public bool RemoveSnapTime(TimeSpan time)
		{
			return timelineGrid.RemoveSnapPoint(time);
		}


		public void AlignSelectedElementsLeft()
		{
			timelineGrid.AlignSelectedElementsLeft();
		}

		/// <summary>
		/// Clears all elements from the grid, leaving the rows intact.
		/// </summary>
		public void ClearAllElements()
		{
			foreach (TimelineRow row in timelineGrid) {
				row.ClearAllElements();
			}
		}

		/// <summary>
		/// Clears all rows from the grid; effectively emptying the grid. Will also
		/// clear all elements in the grid as well.
		/// </summary>
		public void ClearAllRows()
		{
			ClearAllElements();
			foreach (TimelineRow row in timelineGrid.ToArray()) {
				RemoveRowFromControls(row);
			}
		}

		public IEnumerator<TimelineRow> GetEnumerator()
		{
			return timelineGrid.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return timelineGrid.GetEnumerator();
		}

		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementDoubleClicked
		{
			add { timelineGrid.ElementDoubleClicked += value; }
			remove { timelineGrid.ElementDoubleClicked -= value; }
		}

		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving
		{
			add { timelineGrid.ElementsFinishedMoving += value; }
			remove { timelineGrid.ElementsFinishedMoving -= value; }
		}

		public event EventHandler<TimeSpanEventArgs> CursorMoved
		{
			add { timelineGrid.CursorMoved += value; }
			remove { timelineGrid.CursorMoved -= value; }
		}

		public event EventHandler VisibleTimeStartChanged
		{
			add { timelineGrid.VisibleTimeStartChanged += value; }
			remove { timelineGrid.VisibleTimeStartChanged -= value; }
		}

		public event EventHandler VerticalOffsetChanged
		{
			add { timelineGrid.VerticalOffsetChanged += value; }
			remove { timelineGrid.VerticalOffsetChanged -= value; }
		}

		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows
		{
			add { timelineGrid.ElementChangedRows += value; }
			remove { timelineGrid.ElementChangedRows -= value; }
		}

		public event EventHandler<TimelineDropEventArgs> DataDropped
		{
			add { timelineGrid.DataDropped += value; }
			remove { timelineGrid.DataDropped -= value; }
		}


		#endregion


		#region Event Handlers

        // TODO: we need to add proper drag-n-drop support onto the control, from other controls.
        // eg. like having a toolbar of elements that can be dragged and dropped onto the control
        // to add a new element. I think there's .NET support for drag-and-drop stuff, no idea how
        // to use it though.
        // I would imagine that to do the drag/drop, we would temporarily add an element to whatever
        // row has been dragged over, and then move it around as needed. If they drag off, we delete
        // it. Or something like that?

        // TODO: we need support for key presses on the control. A few I can think of:
        //       - delete key, to delete selected elements
        //       - arrow keys, to move the viewport around (maybe ~20 pixels at a time or something?)
        //       - maybe CTRL-arrow keys, to do large scrolling?

        // TODO: oh, we need cut-copy-paste support, too. Should that be done in the control, or
        // should the control just raise events for the keystrokes that the client can handle? I'm
        // thinking the latter.


		private void GridScrollVerticalHandler(object sender, EventArgs e)
		{
			if (timelineRowList != null)
				timelineRowList.VerticalOffset = timelineGrid.VerticalOffset;
		}

		private void GridScrollHorizontalHandler(object sender, EventArgs e)
		{
			if (timelineHeader != null)
				timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
		}

		private void GridScrolledHandler(object sender, ScrollEventArgs e)
		{
		    if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				GridScrollHorizontalHandler(sender, e);
		    } else {
				GridScrollVerticalHandler(sender, e);
			}
		}

		private void HeaderClickedHandler(object sender, TimeSpanEventArgs e)
		{
			Debug.WriteLine("Header clicked at {0}", e.Time);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			timelineRowList.VerticalOffset = timelineGrid.VerticalOffset;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (Form.ModifierKeys.HasFlag(Keys.Control)) {
				// holding the control key zooms the horizontal axis, by 10% per mouse wheel tick
				// TODO: should we zoom the vertial rows as well?
				Zoom(1.0 - ((double)e.Delta / 1200.0));
			} else if (Form.ModifierKeys.HasFlag(Keys.Shift)) {
				// holding the skift key moves the horizontal axis, by 10% of the visible time span per mouse wheel tick
				VisibleTimeStart += VisibleTimeSpan.Scale(-((double)e.Delta / 1200.0));
			} else {
				// moving the mouse wheel with no modifiers moves the display vertically, 30 pixels per mouse wheel tick
				VerticalOffset += -(e.Delta / 4);
			}
		}



		#endregion

	}
}
