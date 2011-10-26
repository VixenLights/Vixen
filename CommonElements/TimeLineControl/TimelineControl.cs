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
	public partial class TimelineControl : TimelineControlBase, IEnumerable<Row>
	{
		// Controls
		private Ruler ruler;
		private Grid grid;


		public TimelineControl()
			:base(new TimeInfo())	// This is THE TimeInfo object for the whole control (and all sub-controls).
		{
			TimeInfo.TimePerPixel = TimeSpan.FromTicks(100000);
			TimeInfo.VisibleTimeStart = TimeSpan.Zero;

			InitializeComponent();
			InitializePanel2();

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);

			// Splitter
			splitContainer.Panel1MinSize = 100;
			splitContainer.Panel2MinSize = 100;
			splitContainer.FixedPanel = FixedPanel.Panel1;

			grid.Scroll += GridScrolledHandler;
			grid.VerticalOffsetChanged += GridScrollVerticalHandler;
			Row.RowToggled += RowToggledHandler;
			Row.RowHeightChanged += RowHeightChangedHandler;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			timelineRowList.Top = grid.Top;
			base.OnLayout(e);
		}



		private void InitializePanel2()
		{
			// Add all timeline-like controls to panel2
			splitContainer.BeginInit();
			splitContainer.Panel2.SuspendLayout();

			// Grid
			grid = new Grid(TimeInfo)
			{
				Dock = DockStyle.Fill,
			};
			splitContainer.Panel2.Controls.Add(grid);	// gets added first - to fill the remains

			// Ruler
			ruler = new Ruler(TimeInfo)
			{
				Dock = DockStyle.Top,
				Height = 40,
				
			};
			splitContainer.Panel2.Controls.Add(ruler);
			ruler.ClickedAtTime += RulerClickedHandler;


			splitContainer.Panel2.ResumeLayout(false);
			splitContainer.Panel2.PerformLayout();
			splitContainer.EndInit();
		}



		#region Properties

		public int VerticalOffset
		{
			get
			{
				if (grid != null)
					return grid.VerticalOffset;
				else
					return 0;
			}
			set
			{
				if (grid != null)
					grid.VerticalOffset = value;
			}
		}

		public TimeSpan CursorPosition
		{
			get { return grid.CursorPosition; }
			set { grid.CursorPosition = value; }
		}

		public override TimeSpan VisibleTimeSpan
		{
			get { return grid.VisibleTimeSpan; }
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
				TimePerPixel = TimeSpan.FromTicks(TotalTime.Ticks / grid.Width);
				VisibleTimeStart = TimeSpan.Zero;
			} else {
				TimePerPixel = TimePerPixel.Scale(scale);
				if (VisibleTimeEnd > TotalTime)
					VisibleTimeStart = TotalTime - VisibleTimeSpan;
			}
		}

		private void AddRowToControls(Row row, RowLabel label)
		{
			grid.AddRow(row);
			timelineRowList.AddRowLabel(label);
		}

		private void RemoveRowFromControls(Row row)
		{
			grid.RemoveRow(row);
			timelineRowList.RemoveRowLabel(row.RowLabel);
		}

		// adds a given row to the control, optionally as a child of the given parent
		public void AddRow(Row row, Row parent = null)
		{
			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);
		}

		// adds a new, empty row with a default label with the given name, as a child of the (optional) given parent
		public Row AddRow(string name, Row parent = null, int height = 50)
		{
			Row row = new Row();

			row.Name = name;
			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}

		// adds a new, empty row with the given label, as a child of the (optional) given parent
		public Row AddRow(RowLabel label, Row parent = null, int height = 50)
		{
			Row row = new Row(label);

			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}


		public bool AddSnapTime(TimeSpan time, int level, Color color)
		{
			return grid.AddSnapPoint(time, level, color);
		}

		public bool RemoveSnapTime(TimeSpan time)
		{
			return grid.RemoveSnapPoint(time);
		}

		public void ClearAllSnapTimes()
		{
			grid.ClearSnapPoints();
		}


		public void AlignSelectedElementsLeft()
		{
			grid.AlignSelectedElementsLeft();
		}

		/// <summary>
		/// Clears all elements from the grid, leaving the rows intact.
		/// </summary>
		public void ClearAllElements()
		{
			foreach (Row row in grid) {
				row.ClearRowElements();
			}
		}

		/// <summary>
		/// Clears all rows from the grid; effectively emptying the grid. Will also
		/// clear all elements in the grid as well.
		/// </summary>
		public void ClearAllRows()
		{
			ClearAllElements();
			foreach (Row row in grid.ToArray()) {
				RemoveRowFromControls(row);
			}
		}



		public IEnumerator<Row> GetEnumerator()
		{
			return grid.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return grid.GetEnumerator();
		}

		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementDoubleClicked
		{
			add { grid.ElementDoubleClicked += value; }
			remove { grid.ElementDoubleClicked -= value; }
		}

		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving
		{
			add { grid.ElementsFinishedMoving += value; }
			remove { grid.ElementsFinishedMoving -= value; }
		}

		public event EventHandler<TimeSpanEventArgs> CursorMoved
		{
			add { grid.CursorMoved += value; }
			remove { grid.CursorMoved -= value; }
		}

		public event EventHandler VerticalOffsetChanged
		{
			add { grid.VerticalOffsetChanged += value; }
			remove { grid.VerticalOffsetChanged -= value; }
		}

		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows
		{
			add { grid.ElementChangedRows += value; }
			remove { grid.ElementChangedRows -= value; }
		}

		public event EventHandler<TimelineDropEventArgs> DataDropped
		{
			add { grid.DataDropped += value; }
			remove { grid.DataDropped -= value; }
		}


		#endregion


		#region Event Handlers

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
				timelineRowList.VerticalOffset = grid.VerticalOffset;

			// I know it's bad to do this, but when we scroll we can get very nasty artifacts
			// and it looks shit in general. So, force an immediate graphical refresh
			Refresh();
		}

		private void GridScrollHorizontalHandler(object sender, EventArgs e)
		{

		}

		private void GridScrolledHandler(object sender, ScrollEventArgs e)
		{
		    if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				//GridScrollHorizontalHandler(sender, e);
		    } else {
				GridScrollVerticalHandler(sender, e);
			}
		}

		private void RulerClickedHandler(object sender, TimeSpanEventArgs e)
		{
			Debug.WriteLine("Header clicked at {0}", e.Time);
		}

		private void RowToggledHandler(object sender, EventArgs e)
		{
			if (timelineRowList != null)
				timelineRowList.VerticalOffset = grid.VerticalOffset;
			Invalidate();
		}

		private void RowHeightChangedHandler(object sender, EventArgs e)
		{
			// again, icky. But it prevents artifacts.
			Refresh();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (Form.ModifierKeys.HasFlag(Keys.Control)) {
				// holding the control key zooms the horizontal axis, by 10% per mouse wheel tick
				Zoom(1.0 - ((double)e.Delta / 1200.0));
			} else if (Form.ModifierKeys.HasFlag(Keys.Shift)) {
				// holding the skift key moves the horizontal axis, by 10% of the visible time span per mouse wheel tick
				// wheel towards user   --> negative delta --> VisibleTimeStart increases
				// wheel away from user --> positive delta --> VisibleTimeStart decreases
				VisibleTimeStart += VisibleTimeSpan.Scale(-((double)e.Delta / 1200.0));
			} else {
				// moving the mouse wheel with no modifiers moves the display vertically, 40 pixels per mouse wheel tick
				VerticalOffset += -(e.Delta / 3);
			}
		}


		#endregion
	}
}
