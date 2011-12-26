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
		private Brush m_brush = null;

		private TimeSpan m_MinorTick;
		private int m_minorTicksPerMajor;

		public TimeSpan MinorTick { get { return m_MinorTick; } }
		public TimeSpan MajorTick { get { return m_MinorTick.Scale(m_minorTicksPerMajor); } }

		protected override Size DefaultSize
		{
			get { return new Size(400, 40); }
		}


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


			if (m_brush != null)
				m_brush.Dispose();
			m_brush = new SolidBrush(Color.White);


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
					graphics.DrawString(timeStr, m_font, m_brush, curPixelCentre - posOffset, (Height / 4) - (stringSize.Height / 2));
					lastPixel = (int)(curPixelCentre + posOffset);
				}
			}
		}


		#region Mouse

		private int m_mouseDownX;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Debug.WriteLine("MouseDown: x={0}", e.X);
			m_mouseDownX = e.X;
			
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Math.Abs(e.X - m_mouseDownX) <= maxDxForClick)
			{
				TimeSpan t = pixelsToTime(e.X) + VisibleTimeStart;
				if (ClickedAtTime != null)
					ClickedAtTime(this, new TimeSpanEventArgs(t));
			}
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

		public event EventHandler<TimeSpanEventArgs> ClickedAtTime;

		#endregion


	}
}
