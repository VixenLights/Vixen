using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Sys.Marks;

namespace Common.Controls.Timeline
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public class Ruler : TimelineControlBase
	{
		private const int minPxBetweenTimeLabels = 10;
		private const int maxDxForClick = 2;
		private readonly int _arrowBase;
		private readonly int _arrowLength;
		private TimeSpan _dragStartTime;
		private TimeSpan _dragLastTime;

		private readonly List<MarkCollection> _labledMarks;


		public Ruler(TimeInfo timeinfo)
			: base(timeinfo)
		{
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.Gray;
			recalculate();
			_labledMarks = new List<MarkCollection>();
			double factor = ScalingTools.GetScaleFactor();
			_arrowBase = (int) (16 * factor);
			_arrowLength = (int)(10 * factor);
		}

		private Font m_font = null;
		private Brush m_textBrush = null;

		private TimeSpan m_MinorTick;
		private int m_minorTicksPerMajor;

		private TimeSpan MinorTick
		{
			get { return m_MinorTick; }
		}

		private TimeSpan MajorTick
		{
			get { return m_MinorTick.Scale(m_minorTicksPerMajor); }
		}


		protected override Size DefaultSize
		{
			get { return new Size(400, 40); }
		}

		public int StandardNudgeTime { get; set; }

		public int SuperNudgeTime { get; set; }

		public int SnapStrength { get; set; }

		#region Drawing

		protected override void OnPaint(PaintEventArgs e)
		{
			try {
				// Translate the graphics to work the same way the timeline grid does
				// (ie. Drawing coordinates take into account where we start at in time)
				e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

				drawTicks(e.Graphics, MajorTick, 2, 0.4);
				drawTicks(e.Graphics, MinorTick, 1, 0.20);
				drawTimes(e.Graphics);

				using (Pen p = new Pen(Color.Black, 2))
				{
					e.Graphics.DrawLine(p, 0, Height - 1, timeToPixels(TotalTime), Height - 1);
				}

				drawPlaybackIndicators(e.Graphics);

				_drawMarks(e.Graphics);
				
			}
			catch (Exception ex) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Exception in Timeline.Ruler.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace,
					@"Error", false, false);
				messageBox.ShowDialog();
			}
		}

		private void drawTicks(Graphics graphics, TimeSpan interval, int width, double height)
		{
			Single pxint = timeToPixels(interval);

			// calculate first tick - (it is the first multiple of interval greater than start)
			// believe it or not, this math is correct :-)
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart)%pxint) + pxint;
			Single end = timeToPixels(VisibleTimeEnd);

			Pen p = new Pen(Color.Black);
			p.Width = width;
			p.Alignment = PenAlignment.Right;

			for (Single x = start; x <= end; x += pxint) {	
				graphics.DrawLine(p, x, (Single) (Height*(1.0 - height)), x, Height);
			}
		}

		private string markTime;

		private void drawTimes(Graphics graphics)
		{
			SizeF stringSize;
			int lastPixel = 0;

			// calculate the width of a single time, and figure out how regularly we will be able
			// to display times without overlapping. Then we can make sure we only use those intervals
			// to draw strings.
			stringSize = graphics.MeasureString(labelString(VisibleTimeEnd), m_font);
			int timeDisplayInterval = (int) ((stringSize.Width + minPxBetweenTimeLabels)/timeToPixels(MajorTick)) + 1;
			TimeSpan drawnInterval = TimeSpan.FromTicks(MajorTick.Ticks*timeDisplayInterval);

			// get the time of the first tick that is: visible, on a major tick interval, and a multiple of the number of interval ticks
			TimeSpan firstMajor =
				TimeSpan.FromTicks(VisibleTimeStart.Ticks - (VisibleTimeStart.Ticks%drawnInterval.Ticks) + drawnInterval.Ticks);

			for (TimeSpan curTime = firstMajor;
			     // start at the first major tick
			     (curTime <= VisibleTimeEnd);
			     // current time is in the visible region
			     curTime += drawnInterval) // increment by the drawnInterval
			{
				string timeStr = labelString(curTime);

				stringSize = graphics.MeasureString(timeStr, m_font);
				Single posOffset = (stringSize.Width/2);
				Single curPixelCentre = timeToPixels(curTime);

				// if drawing the string wouldn't overlap the last, then draw it
				if (lastPixel + minPxBetweenTimeLabels + posOffset < curPixelCentre) {
					graphics.DrawString(timeStr, m_font, m_textBrush, curPixelCentre - posOffset, ((Height/4) - (stringSize.Height/2)) + 9);
					lastPixel = (int) (curPixelCentre + posOffset);
				}
			}
			
		}

		private void drawPlaybackIndicators(Graphics g)
		{
			// Playback start/end arrows
			if (PlaybackStartTime.HasValue || PlaybackEndTime.HasValue) {
				GraphicsState gstate = g.Save();
				g.TranslateTransform(0, -_arrowBase/2);

				if (PlaybackStartTime.HasValue) {
					// start arrow (faces left)  |<|
					int x = (int) timeToPixels(PlaybackStartTime.Value);
					g.FillPolygon(Brushes.DarkGray, new Point[]
					                                	{
					                                		new Point(x, Height - _arrowBase/2), // left mid point
					                                		new Point(x + _arrowLength, Height - _arrowBase), // right top point
					                                		new Point(x + _arrowLength, Height) // right bottom point
					                                	});
					g.DrawLine(Pens.DarkGray, x, Height - _arrowBase, x, Height);
				}

				if (PlaybackEndTime.HasValue) {
					// end arrow (faces right)   |>|
					int x = (int) timeToPixels(PlaybackEndTime.Value);
					g.FillPolygon(Brushes.DarkGray, new Point[]
					                                	{
					                                		new Point(x, Height - _arrowBase/2), // right mid point
					                                		new Point(x - _arrowLength, Height - _arrowBase), // left top point
					                                		new Point(x - _arrowLength, Height) // left bottom point
					                                	});
					g.DrawLine(Pens.DarkGray, x, Height - _arrowBase, x, Height);
				}

				if (PlaybackStartTime.HasValue && PlaybackEndTime.HasValue) {
					// line between the two
					using (Pen p = new Pen(Color.DarkGray)) {
						p.Width = 4;
						int x1 = (int) timeToPixels(PlaybackStartTime.Value) + _arrowLength;
						int x2 = (int) timeToPixels(PlaybackEndTime.Value) - _arrowLength;
						int y = Height - _arrowBase/2;
						g.DrawLine(p, x1, y, x2, y);
					}
				}

				g.Restore(gstate);
			}

			// Current position arrow
			if (PlaybackCurrentTime.HasValue) {
				int x = (int) timeToPixels(PlaybackCurrentTime.Value);
				g.FillPolygon(Brushes.Green, new Point[]
				                             	{
				                             		new Point(x, _arrowLength), // bottom mid point
				                             		new Point(x - _arrowBase/2, 0), // top left point
				                             		new Point(x + _arrowBase/2, 0), // top right point
				                             	});
			}
		}

		#endregion

		protected override void OnResize(EventArgs e)
		{
			recalculate();
			base.OnResize(e);
		}

		protected override void OnTimePerPixelChanged(object sender, EventArgs e)
		{
			recalculate();
			base.OnTimePerPixelChanged(sender, e);
		}

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			// not ideal, but looks a *shitload* better.
			Refresh();
		}


		// Adapted from from Audacity, Ruler.cpp
		private void recalculate()
		{
			// Calculate the correct font size based on height
			int desiredPixelHeight = (this.Size.Height/3);

			if (m_font != null)
				m_font.Dispose();
			m_font = new Font(Font.FontFamily, desiredPixelHeight, GraphicsUnit.Pixel);

			if (m_textBrush != null)
				m_textBrush.Dispose();
			m_textBrush = new SolidBrush(Color.White);


			// As a heuristic, we want at least 10 pixels between each minor tick
			var t = pixelsToTime(10);

			if (t.TotalSeconds > 0.05) {
				if (t.TotalSeconds < 0.1) {
					m_MinorTick = TimeSpan.FromMilliseconds(100);
					m_minorTicksPerMajor = 5;
				}
				else if (t.TotalSeconds < 0.25) {
					m_MinorTick = TimeSpan.FromMilliseconds(250);
					m_minorTicksPerMajor = 4;
				}
				else if (t.TotalSeconds < 0.5) {
					m_MinorTick = TimeSpan.FromMilliseconds(500);
					m_minorTicksPerMajor = 4;
				}
				else if (t.TotalSeconds < 1) {
					m_MinorTick = TimeSpan.FromSeconds(1);
					m_minorTicksPerMajor = 5;
				}
				else if (t.TotalSeconds < 5) {
					m_MinorTick = TimeSpan.FromSeconds(5);
					m_minorTicksPerMajor = 6; //major = 30.0;
				}
				else if (t.TotalSeconds < 10) {
					m_MinorTick = TimeSpan.FromSeconds(10);
					m_minorTicksPerMajor = 6; //major = 60.0;
				}
				else if (t.TotalSeconds < 15) {
					m_MinorTick = TimeSpan.FromSeconds(15);
					m_minorTicksPerMajor = 4; //major = 60.0;
				}
				else if (t.TotalSeconds < 30) {
					m_MinorTick = TimeSpan.FromSeconds(30);
					m_minorTicksPerMajor = 4; //major = 120.0;
				}
				else if (t.TotalMinutes < 1) {
					m_MinorTick = TimeSpan.FromMinutes(1);
					m_minorTicksPerMajor = 5; //major = 300.0;
				}
				else if (t.TotalMinutes < 5) {
					m_MinorTick = TimeSpan.FromMinutes(5);
					m_minorTicksPerMajor = 3; //major = 900.0;
				}
				else if (t.TotalMinutes < 10) {
					m_MinorTick = TimeSpan.FromMinutes(10);
					m_minorTicksPerMajor = 3; //major = 1800.0;
				}
				else if (t.TotalMinutes < 15) {
					m_MinorTick = TimeSpan.FromMinutes(15);
					m_minorTicksPerMajor = 4; //major = 3600.0;
				}
				else if (t.TotalMinutes < 30) {
					m_MinorTick = TimeSpan.FromMinutes(30);
					m_minorTicksPerMajor = 2; //major = 3600.0;
				}
				else if (t.TotalHours < 1) {
					m_MinorTick = TimeSpan.FromHours(1);
					m_minorTicksPerMajor = 6; //major = 6 * 3600.0;
				}
				else if (t.TotalHours < 6) {
					m_MinorTick = TimeSpan.FromHours(6);
					m_minorTicksPerMajor = 4; //major = 24 * 3600.0;
				}
				else if (t.TotalDays < 1) {
					m_MinorTick = TimeSpan.FromDays(1);
					m_minorTicksPerMajor = 7; //major = 7 * 24 * 3600.0;
				}
				else {
					m_MinorTick = TimeSpan.FromDays(7);
					m_minorTicksPerMajor = 1; //major = 24.0 * 7.0 * 3600.0;
				}
			}
			else {
				// Fractional seconds
				double d = 0.000001;
				for (;;) {
					if (t.TotalSeconds < d) {
						m_MinorTick = TimeSpan.FromTicks((long) (TimeSpan.TicksPerMillisecond*1000*d));
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						break;
					}
					d *= 5.0;
					if (t.TotalSeconds < d) {
						m_MinorTick = TimeSpan.FromTicks((long) (TimeSpan.TicksPerMillisecond*1000*d));
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						break;
					}
					d *= 5.0;
					if (t.TotalSeconds < d) {
						m_MinorTick = TimeSpan.FromTicks((long) (TimeSpan.TicksPerMillisecond*1000*d));
						m_minorTicksPerMajor = 4; //major = d * 4.0;
						break;
					}
					d *= 4.0;
				}
			}

			//Debug.WriteLine("update():  t={0}    minor={1}   minPerMaj={2}", t, m_MinorTick, m_minorTicksPerMajor);
		}

		private string labelString(TimeSpan t)
		{
			// Adapted from from Audacity, Ruler.cpp

			string timeFormat = string.Empty;

			if (m_MinorTick >= TimeSpan.FromHours(1)) {
				// Round time to nearest hour
				t = TimeSpan.FromHours((int) t.TotalHours);
				timeFormat = @"h\:mm";
			}
			else if (m_MinorTick >= TimeSpan.FromMinutes(1)) {
				// Round time to nearest minute
				t = TimeSpan.FromMinutes((int) t.TotalMinutes);

				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss";
				else
					timeFormat = @"m\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromSeconds(1)) {
				// Round time to nearest second
				t = TimeSpan.FromSeconds((int) t.TotalSeconds);

				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss";
				else
					timeFormat = @"m\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(100)) {
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.f";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.f";
				else
					timeFormat = @"s\.f";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(10)) {
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.ff";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.ff";
				else
					timeFormat = @"s\.ff";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(1)) {
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.fff";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.fff";
				else
					timeFormat = @"s\.fff";
			}
			else {
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.ffffff";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.ffffff";
				else
					timeFormat = @"s\.ffffff";
			}

			return t.ToString(timeFormat);
		}

		#region Mouse

		private enum MouseState
		{
			Normal,
			DragWait,
			Dragging,
			DraggingMark,
			ResizeRuler
		}

		private MouseState m_mouseState = MouseState.Normal;

		private int m_mouseDownX;
		private MouseButtons m_button;
		private TimeSpan m_mark;
		private Mark _selectedMark = null;
		//public SortedDictionary<TimeSpan, SnapDetails> selectedMarks = new SortedDictionary<TimeSpan, SnapDetails>();
		public List<Mark> SelectedMarks = new List<Mark>();
		protected override void OnMouseDown(MouseEventArgs e)
		{
			//Console.WriteLine("Clicks: " + e.Clicks);

			m_button = e.Button;
			m_mouseDownX = e.X;
			if (e.Button != MouseButtons.Left) return;

			// If we're hovering over a mark when left button is clicked, then select/move the mark 
			var marksAtTime = MarksAt(pixelsToTime(e.X) + VisibleTimeStart);
			if (marksAtTime.Any())
			{
				if (ModifierKeys != Keys.Control)
				{
					ClearSelectedMarks();
				}

				if (SelectedMarks.Contains(marksAtTime.First()))
				{
					SelectedMarks.Remove(marksAtTime.First());
				}
				else
				{
					SelectedMarks.Add(marksAtTime.First());
				}
				_dragStartTime = _dragLastTime = marksAtTime.First().StartTime;
				m_mouseState = MouseState.DraggingMark;
			}
			else if (Cursor == Cursors.HSplit)
				m_mouseState = MouseState.ResizeRuler;
			else
			{
				ClearSelectedMarks();
				m_mouseState = MouseState.DragWait;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (m_button == System.Windows.Forms.MouseButtons.Left)
			{
				switch (m_mouseState)
				{
					case MouseState.Normal:
						return;

					case MouseState.DragWait:
						// Move enough to be considered a drag?
						if (Math.Abs(e.X - m_mouseDownX) <= maxDxForClick)
							return;
						m_mouseState = MouseState.Dragging;
						OnBeginDragTimeRange();
						PlaybackStartTime = pixelsToTime(e.X) + VisibleTimeStart;
						PlaybackEndTime = null;
						goto case MouseState.Dragging;

					case MouseState.Dragging:
						int start, end;
						if (e.X > m_mouseDownX)
						{
							// Start @ mouse down, end @ mouse current
							start = m_mouseDownX;
							end = e.X;
						}
						else
						{
							// Start @ mouse current, end @ mouse down
							start = e.X;
							end = m_mouseDownX;
						}

						PlaybackStartTime = pixelsToTime(start) + VisibleTimeStart;
						PlaybackEndTime = pixelsToTime(end) + VisibleTimeStart;
						return;
					case MouseState.DraggingMark:

						var newTime = pixelsToTime(e.X) + VisibleTimeStart;
						OnMarksMoving(new MarksMovingEventArgs(SelectedMarks));
						var diff = newTime - _dragLastTime;
						SelectedMarks.ForEach(x => x.StartTime += diff);
						_dragLastTime = newTime;
						if (Convert.ToInt16(m_mark.Minutes) >= 10)
						{
							markTime = string.Format("{0:mm\\:ss\\.fff}", newTime);
						}
						else if (Convert.ToInt16(m_mark.Minutes) >= 1)
						{
							markTime = string.Format("{0:m\\:ss\\.fff}", newTime);
						}
						else if (Convert.ToInt16(m_mark.Seconds) >= 10)
						{
							markTime = string.Format("{0:ss\\.fff}", newTime);
						}
						else if (Convert.ToInt16(m_mark.Seconds) <= 9)
						{
							markTime = string.Format("{0:s\\.fff}", newTime);
						}

						Refresh();
						
						OnSelectedMarkMove(new SelectedMarkMoveEventArgs(true, m_mark));
						
						break;
					case MouseState.ResizeRuler:
						//Adjusts Ruler Height
						if (e.Location.Y > 40)
							Height = e.Location.Y + 1;
						break;
					default:
						throw new Exception("Invalid MouseState. WTF?!");
				}
			}
			else
			{
				// We'll get to this point if there is no mouse button selected
				var marks = MarksAt(pixelsToTime(e.X) + VisibleTimeStart);
				if (marks.Any())
				{
					Cursor = Cursors.VSplit;
					OnSelectedMarkMove(new SelectedMarkMoveEventArgs(true, marks.First().StartTime));
				}
				else if (e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6)
				{
					Cursor = Cursors.HSplit;
				}
				else
				{
					Cursor = Cursors.Hand;
					OnSelectedMarkMove(new SelectedMarkMoveEventArgs(false, TimeSpan.Zero));
				}
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			//Resets Ruler Height to default value of 50 when you double click the HSplit
			if (Cursor == Cursors.HSplit)
			{
				Height = 50;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Clicks == 2)
			{
				// Add a mark
				OnClickedAtTime(new RulerClickedEventArgs(pixelsToTime(e.X) + VisibleTimeStart, Form.ModifierKeys, m_button));
			}
			else
			{
				if (m_button == MouseButtons.Left)
				{
					switch (m_mouseState)
					{
						case MouseState.Normal:
							break; // this is okay and will happen
						//throw new Exception("MouseUp in MouseState.Normal - WTF?");

						case MouseState.DragWait:
							// Didn't move enough to be considered dragging. Just a click.
							OnClickedAtTime(new RulerClickedEventArgs(pixelsToTime(e.X) + VisibleTimeStart, Form.ModifierKeys, m_button));
							break;

						case MouseState.Dragging:
							// Finished a time range drag.
							OnTimeRangeDragged(new ModifierKeysEventArgs(Form.ModifierKeys));
							break;
						case MouseState.DraggingMark:
							if (_selectedMark != null)
							{
								// Did we move the mark?
								if (e.X != m_mouseDownX)
								{
									//ClearSelectedMarks();
									var newTime = pixelsToTime(e.X) + VisibleTimeStart;
									OnMarkMoved(new MarksMovedEventArgs(_dragStartTime - newTime, SelectedMarks));
									OnSelectedMarkMove(new SelectedMarkMoveEventArgs(false, newTime));
								}
							}
							break;
						case MouseState.ResizeRuler:
							break;
						default:
							throw new Exception("Invalid MouseState. WTF?!");
					}
				}
				else if (m_button == MouseButtons.Right)
				{
					var marks = MarksAt(pixelsToTime(e.X) + VisibleTimeStart);
					if (marks.Any())
					{
						// See if we got a right-click on top of a mark.
						if (e.X == m_mouseDownX)
						{
							ContextMenuStrip c = new ContextMenuStrip();
							c.Renderer = new ThemeToolStripRenderer();
							c.Items.Add("&Delete Selected Marks");
							c.Click += DeleteMark_Click;
							c.Show(this, new Point(e.X, e.Y));
						}
					}
					// Otherwise, we've added a mark
					else
					{
						OnClickedAtTime(new RulerClickedEventArgs(pixelsToTime(e.X) + VisibleTimeStart, Form.ModifierKeys, m_button));
					}
				}
			}
			m_mouseState = MouseState.Normal;
			m_button = MouseButtons.None;
			Invalidate();
		}

		public void ClearSelectedMarks()
		{
			SelectedMarks.Clear();
		}

		public void NudgeMark(int offset)
		{
			//TimeSpan timeOffset = TimeSpan.FromMilliseconds(offset);

			//OnMarkNudge(new MarkNudgeEventArgs(selectedMarks, timeOffset));

			//SortedDictionary<TimeSpan, SnapDetails> newSelectedMarks = new SortedDictionary<TimeSpan, SnapDetails>();

			//foreach (KeyValuePair<TimeSpan, SnapDetails> kvp in selectedMarks)
			//{
			//	newSelectedMarks.Add(kvp.Key + timeOffset, kvp.Value);
			//}

			//selectedMarks = newSelectedMarks;
		}

		void DeleteMark_Click(object sender, EventArgs e)
		{
			//DeleteSelectedMarks();

			OnDeleteMark(new MarksDeletedEventArgs(SelectedMarks));
		}

		public void DeleteSelectedMarks()
		{

			

		}

		protected override void OnMouseEnter(EventArgs e)
		{
			Cursor = Cursors.Hand;
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			Cursor = Cursors.Default;
			base.OnMouseLeave(e);
			OnSelectedMarkMove(new SelectedMarkMoveEventArgs(false, TimeSpan.Zero));
		}

		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;
		public event EventHandler<MarkNudgeEventArgs> MarkNudge;
		public event EventHandler<MarksDeletedEventArgs> DeleteMark;
		public event EventHandler<RulerClickedEventArgs> ClickedAtTime;
		public event EventHandler<ModifierKeysEventArgs> TimeRangeDragged;
		public event EventHandler BeginDragTimeRange;
		public static event EventHandler<SelectedMarkMoveEventArgs> SelectedMarkMove;

		public virtual void OnSelectedMarkMove(SelectedMarkMoveEventArgs e)
		{
			if (SelectedMarkMove != null)
				SelectedMarkMove(this, e);
		}

		protected virtual void OnMarkMoved(MarksMovedEventArgs e)
		{
			if (MarksMoved != null)
				MarksMoved(this, e);
		}

		protected virtual void OnMarksMoving(MarksMovingEventArgs e)
		{
			MarksMoving?.Invoke(this, e);
		}

		protected virtual void OnMarkNudge(MarkNudgeEventArgs e)
		{
			if (MarkNudge != null)
				MarkNudge(this, e);
		}

		protected virtual void OnDeleteMark(MarksDeletedEventArgs e)
		{
			if (DeleteMark != null)
				DeleteMark(this, e);
		}

		protected virtual void OnClickedAtTime(RulerClickedEventArgs e)
		{
			if (ClickedAtTime != null)
				ClickedAtTime(this, e);
		}

		protected virtual void OnTimeRangeDragged(ModifierKeysEventArgs e)
		{
			if (TimeRangeDragged != null)
				TimeRangeDragged(this, e);
		}

		protected virtual void OnBeginDragTimeRange()
		{
			if (BeginDragTimeRange != null)
				BeginDragTimeRange(this, EventArgs.Empty);
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (m_font != null)
					m_font.Dispose();
				if (m_textBrush != null)
					m_textBrush.Dispose();
				m_font = null;
				m_textBrush = null;
			}
			base.Dispose(disposing);
		}

		#region "Snap Points (Marks)"

		public void AddMarks(MarkCollection labeledMarkCollection)
		{
			_labledMarks.Add(labeledMarkCollection);
			labeledMarkCollection.EnsureOrder();
			if (!SuppressInvalidate)
			{
				Invalidate();
			}
		}

		public void ClearMarks()
		{
			_labledMarks.Clear();
			if (!SuppressInvalidate) Invalidate();
		}

		/// <summary>
		/// Returns a list of marks at a time ordered by level from highest to lowest.
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		private List<Mark> MarksAt(TimeSpan ts)
		{
			const int markDifferential = 20;
			var marksAtTime = new List<Mark>();
			foreach (var labeledMarkCollection in _labledMarks.Where(x => x.IsEnabled).OrderByDescending(x => x.Level))
			{
				labeledMarkCollection.EnsureOrder();
				foreach (var labeledMark in labeledMarkCollection.Marks)
				{
					TimeSpan markStart = TimeSpan.FromMilliseconds(labeledMark.StartTime.TotalMilliseconds - (markDifferential));
					TimeSpan markEnd = TimeSpan.FromMilliseconds(labeledMark.StartTime.TotalMilliseconds + (markDifferential));
					if (ts >= markStart && ts <= markEnd)
					{
						marksAtTime.Add(labeledMark);
					}
					else if (labeledMark.StartTime > ts)
					{
						break;
					}
				}
			}

			return marksAtTime;
		}

		private void _drawMarks(Graphics g)
		{
			Pen p;

			// iterate through all marks, and if it's visible, draw it
			foreach (var labeledMarkCollection in _labledMarks.Where(x => x.IsEnabled))
			{
				int lineBold = 1;
				if (labeledMarkCollection.Decorator.IsBold)
				{
					lineBold = 3;
				}

				p = new Pen(labeledMarkCollection.Decorator.Color, lineBold);
				if (!labeledMarkCollection.Decorator.IsSolidLine)
				{
					p.DashPattern = new float[] {labeledMarkCollection.Level, labeledMarkCollection.Level};
				}

				foreach (var labeledMark in labeledMarkCollection.Marks)
				{
					//Only draw visible marks
					if (labeledMark.StartTime < VisibleTimeStart)
					{
						continue;
					}
					if (labeledMark.StartTime > VisibleTimeEnd)
					{
						break;
					}
					Single x = timeToPixels(labeledMark.StartTime);
					p.Width = SelectedMarks.Contains(labeledMark) ? 3 : 1;
					g.DrawLine(p, x, 0, x, Height);
				}

				p.Dispose();
			}

			if (m_button == MouseButtons.Left && m_mouseState == MouseState.DraggingMark)
			{
				p = new Pen(Brushes.Yellow) {DashPattern = new float[] {2, 2}};
				TimeSpan newMarkPosition = pixelsToTime(PointToClient(new Point(MousePosition.X, MousePosition.Y)).X) + VisibleTimeStart;
				Single x = timeToPixels(newMarkPosition);
				g.DrawLine(p, x, 0, x, Height);
				p.Dispose();

				//Draws the time next to the selected mark that is being moved.
				Font drawFont = new Font("Arial", 8, FontStyle.Bold);
				SolidBrush drawBrush = new SolidBrush(Color.White);
				StringFormat drawFormat = new StringFormat();
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
				g.DrawString(markTime, drawFont, drawBrush, x, 0, drawFormat);
				drawFont.Dispose();
				drawBrush.Dispose();
				drawFormat.Dispose();
			}
		}

		#endregion

		public void BeginDraw()
		{
			SuppressInvalidate = true;
		}

		public void EndDraw()
		{
			SuppressInvalidate = false;
			Invalidate();
		}

		public bool SuppressInvalidate { get; set; }
	}

	public class TimeRangeDraggedEventArgs : EventArgs
	{
		public TimeRangeDraggedEventArgs(TimeSpan start, TimeSpan end, Keys modifiers)
		{
			StartTime = start;
			EndTime = end;
			ModifierKeys = modifiers;
		}

		public TimeSpan StartTime { get; private set; }
		public TimeSpan EndTime { get; private set; }
		public Keys ModifierKeys { get; private set; }
	}

	public class RulerClickedEventArgs : EventArgs
	{
		public RulerClickedEventArgs(TimeSpan time, Keys modifiers, MouseButtons button)
		{
			Time = time;
			ModifierKeys = modifiers;
			Button = button;
		}

		public TimeSpan Time { get; private set; }
		public Keys ModifierKeys { get; private set; }
		public MouseButtons Button { get; private set; }
	}
}