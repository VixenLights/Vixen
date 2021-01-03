using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using NLog;
using Vixen;
using Vixen.Execution.Context;
using Vixen.Marks;
using Vixen.Sys.LayerMixing;
using VixenModules.App.Marks;

namespace Common.Controls.Timeline
{
	/// <summary>
	/// Makes up the main part of the TimelineControl. A scrollable container which presents rows which contain elements.
	/// </summary>
	[DesignerCategory("")] // Prevent this from showing up in designer.
	public partial class Grid : TimelineControlBase, IEnumerable<Row>
	{
		#region Members
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private List<Row> m_rows; // the rows in the grid
		private DragState m_dragState = DragState.Normal; // the current dragging state

		private Point m_selectionRectangleStart; // the location (on the grid canvas) where the selection box starts.
		private Point m_drawingRectangleStart; // the location (on the grid canvas) where the drawing box starts.
		private Rectangle m_ignoreDragArea; // the area in which move movements should be ignored, before we start dragging
		private Point m_waitingBeginGridLocation;
		private List<Element> m_mouseDownElements; // the elements under the cursor on a mouse click
		private Point m_lastSingleSelectedElementLocation;
		private Row m_mouseDownElementRow = null;
		            // the row that the clicked m_mouseDownElement belongs to (a single element may be in multiple rows)

		private BackgroundWorker renderWorker;
		private BlockingCollection<Element> _blockingElementQueue = new BlockingCollection<Element>();
		private ManualResetEventSlim renderWorkerFinished;
		
		#endregion
		private ElementMoveInfo m_elemMoveInfo;
		public ISequenceContext Context = null;
		public bool SequenceLoading { get; set; }
		public Element _workingElement; //This is the element that was left clicked, is set to null on mouse up
		public bool isColorDrop { get; set; }
		public bool isCurveDrop { get; set; }
		public bool isGradientDrop { get; set; }
		public string alignmentHelperWarning = @"Too many effects selected on the same row for this action.\nMax selected effects per row for this action is 4";
		public bool aCadStyleSelectionBox { get; set; }

		private List<Row> _visibleRows = new List<Row>();
		private bool _visibleRowsDirty = false;

		//We turn these into snap points for effects.
		private ObservableCollection<IMarkCollection> _markCollections;

		#region Initialization

		public Grid(TimeInfo timeinfo)
			: base(timeinfo)
		{
			AutoScroll = true;
			AllowGridResize = true;
			AutoScrollMargin = new Size(24, 24);
			TotalTime = TimeSpan.FromMinutes(1);
			RowSeparatorColor = ThemeColorTable.TimeLineGridColor;
			MajorGridlineColor = ThemeColorTable.TimeLineGridColor;
			GridlineInterval = TimeSpan.FromSeconds(1.0);
			BackColor = ThemeColorTable.TimeLineEffectsBackColor;
			SelectionColor = Color.FromArgb(100, 40, 100, 160);
			SelectionBorder = Color.Blue;
			DrawColor = Color.FromArgb(100, 255, 255, 255);
			DrawBorder = Color.Black;
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
			Row.RowVisibilityChanged += RowVisibilityChangedHandler;
			TimeLineGlobalEventManager.Manager.AlignmentActivity += TimeLineAlignmentHandler;

			// Drag & Drop
			AllowDrop = true;
			//DragEnter += TimelineGrid_DragEnter;
			//DragDrop += TimelineGrid_DragDrop;
			//DragOver += TimelineGrid_DragOver;
			StartBackgroundRendering();
			CurrentDragSnapPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>();
			EnableSnapTo = true;
			SnapStrength = 2;
		}

		

		protected override void Dispose(bool disposing)
		{
			// Cancel/complete the rendering background worker
			_blockingElementQueue.CompleteAdding();

			if (renderWorker != null)
			{
				// wait up to a few seconds for it to finish rendering
				renderWorkerFinished.Wait(5000);

				if (!renderWorkerFinished.IsSet) {
					Logging.Error("Grid: background rendering didn't finish. Forcibly killing.");
					renderWorker.CancelAsync();
					// this really shouldn't be using DoEvents. Actually, it shouldn't be a background worker: we're
					// treating it like a thread, so should probably just _make_ it a thread.
					while (renderWorker.IsBusy) {
						Application.DoEvents();
					}
				}
			}

			if (m_rows != null) {
				m_rows.Clear();
				m_rows=null;
				m_rows=new List<Row>();
			}

			Row.RowChanged -= RowChangedHandler;
			Row.RowSelectedChanged -= RowSelectedChangedHandler;
			Row.RowToggled -= RowToggledHandler;
			Row.RowHeightChanged -= RowHeightChangedHandler;
			TimeLineGlobalEventManager.Manager.AlignmentActivity -= TimeLineAlignmentHandler;

			TimeInfo= null;

			_blockingElementQueue.Dispose();
			_blockingElementQueue= null;
			Context=null;

			UnConfigureMarks();
			
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
            //ResetAllElements();
			base.OnTimePerPixelChanged(sender, e);
		}


		protected override void OnTotalTimeChanged(object sender, EventArgs e)
		{
			ResizeGridHorizontally();
			base.OnTotalTimeChanged(sender, e);
		}


		private void ResizeGridHorizontally()
		{
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(ResizeGridHorizontally));
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

		public bool ShowEffectToolTip { get; set; } = true;

		public bool EnableSnapTo { get; set; }

		public int SnapStrength
		{
			get => _snapStrength;
			set
			{
				_snapStrength = value;
				CreateSnapPointsFromMarks();
			}
		}

		public string CloseGap_Threshold { get; set; }
		
		public bool ResizeIndicator_Enabled { get; set; }

		public string ResizeIndicator_Color { get; set; }

		public bool SuppressInvalidate { get; set; }

		public bool SupressRendering { get; set; }

		public bool SupressSelectionEvents { get; set; }

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

		/// <summary>
		/// Returns the Rows in the grid
		/// </summary>
		public List<Row> Rows
		{
			get { return m_rows; }
			protected set { m_rows = value; }
		}

		public void HighlightRowsWithEffects(bool enable)
		{
			RowLabel.ShowActiveIndicators = enable;
			InvalidateRowLabels();
		}

		/// <summary>
		/// Invalidates the Visible row labels so they will be redrawn.
		/// </summary>
		internal void InvalidateRowLabels()
		{
			foreach (var gridVisibleRow in VisibleRows)
			{
				gridVisibleRow.RowLabel.Invalidate();
			}
		}

		private IEnumerable<Element> _selectedElements;
		public IEnumerable<Element> SelectedElements
		{
			get
			{
				if (_selectedElements == null)
				{
					_selectedElements = Rows.SelectMany(x => x.SelectedElements).Distinct().ToList();
				}

				return _selectedElements;
			}
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

		public Row FirstSelectedRow { get; set; }

		/// <summary>
		/// Get the active row the user is working on.
		/// </summary>
		public Row ActiveRow
		{
			get { return Rows.FirstOrDefault(x => x.Active); }
		}

		public List<Row> VisibleRows
		{
			get
			{
				if (_visibleRowsDirty)
				{
					_visibleRows = Rows.Where(x => x.Visible).ToList();
					_visibleRowsDirty = false;
				}
				return _visibleRows;
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

		private TimeSpan CursorPosition
		{
			get => TimeLineGlobalStateManager.Manager.CursorPosition;
			set
			{
				TimeLineGlobalStateManager.Manager.CursorPosition = value;
				Invalidate();
			}
		}

		//Drag Box Filter stuff
		public bool DragBoxFilterEnabled { get; set; }
		public List<Guid> DragBoxFilterTypes = new List<Guid>();
		
		public TimeSpan GridlineInterval { get; set; }
		public bool OnlySnapToCurrentRow { get; set; }
		public int SnapPriorityForElements { get; set; }
		public int DragThreshold { get; set; }
		public Rectangle SelectionArea { get; set; }
		public Rectangle DrawingArea { get; set; }
		public bool ClickingGridSetsCursor { get; set; }

		// drawing colours, information, etc.
		public Color RowSeparatorColor { get; set; }
		public Color MajorGridlineColor { get; set; }
		public Color SelectionColor { get; set; }
		public Color SelectionBorder { get; set; }
		public Color DrawColor { get; set; }
		public Color DrawBorder { get; set; }
		public static Color CursorColor { get; set; }
		public static Single CursorWidth { get; set; }

		// private properties
		private bool CtrlPressed
		{
			get { return ModifierKeys.HasFlag(Keys.Control); }
		}

		private bool ShiftPressed
		{
			get { return ModifierKeys.HasFlag(Keys.Shift); }
		}

		private bool AltPressed
		{
			get { return ModifierKeys.HasFlag(Keys.Alt); }
		}

		public bool IsResizeDragInProgress => m_dragState != DragState.Normal;

		private int CurrentRowIndexUnderMouse { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> StaticSnapPoints { get; set; }
		private SortedDictionary<TimeSpan, List<SnapDetails>> CurrentDragSnapPoints { get; set; }

		private IEnumerable<TimeSpan> MarkAlignmentPoints { get; set; } = Enumerable.Empty<TimeSpan>();
		private List<Element> tempSelectedElements = new List<Element>();

		public SequenceLayers SequenceLayers { get; set; }

		#endregion

		#region Events

		public event EventHandler SelectionChanged;
		public event EventHandler<ElementEventArgs> ElementDoubleClicked;
		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving;
		
		public event EventHandler VerticalOffsetChanged;
		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows;
		public event EventHandler<ElementsSelectedEventArgs> ElementsSelected;
		public event EventHandler<ContextSelectedEventArgs> ContextSelected;
		public event EventHandler<RenderElementEventArgs> RenderProgressChanged;

		private void _SelectionChanged()
		{
			_selectedElements = null;
			if (SupressSelectionEvents) return;
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

		// returns true if the selection should be automatically handled by the grid, or false if
		// another event handler will handle the selection process
		private bool _ContextSelected(IEnumerable<Element> elements, TimeSpan gridTime, Row row)
		{
			if (elements == null)
				return true;

			if (ContextSelected != null)
			{
				ContextSelectedEventArgs args = new ContextSelectedEventArgs(elements, gridTime, row);
				ContextSelected(this, args);
				return args.AutomaticallyHandleSelection;
			}
			return true;
		}

		private void _RenderProgressChanged(int percent)
		{
			if (InvokeRequired) {
				//this.Invoke(new MethodInvoker(_RenderProgressChanged));
				BeginInvoke((MethodInvoker) (() => _RenderProgressChanged(percent)));

			} else {
				if (RenderProgressChanged != null) {
					RenderProgressChanged(this, new RenderElementEventArgs(percent));
				}
			}
		}

		#endregion

		#region Event Handlers - non-mouse events

		private void TimeLineAlignmentHandler(object sender, AlignmentEventArgs e)
		{
			Logging.Info($"Alignment event {e.Active}");
			MarkAlignmentPoints = e.Times ?? Enumerable.Empty<TimeSpan>();
		}

		protected void RowChangedHandler(object sender, EventArgs e)
		{
			// when dragging, the control will invalidate after it's done, in case multiple elements are changing.
			if (m_dragState != DragState.Moving && !SequenceLoading)
				if (!SuppressInvalidate) Invalidate();
		}

		protected void RowVisibilityChangedHandler(object sender, EventArgs e)
		{
			_visibleRowsDirty = true;
		}

		protected void RowSelectedChangedHandler(object sender, ModifierKeysEventArgs e)
		{
			Row selectedRow = sender as Row;
			//Handle full selection logic
			if (e.ModifierKeys.HasFlag(Keys.Control) || e.ModifierKeys.HasFlag(Keys.Shift))
			{
				if (e.ModifierKeys.HasFlag(Keys.Control) || !SelectedRows.Any())
				{
					if (selectedRow.Selected)
					{
						if (DragBoxFilterEnabled)
						{
							foreach (Element element in selectedRow)
							{
								element.Selected = DragBoxFilterTypes.Contains(element.EffectNode.Effect.TypeId);
							}
						}
						else
						{
							selectedRow.SelectAllElements();
						}
						FirstSelectedRow = selectedRow;
					} else
					{
						if (!SelectedRows.Any())
						{
							FirstSelectedRow = null;
						}
						selectedRow.DeselectAllElements();
					}
				} else
				{
					//Multi select.
					int indexFirst = Rows.IndexOf(FirstSelectedRow);
					int indexSelected = Rows.IndexOf(selectedRow);
					if (indexSelected > indexFirst) //Selecting down in the grid
					{
						for (int i = indexFirst; i <= indexSelected; i++)
						{
							if (Rows[i].Visible)
							{
								Rows[i].Selected = true;
								if (DragBoxFilterEnabled)
								{
									foreach (Element element in Rows[i])
									{
										element.Selected = DragBoxFilterTypes.Contains(element.EffectNode.Effect.TypeId);
									}
								}
								else
								{
									Rows[i].SelectAllElements();									
								}
							}
						}
					}else{	
						for(int i=indexSelected; i <= indexFirst; i++){
							if (Rows[i].Visible)
							{
								Rows[i].Selected = true;
								if (DragBoxFilterEnabled)
								{
									foreach (Element element in Rows[i])
									{
										element.Selected = DragBoxFilterTypes.Contains(element.EffectNode.Effect.TypeId);
									}
								}
								else
								{
									Rows[i].SelectAllElements();
								}
							}
						}
					}
				}

			} else
			{
				ClearSelectedElements();
				ClearSelectedRows(selectedRow);
				ClearActiveRows();
				if (DragBoxFilterEnabled)
				{
					foreach (Element element in selectedRow)
					{
						element.Selected = DragBoxFilterTypes.Contains(element.EffectNode.Effect.TypeId);
					}
				}
				else
				{
					selectedRow.SelectAllElements();					
				}
				FirstSelectedRow = selectedRow;
			}

			_SelectionChanged();
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
 
			if (!SuppressInvalidate) Invalidate();
		}

		

		private void RowHeightChangedHandler(object sender, EventArgs e)
		{
			ResizeGridHeight();
			if (!SuppressInvalidate) Invalidate();
		}

        #endregion

        #region Methods - Rows, Elements

        public IEnumerator<Row> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public delegate List<Element> CloneSelectedElementsDelegate(IEnumerable<Element> elements);

		public CloneSelectedElementsDelegate SelectedElementsCloneDelegate { get; set; }

		protected void CloneSelectedElementsForMove()
		{
			List<Element> elements = SelectedElementsCloneDelegate.Invoke(SelectedElements);
			ClearSelectedElements();
			foreach (var element in elements)
			{
				element.Selected = true;
			}
			_SelectionChanged();
		}

		/// <summary>
		/// Clears all the selected flags on the currently selected elements.
		/// </summary>
		public void ClearSelectedElements()
		{
			foreach (Element te in SelectedElements.ToArray())
				te.Selected = false;
			Invalidate();
			_SelectionChanged();
		}

		/// <summary>
		/// Clears all the selected rows. Optional row to be excluded.
		/// </summary>
		/// <param name="leaveRowSelected">Optional row will be left alone.</param>
		public void ClearSelectedRows(Row leaveRowSelected = null)
		{
			foreach (Row row in Rows) {
				if (row != leaveRowSelected)
					row.Selected = false;
			}
			Invalidate();
			_SelectionChanged();
		}

		/// <summary>
		/// Clears the Active row flag on all rows. Optional row to be excluded.
		/// </summary>
		/// <param name="leaveRowActive"></param>
		public void ClearActiveRows(Row leaveRowActive = null)
		{
			foreach (Row row in Rows)
			{
				if (row != leaveRowActive)
					row.Active = false;
			}
			Invalidate();
			//_SelectionChanged();
		}

		/// <summary>
		/// Add a row to the Grid
		/// </summary>
		/// <param name="row"></param>
		public void AddRow(Row row)
		{
			Rows.Add(row);
			ResizeGridHeight();
			if (!SuppressInvalidate) Invalidate();
		}

		/// <summary>
		/// Remove a specific Row from the Grid
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public bool RemoveRow(Row row)
		{
			bool rv = Rows.Remove(row);
			ResizeGridHeight();
			if (!SuppressInvalidate) Invalidate();
			return rv;
		}

		//Determines if the count of selected elements per row is an acecptable level for use by the alignment helpers
		public bool OkToUseAlignmentHelper(IEnumerable<Element> elements)
		{

			foreach (Element elem in elements)
			{
				if (elem.Row.SelectedElements.Count() > 32)
					return false;
			}
			return true;
		}

		//Locates and Displays selected effects on the Find Effects Form
		public void DisplaySelectedEffects(List<Element> elements)
		{
			//int rowVerticleOffset = 0;
			if (!elements.Any()) return;
			foreach (var element in elements)
			{
				element.Selected = true;
				element.Row.Visible = true;

				//Make selected effect and any Parent nodes visible and Tree expanded.
				Row parent = element.Row.ParentRow;
				while (parent != null)
				{
					parent.TreeOpen = true;
					parent.Visible = true;
					parent = parent.ParentRow;	
				} 
				element.EndUpdate();
			}

			var lastElement = elements.Last();
			VisibleTimeStart = lastElement.StartTime; //Adjusts the Horixontal Start Time position so the last selected effect is visible
			VerticalOffset = lastElement.Row.DisplayTop; //Adjust the vertical grid position so the last selected effect is visible.
			_SelectionChanged(); //Ensures Effect editor docker is updated with the Selected effects.
			Refresh();
		}

		/// <summary>
		/// Aligns the elements start times to the reference element as a single atomic operation
		/// </summary>
		/// <param name="elements">The elements to align the start times</param>
		/// <param name="referenceElement">The element to use for the start time reference</param>
		/// <param name="holdDuration">Lock the durations</param>
		public void AlignElementStartTimes(IEnumerable<Element> elements, Element referenceElement, bool holdDuration)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.StartTime == referenceElement.StartTime) continue;
				//If elements end time is before or the same as the reference start time, just move the element, otherwise element becomes invalid
				if (selectedElement.EndTime < referenceElement.StartTime || selectedElement.EndTime == referenceElement.StartTime)
				{
					elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(referenceElement.StartTime, referenceElement.StartTime + selectedElement.Duration));
					continue;
				}
				elementsToAlign.Add(selectedElement,
					holdDuration
						? new Tuple<TimeSpan, TimeSpan>(referenceElement.StartTime, referenceElement.StartTime + selectedElement.Duration)
						: new Tuple<TimeSpan, TimeSpan>(referenceElement.StartTime, selectedElement.EndTime));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignStart);
		}

		/// <summary>
		/// Aligns the elements end times to the reference element as a single atomic operation
		/// </summary>
		/// <param name="elements">The elements to align the start times</param>
		/// <param name="referenceElement">The element to use for the start time reference</param>
		/// <param name="holdDuration">Lock the durations</param>
		public void AlignElementEndTimes(IEnumerable<Element> elements, Element referenceElement, bool holdDuration)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.EndTime == referenceElement.EndTime) continue;
				//If elements end time is before or the same as the reference start time, just move the element, otherwise element becomes invalid
				if (selectedElement.StartTime > referenceElement.EndTime || selectedElement.StartTime == referenceElement.EndTime)
				{
					elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(referenceElement.EndTime - selectedElement.Duration, referenceElement.EndTime));
					continue;
				}
				elementsToAlign.Add(selectedElement,
					holdDuration
						? new Tuple<TimeSpan, TimeSpan>(referenceElement.EndTime - selectedElement.Duration, referenceElement.EndTime)
						: new Tuple<TimeSpan, TimeSpan>(selectedElement.StartTime, referenceElement.EndTime));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignEnd);
		}

		/// <summary>
		/// Aligns the elements Durations to the reference element by extending the end time or optionally the start time as a single atomic operation
		/// </summary>
		/// <param name="elements">The elements to align the start times</param>
		/// <param name="referenceElement">The element to use for the start time reference</param>
		/// <param name="holdEndTime">Lock the end times and extend the start time</param>
		public void AlignElementDurations(IEnumerable<Element> elements, Element referenceElement, bool holdEndTime)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.Duration == referenceElement.Duration) continue;
				elementsToAlign.Add(selectedElement,
					holdEndTime
						? new Tuple<TimeSpan, TimeSpan>(selectedElement.EndTime - referenceElement.Duration, selectedElement.EndTime)
						: new Tuple<TimeSpan, TimeSpan>(selectedElement.StartTime, selectedElement.StartTime + referenceElement.Duration));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignDurations);
		}

		/// <summary>
		/// Aligns the elements start and end times to the reference element as a single atomic operation
		/// </summary>
		/// <param name="elements">The elements to align the start times</param>
		/// <param name="referenceElement">The element to use for the start time reference</param>
		public void AlignElementStartEndTimes(IEnumerable<Element> elements, Element referenceElement)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.StartTime == referenceElement.StartTime && selectedElement.EndTime == referenceElement.EndTime) continue;
				elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(referenceElement.StartTime, referenceElement.EndTime));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignBoth);
		}

		/// <summary>
		/// Align the start times to the end time of the elements to the referenced element as a single atomic operation
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="referenceElement"></param>
		/// <param name="holdEndTime"></param>
		public void AlignElementStartToEndTimes(IEnumerable<Element> elements, Element referenceElement, bool holdEndTime)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.EndTime == referenceElement.EndTime) continue;

				var startTime = referenceElement.EndTime;
				var endTime = holdEndTime ? selectedElement.EndTime : startTime + selectedElement.Duration;
				if (endTime - startTime < TimeSpan.FromSeconds(.05)) endTime = startTime + selectedElement.Duration;
				if (endTime > TotalTime) endTime = TotalTime;

				elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignStartToEnd);
		}

		/// <summary>
		/// Align the start times to the end time of the elements to referenced element as a single atomic operation
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="referenceElement"></param>
		/// <param name="holdStartTime"></param>
		public void AlignElementEndToStartTime(IEnumerable<Element> elements, Element referenceElement, bool holdStartTime)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.StartTime == referenceElement.StartTime) continue;

				var endTime = referenceElement.StartTime;
				var startTime = holdStartTime ? selectedElement.StartTime : endTime - selectedElement.Duration;
				if (endTime - startTime < TimeSpan.FromSeconds(.05)) startTime = endTime - selectedElement.Duration;
				if (startTime < TimeSpan.Zero) startTime = TimeSpan.Zero;
				
				elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignEndToStart);
		}

		/// <summary>
		/// Align the center lines of the elements to the referenced element as a single atomic operation
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="referenceElement"></param>
		public void AlignElementCenters(IEnumerable<Element> elements, Element referenceElement)
		{
			if (!OkToUseAlignmentHelper(elements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(alignmentHelperWarning,
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var centerPoint = referenceElement.StartTime.TotalSeconds + (referenceElement.Duration.TotalSeconds / 2);
			var elementsToAlign = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			foreach (Element selectedElement in elements)
			{
				if (selectedElement.StartTime == referenceElement.StartTime) continue;
				var thisStartTime = centerPoint - (selectedElement.Duration.TotalSeconds / 2);
				elementsToAlign.Add(selectedElement, new Tuple<TimeSpan, TimeSpan>(TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisStartTime) + selectedElement.Duration));
			}

			MoveResizeElements(elementsToAlign, ElementMoveType.AlignCenters);
		}

		/// <summary>
		/// Splits the given elements at the given time as clones.
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="splitTime"></param>
		public void SplitElementsAtTime(List<Element> elements, TimeSpan splitTime)
		{
			//Clone the elements
			List<Element> newElements = SelectedElementsCloneDelegate.Invoke(elements);

			//Adjust the end time of the elements
			elements.ForEach(elem => MoveResizeElement(elem, elem.StartTime, splitTime - elem.StartTime));

			//Adjust the start time of the new elements
			newElements.ForEach(elem => MoveResizeElement(elem, splitTime, elem.EndTime - splitTime));

		}

		public void SplitSelectedElementsAtMouseLocation()
		{
			if (SelectedElements.Any())
			{
				var time = TimeAtPosition(MousePosition);
				if(VisibleTimeStart < time && time < VisibleTimeEnd)
				{
					SplitElementsAtTime(SelectedElements.Where(elem => elem.StartTime < time && elem.EndTime > time)
						.ToList(), time);
				}
			}
			
		}

		/// <summary>
		/// Closes the gap between elements in which the gap is less than the set threshold - time in seconds.
		/// If any effects are selected, this applies to only selected effects, otherwise it applies to all elements
		/// of the sequence.
		/// </summary>
		public void CloseGapsBetweenElements()
		{
			List<Row> processRows;
			if (!SelectedElements.Any())
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("No effects have been selected and action will be applied to your entire sequence. This can take a considerable length of time, are you sure ?",
					@"Close element gaps", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No) return;
				processRows = Rows;
			}
			else
			{
				processRows = VisibleRows;
			}

			Dictionary<Element, Tuple<TimeSpan, TimeSpan>> moveElements = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();

			foreach (Row row in processRows)
			{
				List<Element> elements = new List<Element>();
				elements = SelectedElements.Any() ? row.SelectedElements.ToList() : row.ToList();

				if (!elements.Any()) continue;

				Element activeElement = elements.First();
				foreach (Element element in elements.Skip(1))
				{
					//NOTE: we check for duplicate entries because the same row can be a member of more than one group
					//in which case the element can also exist more than once, but we only need to modify one instance of it to get them all.
					if (element.StartTime.TotalSeconds - activeElement.EndTime.TotalSeconds < Convert.ToDouble(CloseGap_Threshold) && element.StartTime != activeElement.EndTime && !moveElements.ContainsKey(element))
					{
						moveElements.Add(element,
							new Tuple<TimeSpan, TimeSpan>(activeElement.EndTime, element.EndTime));
					}

					activeElement = element;
				}
			}

			MoveResizeElements(moveElements);
		}

		/// <summary>
		/// Move/Resize a group of elements as an atomic operation
		/// </summary>
		/// <param name="elements">List of elements with tuple of new start time, new end time</param>
		/// <param name="moveType">Optional move type. Defaults to move.</param>
		public void MoveResizeElements(Dictionary<Element, Tuple<TimeSpan, TimeSpan>> elements, ElementMoveType moveType = ElementMoveType.Move)
		{
			m_elemMoveInfo = new ElementMoveInfo(new Point(), elements.Keys , VisibleTimeStart);
			foreach (KeyValuePair<Element, Tuple<TimeSpan, TimeSpan>> elementInfo in elements)
			{
				if (elementInfo.Key == null || elementInfo.Value.Item2 > TotalTime
				|| elementInfo.Value.Item2 - elementInfo.Value.Item1 < TimeSpan.FromMilliseconds(1))
				{
					continue;  // Skip elements that are not valid for move resize.
				}

				elementInfo.Key.BeginUpdate();
				elementInfo.Key.StartTime = elementInfo.Value.Item1;
				elementInfo.Key.EndTime = elementInfo.Value.Item2;
				elementInfo.Key.EndUpdate();
				RenderElement(elementInfo.Key);
					
			}

			MultiElementEventArgs evargs = new MultiElementEventArgs { Elements = elements.Keys };
			_ElementsFinishedMoving(evargs);

			if (ElementsMovedNew != null)
				ElementsMovedNew(this, new ElementsChangedTimesEventArgs(m_elemMoveInfo, moveType));

			m_elemMoveInfo = null;
		}

		/// <summary>
		/// Handles moving/resizing a single element.
		/// it's a single 'atomic' operation that moves the elements and raises an event to indicate they have moved.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="start"></param>
		/// <param name="duration"></param>
		/// <param name="moveType">Optional move type. Defaults to Move</param>
		/// <returns>Boolen indicating whether the move occurred</returns>
		public bool MoveResizeElement(Element element, TimeSpan start, TimeSpan duration, ElementMoveType moveType = ElementMoveType.Move)
		{
			if (element == null || start > TotalTime || start + duration > TotalTime 
				|| duration < TimeSpan.FromMilliseconds(1))
			{
				return false;
			}

			m_elemMoveInfo = new ElementMoveInfo(new Point(), new List<Element>{element}, VisibleTimeStart);
			element.BeginUpdate();
			element.StartTime = start;
			element.Duration = duration;
			element.EndUpdate();
			RenderElement(element);

			if (ElementsMovedNew != null)
				ElementsMovedNew(this, new ElementsChangedTimesEventArgs(m_elemMoveInfo, moveType));
			
			m_elemMoveInfo = null;

			return true;
			 
		}

		/// <summary>
		/// Handles moving/resizing a single element.
		/// it's a single 'atomic' operation that moves the elements and raises an event to indicate they have moved.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns>Boolen indicating whether the move occurred</returns>
		public bool MoveResizeElementByStartEnd(Element element, TimeSpan start, TimeSpan end)
		{
			if (element == null || start > TotalTime || end > TotalTime || start >= end)
			{
				return false;
			}

			TimeSpan duration = end - start;
			return MoveResizeElement(element, start, duration);

		}

		/// <summary>
		/// Moves the discovered elements within a range by the given amount of time. This is similar to the mouse dragging events, except
		/// it's a single 'atomic' operation that moves the elements and raises an event to indicate they have moved.
		/// </summary>
		public void MoveElementsInRangeByTime(TimeSpan startTime, TimeSpan endTime, TimeSpan offset, bool processVisibleRows)
		{
			IEnumerable<Element> elementsToMove = ElementsWithinRange(startTime, endTime, processVisibleRows);
			MoveElementsByTime(elementsToMove, offset);
		}

		/// <summary>
		/// Moves the given elements by the given amount of time. This is similar to the mouse dragging events, except
		/// it's a single 'atomic' operation that moves the elements and raises an event to indicate they have moved.
		/// Note that it does not utilize snap points at all.
		/// </summary>
		public void MoveElementsByTime(IEnumerable<Element> elements, TimeSpan offset)
		{
			if (!elements.Any())
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
				RenderElement(elem);
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
			foreach (Row row in VisibleRows) {
				if (p.Y < curheight + row.Height) {
					containingRow = row;
					break;
				}
				curheight += row.Height;
			}

			return containingRow;
		}

		protected List<Tuple<Row, int>> RowsIn(Rectangle r)
		{
			List<Tuple<Row,int>> containingRows= new List<Tuple<Row, int>>();
			int currentHeight = 0;
			var areaBottomY = r.Y + r.Height;
			foreach (Row row in VisibleRows)
			{
				if (r.Y < currentHeight + row.Height) {
					containingRows.Add(new Tuple<Row, int>(row, currentHeight));
				}
				currentHeight += row.Height;
				if (currentHeight > areaBottomY)
				{
					break;
				}
			}

			return containingRows;
		}

		public Element ElementAtPosition(Point p)
		{
			Point client = PointToClient(p);
			Point gridPoint = TranslateLocation(client);
			return elementAt(gridPoint);
		}

		public Row RowAtPosition(Point p)
		{
			Point client = PointToClient(p);
			Point gridPoint = TranslateLocation(client);
			return rowAt(gridPoint);
		}

		public Point GridPoint(Point p)
		{
			Point client = PointToClient(p);
			return TranslateLocation(client);
		}

		public TimeSpan TimeAtPosition(Point p)
		{
			return PixelsToTime(GridPoint(p).X);
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
				if (p.X >= elemX &&
					p.X <= elemX + elemW &&
					p.Y >= containingRow.DisplayTop + elem.RowTopOffset &&
					p.Y < containingRow.DisplayTop + elem.RowTopOffset + elem.DisplayHeight)
				{
					//Single elemX = timeToPixels(elem.StartTime);
					//Single elemW = timeToPixels(elem.Duration);
					//if (p.X >= elemX && p.X <= elemX + elemW)
					return elem;
				}
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
				if (elemX > p.X) break; //The rest of them are beyond our point.
				Single elemW = timeToPixels(elem.Duration);
				if (p.X >= elemX &&
					p.X <= elemX + elemW &&
					p.Y >= containingRow.DisplayTop + elem.RowTopOffset &&
					p.Y < containingRow.DisplayTop + elem.RowTopOffset + elem.DisplayHeight)
				{
					result.Add(elem);
				}
			}

			return result;
		}

		protected Row RowContainingElement(Element element)
		{
			return Rows.FirstOrDefault(row => row.ContainsElement(element));
		}

		/// <summary>
		/// Get a list of elements the exist within the the start and end time inclusive.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public IEnumerable<Element> ElementsWithinRange(TimeSpan startTime, TimeSpan endTime, bool processVisibleRows)
		{
			List<Row> processRows = processVisibleRows ? VisibleRows : Rows;
			List<Element> result = new List<Element>();
			foreach (Row row in processRows)
			{
				foreach (Element elem in row)
				{
					if ((startTime <= elem.StartTime) && (endTime >= elem.StartTime))
					{
						result.Add(elem);
					}
					else if (elem.StartTime > endTime)
					{
						break;
					}
						
				}
			}
			//Elements can be in multiple rows, so only return a distinct list
			return result.Distinct();
			
		} 

		/// <summary>
		/// Get a list of elements that contain the specified time in the Visible rows.
		/// </summary>
		/// <param name="time"></param>
		/// <returns>List of Element</returns>
		public List<Element> ElementsAtTime(TimeSpan time)
		{
			List<Element> result = new List<Element>();
			foreach (Row row in VisibleRows) {
				foreach (Element elem in row) {
					if ((time >= elem.StartTime) && (time < (elem.StartTime + elem.Duration)))
						result.Add(elem);
				}
			}

			return result;
		}

		/// <summary>
		/// Select all elements that are in the Virtual box created by the two diagonal points
		/// </summary>
		/// <param name="startingPoint"></param>
		/// <param name="endingPoint"></param>
		private void SelectElementsBetween(Point startingPoint, Point endingPoint)
		{
			ClearSelectedRows();
			ClearActiveRows();
			//find all the elements between us and the last selected
			if (endingPoint.X > startingPoint.X)
			{
				//Right
				if (endingPoint.Y > startingPoint.Y)
				{
					//Below
					selectElementsWithin(new Rectangle(startingPoint,
						new Size(endingPoint.X - startingPoint.X,
							endingPoint.Y - startingPoint.Y)));
				} else
				{
					//Above
					selectElementsWithin(new Rectangle(startingPoint.X, endingPoint.Y, endingPoint.X - startingPoint.X,
						startingPoint.Y - endingPoint.Y));
				}
			} else
			{
				//Left
				if (endingPoint.Y > startingPoint.Y)
				{
					//Below
					selectElementsWithin(new Rectangle(endingPoint.X, startingPoint.Y, startingPoint.X - endingPoint.X,
						endingPoint.Y - startingPoint.Y));
				} else
				{
					//Above
					selectElementsWithin(new Rectangle(endingPoint,
						new Size(startingPoint.X - endingPoint.X,
							startingPoint.Y - endingPoint.Y)));
				}
			}
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
		private void selectElementsWithin(Rectangle SelectedArea, bool useCAD = false)
		{
			if (SelectedArea.Size.IsEmpty) return;
			var containingRows = RowsIn(SelectedArea);
			TimeSpan selStart = pixelsToTime(SelectedArea.Left);
			TimeSpan selEnd = pixelsToTime(SelectedArea.Right);
			int selBottom = SelectedArea.Top + SelectedArea.Height;
			string moveDirection = "Left";
			if (useCAD)
			{
				moveDirection = (SelectedArea.Left < mouseDownGridLocation.X || !aCadStyleSelectionBox) ? "Left" : "Right";
			}

			SelectionBorder = moveDirection == "Right" ? Color.Green : Color.Blue;

			SupressSelectionEvents = true;
			// deselect all elements in the grid first, then only select the ones in the box.
			ClearSelectedElements();

			//Reselect the effects that were selected before we began the new selection box
			if (ShiftPressed)
			{
				foreach (Element e in tempSelectedElements)
				{
					e.Selected = true;
				}
			}

			// Iterate all elements of only the rows within our selection.
			foreach (var row in containingRows) {
				
				// This row is in our selection
				foreach (var elem in row.Item1) {
					if(elem.StartTime > selEnd) break;

					var elemTop = row.Item2 + elem.RowTopOffset;
					var elemBottom = elemTop + elem.DisplayHeight;

					if (DragBoxFilterEnabled)
					{
						if (moveDirection == "Left")
						{
							elem.Selected = (ShiftPressed && tempSelectedElements.Contains(elem) || ((elem.StartTime < selEnd && elem.EndTime > selStart) && 
							                                                                         elemTop < selBottom && elemBottom > SelectedArea.Top && DragBoxFilterTypes.Contains(elem.EffectNode.Effect.TypeId)));
						}
						else
						{
							elem.Selected = (ShiftPressed && tempSelectedElements.Contains(elem) || ((elem.StartTime > selStart && elem.EndTime < selEnd) && 
							                                                                         elemTop > SelectedArea.Top && elemBottom < selBottom && DragBoxFilterTypes.Contains(elem.EffectNode.Effect.TypeId)));
						}
					}
					else
					{
						if (moveDirection == "Left")
						{
							elem.Selected = (ShiftPressed && tempSelectedElements.Contains(elem) || (elem.StartTime < selEnd && elem.EndTime > selStart && 
							                                                                         elemTop < selBottom && elemBottom > SelectedArea.Top ));
						}
						else
						{
							elem.Selected = (ShiftPressed && tempSelectedElements.Contains(elem) || elem.StartTime > selStart && elem.EndTime < selEnd && 
							                 elemTop > SelectedArea.Top && elemBottom < selBottom);
						}
					}
				}
			
			} // end foreach
			SupressSelectionEvents = false;
			_SelectionChanged();
		}

		/// <summary>
		/// Returns a list of the visible rows wihtin a given Rectangle
		/// </summary>
		private List<Row> GetRowsWithin(Rectangle SelectedArea)
		{
			Row startRow = rowAt(SelectedArea.Location);
			Row endRow = rowAt(SelectedArea.BottomRight());

			List<Row> DrawingRows = new List<Row>();
			
			bool startFound = false, endFound = false;
			foreach (var row in Rows)
			{
				if (!row.Visible || endFound || (!startFound && (row != startRow)))
				{
					continue;
				}

				if (startFound || row == startRow)
				{
					startFound = true;
					DrawingRows.Add(row);

					if (row == endRow)
					{
						endFound = true;
						continue;
					}
				}
			}
			return DrawingRows;
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
			List<Row> processRows = visibleOnly ? VisibleRows : Rows;
			Dictionary<Element, int> result = elements.ToDictionary(e => e, e => 0);
			foreach (Row row in processRows)
			{

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
			List<Row> processRows = visibleOnly ? VisibleRows : Rows;
			for (int i = findTopLimitRow ? 0 : processRows.Count - 1;
				 findTopLimitRow ? (i < processRows.Count) : (i >= 0);
			     i += (findTopLimitRow ? 1 : -1)) {

				// iterate through each element we're checking for a grid limit, and check if it's in this row
				foreach (Element element in elements) {
					if (processRows[i].ContainsElement(element))
					{
						// if we're not bothering to check for duplicates, then this is the first row that
						// contains an element: good enough, return it!
						if (skipDuplicatesUnlessInRow == null)
							return processRows[i];

						// if this row shouldn't be checked for duplicates, end here, as we've found a 'good enough' match
						if (VisibleRows[i] == skipDuplicatesUnlessInRow)
							return processRows[i];

						// decrement the 'element duplicate counter': if this was the last instance of this element seen,
						// then it must stop here, so return it.
						elementsLeft[element]--;
						if (elementsLeft[element] <= 0)
							return processRows[i];
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
			// if the offset was zero or snapto is not enabled, we don't need to do anything.
			if (offset == TimeSpan.Zero || !EnableSnapTo)
				return offset;

			// grab all the elements we need to check for snapping against things (ie. filter them based on row
			// if we're only snapping to things in the current row.) Also, record the row this element is in
			// as well, since we'll need it later on, and it saves recalculating multiple times
			List<Tuple<Element, Row>> elementsToCheckSnapping = new List<Tuple<Element, Row>>();
			if (OnlySnapToCurrentRow && CurrentRowIndexUnderMouse>0) {
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

		public void SwapElementPlacement(Dictionary<Element, ElementTimeInfo> changedElements)
		{
			foreach (KeyValuePair<Element, ElementTimeInfo> e in changedElements)
			{
				// Key is reference to actual element. Value is class with its times before move.
				// Swap the element's times with the saved times from before the move, so we can restore them later in redo.
				ElementTimeInfo.SwapPlaces(e.Key, e.Value);
				if (e.Key.Row != e.Value.Row)
				{
					e.Value.Row.RemoveElement(e.Key);
					e.Key.Row.AddElement(e.Key);
					_ElementChangedRows(e.Key, e.Value.Row, e.Key.Row);
				}
				if(!e.Key.IsRendered) RenderElement(e.Key);
					
			}
		}

		public void MoveElementsVerticallyToLocation(IEnumerable<Element> elements, Point gridLocation)
		{
			Row destRow = rowAt(gridLocation);

			if (destRow == null)
				return;

			if (Rows.IndexOf(destRow) == CurrentRowIndexUnderMouse)
				return;

			List<Row> visibleRows = VisibleRows;

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
			for (int i = 0; i < visibleRows.Count; i++)
			{
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
					if (visibleRows[i].ContainsElement(element))
					{
						// note that we've seen another of this type of element
						elementCounts[element]--;

						// if the element would be moved outside the bounds of the grid, the ignore it. (check that there will
						// be another instance later: there should be, otherwise the calculations were wrong before!)
						if (i + visibleRowsToMove < 0 || i + visibleRowsToMove >= visibleRows.Count)
						{
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

			if (!SuppressInvalidate) Invalidate();
		}

		private void RecalculateAllStaticSnapPoints()
		{
			if (StaticSnapPoints == null)
				return;

			SortedDictionary<TimeSpan, List<SnapDetails>> newPoints = new SortedDictionary<TimeSpan, List<SnapDetails>>();
			foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints) {
				newPoints[kvp.Key] = new List<SnapDetails>();
				foreach (SnapDetails details in kvp.Value) {
					newPoints[kvp.Key].Add(CalculateSnapDetailsForPoint(details.SnapTime, details.SnapLevel, details.SnapColor, details.SnapBold, details.SnapSolidLine));
				}
			}
			StaticSnapPoints = newPoints;
		}

		private SnapDetails CalculateSnapDetailsForPoint(TimeSpan snapTime, int level, Color color, bool lineBold, bool solidLine)
		{
			SnapDetails result = new SnapDetails();
			result.SnapLevel = level;
			result.SnapTime = snapTime;
			result.SnapColor = color;
			result.SnapBold = lineBold;
			result.SnapSolidLine = solidLine;

			// the start time and end times for specified points are 2 pixels
			// per snap level away from the snap time.
			result.SnapStart = snapTime - TimeSpan.FromTicks(TimePerPixel.Ticks*level*SnapStrength);
			result.SnapEnd = snapTime + TimeSpan.FromTicks(TimePerPixel.Ticks*level*SnapStrength);
			return result;
		}

		public void AddSnapPoint(TimeSpan snapTime, int level, Color color, bool lineBold, bool solidLine)
		{
			if (!StaticSnapPoints.ContainsKey(snapTime))
				StaticSnapPoints.Add(snapTime, new List<SnapDetails> {CalculateSnapDetailsForPoint(snapTime, level, color, lineBold, solidLine)});
			else
				StaticSnapPoints[snapTime].Add(CalculateSnapDetailsForPoint(snapTime, level, color, lineBold, solidLine));

			if (!SuppressInvalidate) Invalidate();
		}

		public bool RemoveSnapPoint(TimeSpan snapTime)
		{
			bool rv = StaticSnapPoints.Remove(snapTime);
			if (!SuppressInvalidate) Invalidate();
			return rv;
		}

		public void ClearSnapPoints()
		{
			StaticSnapPoints.Clear();
			if (!SuppressInvalidate) Invalidate();
		}

		#region Marks

		private void ConfigureMarks()
		{
			if (_markCollections == null)
			{
				return;
			}
			_markCollections.CollectionChanged += _markCollections_CollectionChanged;
			AddMarkCollectionEvents();
			CreateSnapPointsFromMarks();
		}

		private void AddMarkCollectionEvents()
		{
			foreach (var markCollection in _markCollections)
			{
				markCollection.PropertyChanged += MarkCollection_PropertyChanged;
				markCollection.Decorator.PropertyChanged += MarkCollection_PropertyChanged;
			}
		}

		private void RemoveMarkCollectionEvents()
		{
			foreach (var markCollection in _markCollections)
			{
				markCollection.PropertyChanged -= MarkCollection_PropertyChanged;
				markCollection.Decorator.PropertyChanged -= MarkCollection_PropertyChanged;
			}
		}

		public void CreateSnapPointsFromMarks()
		{
			ClearSnapPoints();
			if (_markCollections == null) return;
			foreach (var mc in _markCollections)
			{
				if (!mc.ShowGridLines) continue;
				mc.EnsureOrder();
				foreach (var mark in mc.Marks)
				{
					AddSnapPoint(mark.StartTime, mc.Level, mc.Decorator.Color, mc.Decorator.IsBold, mc.Decorator.IsSolidLine);
				}
			}
		}

		private void UnConfigureMarks()
		{
			if (_markCollections == null)
			{
				return;
			}

			_markCollections.CollectionChanged -= _markCollections_CollectionChanged;
			RemoveMarkCollectionEvents();
			ClearSnapPoints();
		}

		private void _markCollections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//TODO It would be nice to just update the appropriate piece instead of rebuilding the entire thing.
			RemoveMarkCollectionEvents();
			AddMarkCollectionEvents();
			CreateSnapPointsFromMarks();
		}

		private void MarkCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//TODO It would be nice to just update the appropriate piece instead of rebuilding the entire thing.
			CreateSnapPointsFromMarks();
		}

		#endregion

		private void CalculateVisibleRowDisplayTops(bool visibleRowsOnly = true)
		{
			int top = 0;
			List<Row> processRows = visibleRowsOnly ? VisibleRows : Rows;
			foreach (var visibleRow in processRows)
			{
				visibleRow.DisplayTop = top;
				top += visibleRow.Height;
			}
		}

		private int CalculateAllRowsHeight()
		{
			int total = 0;

			foreach (Row row in VisibleRows) {

				total += row.Height;
			}

			return total;
		}

		public bool AllowGridResize { get; set; }

		public void ResizeGridHeight()
		{
		
			if (AllowGridResize) {
				if (InvokeRequired) {
					Invoke(new Delegates.GenericDelegate(ResizeGridHeight));
				} else {
					AutoScrollMinSize = new Size((int)timeToPixels(TotalTime), CalculateAllRowsHeight());
					CalculateVisibleRowDisplayTops();
					//Invalidate();
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

		public void SelectElements(IEnumerable<Element> elements)
		{
			elements.All(x => x.Selected = true);
			_SelectionChanged();
		}

		public void ToggleElementSelection(Element element)
		{
			element.Selected = !element.Selected;
			_SelectionChanged();
		}

		public void ToggleSelectedRows(bool includeChildren)
		{
			SuppressInvalidate = true;
			AllowGridResize = false;
			if (SelectedRows.Any())
			{
				foreach (var selectedRow in SelectedRows)
				{
					if (includeChildren)
					{
						selectedRow.ToggleTree(!selectedRow.TreeOpen);
					}
					else
					{
						selectedRow.TreeOpen = !selectedRow.TreeOpen;
					}
				}
			}
			else if (ActiveRow != null)
			{
				if (includeChildren)
				{
					ActiveRow.ToggleTree(!ActiveRow.TreeOpen);
				}
				else
				{
					ActiveRow.TreeOpen = !ActiveRow.TreeOpen;
				}
			}

			SuppressInvalidate = false;
			AllowGridResize = true;
			ResizeGridHeight();
			Invalidate();
		}

		#endregion
	
		#region Drawing

		public void BeginDraw()
		{
			SuppressInvalidate = true;
		}

		public void EndDraw()
		{
			SuppressInvalidate = false;
			Invalidate();
		}

		private void _drawRows(Graphics g)
		{
			int curY = 0;

			// Draw row separators
			using (Pen p = new Pen(RowSeparatorColor))
			using (SolidBrush b = new SolidBrush(SelectionColor)) {
				foreach (Row row in VisibleRows) {
					Point selectedTopLeft = new Point((-AutoScrollPosition.X), curY);
					curY += row.Height;
					Point lineLeft = new Point((-AutoScrollPosition.X), curY);
					Point lineRight = new Point((-AutoScrollPosition.X) + Width, curY);

					if (row.Selected)
					{
						g.FillRectangle(b, Util.RectangleFromPoints(selectedTopLeft, lineRight));
						using (Pen bp = new Pen(SelectionBorder))
						{
							g.DrawRectangle(bp, Util.RectangleFromPoints(selectedTopLeft, lineRight));
						}
					}
					if (row.Active)
					{
						g.FillRectangle(b, Util.RectangleFromPoints(selectedTopLeft, lineRight));
					}
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
			Pen p = new Pen(Color.Black, 1);

			// iterate through all snap points, and if it's visible, draw it
			foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints)
			{
				if (kvp.Key >= VisibleTimeEnd) break;
				if(MarkAlignmentPoints.Contains(kvp.Key)) continue;
				if (kvp.Key >= VisibleTimeStart) {
					SnapDetails details = null;
					foreach (SnapDetails d in kvp.Value)
					{
						if (details == null || (d.SnapLevel > details.SnapLevel && d.SnapColor != Color.Empty))
							details = d;
					}

					p.Color = details.SnapColor;
					p.Width = details.SnapBold?3:1;
					Single x = timeToPixels(kvp.Key);
					if (!details.SnapSolidLine)
					{
						p.DashPattern = new float[] {details.SnapLevel, details.SnapLevel};
					}
					else
					{
						p.DashStyle = DashStyle.Solid;
					}
					g.DrawLine(p, x, 0, x, AutoScrollMinSize.Height);
					
				}
			}

			if (m_dragState == DragState.Normal)
			{
				p = new Pen(Brushes.Yellow) { DashPattern = new float[] { 2, 2 } };

				foreach (var activeTime in MarkAlignmentPoints)
				{
					var x1 = timeToPixels(activeTime);
					g.DrawLine(p, x1, 0, x1, AutoScrollMinSize.Height);
				}
			}

			p.Dispose();
		}

		#region Element rendering background worker

		public void StartBackgroundRendering()
		{
			if (InvokeRequired) {
				Invoke(new Delegates.GenericDelegate(StartBackgroundRendering));
			} else {
				if (renderWorker != null) {
					if (!renderWorker.IsBusy) {
						renderWorker.RunWorkerAsync();
					}
				} else {
					renderWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
					renderWorker.DoWork += renderWorker_DoWork;
					renderWorker.ProgressChanged += renderWorker_ProgressChanged;
					renderWorkerFinished = new ManualResetEventSlim(false);
					renderWorker.RunWorkerAsync();
				}
			}
		}

		private void renderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			_RenderProgressChanged(e.ProgressPercentage);
		}

		private long _renderQueueSize = 0;
		private long _processed = 0;
		private int _snapStrength;

		//This whole thing need to be redone as a task once we get to .NET 4.5 where we can easily report progress
		//from it.
		private void renderWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			if (worker == null) {
				Logging.Error("renderWorker: sender was null");
				return;
			}
            CancellationTokenSource cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;
			
			try
		    {
		        if (_blockingElementQueue != null)
		        {
                    //Use or fancy multi cpu boxes more effectively.
		            //foreach (Element element in _blockingElementQueue.GetConsumingEnumerable()) 
		            Parallel.ForEach(_blockingElementQueue.GetConsumingPartitioner(), po, element =>
		            {
			            
		                // This will likely never be hit: the blocking element queue above will always block waiting for more
		                // elements, until it completes because CompleteAdding() is called. At which point it will exit the loop,
		                // as it will be empty, and this function will terminate normally.
		                if (worker.CancellationPending)
		                {
		                    Logging.Warn("render worker: cancellation detected. Aborting.");
                            cts.Cancel(false);
		                    //break;
		                }
		                try
		                {
		                    element.RenderElement();
		                    if (!SuppressInvalidate)
		                    {
		                        if (element.EndTime > VisibleTimeStart && element.StartTime < VisibleTimeEnd)
		                        {
		                            Invalidate();
		                        }
		                    }

                            //this is a bit of a kludge until we get to .NET 4.5 and can do this whole thing
                            //in a task. Reporting progress from Tasks is not well supported until 4.5
                            //With the multi-threading the last element can be processed before the count is 
                            //fully updated
			                lock (worker)
			                {
								var progress = (int)((float)Interlocked.Increment(ref _processed) / _renderQueueSize * 100);
								worker.ReportProgress(progress);
							}
						}
		                catch (Exception ex)
		                {
							worker.ReportProgress(100);
							Logging.Error(ex, "Error in rendering.");
		                }
		            });
		        }
		    }
		    catch (OperationCanceledException ce)
		    {
                Logging.Info("Canceled render thread" , ce);
		    }
		    catch (Exception exception)
		    {
		        // there may be some threading exceptions; if so, they're unexpected.  Log them.
		        Logging.Error(exception,"background rendering worker exception:");
		    }
		    renderWorkerFinished.Set();
		}

		/// <summary>
		/// Renders the specific element.
		/// </summary>
		/// <param name="element"></param>
        public void RenderElement(Element element)
        {
			if (SupressRendering) return;
			if (_blockingElementQueue.Any())
			{
				//Render single elements right now instead of tossing in the queue if the queue is busy. If we have single elements it is probably
				//because someone is directly working with it.
				Task.Factory.StartNew(() =>
										{
											element.RenderElement();
											Invalidate();
										});
			} else
			{
				_blockingElementQueue.Add(element);
				_renderQueueSize++;
			}
        }

		/// <summary>
		/// Renders elements in the selected rows in the grid.
		/// </summary>
		/// <param name="rows"></param>
		public void RenderRows(List<Row> rows)
		{
			if (SupressRendering) return;
			foreach (Row row in rows)
			{
				for (int i = 0; i < row.ElementCount; i++)
				{
					Element element = row.GetElementAtIndex(i);
					if (!element.IsRendered)
					{
						_blockingElementQueue.Add(element);
						_renderQueueSize++;
					}
				}
			}
		}

		/// <summary>
		/// Renders all elements in all the rows.
		/// </summary>
		public void RenderAllRows()
		{
			ClearElementRenderQueue();
			RenderRows(Rows);
		}

        private void ClearElementRenderQueue()
        {
			SupressRendering = true;
			while (_blockingElementQueue.Count > 0)
			{
				Element element;
				_blockingElementQueue.TryTake(out element);
			}
	        _renderQueueSize = 0;
	        _processed = 0;
			renderWorker.ReportProgress(100);
			SupressRendering=false;
			
        }

		#endregion

		private void DrawElement(Graphics g, Row row, Element currentElement, int top)
		{
			int width;
			bool redBorder = false;

			//Sanity check - it is possible for .DisplayHeight to become zero if there are too many effects stacked.
			//We set the DisplayHeight to the row height for the currentElement, and change the border to red.		
			currentElement.DisplayHeight = 
				(currentElement.StackCount != 0) ? ((row.Height - 1) / currentElement.StackCount) : row.Height - 1;

			currentElement.DisplayTop = top + (currentElement.DisplayHeight * currentElement.StackIndex);
			currentElement.RowTopOffset = currentElement.DisplayHeight * currentElement.StackIndex;

			if (currentElement.DisplayHeight == 0)
			{
				redBorder = true;
				currentElement.DisplayHeight = currentElement.Row.Height;
			}

			
			if (currentElement.StartTime >= VisibleTimeStart)
			{
				if (currentElement.EndTime < VisibleTimeEnd)
				{
					width = (int) timeToPixels(currentElement.Duration);
				}
				else
				{
					width = (int) (timeToPixels(VisibleTimeEnd) - timeToPixels(currentElement.StartTime));
				}
			}
			else
			{
				if (currentElement.EndTime <= VisibleTimeEnd)
				{
					width = (int) (timeToPixels(currentElement.EndTime) - timeToPixels(VisibleTimeStart));
				} else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(VisibleTimeStart));
				}	
			}
			if (width <= 0) return;
			Size size = new Size(width, currentElement.DisplayHeight);

			Point finalDrawLocation = new Point((int)Math.Floor(timeToPixels(currentElement.StartTime>VisibleTimeStart?currentElement.StartTime:VisibleTimeStart)), currentElement.DisplayTop);
			
			Rectangle destRect = new Rectangle(finalDrawLocation.X, finalDrawLocation.Y, size.Width, currentElement.DisplayHeight);
			currentElement.DisplayRect = destRect;

			Bitmap elementImage = currentElement.Draw(size, g, VisibleTimeStart, VisibleTimeEnd, (int)timeToPixels(currentElement.Duration), redBorder);
			if (elementImage == null) return;

			try
			{
				lock (elementImage)
				{
					g.DrawImage(elementImage,destRect);
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "Unable to draw element image.");
			}
			
		}

		private void _drawInfo(Graphics g)
		{

			if (ShowEffectToolTip && capturedElements.Any())
			{
				Element element = capturedElements.First();
				if (element == null) return;
				//This element may be part of more than one row. So it's internal Display rect can be wrong thus
				//placing the info tool tip in the wrong place
				//Until that is fixed which is a bigger effort lets use our current row for part of the rectangle.
				Row row = rowAt(m_lastGridLocation);
				if (row != null) //null check to prevent mouse off screen locations trying to find a row.
				{
					var layerInfo = SequenceLayers.GetLayer(element.EffectNode);
					element.DrawInfo(g, new Rectangle(element.DisplayRect.X, row.DisplayTop, element.DisplayRect.Width, row.Height), layerInfo);
				}
			}
		}

		private void _drawElements(Graphics g)
		{
			// Draw each row
			foreach (Row row in VisibleRows) {
				row.SetStackIndexes(VisibleTimeStart, VisibleTimeEnd);
				for (int i = 0; i < row.ElementCount; i++) {
					Element currentElement = row.GetElementAtIndex(i);
					if (currentElement.EndTime < VisibleTimeStart)
						continue;

					if (currentElement.StartTime > VisibleTimeEnd) {
						break;
					}
					
					DrawElement(g, row, currentElement, row.DisplayTop);
				}

			}
		}

		private void _drawSelection(Graphics g)
		{
			if (SelectionArea.IsEmpty)
				return;

				using (SolidBrush b = new SolidBrush(SelectionColor))
				{
					g.FillRectangle(b, SelectionArea);
				}
				using (Pen p = new Pen(SelectionBorder))
				{
					g.DrawRectangle(p, SelectionArea);
				}
		}

		private void _drawDrawBox(Graphics g)
		{
			if (DrawingArea.IsEmpty)
				return;

			using (SolidBrush b = new SolidBrush(DrawColor))
			{
				g.FillRectangle(b, DrawingArea);
			}
			using (Pen p = new Pen(DrawBorder,2))
			{
				g.DrawRectangle(p, DrawingArea);
			}

			if (ResizeIndicator_Enabled)
			{
				using (Pen p = new Pen(Color.FromName(ResizeIndicator_Color),1))
				{
					g.DrawLine(p, DrawingArea.X, 0, DrawingArea.X, AutoScrollMinSize.Height);
					g.DrawLine(p, DrawingArea.X + DrawingArea.Width - 1, 0, DrawingArea.X + DrawingArea.Width - 1, AutoScrollMinSize.Height);
				}
			}
		}

		/// <summary>
		/// Draws an indicator line at the start or end of an effect when resizing.
		/// When dragging an entire effect a line is drawn at both ends.
		/// </summary>
		/// <param name="g"></param>
		private void _drawResizeIndicator(Graphics g)
		{
			//We must - 1px for the end time for a spot on alignment

			if (!ResizeIndicator_Enabled) //If this option isn't enabled leave
				return;

			if (m_dragState == DragState.HResizing) //Draw line at start or end of effect, depending which end the user grabbed
			{
				TimeLineGlobalEventManager.Manager.OnAlignmentActivity(new AlignmentEventArgs(true, new []{ _workingElement.EndTime }));
				using (Pen p = new Pen(Color.FromName(ResizeIndicator_Color), 1))
				{
					var X = (m_mouseResizeZone == ResizeZone.Front ? timeToPixels(_workingElement.StartTime) : timeToPixels(_workingElement.EndTime) - 1);
					g.DrawLine(p, X, 0, X, AutoScrollMinSize.Height);
				}
			}

			if (m_dragState == DragState.Waiting || m_dragState == DragState.Moving) //Draw line at both ends, the user is dragging the entire effect
			{
				TimeLineGlobalEventManager.Manager.OnAlignmentActivity(new AlignmentEventArgs(true, new[] { _workingElement. StartTime, _workingElement.EndTime }));
				using (Pen p = new Pen(Color.FromName(ResizeIndicator_Color), 1))
				{
					g.DrawLine(p, timeToPixels(_workingElement.StartTime), 0, timeToPixels(_workingElement.StartTime), AutoScrollMinSize.Height);
					g.DrawLine(p, timeToPixels(_workingElement.EndTime) - 1, 0, timeToPixels(_workingElement.EndTime) - 1, AutoScrollMinSize.Height);
				}
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
					e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

					//Stopwatch s = new Stopwatch();
					//s.Start();

					_drawGridlines(e.Graphics);
					_drawRows(e.Graphics);
					_drawSnapPoints(e.Graphics);
					_drawElements(e.Graphics);
					_drawInfo(e.Graphics);
					_drawSelection(e.Graphics);
					_drawDrawBox(e.Graphics);
					_drawCursors(e.Graphics);
					_drawResizeIndicator(e.Graphics);

					//s.Stop();
					//Logging.Info("OnPaint: " + s.ElapsedMilliseconds);
				}
				catch (Exception ex) {
					Logging.Error(ex, "Exception in TimelineGrid.OnPaint()");
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("An unexpected error occurred while drawing the grid. Please notify the Vixen team and provide the error logs.",
						@"Error", false, false);
					messageBox.ShowDialog();
				}
		}

		#endregion

	}

	public class SnapDetails
	{
		public TimeSpan SnapTime; // the particular time to snap to
		public TimeSpan SnapStart; // the start time that should snap to this time; ie. before or equal to the snap time
		public TimeSpan SnapEnd; // the end time that should snap to this time; ie. after or equal to the snap time
		public int SnapLevel; // the "priority" of this snap point; bigger is higher priority
		public Row SnapRow; // the rows that this point should affect; null if all rows
		public Color SnapColor; // the color to draw the snap point
		public bool SnapBold; // snap point is bold
		public bool SnapSolidLine; // snap point is a solidline or dotted
	}

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