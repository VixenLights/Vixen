using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;


namespace Timeline
{
	public class TimelineGrid : TimelineControlBase
	{
		#region Members

		private List<TimelineRow> m_rows;						// the rows in the grid
		private DragState m_dragState = DragState.Normal;		// the current dragging state
		private Point m_lastMouseLocation;						// the location of the mouse at last draw; used to update the dragging
		private Rectangle m_ignoreDragArea;						// the area in which move movements should be ignored, before we start dragging
		private TimelineElement m_mouseDownElement = null;		// the element under the cursor on a mouse click
		private SortedDictionary<int, SnapDetails> m_snapPixels;	// a mapping of pixel location to details to snap to
		private TimeSpan m_totalTime;							// the total amount of time this grid represents
		private TimeSpan m_cursorPosition;						// the current grid 'cursor' position (line drawn vertically)

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
			OnlySnapToCurrentRow = false;		// setting this to 'true' doesn't quite work yet.
			DragThreshold = 8;

			m_rows = new List<TimelineRow>();
			SnapPoints = new SortedDictionary<TimeSpan, int>();

			// thse changed events are static for the class. If we make them per element or row
			//  later, we will need to attach/detach from each event manually.
			TimelineElement.ElementChanged += ElementChangedHandler;
			TimelineRow.RowChanged += RowChangedHandler;
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
			get { return pixelsToTime(-AutoScrollPosition.X); }
			set
			{
				if (value < TimeSpan.Zero)
					value = TimeSpan.Zero;

				// TODO: check negatives here: either they need to both be negative, or neither
				AutoScrollPosition = new Point((int)timeToPixels(value), -AutoScrollPosition.Y);
			}
		}


		public int VerticalOffset
		{
			get { return -AutoScrollPosition.Y; }
			set
			{
				if (value < 0)
					value = 0;

				// TODO: check negatives here: either they need to both be negative, or neither
				AutoScrollPosition = new Point(-AutoScrollPosition.X, value);
			}
		}

		public List<TimelineRow> Rows
		{
			get { return m_rows; }
			set { m_rows = value; }
		}

		public List<TimelineElement> SelectedElements
		{
			get
			{
				List<TimelineElement> result = new List<TimelineElement>();
				foreach (TimelineRow row in Rows) {
					result.AddRange(row.SelectedElements);
				}
				return result;
			}
		}

		public TimeSpan CursorPosition
		{
			get { return m_cursorPosition; }
			set { m_cursorPosition = value; _CursorMoved(value); Invalidate(); }
		}

		private bool CtrlPressed { get { return Form.ModifierKeys.HasFlag(Keys.Control); } }

		public SortedDictionary<TimeSpan, int> SnapPoints { get; set; }
		public TimeSpan GridlineInterval { get; set; }
		public bool OnlySnapToCurrentRow { get; set; }
		public int DragThreshold { get; set; }
		public Rectangle SelectedArea { get; set; }
	
		// drawing colours, information, etc.
		public Color RowSeparatorColor { get; set; }
		public Color MajorGridlineColor { get; set; }
		public Color SelectionColor { get; set; }
		public Color SelectionBorder { get; set; }
		public Color CursorColor { get; set; }
		public Single CursorWidth { get; set; }


		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementDoubleClicked;
		public event EventHandler<MultiElementEventArgs> ElementsMoved;
		public event EventHandler<TimeSpanEventArgs> CursorMoved;

		private void _ElementDoubleClicked(TimelineElement te) { if (ElementDoubleClicked != null) ElementDoubleClicked(this, new ElementEventArgs(te)); }
		private void _ElementsMoved(MultiElementEventArgs args) { if (ElementsMoved != null) ElementsMoved(this, args); }
		private void _CursorMoved(TimeSpan t) { if (CursorMoved != null) CursorMoved(this, new TimeSpanEventArgs(t)); }

		#endregion


		#region Event Handlers

		protected void ElementChangedHandler(object sender, EventArgs e)
		{
			// when dragging, the control will invalidate after it's done, in case multiple elements are changing.
			if (m_dragState != DragState.Dragging) 
				Invalidate();
		}

		protected void RowChangedHandler(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			if (e.KeyChar == (char)27)  // ESC
            {
				SelectedElements.Clear();
				endDrag();  // do this regardless of if we're dragging or not.
			}
		}

		/// <summary>
		/// Translates a MouseEventArgs so that its coordinates represent the coordinates on the underlying timeline, taking into account scroll position.
		/// </summary>
		/// <param name="e"></param>
		private void _translateMouseArgs(ref MouseEventArgs e)
		{
			// Translate this location based on the auto scroll position.
			Point p = e.Location;
			p.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);

			// Just "fix" it :-)
			e = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			_translateMouseArgs(ref e);

			m_mouseDownElement = elementAt(e.Location);

			switch (e.Button) {
				case MouseButtons.Left:
					OnLeftMouseDown(e);
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_translateMouseArgs(ref e);

			switch (e.Button) {
				case MouseButtons.Left:
					OnLeftMouseUp(e);
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			// don't call the base mouse wheel event; it scrolls the grid left and right
			// if there isn't any vertical movement available (ie. no vertical scrollbar)
			//base.OnMouseWheel(e);
		}

		private void OnLeftMouseDown(MouseEventArgs e)
		{
			// e is already translated.

			// we clicked nothing - clear selection
			if (m_mouseDownElement == null)
            {
				if (!CtrlPressed) {
					ClearSelectedElements();
					m_dragState = DragState.Selecting;
					SelectedArea = new Rectangle(e.X, e.Y, 0, 0);
					m_lastMouseLocation = e.Location;
				}
			}
			// our mouse is down on something
			else
			{
				if (m_mouseDownElement.Selected) {
					// unselect
					if (CtrlPressed)
						m_mouseDownElement.Selected = false;
				} else {
					// select
					if (!CtrlPressed)
						ClearSelectedElements();
					m_mouseDownElement.Selected = true;
				}

				dragWait(e.Location);
			}
			Invalidate();
		}


		private void OnLeftMouseUp(MouseEventArgs e)
		{
			// e is already translated

			if (m_dragState == DragState.Dragging) {
				MultiElementEventArgs evargs = new MultiElementEventArgs { Elements = SelectedElements };
				_ElementsMoved(evargs);

			} else if (m_dragState == DragState.Selecting) {
				// we will only be Selecting if we clicked on the grid background, so on mouse up, check if
				// we didn't move (or very far): if so, move the cursor to the clicked position.
				if (SelectedArea.Width < 2 && SelectedArea.Height < 2) {
					CursorPosition = pixelsToTime(e.X);
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

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (m_dragState == DragState.Normal)
				return;

			_translateMouseArgs(ref e);

			if (m_dragState == DragState.Waiting) {
				if (!m_ignoreDragArea.Contains(e.Location)) {
					//begin the dragging process
					beginDrag(e.Location);
				}
			}
			if (m_dragState == DragState.Dragging) {
				/*
                // Determine if the mouse has moved outside the control.
                int dragOutX = 0;   // How far outside (used for scroll speed)
                if (e.X < 0)
                    dragOutX = e.X;
                else if (e.X > this.Width)
                    dragOutX = e.X - this.Width;

                int dragOutY = 0;   // How far outside (used for scroll speed)
                if (e.Y < 0)
                    dragOutY = e.Y;
                else if (e.Y > this.Height)
                    dragOutY = e.Y - this.Height;

                if (dragOutX != 0 || dragOutY != 0)
                    Debug.WriteLine("dragOutX = {0}   dragOutY = {1}", dragOutX, dragOutY);
                
                
                // Calculate delta to move element
                int dX = e.X - m_oldLoc.X;
                m_oldLoc.X = e.X;
				*/

				int updatedX = e.X;

				// if the cursor position is in a snap location, change the position
				// we update elements to be, to the snapped position
				if (m_snapPixels.ContainsKey(e.X)) {
					updatedX = m_snapPixels[e.X].DestinationPixel;
				}

				int dX = updatedX - m_lastMouseLocation.X;
				m_lastMouseLocation.X = updatedX;

				int dY = e.Y - m_lastMouseLocation.Y;
				m_lastMouseLocation.Y = e.Y;

				if (dX != 0) {
					foreach (TimelineElement element in SelectedElements) {
						if (m_snapPixels.ContainsKey(e.X) && m_snapPixels[e.X].SnapElements.ContainsKey(element)) {
							element.StartTime = m_snapPixels[e.X].SnapElements[element];
						} else {
							element.StartTime += pixelsToTime(dX);
						}
					}
					Invalidate();
				}
			}
			if (m_dragState == DragState.Selecting) {
				// for the selecting, we're using the "last mouse location" variable to record the original click point.
				Point topLeft = new Point(Math.Min(m_lastMouseLocation.X, e.X), Math.Min(m_lastMouseLocation.Y, e.Y));
				Point bottomRight = new Point(Math.Max(m_lastMouseLocation.X, e.X), Math.Max(m_lastMouseLocation.Y, e.Y));

				SelectedArea = Util.RectangleFromPoints(topLeft, bottomRight);
				selectElementsWithin(SelectedArea);
				Invalidate();
			}
		}

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
			foreach (var row in Rows)
			{
				if (
					!row.Visible	||					// row is invisible
					endFound		||					// we already passed the end row
					(!startFound && (row != startRow))	//we haven't found the first row, and this isn't it
					)
				{
					row.Elements.ForEach(elem => elem.Selected = false);
					continue;
				}

				 
				// If we already found the first row, or we haven't, but this is it
				if (startFound || row == startRow)
				{
					startFound = true;

					// This row is in our selection
					foreach (var elem in row.Elements)
					{
						elem.Selected = (elem.StartTime < selEnd && elem.EndTime > selStart);
					}

					if (row == endRow)
					{
						endFound = true;
						continue;
					}
				}

			} // end foreach
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			_translateMouseArgs(ref e);

			TimelineElement elem = elementAt(e.Location);

			if (elem != null) {
				_ElementDoubleClicked(elem);
			}
		}

		#endregion


		#region Methods - Rows, Elements

		public void ClearSelectedElements()
		{
			foreach (TimelineElement te in SelectedElements)
				te.Selected = false;
		}

		/// <summary>
		/// Returns the row located at the current point in screen coordinates
		/// </summary>
		/// <param name="p">Screen coordinates</param>
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
		/// Returns the element located at the current point in screen coordinates
		/// </summary>
		/// <param name="p">Screen coordinates</param>
		/// <returns>Element at given point, or null if none exists.</returns>
		protected TimelineElement elementAt(Point p)
		{
			// First figure out which row we are in
			TimelineRow containingRow = rowAt(p);

			if (containingRow == null)
				return null;

			// Now figure out which element we are on
			foreach (TimelineElement elem in containingRow.Elements) {
				Single elemX = timeToPixels(elem.StartTime);
				Single elemW = timeToPixels(elem.Duration);
				if (p.X >= elemX && p.X <= elemX + elemW)
					return elem;
			}

			return null;
		}

		public List<TimelineElement> ElementsAtTime(TimeSpan time)
		{
			List<TimelineElement> result = new List<TimelineElement>();
			foreach (TimelineRow row in Rows) {
				foreach (TimelineElement elem in row.Elements) {
					if ((time >= elem.StartTime) && (time < (elem.StartTime + elem.Duration)))
						result.Add(elem);
				}
			}

			return result;
		}



		public void AlignSelectedElementsLeft()
		{
			// Find the leftmost element in each row.
			var leftmostEachRow = new Dictionary<TimelineRow,TimelineElement>();
			TimelineElement leftmost = null;
			foreach (var row in Rows)
			{
				TimelineElement leftMostThisRow = null;
				foreach (var elem in row.Elements)
				{
					if (!elem.Selected)
						continue;

					// Record leftmost element for this row.
					if (leftMostThisRow == null || (elem.StartTime < leftMostThisRow.StartTime))
						leftMostThisRow	= elem;

					// Record altogether leftmost element.
					if (leftmost == null || (elem.StartTime < leftmost.StartTime))
						leftmost = elem;
				}
				leftmostEachRow.Add(row, leftMostThisRow);
			}


			// Now adjust all elements in each row, such that the leftmost element (in this row)
			// is at the same start time as the alltogether leftmost element.
			foreach (var row in Rows)
			{
				TimeSpan thisRowAdjust = TimeSpan.Zero;

				TimelineElement leftmostThisRow = leftmostEachRow[row];
				if (leftmostThisRow != null)
					thisRowAdjust = leftmostThisRow.StartTime - leftmost.StartTime;

				foreach (var elem in row.Elements)
				{
					if (!elem.Selected)
						continue;

					elem.StartTime -= thisRowAdjust;
				}
			}

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
			this.Cursor = Cursors.Hand;

			// calculate all the snap points (in pixels) for all selected elements
			// for every visible drag point (and a width either side, so they can snap
			// to non-visible points that are close)
			m_snapPixels = new SortedDictionary<int, SnapDetails>();

			foreach (KeyValuePair<TimeSpan, int> kvp in SnapPoints) {
				if ((kvp.Key >= VisibleTimeStart - VisibleTimeSpan) &&
					(kvp.Key <= VisibleTimeEnd + VisibleTimeSpan)) {

					int snapTimePixelCentre = (int)timeToPixels(kvp.Key);
					int snapRange = kvp.Value;
					int snapLevel = kvp.Value;

					List<TimelineElement> snapElements;

					if (OnlySnapToCurrentRow) {
						TimelineRow row = rowAt(location);
						if (row != null)
							snapElements = row.SelectedElements;
						else
							snapElements = new List<TimelineElement>();
					} else {
						snapElements = SelectedElements;
					}

					foreach (TimelineElement element in snapElements) {
						int elementPixelStart = (int)timeToPixels(element.StartTime);
						int elementPixelEnd = (int)timeToPixels(element.StartTime + element.Duration);

						// iterate through all pixels for this particular snap point, for this element
						for (int offset = -snapRange; offset <= snapRange; offset++) {

							// calculate the relative pixel (to the mouse location) for this point
							int rp = location.X + snapTimePixelCentre + offset - elementPixelStart;

							bool addNewSnapDetail = false;

							// if it doesn't have a Snap entry for this item, make one
							if (!m_snapPixels.ContainsKey(rp)) {
								addNewSnapDetail = true;
							} else {
								// if it does, we have to figure out an intelligent way to combine them. If it's
								// going to snap to the same pixel, then just add it to the list. Also update
								// the priority if needed.
								if (m_snapPixels[rp].DestinationPixel == rp - offset) {
									m_snapPixels[rp].SnapElements[element] = kvp.Key;
									m_snapPixels[rp].SnapLevel = Math.Max(m_snapPixels[rp].SnapLevel, snapLevel);
								}
									// if it's not going to snap to the same pixel as the existing one, then only
									// update it if the new one's of a higher priority.
								else {
									if (m_snapPixels[rp].SnapLevel < snapLevel) {
										addNewSnapDetail = true;
									}
								}
							}

							// add the new one if needed
							if (addNewSnapDetail) {
								SnapDetails sd = new SnapDetails();
								sd.DestinationPixel = rp - offset;
								sd.SnapLevel = snapLevel;
								sd.SnapElements = new Dictionary<TimelineElement, TimeSpan>();
								sd.SnapElements[element] = kvp.Key;
								m_snapPixels[rp] = sd;
							}


							// do the same for the end of the element
							addNewSnapDetail = false;
							rp = location.X + snapTimePixelCentre + offset - elementPixelEnd;




							// if it doesn't have a Snap entry for this item, make one
							if (!m_snapPixels.ContainsKey(rp)) {
								addNewSnapDetail = true;
							} else {
								// if it does, we have to figure out an intelligent way to combine them. If it's
								// going to snap to the same pixel, then just add it to the list. Also update
								// the priority if needed.
								if (m_snapPixels[rp].DestinationPixel == rp - offset) {
									m_snapPixels[rp].SnapElements[element] = kvp.Key - element.Duration;
									m_snapPixels[rp].SnapLevel = Math.Max(m_snapPixels[rp].SnapLevel, snapLevel);
								}
									// if it's not going to snap to the same pixel as the existing one, then only
									// update it if the new one's of a higher priority.
								else {
									if (m_snapPixels[rp].SnapLevel < snapLevel) {
										addNewSnapDetail = true;
									}
								}
							}

							// add the new one if needed
							if (addNewSnapDetail) {
								SnapDetails sd = new SnapDetails();
								sd.DestinationPixel = rp - offset;
								sd.SnapLevel = snapLevel;
								sd.SnapElements = new Dictionary<TimelineElement, TimeSpan>();
								sd.SnapElements[element] = kvp.Key - element.Duration;
								m_snapPixels[rp] = sd;
							}
						}
					}
				}
			}
		}

		private void endDrag()
		{
			m_dragState = DragState.Normal;
			this.Cursor = Cursors.Default;
		}

		#endregion


		#region Drawing

		private int _drawRows(Graphics g)
		{
			// Draw row separators
			int curY = 0;

			using (Pen p = new Pen(RowSeparatorColor))
			{
				foreach (TimelineRow row in Rows)
				{
					if (!row.Visible)
						continue;

					curY += row.Height;
					Point left = new Point((-AutoScrollPosition.X), curY);
					Point right = new Point((-AutoScrollPosition.X) + Width, curY);
					g.DrawLine(p, left, right);
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
				foreach (KeyValuePair<TimeSpan, int> kvp in SnapPoints)
				{
					if (kvp.Key >= VisibleTimeStart && kvp.Key < VisibleTimeEnd)
					{
						Single x = timeToPixels(kvp.Key);
						p.DashPattern = new float[] { kvp.Value, kvp.Value };
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

				// Draw each element
				foreach (var element in row.Elements) {

					Point location = new Point((int)timeToPixels(element.StartTime), top);
					Size size = new Size((int)timeToPixels(element.Duration), row.Height);

                    // The rectangle where this element will be drawn
                    Rectangle dstRect = new Rectangle(location, size);

                    // The rectangle this element will draw itself in
                    Rectangle srcRect = new Rectangle(new Point(0, 0), size);

                    // Perform the transformation and save the state.
                    GraphicsContainer containerState = g.BeginContainer(dstRect, srcRect, GraphicsUnit.Pixel);

                    // Prevent the element from drawing outside its bounds
                    g.Clip = new System.Drawing.Region(srcRect);
                    
                    element.Draw(g, srcRect);

                    g.EndContainer(containerState);
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
			try {
				e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

				_drawGridlines(e.Graphics);
				int totalHeight = _drawRows(e.Graphics);
				AutoScrollMinSize = new Size((int)timeToPixels(TotalTime), totalHeight);
				_drawSnapPoints(e.Graphics);
				_drawElements(e.Graphics);
				_drawSelection(e.Graphics);
				_drawCursor(e.Graphics);

			} catch (Exception ex) {
				MessageBox.Show("Unhandled Exception while drawing TimelineGrid:\n\n" + ex.Message);
				throw;
			}
		}

		#endregion
    }

	class SnapDetails
	{
		public int DestinationPixel;
		public int SnapLevel;
		public Dictionary<TimelineElement, TimeSpan> SnapElements;
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
