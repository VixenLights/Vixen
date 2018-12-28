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
using Vixen.Marks;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{
		private readonly List<MarkRow> _rows;
		private Point _mouseDownLocation;
		private Point _moveResizeStartLocation;
		private Point _lastSingleSelectedMarkLocation;
		private IMark _mouseDownMark;
		
		private DragState _dragState = DragState.Normal; // the current dragging state
		private ResizeZone _markResizeZone = ResizeZone.None;
		private readonly MarksSelectionManager _marksSelectionManager;
		private readonly TimeLineGlobalEventManager _timeLineGlobalEventManager;
		private Rectangle _ignoreDragArea;
		private const int DragThreshold = 8;
		private const int MinMarkWidthPx = 12;
		private MarksMoveResizeInfo _marksMoveResizeInfo;
		private ObservableCollection<IMarkCollection> _markCollections;
		private readonly Font _textFont;
		private List<IMark> _clipboard = new List<IMark>();

		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
			BackColor = Color.Gray;
			_textFont = Font;
			_marksSelectionManager = MarksSelectionManager.Manager();
			_timeLineGlobalEventManager = TimeLineGlobalEventManager.Manager;
			_timeLineGlobalEventManager.MarksMoving += TimeLineGlobalEventManagerTimeLineGlobalMoving;
			_timeLineGlobalEventManager.MarksMoved += TimeLineGlobalEventManager_MarksMoved;
			_timeLineGlobalEventManager.DeleteMark += TimeLineGlobalEventManagerDeleteTimeLineGlobal;
			_timeLineGlobalEventManager.MarksTextChanged += TimeLineGlobalEventManager_MarksTextChanged;
			_marksSelectionManager.SelectionChanged += MarksSelectionManager_SelectionChanged;
			_rows = new List<MarkRow>();
			MarkRow.MarkRowChanged += MarkRow_MarkRowChanged;
		}
		
		public ObservableCollection<IMarkCollection> MarkCollections
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
			if (Height == 0)
			{
				if (_rows.Any(x => x.Visible))
				{
					CalculateHeight();
				}
			}
			Invalidate();
			Refresh();
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

		private void CreateMarkRow(IMarkCollection markCollection)
		{
			MarkRow row = new MarkRow(markCollection);
			_rows.Add(row);
		}

		private void _markCollections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

		#region Overrides of Control

		/// <inheritdoc />
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteSelectedMarks();
				e.SuppressKeyPress = true;
			}
		}

		#endregion

		#region Mouse Down

		/// <inheritdoc />
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();
			_mouseDownMark = null;
			Point location = _mouseDownLocation = TranslateLocation(e.Location);

			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				_mouseDownMark = MarkAt(location);

				if (e.Clicks == 2 && _mouseDownMark != null)
				{
					if (AltPressed)
					{
						BeginMoveResizeMarks(location);
						_mouseDownMark.Parent.FillGapTimes(_mouseDownMark);
						FinishedResizeMoveMarks(ElementMoveType.Resize);
						return;
					}
					
					_timeLineGlobalEventManager.OnPlayRange(new PlayRangeEventArgs(_mouseDownMark.StartTime,
						_mouseDownMark.EndTime));
					return;
				}
				
				if (!CtrlPressed && !ShiftPressed)
				{
					if (!_marksSelectionManager.IsSelected(_mouseDownMark))
					{
						_lastSingleSelectedMarkLocation = location;
						_marksSelectionManager.ClearSelected();
						_marksSelectionManager.Select(_mouseDownMark);
					}
				}
				else if(ShiftPressed)
				{

					if (_lastSingleSelectedMarkLocation != Point.Empty)
					{
						SelectMarksBetween(_lastSingleSelectedMarkLocation, location);
					}
				}
				else
				{
					if (_marksSelectionManager.IsSelected(_mouseDownMark))
					{
						_marksSelectionManager.DeSelect(_mouseDownMark);
					}
					else
					{
						_lastSingleSelectedMarkLocation = location;
						_marksSelectionManager.Select(_mouseDownMark);
					}
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
			Point location = TranslateLocation(e.Location);
			if (e.Button == MouseButtons.Right)
			{
				ContextMenuStrip c = new ContextMenuStrip();
				c.Renderer = new ThemeToolStripRenderer();

				var rename = c.Items.Add("Edit Text");
				rename.Enabled = _mouseDownLocation == location && _mouseDownMark != null;
				rename.Click += Rename_Click;

				var cutMarks = c.Items.Add("Cut");
				cutMarks.Click += CutMarksOnClick;
				cutMarks.Enabled = _marksSelectionManager.SelectedMarks.Any();
				c.Items.Add(cutMarks);

				var copyMarks = c.Items.Add("Copy");
				copyMarks.Click += CopyMarksOnClick;
				copyMarks.Enabled = _marksSelectionManager.SelectedMarks.Any();
				c.Items.Add(copyMarks);

				//var copy = c.Items.Add("Copy Text");
				//copy.Click += Copy_Click;
				//copy.Enabled = _marksSelectionManager.SelectedMarks.Count == 1;

				var pasteMarks = c.Items.Add("Paste");
				pasteMarks.Click += PasteMarks_Click;
				pasteMarks.Enabled = _mouseDownLocation == location && _clipboard.Count > 0;
				c.Items.Add(pasteMarks);
				
				//var paste = c.Items.Add("Paste Text");
				//paste.Click += Paste_Click;
				//paste.Enabled = _marksSelectionManager.SelectedMarks.Any() && Clipboard.ContainsText();

				if (c.Items.Count > 0)
				{
					c.Items.Add(new ToolStripSeparator());
				}

				var delete = c.Items.Add("Delete");
				delete.Enabled = _marksSelectionManager.SelectedMarks.Any();
				delete.Click += DeleteMark_Click;
				
				if (c.Items.Count > 0)
				{
					c.Items.Add(new ToolStripSeparator());
				}

				if (_mouseDownLocation == location && _mouseDownMark != null)
				{
					if (_marksSelectionManager.SelectedMarks.All(x =>
						x.Parent.CollectionType == MarkCollectionType.Phoneme))
					{
						c.Items.Add(CreatePhonemeMenuItem());
					}

					if (_marksSelectionManager.SelectedMarks.All(x =>
						x.Parent.CollectionType == MarkCollectionType.Phrase))
					{
						var breakdownPhrase = c.Items.Add("Breakdown Phrase");
						breakdownPhrase.Click += BreakdownPhrase_Click;
					}

					if (_marksSelectionManager.SelectedMarks.All(x =>
						x.Parent.CollectionType == MarkCollectionType.Word))
					{
						var breakdownWord = c.Items.Add("Breakdown Word");
						breakdownWord.Click += BreakdownWord_Click;
					}
				}
				

				if (c.Items.Count > 0)
				{
					c.Show(this, new Point(e.X, e.Y));
				}

			}

			switch (_dragState)
			{
				case DragState.Moving:
					
					MouseUp_DragMoving(location);
					break;
				case DragState.HResizing:
					MouseUp_HResizing();
					break;
				default:
					if (!CtrlPressed && !ShiftPressed && e.Button != MouseButtons.Right)
					{
						if (_marksSelectionManager.SelectedMarks.Count > 1)
						{
							_lastSingleSelectedMarkLocation = location;
							_marksSelectionManager.ClearSelected();
							_marksSelectionManager.Select(_mouseDownMark);
						}
					}
					EndAllDrag();
					break;
			}

		}

		private void PasteMarks_Click(object sender, EventArgs e)
		{
			var groupedMarks = _clipboard.GroupBy(m => m.Parent).OrderBy(g => _markCollections.IndexOf(g.Key));
			if (!groupedMarks.Any()) return;
			TimeSpan startTime = pixelsToTime(_mouseDownLocation.X);

			var startRowIndex = _rows.IndexOf(RowAt(_mouseDownLocation));
			if (startRowIndex < 0) return;

			List<IMark> pastedMarks = new List<IMark>();
			var offset = _markCollections.IndexOf(groupedMarks.First().Key);

			foreach (IGrouping<IMarkCollection, IMark> groupedMark in groupedMarks)
			{
				var previousIndex = _markCollections.IndexOf(groupedMark.Key);
				var index = startRowIndex + previousIndex - offset;
				if(index > _rows.Count-1) continue;
				var insertRow = _rows[index];

				if (groupedMark.Key.CollectionType != insertRow.MarkCollection.CollectionType)
				{
					var messageBox = new MessageBoxForm($@"Warning, {groupedMark.Key.Name} is of type {groupedMark.Key.CollectionType} and
the target {insertRow.MarkCollection.Name} is of type {insertRow.MarkCollection.CollectionType}. Proceed?", @"Warning", MessageBoxButtons.YesNo, SystemIcons.Warning);
					var response = messageBox.ShowDialog();
					if (response.Equals(DialogResult.No))
					{
						break;
					}
				}

				var orderedMarks = groupedMark.OrderBy(m => m.StartTime);
				var referenceTime = _clipboard.Min(m => m.StartTime);
				var currentTime = startTime;
				var newMarks = new List<IMark>();
				foreach (var orderedMark in orderedMarks)
				{
					IMark mark = (IMark)orderedMark.Clone();
					mark.StartTime = currentTime + (orderedMark.StartTime - referenceTime);
					newMarks.Add(mark);
				}

				insertRow.MarkCollection.AddMarks(newMarks);
				pastedMarks.AddRange(newMarks);
			}

			if (pastedMarks.Any())
			{
				_timeLineGlobalEventManager.OnMarksPasted(new MarksPastedEventArgs(pastedMarks));
				_marksSelectionManager.ClearSelected();
				_marksSelectionManager.Select(pastedMarks);
			}
		}

		private void CopyMarksOnClick(object sender, EventArgs e)
		{
			_clipboard.Clear();
			_clipboard.AddRange(_marksSelectionManager.SelectedMarks);
		}

		private void CutMarksOnClick(object sender, EventArgs e)
		{
			_clipboard.Clear();
			_clipboard.AddRange(_marksSelectionManager.SelectedMarks);
			DeleteSelectedMarks();
		}

		#endregion

		#region Mouse Move

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) return;
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
					if (!_ignoreDragArea.Contains(location) && _mouseDownMark != null)
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
			IMark mark = MarkAt(gridLocation);
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
			if (!_marksSelectionManager.SelectedMarks.Any() || _mouseDownMark == null)
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

			_timeLineGlobalEventManager.OnMarksMoving(new MarksMovingEventArgs(_marksSelectionManager.SelectedMarks.ToList()));
			_timeLineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(true, new[] {_mouseDownMark.StartTime, _mouseDownMark.EndTime }));

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

		private void MouseUp_DragMoving(Point location)
		{
			_lastSingleSelectedMarkLocation = location;
			FinishedResizeMoveMarks(ElementMoveType.Move);
			EndAllDrag();
		}

		private void EndAllDrag()
		{
			_dragState = DragState.Normal;
			Cursor = Cursors.Default;
			_mouseDownMark = null;
			//Invalidate();
		}

		#endregion

		#region Resize

		private void MouseMove_HResizing(Point location)
		{
			if (_mouseDownMark == null) return;
			var moveInfo = _marksMoveResizeInfo; //Make a reference copy so it won't get changed out from under us.
			if(!moveInfo.OriginalMarks.Values.Any()) return;
			TimeSpan dt = pixelsToTime(location.X - _moveResizeStartLocation.X);

			if (dt == TimeSpan.Zero)
				return;

			// Ensure minimum size
			TimeSpan shortest = moveInfo.OriginalMarks.Values.Min(x => x.Duration);
			IMark gluedMark = null;
			var handleGluedMark = AltPressed && _marksSelectionManager.SelectedMarks.Count == 1;
			
			// Check boundary conditions
			switch (_markResizeZone)
			{
				case ResizeZone.Front:

					if (handleGluedMark)
					{
						int index = _mouseDownMark.Parent.Marks.IndexOf(_mouseDownMark);
						if (index > 0)
						{
							gluedMark = _mouseDownMark.Parent.Marks[index - 1];
							shortest = Min(shortest, gluedMark.Duration);
						}
					}

					// Clip earliest element StartTime at zero
					TimeSpan earliest = moveInfo.OriginalMarks.Values.Min(x => x.StartTime);
					if (earliest + dt < TimeSpan.Zero)
					{
						dt = -earliest;
					}

					// Ensure the shortest meets minimum width (in px)
					if (timeToPixels(shortest - dt) < MinMarkWidthPx)
					{
						dt = shortest - pixelsToTime(MinMarkWidthPx);
					}
					_timeLineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(true, new[] { _mouseDownMark.StartTime }));
					break;

				case ResizeZone.Back:

					if (handleGluedMark)
					{
						int index = _mouseDownMark.Parent.Marks.IndexOf(_mouseDownMark);
						if (index < _mouseDownMark.Parent.Marks.Count-1)
						{
							gluedMark = _mouseDownMark.Parent.Marks[index + 1];
							shortest = Min(shortest, gluedMark.Duration);
						}
					}
					// Clip latest mark EndTime at TotalTime
					TimeSpan latest = moveInfo.OriginalMarks.Values.Max(x => x.EndTime);
					if (latest + dt > TimeInfo.TotalTime)
					{
						dt = TimeInfo.TotalTime - latest;
					}

					// Ensure the shortest meets minimum width (in px)
					if (timeToPixels(shortest + dt) < MinMarkWidthPx)
					{
						dt = pixelsToTime(MinMarkWidthPx) - shortest;
					}
					_timeLineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(true, new[] { _mouseDownMark.EndTime }));
					break;
			}


			// Apply dt to all selected elements.
			foreach (var originalMarkInfo in moveInfo.OriginalMarks)
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
			var movedMarks = moveInfo.OriginalMarks.Keys.ToList();

			if (handleGluedMark && gluedMark != null)
			{
				switch (_markResizeZone)
				{
					case ResizeZone.Front:
						gluedMark.Duration = _mouseDownMark.StartTime - gluedMark.StartTime;
						break;

					case ResizeZone.Back:
						var endTime = gluedMark.EndTime;
						gluedMark.StartTime = _mouseDownMark.EndTime;
						gluedMark.Duration = endTime - gluedMark.StartTime;
						break;
				}

				movedMarks.Add(gluedMark);
			}

			_timeLineGlobalEventManager.OnMarksMoving(new MarksMovingEventArgs(movedMarks));
			
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
			_marksMoveResizeInfo = new MarksMoveResizeInfo(_marksSelectionManager.SelectedMarks.ToArray());
		}

		private void FinishedResizeMoveMarks(ElementMoveType type)
		{
			//TODO This selected mark move thing needs help
			_timeLineGlobalEventManager.OnAlignmentActivity(new AlignmentEventArgs(false, null));

			_timeLineGlobalEventManager.OnMarkMoved(new MarksMovedEventArgs(_marksMoveResizeInfo, type));

			_marksMoveResizeInfo = null;
			_moveResizeStartLocation = Point.Empty;
		}

		/// <summary>
		/// Select all elements that are in the Virtual box created by the two diagonal points
		/// </summary>
		/// <param name="startingPoint"></param>
		/// <param name="endingPoint"></param>
		private void SelectMarksBetween(Point startingPoint, Point endingPoint)
		{
			_marksSelectionManager.ClearSelected();
			//find all the elements between us and the last selected
			if (endingPoint.X > startingPoint.X)
			{
				//Right
				if (endingPoint.Y > startingPoint.Y)
				{
					//Below
					SelectElementsWithin(new Rectangle(startingPoint,
						new Size(endingPoint.X - startingPoint.X,
							endingPoint.Y - startingPoint.Y)));
				}
				else
				{
					//Above
					SelectElementsWithin(new Rectangle(startingPoint.X, endingPoint.Y, endingPoint.X - startingPoint.X,
						startingPoint.Y - endingPoint.Y));
				}
			}
			else
			{
				//Left
				if (endingPoint.Y > startingPoint.Y)
				{
					//Below
					SelectElementsWithin(new Rectangle(endingPoint.X, startingPoint.Y, startingPoint.X - endingPoint.X,
						endingPoint.Y - startingPoint.Y));
				}
				else
				{
					//Above
					SelectElementsWithin(new Rectangle(endingPoint,
						new Size(startingPoint.X - endingPoint.X,
							startingPoint.Y - endingPoint.Y)));
				}
			}
		}

		private void SelectElementsWithin(Rectangle selectedArea)
		{
			var startRow = RowAt(selectedArea.Location);
			var endRow = RowAt(selectedArea.BottomRight());

			TimeSpan selStart = pixelsToTime(selectedArea.Left);
			TimeSpan selEnd = pixelsToTime(selectedArea.Right);
			
			// Iterate all elements of only the rows within our selection.
			bool startFound = false, endFound = false;
			int top = 0;
			foreach (var row in _rows.Where(x => x.Visible))
			{
				if (endFound || // we already passed the end row
					(!startFound && (row != startRow)) //we haven't found the first row, and this isn't it
					)
				{
					top += row.Height;
					continue;
				}

				// If we already found the first row, or we haven't, but this is it
				if (startFound || row == startRow)
				{
					startFound = true;

					// This row is in our selection
					foreach (var elem in row)
					{
						var selected = ((elem.StartTime < selEnd && elem.EndTime > selStart) );
						if (selected)
						{
							_marksSelectionManager.Select(elem);
						}
					}

					top += row.Height;

					if (row == endRow)
					{
						endFound = true;
					}
				}
			} // end foreach

		}

		#region Events

		private void TimeLineGlobalEventManagerDeleteTimeLineGlobal(object sender, MarksDeletedEventArgs e)
		{
			Invalidate();
		}

		private void TimeLineGlobalEventManagerTimeLineGlobalMoving(object sender, MarksMovingEventArgs e)
		{
			Invalidate();
		}

		private void MarksSelectionManager_SelectionChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void TimeLineGlobalEventManager_MarksMoved(object sender, MarksMovedEventArgs e)
		{
			Invalidate();
		}

		private void TimeLineGlobalEventManager_MarksTextChanged(object sender, MarksTextChangedEventArgs e)
		{
			Invalidate();
		}

		private ToolStripItem CreatePhonemeMenuItem()
		{
			var phonemes = new[] { "REST", "AI", "E", "ETC", "FV", "L", "MBP", "O", "U", "WQ" };
			ToolStripMenuItem menu = new ToolStripMenuItem("Phoneme");
			foreach (var phoneme in phonemes)
			{
				var item = menu.DropDownItems.Add(phoneme);
				item.Click += PhonemeSelect_Clicked;
				item.Tag = phoneme;
			}
			
			return menu;
		}

		private void PhonemeSelect_Clicked(object sender, EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				List<MarksTextInfo> changedMarks = new List<MarksTextInfo>();
				foreach (var mark in _marksSelectionManager.SelectedMarks)
				{
					var mti = new MarksTextInfo(mark, menuItem.Text, mark.Text);
					changedMarks.Add(mti);
					mark.Text = menuItem.Text;
				}
				TimeLineGlobalEventManager.Manager.OnMarksTextChanged(new MarksTextChangedEventArgs(changedMarks));
			}
			
		}

		private void Rename_Click(object sender, EventArgs e)
		{
			var single = _marksSelectionManager.SelectedMarks.Count == 1;
			TextDialog td = new TextDialog("Enter the new text.", _marksSelectionManager.SelectedMarks.Count==1?"Edit Mark":"Edit Multiple Marks", single?_marksSelectionManager.SelectedMarks.First().Text:string.Empty, single);
			var result = td.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				List<MarksTextInfo> changedMarks = new List<MarksTextInfo>();
				foreach (var mark in _marksSelectionManager.SelectedMarks)
				{
					var mti = new MarksTextInfo(mark, td.Response, mark.Text);
					changedMarks.Add(mti);
					mark.Text = td.Response;
				}
				TimeLineGlobalEventManager.Manager.OnMarksTextChanged(new MarksTextChangedEventArgs(changedMarks));
			}
		}

		private void DeleteMark_Click(object sender, EventArgs e)
		{
			DeleteSelectedMarks();
		}

		private void BreakdownWord_Click(object sender, EventArgs e)
		{
			_timeLineGlobalEventManager.OnPhonemeBreakdownAction(new PhonemeBreakdownEventArgs(_marksSelectionManager.SelectedMarks.ToList(), BreakdownType.Word));
		}

		private void BreakdownPhrase_Click(object sender, EventArgs e)
		{
			_timeLineGlobalEventManager.OnPhonemeBreakdownAction(new PhonemeBreakdownEventArgs(_marksSelectionManager.SelectedMarks.ToList(), BreakdownType.Phrase));
		}

		private void DeleteSelectedMarks()
		{
			foreach (var mark in _marksSelectionManager.SelectedMarks)
			{
				mark.Parent.RemoveMark(mark);
			}

			_timeLineGlobalEventManager.OnDeleteMark(new MarksDeletedEventArgs(_marksSelectionManager.SelectedMarks.ToList()));
		}

		private void Copy_Click(object sender, EventArgs e)
		{
			if (_marksSelectionManager.SelectedMarks.Any())
			{
				var text = _marksSelectionManager.SelectedMarks.First().Text;
				if(!string.IsNullOrEmpty(text))
				{
					Clipboard.SetText(_marksSelectionManager.SelectedMarks.First().Text);
				}
				else
				{
					Clipboard.Clear();
				}
			}
		}

		private void Paste_Click(object sender, EventArgs e)
		{
			var text = Clipboard.GetText(TextDataFormat.Text);
			foreach (var mark in _marksSelectionManager.SelectedMarks)
			{
				mark.Text = text;
			}

			Invalidate();
		}


		#endregion

		private bool CtrlPressed => ModifierKeys.HasFlag(Keys.Control);

		private bool ShiftPressed => ModifierKeys.HasFlag(Keys.Shift);

		private bool AltPressed => ModifierKeys.HasFlag(Keys.Alt);

		/// <summary>
		/// Returns all elements located at the given point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Elements at given point, or null if none exists.</returns>
		private IMark MarkAt(Point p)
		{
			
			// First figure out which row we are in
			MarkRow containingRow = RowAt(p);

			if (containingRow == null)
				return null;

			var pointTime = pixelsToTime(p.X);
			// Now figure out which element we are on
			foreach (IMark mark in containingRow)
			{
				if (pointTime > mark.EndTime) continue; //Mark is before our point time.
				if (pointTime < mark.StartTime) break; //The rest of them are beyond our point.
				Single x = timeToPixels(mark.StartTime);
				Single width = timeToPixels(mark.Duration);
				MarkRow.MarkStack ms = containingRow.GetStackForMark(mark);
				var displayHeight = ms.StackCount == 1?containingRow.Height:containingRow.Height / containingRow.StackCount;
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

		private bool MarkCollectionAt(Point p, out IMarkCollection markCollection)
		{
			bool success = false;
			markCollection = null;
			var row = RowAt(p);
			if (row != null)
			{
				markCollection = row.MarkCollection;
				success = true;
			}

			return success;
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
			foreach (MarkRow row in _rows.Where(x => x.Visible))
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
			//As in the ruler, looks a lot better
			Refresh();
		}

		private void CalculateHeight()
		{
			Height = _rows.Where(x => x.Visible).Sum(x => x.Height);
		}

		public static TimeSpan Min(TimeSpan val1, TimeSpan val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			
			return val1;
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
				var messageBox = new MessageBoxForm("Exception in Timeline.MarksBar.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace,
					@"Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
			}
		}

		private void DrawRows(Graphics g)
		{
			int curY = 0;
			var start = (int)timeToPixels(VisibleTimeStart);
			var end = (int)timeToPixels(VisibleTimeEnd);
			foreach (var markRow in _rows.Where(x => x.Visible))
			{
				markRow.SetStackIndexes(VisibleTimeStart, VisibleTimeEnd);
			}
			CalculateHeight();
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
				using (Brush b = new SolidBrush(row.MarkDecorator.Color))
				{
					for (int i = 0; i < row.MarksCount; i++)
					{
						var currentElement = row.GetMarkAtIndex(i);
						if (currentElement.EndTime < VisibleTimeStart)
							continue;

						if (currentElement.StartTime > VisibleTimeEnd)
						{
							break;
						}

						DrawMark(g, row, currentElement, displaytop, b);
					}
				}

				displaytop += row.Height;
			}

		}

		private static readonly Pen MarkBorderPen = new Pen(Color.Black);
		private static readonly StringFormat DrawFormat = StringFormat.GenericDefault;
		private static readonly SolidBrush TextBrush = new SolidBrush(Color.Black);
		private void DrawMark(Graphics g, MarkRow row, IMark mark, int top, Brush b)
		{
			//Sanity check - it is possible for .DisplayHeight to become zero if there are too many marks stacked.
			//We set the DisplayHeight to the row height for the mark, and change the border to red.	
			var markStack = row.GetStackForMark(mark);
			var displayHeight =
				(markStack.StackCount > 1) ? ((row.Height - 2) / row.StackCount) : row.Height - 2;

			var displayTop = top + displayHeight * markStack.StackIndex;
			
			if (displayHeight == 0)
			{
				displayHeight = row.Height;
			}

			var width = (int)timeToPixels(mark.Duration);
			if (width <= 0) return;
			Size size = new Size(width, displayHeight);

			Point finalDrawLocation = new Point((int)Math.Floor(timeToPixels(mark.StartTime)), displayTop);

			Rectangle destRect = new Rectangle(finalDrawLocation.X, finalDrawLocation.Y, size.Width, displayHeight);
			g.FillRectangle(b,destRect);
			
			var isSelected = _marksSelectionManager.SelectedMarks.Contains(mark);

			MarkBorderPen.Width = isSelected ? 3 : 1;
			g.DrawRectangle(MarkBorderPen, destRect);
			
			if (isSelected)
			{
				using (Pen bp = new Pen(ThemeColorTable.ForeColor,1))
				{
					bp.Alignment = PenAlignment.Inset;
					bp.DashPattern = new [] { 1.0F, 2.0F };
					g.DrawRectangle(bp, destRect);
				}
			}
			
			//Draw the text
			TextBrush.Color = IdealTextColor(row.MarkDecorator.Color);
			g.DrawString(mark.Text, _textFont, TextBrush, destRect, DrawFormat);

		}

		public Color IdealTextColor(Color bg)
		{
			int threshold = 105;
			int bgDelta = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) +
			                              (bg.B * 0.114));

			Color foreColor = (255 - bgDelta <= threshold) ? Color.Black : Color.White;
			return foreColor;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_timeLineGlobalEventManager.MarksMoving -= TimeLineGlobalEventManagerTimeLineGlobalMoving;
				_timeLineGlobalEventManager.DeleteMark -= TimeLineGlobalEventManagerDeleteTimeLineGlobal;
				_timeLineGlobalEventManager.MarksTextChanged -= TimeLineGlobalEventManager_MarksTextChanged;
				_timeLineGlobalEventManager.MarksMoved -= TimeLineGlobalEventManager_MarksMoved;
				_marksSelectionManager.SelectionChanged -= MarksSelectionManager_SelectionChanged;
				MarkRow.MarkRowChanged -= MarkRow_MarkRowChanged;

				UnConfigureMarks();
			}
			base.Dispose(disposing);
		}
	}
}
