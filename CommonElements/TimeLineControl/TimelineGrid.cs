using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Imaging;


namespace CommonElements.Timeline
{
	public class TimelineGrid : TimelineControlBase, IEnumerable<TimelineRow>
	{
		#region Members

		private List<TimelineRow> m_rows;						// the rows in the grid
		private DragState m_dragState = DragState.Normal;		// the current dragging state
		private Point m_lastMouseLocation;						// the location of the mouse at last draw; used to update the dragging.
																// Relative to the control, not the grid canvas.
		private Point m_selectionRectangleStart;				// the location (on the grid canvas) where the selection box starts.
		private Rectangle m_ignoreDragArea;						// the area in which move movements should be ignored, before we start dragging
		private TimelineElement m_mouseDownElement = null;		// the element under the cursor on a mouse click
		private TimelineRow m_mouseDownElementRow = null;		// the row that the clicked m_mouseDownElement belongs to (a single element may be in multiple rows)
		private TimeSpan m_totalTime;							// the total amount of time this grid represents
		private TimeSpan m_cursorPosition;						// the current grid 'cursor' position (line drawn vertically)
		private Size m_dragAutoscrollDistance;					// how far in either dimension the mouse has moved outside a bounding area,
																// so we should scroll the viewable pane that direction
		#endregion


		#region Initialization

		public TimelineGrid()
		{
			this.AutoScroll = true;
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			TotalTime = TimeSpan.FromMinutes(1);
			RowSeparatorColor = Color.Black;
			MajorGridlineColor = Color.FromArgb(120, 120, 120);
			GridlineInterval = TimeSpan.FromSeconds(1.0);
			BackColor = Color.FromArgb(140, 140, 140);
			SelectionColor = Color.FromArgb(100, 40, 100, 160);
			SelectionBorder = Color.Blue;
			CursorColor = Color.FromArgb(150, 50, 50, 50);
			CursorWidth = 2.5F;
			CursorPosition = TimeSpan.Zero;
			OnlySnapToCurrentRow = true;
			DragThreshold = 8;
			m_dragAutoscrollDistance.Height = m_dragAutoscrollDistance.Width = 0;
			StaticSnapPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>();
			SnapPriorityForElements = 5;

			m_rows = new List<TimelineRow>();
			ScrollTimer = new Timer();
			ScrollTimer.Interval = 20;
			ScrollTimer.Enabled = false;
			ScrollTimer.Tick += ScrollTimerHandler;

			// thse changed events are static for the class. If we make them per element or row
			//  later, we will need to attach/detach from each event manually.
			TimelineRow.RowChanged += RowChangedHandler;
			TimelineRow.RowSelectedChanged += RowSelectedChangedHandler;

			// Drag-drop 9/20/2011
			AllowDrop = true;
			this.DragEnter += TimelineGrid_DragEnter;
			this.DragDrop += TimelineGrid_DragDrop;
		}


		#endregion


		#region Properties

		/// <summary>
		/// The maximum amount of time represented by this Grid.
		/// </summary>
		public TimeSpan TotalTime
		{
			get { return m_totalTime; }
			set { m_totalTime = value; Invalidate(); }
		}

		/// <summary>
		/// The time at the left of the control (the visible beginning).
		/// </summary>
		public override TimeSpan VisibleTimeStart
		{
			get { return base.VisibleTimeStart; }
			set
			{
				if (value < TimeSpan.Zero)
					value = TimeSpan.Zero;

				if (value > TotalTime - VisibleTimeSpan)
					value = TotalTime - VisibleTimeSpan;

				base.VisibleTimeStart = value;
				AutoScrollPosition = new Point((int)timeToPixels(value), -AutoScrollPosition.Y);
				_VisibleTimeStartChanged();
			}
		}

		// this needs to be overridden to maintain the visible time start accurately
		// (as it isn't stored as a value, but inferred through the AutoScroll position).
		public override TimeSpan TimePerPixel
		{
			get { return base.TimePerPixel; }
			set
			{
				TimeSpan start = VisibleTimeStart;
				base.TimePerPixel = value;
				VisibleTimeStart = start;
				RecalculateAllStaticSnapPoints();
			}
		}


		public int VerticalOffset
		{
			get { return -AutoScrollPosition.Y; }
			set
			{
				if (value < 0)
					value = 0;

				if (value > AutoScrollMinSize.Height - ClientSize.Height)
					value = AutoScrollMinSize.Height - ClientSize.Height;

				if (-AutoScrollPosition.Y == value)
					return;

				AutoScrollPosition = new Point(-AutoScrollPosition.X, value);
				_VerticalOffsetChanged();
			}
		}

		public List<TimelineRow> Rows
		{
			get { return m_rows; }
			set { m_rows = value; }
		}

		public IEnumerable<TimelineElement> SelectedElements
		{
			get
			{
				return Rows.SelectMany(x => x.SelectedElements).Distinct();
			}
		}

		public TimeSpan CursorPosition
		{
			get { return m_cursorPosition; }
			set { m_cursorPosition = value; _CursorMoved(value); Invalidate(); }
		}

		public TimeSpan GridlineInterval { get; set; }
		public bool OnlySnapToCurrentRow { get; set; }
		public int SnapPriorityForElements { get; set; }
		public int DragThreshold { get; set; }
		public Rectangle SelectedArea { get; set; }
	
		// drawing colours, information, etc.
		public Color RowSeparatorColor { get; set; }
		public Color MajorGridlineColor { get; set; }
		public Color SelectionColor { get; set; }
		public Color SelectionBorder { get; set; }
		public Color CursorColor { get; set; }
		public Single CursorWidth { get; set; }

		// private properties
		private bool CtrlPressed { get { return Form.ModifierKeys.HasFlag(Keys.Control); } }
		private Timer ScrollTimer { get; set; }
		private int CurrentRowIndexUnderMouse { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> StaticSnapPoints { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> CurrentDragSnapPoints { get; set; }
		private TimeSpan DragTimeLeftOver { get; set; }
		private bool ResizingElement { get; set; }
		private bool ResizingFront { get; set; }

		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementDoubleClicked;
		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving;
		public event EventHandler<TimeSpanEventArgs> CursorMoved;
		public event EventHandler VisibleTimeStartChanged;
		public event EventHandler VerticalOffsetChanged;
		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows;

		private void _ElementDoubleClicked(TimelineElement te) { if (ElementDoubleClicked != null) ElementDoubleClicked(this, new ElementEventArgs(te)); }
		private void _ElementsFinishedMoving(MultiElementEventArgs args) { if (ElementsFinishedMoving != null) ElementsFinishedMoving(this, args); }
		private void _CursorMoved(TimeSpan t) { if (CursorMoved != null) CursorMoved(this, new TimeSpanEventArgs(t)); }
		private void _VisibleTimeStartChanged() { if (VisibleTimeStartChanged != null) VisibleTimeStartChanged(this, EventArgs.Empty); }
		private void _VerticalOffsetChanged() { if (VerticalOffsetChanged != null) VerticalOffsetChanged(this, EventArgs.Empty); }
		private void _ElementChangedRows(TimelineElement element, TimelineRow oldRow, TimelineRow newRow) { if (ElementChangedRows != null) ElementChangedRows(this, new ElementRowChangeEventArgs(element, oldRow, newRow)); }

		#endregion


		#region Event Handlers - non-mouse events

		protected void RowChangedHandler(object sender, EventArgs e)
		{
			// when dragging, the control will invalidate after it's done, in case multiple elements are changing.
			if (m_dragState != DragState.Dragging)
				Invalidate();
		}

		protected void RowSelectedChangedHandler(object sender, ModifierKeysEventArgs e)
		{
			TimelineRow selectedRow = sender as TimelineRow;

			// if CTRL wasn't down, then we want to clear all the other rows
			if (!e.ModifierKeys.HasFlag(Keys.Control)) {
				ClearSelectedRows();
				selectedRow.Selected = true;
			}
		}

		protected void ScrollTimerHandler(object sender, EventArgs e)
		{
			// move by the number of pixels we are outside, scaled down by a constant
			if (m_dragAutoscrollDistance.Width != 0) {
				TimeSpan offset = pixelsToTime(m_dragAutoscrollDistance.Width / 8);

				if (m_dragState == DragState.Dragging) {
					// don't do any dragging if we're hard left in the viewport and trying to drag left, or
					// the same at the other end: otherwise the 'desired time' counter (of time left still
					// to drag) continually gets incremented
					if (!(m_dragAutoscrollDistance.Width < 0 && VisibleTimeStart == TimeSpan.Zero) &&
						!(m_dragAutoscrollDistance.Width > 0 && VisibleTimeEnd == TotalTime)) {
						TimeSpan desiredMoveTime = DragTimeLeftOver + offset;
						TimeSpan realMoveTime = OffsetElementsByTime(SelectedElements, desiredMoveTime);
						DragTimeLeftOver = desiredMoveTime - realMoveTime;
					}
				}

				if (m_dragState == DragState.Selecting) {
					Point gridLocation = translateLocation(m_lastMouseLocation);
					UpdateSelectionRectangle(gridLocation);
				}

				// move the view window. Note: this is done after the element movement to prevent some graphical artifacts.
				VisibleTimeStart += offset;
			}
			if (m_dragAutoscrollDistance.Height != 0) {
				VerticalOffset += m_dragAutoscrollDistance.Height / 8;
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			if (e.KeyChar == (char)27)  // ESC
            {
				endDrag();  // do this regardless of if we're dragging or not.
			}
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				base.VisibleTimeStart = pixelsToTime(-AutoScrollPosition.X);
			}

			// This MUST be done last! Otherwise, event handlers get called with the OLD values.
			base.OnScroll(se);
		}

		#endregion


		#region Event Handlers - mouse events

		/// <summary>
		/// Translates a location (Point) so that its coordinates represent the coordinates on the underlying timeline, taking into account scroll position.
		/// </summary>
		/// <param name="e"></param>
		protected Point translateLocation(Point originalLocation)
		{
			// Translate this location based on the auto scroll position.
			Point p = originalLocation;
			p.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
			return p;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			Point gridLocation = translateLocation(e.Location);
			m_mouseDownElement = elementAt(gridLocation);
			m_mouseDownElementRow = rowAt(gridLocation);

			if (e.Button == MouseButtons.Left) {
				// we clicked on the background - clear anything that is selected, and begin a
				// 'selection' drag.
				if (m_mouseDownElement == null) {
					if (!CtrlPressed) {
						ClearSelectedElements();
						ClearSelectedRows();
						m_dragState = DragState.Selecting;
						SelectedArea = new Rectangle(gridLocation.X, gridLocation.Y, 0, 0);
						m_lastMouseLocation = e.Location;
						m_selectionRectangleStart = gridLocation;
						m_dragAutoscrollDistance.Height = m_dragAutoscrollDistance.Width = 0;
					}
				} else {
					// our mouse is down on something
					if (m_mouseDownElement.Selected) {
						// unselect
						if (CtrlPressed)
							m_mouseDownElement.Selected = false;
					} else {
						// select
						if (!CtrlPressed) {
							ClearSelectedElements();
							ClearSelectedRows();
						}
						m_mouseDownElement.Selected = true;
					}

					if (ResizingElement) {
						m_lastMouseLocation = e.Location;
						beginDrag(gridLocation);
					} else {
						dragWait(e.Location);
					}
				}
				Invalidate();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			Point gridLocation = translateLocation(e.Location);

			if (e.Button == MouseButtons.Left) {
				if (m_dragState == DragState.Dragging) {
					MultiElementEventArgs evargs = new MultiElementEventArgs { Elements = SelectedElements };
					_ElementsFinishedMoving(evargs);

				} else if (m_dragState == DragState.Selecting) {
					// we will only be Selecting if we clicked on the grid background, so on mouse up, check if
					// we didn't move (or very far): if so, move the cursor to the clicked position.
					if (SelectedArea.Width < 2 && SelectedArea.Height < 2) {
						CursorPosition = pixelsToTime(gridLocation.X);
					}

					SelectedArea = new Rectangle();
				} else {
					// If we're not dragging on mouse up, it could be a click on one of multiple
					// selected elements. (In which case we select only that one)
					if (m_mouseDownElement != null && !CtrlPressed) {
						ClearSelectedElements();
						m_mouseDownElement.Selected = true;
					}
				}

				endDrag();  // we always do this, even if we weren't dragging.

				Invalidate();
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			// don't call the base mouse wheel event; it scrolls the grid left and right
			// if there isn't any vertical movement available (ie. no vertical scrollbar)
			//base.OnMouseWheel(e);
		}

		protected override void OnMouseHWheel(MouseEventArgs args)
		{
			//base.OnMouseHWheel(args);

			Debug.WriteLine("Grid OnMouseHWheel: delta={0}", args.Delta);

			//AutoScrollPosition = new Point(-AutoScrollPosition.X + args.Delta/12, -AutoScrollPosition.Y);

			double scale;
			if (args.Delta > 0)
				scale = 0.10;
			else
				scale = -0.10;
			VisibleTimeStart += VisibleTimeSpan.Scale(scale);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			Point gridLocation = translateLocation(e.Location);

			if (m_dragState == DragState.Normal) {
				// if we're near the edge of an element, change the cursor to a 'resize'
				TimelineElement element = elementAt(gridLocation);

				if (element != null) {
					int maxGrabWidth = Math.Min(12, (int)(timeToPixels(element.Duration) / 2));		// cap it to 12 pixels. Officially pulled out my arse.
					if (gridLocation.X >= timeToPixels(element.StartTime) && gridLocation.X < timeToPixels(element.StartTime) + maxGrabWidth) {
						ResizingElement = true;
						ResizingFront = true;
					} else if (gridLocation.X <= timeToPixels(element.EndTime) && gridLocation.X > timeToPixels(element.EndTime) - maxGrabWidth) {
						ResizingElement = true;
						ResizingFront = false;
					} else {
						ResizingElement = false;
					}
				} else {
					ResizingElement = false;
				}

				if (ResizingElement)
					Cursor = Cursors.SizeWE;
				else
					Cursor = Cursors.Default;

				return;
			}

			if (m_dragState == DragState.Waiting) {
				if (!m_ignoreDragArea.Contains(e.Location)) {
					//begin the dragging process
					beginDrag(gridLocation);
				}
			}
			if (m_dragState == DragState.Dragging) {

				// if we don't have anything selected, there's no point dragging anything...
				if (SelectedElements.Count() == 0)
					return;

				Point d = new Point(e.X - m_lastMouseLocation.X, e.Y - m_lastMouseLocation.Y);
				m_lastMouseLocation = e.Location;

				// only stuff around figuring out the best way to do dragging if we're really dragging; if we're
				// resizing, then do a simple calculation
				if (!ResizingElement) {
					// calculate the points at which we should start dragging; ie. account for any selected elements.
					// (we subtract VisibleTimeStart to make it relative to the control, instead of the grid canvas.)
					TimeSpan earliestTime = GetEarliestTimeForElements(SelectedElements);
					TimeSpan latestTime = GetLatestTimeForElements(SelectedElements);
					int leftBoundary = (int)timeToPixels(earliestTime - VisibleTimeStart + DragTimeLeftOver);
					int rightBoundary = (int)timeToPixels(latestTime - VisibleTimeStart + DragTimeLeftOver);

					// if the mouse moved left, only add it to the scroll size if:
					// 1) the elements are hard left (or more) in the viewport
					// 2) the elements are hard right, and we are moving right. This provides deceleration.
					//    Cap this value to 0.
					if (d.X < 0) {
						if (leftBoundary <= 0)
							m_dragAutoscrollDistance.Width += d.X;
						else if (rightBoundary >= ClientSize.Width && m_dragAutoscrollDistance.Width > 0)
							m_dragAutoscrollDistance.Width = Math.Max(0, m_dragAutoscrollDistance.Width + d.X);
					}

					// if the mouse moved right, do the inverse of the above rules.
					if (d.X > 0) {
						if (rightBoundary >= ClientSize.Width)
							m_dragAutoscrollDistance.Width += d.X;
						else if (leftBoundary <= 0 && m_dragAutoscrollDistance.Width < 0)
							m_dragAutoscrollDistance.Width = Math.Min(0, m_dragAutoscrollDistance.Width + d.X);
					}

					// if the left and right boundaries are within the viewport, then stop all
					// horizontal scrolling. This can happen if the user scrolls, and mouse-wheels
					// (to zoom out). the control is stuck scrolling, and can't be stopped.
					if (leftBoundary > 0 && rightBoundary < ClientSize.Width)
						m_dragAutoscrollDistance.Width = 0;
				} else {
					m_dragAutoscrollDistance.Width = (e.X < 0) ? e.X : ((e.X > ClientSize.Width) ? e.X - ClientSize.Width : 0);
				}

				m_dragAutoscrollDistance.Height = (e.Y < 0) ? e.Y : ((e.Y > ClientSize.Height) ? e.Y - ClientSize.Height : 0);

				// if we're scrolling, start the timer if needed. If not, vice-versa.
				if (m_dragAutoscrollDistance.Width != 0 || m_dragAutoscrollDistance.Height != 0) {
					if (!ScrollTimer.Enabled)
						ScrollTimer.Start();
				} else {
					if (ScrollTimer.Enabled)
						ScrollTimer.Stop();
				}
                
				// only move the elements here if we aren't going to be auto-dragging while scrolling in the timer events.
				if (d.X != 0 && m_dragAutoscrollDistance.Width == 0) {
					TimeSpan desiredMoveTime = DragTimeLeftOver + pixelsToTime(d.X);
					TimeSpan realMoveTime = OffsetElementsByTime(SelectedElements, desiredMoveTime);
					DragTimeLeftOver = desiredMoveTime - realMoveTime;
				}

				// if we've moved vertically, we may need to move elements between rows
				if (d.Y != 0 && !ResizingElement) {
					MoveElementsVerticallyToLocation(SelectedElements, gridLocation);
				}
			}
			if (m_dragState == DragState.Selecting) {
				Point d = new Point(e.X - m_lastMouseLocation.X, e.Y - m_lastMouseLocation.Y);
				m_lastMouseLocation = e.Location;

				m_dragAutoscrollDistance.Width = (e.X < 0) ? e.X : ((e.X > ClientSize.Width) ? e.X - ClientSize.Width : 0);
				m_dragAutoscrollDistance.Height = (e.Y < 0) ? e.Y : ((e.Y > ClientSize.Height) ? e.Y - ClientSize.Height : 0);

				// if we're scrolling, start the timer if needed. If not, vice-versa.
				if (m_dragAutoscrollDistance.Width != 0 || m_dragAutoscrollDistance.Height != 0) {
					if (!ScrollTimer.Enabled)
						ScrollTimer.Start();
				} else {
					if (ScrollTimer.Enabled)
						ScrollTimer.Stop();
				}

				UpdateSelectionRectangle(gridLocation);
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			Point gridLocation = translateLocation(e.Location);
			TimelineElement elem = elementAt(gridLocation);

			if (elem != null) {
				_ElementDoubleClicked(elem);
			}
		}

		#endregion


		#region Methods - Rows, Elements

		public IEnumerator<TimelineRow> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void ClearSelectedElements()
		{
			foreach (TimelineElement te in SelectedElements.ToArray())
				te.Selected = false;
		}

		public void ClearSelectedRows()
		{
			foreach (TimelineRow row in Rows) {
				row.Selected = false;
			}
		}

		/// <summary>
		/// Returns the row located at the current point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Row at given point, or null if none exists.</returns>
		protected TimelineRow rowAt(Point p)
		{
			TimelineRow containingRow = null;
			int curheight = 0;
			foreach (TimelineRow row in Rows) {
				if (!row.Visible)
					continue;

				if (p.Y < curheight + row.Height) {
					containingRow = row;
					break;
				}
				curheight += row.Height;
			}

			return containingRow;
		}

		/// <summary>
		/// Returns the element located at the current point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Element at given point, or null if none exists.</returns>
		protected TimelineElement elementAt(Point p)
		{
			// First figure out which row we are in
			TimelineRow containingRow = rowAt(p);

			if (containingRow == null)
				return null;

			// Now figure out which element we are on
			foreach (TimelineElement elem in containingRow) {
				Single elemX = timeToPixels(elem.StartTime);
				Single elemW = timeToPixels(elem.Duration);
				if (p.X >= elemX && p.X <= elemX + elemW)
					return elem;
			}

			return null;
		}

		protected TimelineRow RowContainingElement(TimelineElement element)
		{
			foreach (TimelineRow row in Rows) {
				if (row.ContainsElement(element))
					return row;
			}
			return null;
		}

		public List<TimelineElement> ElementsAtTime(TimeSpan time)
		{
			List<TimelineElement> result = new List<TimelineElement>();
			foreach (TimelineRow row in Rows) {
				foreach (TimelineElement elem in row) {
					if ((time >= elem.StartTime) && (time < (elem.StartTime + elem.Duration)))
						result.Add(elem);
				}
			}

			return result;
		}

		// TODO: as per Jono's comment below, if we find performance lacking with large data sets,
		// implement lists of selected elements for both rows and the grid (listens for events from
		// child elements, and attaches/removes them to/from the list).

		// TODO: Considering that the selection is determined by a flag in each element,
		// this is (IMHO) as optimized as this can be.  To do much better, the "selected elements"
		// should instead be a list. (Which can be cleared O(1)). However, that would cause each element
		// draw() to do a lookup in the selected elements list.
		//
		// With "selected" flag in element:  draw one: O(1)   draw all: O(n)		select: O(n)
		// With "selected elements" list:    draw one: O(n)   draw all: O(n^2)		select: less than O(n_row)
		// 
		// Thus, until proven otherwise, I say we leave it like this. A cursory look at CPU usage says we
		// are no worse than Windows Explorer (although it would be hard to be much worse.)
		private void selectElementsWithin(Rectangle SelectedArea)
		{
			TimelineRow startRow = rowAt(SelectedArea.Location);
			TimelineRow endRow = rowAt(SelectedArea.BottomRight());

			TimeSpan selStart = pixelsToTime(SelectedArea.Left);
			TimeSpan selEnd = pixelsToTime(SelectedArea.Right);

			// Iterate all elements of only the rows within our selection.
			bool startFound = false, endFound = false;
			foreach (var row in Rows) {
				if (
					!row.Visible ||					// row is invisible
					endFound ||					// we already passed the end row
					(!startFound && (row != startRow))	//we haven't found the first row, and this isn't it
					) {
					foreach (TimelineElement e in row)
						e.Selected = false;
					continue;
				}


				// If we already found the first row, or we haven't, but this is it
				if (startFound || row == startRow) {
					startFound = true;

					// This row is in our selection
					foreach (var elem in row) {
						elem.Selected = (elem.StartTime < selEnd && elem.EndTime > selStart);
					}

					if (row == endRow) {
						endFound = true;
						continue;
					}
				}

			} // end foreach
		}

		private struct RowElementCombo
		{
			public TimelineRow Row;
			public TimelineElement Element;
		}

		public TimelineRow GetHighestRowForElements(IEnumerable<TimelineElement> elements, bool visibleOnly, TimelineRow skipDuplicatesUnlessInRow)
		{
			List<RowElementCombo> results = new List<RowElementCombo>();
			Dictionary<TimelineElement, int> seenElements = new Dictionary<TimelineElement, int>();

			for (int i = 0; i < Rows.Count; i++) {
				if (visibleOnly && !Rows[i].Visible)
					continue;

				foreach (TimelineElement element in elements) {
					if (Rows[i].ContainsElement(element)) {
						if (skipDuplicatesUnlessInRow == null)
							return Rows[i];

						if (Rows[i] == skipDuplicatesUnlessInRow)
							return Rows[i];

						results.Add(new RowElementCombo { Row = Rows[i], Element = element });
						if (seenElements.ContainsKey(element))
							seenElements[element]++;
						else
							seenElements[element] = 0;
					}
				}
			}

			foreach (RowElementCombo result in results) {
				if (seenElements[result.Element] <= 1 || result.Row == skipDuplicatesUnlessInRow)
					return result.Row;
			}

			return null;
		}

		public TimelineRow GetLowestRowForElements(IEnumerable<TimelineElement> elements, bool visibleOnly, TimelineRow skipDuplicatesUnlessInRow)
		{
			List<RowElementCombo> results = new List<RowElementCombo>();
			Dictionary<TimelineElement, int> seenElements = new Dictionary<TimelineElement, int>();

			for (int i = Rows.Count - 1; i >= 0; i--) {
				if (visibleOnly && !Rows[i].Visible)
					continue;

				foreach (TimelineElement element in elements) {
					if (Rows[i].ContainsElement(element)) {
						if (skipDuplicatesUnlessInRow == null)
							return Rows[i];

						if (Rows[i] == skipDuplicatesUnlessInRow)
							return Rows[i];

						results.Add(new RowElementCombo { Row = Rows[i], Element = element });
						if (seenElements.ContainsKey(element))
							seenElements[element]++;
						else
							seenElements[element] = 0;
					}
				}
			}

			foreach (RowElementCombo result in results) {
				if (seenElements[result.Element] <= 1 || result.Row == skipDuplicatesUnlessInRow)
					return result.Row;
			}

			return null;
		}

		public TimeSpan GetEarliestTimeForElements(IEnumerable<TimelineElement> elements)
		{
			TimeSpan result = TimeSpan.MaxValue;
			foreach (TimelineElement e in elements) {
				if (e.StartTime < result)
					result = e.StartTime;
			}

			return result;
		}

		public TimeSpan GetLatestTimeForElements(IEnumerable<TimelineElement> elements)
		{
			TimeSpan result = TimeSpan.MinValue;
			foreach (TimelineElement e in elements) {
				if (e.EndTime > result)
					result = e.EndTime;
			}

			return result;
		}

		public void AlignSelectedElementsLeft()
		{
			// find the earliest time of all elements
			TimeSpan earliest = GetEarliestTimeForElements(SelectedElements);

			// Find the earliest time of each row
			var rowsToStartTimes = new Dictionary<TimelineRow,TimeSpan>();
			foreach (TimelineRow row in Rows) {
				TimeSpan time = GetEarliestTimeForElements(row.SelectedElements);

				if (time != TimeSpan.MaxValue) {
					rowsToStartTimes.Add(row, time);
				}
			}

			// Now adjust all elements in each row, such that the leftmost element (in this row)
			// is at the same start time as the alltogether leftmost element.
			foreach (KeyValuePair<TimelineRow, TimeSpan> kvp in rowsToStartTimes)
			{
				// calculate how much to offset elements of this row
				TimeSpan thisRowAdjust = kvp.Value - earliest;

				if (thisRowAdjust == TimeSpan.Zero)
					continue;

				foreach (TimelineElement elem in kvp.Key.SelectedElements)
					elem.StartTime -= thisRowAdjust;
			}

		}

		public TimeSpan OffsetElementsByTime(IEnumerable<TimelineElement> elements, TimeSpan offset)
		{
			// check to see if the offset that is applied to all elements will take it outside
			// the total time at all. If so, use a smaller offset, or none at all.
			TimeSpan earliest = GetEarliestTimeForElements(elements);
			TimeSpan latest = GetLatestTimeForElements(elements);

			// if we're going backwards, check that the earliest time isn't already at zero, or
			// that the move will try to take it past 0.
			if (offset.Ticks < 0 && earliest < -offset && !ResizingElement) {
				offset = -earliest;
			}

			// same for moving forwards.
			if (offset.Ticks > 0 && TotalTime - latest < offset && !ResizingElement) {
				offset = TotalTime - latest;
			}

			// if the offset was zero, or was set to it, we don't need to do anything else now.
			if (offset == TimeSpan.Zero)
				return offset;

			// grab all the elements we need to check for snapping against things (ie. filter them based on row
			// if we're only snapping to things in the current row.) Also, record the row this element is in
			// as well, since we'll need it later on, and it saves recalculating multiple times
			List<Tuple<TimelineElement, TimelineRow>> elementsToCheckSnapping = new List<Tuple<TimelineElement, TimelineRow>>();
			if (OnlySnapToCurrentRow) {
				TimelineRow targetRow = Rows[CurrentRowIndexUnderMouse];
				foreach (TimelineElement element in elements) {
					if (targetRow.ContainsElement(element))
						elementsToCheckSnapping.Add(new Tuple<TimelineElement, TimelineRow>(element, targetRow));
				}
			} else {
				foreach (TimelineElement element in elements) {
					elementsToCheckSnapping.Add(new Tuple<TimelineElement, TimelineRow>(element, RowContainingElement(element)));
				}
			}

			// now go through all the elements we need against snap points, and check them
			SnapDetails bestSnapPoint = null;
			TimeSpan snappedOffset = offset;
			foreach (Tuple<TimelineElement, TimelineRow> tuple in elementsToCheckSnapping) {
				TimelineElement element = tuple.Item1;
				TimelineRow thisElementsRow = tuple.Item2;
				foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in CurrentDragSnapPoints) {
					foreach (SnapDetails details in kvp.Value) {
						// skip this point if it's not any higher priority than our highest so far
						if (bestSnapPoint != null && details.SnapLevel <= bestSnapPoint.SnapLevel)
							continue;

						// skip this one if it's not for the row that the current element is on
						if (details.SnapRow != null && details.SnapRow != thisElementsRow)
							continue;

						// figure out if the element start time or end times are in the snap range; if not, skip it
						bool startInRange = (element.StartTime + offset > details.SnapStart && element.StartTime + offset < details.SnapEnd);
						bool endInRange = (element.EndTime + offset > details.SnapStart && element.EndTime + offset < details.SnapEnd);

						// if we're resizing, ignore snapping on the end of the element that is not getting resized.
						if (ResizingElement) {
							if (ResizingFront)
								endInRange = false;
							else
								startInRange = false;
						}

						if (!startInRange && !endInRange)
							continue;

						// calculate the best side (start or end) to snap to the snap time
						if (startInRange && endInRange) {
							if (details.SnapTime - element.StartTime + offset < element.EndTime + offset - details.SnapTime)
								snappedOffset = details.SnapTime - element.StartTime;
							else
								snappedOffset = details.SnapTime - element.EndTime;
						} else {
							if (startInRange)
								snappedOffset = details.SnapTime - element.StartTime;
							else
								snappedOffset = details.SnapTime - element.EndTime;
						}

						bestSnapPoint = details;
					}
				}
			}

			// by now, we should know what the most applicable snap point is (or none at all), and we've
			// also figured out along the way what the actual offset should be to snap the elements to. So have at it.
			foreach (TimelineElement e in elements) {
				if (ResizingElement) {
					if (ResizingFront) {
						// don't do any resizing if it will make the element smaller than 6 pixels.
						// (figure scientifically confirmed to be pulled out my arse.)
						if (e.Duration - snappedOffset > pixelsToTime(6)) {
							e.MoveStartTime(snappedOffset);
						}
					} else {
						if (e.Duration + snappedOffset > pixelsToTime(6)) {
							// resize it to fit within the total time if it would exceed it
							if (e.StartTime + e.Duration + snappedOffset > TotalTime)
								snappedOffset = TotalTime - e.StartTime - e.Duration;

							e.Duration += snappedOffset;
						}
					}
				} else {
					e.StartTime += snappedOffset;
				}
			}

			Invalidate();

			// return the amount of time that the elements were *actually* moved, rather than the amount they were intended to
			return snappedOffset;
		}

		public void MoveElementsVerticallyToLocation(IEnumerable<TimelineElement> elements, Point gridLocation)
		{
			TimelineRow destRow = rowAt(gridLocation);

			if (destRow == null)
				return;

			if (Rows.IndexOf(destRow) == CurrentRowIndexUnderMouse)
				return;

			List<TimelineRow> visibleRows = new List<TimelineRow>();

			for (int i = 0; i < Rows.Count; i++) {
				if (Rows[i].Visible)
					visibleRows.Add(Rows[i]);
			}

			int visibleRowsToMove = visibleRows.IndexOf(destRow) - visibleRows.IndexOf(Rows[CurrentRowIndexUnderMouse]);

			// find the highest and lowest visible rows with selected elements in them
			int topElementVisibleRowIndex = visibleRows.IndexOf(GetHighestRowForElements(elements, true, m_mouseDownElementRow));
			int bottomElementVisibleRowIndex = visibleRows.IndexOf(GetLowestRowForElements(elements, true, m_mouseDownElementRow));

			if (visibleRowsToMove < 0 && -visibleRowsToMove > topElementVisibleRowIndex)
				visibleRowsToMove = -topElementVisibleRowIndex;

			if (visibleRowsToMove > 0 && visibleRowsToMove > visibleRows.Count - 1 - bottomElementVisibleRowIndex)
				visibleRowsToMove = visibleRows.Count - 1 - bottomElementVisibleRowIndex;

			if (visibleRowsToMove == 0)
				return;

			Dictionary<TimelineElement, bool> elementsToMove = new Dictionary<TimelineElement, bool>();
			foreach (TimelineElement e in elements) {
				elementsToMove.Add(e, false);
			}

			// take note of which elements should be moved with respect to the current mouseover row, and not just the first
			// row instance for the element that comes along
			HashSet<TimelineElement> ElementsToMoveInMouseRow = new HashSet<TimelineElement>(m_mouseDownElementRow.SelectedElements);

			// record the row that the mouse down element moves to, to update the m_mouseDownElementRow variable later
			TimelineRow newMouseDownRow = m_mouseDownElementRow;

			for (int i = 0; i < visibleRows.Count; i++) {
				List<TimelineElement> elementsMoved = new List<TimelineElement>();

				// go through each element that hasn't been moved yet, and move it if it's in this row.
				foreach (KeyValuePair<TimelineElement, bool> kvp in elementsToMove) {
					TimelineElement element = kvp.Key;
					bool moved = kvp.Value;

					// if the element has already been moved, ignore it
					if (moved)
						continue;

					// if the element should be ignored (ie. it's a duplicate of an item in the mouseDownElement row)
					if (ElementsToMoveInMouseRow.Contains(element) && visibleRows[i] != m_mouseDownElementRow)
						continue;

					// if the current element is in the current visble row, move it to wherever it needs
					// to go, and also flag that it has been moved
					if (visibleRows[i].ContainsElement(element)) {
						visibleRows[i].RemoveElement(element);
						visibleRows[i + visibleRowsToMove].AddElement(element);
						elementsMoved.Add(element);
						_ElementChangedRows(element, visibleRows[i], visibleRows[i + visibleRowsToMove]);

						if (element == m_mouseDownElement)
							newMouseDownRow = visibleRows[i + visibleRowsToMove];
					}
				}

				foreach (TimelineElement e in elementsMoved)
					elementsToMove[e] = true;
			}

			CurrentRowIndexUnderMouse = Rows.IndexOf(visibleRows[(visibleRows.IndexOf(Rows[CurrentRowIndexUnderMouse]) + visibleRowsToMove)]);
			m_mouseDownElementRow = newMouseDownRow;

			Invalidate();
		}

		private void RecalculateAllStaticSnapPoints()
		{
			if (StaticSnapPoints == null)
				return;

			SortedDictionary<TimeSpan, List<SnapDetails>> newPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>();
			foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints) {
				newPoints[kvp.Key] = new List<SnapDetails>();
				foreach (SnapDetails details in kvp.Value) {
					newPoints[kvp.Key].Add(CalculateSnapDetailsForPoint(details.SnapTime, details.SnapLevel));
				}
			}
			StaticSnapPoints = newPoints;
		}

		private SnapDetails CalculateSnapDetailsForPoint(TimeSpan snapTime, int level)
		{
			SnapDetails result = new SnapDetails();
			result.SnapLevel = level;
			result.SnapTime = snapTime;

			// the start time and end times for specified points are 2 pixels
			// per snap level away from the snap time.
			result.SnapStart = snapTime - TimeSpan.FromTicks(TimePerPixel.Ticks * level * 2);
			result.SnapEnd = snapTime + TimeSpan.FromTicks(TimePerPixel.Ticks * level * 2);
			return result;
		}

		public bool AddSnapPoint(TimeSpan snapTime, int level)
		{
			// even though we can have multiple snap details snapping to a given timespan,
			// this part is only for statc snap points, common to all rows. So for now,
			// we don't want to allow them.
			if (StaticSnapPoints.ContainsKey(snapTime))
				return false;

			StaticSnapPoints.Add(snapTime, new List<SnapDetails> { CalculateSnapDetailsForPoint(snapTime, level) } );
			return true;
		}

		public bool RemoveSnapPoint(TimeSpan snapTime)
		{
			return StaticSnapPoints.Remove(snapTime);
		}


		#endregion


		#region Methods - UI

		private void dragWait(Point location)
		{
			// begin the dragging process -- calculate a area outside which a drag starts
			m_dragState = DragState.Waiting;
			m_ignoreDragArea = new Rectangle(location.X - DragThreshold, location.Y - DragThreshold, DragThreshold*2, DragThreshold*2);
			m_lastMouseLocation = location;
		}

		private void beginDrag(Point location)
		{
			m_dragState = DragState.Dragging;
			if (ResizingElement)
				Cursor = Cursors.SizeWE;
			else
				Cursor = Cursors.SizeAll;

			m_dragAutoscrollDistance.Height = m_dragAutoscrollDistance.Width = 0;
			DragTimeLeftOver = TimeSpan.Zero;
			CurrentRowIndexUnderMouse = Rows.IndexOf(rowAt(location));

			// build up a full set of snap points/details, from (a) the static snap points for the grid, and
			// (b) calculated snap points for this move (ie. other elements in the row[s]).
			CurrentDragSnapPoints = new SortedDictionary<TimeSpan,List<SnapDetails>>(StaticSnapPoints);

			// iterate through the rows, calculating snap points for every single element in each row that has any selected elements
			foreach (TimelineRow row in Rows) {
				// This would skip generating snap points for elements on any rows that have nothing selected.
				// However, we still need to do that; as we might be dragging elements vertically into rows that
				// (currently) have nothing selected. So we'll generate for everything, but that's going to be
				// quite overkill. So, the big TODO: here is to regenerate element snap points for only rows with
				// selected elements, but also regenerate them whenever we move vertically.
				//if (row.SelectedElements.Count == 0)
				//    continue;

				// skip any elements in rows that aren't visible.
				if (!row.Visible)
					continue;

				foreach (TimelineElement element in row) {
					// skip it if it's a selected element; we don't want to snap to them, as they'll be moving as well
					if (element.Selected)
						continue;

					// if it's a non-selected element, generate snap points for it; for the start and end times. Also record the
					// row its from in the generated point, so when snapping we can check against only elements from this row.
					SnapDetails details = CalculateSnapDetailsForPoint(element.StartTime, SnapPriorityForElements);
					details.SnapRow = row;

					if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime)) {
						CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
					}
					CurrentDragSnapPoints[details.SnapTime].Add(details);

					details = CalculateSnapDetailsForPoint(element.EndTime, SnapPriorityForElements);
					details.SnapRow = row;

					if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime)) {
						CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
					}
					CurrentDragSnapPoints[details.SnapTime].Add(details);
				}
			}
		}

		private void endDrag()
		{
			m_dragState = DragState.Normal;
			this.Cursor = Cursors.Default;
			if (ScrollTimer.Enabled)
				ScrollTimer.Stop();
		}

		private void UpdateSelectionRectangle(Point gridLocation)
		{
			Point topLeft = new Point(Math.Min(m_selectionRectangleStart.X, gridLocation.X), Math.Min(m_selectionRectangleStart.Y, gridLocation.Y));
			Point bottomRight = new Point(Math.Max(m_selectionRectangleStart.X, gridLocation.X), Math.Max(m_selectionRectangleStart.Y, gridLocation.Y));

			SelectedArea = Util.RectangleFromPoints(topLeft, bottomRight);
			selectElementsWithin(SelectedArea);
			Invalidate();
		}


		#endregion


		#region Drawing

		private int _drawRows(Graphics g)
		{
			// Draw row separators
			int curY = 0;

			using (Pen p = new Pen(RowSeparatorColor))
			using (SolidBrush b = new SolidBrush(SelectionColor))
			{
				foreach (TimelineRow row in Rows)
				{
					if (!row.Visible)
						continue;

					Point selectedTopLeft = new Point((-AutoScrollPosition.X), curY);
					curY += row.Height;
					Point selectedBottomRight = new Point((-AutoScrollPosition.X) + Width, curY);
					Point lineLeft = new Point((-AutoScrollPosition.X), curY);
					Point lineRight = new Point((-AutoScrollPosition.X) + Width, curY);
					
					if (row.Selected)
						g.FillRectangle(b, Util.RectangleFromPoints(selectedTopLeft, selectedBottomRight));
					g.DrawLine(p, lineLeft, lineRight);
				}
			}

			return curY;
		}

		private void _drawGridlines(Graphics g)
		{
			// Draw vertical gridlines
			Single interval = timeToPixels(GridlineInterval);

			// calculate first tick - (it is the first multiple of interval greater than start)
			// believe it or not, this math is correct :-)
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart) % interval) + interval;

			using (Pen p = new Pen(MajorGridlineColor))
			{
				p.DashStyle = DashStyle.Dash;
				for (Single x = start; x < start + Width; x += interval)
				{
					g.DrawLine(p, x, (-AutoScrollPosition.Y), x, (-AutoScrollPosition.Y) + Height);
				}
			}
		}

		private void _drawSnapPoints(Graphics g)
		{
			using (Pen p = new Pen(Color.Blue))
			{
				// iterate through all snap points, and if it's visible, draw it

				foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints)
				{
					SnapDetails details = kvp.Value[0];
					if (kvp.Key >= VisibleTimeStart && kvp.Key < VisibleTimeEnd)
					{
						Single x = timeToPixels(kvp.Key);
						p.DashPattern = new float[] { details.SnapLevel, details.SnapLevel };
						g.DrawLine(p, x, 0, x, AutoScrollMinSize.Height);
					}
				}
			}
		}

		private void _drawElements(Graphics g)
		{
			// Draw each row
			int top = 0;    // y-coord of top of current row
			foreach (TimelineRow row in Rows) {
				if (!row.Visible)
					continue;

				// a list of generated bitmaps, with starttime and endtime for where they are supposed to be drawn.
				List<BitmapDrawDetails> bitmapsToDraw = new List<BitmapDrawDetails>();
				TimeSpan currentlyDrawnTo = TimeSpan.Zero;
				TimeSpan desiredDrawTo = TimeSpan.Zero;
				for (int i = 0; i < row.ElementCount; i++) {
					TimelineElement currentElement = row.GetElementAtIndex(i);
					desiredDrawTo = currentElement.StartTime;

					// if this is the last element, draw everything
					if (i == row.ElementCount - 1) {
						desiredDrawTo = TotalTime;
					}

					Size size = new Size((int)Math.Ceiling(timeToPixels(currentElement.Duration)), row.Height);
					Bitmap elementImage = currentElement.Draw(size);
					bitmapsToDraw.Add(new BitmapDrawDetails() { bmp = elementImage, startTime = currentElement.StartTime, duration = currentElement.Duration });

					while (currentlyDrawnTo < desiredDrawTo) {
						// if there's nothing left to draw, the rest of it is empty; skip to the desired draw point
						if (bitmapsToDraw.Count == 0) {
							currentlyDrawnTo = desiredDrawTo;
							break;
						}

						// find how many bitmaps are going to be in this next segment, and also take note of the earliest time they finish
						TimeSpan segmentDuration = TimeSpan.MaxValue;
						TimeSpan earliestStart = TimeSpan.MaxValue;
						int bitmapLayers = 0;
						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw) {
							TimeSpan start = drawDetails.startTime;
							TimeSpan duration = drawDetails.duration;
							if (start == currentlyDrawnTo) {
								bitmapLayers++;
								if (duration < segmentDuration)
									segmentDuration = duration;
							} else if (start - currentlyDrawnTo < segmentDuration) {
								segmentDuration = start - currentlyDrawnTo;
							}

							// record the earliest start time for drawable blocks we've found; if we
							// don't draw anything here, we just skip through to this time later
							if (start < earliestStart)
								earliestStart = start;
						}

						bool firstDraw = true;
						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw.ToArray()) {
							Bitmap bmp = drawDetails.bmp;
							TimeSpan start = drawDetails.startTime;
							TimeSpan duration = drawDetails.duration;

							// only draw elements that are at the point we are currently drawing from
							if (start != currentlyDrawnTo)
								continue;

							PointF location = new PointF(timeToPixels(start), top);

							if (duration != segmentDuration) {
								// it must be longer; crop the bitmap into a smaller one
								float croppedWidth = timeToPixels(segmentDuration);
								Bitmap croppedBitmap = bmp.Clone(new RectangleF(0, 0, croppedWidth, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
								drawDetails.bmp = bmp.Clone(new RectangleF(croppedWidth, 0, bmp.Width - croppedWidth, bmp.Height), bmp.PixelFormat);
								drawDetails.startTime = start + segmentDuration;
								drawDetails.duration = duration - segmentDuration;
								bmp = croppedBitmap;
							}

							if (firstDraw) {
								g.DrawImage(bmp, location);
								firstDraw = false;
							} else {
								// get the bitmap data in a nice, quick format
								BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
								IntPtr ptr = bmpdata.Scan0;
								int bytes = bmpdata.Stride * bmp.Height;
								byte[] argbValues = new byte[bytes];
								System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

								// set the alpha to 100/bitmapLayers percent
								byte value = (byte)(255 / bitmapLayers);
								for (int counter = 3; counter < argbValues.Length; counter += 4)
									argbValues[counter] = value;

								// put the bitmap data back
								System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
								bmp.UnlockBits(bmpdata);
								g.DrawImage(bmp, location);
							}

							if (duration == segmentDuration) {
								bitmapsToDraw.Remove(drawDetails);
							}
						}

						if (segmentDuration < TimeSpan.MaxValue)
							currentlyDrawnTo += segmentDuration;
						else
							currentlyDrawnTo = earliestStart;
					}

				}


				top += row.Height;  // next row starts just below this row
			}
		}

		private void _drawSelection(Graphics g)
		{
			if (SelectedArea == null)
				return;

			using (SolidBrush b = new SolidBrush(SelectionColor))
			{
				g.FillRectangle(b, SelectedArea);
			}
			using (Pen p = new Pen(SelectionBorder))
			{
				g.DrawRectangle(p, SelectedArea);
			}
		}

		private void _drawCursor(Graphics g)
		{
			using (Pen p = new Pen(CursorColor, CursorWidth)) {
				g.DrawLine(p, timeToPixels(CursorPosition), 0, timeToPixels(CursorPosition), AutoScrollMinSize.Height);
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

				_drawGridlines(e.Graphics);
				int totalHeight = _drawRows(e.Graphics);
				AutoScrollMinSize = new Size((int)timeToPixels(TotalTime), totalHeight);
				_drawSnapPoints(e.Graphics);
				_drawElements(e.Graphics);
				_drawSelection(e.Graphics);
				_drawCursor(e.Graphics);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in TimelineGrid.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace);
			}
		}

		#endregion


		#region External Drag/Drop

		void TimelineGrid_DragDrop(object sender, DragEventArgs e)
		{
			Point client = PointToClient(new Point(e.X, e.Y));

			TimelineRow row = rowAt(client);
			TimeSpan time = pixelsToTime(translateLocation(client).X);
			IDataObject data = e.Data;

			if (DataDropped != null)
				DataDropped(this, new TimelineDropEventArgs(row, time, data));
		}

		void TimelineGrid_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		internal event EventHandler<TimelineDropEventArgs> DataDropped;

		#endregion

	}

	class SnapDetails
	{
		public TimeSpan SnapTime;	// the particular time to snap to
		public TimeSpan SnapStart;	// the start time that should snap to this time; ie. before or equal to the snap time
		public TimeSpan SnapEnd;	// the end time that should snap to this time; ie. after or equal to the snap time
		public int SnapLevel;		// the "priority" of this snap point; bigger is higher priority
		public TimelineRow SnapRow;	// the rows that this point should affect; null if all rows
	}

	class BitmapDrawDetails
	{
		public Bitmap bmp;			// the bitmap to be drawn from the start time onwards
		public TimeSpan startTime;	// the start time that this bitmap should be drawn from
		public TimeSpan duration;	// how long (time) this bitmap should be drawn for
	}

	// Enumerations
	enum DragState
	{
		/// <summary>
		/// Not dragging, mouse is up.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Mouse down, but hasn't moved past threshold yet to be considered dragging
		/// </summary>
		Waiting,

		/// <summary>
		/// Actively dragging objects
		/// </summary>
		Dragging,

		/// <summary>
		/// Like "Dragging", but dragging on the background, not an object
		/// </summary>
		Selecting,


	}
}
