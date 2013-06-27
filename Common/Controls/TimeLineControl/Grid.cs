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
using System.Runtime.InteropServices;
using Common.Controls.ColorManagement.ColorModels;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Execution.Context;
using System.Collections.Concurrent;

namespace Common.Controls.Timeline
{
	/// <summary>
	/// Makes up the main part of the TimelineControl. A scrollable container which presents rows which contain elements.
	/// </summary>
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public partial class Grid : TimelineControlBase, IEnumerable<Row>, IDisposable
	{
		#region Members

		private List<Row> m_rows; // the rows in the grid
		private DragState m_dragState = DragState.Normal; // the current dragging state

		private Point m_selectionRectangleStart; // the location (on the grid canvas) where the selection box starts.
		private Rectangle m_ignoreDragArea; // the area in which move movements should be ignored, before we start dragging
		private List<Element> m_mouseDownElements; // the elements under the cursor on a mouse click

		private Row m_mouseDownElementRow = null;
		            // the row that the clicked m_mouseDownElement belongs to (a single element may be in multiple rows)

		private TimeSpan m_cursorPosition; // the current grid 'cursor' position (line drawn vertically)

		#endregion

		private ElementMoveInfo m_elemMoveInfo;
		private BackgroundWorker renderWorker = null;
		public ISequenceContext Context = null;
		public bool SequenceLoading { get; set; }

		#region Initialization

		public Grid(TimeInfo timeinfo)
			: base(timeinfo)
		{
			this.AutoScroll = true;
			AllowGridResize = true;
			AutoScrollMargin = new Size(24, 24);
			TotalTime = TimeSpan.FromMinutes(1);
			RowSeparatorColor = Color.Black;
			MajorGridlineColor = Color.FromArgb(120, 120, 120);
			GridlineInterval = TimeSpan.FromSeconds(1.0);
			BackColor = Color.FromArgb(60, 60, 60);
			SelectionColor = Color.FromArgb(100, 40, 100, 160);
			SelectionBorder = Color.Blue;
			CursorColor = Color.FromArgb(150, 50, 50, 50);
			CursorWidth = 2.5F;
			CursorPosition = TimeSpan.Zero;
			OnlySnapToCurrentRow = true;
			DragThreshold = 8;
			StaticSnapPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>();
			SnapPriorityForElements = 5;
			ClickingGridSetsCursor = true;

			m_rows = new List<Row>();

			InitAutoScrollTimer();

			// thse changed events are static for the class. If we make them per element or row
			//  later, we will need to attach/detach from each event manually.
			Row.RowChanged += RowChangedHandler;
			Row.RowSelectedChanged += RowSelectedChangedHandler;
			Row.RowToggled += RowToggledHandler;
			Row.RowHeightChanged += RowHeightChangedHandler;

			// Drag & Drop
			AllowDrop = true;
			DragEnter += TimelineGrid_DragEnter;
			DragDrop += TimelineGrid_DragDrop;
		}

		protected override void Dispose(bool disposing)
		{
			// Cancel the background worker
			if (renderWorker != null) {
				renderWorker.CancelAsync();
				while (renderWorker.IsBusy) Application.DoEvents();
			}

			base.Dispose(disposing);
		}

		#endregion

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			AutoScrollPosition = new Point((int) timeToPixels(VisibleTimeStart), -AutoScrollPosition.Y);
			base.OnVisibleTimeStartChanged(sender, e);
		}

		protected override void OnTimePerPixelChanged(object sender, EventArgs e)
		{
			RecalculateAllStaticSnapPoints();
			ResizeGridHorizontally();
			base.OnTimePerPixelChanged(sender, e);
		}


		protected override void OnTotalTimeChanged(object sender, EventArgs e)
		{
			ResizeGridHorizontally();
			base.OnTotalTimeChanged(sender, e);
		}


		private void ResizeGridHorizontally()
		{
			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericDelegate(ResizeGridHorizontally));
			else {
				// resize the scroll canvas to be the new size of the whole display time, and cap it to not go past the end
				AutoScrollMinSize = new Size((int) timeToPixels(TotalTime), AutoScrollMinSize.Height);

				if (VisibleTimeEnd > TotalTime)
					VisibleTimeStart = TotalTime - VisibleTimeSpan;

				//shuffle the grid position to line up with where the visible time start should be
				AutoScrollPosition = new Point((int) timeToPixels(VisibleTimeStart), -AutoScrollPosition.Y);
			}
		}

		#region Properties

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
			get { return Rows.Where(x => x.Visible); }
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

		public Row BottomVisibleRow
		{
			get { return rowAt(new Point(0, VerticalOffset + ClientRectangle.Height)); }
		}

		public TimeSpan CursorPosition
		{
			get { return m_cursorPosition; }
			set
			{
				m_cursorPosition = value;
				_CursorMoved(value);
				Invalidate();
			}
		}

		public TimeSpan GridlineInterval { get; set; }
		public bool OnlySnapToCurrentRow { get; set; }
		public int SnapPriorityForElements { get; set; }
		public int DragThreshold { get; set; }
		public Rectangle SelectionArea { get; set; }
		public bool ClickingGridSetsCursor { get; set; }

		// drawing colours, information, etc.
		public Color RowSeparatorColor { get; set; }
		public Color MajorGridlineColor { get; set; }
		public Color SelectionColor { get; set; }
		public Color SelectionBorder { get; set; }
		public Color CursorColor { get; set; }
		public Single CursorWidth { get; set; }

		// private properties
		private bool CtrlPressed
		{
			get { return Form.ModifierKeys.HasFlag(Keys.Control); }
		}

		private int CurrentRowIndexUnderMouse { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> StaticSnapPoints { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> CurrentDragSnapPoints { get; set; }

		#endregion

		#region Events

		public event EventHandler SelectionChanged;
		public event EventHandler<ElementEventArgs> ElementDoubleClicked;
		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving;
		public event EventHandler<TimeSpanEventArgs> CursorMoved;
		public event EventHandler VerticalOffsetChanged;
		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows;
		public event EventHandler<ElementsSelectedEventArgs> ElementsSelected;
		public event EventHandler<RenderElementEventArgs> RenderProgressChanged;

		private void _SelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		private void _ElementDoubleClicked(Element te)
		{
			if (ElementDoubleClicked != null)
				ElementDoubleClicked(this, new ElementEventArgs(te));
		}

		private void _ElementsFinishedMoving(MultiElementEventArgs args)
		{
			if (ElementsFinishedMoving != null)
				ElementsFinishedMoving(this, args);
		}

		private void _CursorMoved(TimeSpan t)
		{
			if (CursorMoved != null)
				CursorMoved(this, new TimeSpanEventArgs(t));
		}

		private void _VerticalOffsetChanged()
		{
			if (VerticalOffsetChanged != null)
				VerticalOffsetChanged(this, EventArgs.Empty);
		}

		private void _ElementChangedRows(Element element, Row oldRow, Row newRow)
		{
			if (ElementChangedRows != null)
				ElementChangedRows(this, new ElementRowChangeEventArgs(element, oldRow, newRow));
		}

		// returns true if the selection should be automatically handled by the grid, or false if
		// another event handler will handle the selection process
		private bool _ElementsSelected(IEnumerable<Element> elements)
		{
			if (elements == null)
				return true;

			if (ElementsSelected != null) {
				ElementsSelectedEventArgs args = new ElementsSelectedEventArgs(elements);
				ElementsSelected(this, args);
				return args.AutomaticallyHandleSelection;
			}
			return true;
		}

		private void _RenderProgressChanged(int percent)
		{
			if (RenderProgressChanged != null) {
				EventArgs args = new EventArgs();
				RenderProgressChanged(this, new RenderElementEventArgs(percent));
			}
		}

		#endregion

		#region Event Handlers - non-mouse events

		protected void RowChangedHandler(object sender, EventArgs e)
		{
			// when dragging, the control will invalidate after it's done, in case multiple elements are changing.
			if (m_dragState != DragState.Moving && !SequenceLoading)
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
				_SelectionChanged();
			}
		}


		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			if (e.KeyChar == (char) 27) // ESC
			{
				elementsCancelMove();
			}
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			// These functions are broken out so they can be called manually elsewhere.
			if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				HandleHorizontalScroll();
			}
			else if (se.ScrollOrientation == ScrollOrientation.VerticalScroll) {
				HandleVerticalScroll();
			}

			base.OnScroll(se);
		}

		private void HandleVerticalScroll()
		{
			_VerticalOffsetChanged();
		}

		///<summary>Should be called whenever the scroll position of the grid changes, be it 
		///via OnScroll event, or manually.</summary>
		private void HandleHorizontalScroll()
		{
			VisibleTimeStart = pixelsToTime(-AutoScrollPosition.X);
		}


		protected override void OnResize(EventArgs e)
		{
			// If the *would be* Visible end time is past the total time, adjust the start
			// such that the visible end remains at the total time.
			// Note: we cannot use VisibleTimeEnd because it protects against this out-of-bounds condition (Min).
			if (VisibleTimeStart + VisibleTimeSpan > TotalTime)
				VisibleTimeStart = TotalTime - VisibleTimeSpan;

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
			Invalidate();
			_SelectionChanged();
		}

		public void ClearSelectedRows(Row leaveRowSelected = null)
		{
			foreach (Row row in Rows) {
				if (row != leaveRowSelected)
					row.Selected = false;
			}
			Invalidate();
			_SelectionChanged();
		}

		public void AddRow(Row row)
		{
			Rows.Add(row);
			ResizeGridHeight();
			Invalidate();
		}

		public bool RemoveRow(Row row)
		{
			bool rv = Rows.Remove(row);
			ResizeGridHeight();
			Invalidate();
			return rv;
		}

		/// <summary>
		/// Moves the given elements by the given amount of time. This is similar to the mouse dragging events, except
		/// it's a single 'atomic' operation that moves the elements and raises an event to indicate they have moved.
		/// Note that it does not utilize snap points at all.
		/// </summary>
		public void MoveElementsByTime(IEnumerable<Element> elements, TimeSpan offset)
		{
			if (elements.Count() <= 0)
				return;

			m_elemMoveInfo = new ElementMoveInfo(new Point(), elements, VisibleTimeStart);

			TimeSpan earliest = elements.Min(x => x.StartTime);
			if ((earliest + offset) < TimeSpan.Zero)
				offset = -earliest;

			TimeSpan latest = elements.Max(x => x.EndTime);
			if ((latest + offset) > TimeInfo.TotalTime)
				offset = TimeInfo.TotalTime - latest;

			foreach (Element elem in elements) {
				elem.BeginUpdate();
				elem.StartTime += offset;
				elem.EndUpdate();
			}

			MultiElementEventArgs evargs = new MultiElementEventArgs {Elements = elements};
			_ElementsFinishedMoving(evargs);

			if (ElementsMovedNew != null)
				ElementsMovedNew(this, new ElementsChangedTimesEventArgs(m_elemMoveInfo, ElementMoveType.Move));

			m_elemMoveInfo = null;
		}


		//TODO: Move me


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
		/// Returns the first element found that is located at the given point in client coordinates
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

		/// <summary>
		/// Returns all elements located at the given point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Elements at given point, or null if none exists.</returns>
		protected List<Element> elementsAt(Point p)
		{
			List<Element> result = new List<Element>();

			// First figure out which row we are in
			Row containingRow = rowAt(p);

			if (containingRow == null)
				return result;

			// Now figure out which element we are on
			foreach (Element elem in containingRow) {
				Single elemX = timeToPixels(elem.StartTime);
				Single elemW = timeToPixels(elem.Duration);
				if (p.X >= elemX && p.X <= elemX + elemW)
					result.Add(elem);
			}

			return result;
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

			// deselect all elements in the grid first, then only select the ones in the box.
			ClearSelectedElements();

			// Iterate all elements of only the rows within our selection.
			bool startFound = false, endFound = false;
			foreach (var row in Rows) {
				if (
					!row.Visible || // row is invisible
					endFound || // we already passed the end row
					(!startFound && (row != startRow)) //we haven't found the first row, and this isn't it
					) {
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

			_SelectionChanged();
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

			Dictionary<Element, int> elementsLeft = new Dictionary<Element, int>();
			if (elementInstanceCounts != null) {
				// copy the given instance counts of each element into a new dictionary; we'll decrement the
				// counts as we go to find the 'last' possible item
				elementsLeft = new Dictionary<Element, int>(elementInstanceCounts);
			}

			// we either go forwards through the row list to find the highest row possible, or backwards to find the
			// lowest, based on which one was requested through findTopLimitRow
			for (int i = findTopLimitRow ? 0 : Rows.Count - 1;
			     findTopLimitRow ? (i < Rows.Count) : (i >= 0);
			     i += (findTopLimitRow ? 1 : -1)) {
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

		public TimeSpan EarliestStartTime(IEnumerable<Element> elements)
		{
			TimeSpan result = TimeSpan.MaxValue;
			foreach (Element e in elements) {
				if (e.StartTime < result)
					result = e.StartTime;
			}

			return result;
		}

		public TimeSpan LatestEndTime(IEnumerable<Element> elements)
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
			TimeSpan earliest = EarliestStartTime(SelectedElements);

			// Find the earliest time of each row
			var rowsToStartTimes = new Dictionary<Row, TimeSpan>();
			foreach (Row row in Rows) {
				TimeSpan time = EarliestStartTime(row.SelectedElements);

				if (time != TimeSpan.MaxValue) {
					rowsToStartTimes.Add(row, time);
				}
			}

			// Now adjust all elements in each row, such that the leftmost element (in this row)
			// is at the same start time as the alltogether leftmost element.
			foreach (KeyValuePair<Row, TimeSpan> kvp in rowsToStartTimes) {
				// calculate how much to offset elements of this row
				TimeSpan thisRowAdjust = kvp.Value - earliest;

				if (thisRowAdjust == TimeSpan.Zero)
					continue;

				foreach (Element elem in kvp.Key.SelectedElements)
					elem.StartTime -= thisRowAdjust;
			}
		}

		/// <summary>
		/// Given a collection of elements, and a desired offset time (from the original pre-drag time), this method will
		/// find the best matching offset time after considering all possible snap points that would be applicable to those
		/// elements. If there's no snap points currently used, the return value should be the same as the given offset.
		/// </summary>
		/// <param name="elements">The elements to consider for snapping.</param>
		/// <param name="offset">The desired offset time (from the element's original time position, pre-drag).</param>
		/// <param name="resize">if the elements are being resized, and if so, from the front or back.</param>
		/// <returns>The real offset time that should be used from the element's original time position.</returns>
		public TimeSpan FindSnapTimeForElements(IEnumerable<Element> elements, TimeSpan offset, ResizeZone resize)
		{
			// if the offset was zero, we don't need to do anything.
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
			}
			else {
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
				ElementTimeInfo orig = m_elemMoveInfo.OriginalElements[element];

				if (orig == null) {
					throw new Exception(
						"Timeline Control: trying to find snap point for element, but can't find original time location!");
				}

				foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in CurrentDragSnapPoints) {
					foreach (SnapDetails details in kvp.Value) {
						// skip this point if it's not any higher priority than our highest so far
						if (bestSnapPoint != null && details.SnapLevel <= bestSnapPoint.SnapLevel)
							continue;

						// skip this one if it's not for the row that the current element is on
						if (details.SnapRow != null && details.SnapRow != thisElementsRow)
							continue;

						// figure out if the element start time or end times are in the snap range; if not, skip it
						bool startInRange = (orig.StartTime + offset > details.SnapStart && orig.StartTime + offset < details.SnapEnd);
						bool endInRange = (orig.EndTime + offset > details.SnapStart && orig.EndTime + offset < details.SnapEnd);

						// if we're resizing, ignore snapping on the end of the element that is not getting resized.
						if (resize == ResizeZone.Front)
							endInRange = false;
						else if (resize == ResizeZone.Back)
							startInRange = false;

						if (!startInRange && !endInRange)
							continue;

						// calculate the best side (start or end) to snap to the snap time
						if (startInRange && endInRange) {
							if (details.SnapTime - orig.StartTime + offset < orig.EndTime + offset - details.SnapTime)
								snappedOffset = details.SnapTime - orig.StartTime;
							else
								snappedOffset = details.SnapTime - orig.EndTime;
						}
						else {
							if (startInRange)
								snappedOffset = details.SnapTime - orig.StartTime;
							else
								snappedOffset = details.SnapTime - orig.EndTime;
						}

						bestSnapPoint = details;
					}
				}
			}

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
			int topElementVisibleRowIndex =
				visibleRows.IndexOf(GetVerticalLimitRowForElements(elements, true, true, elementCounts, m_mouseDownElementRow));
			int bottomElementVisibleRowIndex =
				visibleRows.IndexOf(GetVerticalLimitRowForElements(elements, false, true, elementCounts, m_mouseDownElementRow));

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
								throw new Exception(
									"Trying to move element off-grid, but there's no more instances of this element to move instead!");
							continue;
						}

						visibleRows[i].RemoveElement(element);
						visibleRows[i + visibleRowsToMove].AddElement(element);
						elementsMoved.Add(element);
						_ElementChangedRows(element, visibleRows[i], visibleRows[i + visibleRowsToMove]);

						// if this element was the mouse down element, update the mouse down element row that we're tracking
						if (m_mouseDownElements != null && element == m_mouseDownElements.FirstOrDefault())
							newMouseDownRow = visibleRows[i + visibleRowsToMove];
					}
				}

				foreach (Element e in elementsMoved)
					elementsToMove[e] = true;
			}

			CurrentRowIndexUnderMouse =
				Rows.IndexOf(visibleRows[(visibleRows.IndexOf(Rows[CurrentRowIndexUnderMouse]) + visibleRowsToMove)]);
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
			result.SnapStart = snapTime - TimeSpan.FromTicks(TimePerPixel.Ticks*level*2);
			result.SnapEnd = snapTime + TimeSpan.FromTicks(TimePerPixel.Ticks*level*2);
			return result;
		}

		public void AddSnapPoint(TimeSpan snapTime, int level, Color color)
		{
			if (!StaticSnapPoints.ContainsKey(snapTime))
				StaticSnapPoints.Add(snapTime, new List<SnapDetails> {CalculateSnapDetailsForPoint(snapTime, level, color)});
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

		public bool AllowGridResize { get; set; }

		public void ResizeGridHeight()
		{
			if (AllowGridResize) {
				if (this.InvokeRequired) {
					this.Invoke(new Vixen.Delegates.GenericDelegate(ResizeGridHeight));
				}
				else {
					AutoScrollMinSize = new Size((int) timeToPixels(TotalTime), CalculateAllRowsHeight());
					Invalidate();
				}
			}
		}

		public void SelectElement(Element element)
		{
			element.Selected = true;
			_SelectionChanged();
		}

		public void DeselectElement(Element element)
		{
			element.Selected = false;
			_SelectionChanged();
		}

		public void ToggleElementSelection(Element element)
		{
			element.Selected = !element.Selected;
			_SelectionChanged();
		}

		#endregion

		#region Drawing

		private void _drawRows(Graphics g)
		{
			int curY = 0;

			// Draw row separators
			using (Pen p = new Pen(RowSeparatorColor))
			using (SolidBrush b = new SolidBrush(SelectionColor)) {
				foreach (Row row in Rows) {
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
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart)%interval) + interval;
			Single end = timeToPixels(VisibleTimeEnd);

			using (Pen p = new Pen(MajorGridlineColor)) {
				p.DashStyle = DashStyle.Dash;
				for (Single x = start; x <= end; x += interval) {
					g.DrawLine(p, x, (-AutoScrollPosition.Y), x, (-AutoScrollPosition.Y) + Height);
				}
			}
		}

		private void _drawSnapPoints(Graphics g)
		{
			Pen p;

			// iterate through all snap points, and if it's visible, draw it
			foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints) {
				SnapDetails details = null;
				foreach (SnapDetails d in kvp.Value) {
					if (details == null || (d.SnapLevel > details.SnapLevel && d.SnapColor != Color.Empty))
						details = d;
				}
				if (kvp.Key >= VisibleTimeStart && kvp.Key < VisibleTimeEnd) {
					p = new Pen(details.SnapColor);
					Single x = timeToPixels(kvp.Key);
					p.DashPattern = new float[] {details.SnapLevel, details.SnapLevel};
					g.DrawLine(p, x, 0, x, AutoScrollMinSize.Height);
				}
			}
		}

		#region Element rendering background worker

		private void renderWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Bitmap elementImage;

			// Continue looping through the elements to see if they need to be rendered.
			// They will the first time and when a row is expanded.
			while (!renderWorker.CancellationPending) {
				// Count the rows for progress reporting
				int totalElements = 0;
				foreach (Row row in Rows) {
					if (row.Visible) {
						for (int i = 0; i < row.ElementCount; i++) {
							if (row.GetElementAtIndex(i).Changed)
								totalElements += 1;
						}
					}
				}

				if (totalElements == 1)
					renderWorker.ReportProgress(50);

				int currentElementNum = 0;
				foreach (Row row in Rows) {
					if (row.Visible) {
						for (int i = 0; i < row.ElementCount; i++) {
							if (renderWorker.CancellationPending)
								return;

							// If we have a sequence playing, stop rendering the effects in the background
							while (Context != null && Context.IsRunning)
								Thread.Sleep(250);

							Element currentElement = row.GetElementAtIndex(i);
							Size size = new Size((int) Math.Ceiling(timeToPixels(currentElement.Duration)), row.Height - 1);
							if (currentElement.Changed || !currentElement.CachedCanvasIsCurrent) {
								elementImage = currentElement.SetupCachedImage(size);
								if (currentElement.StartTime <= VisibleTimeEnd && currentElement.EndTime >= VisibleTimeStart)
									Invalidate();
								renderWorker.ReportProgress((int) ((double) (currentElementNum)/(double) totalElements*100.0));
							}

							currentElementNum++;
						}

						Thread.Sleep(5);
						Application.DoEvents();
					}
				}

				// Turn off the progress reporting if it is on
				renderWorker.ReportProgress(100);

				// Sleep between each iteration
				Thread.Sleep(250);
			}
		}

		private void renderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			_RenderProgressChanged(e.ProgressPercentage);
		}

		public void StartBackgroundRendering()
		{
			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericDelegate(StartBackgroundRendering));
			else {
				if (renderWorker != null) {
					//while (renderWorker.IsBusy) { Thread.Sleep(10); }; 
					if (!renderWorker.IsBusy)
						renderWorker.RunWorkerAsync();
				}
				else {
					renderWorker = new BackgroundWorker();
					renderWorker.WorkerReportsProgress = true;
					renderWorker.WorkerSupportsCancellation = true;
					renderWorker.DoWork += new DoWorkEventHandler(renderWorker_DoWork);
					//renderWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(renderWorker_RunWorkerCompleted);
					renderWorker.ProgressChanged += new ProgressChangedEventHandler(renderWorker_ProgressChanged);
					renderWorker.RunWorkerAsync();
				}
			}
		}

		#endregion

		//static public System.Object drawLock = new System.Object();
		private void _drawElements(Graphics g)
		{
			// Draw each row
			int top = 0; // y-coord of top of current row
			foreach (Row row in Rows) {
				if (!row.Visible)
					continue;

				if (top < VerticalOffset || top > VerticalOffset + ClientSize.Height) {
					top += row.Height; // next row starts just below this row
					continue;
				}

				// a list of generated bitmaps, with starttime and endtime for where they are supposed to be drawn.
				List<BitmapDrawDetails> bitmapsToDraw = new List<BitmapDrawDetails>();
				TimeSpan currentlyDrawnTo = TimeSpan.Zero;
				TimeSpan desiredDrawTo = TimeSpan.Zero;
				bool lastItemDrawn = false;

				for (int i = 0; i < row.ElementCount; i++) {
					Element currentElement = row.GetElementAtIndex(i);
					if (currentElement.EndTime < VisibleTimeStart)
						continue;

					if (currentElement.StartTime > VisibleTimeEnd) {
						if (lastItemDrawn) {
							continue;
						}
						else
							lastItemDrawn = true;
					}

					desiredDrawTo = currentElement.StartTime;

					// if this is the last element, draw everything
					if (i == row.ElementCount - 1) {
						desiredDrawTo = TotalTime;
					}

					Size size = new Size((int) Math.Ceiling(timeToPixels(currentElement.Duration)), row.Height - 1);
					//Bitmap elementImage = currentElement.Draw(size, Context.IsRunning);
					//Bitmap elementImage = currentElement.Draw(size, true);
					Bitmap elementImage = currentElement.Draw(size);

					Point pt = new Point((int) Math.Floor(timeToPixels(currentElement.StartTime)), top);

					bitmapsToDraw.Add(new BitmapDrawDetails()
					                  	{bmp = elementImage, startTime = currentElement.StartTime, duration = currentElement.Duration});


					//
					//// Everything below combines the bitmaps to display them
					//

					// oh god make it stop
					int iterations = 0;

					while (currentlyDrawnTo < desiredDrawTo) {
						// if there's nothing left to draw, the rest of it is empty; skip to the desired draw point
						if (bitmapsToDraw.Count == 0) {
							currentlyDrawnTo = desiredDrawTo;
							break;
						}

						TimeSpan processingSegmentDuration = TimeSpan.MaxValue;
						TimeSpan drawingSegmentDuration;
						TimeSpan earliestStart = TimeSpan.MaxValue;

						// these handle the fact that our pixels don't perfectly align with exact start times of elements, and are used to
						// capture any timespan that's within the single pixel range of the 'current drawn to' point
						int lowerBoundPixels = (int) Math.Floor(timeToPixels(currentlyDrawnTo));
						int upperBoundPixels = (int) Math.Ceiling(timeToPixels(currentlyDrawnTo));
						if (upperBoundPixels <= lowerBoundPixels)
							upperBoundPixels = lowerBoundPixels + 1;
						TimeSpan currentlyDrawnMin = pixelsToTime(lowerBoundPixels);
						TimeSpan currentlyDrawnMax = pixelsToTime(upperBoundPixels);


						// find how many bitmaps are going to be in this next segment, and also figure out the smallest whole section
						// to draw before another element starts or until one of the current ones end
						int bitmapLayers = 0;
						TimeSpan smallestDrawingDuration = TimeSpan.MaxValue;
						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw) {
							TimeSpan start = drawDetails.startTime;
							TimeSpan duration = drawDetails.duration;

							// dodgy workaround: if the current element somehow has a start time BEFORE the currently drawn to time,
							// something went wrong. Set it to the currently drawn time, and it shouldget picked up as part of the process.
							if (start <= currentlyDrawnMin)
								drawDetails.startTime = start = currentlyDrawnTo;

							// if this bitmap is within the single pixel range that is currently drawing, we'll be drawing it.
							// check the duration to make sure we're drawing the smallest possible slice of elements.
							if (start >= currentlyDrawnMin && start <= currentlyDrawnMax) {
								bitmapLayers++;
								if (duration < smallestDrawingDuration)
									smallestDrawingDuration = duration;
							}

							// record the earliest start time for drawable blocks we've found; if we
							// don't draw anything here, we just skip through to this time later
							if (start < earliestStart)
								earliestStart = start;
						}

						TimeSpan earliestTimeToNextDrawing = TimeSpan.MaxValue;
						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw.ToArray()) {
							TimeSpan start = drawDetails.startTime;

							// check if the start for this one is out of range (too early); if it is, delete it
							// workaround -- if the currently drawn one actually starts EARLIER than we're up to, something
							// went wrong, don't draw it. I can't track the issue down...
							// (for Future Me: I think the problem is down below, where it crops the bitmap into smaller ones.)
							if (start < currentlyDrawnTo) {
								//Debugger.Break();
								bitmapsToDraw.Remove(drawDetails);
								continue;
							}


							// if this drawing bitmap isn't starting at the current point, it might be the next one; check if we can
							// cut down the current drawing segment as another one will be starting soon
							if (start > currentlyDrawnMax && start - currentlyDrawnTo < earliestTimeToNextDrawing) {
								earliestTimeToNextDrawing = start - currentlyDrawnTo;
							}
						}

						if (smallestDrawingDuration < earliestTimeToNextDrawing)
							processingSegmentDuration = smallestDrawingDuration;
						else
							processingSegmentDuration = earliestTimeToNextDrawing;

						bool firstDraw = true;
						Bitmap finalBitmap = null;
						BitmapData finalBitmapData = null;
						Point? finalDrawLocation = null;

						foreach (BitmapDrawDetails drawDetails in bitmapsToDraw.ToArray()) {
							Bitmap bmp = drawDetails.bmp;
							TimeSpan start = drawDetails.startTime;
							TimeSpan duration = drawDetails.duration;
							bool bitmapContinuesPastCurrentDrawDuration = false;

							// only draw elements that are at the point we are currently drawing from
							if (start < currentlyDrawnMin || start > currentlyDrawnMax)
								continue;

							if (duration != processingSegmentDuration) {
								// it must be longer; crop the bitmap into a smaller one
								int croppedWidth = (int) Math.Ceiling(timeToPixels(processingSegmentDuration));
								drawingSegmentDuration = pixelsToTime(croppedWidth);
								if (croppedWidth > 0 && bmp.Width - croppedWidth > 0) {
									bitmapContinuesPastCurrentDrawDuration = true;
									Bitmap croppedBitmap = bmp.Clone(new Rectangle(0, 0, croppedWidth, bmp.Height), PixelFormat.Format32bppArgb);
									drawDetails.bmp = bmp.Clone(new Rectangle(croppedWidth, 0, bmp.Width - croppedWidth, bmp.Height),
									                            bmp.PixelFormat);
									//if (start + drawingSegmentDuration < currentlyDrawnTo + processingSegmentDuration) Debugger.Break();
									drawDetails.startTime = start + drawingSegmentDuration;
									drawDetails.duration = duration - drawingSegmentDuration;
									bmp = croppedBitmap;
								}
							}

							try {
								// for the first time around, set up the final bitmap data item and lock it
								if (firstDraw) {
									finalBitmap = new Bitmap((int) Math.Ceiling(timeToPixels(processingSegmentDuration)), bmp.Height);
									finalDrawLocation = new Point((int) Math.Floor(timeToPixels(start)), top);
									firstDraw = false;
								}
							}
							catch (Exception e) {
								Debugger.Break();
							}

							if (bitmapLayers == 1) {
								g.DrawImage(bmp, (Point) finalDrawLocation);
							}
							else {
								if (finalBitmapData == null) {
									finalBitmapData = finalBitmap.LockBits(new Rectangle(0, 0, finalBitmap.Width, finalBitmap.Height),
									                                       ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
								}

								unsafe {
									// get the drawing bitmap data in a nice, quick format, and blat it onto the final bitmap
									// (http://stackoverflow.com/questions/12170894/drawing-image-with-additive-blending/12210258#12210258)
									BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
									                                  PixelFormat.Format32bppArgb);
									byte* pbmp = (byte*) bmpdata.Scan0.ToPointer();
									byte* pfinal = (byte*) finalBitmapData.Scan0.ToPointer();

									int bmpBPP = bmpdata.Stride/bmpdata.Width;
									int finalBPP = finalBitmapData.Stride/finalBitmapData.Width;

									// we're going to assume that the final bitmap data is the same size as the bmpdata (they should be!)
									// and iterate over them both at the same time
									for (int j = 0; j < bmpdata.Height; j++) {
										for (int k = 0; k < bmpdata.Width; k++) {
											//double scaleB = (double)*(pbmp + 3) / byte.MaxValue;
											//double scaleF = (double)*(pfinal + 3) / byte.MaxValue;

											//Color c = Color.FromArgb(
											//    Math.Max((byte)(*(pbmp + 2) * scaleB), (byte)(*(pfinal + 2) * scaleF)),
											//    Math.Max((byte)(*(pbmp + 1) * scaleB), (byte)(*(pfinal + 1) * scaleF)),
											//    Math.Max((byte)(*(pbmp + 0) * scaleB), (byte)(*(pfinal + 0) * scaleF))
											//    );

											//byte intensity = (byte)(HSV.FromRGB(c).V * byte.MaxValue);

											//*(pfinal + 0) = c.B;
											//*(pfinal + 1) = c.G;
											//*(pfinal + 2) = c.R;
											//*(pfinal + 3) = intensity;


											// get the scale of the alpha to apply to the other input components
											double inputIntensityScale = (double) *(pbmp + 3)/byte.MaxValue;

											// do alpha byte. It should be the max of any pixel at the position (ie. be as opaque as possible)
											*(pfinal + 3) = Math.Max(*(pfinal + 3), *(pbmp + 3));

											// do RGB components of the pixel. Scale any incoming data by the alpha of its channel (to dial the intensity back
											// to what it would really be), then try and find the highest of any individual component to get the final color.
											*(pfinal + 0) = Math.Max(*(pfinal + 0), (byte) (*(pbmp + 0)*inputIntensityScale));
											*(pfinal + 1) = Math.Max(*(pfinal + 1), (byte) (*(pbmp + 1)*inputIntensityScale));
											*(pfinal + 2) = Math.Max(*(pfinal + 2), (byte) (*(pbmp + 2)*inputIntensityScale));


											pfinal += finalBPP;
											pbmp += bmpBPP;
										}
									}

									bmp.UnlockBits(bmpdata);
								}
							}

							if (!bitmapContinuesPastCurrentDrawDuration) {
								bitmapsToDraw.Remove(drawDetails);
							}
						}

						if (finalBitmap != null) {
							if (finalBitmapData != null) {
								finalBitmap.UnlockBits(finalBitmapData);
							}
							g.DrawImage(finalBitmap, (Point) finalDrawLocation);
						}

						if (processingSegmentDuration < TimeSpan.MaxValue)
							currentlyDrawnTo += processingSegmentDuration;
						else
							currentlyDrawnTo = earliestStart;

						// dodgy, dodgy hack to avoid infinite loops and hangs: if we are looping too long, bail!
						if (iterations > 10000) {
							currentlyDrawnTo = desiredDrawTo;
						}
						else
							iterations++;
					}


					//// TO HERE
				}

				top += row.Height; // next row starts just below this row
			}
		}

		private void _drawSelection(Graphics g)
		{
			if (SelectionArea == null)
				return;

			using (SolidBrush b = new SolidBrush(SelectionColor)) {
				g.FillRectangle(b, SelectionArea);
			}
			using (Pen p = new Pen(SelectionBorder)) {
				g.DrawRectangle(p, SelectionArea);
			}
		}

		private void _drawCursors(Graphics g)
		{
			float curPos;

			using (Pen p = new Pen(CursorColor, CursorWidth)) {
				curPos = timeToPixels(CursorPosition);
				g.DrawLine(p, curPos, 0, curPos, AutoScrollMinSize.Height);
			}

			if (PlaybackCurrentTime.HasValue) {
				curPos = timeToPixels(PlaybackCurrentTime.Value);
				g.DrawLine(Pens.Green, curPos, 0, curPos, AutoScrollMinSize.Height);
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			if (!SequenceLoading)
				try {
					e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

					Stopwatch s = new Stopwatch();
					s.Start();

					_drawGridlines(e.Graphics);
					_drawRows(e.Graphics);
					_drawSnapPoints(e.Graphics);
					_drawElements(e.Graphics);
					_drawSelection(e.Graphics);
					_drawCursors(e.Graphics);

					s.Stop();
					//Vixen.Sys.VixenSystem.Logging.Info("OnPaint: " + s.ElapsedMilliseconds);
				}
				catch (Exception ex) {
					MessageBox.Show("Exception in TimelineGrid.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace);
				}
		}

		#endregion

		#region External Drag/Drop

		private void TimelineGrid_DragDrop(object sender, DragEventArgs e)
		{
			Point client = PointToClient(new Point(e.X, e.Y));
			Point gridPoint = translateLocation(client);

			Row row = rowAt(gridPoint);
			TimeSpan time = pixelsToTime(gridPoint.X);
			IDataObject data = e.Data;

			if (DataDropped != null)
				DataDropped(this, new TimelineDropEventArgs(row, time, data));
		}

		private void TimelineGrid_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		internal event EventHandler<TimelineDropEventArgs> DataDropped;

		#endregion
	}

	internal class SnapDetails
	{
		public TimeSpan SnapTime; // the particular time to snap to
		public TimeSpan SnapStart; // the start time that should snap to this time; ie. before or equal to the snap time
		public TimeSpan SnapEnd; // the end time that should snap to this time; ie. after or equal to the snap time
		public int SnapLevel; // the "priority" of this snap point; bigger is higher priority
		public Row SnapRow; // the rows that this point should affect; null if all rows
		public Color SnapColor; // the color to draw the snap point
	}

	internal class BitmapDrawDetails
	{
		public Bitmap bmp; // the bitmap to be drawn from the start time onwards
		public TimeSpan startTime; // the start time that this bitmap should be drawn from
		public TimeSpan duration; // how long (time) this bitmap should be drawn for
	}

	// Enumerations
	internal enum DragState
	{
		///<summary>Not dragging, mouse is up.</summary>
		Normal = 0,

		///<summary>Mouse down, but hasn't moved past threshold yet to be considered dragging.</summary>
		Waiting,

		///<summary>Actively dragging objects.</summary>
		Moving,

		///<summary>Like "Dragging", but dragging on the background, not an object.</summary>
		Selecting,

		///<summary>Dragging the mouse horizontally to resize an object in time.</summary>
		HResizing,
	}


	///<summary>Describes where the mouse is at in an element's resize zone.</summary>
	public enum ResizeZone
	{
		/// <summary>Mouse is not in an element's resize zone.</summary>
		None,

		///<summary>Mouse is in the element's front (left) resize zone.</summary>
		Front,

		///<summary>Moouse is in the element's back (right) resize zone.</summary>
		Back
	};


	///<summary>Maintains all necessary information during the user modification of selected Elements.</summary>
	public class ElementMoveInfo
	{
		public ElementMoveInfo(Point initGridLocation, IEnumerable<Element> modifyingElements, TimeSpan visibleTimeStart)
		{
			InitialGridLocation = initGridLocation;

			OriginalElements = new Dictionary<Element, ElementTimeInfo>();
			foreach (var elem in modifyingElements) {
				OriginalElements.Add(elem, new ElementTimeInfo(elem));
			}

			VisibleTimeStart = visibleTimeStart;
		}


		///<summary>The point on the grid where the mouse first went down.</summary>
		public Point InitialGridLocation { get; private set; }

		///<summary>All elements being modified and their original parameters.</summary>
		public Dictionary<Element, ElementTimeInfo> OriginalElements { get; private set; }

		public TimeSpan VisibleTimeStart { get; private set; }
	}
}