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

namespace Timeline
{
	public partial class TimelineHeader : TimelineControlBase
	{
		public TimelineHeader()
		{
			recalculate();
		}

		private const int minPxBetweenTimeLabels = 10;

		private Font m_font = null;
		private Brush m_brush = null;

		private int m_digits;
		private TimeSpan m_MinorTick;
		private int m_minorTicksPerMajor;

		public TimeSpan MinorTick { get { return m_MinorTick; } }
		public TimeSpan MajorTick { get { return m_MinorTick.Scale(m_minorTicksPerMajor); } }

		
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, Size.Width, Size.Height);

            // Translate the graphics to work the same way the timeline grid does
            // (ie. Drawing coordinates take into account where we start at in time)
            e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

            drawTicks(e.Graphics, MajorTick, 2, 0.5);
            drawTicks(e.Graphics, MinorTick, 1, 0.25);
			drawTimes(e.Graphics);
        }

		/*
		private void drawTicksNEW(Graphics graphics)
		{
			Single visStart = timeToPixels(VisibleTimeStart);
			Single minTick = timeToPixels(m_MinorTick);
			Single majTick = minTick * m_minorTicksPerMajor;

			// calculate first minor and major tick locations
			//(first multiple of interval greater than start)
			var firstMin = visStart - (visStart % minTick) + minTick;
			var firstMaj = visStart - (visStart % majTick) + majTick;

			// Number of minor ticks remaining before a major tick
			int minTickRem = (int)((firstMaj - firstMin) / minTick);

			float tickHeight;

			using (Pen p = new Pen(Color.Black))
			{
				p.Alignment = PenAlignment.Right;

				for (var x = firstMin; x < (firstMin + Width); x += minTick)
				{
					if (minTickRem == 0)
					{
						// Major Tick
						minTickRem = m_minorTicksPerMajor;
						p.Width = 2;
						tickHeight = Height * 0.5f;
					}
					else
					{
						// Minor Tick
						p.Width = 1;
						tickHeight = Height * 0.25f;
					}
					graphics.DrawLine(p, x, Height-tickHeight, x, Height);
					
					minTickRem--;
				}
			}

		}
		 */


		private void drawTicks(Graphics graphics, TimeSpan interval, int width, double height)
        {
            Single pxint = timeToPixels(interval);

            // calculate first tick - (it is the first multiple of interval greater than start)
            // believe it or not, this math is correct :-)
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart) % pxint) + pxint;

            for (Single x = start; x < start + Width; x += pxint)   
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

		public override TimeSpan TimePerPixel
		{
			get { return base.TimePerPixel; }
			set { base.TimePerPixel = value;	recalculate(); }
		}





		// Adapted from from Audacity, Ruler.cpp
		private void recalculate()
		{
			// Calculate the correct font size based on height
			int desiredPixelHeight = (this.Size.Height / 2) - 4;

			if (m_font != null)
				m_font.Dispose();
			m_font = new Font("Arial", desiredPixelHeight, GraphicsUnit.Pixel);


			if (m_brush != null)
				m_brush.Dispose();
			m_brush = new SolidBrush(Color.White);


			// As a heuristic, we want at least 16 pixels between each minor tick
			var t = pixelsToTime(16);

			if (t.TotalSeconds > 0.5)
			{
				if (t.TotalSeconds < 1)
				{
					m_MinorTick = TimeSpan.FromSeconds(1);
					m_minorTicksPerMajor = 5;
				}
				else if (t.TotalSeconds < 5)
				{
					m_MinorTick = TimeSpan.FromSeconds(5);
					m_minorTicksPerMajor = 3; //major = 15.0;
				}
				else if (t.TotalSeconds < 10)
				{
					m_MinorTick = TimeSpan.FromSeconds(10);
					m_minorTicksPerMajor = 3; //major = 30.0;
				}
				else if (t.TotalSeconds < 15)
				{
					m_MinorTick = TimeSpan.FromSeconds(15);
					m_minorTicksPerMajor = 4; //major = 60.0;
				}
				else if (t.TotalSeconds < 30)
				{
					m_MinorTick = TimeSpan.FromSeconds(30);
					m_minorTicksPerMajor = 2; //major = 60.0;
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
				m_digits = 6;
				for (; ; )
				{
					if (t.TotalSeconds < d)
					{
						m_MinorTick = TimeSpan.FromSeconds(d);
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						break;
					}
					d *= 5.0;
					if (t.TotalSeconds < d)
					{
						m_MinorTick = TimeSpan.FromSeconds(d);
						m_minorTicksPerMajor = 2; //major = d * 2.0;
						break;
					}
					d *= 2.0;
					m_digits--;
				}
			}

			//Debug.WriteLine("update():  t={0}    minor={1}   minPerMaj={2}", t, m_MinorTick, m_minorTicksPerMajor);
		}

		private string labelString(TimeSpan t)
		{
			// Adapted from from Audacity, Ruler.cpp

			double d = t.TotalSeconds;
			string timeFormat = string.Empty;

			if (m_MinorTick >= TimeSpan.FromHours(1))
			{
				// Round time to nearest hour
				t = TimeSpan.FromHours((int)t.TotalHours);
				timeFormat = @"hh\:mm\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromMinutes(1))
			{
				// Round time to nearest minute
				t = TimeSpan.FromMinutes((int)t.TotalMinutes);

				if (t > TimeSpan.FromHours(1))
					timeFormat = @"hh\:mm\:ss";
				else
					timeFormat = @"mm\:ss";
			}
			else if (m_MinorTick >= TimeSpan.FromSeconds(1))
			{
				// Round time to nearest second
				t = TimeSpan.FromSeconds((int)t.TotalSeconds);

				if (t > TimeSpan.FromHours(1))
					timeFormat = @"hh\:mm\:ss";
				else if (t > TimeSpan.FromMinutes(1))
					timeFormat = @"mm\:ss";
				else
					timeFormat = @"\:ss";
			}
			else	// m_MinorTickNew < 1 sec
			{
				if (t > TimeSpan.FromHours(1))
					timeFormat = @"hh\:mm\:ss\.";
				else if (t > TimeSpan.FromMinutes(1))
					timeFormat = @"mm\:ss\.";
				else
					timeFormat = @"\:ss\.";

				StringBuilder frac = new StringBuilder();
				for (int i=0; i < m_digits; i++)
					frac.Append('f');

				timeFormat += frac.ToString();
			}

			//Debug.WriteLine("MinorTick: {0}    timeFormat: {1}", m_MinorTick, timeFormat);
			return t.ToString(timeFormat);
		}


		private void drawTimes(Graphics graphics)
		{
			//Font f = new Font("Arial", 8);
			//SolidBrush b = new SolidBrush(Color.White);
			String timeFormat = @"mm\:ss\.fff";
			SizeF stringSize;
			int lastPixel = 0;

			// calculate the width of a single time, and figure out how regularly we will be able
			// to display times without overlapping. Then we can make sure we only use those intervals
			// to draw strings.
			// TODO: format strings differently based on the total time: at the moment, it's always
			// in the format of mm:ss.xxx.
			stringSize = graphics.MeasureString(TimeSpan.FromSeconds(0).ToString(timeFormat), m_font);
			int timeDisplayInterval = (int)((stringSize.Width + minPxBetweenTimeLabels) / timeToPixels(MajorTick)) + 1;
			TimeSpan drawnInterval = TimeSpan.FromTicks(MajorTick.Ticks * timeDisplayInterval);


			// get the time of the first tick that is: visible, on a major tick interval, and a multiple of the number of interval ticks
			TimeSpan firstMajor = TimeSpan.FromTicks(VisibleTimeStart.Ticks - (VisibleTimeStart.Ticks % drawnInterval.Ticks) + drawnInterval.Ticks);

			for (TimeSpan curTime = firstMajor; curTime <= firstMajor + VisibleTimeSpan; curTime += drawnInterval)
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



		#region Mouse Events



		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Cursor = Cursors.Hand;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Cursor = Cursors.Default;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			//base.OnMouseClick(e);
			TimeSpan t = pixelsToTime(e.X) + VisibleTimeStart;

			if (Click != null)
				Click(this, new TimeSpanEventArgs(t));
		}

		public new event EventHandler<TimeSpanEventArgs> Click;

		#endregion


	}
}
