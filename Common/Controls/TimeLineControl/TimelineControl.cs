using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using VixenModules.Media.Audio;
using Common.Controls.Scaling;
using Common.Controls.TimelineControl;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace Common.Controls.Timeline
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public class TimelineControl : TimelineControlBase, IEnumerable<Row>
	{
		//These are the 96 DPI based defaults. They should be scaled if used.
		public const int DefaultSplitterDistance = 200;
		public const int DefaultRowHeight = 32;

		#region Member Controls

		public SplitContainer splitContainer;

		// Left side (Panel 1)
		private RowList timelineRowList;

		// Right side (Panel 2)
		public Ruler ruler;
		public Grid grid;
		public Waveform waveform;

		#endregion

		private bool _sequenceLoading = false;

		public bool ZoomToMousePosition { get; set; }

		public int rowHeight;
		private Row selectedRow;

		public bool SequenceLoading
		{
			get { return _sequenceLoading; }
			set
			{
				_sequenceLoading = value;
				if (grid != null)
					grid.SequenceLoading = value;
			}
		}

		public TimelineControl()
			: base(new TimeInfo()) // This is THE TimeInfo object for the whole control (and all sub-controls).
		{
			rowHeight = (int)(DefaultRowHeight*ScalingTools.GetScaleFactor());
			TimeInfo.TimePerPixel = TimeSpan.FromTicks(100000);
			TimeInfo.VisibleTimeStart = TimeSpan.Zero;

			InitializeControls();

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);

			// Event handlers for Row class static events
			Row.RowToggled += RowToggledHandler;
			Row.RowHeightChanged += RowHeightChangedHandler;
			Row.RowHeightResized += RowHeightResizedHandler;
		}
	 
		public void EnableDisableHandlers(bool enabled = true)
		{
			if (enabled) {
				Row.RowToggled -= RowToggledHandler;
				Row.RowHeightChanged -= RowHeightChangedHandler;
				Row.RowHeightResized -= RowHeightResizedHandler;
				Row.RowToggled += RowToggledHandler;
				Row.RowHeightChanged += RowHeightChangedHandler;
				Row.RowHeightResized += RowHeightResizedHandler;
				Row.RowLabelContextMenuSelect += RowLabelContextMenuHandler;
			} else {
				Row.RowToggled -= RowToggledHandler;
				Row.RowHeightChanged -= RowHeightChangedHandler;
				Row.RowHeightResized -= RowHeightResizedHandler;
				Row.RowLabelContextMenuSelect -= RowLabelContextMenuHandler;
			}
			this.timelineRowList.EnableDisableHandlers(enabled);
		}
		#region Initialization
		protected override void Dispose(bool disposing)
		{
			
			Row.RowToggled -= RowToggledHandler;
			Row.RowHeightChanged -= RowHeightChangedHandler;
			Row.RowHeightResized -= RowHeightResizedHandler;
			Row.RowLabelContextMenuSelect -= RowLabelContextMenuHandler;
			Vixen.Utility.cEventHelper.RemoveAllEventHandlers(this);
			Vixen.Utility.cEventHelper.RemoveAllEventHandlers(TimeInfo);
			TimeInfo = null;

			if (grid != null) {
				grid.Scroll -= GridScrolledHandler;
				grid.VerticalOffsetChanged -= GridScrollVerticalHandler;
				grid.Dispose();
				Vixen.Utility.cEventHelper.RemoveAllEventHandlers(grid);
				grid = null;
			}
			
			if (timelineRowList != null) {
				timelineRowList.Dispose();
				timelineRowList= null;
			}
			waveform?.Dispose();
			waveform= null;

			MarksBar?.Dispose();
			MarksBar = null;

			ruler?.Dispose();
			ruler = null;

			base.Dispose(disposing);
		}
		private void InitializeControls()
		{
			this.SuspendLayout();

			// (this) Timeline Control
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Name = "TimelineControl";


			// Split Container
			splitContainer = new SplitContainer()
			                 	{
			                 		Dock = DockStyle.Fill,
			                 		Orientation = Orientation.Vertical,
			                 		Name = "splitContainer",
			                 		FixedPanel = FixedPanel.Panel1,
			                 		Panel1MinSize = 100,
			                 	};
			this.Controls.Add(this.splitContainer);

			// Split container panels
			splitContainer.BeginInit();
			splitContainer.SuspendLayout();

			InitializePanel1();
			InitializePanel2();
			
			splitContainer.ResumeLayout(false);
			splitContainer.EndInit();
		
			splitContainer.PerformAutoScale();
			this.ResumeLayout(false);
		}

		// Panel 1 - the left side of the splitContainer
		private void InitializePanel1()
		{
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel1.BackColor = ThemeColorTable.TimeLinePanel1BackColor;

			// Row List
			timelineRowList = new RowList()
			                  	{
			                  		Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
			                  		DottedLineColor = ThemeColorTable.ForeColor,
			                  		Name = "timelineRowList"
			                  	};
			splitContainer.Panel1.Controls.Add(timelineRowList);

			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel1.PerformLayout();
		}


		// Panel 2 - the right side of the splitContainer
		private void InitializePanel2()
		{
			// Add all timeline-like controls to panel2
			splitContainer.Panel2.SuspendLayout();

			// Grid
			grid = new Grid(TimeInfo)
			       	{
			       		Dock = DockStyle.Fill,
			       	};
			splitContainer.Panel2.Controls.Add(grid); // gets added first - to fill the remains
			grid.Scroll += GridScrolledHandler;
			grid.VerticalOffsetChanged += GridScrollVerticalHandler;


			//Marks
			MarksBar = new MarksBar(TimeInfo)
			{
				Dock = DockStyle.Top,
				Height = 50
			};

			splitContainer.Panel2.Controls.Add(MarksBar);

			// Ruler
			ruler = new Ruler(TimeInfo)
			        	{
			        		Dock = DockStyle.Top,
			        		Height = 50,
			        	};
			splitContainer.Panel2.Controls.Add(ruler);

			//WaveForm
			//TODO deal with positioning, can we dock two controls to the top
			//Looks like the last one wins.
			waveform = new Waveform(TimeInfo)
			           	{
			           		Dock = DockStyle.Top,
			           		Height = 50
			           	};

			splitContainer.Panel2.Controls.Add(waveform);

			splitContainer.Panel2.ResumeLayout(false);
			splitContainer.Panel2.PerformLayout();
		}

		#endregion

		#region Properties

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VerticalOffset
		{
			get
			{
				if (grid != null)
					return grid.VerticalOffset;
				else
					return 0;
			}
			set
			{
				if (grid != null)
					grid.VerticalOffset = value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleHeight
		{
			get { return grid.ClientSize.Height; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TimeSpan VisibleTimeSpan
		{
			get { return grid.VisibleTimeSpan; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<Element> SelectedElements
		{
			get { return grid.SelectedElements; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<string> SelectedElementTypes
		{
			get
			{
				List<string> elementTypesList = new List<string>();
				foreach (Element element in grid.SelectedElements.Where(element => !elementTypesList.Contains(element.EffectNode.Effect.EffectName)))
				{
					elementTypesList.Add(element.EffectNode.Effect.EffectName);
				}

				return elementTypesList;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<Row> Rows
		{
			get { return grid.Rows; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<Row> SelectedRows
		{
			get { return grid.SelectedRows; }
			set { grid.SelectedRows = value; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Row SelectedRow
		{
			get { return SelectedRows.FirstOrDefault(); }
			set { SelectedRows = new Row[] {value}; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Row ActiveRow
		{
			get { return grid.ActiveRow; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<Row> VisibleRows
		{
			get { return grid.VisibleRows; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Row TopVisibleRow
		{
			get { return grid.TopVisibleRow; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MarksBar MarksBar { get; set; }


		#endregion

		#region Methods

		public void LayoutRows()
		{
			timelineRowList.DoLayout();
		}

		// Zoom in or out (ie. change the visible time span): give a scale < 1.0
		// and it zooms in, > 1.0 and it zooms out.
		public void Zoom(double scale)
		{
			if (scale <= 0.0)
				return;
			grid.BeginDraw();
			if (VisibleTimeSpan.Scale(scale) > TotalTime) {
				var t = TimeSpan.FromTicks(TotalTime.Ticks / grid.Width);
				if(t.Ticks > 2000)
				{
					TimePerPixel = t;
					VisibleTimeStart = TimeSpan.Zero;
				}
			}
			else {
				var t = TimePerPixel.Scale(scale);
				if (t.Ticks > 2000)
				{
					TimePerPixel = t;
					if (VisibleTimeEnd >= TotalTime)
						VisibleTimeStart = TotalTime - VisibleTimeSpan;
				} 
				
			}
			grid.EndDraw();
			}

		public void ZoomTime(double scale, Point mousePosition)
		{
			if (scale <= 0.0)
				return;
			grid.BeginDraw();

			decimal gridPixelWidth = splitContainer.Panel2.Width;
			TimeSpan originalTimeSpan = VisibleTimeSpan;
			TimePerPixel = TimePerPixel.Scale(scale);
			TimeSpan newTimeSpan = VisibleTimeSpan;
			decimal timeSpanOffset = ((100 / gridPixelWidth) * (mousePosition.X - splitContainer.SplitterDistance) / 100);
			VisibleTimeStart = scale > 1
				? VisibleTimeStart - (pixelsToTime((int)(timeToPixels(newTimeSpan - originalTimeSpan) * (float)timeSpanOffset)))
				: VisibleTimeStart + (pixelsToTime((int)(timeToPixels(originalTimeSpan - newTimeSpan) * (float)timeSpanOffset)));

			grid.EndDraw();
		}

		public void ZoomRows(double scale)
		{
			if (scale <= 0.0)
				return;
			//The following ensures the screen is not refreshed in any way, saving a lot of redraw time and rows now resize smoothly.
			grid.AllowGridResize = false;
			EnableDisableHandlers(false);
			//sets the new rowheight
			rowHeight = (int)(rowHeight * scale);
			//Updastes all rows with new rowheight
			foreach (Row r in Rows)
			{
				if (r.Height * scale > grid.Height) continue; //Don't scale a row beyond the grid height. How big do you need it?
				r.Height = (int)(r.Height * scale);
			}
			//Enables handlers and refreshes row and grid heights.
			EnableDisableHandlers();
			grid.AllowGridResize = true;
			LayoutRows();
			Rows.ElementAt(0).Height = Rows.ElementAt(0).Height;
		}

		public void ResizeGrid()
		{
			grid.AllowGridResize = true;
			grid.ResizeGridHeight();
		}

		public bool AllowGridResize
		{
			get { return grid.AllowGridResize; }
			set { grid.AllowGridResize = value; }
		}

		private void AddRowToControls(Row row, RowLabel label)
		{
			grid.AddRow(row);
			timelineRowList.AddRowLabel(label);
		}

		private void RemoveRowFromControls(Row row)
		{
			grid.RemoveRow(row);
			timelineRowList.RemoveRowLabel(row.RowLabel);
		}

		// adds a given row to the control, optionally as a child of the given parent
		public void AddRow(Row row, Row parent = null)
		{
			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);
		}

		// adds a new, empty row with a default label with the given name, as a child of the (optional) given parent
		public Row AddRow(string name, Row parent = null, int height = 50)
		{
			Row row = new Row();

			row.Name = name;
			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}

		// adds a new, empty row with the given label, as a child of the (optional) given parent
		public Row AddRow(RowLabel label, Row parent = null, int height = 50)
		{
			Row row = new Row(label);

			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}

		public Audio Audio
		{
			get { return waveform.Audio; }
			set { waveform.Audio = value; }
		}

		public void AddMarks(ObservableCollection<IMarkCollection> marks)
		{
			
			MarksBar.MarkCollections = marks;
			ruler.MarkCollections = marks;
			grid.MarkCollections = marks;
			//ClearAllSnapTimes();
			//foreach (MarkCollection mc in marks)
			//{
			//	if (!mc.IsEnabled) continue;
			//	mc.EnsureOrder();
			//	foreach (var mark in mc.Marks)
			//	{
			//		AddSnapTime(mark, mc.Level, mc.Decorator.Color, mc.Decorator.IsBold, mc.Decorator.IsSolidLine);
			//	}
			//}
		}

		//public void AddSnapTime(Mark labeledMark, int level, Color color, bool lineBold, bool solidLine)
		//{
		//	grid.AddSnapPoint(labeledMark.StartTime, level, color, lineBold, solidLine);
		//}

		//public void ClearAllSnapTimes()
		//{
		//	grid.ClearSnapPoints();
		//}

		public void AlignSelectedElementsLeft()
		{
			grid.AlignSelectedElementsLeft();
		}

		/// <summary>
		/// Clears all elements from the grid, leaving the rows intact.
		/// </summary>
		public void ClearAllElements()
		{
			foreach (Row row in grid) {
				row.ClearRowElements();
			}
		}

		/// <summary>
		/// Clears all rows from the grid; effectively emptying the grid. Will also
		/// clear all elements in the grid as well.
		/// </summary>
		public void ClearAllRows()
		{
			ClearAllElements();
			foreach (Row row in grid.ToArray()) {
				RemoveRowFromControls(row);
			}
		}

		/// <summary>
		/// Selects all elements in the grid.
		/// </summary>
		public void SelectAllElements()
		{
			foreach (Row r in grid) {
				r.SelectAllElements();
			}
		}

		public void SelectElement(Element element)
		{
			grid.SelectElement(element);
		}

		public void DeselectElement(Element element)
		{
			grid.DeselectElement(element);
		}

		public void ToggleElementSelection(Element element)
		{
			grid.ToggleElementSelection(element);
		}

		/// <summary>
		/// Moves all selected elements by the given amount of time.
		/// </summary>
		/// <param name="offset"></param>
		public void MoveSelectedElementsByTime(TimeSpan offset)
		{
			grid.MoveElementsByTime(SelectedElements, offset);
		}


		public IEnumerator<Row> GetEnumerator()
		{
			return grid.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return grid.GetEnumerator();
		}

		#endregion

		#region Events exposed from sub-controls (Grid, Ruler, etc)

		public event EventHandler SelectionChanged
		{
			add { grid.SelectionChanged += value; }
			remove { if (grid != null) grid.SelectionChanged -= value; }
		}

		public event EventHandler<ElementEventArgs> ElementDoubleClicked
		{
			add { grid.ElementDoubleClicked += value; }
			remove { if (grid != null) grid.ElementDoubleClicked -= value; }
		}

		public event EventHandler<MultiElementEventArgs> ElementsFinishedMoving
		{
			add { grid.ElementsFinishedMoving += value; }
			remove { if (grid != null) grid.ElementsFinishedMoving -= value; }
		}

		public event EventHandler VerticalOffsetChanged
		{
			add { grid.VerticalOffsetChanged += value; }
			remove { if (grid != null) grid.VerticalOffsetChanged -= value; }
		}

		public event EventHandler<ElementRowChangeEventArgs> ElementChangedRows
		{
			add { grid.ElementChangedRows += value; }
			remove { if (grid != null) grid.ElementChangedRows -= value; }
		}

		public event EventHandler<ElementsChangedTimesEventArgs> ElementsMovedNew
		{
			add { grid.ElementsMovedNew += value; }
			remove { if (grid != null) grid.ElementsMovedNew -= value; }
		}

		public event EventHandler<ElementsSelectedEventArgs> ElementsSelected
		{
			add { grid.ElementsSelected += value; }
			remove { if (grid != null) grid.ElementsSelected -= value; }
		}

		public event EventHandler<ContextSelectedEventArgs> ContextSelected
		{
			add { grid.ContextSelected += value; }
			remove { if (grid != null) grid.ContextSelected -= value; }
		}

		public event EventHandler<RulerClickedEventArgs> RulerClicked
		{
			add { ruler.ClickedAtTime += value; }
			remove { ruler.ClickedAtTime -= value; }
		}

		//public event EventHandler<MarksMovedEventArgs> MarksMoved
		//{
		//	add
		//	{
		//		ruler.MarksMoved += value;
		//		MarksBar.MarksMoved += value;
		//	}
		//	remove
		//	{
		//		ruler.MarksMoved -= value;
		//		MarksBar.MarksMoved -= value;
		//	}
		//}

		//public event EventHandler<MarksMovingEventArgs> MarksMoving
		//{
		//	add
		//	{
		//		ruler.MarksMoving += value;
		//		MarksBar.MarksMoving += value;
		//	}
		//	remove
		//	{
		//		ruler.MarksMoving -= value;
		//		MarksBar.MarksMoving -= value;
		//	}
		//}

		//public event EventHandler<MarkNudgeEventArgs> MarkNudge
		//{
		//	add { ruler.MarkNudge += value; }
		//	remove { ruler.MarkNudge -= value; }
		//}

		//public event EventHandler<MarksDeletedEventArgs> DeleteMark
		//{
		//	add
		//	{
		//		ruler.DeleteMark += value;
		//		MarksBar.DeleteMark += value;
		//	}
		//	remove
		//	{
		//		ruler.DeleteMark -= value;
		//		MarksBar.DeleteMark -= value;
		//	}
		//}

		public event EventHandler RulerBeginDragTimeRange
		{
			add { ruler.BeginDragTimeRange += value; }
			remove { ruler.BeginDragTimeRange -= value; }
		}

		public event EventHandler<ModifierKeysEventArgs> RulerTimeRangeDragged
		{
			add { ruler.TimeRangeDragged += value; }
			remove { ruler.TimeRangeDragged -= value; }
		}

		//public event EventHandler<SelectedMarkMoveEventArgs> SelectedMarkMove
		//{
		//	add { Ruler.SelectedMarkMove += value; }
		//	remove { Ruler.SelectedMarkMove -= value; }
		//}

		#endregion
		
		#region Event Handlers

		private void GridScrollVerticalHandler(object sender, EventArgs e)
		{
			if (timelineRowList != null)
			{
				timelineRowList.Top = grid.Top;
				timelineRowList.Height = grid.ClientSize.Height;
			}
			timelineRowList.VerticalOffset = grid.VerticalOffset;

			// I know it's bad to do this, but when we scroll we can get very nasty artifacts
			// and it looks shit in general. So, force an immediate graphical refresh
			Refresh();
		}

		private void GridScrollHorizontalHandler(object sender, EventArgs e)
		{
		}

		private void GridScrolledHandler(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				//GridScrollHorizontalHandler(sender, e);
			}
			else {
				GridScrollVerticalHandler(sender, e);
			}
		}

		private void RowToggledHandler(object sender, EventArgs e)
		{
			if (timelineRowList != null)
				timelineRowList.VerticalOffset = grid.VerticalOffset;
			Invalidate();
		}

		private void RowHeightChangedHandler(object sender, EventArgs e)
		{
				
		}

		private void RowHeightResizedHandler(object sender, EventArgs e)
		{
			Invalidate();

			//resizes all other copies of the same element
			var selectedRow = sender as Row;
			foreach (Row row in Rows)
			{
				if (row.Name == selectedRow.Name)
				{
					row.Height = selectedRow.Height;
		}
			}
		}


		public static readonly ContextMenuStrip RowListMenu = new ContextMenuStrip();

		#region RowLabel Context Menu Strip

		private void RowLabelContextMenuHandler(object sender, EventArgs e)
		{
			//Conext menu for the RowLabel when right clicking.
			RowListMenu.Items.Clear();
			ToolStripMenuItem RowListMenuCollapse = new ToolStripMenuItem("Collapse All Groups");
			ToolStripMenuItem RowListMenuResetRowHeight = new ToolStripMenuItem("Reset All Rows to Default Height");
			ToolStripMenuItem RowListMenuResetSelectedRowHeight =
				new ToolStripMenuItem("Reset Selected and Child rows to Default Height");
			RowListMenuCollapse.Click += RowListMenuCollapse_Click;
			RowListMenuResetRowHeight.Click += ResetRowHeight_Click;
			RowListMenuResetSelectedRowHeight.Click += ResetSelectedRowHeight_Click;
			RowListMenu.Items.AddRange(new ToolStripItem[]
			{RowListMenuCollapse, RowListMenuResetRowHeight, RowListMenuResetSelectedRowHeight});
			RowListMenu.Renderer = new ThemeToolStripRenderer();
			RowListMenu.Show(MousePosition);
		}

		private void ResetRowHeight_Click(object sender, EventArgs e)
		{
			ResetRowHeight();
		}
		private void ResetSelectedRowHeight_Click(object sender, EventArgs e)
		{
			ResetSelectedRowHeight();
		}

		private void RowListMenuCollapse_Click(object sender, EventArgs e)
		{
			RowListMenuCollapse();
		}

		public void ResetRowHeight()
		{
			//Resets all row heights back to default
			//ensure that rows are completed before refreshing allowing a smooth transistion.
			EnableDisableHandlers(false);
			grid.AllowGridResize = false;
			rowHeight = (int)(DefaultRowHeight*ScalingTools.GetScaleFactor());
			foreach (Row row in Rows)
			{
				if (row.Height != rowHeight )
					row.Height = rowHeight;
			}
			
			EnableDisableHandlers();
			grid.AllowGridResize = true;
			LayoutRows();
			Rows.ElementAt(0).Height = Rows.ElementAt(0).Height;
		}

		public void RowListMenuCollapse()
		{
			//Collapses all open groups
			foreach (Row row in Rows)
			{
				if (row.TreeOpen)
				{
					row.TreeOpen = false;
				}
			}
		}

		private void ResetSelectedRowHeight()
		{
			//Resets the selected row and childs to current default height.
			SelectedRow.Height = rowHeight;
			foreach (Row rH in SelectedRow.ChildRows)
			{
				rH.Height = rowHeight;
				ChangeRowHeight(rH); 
			}
		}

		public void ChangeRowHeight(Row childs)
		{
			// iterate through all of its children, changing each row height to the current default
			foreach (Row child in childs.ChildRows)
			{
				child.Height = rowHeight;
				ChangeRowHeight(child);
			}
		}
		#endregion

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (ModifierKeys.HasFlag(Keys.Control) & ModifierKeys.HasFlag(Keys.Alt))
			{
				//holding the control and alt keys while scrolling adjusts the row height under the cursor.
				Point gridLocation = e.Location;
				int delta = e.Delta;
				zoomRowHeight(gridLocation, delta);
			}
			else if (ModifierKeys.HasFlag(Keys.Control) & ModifierKeys.HasFlag(Keys.Shift))
			{
				//holding the control and shift while scrolling adjusts all row heights
				double zoomScale = e.Delta < 0.0 ? 0.8 : 1.25;
				ZoomRows(zoomScale);
			}
			else if (ModifierKeys.HasFlag(Keys.Control))
			{
				// holding the control key zooms the horizontal axis, by 10% per mouse wheel tick
				if (ZoomToMousePosition)
				{
					// holding the control key zooms the horizontal axis under the cursor, by 10% per mouse wheel tick
					ZoomTime(1.0 - ((double)e.Delta / 1200.0), e.Location);
	//			waveform.Invalidate();
			}
				else
				{
					// holding the control key zooms the horizontal axis, by 10% per mouse wheel tick
					Zoom(1.0 - ((double)e.Delta / 1200.0));
				}
			}
			else if (ModifierKeys.HasFlag(Keys.Shift)) {
				// holding the skift key moves the horizontal axis, by 10% of the visible time span per mouse wheel tick
				// wheel towards user   --> negative delta --> VisibleTimeStart increases
				// wheel away from user --> positive delta --> VisibleTimeStart decreases
				VisibleTimeStart += VisibleTimeSpan.Scale(-((double)e.Delta / 1200.0));
	//			waveform.Invalidate();
			}
			else {
				// moving the mouse wheel with no modifiers moves the display vertically, 40 pixels per mouse wheel tick
				VerticalOffset += -(e.Delta/3);
			}
		}

		#endregion
		private void zoomRowHeight(Point gridLocation, int delta)
		{
			//Changes Row height with the control shift and mouse scroll
			grid.BeginDraw();
			double zoomScale = delta < 0.0 ? 0.8 : 1.25;
			int waveFormHeight = 0;
			if (waveform.Audio != null)
				waveFormHeight = waveform.Height;
			int curheight = ruler.Height + waveFormHeight;

			foreach (Row row in Rows)
			{
				if (!row.Visible)
					continue;

				if (gridLocation.Y < curheight + row.Height)
				{
					selectedRow = row;
					break;
				}
				curheight += row.Height;
			}
			if (selectedRow != null) selectedRow.Height = (int)(selectedRow.Height * zoomScale);
			grid.EndDraw();
		}

		#region Overridden methods (On__)

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			//Console.WriteLine("Layout");
			if (grid != null)
			{
				timelineRowList.Top = grid.Top;
				timelineRowList.Height = grid.ClientSize.Height;
			}
			base.OnLayout(e);
		}

		protected override void OnPlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			// check if the playback cursor position would be over 90% of the grid width: if so, scroll the grid so it would be at 10%
			if (PlaybackCurrentTime.HasValue) {
				if (PlaybackCurrentTime.Value > VisibleTimeStart + VisibleTimeSpan.Scale(0.9))
					VisibleTimeStart = PlaybackCurrentTime.Value - VisibleTimeSpan.Scale(0.1);

				if (PlaybackCurrentTime.Value < VisibleTimeStart)
					VisibleTimeStart = PlaybackCurrentTime.Value - VisibleTimeSpan.Scale(0.1);
			}

			base.OnPlaybackCurrentTimeChanged(sender, e);
		}

		#endregion
	}
}