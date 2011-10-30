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
	/// <summary>
	/// Makes up the main part of the TimelineControl. A scrollable container which presents rows which contain elements.
	/// </summary>
	public class Grid : TimelineControlBase, IEnumerable<Row>
	{
		#region Members

		private List<Row> m_rows;								// the rows in the grid
		private DragState m_dragState = DragState.Normal;		// the current dragging state
		private Point m_lastMouseLocation;						// the location of the mouse at last draw; used to update the dragging.
																// Relative to the control, not the grid canvas.
		private Point m_selectionRectangleStart;				// the location (on the grid canvas) where the selection box starts.
		private Rectangle m_ignoreDragArea;						// the area in which move movements should be ignored, before we start dragging
		private Element m_mouseDownElement = null;				// the element under the cursor on a mouse click
		private Row m_mouseDownElementRow = null;				// the row that the clicked m_mouseDownElement belongs to (a single element may be in multiple rows)
		private TimeSpan m_cursorPosition;						// the current grid 'cursor' position (line drawn vertically)
		private Size m_dragAutoscrollDistance;					// how far in either dimension the mouse has moved outside a bounding area,
																// so we should scroll the viewable pane that direction
		#endregion


		#region Initialization

		//public Grid()
		public Grid(TimeInfo timeinfo)
			:base(timeinfo)
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

			m_rows = new List<Row>();
			ScrollTimer = new Timer();
			ScrollTimer.Interval = 20;
			ScrollTimer.Enabled = false;
			ScrollTimer.Tick += ScrollTimerHandler;

			// thse changed events are static for the class. If we make them per element or row
			//  later, we will need to attach/detach from each event manually.
			Row.RowChanged += RowChangedHandler;
			Row.RowSelectedChanged += RowSelectedChangedHandler;
			Row.RowToggled += RowToggledHandler;
			Row.RowHeightChanged += RowHeightChangedHandler;

			// Drag-drop 9/20/2011
			AllowDrop = true;
			DragEnter += TimelineGrid_DragEnter;
			DragDrop += TimelineGrid_DragDrop;
		}


		#endregion


		#region Properties

		protected override void  VisibleTimeStartChanged(object sender, EventArgs e)
		{
			AutoScrollPosition = new Point((int)timeToPixels(VisibleTimeStart), -AutoScrollPosition.Y);
			Invalidate();
		}

		protected override void TimePerPixelChanged(object sender, EventArgs e)
		{
			RecalculateAllStaticSnapPoints();
			Invalidate();
		}


		protected override void TotalTimeChanged(object sender, EventArgs e)
		{
			AutoScrollMinSize = new Size((int)timeToPixels(TotalTime), AutoScrollMinSize.Height);
			if (VisibleTimeEnd > TotalTime) {
				VisibleTimeStart = TotalTime - VisibleTimeSpan;
			}
			Invalidate();
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

		public List<Row> Rows
		{
			get { return m_rows; }
			protected set { m_rows = value; }
		}

		public IEnumerable<Element> SelectedElements
		{
			get { return Rows.SelectMany(x => x.SelectedElements).Distinct(); }
		}

		public IEnumerable<Row> SelectedRows
		{
			get { return Rows.Where(x => x.Selected); }
			set
			{
				foreach (Row row in Rows)
					row.Selected = value.Contains(row);
			}
		}

		public IEnumerable<Row> VisibleRows
		{
			get
			{
				return Rows.Where(x => x.Visible);
			}
			set
			{
				foreach (Row row in Rows)
					row.Selected = value.Contains(row);
			}
		}

		public Row TopVisibleRow
		{
			get { return rowAt(new Point(0, VerticalOffset)); }
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
		public event EventHandler VerticalOffsetChanged;
		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows;

		private void _ElementDoubleClicked(Element te) { if (ElementDoubleClicked != null) ElementDoubleClicked(this, new ElementEventArgs(te)); }
		private void _ElementsFinishedMoving(MultiElementEventArgs args) { if (ElementsFinishedMoving != null) ElementsFinishedMoving(this, args); }
		private void _CursorMoved(TimeSpan t) { if (CursorMoved != null) CursorMoved(this, new TimeSpanEventArgs(t)); }
		private void _VerticalOffsetChanged() { if (VerticalOffsetChanged != null) VerticalOffsetChanged(this, EventArgs.Empty); }
		private void _ElementChangedRows(Element element, Row oldRow, Row newRow) { if (ElementChangedRows != null) ElementChangedRows(this, new ElementRowChangeEventArgs(element, oldRow, newRow)); }

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
			Row selectedRow = sender as Row;

			// if CTRL wasn't down, then we want to clear all the other rows
			if (!e.ModifierKeys.HasFlag(Keys.Control)) {
				ClearSelectedElements();
				ClearSelectedRows();
				selectedRow.Selected = true;
				selectedRow.SelectAllElements();
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
					if (!(m_dragAutoscrollDistance.Width < 0 && VisibleTimeStart <= TimeSpan.Zero) &&
						!(m_dragAutoscrollDistance.Width > 0 && VisibleTimeEnd >= TotalTime)) {
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
				VisibleTimeStart = pixelsToTime(-AutoScrollPosition.X);
			}

			// This MUST be done last! Otherwise, event handlers get called with the OLD values.
			base.OnScroll(se);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_VerticalOffsetChanged();
		}

		private void RowToggledHandler(object sender, EventArgs e)
		{
			ResizeGridHeight();
		}

		private void RowHeightChangedHandler(object sender, EventArgs e)
		{
			ResizeGridHeight();
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
						ClearSelectedRows(m_mouseDownElementRow);
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
						if (m_mouseDownElementRow != null)
							m_mouseDownElementRow.Selected = true;
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
			Debug.WriteLine("Grid OnMouseHWheel: delta={0}", args.Delta);

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
				Element element = elementAt(gridLocation);

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
			Element elem = elementAt(gridLocation);

			if (elem != null) {
				_ElementDoubleClicked(elem);
			}
		}

		#endregion


		#region Methods - Rows, Elements

		public IEnumerator<Row> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void ClearSelectedElements()
		{
			foreach (Element te in SelectedElements.ToArray())
				te.Selected = false;
		}

		public void ClearSelectedRows(Row leaveRowSelected = null)
		{
			foreach (Row row in Rows) {
				if (leaveRowSelected != null && row != leaveRowSelected)
					row.Selected = false;
			}
		}

		public void AddRow(Row row)
		{
			Rows.Add(row);
			ResizeGridHeight();
		}

		public bool RemoveRow(Row row)
		{
			bool rv = Rows.Remove(row);
			ResizeGridHeight();
			return rv;
		}

		/// <summary>
		/// Returns the row located at the current point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Row at given point, or null if none exists.</returns>
		protected Row rowAt(Point p)
		{
			Row containingRow = null;
			int curheight = 0;
			foreach (Row row in Rows) {
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
		protected Element elementAt(Point p)
		{
			// First figure out which row we are in
			Row containingRow = rowAt(p);

			if (containingRow == null)
				return null;

			// Now figure out which element we are on
			foreach (Element elem in containingRow) {
				Single elemX = timeToPixels(elem.StartTime);
				Single elemW = timeToPixels(elem.Duration);
				if (p.X >= elemX && p.X <= elemX + elemW)
					return elem;
			}

			return null;
		}

		protected Row RowContainingElement(Element element)
		{
			foreach (Row row in Rows) {
				if (row.ContainsElement(element))
					return row;
			}
			return null;
		}

		public List<Element> ElementsAtTime(TimeSpan time)
		{
			List<Element> result = new List<Element>();
			foreach (Row row in Rows) {
				foreach (Element elem in row) {
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
			Row startRow = rowAt(SelectedArea.Location);
			Row endRow = rowAt(SelectedArea.BottomRight());

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
					foreach (Element e in row)
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

		/// <summary>
		/// Given a collection of elements, this method will count the number of times each element 'exists' in the rows of the grid.
		/// (This could be more than once for each element, since a single element can be added to multiple rows at the same time).
		/// </summary>
		/// <param name="elements">The collection of elements to count.</param>
		/// <param name="visibleOnly">If only visible rows should be counted or not.</param>
		/// <returns>A dictionary which maps each supplied element to an int, for the number of times it exists.</returns>
		private Dictionary<Element, int> CountRowsForElements(IEnumerable<Element> elements, bool visibleOnly)
		{
			Dictionary<Element, int> result = elements.ToDictionary(e => e, e => 0);
			foreach (Row row in Rows) {
				if (visibleOnly && !row.Visible)
					continue;

				foreach (Element element in row) {
					if (result.ContainsKey(element))
						result[element]++;
				}
			}

			return result;
		}

		/// <summary>
		/// Given a collection of elements in the grid, finds which row vertically bounds the collection. It can optionally consider
		/// duplicate instances of an individual element, and finds the lowest/highest of the elements which includes the smallest set
		/// of those duplicates.
		/// </summary>
		/// <param name="elements">The collection of elements to find a bounding row for in the grid.</param>
		/// <param name="findTopLimitRow">If the top bounding row should be calculated; if false, the bottom bounding row is calculated.</param>
		/// <param name="visibleOnly">If only visible rows and their elements should be considered.</param>
		/// <param name="elementInstanceCounts">Optional: a dictionary which maps each element to a count of 'instances' in the grid. If null, all element instances are considered.</param>
		/// <param name="skipDuplicatesUnlessInRow">Optional: a row which limits the duplicate element consideration to a max/min of this row.</param>
		/// <returns></returns>
		private Row GetVerticalLimitRowForElements(
			IEnumerable<Element> elements, 
			bool findTopLimitRow,
			bool visibleOnly,
			Dictionary<Element, int> elementInstanceCounts,
			Row skipDuplicatesUnlessInRow
			)
		{
			// check if we are considering duplicate instances or not: if not, both of the last parameters should be empty
			if (skipDuplicatesUnlessInRow == null && elementInstanceCounts != null)
				throw new Exception("GetVerticalLimitRowForElements: two last parameters need to be either both set, or both null!");

			Dictionary<Element, int> elementsLeft = new Dictionary<Element,int>();
			if (elementInstanceCounts != null) {
				// copy the given instance counts of each element into a new dictionary; we'll decrement the
				// counts as we go to find the 'last' possible item
				elementsLeft = new Dictionary<Element, int>(elementInstanceCounts);
			}

			// we either go forwards through the row list to find the highest row possible, or backwards to find the
			// lowest, based on which one was requested through findTopLimitRow
			for (int i = findTopLimitRow ? 0 : Rows.Count - 1;
				findTopLimitRow ? (i < Rows.Count) : (i >= 0);
				i += (findTopLimitRow ? 1 : -1))
			{

				// skip this row if it's not visible
				if (visibleOnly && !Rows[i].Visible)
					continue;

				// iterate through each element we're checking for a grid limit, and check if it's in this row
				foreach (Element element in elements) {
					if (Rows[i].ContainsElement(element)) {
						// if we're not bothering to check for duplicates, then this is the first row that
						// contains an element: good enough, return it!
						if (skipDuplicatesUnlessInRow == null)
							return Rows[i];

						// if this row shouldn't be checked for duplicates, end here, as we've found a 'good enough' match
						if (Rows[i] == skipDuplicatesUnlessInRow)
							return Rows[i];

						// decrement the 'element duplicate counter': if this was the last instance of this element seen,
						// then it must stop here, so return it.
						elementsLeft[element]--;
						if (elementsLeft[element] <= 0)
							return Rows[i];
					}
				}
			}

			return null;
		}

		public TimeSpan GetEarliestTimeForElements(IEnumerable<Element> elements)
		{
			TimeSpan result = TimeSpan.MaxValue;
			foreach (Element e in elements) {
				if (e.StartTime < result)
					result = e.StartTime;
			}

			return result;
		}

		public TimeSpan GetLatestTimeForElements(IEnumerable<Element> elements)
		{
			TimeSpan result = TimeSpan.MinValue;
			foreach (Element e in elements) {
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
			var rowsToStartTimes = new Dictionary<Row,TimeSpan>();
			foreach (Row row in Rows) {
				TimeSpan time = GetEarliestTimeForElements(row.SelectedElements);

				if (time != TimeSpan.MaxValue) {
					rowsToStartTimes.Add(row, time);
				}
			}

			// Now adjust all elements in each row, such that the leftmost element (in this row)
			// is at the same start time as the alltogether leftmost element.
			foreach (KeyValuePair<Row, TimeSpan> kvp in rowsToStartTimes)
			{
				// calculate how much to offset elements of this row
				TimeSpan thisRowAdjust = kvp.Value - earliest;

				if (thisRowAdjust == TimeSpan.Zero)
					continue;

				foreach (Element elem in kvp.Key.SelectedElements)
					elem.StartTime -= thisRowAdjust;
			}

		}

		public TimeSpan OffsetElementsByTime(IEnumerable<Element> elements, TimeSpan offset)
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
			List<Tuple<Element, Row>> elementsToCheckSnapping = new List<Tuple<Element, Row>>();
			if (OnlySnapToCurrentRow) {
				Row targetRow = Rows[CurrentRowIndexUnderMouse];
				foreach (Element element in elements) {
					if (targetRow.ContainsElement(element))
						elementsToCheckSnapping.Add(new Tuple<Element, Row>(element, targetRow));
				}
			} else {
				foreach (Element element in elements) {
					elementsToCheckSnapping.Add(new Tuple<Element, Row>(element, RowContainingElement(element)));
				}
			}

			// now go through all the elements we need against snap points, and check them
			SnapDetails bestSnapPoint = null;
			TimeSpan snappedOffset = offset;
			foreach (Tuple<Element, Row> tuple in elementsToCheckSnapping) {
				Element element = tuple.Item1;
				Row thisElementsRow = tuple.Item2;
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
			foreach (Element e in elements) {
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

		public void MoveElementsVerticallyToLocation(IEnumerable<Element> elements, Point gridLocation)
		{
			Row destRow = rowAt(gridLocation);

			if (destRow == null)
				return;

			if (Rows.IndexOf(destRow) == CurrentRowIndexUnderMouse)
				return;

			List<Row> visibleRows = new List<Row>();

			for (int i = 0; i < Rows.Count; i++) {
				if (Rows[i].Visible)
					visibleRows.Add(Rows[i]);
			}

			int visibleRowsToMove = visibleRows.IndexOf(destRow) - visibleRows.IndexOf(Rows[CurrentRowIndexUnderMouse]);

			// The count of each element to the number of times it is visible in the grid. Used for calculations later.
			Dictionary<Element, int> elementCounts = CountRowsForElements(elements, true);

			// find the highest and lowest visible rows with selected elements in them, but ignore duplicates unless they
			// are in the row that the mouse down element is in, or it's the last instance of the element
			int topElementVisibleRowIndex = visibleRows.IndexOf(GetVerticalLimitRowForElements(elements, true, true, elementCounts, m_mouseDownElementRow));
			int bottomElementVisibleRowIndex = visibleRows.IndexOf(GetVerticalLimitRowForElements(elements, false, true, elementCounts, m_mouseDownElementRow));

			if (visibleRowsToMove < 0 && -visibleRowsToMove > topElementVisibleRowIndex)
				visibleRowsToMove = -topElementVisibleRowIndex;

			if (visibleRowsToMove > 0 && visibleRowsToMove > visibleRows.Count - 1 - bottomElementVisibleRowIndex)
				visibleRowsToMove = visibleRows.Count - 1 - bottomElementVisibleRowIndex;

			if (visibleRowsToMove == 0)
				return;

			Dictionary<Element, bool> elementsToMove = new Dictionary<Element, bool>();
			foreach (Element e in elements) {
				elementsToMove.Add(e, false);
			}

			// take note of which elements should be moved with respect to the current mouseover row, and not just the first
			// row instance for the element that comes along
			HashSet<Element> ElementsToMoveInMouseRow = new HashSet<Element>(m_mouseDownElementRow.SelectedElements);

			// record the row that the mouse down element moves to, to update the m_mouseDownElementRow variable later
			Row newMouseDownRow = m_mouseDownElementRow;

			// OK, crazy shit's about to get real.
			// To move the elements vertically, we go through the visible rows, top down. As we find an element to be moved around
			// (by visibleRowsToMove), we move it. All well and good. HOWEVER, we have to consider elements which have multiple instances
			// in the grid. Hopefully, most of the instances would be moving to the same row, however that may not be the case: instance
			// 1 in the grid might be dragging into row A, instance 2 might be dragging into row B. Which row do we move the single instance
			// to?
			// One rule we can follow is, "if it's in same row as the element being dragged (ie. the mouse row), preferentially use that one."
			// That's easy enough. But what about elements that are not in the same row? (ie. we've clicked and dragged element 1, in row C.
			// however, element 2 which exists in rows A and E at the same time are also selected and being dragged. Do we drag to where the
			// instance in row A would go, or where the instance in row E would go?) That's the crux of the problem.
			// This is probably overkill; it's probably not going to be used *that* much.
			// for now, we will attempt to move the first element found. If it would barf (ie. moved 'off' the grid), then ignore it and try
			// the next duplicate instance of that element later on. This will probably need to be revisited later.
			// (maybe some way of trying to move the elements closest vertically, to the current mouse row? This would work best for when
			// the user selects a block of elements and moves it. The spurious ones that are also selected at extremities would be ignored.)
			for (int i = 0; i < visibleRows.Count; i++) {
				List<Element> elementsMoved = new List<Element>();

				// go through each element that hasn't been moved yet, and move it if it's in this row.
				foreach (KeyValuePair<Element, bool> kvp in elementsToMove) {
					Element element = kvp.Key;
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

						// note that we've seen another of this type of element
						elementCounts[element]--;

						// if the element would be moved outside the bounds of the grid, the ignore it. (check that there will
						// be another instance later: there should be, otherwise the calculations were wrong before!)
						if (i + visibleRowsToMove < 0 || i + visibleRowsToMove >= visibleRows.Count) {
							if (elementCounts[element] <= 0)
								throw new Exception("Trying to move element off-grid, but there's no more instances of this element to move instead!");
							continue;
						}

						visibleRows[i].RemoveElement(element);
						visibleRows[i + visibleRowsToMove].AddElement(element);
						elementsMoved.Add(element);
						_ElementChangedRows(element, visibleRows[i], visibleRows[i + visibleRowsToMove]);

						// if this element was the mouse down element, update the mouse down element row that we're tracking
						if (element == m_mouseDownElement)
							newMouseDownRow = visibleRows[i + visibleRowsToMove];
					}
				}

				foreach (Element e in elementsMoved)
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
					newPoints[kvp.Key].Add(CalculateSnapDetailsForPoint(details.SnapTime, details.SnapLevel, details.SnapColor));
				}
			}
			StaticSnapPoints = newPoints;
		}

		private SnapDetails CalculateSnapDetailsForPoint(TimeSpan snapTime, int level, Color color)
		{
			SnapDetails result = new SnapDetails();
			result.SnapLevel = level;
			result.SnapTime = snapTime;
			result.SnapColor = color;

			// the start time and end times for specified points are 2 pixels
			// per snap level away from the snap time.
			result.SnapStart = snapTime - TimeSpan.FromTicks(TimePerPixel.Ticks * level * 2);
			result.SnapEnd = snapTime + TimeSpan.FromTicks(TimePerPixel.Ticks * level * 2);
			return result;
		}

		public void AddSnapPoint(TimeSpan snapTime, int level, Color color)
		{
			if (!StaticSnapPoints.ContainsKey(snapTime))
				StaticSnapPoints.Add(snapTime, new List<SnapDetails> { CalculateSnapDetailsForPoint(snapTime, level, color) });
			else
				StaticSnapPoints[snapTime].Add(CalculateSnapDetailsForPoint(snapTime, level, color));

			Invalidate();
		}

		public bool RemoveSnapPoint(TimeSpan snapTime)
		{
			bool rv = StaticSnapPoints.Remove(snapTime);
			Invalidate();
			return rv;
		}

		public void ClearSnapPoints()
		{
			StaticSnapPoints.Clear();
			Invalidate();
		}

		private int CalculateAllRowsHeight(bool visibleRowsOnly = true)
		{
			int total = 0;

			foreach (Row row in Rows) {
				if (visibleRowsOnly && !row.Visible)
					continue;

				total += row.Height;
			}
			
			return total;
		}

		private void ResizeGridHeight()
		{
			AutoScrollMinSize = new Size((int)timeToPixels(TotalTime), CalculateAllRowsHeight());
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
			foreach (Row row in Rows) {
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

				foreach (Element element in row) {
					// skip it if it's a selected element; we don't want to snap to them, as they'll be moving as well
					if (element.Selected)
						continue;

					// if it's a non-selected element, generate snap points for it; for the start and end times. Also record the
					// row its from in the generated point, so when snapping we can check against only elements from this row.
					SnapDetails details = CalculateSnapDetailsForPoint(element.StartTime, SnapPriorityForElements, Color.Empty);
					details.SnapRow = row;

					if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime)) {
						CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
					}
					CurrentDragSnapPoints[details.SnapTime].Add(details);

					details = CalculateSnapDetailsForPoint(element.EndTime, SnapPriorityForElements, Color.Empty);
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

		private void _drawRows(Graphics g)
		{
			int curY = 0;

			// Draw row separators
			using (Pen p = new Pen(RowSeparatorColor))
			using (SolidBrush b = new SolidBrush(SelectionColor))
			{
				foreach (Row row in Rows)
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
					g.DrawLine(p, lineLeft.X, lineLeft.Y - 1, lineRight.X, lineRight.Y - 1);
				}
			}
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
			Pen p;

			// iterate through all snap points, and if it's visible, draw it
			foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints)
			{
				SnapDetails details = null;
				foreach (SnapDetails d in kvp.Value) {
					if (details == null || (d.SnapLevel > details.SnapLevel && d.SnapColor != Color.Empty))
						details = d;
				}
				if (kvp.Key >= VisibleTimeStart && kvp.Key < VisibleTimeEnd)
				{
					p = new Pen(details.SnapColor);
					Single x = timeToPixels(kvp.Key);
					p.DashPattern = new float[] { details.SnapLevel, details.SnapLevel };
					g.DrawLine(p, x, 0, x, AutoScrollMinSize.Height);
				}
			}
		}

		private void _drawElements(Graphics g)
		{
			// Draw each row
			int top = 0;    // y-coord of top of current row
			foreach (Row row in Rows) {
				if (!row.Visible)
					continue;

				// a list of generated bitmaps, with starttime and endtime for where they are supposed to be drawn.
				List<BitmapDrawDetails> bitmapsToDraw = new List<BitmapDrawDetails>();
				TimeSpan currentlyDrawnTo = TimeSpan.Zero;
				TimeSpan desiredDrawTo = TimeSpan.Zero;
				for (int i = 0; i < row.ElementCount; i++) {
					Element currentElement = row.GetElementAtIndex(i);
					desiredDrawTo = currentElement.StartTime;

					// if this is the last element, draw everything
					if (i == row.ElementCount - 1) {
						desiredDrawTo = TotalTime;
					}

					Size size = new Size((int)Math.Ceiling(timeToPixels(currentElement.Duration)), row.Height - 1);
					Bitmap elementImage = currentElement.Draw(size);
					bitmapsToDraw.Add(new BitmapDrawDetails() { bmp = elementImage, startTime = currentElement.StartTime, duration = currentElement.Duration });

					while (currentlyDrawnTo < desiredDrawTo) {
						// if there's nothing left to draw, the rest of it is empty; skip to the desired draw point
						if (bitmapsToDraw.Count == 0) {
							currentlyDrawnTo = desiredDrawTo;
							break;
						}

						// find how many bitmaps are going to be in this next segment, and also take note of the earliest time they finish
						TimeSpan processingSegmentDuration = TimeSpan.MaxValue;
						TimeSpan drawingSegmentDuration;
						TimeSpan earliestStart = TimeSpan.MaxValue;
						int bitmapLayers = 0;
						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw) {
							TimeSpan start = drawDetails.startTime;
							TimeSpan duration = drawDetails.duration;
							TimeSpan currentlyDrawnMin = pixelsToTime((int)Math.Floor(timeToPixels(currentlyDrawnTo)));
							TimeSpan currentlyDrawnMax = pixelsToTime((int)Math.Ceiling(timeToPixels(currentlyDrawnTo)));
							if (start >= currentlyDrawnMin && start <= currentlyDrawnMax) {
								bitmapLayers++;
								if (duration < processingSegmentDuration)
									processingSegmentDuration = duration;
							} else if (start - currentlyDrawnTo < processingSegmentDuration) {
								processingSegmentDuration = start - currentlyDrawnTo;
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
							bool overlapping = false;

							// only draw elements that are at the point we are currently drawing from
							TimeSpan currentlyDrawnMin = pixelsToTime((int)Math.Floor(timeToPixels(currentlyDrawnTo)));
							TimeSpan currentlyDrawnMax = pixelsToTime((int)Math.Ceiling(timeToPixels(currentlyDrawnTo)));
							if (start < currentlyDrawnMin || start > currentlyDrawnMax)
								continue;

							Point location = new Point((int)Math.Floor(timeToPixels(start)), top);

							if (duration != processingSegmentDuration) {
								// it must be longer; crop the bitmap into a smaller one
								int croppedWidth = (int)Math.Ceiling(timeToPixels(processingSegmentDuration));
								drawingSegmentDuration = pixelsToTime(croppedWidth);
								if (croppedWidth > 0 && bmp.Width - croppedWidth > 0) {
									overlapping = true;
									Bitmap croppedBitmap = bmp.Clone(new Rectangle(0, 0, croppedWidth, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
									drawDetails.bmp = bmp.Clone(new Rectangle(croppedWidth, 0, bmp.Width - croppedWidth, bmp.Height), bmp.PixelFormat);
									drawDetails.startTime = start + drawingSegmentDuration;
									drawDetails.duration = duration - drawingSegmentDuration;
									bmp = croppedBitmap;
								}
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

							if (!overlapping) {
								bitmapsToDraw.Remove(drawDetails);
							}
						}

						if (processingSegmentDuration < TimeSpan.MaxValue)
							currentlyDrawnTo += processingSegmentDuration;
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
				_drawRows(e.Graphics);
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
			Point gridPoint = translateLocation(client);

			Row row = rowAt(gridPoint);
			TimeSpan time = pixelsToTime(gridPoint.X);
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
		public Row SnapRow;			// the rows that this point should affect; null if all rows
		public Color SnapColor;		// the color to draw the snap point
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
