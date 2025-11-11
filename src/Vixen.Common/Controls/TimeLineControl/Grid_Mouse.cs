using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using System.ComponentModel;
using Vixen.Sys.LayerMixing;
using Timer = System.Windows.Forms.Timer;

namespace Common.Controls.Timeline
{
	public partial class Grid
	{
		List<Element> capturedElements = new List<Element>();
		List<Element> adjoiningElements = null;
		public bool EnableDrawMode = false;
		public Guid SelectedEffect { get; set; }
		public bool _beginEffectDraw;
		private TimeSpan effectDrawMouseDownTime;
		private TimeSpan effectDrawMouseUpTime;
		private Point mouseDownGridLocation, mouseUpGridLocation;

		#region Public Properties 
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan MinimumElementDuration
		{
			get { return PixelsToTime(Common.Controls.Timeline.Grid.MinElemWidthPx); }
		}
		#endregion

		#region General Mouse Event-Related

		public event EventHandler<DrawElementEventArgs> StartDrawMode;

		/// <summary>
		/// Translates a location (Point) so that its coordinates represent the coordinates on the underlying timeline, taking into account scroll position.
		/// </summary>
		/// <param name="originalLocation"></param>
		public Point TranslateLocation(Point originalLocation)
		{
			// Translate this location based on the auto scroll position.
			Point p = originalLocation;
			p.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
			return p;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			Point gridLocation = mouseDownGridLocation = TranslateLocation(e.Location);

			m_lastGridLocation = gridLocation; //new
			m_mouseDownElementRow = rowAt(gridLocation);

			m_mouseDownElements = elementsAt(gridLocation);
			if (m_mouseDownElements == null)
				m_mouseDownElements = new List<Element>();

			if (e.Button == MouseButtons.Middle && SelectedEffect != Guid.Empty)
			{
				if (m_mouseDownElementRow == null)
					return;

				_beginEffectDraw = true;
				this.Cursor = Cursors.Cross;
				effectDrawMouseDownTime = PixelsToTime(gridLocation.X);
				beginDrawBox(gridLocation);
				m_lastSingleSelectedElementLocation = Point.Empty;				
			}
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				_workingElement = elementAt(gridLocation);
				if ((EnableDrawMode && !AltPressed) && SelectedEffect != Guid.Empty && m_mouseResizeZone == ResizeZone.None)
				{
					if (m_mouseDownElementRow == null)
						return;

					_beginEffectDraw = true;
					effectDrawMouseDownTime = PixelsToTime(gridLocation.X);
					beginDrawBox(gridLocation);
					m_lastSingleSelectedElementLocation = Point.Empty;
					return;
				}
				if (m_mouseDownElements.Count <= 0 && e.Button != MouseButtons.Right)
				{
					// we clicked on the background - clear anything that is selected, and begin a 'selection' drag.
					if (!CtrlPressed)
					{
						beginDragSelect(gridLocation);
						m_lastSingleSelectedElementLocation = Point.Empty;
					}
				}
				else {
					// our mouse is down on something -- if it's something selected, unselect it with CTRL
					if (m_mouseDownElements.Any(x => x.Selected)) {
						// unselect
						if (CtrlPressed)
						{
							foreach (Element elem in m_mouseDownElements.Where(x => x.Selected))
								elem.Selected = false;
							_SelectionChanged();
						}
						else
						{
							if (ShiftPressed)
							{

								if (m_lastSingleSelectedElementLocation != Point.Empty)
								{
									SelectElementsBetween(m_lastSingleSelectedElementLocation, gridLocation);
								}
							}
							//else
							//{
							//	//Unselect everything and make this the new selection
								
							//	SupressSelectionEvents = true;
							//	ClearSelectedElements();
							//	ClearSelectedRows();
							//	ClearActiveRows();
							//	SupressSelectionEvents = false;
								
							//	if (_ElementsSelected(m_mouseDownElements))
							//	{
							//		foreach (Element element in m_mouseDownElements)
							//		{
							//			element.Selected = true;
							//		}
							//		m_lastSingleSelectedElementLocation = gridLocation;
									
							//	}

							//	_SelectionChanged();
							//}
						}
					}
					else {
						// there is one-or-more elements under the mouse, and none of them are selected. 
						if (ShiftPressed && m_lastSingleSelectedElementLocation != Point.Empty)
						{
							SelectElementsBetween(m_lastSingleSelectedElementLocation, gridLocation);	
						}
						else
						{
							//If CTRL isn't pressed
							// (ie. we aren't going to ADD to the selection, but replace it) clear the current selection, then pass
							// it to the ElementsSelected event handler to handle selection if an external party wants to handle selection.
							if (!CtrlPressed)
							{
								SupressSelectionEvents = true;
								ClearSelectedElements();
								ClearSelectedRows();
								ClearActiveRows();
								SupressSelectionEvents = false;
							}
							if (_ElementsSelected(m_mouseDownElements))
							{
								foreach (Element element in m_mouseDownElements)
								{
									element.Selected = true;
								}
								m_lastSingleSelectedElementLocation = gridLocation;
								Row row = rowAt(gridLocation);
								if (row != null) row.Active = true;
								_SelectionChanged();
							}
							else if (!CtrlPressed)
							{
								_SelectionChanged();
							}
						}
						
					}

					if (e.Button == MouseButtons.Left)
					{
						if (m_mouseResizeZone == ResizeZone.None)
						{
							//waitForDragMove(e.Location);	// begin waiting for a normal drag
							waitForDragMove(gridLocation);
						}
						else if (!CtrlPressed)
						{
							if (AltPressed && SelectedElements.Count() > 1)
							{
								MessageBoxForm.msgIcon = SystemIcons.Warning;
								var messageBox = new MessageBoxForm("Too many effects selected.\nMaximum selected effects for this action is 1",
									"Warning", false, false);
								messageBox.ShowDialog();
								return;
							}
							beginHResize(gridLocation); // begin a resize.
						}
					}
				}
				//Invalidate();
			}
		}


		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_timelineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(false, null));

			Point gridLocation = mouseUpGridLocation = TranslateLocation(e.Location);

			if (e.Button == MouseButtons.Middle && _beginEffectDraw)
			{
				//Just doing this for looks and unifority for now
				//until a decision is made on how to switch from draw to selection mode
				switch (m_dragState) { 
					case DragState.Drawing:
						_beginEffectDraw = false;
						this.Cursor = Cursors.Default;
						effectDrawMouseUpTime = PixelsToTime(gridLocation.X);
						StartDrawMode(this, new DrawElementEventArgs(SelectedEffect, GetRowsWithin(DrawingArea), effectDrawMouseDownTime, effectDrawMouseUpTime));
						MouseUp_DrawSelect(gridLocation);
						break;
					default:
						endAllDrag();
						break; //<This is really dumb
				}
			}
			if (e.Button == MouseButtons.Left) {
				_workingElement = null;
				switch (m_dragState) {
					case DragState.Moving:
						MouseUp_DragMoving(gridLocation);
						break;

					case DragState.Drawing:
						_beginEffectDraw = false;
						effectDrawMouseUpTime = PixelsToTime(gridLocation.X);
						StartDrawMode(this, new DrawElementEventArgs(SelectedEffect, GetRowsWithin(DrawingArea), effectDrawMouseDownTime, effectDrawMouseUpTime));
						MouseUp_DrawSelect(gridLocation);
						break;

					case DragState.Selecting:
						MouseUp_DragSelect(gridLocation);
						break;

					case DragState.HResizing:
						MouseUp_HResizing(gridLocation);
						break;

					default:
						endAllDrag();
						// If we're not dragging on mouse up, it could be a click on one of multiple
						// selected elements. (In which case we select only that one)
						if (m_mouseDownElements != null && m_mouseDownElements.Any() && !CtrlPressed && !ShiftPressed)
						{
							ClearSelectedElements();
							if (_ElementsSelected(m_mouseDownElements))
							{
								m_mouseDownElements.First().Selected = true;
								m_lastSingleSelectedElementLocation = gridLocation;
								_SelectionChanged();
								Row row = rowAt(gridLocation);
								if (row != null) row.Active = true;
							}
						}
						if (m_mouseDownElements != null && m_mouseDownElements.Any() && ShiftPressed ||
							m_mouseDownElements != null && m_mouseDownElements.Any() && CtrlPressed)
						{
							ClearActiveRows();
							Row row = rowAt(gridLocation);
							if (row != null) row.Active = true;
						}
						break;
				}
			} else if(e.Button == MouseButtons.Right)
			{
				ClearActiveRows();
				Row row = rowAt(gridLocation);
				if (row == null)
					return;
				row.Active = true;
				if (ClickingGridSetsCursor)
					CursorPosition = PixelsToTime(gridLocation.X);
				_ContextSelected(m_mouseDownElements, PixelsToTime(gridLocation.X), row);
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
			//Debug.WriteLine("Grid OnMouseHWheel: delta={0}", args.Delta);

			double scale;
			if (args.Delta > 0)
				scale = 0.10;
			else
				scale = -0.10;
			VisibleTimeStart += VisibleTimeSpan.Scale(scale);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			Point gridLocation = TranslateLocation(e.Location);
			Element elem = elementAt(gridLocation);

			if (elem != null) {
				_ElementDoubleClicked(elem);
			}
		}

		#endregion

		#region Mouse Move

		///<summary>The last location on the grid the mouse was at.</summary>
		private Point m_lastGridLocation;

		protected void ShowToolTip(Point location) 
		{
			Point gridLocation = TranslateLocation(location);
			List<Element> elements = elementsAt(gridLocation);
			//Determine if we have new elements captured.
			if (capturedElements.Except(elements).Any() || elements.Except(capturedElements).Any())
			{
				capturedElements.ForEach(x => x.MouseCaptured = false);
				capturedElements = elements;
				capturedElements.ForEach(x => x.MouseCaptured = true);
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			ShowToolTip(e.Location);

			// Determine if we need to start auto-drag
			switch (m_dragState) {
				case DragState.Moving:
				case DragState.Selecting:
				case DragState.Drawing:
				case DragState.HResizing:

					m_mouseOutside.X = (e.X <= AutoScrollMargin.Width)
					                   	? -(AutoScrollMargin.Width - e.X)
					                   	: (e.X > ClientSize.Width - AutoScrollMargin.Width)
					                   	  	? e.X - ClientSize.Width + AutoScrollMargin.Width
					                   	  	: 0;

					m_mouseOutside.Y = (e.Y <= AutoScrollMargin.Height)
					                   	? -(AutoScrollMargin.Height - e.Y)
					                   	: (e.Y > ClientSize.Height - AutoScrollMargin.Height)
					                   	  	? e.Y - ClientSize.Height + AutoScrollMargin.Height
					                   	  	: 0;

					if (m_mouseOutside.X != 0 || m_mouseOutside.Y != 0) {
						if (!m_autoScrollTimer.Enabled)
							m_autoScrollTimer.Start(); // Mouse is outside viewport - start auto scroll timer.
					}
					else {
						if (m_autoScrollTimer.Enabled)
							m_autoScrollTimer.Stop(); // Mouse is inside viewport - stop auto scroll timer.
					}

					break;
			}

			m_lastMouseMove = e;
			HandleMouseMove(e);
		}

		///<summary>Calls a different MouseMove_ function depending on the current drag state.</summary>
		private void HandleMouseMove(MouseEventArgs e)
		{
			Point gridLocation = TranslateLocation(e.Location);
			if (m_mouseDownElements != null)
			{
				if (ModifierKeys == Keys.Shift && m_mouseDownElements.Any())
					gridLocation.X = m_lastGridLocation.X;

				if (ModifierKeys == (Keys.Alt | Keys.Control) && m_mouseDownElements.Any())
					gridLocation.X = m_lastGridLocation.X;

				if (ModifierKeys == (Keys.Shift | Keys.Alt) && m_mouseDownElements.Any())
					gridLocation.Y = m_lastGridLocation.Y;	
			}
			
			Point delta = new Point(
				gridLocation.X - m_lastGridLocation.X,
				gridLocation.Y - m_lastGridLocation.Y
				);

			m_lastGridLocation = gridLocation;

			//Debug.WriteLine("Grid Location: {0},{1}    Delta: {2},{3}", gridLocation.X, gridLocation.Y, delta.X, delta.Y);

			switch (m_dragState) {
				case DragState.Normal: // Normal -> Not dragging; mouse is up
					MouseMove_Normal(gridLocation);
					break;

				case DragState.Waiting: // Waiting to start moving an object
					if (!m_ignoreDragArea.Contains(gridLocation))
					{
						if (CtrlPressed)
						{
							//Ensure the element under our pointer is selected.
							List<Element> elements = elementsAt(m_waitingBeginGridLocation);
							if (elements.Any())
							{
								foreach (Element elem in elements.Where(x => x.Selected == false))
								{
									elem.Selected = true;
								}
								_SelectionChanged();
							}

							CloneSelectedElementsForMove();
						}
						beginDragMove(gridLocation);
					}
					break;

				case DragState.Moving: // Moving objects
					//Gets the element we are working with 
					if (ResizeIndicator_Enabled && elementsAt(gridLocation).Any())
						foreach (Element elem in elementsAt(gridLocation))
						{
							if (elem.Selected) _workingElement = elem;
						}
					MouseMove_DragMoving(gridLocation);
					break;

				case DragState.Selecting: // Dragging a selection rectangle
					MouseMove_DragSelect(gridLocation);
					break;

				case DragState.Drawing: // Dragging a drawing rectangle
					MouseMove_DrawSelect(gridLocation);
					break;

				case DragState.HResizing: // Resizing an element
					MouseMove_HResizing(gridLocation, delta);
					break;

				default:
					throw new Exception("Unknown DragState.");
			}

			base.OnMouseMove(e);
		}

		#endregion

		#region [Mouse Drag] Normal (not dragging)

		private const int ElementResizeThreshold = 12; // number of pixels to edge where mouse must be for risize.
		private ResizeZone m_mouseResizeZone = ResizeZone.None;


		//private void MouseMove_DragNormal(MouseEventArgs e)
		private void MouseMove_Normal(Point gridLocation)
		{
			//Point gridLocation = TranslateLocation(e.Location);

			// Are we in a 'resize zone' at the front or back of an element?
			Element element = elementAt(gridLocation);
			if (element == null) {
				m_mouseResizeZone = ResizeZone.None;
			}
			else {
				// smaller of constant, or half of element width
				int grabThreshold = Math.Min(ElementResizeThreshold, (int) (timeToPixels(element.Duration)/2));
				float elemStart = timeToPixels(element.StartTime);
				float elemEnd = timeToPixels(element.EndTime);
				int x = gridLocation.X;

				if ((x >= elemStart) && (x < (elemStart + grabThreshold)))
					m_mouseResizeZone = ResizeZone.Front;
				else if ((x <= elemEnd) && (x > (elemEnd - grabThreshold)))
					m_mouseResizeZone = ResizeZone.Back;
				else
					m_mouseResizeZone = ResizeZone.None;
			}

			Cursor = (m_mouseResizeZone == ResizeZone.None) ? (((EnableDrawMode && !AltPressed) && SelectedEffect != Guid.Empty) ? Cursors.Cross : Cursors.Default) : Cursors.SizeWE;
		}

		#endregion

		#region [Mouse Drag] (Move/Resize common)

		///<summary>Called when any operation that moves element times (namely drag-move and hresize).
		///Saves the pre-move information and begins update on all selected elements.</summary>
		private void elementsBeginMove(Point gridLocation)
		{
			m_elemMoveInfo = new ElementMoveInfo(gridLocation, SelectedElements, VisibleTimeStart);

			foreach (var elem in SelectedElements)
				elem.BeginUpdate();
		}

		/// <summary>
		/// Saves the pre-move information and begins update on all selected elements.<br/>
		/// Called when any operation moves element times.
		/// </summary>
		/// <param name="elements">List of Elements that are moving</param>
		/// <exception cref="m_elemMoveInfo">Thrown if elementsBeginMove not called prior</exception>
		private void appendElementsBeginMove(List<Element> elements)
		{
			if (m_elemMoveInfo == null)
				throw new Exception("Calling appendMove prior to a beginMove.");

			if (elements is null)
				return;


			foreach (var elem in elements)
			{
				if (!elem.InAdjoiningChange())
				{
					m_elemMoveInfo.AppendElementMoveInfo(elem);
					elem.BeginUpdate();
				}
			}
			return;
		}

		private void elementsFinishedMoving(ElementMoveType type)
		{
			foreach (var elem in SelectedElements) {
				elem.EndUpdate();
				RenderElement(elem);
			}

			// If there were any adjoining elements, then finish processing those
			if (adjoiningElements is not null)
			{
				foreach (var elem in adjoiningElements)
				{
					elem.EndUpdate();
					RenderElement(elem);
				}
				adjoiningElements = null;
			}

			MultiElementEventArgs evargs = new MultiElementEventArgs {Elements = SelectedElements};
			_ElementsFinishedMoving(evargs);

			if (ElementsMovedNew != null) {
				ElementsMovedNew(this, new ElementsChangedTimesEventArgs(m_elemMoveInfo, type));
			}

			m_elemMoveInfo = null;
		}


		private void elementsCancelMove()
		{
			if (m_elemMoveInfo == null)
				return;

			VisibleTimeStart = m_elemMoveInfo.VisibleTimeStart;

			foreach (var kvp in m_elemMoveInfo.OriginalElements) {
				// Restore each element's times.
				kvp.Key.StartTime = kvp.Value.StartTime;
				kvp.Key.Duration = kvp.Value.Duration;
			}

			endAllDrag();
		}

		#endregion

		#region [Mouse Drag] Drawing Box
		
		private void beginDrawBox(Point gridLocation)
		{
			m_dragState = DragState.Drawing;
			ClearSelectedElements();
			ClearSelectedRows(m_mouseDownElementRow);
			ClearActiveRows(m_mouseDownElementRow);
			DrawingArea = new Rectangle(gridLocation.X, gridLocation.Y, 0, 0);
			m_drawingRectangleStart = gridLocation;
		}

		private void MouseMove_DrawSelect(Point gridLocation)
		{
			Point topLeft = new Point(Math.Min(m_drawingRectangleStart.X, gridLocation.X),
									  Math.Min(m_drawingRectangleStart.Y, gridLocation.Y));
			Point bottomRight = new Point(Math.Max(m_drawingRectangleStart.X, gridLocation.X),
										  Math.Max(m_drawingRectangleStart.Y, gridLocation.Y));

			//Modifies area to include full row in drawing area
			var TempDrawArea = Util.RectangleFromPoints(topLeft, bottomRight);
			var RowMembers = GetRowsWithin(TempDrawArea);
			var FirstRow = (Row)RowMembers.First();
			var LastRow = (Row)RowMembers.Last();
			topLeft = new Point(Math.Min(m_drawingRectangleStart.X, gridLocation.X),
										Math.Min(m_drawingRectangleStart.Y, (int)FirstRow.DisplayTop) + 1);
			bottomRight = new Point(Math.Max(m_drawingRectangleStart.X, gridLocation.X),
											Math.Max(m_drawingRectangleStart.Y, ((int)LastRow.DisplayTop + (int)LastRow.Height)) - 2);

			DrawingArea = Util.RectangleFromPoints(topLeft, bottomRight);
			Invalidate();
		}

		private void MouseUp_DrawSelect(Point gridLocation)
		{
			// we will be Drawing anywhere
			// if we didn't move (or very far): if so, consider it just a background click.
			if (DrawingArea.Width < 2 && DrawingArea.Height < 2)
			{
				OnBackgroundClick(new TimelineEventArgs(rowAt(gridLocation), PixelsToTime(gridLocation.X)));
			}

			m_drawingRectangleStart = Point.Empty;
			DrawingArea = Rectangle.Empty;
			endAllDrag();
		}
		#endregion

		#region [Mouse Drag] Selection Box

		private void beginDragSelect(Point gridLocation)
		{
			m_dragState = DragState.Selecting;
			if (!ShiftPressed && SelectedElements.Any()) ClearSelectedElements();
			else tempSelectedElements = SelectedElements.ToList();
			ClearSelectedRows(m_mouseDownElementRow);
			ClearActiveRows(m_mouseDownElementRow);
			SelectionArea = new Rectangle(gridLocation.X, gridLocation.Y, 0, 0);
			m_selectionRectangleStart = gridLocation;
		}


		//private void MouseMove_DragSelecting(MouseEventArgs e)
		private void MouseMove_DragSelect(Point gridLocation)
		{
			Point topLeft = new Point(Math.Min(m_selectionRectangleStart.X, gridLocation.X),
			                          Math.Min(m_selectionRectangleStart.Y, gridLocation.Y));
			Point bottomRight = new Point(Math.Max(m_selectionRectangleStart.X, gridLocation.X),
			                              Math.Max(m_selectionRectangleStart.Y, gridLocation.Y));

			SelectionArea = Util.RectangleFromPoints(topLeft, bottomRight);
			selectElementsWithin(SelectionArea, true);
			Invalidate();
			Update();
		}

		private void MouseUp_DragSelect(Point gridLocation)
		{
			// we will only be Selecting if we clicked on the grid background, so on mouse up, check if
			// we didn't move (or very far): if so, consider it just a background click.
			if (SelectionArea.Width < 2 && SelectionArea.Height < 2) {
				OnBackgroundClick(new TimelineEventArgs(rowAt(gridLocation), PixelsToTime(gridLocation.X)));
			}

			// done with the selection rectangle.
			m_selectionRectangleStart = Point.Empty;
			SelectionArea = Rectangle.Empty;

			// done with temp selection list
			tempSelectedElements.Clear();

			endAllDrag();
		}

		#endregion

		protected void OnBackgroundClick(TimelineEventArgs e)
		{
			if (e.Row != null)
				e.Row.Active = true;

			if (ClickingGridSetsCursor)
				CursorPosition = e.Time;

			if (BackgroundClicked != null)
				BackgroundClicked(this, e);
		}

		#region [Mouse Drag] Moving Elements

		private void waitForDragMove(Point gridLocation)
		{
			// begin the dragging process -- calculate a area outside which a drag (move) starts
			m_dragState = DragState.Waiting;
			m_waitingBeginGridLocation = gridLocation;
			m_ignoreDragArea = new Rectangle(gridLocation.X - DragThreshold, gridLocation.Y - DragThreshold, DragThreshold*2,
			                                 DragThreshold*2);
		}

		private void beginDragMove(Point gridLocation)
		{
			m_dragState = DragState.Moving;
			m_ignoreDragArea = Rectangle.Empty;
			Cursor = Cursors.SizeAll;
			CurrentRowIndexUnderMouse = Rows.IndexOf(rowAt(gridLocation));
			calculateSnapPoints();

			elementsBeginMove(gridLocation);
		}

		private void calculateSnapPoints()
		{
			if (!EnableSnapTo) return;
			// build up a full set of snap points/details, from (a) the static snap points for the grid, and
			// (b) calculated snap points for this move (ie. other elements in the row[s]).
			
			// iterate through the rows, calculating snap points for every single element in each row that has any selected elements
			foreach (Row row in Rows.Where(x => x.Visible)) {
				// This would skip generating snap points for elements on any rows that have nothing selected.
				// However, we still need to do that; as we might be dragging elements vertically into rows that
				// (currently) have nothing selected. So we'll generate for everything, but that's going to be
				// quite overkill. So, the big TODO: here is to regenerate element snap points for only rows with
				// selected elements, but also regenerate them whenever we move vertically.
				
				foreach (Element element in row) {
					// skip it if it's a selected element; we don't want to snap to them, as they'll be moving as well
					if (element.Selected)
						continue;

					// if it's a non-selected element, generate snap points for it; for the start and end times. Also record the
					// row its from in the generated point, so when snapping we can check against only elements from this row.
					SnapDetails details = CalculateSnapDetailsForPoint(element.StartTime, SnapPriorityForElements, Color.Empty, false, false);
					details.SnapRow = row;

					if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime)) {
						CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
					}
					CurrentDragSnapPoints[details.SnapTime].Add(details);

					details = CalculateSnapDetailsForPoint(element.EndTime, SnapPriorityForElements, Color.Empty, false, false);
					details.SnapRow = row;

					if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime)) {
						CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
					}
					CurrentDragSnapPoints[details.SnapTime].Add(details);
				}
			}

			//Add in our static snap points as a copy so they are not modified
			foreach (var staticSnapPoint in StaticSnapPoints)
			{
				if (CurrentDragSnapPoints.ContainsKey(staticSnapPoint.Key))
				{
					CurrentDragSnapPoints[staticSnapPoint.Key].AddRange(staticSnapPoint.Value);
				} else
				{
					CurrentDragSnapPoints.Add(staticSnapPoint.Key, staticSnapPoint.Value);
				}
			}
		}


		//TODO: This has not been revisited yet.
		/*
        private void MouseMove_DragMoving_OLD(MouseEventArgs e)
        {
            Point gridLocation = TranslateLocation(e.Location);

            // if we don't have anything selected, there's no point dragging anything...
            if (SelectedElements.Count() == 0)
                return;

            Point d = new Point(e.X - m_lastMouseLocation.X, e.Y - m_lastMouseLocation.Y);
            m_lastMouseLocation = e.Location;

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
            if (d.X < 0)
            {
                if (leftBoundary <= 0)
                    m_dragAutoscrollDistance.Width += d.X;
                else if (rightBoundary >= ClientSize.Width && m_dragAutoscrollDistance.Width > 0)
                    m_dragAutoscrollDistance.Width = Math.Max(0, m_dragAutoscrollDistance.Width + d.X);
            }

            // if the mouse moved right, do the inverse of the above rules.
            if (d.X > 0)
            {
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

            m_dragAutoscrollDistance.Height = (e.Y < 0) ? e.Y : ((e.Y > ClientSize.Height) ? e.Y - ClientSize.Height : 0);

            // if we're scrolling, start the timer if needed. If not, vice-versa.
            if (m_dragAutoscrollDistance.Width != 0 || m_dragAutoscrollDistance.Height != 0)
            {
                if (!ScrollTimer.Enabled)
                    ScrollTimer.Start();
            }
            else
            {
                if (ScrollTimer.Enabled)
                    ScrollTimer.Stop();
            }

            // only move the elements here if we aren't going to be auto-dragging while scrolling in the timer events.
            if (d.X != 0 && m_dragAutoscrollDistance.Width == 0)
            {
                TimeSpan desiredMoveTime = DragTimeLeftOver + PixelsToTime(d.X);
                TimeSpan realMoveTime = OffsetElementsByTime(SelectedElements, desiredMoveTime);
                DragTimeLeftOver = desiredMoveTime - realMoveTime;
            }

            // if we've moved vertically, we may need to move elements between rows
            if (d.Y != 0 && !ResizingElement)
            {
                MoveElementsVerticallyToLocation(SelectedElements, gridLocation);
            }
        }
        */


		/// <summary>
		/// Handles mouse move events while in the "Moving" state.
		/// </summary>
		/// <param name="gridLocation">Mouse location on the grid.</param>
		private void MouseMove_DragMoving(Point gridLocation)
		{
			// if we don't have anything selected, there's no point dragging anything...
			if (!SelectedElements.Any())
				return;

			TimeSpan dt = PixelsToTime(gridLocation.X - m_elemMoveInfo.InitialGridLocation.X);
			int dy = gridLocation.Y - m_elemMoveInfo.InitialGridLocation.Y;

			// If we didn't move, get outta here.
			if (dt == TimeSpan.Zero && dy == 0)
				return;


			// Calculate what our actual dt value will be.
			TimeSpan earliest = m_elemMoveInfo.OriginalElements.Values.Min(x => x.StartTime);
			if ((earliest + dt) < TimeSpan.Zero)
				dt = -earliest;

			TimeSpan latest = m_elemMoveInfo.OriginalElements.Values.Max(x => x.EndTime);
			if ((latest + dt) > TimeInfo.TotalTime)
				dt = TimeInfo.TotalTime - latest;

			// modify the dt time based on snap points (ie. marks, and borders of other elements)
			dt = FindSnapTimeForElements(SelectedElements, dt, ResizeZone.None);

			foreach (var elem in SelectedElements) {
				// Get this element's original times (before resize started)
				ElementTimeInfo orig = m_elemMoveInfo.OriginalElements[elem];
				elem.StartTime = orig.StartTime + dt;
				//Control when the time changed event happens.
				elem.UpdateNotifyTimeChanged();
			}

			// if we've moved vertically, we may need to move elements between rows
			if (dy != 0)
			{
				SuppressInvalidate = true;
				MoveElementsVerticallyToLocation(SelectedElements, gridLocation);
				SuppressInvalidate = false;
			}

			Invalidate();
		}

		private void MouseUp_DragMoving(Point gridLocation)
		{
			m_lastSingleSelectedElementLocation = gridLocation;
			elementsFinishedMoving(ElementMoveType.Move);
			endAllDrag();
			CurrentDragSnapPoints.Clear();
		}

		#endregion

		#region [Mouse Drag] Horizontal Resize

		private const int MinElemWidthPx = 10;

		private void beginHResize(Point gridLocation)
		{
			m_dragState = DragState.HResizing;
			//Cursor.Clip = new Rectangle(0, Cursor.Position.Y, Screen.FromControl(this).Bounds.Width, 1);
			CurrentRowIndexUnderMouse = Rows.IndexOf(rowAt(gridLocation));
			calculateSnapPoints();
			elementsBeginMove(gridLocation);
			adjoiningElements = new List<Element>();
		}

		/// <summary>
		/// Adds to the process of beginHResize
		/// </summary>
		/// <param name="elements">List of elements that are about to change</param>
		/// <remarks>Must call beginHResize prior to calling this method</remarks>
		private void beginAppendHResize(List<Element> elements)
		{
			appendElementsBeginMove(elements);
		}

		/// <summary>
		/// Resize the starting or ending times of elements
		/// </summary>
		/// <param name="gridLocation">Current mouse position</param>
		/// <param name="delta">Size and direction of mouse movement</param>
		/// <remarks>If the Ctrl key is active, then the adjoining elements are also resized</remarks>
		private void MouseMove_HResizing(Point gridLocation, Point delta)
		{
			int selectedLayer = 0;

			TimeSpan dt = PixelsToTime(gridLocation.X - m_elemMoveInfo.InitialGridLocation.X);

			// Check to see if the time (resizing) moved
			if (dt == TimeSpan.Zero)
				return;

			// Verify that only one primary Element is selected, if we are moving adjoining Elements
			else if (AltPressed && SelectedElements.Count() > 1)
				return;

			// Modify the dt time based on snap points (ie. marks, and borders of other elements)
			dt = FindSnapTimeForElements(SelectedElements, dt, m_mouseResizeZone);

			// Find the shortest duration of all those elements selected
			TimeSpan shortest = TimeSpan.MaxValue;
			foreach (var element in SelectedElements)
			{
				ElementTimeInfo ei = m_elemMoveInfo.GetElementMoveInfo(element);
				if (ei.Duration < shortest)
					shortest = ei.Duration;
				selectedLayer = SequenceLayers.GetLayer(element.EffectNode).LayerLevel;
			}

			// Check boundary conditions
			switch (m_mouseResizeZone) {
				case ResizeZone.Front:
					// Clip earliest element StartTime at zero
					TimeSpan earliest = TimeInfo.TotalTime;
					foreach (var element in SelectedElements)
					{
						TimeSpan checkTime = m_elemMoveInfo.GetElementMoveInfo(element).StartTime;
						if (checkTime < earliest)
							earliest = checkTime;
					}
					if ((earliest + dt) < TimeSpan.Zero)
						dt = -earliest;

					// Ensure the proposed shortest duration meets minimum width (in px)
					if (shortest - dt < PixelsToTime(MinElemWidthPx))
						dt = shortest - PixelsToTime(MinElemWidthPx);

					if (AltPressed && adjoiningElements.Count() == 0)
					{
						// Get the Elements that are preceding the Selected Element
						List<Element> priorElements = SelectedElements.First().Row.GetPriorsOfElement(SelectedElements.First());

						// In those preceding Elements, find what the latest ending time is
						TimeSpan latestTime = TimeSpan.Zero;
						foreach (var element in priorElements)
						{
							if (element.EndTime > latestTime)
							{
								if (ShiftPressed && (SequenceLayers.GetLayer(element.EffectNode).LayerLevel != selectedLayer))
									continue;
								latestTime = element.EndTime;
							}
						}

						// Reiterate through the list of preceding elements, creating a new list of all
						// those Elements that have that latest ending time
						foreach (var element in priorElements)
						{
							if (element.EndTime == latestTime)
							{
								if (ShiftPressed && (SequenceLayers.GetLayer(element.EffectNode).LayerLevel != selectedLayer))
									continue;
								if (!element.InAdjoiningChange())
									adjoiningElements.Add(element);
							}
						}

						// Save the current state information of these Target Elements
						beginAppendHResize(adjoiningElements);
					}
					break;

				case ResizeZone.Back:
					TimeSpan latest = TimeSpan.Zero;
					foreach (var element in SelectedElements)
					{
						TimeSpan checkTime = m_elemMoveInfo.GetElementMoveInfo(element).EndTime;
						if (checkTime > latest)
							latest = checkTime;
					}
					if ((latest + dt) > TimeInfo.TotalTime)
						dt = TimeInfo.TotalTime - latest;

					// Ensure the proposed shortest duration meets minimum width (in px)
					if (shortest + dt < PixelsToTime(MinElemWidthPx))
						dt = -(shortest - PixelsToTime(MinElemWidthPx));

					// If the Alt key is pressed and we have not already created as Adjoining List
					if (AltPressed && adjoiningElements.Count() == 0)
					{
						// Get the Elements that are following the Selected Element
						List<Element> followElements = SelectedElements.First().Row.GetFollowersOfElement(SelectedElements.First());

						// In those following Elements, find what the earliest starting time is
						TimeSpan earliestTime = TimeSpan.MaxValue;
						foreach (var element in followElements)
						{
							if (element.StartTime < earliestTime)
							{
								if (ShiftPressed && (SequenceLayers.GetLayer(element.EffectNode).LayerLevel != selectedLayer))
									continue;
								earliestTime = element.StartTime;
							}
						}

						// Reiterate through the list of following elements, creating a new list of all
						// those Elements that have that earliest starting time
						adjoiningElements = new List<Element>();
						foreach (var element in followElements)
						{
							if (element.StartTime == earliestTime)
							{
								if (ShiftPressed && (SequenceLayers.GetLayer(element.EffectNode).LayerLevel != selectedLayer))
									continue;
								if (!element.InAdjoiningChange())
									adjoiningElements.Add(element);
							}
						}

						// Save the current state information of these Target Elements
						beginAppendHResize(adjoiningElements);
					}
					break;
			}


			// Apply dt to all selected elements.
			foreach (var elem in SelectedElements) {
				// Get this element's original times (before resize started)
				ElementTimeInfo orig = m_elemMoveInfo.OriginalElements[elem];

				switch (m_mouseResizeZone) {
					case ResizeZone.Front:
						elem.StartTime = orig.StartTime + dt;
						elem.EndTime = orig.EndTime;
						//Control when the time changed event happens.
						elem.UpdateNotifyTimeChanged(); 
						break;

					case ResizeZone.Back:
						elem.EndTime = orig.EndTime + dt;
						//Control when the time changed event happens.
						elem.UpdateNotifyTimeChanged();  
						break;
				}
			}

			// Apply dt to all adjoining elements, if any
			if (AltPressed && adjoiningElements is not null)
			{
				foreach (var element in adjoiningElements)
				{
					switch (m_mouseResizeZone)
					{
						case ResizeZone.Front:
							// Move the starting time(s)
							element.EndTime = SelectedElements.First().StartTime;

							// If the resultant duration is less than the minimum,then stop all further resizing
							if (timeToPixels(element.Duration) <= MinElemWidthPx)
							{ 
								element.EndTime = element.StartTime + PixelsToTime(MinElemWidthPx);
								SelectedElements.First().StartTime = element.EndTime;
								SelectedElements.First().EndTime = m_elemMoveInfo.OriginalElements[SelectedElements.First()].EndTime;
							}

							//Control when the time changed event happens.
							element.UpdateNotifyTimeChanged();
							break;

						case ResizeZone.Back:
							// Move the time(s)
							TimeSpan saveEndTime = element.EndTime;
							element.StartTime = SelectedElements.First().EndTime;
							
							// Keep the end time unchanged
							element.EndTime = saveEndTime;

							// If the resultant duration is less than the minimum,then stop all further resizing
							if (timeToPixels(element.Duration) <= MinElemWidthPx)
							{
								element.StartTime = saveEndTime - PixelsToTime(MinElemWidthPx);
								element.Duration = PixelsToTime(MinElemWidthPx);
								SelectedElements.First().EndTime = element.StartTime;
							}

							//Control when the time changed event happens.
							element.UpdateNotifyTimeChanged();
							break;
					}

					// Render the Adjoining elements as they change
					RenderElement(element);
				}
			}

			Invalidate();
		}

		private void MouseUp_HResizing(Point gridLocation)
		{
			elementsFinishedMoving(ElementMoveType.Resize);
			endAllDrag();
			CurrentDragSnapPoints.Clear();
			adjoiningElements = null;
		}

		#endregion

		#region [Mouse Drag] Auto-Scroll

		///<summary>Distance mouse is outside viewport.</summary>
		private Point m_mouseOutside = new Point(0, 0);

		///<summary>The MouseEventArgs from the last MouseMove event.</summary>
		private MouseEventArgs m_lastMouseMove = null;

		///<summary>The new auto-scroll timer.</summary>
		private Timer m_autoScrollTimer;

		private const int AutoScrollPxScaleFactor = 4;

		private void InitAutoScrollTimer()
		{
			m_autoScrollTimer = new Timer();
			m_autoScrollTimer.Interval = 50;
			m_autoScrollTimer.Tick += m_autoScrollTimer_Tick;
		}

		private void m_autoScrollTimer_Tick(object sender, EventArgs e)
		{
			int x = -AutoScrollPosition.X + (m_mouseOutside.X/AutoScrollPxScaleFactor);
			x = (x < 0) ? 0 : x;
			x = (x > AutoScrollMinSize.Width) ? AutoScrollMinSize.Width : x;

			int y = -AutoScrollPosition.Y + (m_mouseOutside.Y/AutoScrollPxScaleFactor);
			y = (y < 0) ? 0 : y;
			y = (y > AutoScrollMinSize.Height) ? AutoScrollMinSize.Height : y;

			AutoScrollPosition = new Point(x, y);

			HandleHorizontalScroll();
			HandleVerticalScroll();

			// Re-call the mouse move handler (effectively), now that our grid location has changed.
			HandleMouseMove(m_lastMouseMove);
		}

		#endregion

		///<summary>Ends all mouse-drag operations.</summary>
		private void endAllDrag()
		{
			m_dragState = DragState.Normal;
			Cursor = Cursors.Default;

			if (m_autoScrollTimer.Enabled)
				m_autoScrollTimer.Stop();

			Invalidate();
		}


		//events
		public event EventHandler<ElementsChangedTimesEventArgs> ElementsMovedNew;

		public event EventHandler<TimelineEventArgs> BackgroundClicked;
	}
}