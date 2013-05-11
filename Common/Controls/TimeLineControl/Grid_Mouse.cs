using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Common.Controls.Timeline
{
    public partial class Grid
    {

        #region General Mouse Event-Related


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

            m_lastGridLocation = gridLocation;  //new
			m_mouseDownElementRow = rowAt(gridLocation);

			m_mouseDownElements = elementsAt(gridLocation);
			if (m_mouseDownElements == null)
				m_mouseDownElements = new List<Element>();

            if (e.Button == MouseButtons.Left)
            {

				if (m_mouseDownElements.Count <= 0)
                {
                    // we clicked on the background - clear anything that is selected, and begin a 'selection' drag.
                    if (!CtrlPressed)
                        beginDragSelect(gridLocation);
                }
                else
                {
                    // our mouse is down on something -- if it's something selected, unselect it with CTRL
                    if (m_mouseDownElements.Any(x => x.Selected))
                    {
                        // unselect
						if (CtrlPressed) {
							foreach (Element elem in m_mouseDownElements.Where(x => x.Selected))
								elem.Selected = false;
							_SelectionChanged();
						}
                    }
                    else
                    {
                        // there is one-or-more elements under the mouse, and none of them are selected. If CTRL isn't pressed
						// (ie. we aren't going to ADD to the selection, but replace it) clear the current selection, then pass
						// it to the ElementsSelected event handler to handle selection if an external party wants to handle selection.
                        if (!CtrlPressed)
                        {
                            ClearSelectedElements();
                            ClearSelectedRows();
                        }
						if (_ElementsSelected(m_mouseDownElements)) {
							foreach (Element element in m_mouseDownElements) {
								element.Selected = true;
							}
							_SelectionChanged();
						}
                    }

                    if (m_mouseResizeZone == ResizeZone.None)
                    {
                        //waitForDragMove(e.Location);	// begin waiting for a normal drag
                        waitForDragMove(gridLocation);
                    }
                    else
                    {
                        beginHResize(gridLocation);	// begin a resize.
                    }
                }
                //Invalidate();
            }
        }


        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Point gridLocation = translateLocation(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                switch (m_dragState)
                {
                    case DragState.Moving:
                        MouseUp_DragMoving(gridLocation);
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
						if (m_mouseDownElements != null && m_mouseDownElements.Count() > 0 && !CtrlPressed)
                        {
                            ClearSelectedElements();
							if (_ElementsSelected(m_mouseDownElements)) {
								m_mouseDownElements.First().Selected = true;
								_SelectionChanged();
							}
                        }
                        break;
                }
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

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            Point gridLocation = translateLocation(e.Location);
            Element elem = elementAt(gridLocation);

            if (elem != null)
            {
                _ElementDoubleClicked(elem);
            }
        }

        #endregion


        #region Mouse Move

        ///<summary>The last location on the grid the mouse was at.</summary>
        private Point m_lastGridLocation;

		protected override void OnMouseMove(MouseEventArgs e) {
			// Determine if we need to start auto-drag
			switch (m_dragState) {
				case DragState.Moving:
				case DragState.Selecting:
				case DragState.HResizing:

					m_mouseOutside.X = (e.X <= AutoScrollMargin.Width)
						? -(AutoScrollMargin.Width - e.X)
						: (e.X > ClientSize.Width - AutoScrollMargin.Width)
							? e.X - ClientSize.Width + AutoScrollMargin.Width : 0;

					m_mouseOutside.Y = (e.Y <= AutoScrollMargin.Height)
						? -(AutoScrollMargin.Height - e.Y)
						: (e.Y > ClientSize.Height - AutoScrollMargin.Height)
							? e.Y - ClientSize.Height + AutoScrollMargin.Height : 0;

					if (m_mouseOutside.X != 0 || m_mouseOutside.Y != 0) {
						if (!m_autoScrollTimer.Enabled)
							m_autoScrollTimer.Start();  // Mouse is outside viewport - start auto scroll timer.
					}
					else {
						if (m_autoScrollTimer.Enabled)
							m_autoScrollTimer.Stop();   // Mouse is inside viewport - stop auto scroll timer.
					}

					break;
			}

			m_lastMouseMove = e;
			HandleMouseMove(e);
		}

        ///<summary>Calls a different MouseMove_ function depending on the current drag state.</summary>
        private void HandleMouseMove(MouseEventArgs e)
        {
            Point gridLocation = translateLocation(e.Location);

            Point delta = new Point(
                gridLocation.X - m_lastGridLocation.X,
                gridLocation.Y - m_lastGridLocation.Y
                );

            m_lastGridLocation = gridLocation;

            //Debug.WriteLine("Grid Location: {0},{1}    Delta: {2},{3}", gridLocation.X, gridLocation.Y, delta.X, delta.Y);

            switch (m_dragState)
            {
                case DragState.Normal:				// Normal -> Not dragging; mouse is up
                    MouseMove_Normal(gridLocation);
                    break;

                case DragState.Waiting:				// Waiting to start moving an object
                    if (!m_ignoreDragArea.Contains(gridLocation))
                        beginDragMove(gridLocation);
                    break;

                case DragState.Moving:				// Moving objects
                    MouseMove_DragMoving(gridLocation);
                    break;

                case DragState.Selecting:			// Dragging a selection rectangle
                    MouseMove_DragSelect(gridLocation);
                    break;

                case DragState.HResizing:			// Resizing an element
                    MouseMove_HResizing(gridLocation, delta);
                    break;

                default:
                    throw new Exception("Unknown DragState.");
            }

            base.OnMouseMove(e);
        }

        #endregion


        #region [Mouse Drag] Normal (not dragging)

        private const int ElementResizeThreshold = 12;	// number of pixels to edge where mouse must be for risize.
        private ResizeZone m_mouseResizeZone = ResizeZone.None;


        //private void MouseMove_DragNormal(MouseEventArgs e)
        private void MouseMove_Normal(Point gridLocation)
        {
            //Point gridLocation = translateLocation(e.Location);

            // Are we in a 'resize zone' at the front or back of an element?
            Element element = elementAt(gridLocation);
            if (element == null)
            {
                m_mouseResizeZone = ResizeZone.None;
            }
            else
            {
                // smaller of constant, or half of element width
                int grabThreshold = Math.Min(ElementResizeThreshold, (int)(timeToPixels(element.Duration) / 2));
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

            Cursor = (m_mouseResizeZone == ResizeZone.None) ? Cursors.Default : Cursors.SizeWE;
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

        private void elementsFinishedMoving(ElementMoveType type)
        {
            foreach (var elem in SelectedElements)
                elem.EndUpdate();

            endAllDrag();

            MultiElementEventArgs evargs = new MultiElementEventArgs { Elements = SelectedElements };
            _ElementsFinishedMoving(evargs);

            if (ElementsMovedNew != null)
            {
                ElementsMovedNew(this, new ElementsChangedTimesEventArgs(m_elemMoveInfo, type));
            }

            m_elemMoveInfo = null;
        }


        private void elementsCancelMove()
        {
            if (m_elemMoveInfo == null)
                return;

            VisibleTimeStart = m_elemMoveInfo.VisibleTimeStart;

            foreach (var kvp in m_elemMoveInfo.OriginalElements)
            {
                // Restore each element's times.
                kvp.Key.StartTime = kvp.Value.StartTime;
                kvp.Key.Duration = kvp.Value.Duration;
            }

            endAllDrag();
        }


        #endregion

        #region [Mouse Drag] Selection Box

        private void beginDragSelect(Point gridLocation)
        {
            m_dragState = DragState.Selecting;
            ClearSelectedElements();
            ClearSelectedRows(m_mouseDownElementRow);
            SelectionArea = new Rectangle(gridLocation.X, gridLocation.Y, 0, 0);
            m_selectionRectangleStart = gridLocation;
		}


        //private void MouseMove_DragSelecting(MouseEventArgs e)
        private void MouseMove_DragSelect(Point gridLocation)
        {
            Point topLeft = new Point(Math.Min(m_selectionRectangleStart.X, gridLocation.X), Math.Min(m_selectionRectangleStart.Y, gridLocation.Y));
            Point bottomRight = new Point(Math.Max(m_selectionRectangleStart.X, gridLocation.X), Math.Max(m_selectionRectangleStart.Y, gridLocation.Y));

            SelectionArea = Util.RectangleFromPoints(topLeft, bottomRight);
            selectElementsWithin(SelectionArea);
            Invalidate();
        }

        private void MouseUp_DragSelect(Point gridLocation)
        {
            // we will only be Selecting if we clicked on the grid background, so on mouse up, check if
            // we didn't move (or very far): if so, consider it just a background click.
            if (SelectionArea.Width < 2 && SelectionArea.Height < 2)
            {
				OnBackgroundClick(new TimelineEventArgs(rowAt(gridLocation), pixelsToTime(gridLocation.X)));
            }

			// done with the selection rectangle.
			m_selectionRectangleStart = Point.Empty;
			SelectionArea = Rectangle.Empty;

			endAllDrag();
        }

        #endregion

		protected void OnBackgroundClick(TimelineEventArgs e)
		{
			if (e.Row != null)
				e.Row.Selected = true;

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
            m_ignoreDragArea = new Rectangle(gridLocation.X - DragThreshold, gridLocation.Y - DragThreshold, DragThreshold * 2, DragThreshold * 2);
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
            // build up a full set of snap points/details, from (a) the static snap points for the grid, and
            // (b) calculated snap points for this move (ie. other elements in the row[s]).
            CurrentDragSnapPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>(StaticSnapPoints);

            // iterate through the rows, calculating snap points for every single element in each row that has any selected elements
            foreach (Row row in Rows)
            {
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

                foreach (Element element in row)
                {
                    // skip it if it's a selected element; we don't want to snap to them, as they'll be moving as well
                    if (element.Selected)
                        continue;

                    // if it's a non-selected element, generate snap points for it; for the start and end times. Also record the
                    // row its from in the generated point, so when snapping we can check against only elements from this row.
                    SnapDetails details = CalculateSnapDetailsForPoint(element.StartTime, SnapPriorityForElements, Color.Empty);
                    details.SnapRow = row;

                    if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime))
                    {
                        CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
                    }
                    CurrentDragSnapPoints[details.SnapTime].Add(details);

                    details = CalculateSnapDetailsForPoint(element.EndTime, SnapPriorityForElements, Color.Empty);
                    details.SnapRow = row;

                    if (!CurrentDragSnapPoints.ContainsKey(details.SnapTime))
                    {
                        CurrentDragSnapPoints[details.SnapTime] = new List<SnapDetails>();
                    }
                    CurrentDragSnapPoints[details.SnapTime].Add(details);
                }
            }
        }


        //TODO: This has not been revisited yet.
        /*
        private void MouseMove_DragMoving_OLD(MouseEventArgs e)
        {
            Point gridLocation = translateLocation(e.Location);

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
                TimeSpan desiredMoveTime = DragTimeLeftOver + pixelsToTime(d.X);
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
            if (SelectedElements.Count() == 0)
                return;

            TimeSpan dt = pixelsToTime(gridLocation.X - m_elemMoveInfo.InitialGridLocation.X);
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

            foreach (var elem in SelectedElements)
            {
                // Get this elemenent's original times (before resize started)
                ElementTimeInfo orig = m_elemMoveInfo.OriginalElements[elem];
                elem.StartTime = orig.StartTime + dt;
            }

            // if we've moved vertically, we may need to move elements between rows
            if (dy != 0)
            {
                MoveElementsVerticallyToLocation(SelectedElements, gridLocation);
            }

            Invalidate();
        }

        private void MouseUp_DragMoving(Point gridLocation)
        {
            elementsFinishedMoving(ElementMoveType.Move);
            endAllDrag();
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
		}

        private void MouseMove_HResizing(Point gridLocation, Point delta)
        {
            TimeSpan dt = pixelsToTime(gridLocation.X - m_elemMoveInfo.InitialGridLocation.X);

            if (dt == TimeSpan.Zero)
                return;

            // Modifidy our dt, if necessary.

			// modify the dt time based on snap points (ie. marks, and borders of other elements)
			dt = FindSnapTimeForElements(SelectedElements, dt, m_mouseResizeZone);

            // Ensure minimum size
            TimeSpan shortest = m_elemMoveInfo.OriginalElements.Values.Min(x => x.Duration);

            // Check boundary conditions
            switch (m_mouseResizeZone)
            {
                case ResizeZone.Front:
                    // Clip earliest element StartTime at zero
                    TimeSpan earliest = m_elemMoveInfo.OriginalElements.Values.Min(x => x.StartTime);
                    if ((earliest + dt) < TimeSpan.Zero)
                        dt = -earliest;

                    // Ensure the shortest meets minimum width (in px)
                    if (timeToPixels(shortest - dt) < MinElemWidthPx)
                        dt = shortest - pixelsToTime(MinElemWidthPx);
                    break;

                case ResizeZone.Back:
                    // Clip latest element EndTime at TotalTime
                    TimeSpan latest = m_elemMoveInfo.OriginalElements.Values.Max(x => x.EndTime);
                    if ((latest + dt) > TimeInfo.TotalTime)
                        dt = TimeInfo.TotalTime - latest;

                    // Ensure the shortest meets minimum width (in px)
                    if (timeToPixels(shortest + dt) < MinElemWidthPx)
                        dt = pixelsToTime(MinElemWidthPx) - shortest;
                    break;
            }


            // Apply dt to all selected elements.
            foreach (var elem in SelectedElements)
            {
                // Get this elemenent's original times (before resize started)
                ElementTimeInfo orig = m_elemMoveInfo.OriginalElements[elem];

                switch (m_mouseResizeZone)
                {
                    case ResizeZone.Front:
                        elem.StartTime = orig.StartTime + dt;
                        elem.EndTime = orig.EndTime;
                        break;

                    case ResizeZone.Back:
                        elem.EndTime = orig.EndTime + dt;
                        break;
                }
            }

            Invalidate();
        }

        private void MouseUp_HResizing(Point gridLocation)
        {
            elementsFinishedMoving(ElementMoveType.Resize);
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

        void m_autoScrollTimer_Tick(object sender, EventArgs e)
        {
            int x = -AutoScrollPosition.X + (m_mouseOutside.X / AutoScrollPxScaleFactor);
            x = (x < 0) ? 0 : x;
            x = (x > AutoScrollMinSize.Width) ? AutoScrollMinSize.Width : x;

            int y = -AutoScrollPosition.Y + (m_mouseOutside.Y / AutoScrollPxScaleFactor);
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
            this.Cursor = Cursors.Default;

            if (m_autoScrollTimer.Enabled)
                m_autoScrollTimer.Stop();

            Invalidate();
        }






        //events
        public event EventHandler<ElementsChangedTimesEventArgs> ElementsMovedNew;

		public event EventHandler<TimelineEventArgs> BackgroundClicked;
    }


    public enum ElementMoveType { Move, Resize }
}
