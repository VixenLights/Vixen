using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl.LabeledMarks;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{
		private readonly List<MarkRow> _rows;
		private Point _mouseDownLocation;
		private Point _moveResizeStartLocation;
		private Mark _mouseDownMark;
		
		private DragState _dragState = DragState.Normal; // the current dragging state
		private ResizeZone _markResizeZone = ResizeZone.None;
		private readonly MarksSelectionManager _marksSelectionManager;
		private readonly MarksEventManager _marksEventManager;
		private Rectangle _ignoreDragArea;
		private const int DragThreshold = 8;
		private const int MinMarkWidthPx = 12;
		private MarksMoveResizeInfo _marksMoveResizeInfo;
		private ObservableCollection<MarkCollection> _markCollections;

		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
			BackColor = Color.Gray;
			_marksSelectionManager = MarksSelectionManager.Manager();
			_marksEventManager = MarksEventManager.Manager;
			_marksEventManager.MarksMoving += _marksEventManager_MarksMoving;
			_marksEventManager.DeleteMark += _marksEventManager_DeleteMark;
			_marksSelectionManager.SelectionChanged += _marksSelectionManager_SelectionChanged;
			_rows = new List<MarkRow>();
			MarkRow.MarkRowChanged += MarkRow_MarkRowChanged;
		}

		public ObservableCollection<MarkCollection> MarkCollections
		{
			get { return _markCollections; }
			set
			{
				UnConfigureMarks();
				_markCollections = value;
				ConfigureMarks();
			}
		}

		private void ConfigureMarks()
		{
			if (_markCollections == null)
			{
				return;
			}
			_markCollections.CollectionChanged += _markCollections_CollectionChanged;
			CreateRows();
			RefreshView();
		}

		private void RefreshView()
		{
			CalculateHeight();
			Invalidate();
		}

		private void CreateRows()
		{
			foreach (var markCollection in _markCollections)
			{
				CreateMarkRow(markCollection);
			}
		}

		private void UnConfigureMarks()
		{
			if (_markCollections == null)
			{
				return;
			}

			_markCollections.CollectionChanged -= _markCollections_CollectionChanged;
			ClearRows();
		}

		private void ClearRows()
		{
			foreach (var row in _rows.ToList())
			{
				_rows.Remove(row);
				row.Dispose();
			}
		}

		private void CreateMarkRow(MarkCollection markCollection)
		{
			MarkRow row = new MarkRow(markCollection);
			_rows.Add(row);
		}

		private void _markCollections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					var markCollection = item as MarkCollection;
					if (markCollection != null)
					{
						CreateMarkRow(markCollection);
					}
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var item in e.OldItems)
				{
					var markCollection = item as MarkCollection;
					if (markCollection != null)
					{
						var row = _rows.Find(x => x.MarkCollection == markCollection);
						_rows?.Remove(row);
						row?.Dispose();
					}
				}
			}
			else
			{
				//Rebuild the rows
				ClearRows();
				CreateRows();
			}

			RefreshView();
		}

		private void MarkRow_MarkRowChanged(object sender, EventArgs e)
		{
			RefreshView();
		}

		#region Mouse Down

		/// <inheritdoc />
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDownMark = null;
			Point location = _mouseDownLocation = TranslateLocation(e.Location);

			
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				_mouseDownMark = MarkAt(location);
				
				if (!CtrlPressed)
				{
					if (_markResizeZone == ResizeZone.None || !_marksSelectionManager.SelectedMarks.Contains(_mouseDownMark))
					{
						_marksSelectionManager.ClearSelected();
					}
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
					BeginHResize(location); // begin a resize.
				}
			}

		}

		#endregion

		#region Mouse Up

		/// <inheritdoc />
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (_mouseDownLocation == TranslateLocation(e.Location) && _mouseDownMark != null)
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
					
					MouseUp_DragMoving();
					break;
				case DragState.HResizing:
					MouseUp_HResizing();
					break;
				default:
					EndAllDrag();
					break;
			}

		}

		#endregion

		#region Mouse Move

		protected override void OnMouseMove(MouseEventArgs e)
		{
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

				case DragState.Waiting: // Waiting to start moving a Mark
					if (!_ignoreDragArea.Contains(location))
					{
						BeginDragMove(location);
					}
					break;

				case DragState.Moving: // Moving Marks along the row
					MouseMove_DragMoving(location);
					break;

				case DragState.HResizing: // Resizing Mark(s) duration
					MouseMove_HResizing(location);
					break;

				default:
					throw new Exception("Unknown DragState.");
			}

			base.OnMouseMove(e);
		}

		private void MouseMove_Normal(Point gridLocation)
		{
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

		#endregion

		#region Move Marks

		/// <summary>
		/// Handles mouse move events while in the "Moving" state.
		/// </summary>
		/// <param name="location">Mouse location on the grid.</param>
		private void MouseMove_DragMoving(Point location)
		{
			// if we don't have anything selected, there's no point dragging anything...
			if (!_marksSelectionManager.SelectedMarks.Any())
				return;

			TimeSpan dt = pixelsToTime(location.X - _moveResizeStartLocation.X);
			
			// If we didn't move, get outta here.
			if (dt == TimeSpan.Zero)
				return;


			// Calculate what our actual dt value will be.
			TimeSpan earliest = _marksMoveResizeInfo.OriginalMarks.Values.Min(x => x.StartTime);
			if ((earliest + dt) < TimeSpan.Zero)
				dt = -earliest;

			TimeSpan latest = _marksMoveResizeInfo.OriginalMarks.Values.Max(x => x.EndTime);
			if ((latest + dt) > TimeInfo.TotalTime)
				dt = TimeInfo.TotalTime - latest;

			foreach (var markTimeInfo in _marksMoveResizeInfo.OriginalMarks)
			{
				markTimeInfo.Key.StartTime = markTimeInfo.Value.StartTime + dt;
			}

			_marksEventManager.OnMarksMoving(new MarksMovingEventArgs(_marksSelectionManager.SelectedMarks.ToList()));
			_marksEventManager.OnSelectedMarkMove(new SelectedMarkMoveEventArgs(true, _mouseDownMark, ResizeZone.None));

			//Invalidate();
		}

		private void WaitForDragMove(Point gridLocation)
		{
			// begin the dragging process -- calculate a area outside which a drag (move) starts
			_dragState = DragState.Waiting;
			_ignoreDragArea = new Rectangle(gridLocation.X - DragThreshold, gridLocation.Y - DragThreshold, DragThreshold * 2,
				DragThreshold * 2);
		}

		private void BeginDragMove(Point location)
		{
			_dragState = DragState.Moving;
			_ignoreDragArea = Rectangle.Empty;
			Cursor = Cursors.Hand;
			BeginMoveResizeMarks(location);
		}

		private void MouseUp_DragMoving()
		{
			FinishedResizeMoveMarks(ElementMoveType.Move);
			EndAllDrag();
		}

		private void EndAllDrag()
		{
			_dragState = DragState.Normal;
			Cursor = Cursors.Default;
			//Invalidate();
		}

		#endregion

		#region Resize

		private void MouseMove_HResizing(Point location)
		{
			TimeSpan dt = pixelsToTime(location.X - _moveResizeStartLocation.X);

			if (dt == TimeSpan.Zero)
				return;

			// Ensure minimum size
			TimeSpan shortest = _marksMoveResizeInfo.OriginalMarks.Values.Min(x => x.Duration);
			
			// Check boundary conditions
			switch (_markResizeZone)
			{
				case ResizeZone.Front:
					// Clip earliest element StartTime at zero
					TimeSpan earliest = _marksMoveResizeInfo.OriginalMarks.Values.Min(x => x.StartTime);
					if (earliest + dt < TimeSpan.Zero)
					{
						dt = -earliest;
					}

					// Ensure the shortest meets minimum width (in px)
					if (timeToPixels(shortest - dt) < MinMarkWidthPx)
					{
						dt = shortest - pixelsToTime(MinMarkWidthPx);
					}
					break;

				case ResizeZone.Back:
					// Clip latest mark EndTime at TotalTime
					TimeSpan latest = _marksMoveResizeInfo.OriginalMarks.Values.Max(x => x.EndTime);
					if (latest + dt > TimeInfo.TotalTime)
					{
						dt = TimeInfo.TotalTime - latest;
					}

					// Ensure the shortest meets minimum width (in px)
					if (timeToPixels(shortest + dt) < MinMarkWidthPx)
					{
						dt = pixelsToTime(MinMarkWidthPx) - shortest;
					}

					break;
			}


			// Apply dt to all selected elements.
			foreach (var originalMarkInfo in _marksMoveResizeInfo.OriginalMarks)
			{
				switch (_markResizeZone)
				{
					case ResizeZone.Front:
						originalMarkInfo.Key.StartTime = originalMarkInfo.Value.StartTime + dt;
						originalMarkInfo.Key.Duration = originalMarkInfo.Value.Duration - dt;
						break;

					case ResizeZone.Back:
						originalMarkInfo.Key.Duration = originalMarkInfo.Value.Duration + dt;
						break;
				}
			}

			_marksEventManager.OnMarksMoving(new MarksMovingEventArgs(_marksSelectionManager.SelectedMarks.ToList()));
			_marksEventManager.OnSelectedMarkMove(new SelectedMarkMoveEventArgs(true, _mouseDownMark, _markResizeZone));

			//Invalidate();
		}

		private void BeginHResize(Point location)
		{
			_dragState = DragState.HResizing;
			BeginMoveResizeMarks(location);
		}

		private void MouseUp_HResizing()
		{
			EndAllDrag();
			FinishedResizeMoveMarks(ElementMoveType.Resize);
		}

		#endregion

		///<summary>Called when any operation that moves mark times (namely drag-move and hresize).
		///Saves the pre-move information and begins update on all selected marks.</summary>
		private void BeginMoveResizeMarks(Point location)
		{
			_moveResizeStartLocation = location;
			_marksMoveResizeInfo = new MarksMoveResizeInfo(_marksSelectionManager.SelectedMarks);
		}

		private void FinishedResizeMoveMarks(ElementMoveType type)
		{
			//TODO This selected mark move thing needs help
			_marksEventManager.OnSelectedMarkMove(new SelectedMarkMoveEventArgs(false, _mouseDownMark, ResizeZone.None));

			_marksEventManager.OnMarkMoved(new MarksMovedEventArgs(_marksMoveResizeInfo, type));

			_marksMoveResizeInfo = null;
			_moveResizeStartLocation = Point.Empty;
		}

		#region Events

		private void _marksEventManager_DeleteMark(object sender, MarksDeletedEventArgs e)
		{
			Invalidate();
		}

		private void _marksEventManager_MarksMoving(object sender, MarksMovingEventArgs e)
		{
			Invalidate();
		}

		private void _marksSelectionManager_SelectionChanged(object sender, EventArgs e)
		{
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
			_mouseDownMark.Parent.RemoveMark(_mouseDownMark);
			_marksEventManager.OnDeleteMark(new MarksDeletedEventArgs(new List<Mark>(new []{_mouseDownMark})));
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
			Height = _rows.Where(x => x.Visible).Sum(x => x.Height);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				CalculateHeight();
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_marksEventManager.MarksMoving -= _marksEventManager_MarksMoving;
				_marksEventManager.DeleteMark -= _marksEventManager_DeleteMark;
				_marksSelectionManager.SelectionChanged -= _marksSelectionManager_SelectionChanged;
				UnConfigureMarks();
			}
			base.Dispose(disposing);
		}
	}
}
