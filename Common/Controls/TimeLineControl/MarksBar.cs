using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{

		private readonly List<MarkRow> _rows;
		private bool _suppressInvalidate;
		private Point _mouseDownLocation, _mouseUpLocation;
		private Mark _mouseDownMark;
		private TimeSpan _dragStartTime;
		private TimeSpan _dragLastTime;

		private DragState _dragState = DragState.Normal; // the current dragging state
		private ResizeZone _markResizeZone = ResizeZone.None;
		private readonly MarksSelectionManager _marksSelectionManager;
		private Rectangle _ignoreDragArea;
		private Point _waitingBeginGridLocation;
		private const int DragThreshold = 8;

		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;
		public event EventHandler<MarksDeletedEventArgs> DeleteMark;
		public static event EventHandler<SelectedMarkMoveEventArgs> SelectedMarkMove;

		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
			BackColor = Color.Gray;
			_marksSelectionManager = MarksSelectionManager.Manager();
			_marksSelectionManager.SelectionChanged += _marksSelectionManager_SelectionChanged;
			_rows = new List<MarkRow>();
		}

		private void _marksSelectionManager_SelectionChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		public void AddMarks(MarkCollection labeledMarkCollection)
		{
			MarkRow row = new MarkRow(labeledMarkCollection);
			_rows.Add(row);
			if (!_suppressInvalidate)
			{
				CalculateHeight();
				Invalidate();
			}
		}

		public void ClearMarks()
		{
			_rows.Clear();
			if (!_suppressInvalidate) Invalidate();
		}

		#region Overrides of Control

		#region Overrides of UserControl

		/// <inheritdoc />
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDownMark = null;
			Point location = _mouseDownLocation = TranslateLocation(e.Location);

			
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				_mouseDownMark = MarkAt(location);
				_dragStartTime = _dragLastTime = pixelsToTime(location.X) + VisibleTimeStart;

				if (!CtrlPressed)
				{
					_marksSelectionManager.ClearSelected();
					_marksSelectionManager.Select(_mouseDownMark);
				}
				else
				{
					_marksSelectionManager.Select(_mouseDownMark);
				}
			}

			if (e.Button == MouseButtons.Left)
			{
				if (_markResizeZone == ResizeZone.None)
				{
					// begin waiting for a normal drag
					WaitForDragMove(location);
				}
				else if (!CtrlPressed)
				{
					//beginHResize(gridLocation); // begin a resize.
				}
			}

		}

		#endregion

		/// <inheritdoc />
		protected override void OnMouseUp(MouseEventArgs e)
		{

			Point location = _mouseUpLocation = TranslateLocation(e.Location);

			if (e.Button == MouseButtons.Right)
			{
				if (_mouseDownLocation == _mouseUpLocation && _mouseDownMark != null)
				{
					ContextMenuStrip c = new ContextMenuStrip();
					c.Renderer = new ThemeToolStripRenderer();
					var delete = c.Items.Add("&Delete");
					delete.Click += DeleteMark_Click;
					var rename = c.Items.Add("&Rename");
					rename.Click += Rename_Click;
					c.Show(this, new Point(e.X, e.Y));
				}

			}

			switch (_dragState)
			{
				case DragState.Moving:
					var newTime = pixelsToTime(e.X) + VisibleTimeStart;
					OnMarkMoved(new MarksMovedEventArgs(_dragStartTime - newTime, _marksSelectionManager.SelectedMarks.ToList()));
					OnSelectedMarkMove(new SelectedMarkMoveEventArgs(false, _mouseDownMark.StartTime));
					EndAllDrag();
					break;
				default:
					EndAllDrag();
					break;
			}

		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			// Determine if we need to start auto-drag
			switch (_dragState)
			{
				case DragState.Moving:
				case DragState.HResizing:

					//m_mouseOutside.X = (e.X <= AutoScrollMargin.Width)
					//	? -(AutoScrollMargin.Width - e.X)
					//	: (e.X > ClientSize.Width - AutoScrollMargin.Width)
					//		? e.X - ClientSize.Width + AutoScrollMargin.Width
					//		: 0;

					//m_mouseOutside.Y = (e.Y <= AutoScrollMargin.Height)
					//	? -(AutoScrollMargin.Height - e.Y)
					//	: (e.Y > ClientSize.Height - AutoScrollMargin.Height)
					//		? e.Y - ClientSize.Height + AutoScrollMargin.Height
					//		: 0;

					//if (m_mouseOutside.X != 0 || m_mouseOutside.Y != 0)
					//{
					//	if (!m_autoScrollTimer.Enabled)
					//		m_autoScrollTimer.Start(); // Mouse is outside viewport - start auto scroll timer.
					//}
					//else
					//{
					//	if (m_autoScrollTimer.Enabled)
					//		m_autoScrollTimer.Stop(); // Mouse is inside viewport - stop auto scroll timer.
					//}

					break;
			}

			//m_lastMouseMove = e;
			HandleMouseMove(e);
		}

		private void HandleMouseMove(MouseEventArgs e)
		{
			Point location = TranslateLocation(e.Location);
		
			switch (_dragState)
			{
				case DragState.Normal: // Normal -> Not dragging; mouse is up
					MouseMove_Normal(location);
					break;

				case DragState.Waiting: // Waiting to start moving an object
					if (!_ignoreDragArea.Contains(location))
					{
						BeginDragMove();
					}
					break;

				case DragState.Moving: // Moving objects
									   //Gets the element we are working with 
					//if (ResizeIndicator_Enabled && elementsAt(gridLocation).Any())
					//	foreach (Element elem in elementsAt(gridLocation))
					//	{
					//		if (elem.Selected) _workingElement = elem;
					//	}
					MouseMove_DragMoving(location);
					break;

				case DragState.HResizing: // Resizing an element
					//MouseMove_HResizing(gridLocation, delta);
					break;

				default:
					throw new Exception("Unknown DragState.");
			}

			base.OnMouseMove(e);
		}

		private void MouseMove_Normal(Point gridLocation)
		{
			//Point gridLocation = TranslateLocation(e.Location);

			// Are we in a 'resize zone' at the front or back of an element?
			Mark mark = MarkAt(gridLocation);
			if (mark == null)
			{
				_markResizeZone = ResizeZone.None;
			}
			else
			{
				// smaller of constant, or half of element width
				int grabThreshold = Math.Min(12, (int)(timeToPixels(mark.Duration) / 2));
				float elemStart = timeToPixels(mark.StartTime);
				float elemEnd = timeToPixels(mark.EndTime);
				int x = gridLocation.X;

				if ((x >= elemStart) && (x < (elemStart + grabThreshold)))
					_markResizeZone = ResizeZone.Front;
				else if ((x <= elemEnd) && (x > (elemEnd - grabThreshold)))
					_markResizeZone = ResizeZone.Back;
				else
					_markResizeZone = ResizeZone.None;
			}

			Cursor = _markResizeZone == ResizeZone.None ? Cursors.Default : Cursors.SizeWE;
		}


		/// <summary>
		/// Handles mouse move events while in the "Moving" state.
		/// </summary>
		/// <param name="location">Mouse location on the grid.</param>
		private void MouseMove_DragMoving(Point location)
		{
			// if we don't have anything selected, there's no point dragging anything...
			if (!_marksSelectionManager.SelectedMarks.Any())
				return;

			var newTime = pixelsToTime(location.X) + VisibleTimeStart;
			OnMarksMoving(new MarksMovingEventArgs(_marksSelectionManager.SelectedMarks.ToList()));
			var diff = newTime - _dragLastTime;
			_marksSelectionManager.SelectedMarks.ToList().ForEach(x => x.StartTime += diff);
			_dragLastTime = newTime;
			OnSelectedMarkMove(new SelectedMarkMoveEventArgs(true, _mouseDownMark.StartTime));
			Invalidate();
		}

		private void WaitForDragMove(Point gridLocation)
		{
			// begin the dragging process -- calculate a area outside which a drag (move) starts
			_dragState = DragState.Waiting;
			_waitingBeginGridLocation = gridLocation;
			_ignoreDragArea = new Rectangle(gridLocation.X - DragThreshold, gridLocation.Y - DragThreshold, DragThreshold * 2,
				DragThreshold * 2);
		}

		private void BeginDragMove()
		{
			_dragState = DragState.Moving;
			_ignoreDragArea = Rectangle.Empty;
			Cursor = Cursors.Hand;
		}

		private void EndAllDrag()
		{
			_dragState = DragState.Normal;
			Cursor = Cursors.Default;
			Invalidate();
		}

		private void Rename_Click(object sender, EventArgs e)
		{
			TextDialog td = new TextDialog("Enter the new name.", "Rename Mark");
			var result = td.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				_mouseDownMark.Text = td.Response;
				Invalidate();
			}
		}

		private void DeleteMark_Click(object sender, EventArgs e)
		{
			OnDeleteMark(new MarksDeletedEventArgs(new List<Mark>(new []{_mouseDownMark})));
		}

		#endregion

		private bool CtrlPressed => ModifierKeys.HasFlag(Keys.Control);

		/// <summary>
		/// Returns all elements located at the given point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Elements at given point, or null if none exists.</returns>
		private Mark MarkAt(Point p)
		{
			
			// First figure out which row we are in
			MarkRow containingRow = RowAt(p);

			if (containingRow == null)
				return null;

			// Now figure out which element we are on
			foreach (Mark mark in containingRow)
			{
				Single x = timeToPixels(mark.StartTime);
				if (x > p.X) break; //The rest of them are beyond our point.
				Single width = timeToPixels(mark.Duration);
				MarkRow.MarkStack ms = containingRow.GetStackForMark(mark);
				var displayHeight = containingRow.Height / ms.StackCount;
				var rowTopOffset = displayHeight * ms.StackIndex;
				if (p.X >= x &&
				    p.X <= x + width &&
				    p.Y >= containingRow.DisplayTop + rowTopOffset &&
				    p.Y < containingRow.DisplayTop + rowTopOffset + displayHeight)
				{
					return mark;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the row located at the current point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Row at given point, or null if none exists.</returns>
		private MarkRow RowAt(Point p)
		{
			MarkRow containingRow = null;
			int curheight = 0;
			foreach (MarkRow row in _rows)
			{
				if (p.Y < curheight + row.Height)
				{
					containingRow = row;
					break;
				}
				curheight += row.Height;
			}

			return containingRow;
		}

		private void CalculateRowDisplayTops(bool visibleRowsOnly = true)
		{
			int top = 0;
			var processRows = visibleRowsOnly ? _rows.Where(x => x.Visible) : _rows;
			foreach (var row in processRows)
			{
				row.DisplayTop = top;
				top += row.Height;
			}
		}

		/// <summary>
		/// Translates a location (Point) so that its coordinates represent the coordinates on the underlying timeline, taking into account scroll position.
		/// </summary>
		/// <param name="originalLocation"></param>
		public Point TranslateLocation(Point originalLocation)
		{
			// Translate this location based on the auto scroll position.
			Point p = originalLocation;
			var xOffset = (int) timeToPixels(VisibleTimeStart);
			p.Offset(xOffset, 0);
			return p;
		}

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void CalculateHeight()
		{
			Height = _rows.Sum(x => x.Height);
		}

		public void BeginDraw()
		{
			_suppressInvalidate = true;
		}

		public void EndDraw()
		{
			_suppressInvalidate = false;
			CalculateHeight();
			Invalidate();
		}

		public void OnSelectedMarkMove(SelectedMarkMoveEventArgs e)
		{
			if (SelectedMarkMove != null)
				SelectedMarkMove(this, e);
		}

		private void OnMarkMoved(MarksMovedEventArgs e)
		{
			MarksMoved?.Invoke(this, e);
		}

		private void OnMarksMoving(MarksMovingEventArgs e)
		{
			MarksMoving?.Invoke(this, e);
		}

		private void OnDeleteMark(MarksDeletedEventArgs e)
		{
			DeleteMark?.Invoke(this, e);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				// Translate the graphics to work the same way the timeline grid does
				// (ie. Drawing coordinates take into account where we start at in time)
				e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

				DrawRows(e.Graphics);
				DrawMarks(e.Graphics);

			}
			catch (Exception ex)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Exception in Timeline.MarksBar.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace,
					@"Error", false, false);
				messageBox.ShowDialog();
			}
		}

		private void DrawRows(Graphics g)
		{
			int curY = 0;
			var start = (int)timeToPixels(VisibleTimeStart);
			var end = (int)timeToPixels(VisibleTimeEnd);
			CalculateRowDisplayTops();
			// Draw row separators
			using (Pen p = new Pen(ThemeColorTable.TimeLineGridColor))
			{
				foreach (var row in _rows.Where(x => x.Visible))
				{
					curY += row.Height;
					Point lineLeft = new Point(start, curY);
					Point lineRight = new Point(end, curY);
					g.DrawLine(p, lineLeft.X, lineLeft.Y - 1, lineRight.X, lineRight.Y - 1);
				}
			}
		}

		private void DrawMarks(Graphics g)
		{
			int displaytop = 0;
			foreach (var row in _rows.Where(x => x.Visible))
			{
				row.SetStackIndexes(VisibleTimeStart, VisibleTimeEnd);
				for (int i = 0; i < row.MarksCount; i++)
				{
					Mark currentElement = row.GetMarkAtIndex(i);
					if (currentElement.EndTime < VisibleTimeStart)
						continue;

					if (currentElement.StartTime > VisibleTimeEnd)
					{
						break;
					}

					DrawMark(g, row, currentElement, displaytop);
				}

				displaytop += row.Height;
			}

		}

		private void DrawMark(Graphics g, MarkRow row, Mark mark, int top)
		{
			int width;
			
			//Sanity check - it is possible for .DisplayHeight to become zero if there are too many marks stacked.
			//We set the DisplayHeight to the row height for the mark, and change the border to red.	
			var markStack = row.GetStackForMark(mark);
			var displayHeight =
				(markStack.StackCount != 0) ? ((row.Height - 1) / markStack.StackCount) : row.Height - 1;

			var displayTop = top + displayHeight * markStack.StackIndex;
			
			if (displayHeight == 0)
			{
				displayHeight = row.Height;
			}

			if (mark.StartTime >= VisibleTimeStart)
			{
				if (mark.EndTime < VisibleTimeEnd)
				{
					width = (int)timeToPixels(mark.Duration);
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(mark.StartTime));
				}
			}
			else
			{
				if (mark.EndTime <= VisibleTimeEnd)
				{
					width = (int)timeToPixels(mark.Duration);// (int)(timeToPixels(mark.EndTime) - timeToPixels(VisibleTimeStart));
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(VisibleTimeStart));
				}
			}
			if (width <= 0) return;
			Size size = new Size(width, displayHeight);

			Bitmap labelImage = DrawMarkLabel(size, row.MarkDecorator.Color);
			if (labelImage == null) return;
			Point finalDrawLocation = new Point((int)Math.Floor(timeToPixels(mark.StartTime)), displayTop);

			Rectangle destRect = new Rectangle(finalDrawLocation.X, finalDrawLocation.Y, size.Width, displayHeight);
			g.DrawImage(labelImage, destRect);

			if (_marksSelectionManager.SelectedMarks.Contains(mark))
			{
				using (Pen bp = new Pen(Color.Blue,2))
				{
					bp.Alignment = PenAlignment.Inset;
					g.DrawRectangle(bp, destRect);
				}
			}
			

			//Draw the text

			SolidBrush drawBrush = new SolidBrush(Color.Black);
			StringFormat drawFormat = new StringFormat();
			g.DrawString(mark.Text, SystemFonts.MessageBoxFont, drawBrush, destRect, drawFormat);
		}

		public Bitmap DrawMarkLabel(Size imageSize, Color c)
		{
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(c))
				{
					g.FillRectangle(b,
						new Rectangle((int)g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
							(int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));
				}

				
			}

			return result;
		}


	}
}
