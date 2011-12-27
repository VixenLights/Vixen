using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace CommonElements.Timeline
{
	[System.ComponentModel.DesignerCategory("")]    // Prevent this from showing up in designer.
	public class Ruler : TimelineControlBase
	{
		private const int minPxBetweenTimeLabels = 10;
		private const int maxDxForClick = 2;


		public Ruler(TimeInfo timeinfo)
			:base(timeinfo)
		{
			BackColor = Color.Gray;
			recalculate();
		}

		private Font m_font = null;
		private Brush m_textBrush = null;

		private TimeSpan m_MinorTick;
		private int m_minorTicksPerMajor;

		private TimeSpan MinorTick { get { return m_MinorTick; } }
		private TimeSpan MajorTick { get { return m_MinorTick.Scale(m_minorTicksPerMajor); } }


		protected override Size DefaultSize
		{
			get { return new Size(400, 40); }
		}


		#region Drawing

		protected override void OnPaint(PaintEventArgs e)
        {
			try
			{
				// Translate the graphics to work the same way the timeline grid does
				// (ie. Drawing coordinates take into account where we start at in time)
				e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

				drawTicks(e.Graphics, MajorTick, 2, 0.5);
				drawTicks(e.Graphics, MinorTick, 1, 0.25);
				drawTimes(e.Graphics);

				using (Pen p = new Pen(Color.Black, 2)) {
					e.Graphics.DrawLine(p, 0, Height - 1, timeToPixels(TotalTime), Height - 1);
				}

				drawPlaybackIndicators(e.Graphics);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in Timeline.Ruler.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace);
			}
        }

		private void drawTicks(Graphics graphics, TimeSpan interval, int width, double height)
        {
            Single pxint = timeToPixels(interval);

            // calculate first tick - (it is the first multiple of interval greater than start)
            // believe it or not, this math is correct :-)
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart) % pxint) + pxint;
            Single end = timeToPixels(VisibleTimeEnd);

            for (Single x = start; x <= end; x += pxint)   
            {
                Pen p = new Pen(Color.Black);
                p.Width = width;
                p.Alignment = PenAlignment.Right;
                graphics.DrawLine(p, x, (Single)(Height * (1.0-height)), x, Height);

            }
        }

		private void drawTimes(Graphics graphics)
		{
			SizeF stringSize;
			int lastPixel = 0;

			// calculate the width of a single time, and figure out how regularly we will be able
			// to display times without overlapping. Then we can make sure we only use those intervals
			// to draw strings.
			stringSize = graphics.MeasureString(labelString(VisibleTimeEnd), m_font);
			int timeDisplayInterval = (int)((stringSize.Width + minPxBetweenTimeLabels) / timeToPixels(MajorTick)) + 1;
			TimeSpan drawnInterval = TimeSpan.FromTicks(MajorTick.Ticks * timeDisplayInterval);

			// get the time of the first tick that is: visible, on a major tick interval, and a multiple of the number of interval ticks
			TimeSpan firstMajor = TimeSpan.FromTicks(VisibleTimeStart.Ticks - (VisibleTimeStart.Ticks % drawnInterval.Ticks) + drawnInterval.Ticks);

			for (TimeSpan curTime = firstMajor;             // start at the first major tick
				(curTime <= VisibleTimeEnd);                // current time is in the visible region
				curTime += drawnInterval)                   // increment by the drawnInterval
			{
				string timeStr = labelString(curTime);

				stringSize = graphics.MeasureString(timeStr, m_font);
				Single posOffset = (stringSize.Width / 2);
				Single curPixelCentre = timeToPixels(curTime);

				// if drawing the string wouldn't overlap the last, then draw it
				if (lastPixel + minPxBetweenTimeLabels + posOffset < curPixelCentre)
				{
					graphics.DrawString(timeStr, m_font, m_textBrush, curPixelCentre - posOffset, (Height / 4) - (stringSize.Height / 2));
					lastPixel = (int)(curPixelCentre + posOffset);
				}
			}
		}

		private const int ArrowBase = 16;
		private const int ArrowLength = 10;

		private void drawPlaybackIndicators(Graphics g)
		{
			// Playback start/end arrows
			if (PlaybackStartTime.HasValue || PlaybackEndTime.HasValue)
			{
				GraphicsState gstate = g.Save();
				g.TranslateTransform(0, -ArrowBase / 2);

				if (PlaybackStartTime.HasValue)
				{
					// start arrow (faces left)  |<|
					int x = (int)timeToPixels(PlaybackStartTime.Value);
					g.FillPolygon(Brushes.DarkGray, new Point[] {
						new Point(x, Height-ArrowBase/2),				// left mid point
						new Point(x+ArrowLength, Height-ArrowBase),	// right top point
						new Point(x+ArrowLength, Height)					// right bottom point
					});
					g.DrawLine(Pens.DarkGray, x, Height - ArrowBase, x, Height);
				}

				if (PlaybackEndTime.HasValue)
				{
					// end arrow (faces right)   |>|
					int x = (int)timeToPixels(PlaybackEndTime.Value);
					g.FillPolygon(Brushes.DarkGray, new Point[] {
						new Point(x, Height-ArrowBase/2),				// right mid point
						new Point(x-ArrowLength, Height-ArrowBase),	// left top point
						new Point(x-ArrowLength, Height)					// left bottom point
					});
					g.DrawLine(Pens.DarkGray, x, Height - ArrowBase, x, Height);
				}

				if (PlaybackStartTime.HasValue && PlaybackEndTime.HasValue)
				{
					// line between the two
					using (Pen p = new Pen(Color.DarkGray))
					{
						p.Width = 4;
						int x1 = (int)timeToPixels(PlaybackStartTime.Value) + ArrowLength;
						int x2 = (int)timeToPixels(PlaybackEndTime.Value) - ArrowLength;
						int y = Height - ArrowBase / 2;
						g.DrawLine(p, x1, y, x2, y);
					}
				}

				g.Restore(gstate);
			}

			// Current position arrow
			if (PlaybackCurrentTime.HasValue)
			{
				int x = (int)timeToPixels(PlaybackCurrentTime.Value);
				g.FillPolygon(Brushes.Green, new Point[] {
					new Point(x, ArrowLength),		// bottom mid point
					new Point(x-ArrowBase/2, 0),	// top left point
					new Point(x+ArrowBase/2, 0),	// top right point
				});

			}
		}

		#endregion


		protected override void OnResize(EventArgs e)
		{
			recalculate();
			base.OnResize(e);
		}

		protected override void  TimePerPixelChanged(object sender, EventArgs e)
		{
			recalculate();
			Invalidate();
		}

		protected override void VisibleTimeStartChanged(object sender, EventArgs e)
		{
			// not ideal, but looks a *shitload* better.
			Refresh(); 
		}


		// Adapted from from Audacity, Ruler.cpp
		private void recalculate()
		{
			// Calculate the correct font size based on height
			int desiredPixelHeight = (this.Size.Height / 3);

			if (m_font != null)
				m_font.Dispose();
			m_font = new Font("Arial", desiredPixelHeight, GraphicsUnit.Pixel);


			if (m_textBrush != null)
				m_textBrush.Dispose();
			m_textBrush = new SolidBrush(Color.White);


			// As a heuristic, we want at least 10 pixels between each minor tick
			var t = pixelsToTime(10);

			if (t.TotalSeconds > 0.05)
			{
				if (t.TotalSeconds < 0.1)
				{
					m_MinorTick = TimeSpan.FromMilliseconds(100);
					m_minorTicksPerMajor = 5;
				} else if (t.TotalSeconds < 0.25)
				{
					m_MinorTick = TimeSpan.FromMilliseconds(250);
					m_minorTicksPerMajor = 4;
				} else if (t.TotalSeconds < 0.5)
				{
					m_MinorTick = TimeSpan.FromMilliseconds(500);
					m_minorTicksPerMajor = 4;
				} else if (t.TotalSeconds < 1)
				{
					m_MinorTick = TimeSpan.FromSeconds(1);
					m_minorTicksPerMajor = 5;
				} else if (t.TotalSeconds < 5)
				{
					m_MinorTick = TimeSpan.FromSeconds(5);
					m_minorTicksPerMajor = 6; //major = 30.0;
				}
				else if (t.TotalSeconds < 10)
				{
					m_MinorTick = TimeSpan.FromSeconds(10);
					m_minorTicksPerMajor = 6; //major = 60.0;
				}
				else if (t.TotalSeconds < 15)
				{
					m_MinorTick = TimeSpan.FromSeconds(15);
					m_minorTicksPerMajor = 4; //major = 60.0;
				}
				else if (t.TotalSeconds < 30)
				{
					m_MinorTick = TimeSpan.FromSeconds(30);
					m_minorTicksPerMajor = 4; //major = 120.0;
				}
				else if (t.TotalMinutes < 1)
				{
					m_MinorTick = TimeSpan.FromMinutes(1);
					m_minorTicksPerMajor = 5;	//major = 300.0;
				}
				else if (t.TotalMinutes < 5)
				{
					m_MinorTick = TimeSpan.FromMinutes(5);
					m_minorTicksPerMajor = 3;	//major = 900.0;
				}
				else if (t.TotalMinutes < 10)
				{
					m_MinorTick = TimeSpan.FromMinutes(10);
					m_minorTicksPerMajor = 3;	//major = 1800.0;
				}
				else if (t.TotalMinutes < 15)
				{
					m_MinorTick = TimeSpan.FromMinutes(15);
					m_minorTicksPerMajor = 4;	//major = 3600.0;
				}
				else if (t.TotalMinutes < 30)
				{
					m_MinorTick = TimeSpan.FromMinutes(30);
					m_minorTicksPerMajor = 2;	//major = 3600.0;
				}
				else if (t.TotalHours < 1)
				{
					m_MinorTick = TimeSpan.FromHours(1);
					m_minorTicksPerMajor = 6;	//major = 6 * 3600.0;
				}
				else if (t.TotalHours < 6)
				{
					m_MinorTick = TimeSpan.FromHours(6);
					m_minorTicksPerMajor = 4;	//major = 24 * 3600.0;
				}
				else if (t.TotalDays < 1)
				{
					m_MinorTick = TimeSpan.FromDays(1);
					m_minorTicksPerMajor = 7;	//major = 7 * 24 * 3600.0;
				}
				else
				{
					m_MinorTick = TimeSpan.FromDays(7);
					m_minorTicksPerMajor = 1;	//major = 24.0 * 7.0 * 3600.0;
				}
			}
			else
			{
				// Fractional seconds
				double d = 0.000001;
				for (;;)
				{
					if (t.TotalSeconds < d)
					{
						m_MinorTick = TimeSpan.FromTicks((long)(TimeSpan.TicksPerMillisecond * 1000 * d));
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						break;
					}
					d *= 5.0;
					if (t.TotalSeconds < d)
					{
						m_MinorTick = TimeSpan.FromTicks((long)(TimeSpan.TicksPerMillisecond * 1000 * d));
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						break;
					}
					d *= 5.0;
					if (t.TotalSeconds < d)
					{
						m_MinorTick = TimeSpan.FromTicks((long)(TimeSpan.TicksPerMillisecond * 1000 * d));
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

			if (m_MinorTick >= TimeSpan.FromHours(1))
			{
				// Round time to nearest hour
				t = TimeSpan.FromHours((int)t.TotalHours);
				timeFormat = @"h\:mm";
			}
			else if (m_MinorTick >= TimeSpan.FromMinutes(1))
			{
				// Round time to nearest minute
				t = TimeSpan.FromMinutes((int)t.TotalMinutes);

				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss";
				else
					timeFormat = @"m\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromSeconds(1))
			{
				// Round time to nearest second
				t = TimeSpan.FromSeconds((int)t.TotalSeconds);

				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss";
				else
					timeFormat = @"m\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(100))
			{
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.f";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.f";
				else
					timeFormat = @"s\.f";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(10))
			{
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.ff";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.ff";
				else
					timeFormat = @"s\.ff";
			}
			else if (m_MinorTick >= TimeSpan.FromMilliseconds(1))
			{
				if (t >= TimeSpan.FromHours(1))
					timeFormat = @"h\:mm\:ss\.fff";
				else if (t >= TimeSpan.FromMinutes(1))
					timeFormat = @"m\:ss\.fff";
				else
					timeFormat = @"s\.fff";
			}
			else
			{
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

		private enum MouseState { Normal, DragWait, Dragging }
		private MouseState m_mouseState = MouseState.Normal;

		private int m_mouseDownX;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_mouseState = MouseState.DragWait;
			m_mouseDownX = e.X;
			PlaybackStartTime = pixelsToTime(e.X) + VisibleTimeStart;
			PlaybackEndTime = null;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			switch (m_mouseState)
			{
				case MouseState.Normal:
					return;

				case MouseState.DragWait:
					// Move enough to be considered a drag?
					if (Math.Abs(e.X - m_mouseDownX) <= maxDxForClick)
						return;
					m_mouseState = MouseState.Dragging;
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

				default:
					throw new Exception("Invalid MouseState. WTF?!");
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			switch (m_mouseState)
			{
				case MouseState.Normal:
					throw new Exception("MouseUp in MouseState.Normal - WTF?");

				case MouseState.DragWait:
					// Didn't move enough to be considered dragging. Just a click.
					if (ClickedAtTime != null)
						ClickedAtTime(this, new RulerClickedEventArgs(PlaybackStartTime.Value, Form.ModifierKeys));
					break;

				case MouseState.Dragging:
					// Finished a time range drag.
					if (DraggedTimeRange != null)
						DraggedTimeRange(this, new TimeRangeDraggedEventArgs(PlaybackStartTime.Value, PlaybackEndTime.Value));
					break;

				default:
					throw new Exception("Invalid MouseState. WTF?!");
			}

			m_mouseState = MouseState.Normal;
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
		}

		public event EventHandler<RulerClickedEventArgs> ClickedAtTime;
		public event EventHandler<TimeRangeDraggedEventArgs> DraggedTimeRange;

		#endregion




		#region Public Properties

		private TimeSpan? m_playbackStart = null;
		public TimeSpan? PlaybackStartTime
		{
			get { return m_playbackStart; }
			set { m_playbackStart = value; Invalidate(); }
		}

		private TimeSpan? m_playbackEnd = null;
		public TimeSpan? PlaybackEndTime
		{
			get { return m_playbackEnd; }
			set { m_playbackEnd = value; Invalidate(); }
		}

		private TimeSpan? m_playbackCur = null;
		public TimeSpan? PlaybackCurrentTime
		{
			get { return m_playbackCur; }
			set { m_playbackCur = value; Invalidate(); }
		}

		#endregion

	}


	public class TimeRangeDraggedEventArgs : EventArgs
	{
		public TimeRangeDraggedEventArgs(TimeSpan start, TimeSpan end)
		{
			StartTime = start;
			EndTime = end;
		}
		public TimeSpan StartTime { get; private set; }
		public TimeSpan EndTime { get; private set; }
		
	}

	public class RulerClickedEventArgs : EventArgs
	{
		public RulerClickedEventArgs(TimeSpan time, Keys modifiers)
		{
			Time = time;
			ModifierKeys = modifiers;
		}
		public TimeSpan Time { get; private set; }
		public Keys ModifierKeys { get; private set; }
	}
}
