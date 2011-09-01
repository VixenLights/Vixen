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
			updateValues();
            //MajorTickInterval = TimeSpan.FromSeconds(1.0);
            //MinorTicksPerMajor = 4;
		}

		/*
        private TimeSpan m_majorTick;
        public TimeSpan MajorTickInterval
        {
            get { return m_majorTick; }
            set
            {
                m_majorTick = value;
                Invalidate();
            }
        }

        public int MinorTicksPerMajor { get; set; }

        private TimeSpan MinorTickInterval
        {
            get { return TimeSpan.FromTicks(MajorTickInterval.Ticks / MinorTicksPerMajor); }
        }
		*/

		private const int minPxBetweenTimeLabels = 10;


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, Size.Width, Size.Height);

            // Translate the graphics to work the same way the timeline grid does
            // (ie. Drawing coordinates take into account where we start at in time)
            e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

			// TODO: do something intelligent with the scales for these. Try scaling based on the zoom again maybe?
            drawTicks(e.Graphics, MajorTick, 2, 0.5);
            drawTicks(e.Graphics, MinorTick, 1, 0.25);
			//drawTimes(e.Graphics);
        }

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



		private Font m_font = null;

		private int m_digits;
		private TimeSpan  m_MinorTick;	//m_MajorTick
		private int m_minorTicksPerMajor;

		public TimeSpan MinorTick { get { return m_MinorTick; } }
		public TimeSpan MajorTick { get { return m_MinorTick.Scale(m_minorTicksPerMajor); } }

		protected override void OnResize(EventArgs e)
		{
			updateValues();
			base.OnResize(e);
		}

		private void updateValues()
		{
			// Calculate the correct font size based on height
			int desiredPixelHeight = (this.Size.Height / 2) - 4;

			if (m_font != null)
				m_font.Dispose();
			m_font = new Font("Arial", desiredPixelHeight, GraphicsUnit.Pixel);

			calculateTickSizes();
		}

		private void calculateTickSizes()
		{
			// Adapted from from Audacity, Ruler.cpp

			// As a heuristic, we want at least 16 pixels between each minor tick
			//var minSec = pixelsToTime(16).TotalSeconds;
			
			var t = pixelsToTime(16);
			double minor; //, major;

			if (t.TotalSeconds > 0.5)
			{
				if (t.TotalSeconds < 1.0)
				{
					minor = 1.0;
					m_minorTicksPerMajor = 5;	//major = 5.0;
				}
				else if (t.TotalSeconds < 5.0)
				{
					minor = 5.0;
					m_minorTicksPerMajor = 3; //major = 15.0;
				}
				else if (t.TotalSeconds < 10.0)
				{
					minor = 10.0;
					m_minorTicksPerMajor = 3; //major = 30.0;
				}
				else if (t.TotalSeconds < 15.0)
				{
					minor = 15.0;
					m_minorTicksPerMajor = 4; //major = 60.0;
				}
				else if (t.TotalSeconds < 30.0)
				{
					minor = 30.0;
					m_minorTicksPerMajor = 2; //major = 60.0;
				}

				//else if (minSec < 60.0)
				else if (t.TotalMinutes < 1.0)
				{ // 1 min
					minor = 60.0;
					m_minorTicksPerMajor = 5;	//major = 300.0;
				}
				//else if (minSec < 300.0)
				else if (t.TotalMinutes < 5.0)
				{ // 5 min
					minor = 300.0;
					m_minorTicksPerMajor = 3;	//major = 900.0;
				}
				//else if (minSec < 600.0)
				else if (t.TotalMinutes < 10.0)
				{ // 10 min
					minor = 600.0;
					m_minorTicksPerMajor = 3;	//major = 1800.0;
				}
				//else if (minSec < 900.0)
				else if (t.TotalMinutes < 15.0)
				{ // 15 min
					minor = 900.0;
					m_minorTicksPerMajor = 4;	//major = 3600.0;
				}
				//else if (minSec < 1800.0)
				else if (t.TotalMinutes < 30.0)
				{ // 30 min
					minor = 1800.0;
					m_minorTicksPerMajor = 2;	//major = 3600.0;
				}
				
				//else if (minSec < 3600.0)
				else if (t.TotalHours < 1.0)
				{ // 1 hr
					minor = 3600.0;
					m_minorTicksPerMajor = 6;	//major = 6 * 3600.0;
				}
				//else if (minSec < 6 * 3600.0)
				else if (t.TotalHours < 6.0)
				{ // 6 hrs
					minor = 6 * 3600.0;
					m_minorTicksPerMajor = 4;	//major = 24 * 3600.0;
				}

				//else if (minSec < 24 * 3600.0)
				else if (t.TotalDays < 1.0)
				{ // 1 day
					minor = 24 * 3600.0;
					m_minorTicksPerMajor = 7;	//major = 7 * 24 * 3600.0;
				}

				else
				{
					minor = 24.0 * 7.0 * 3600.0; // 1 week
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
						minor = d;
						m_minorTicksPerMajor = 5; //major = d * 5.0;
						goto done;
					}
					d *= 5.0;
					if (t.TotalSeconds < d)
					{
						minor = d;
						m_minorTicksPerMajor = 2; //major = d * 2.0;
						goto done;
					}
					d *= 2.0;
					m_digits--;
				}
			}

			done:
			//m_MajorTick = TimeSpan.FromSeconds(major);
			m_MinorTick = TimeSpan.FromSeconds(minor);
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

				StringBuilder frac = new StringBuilder();
				for (int i=0; i < m_digits; i++)
					frac.Append('f');

				timeFormat += frac.ToString();
			}

			Debug.WriteLine("MinorTick: {0}    timeFormat: {1}", m_MinorTick, timeFormat);
			return t.ToString(timeFormat);
		}


		/*
        private void drawTimes(Graphics graphics)
        {
			Font f = new Font("Arial", 8);
			SolidBrush b = new SolidBrush(Color.White);
			String timeFormat = @"mm\:ss\.fff";
			SizeF stringSize;
			int lastPixel = 0;

			// calculate the width of a single time, and figure out how regularly we will be able
			// to display times without overlapping. Then we can make sure we only use those intervals
			// to draw strings.
			// TODO: format strings differently based on the total time: at the moment, it's always
			// in the format of mm:ss.xxx.
			stringSize = graphics.MeasureString(TimeSpan.FromSeconds(0).ToString(timeFormat), f);
			int timeDisplayInterval = (int)((stringSize.Width + minPxBetweenTimeLabels) / timeToPixels(MajorTickInterval)) + 1;
			TimeSpan drawnInterval = TimeSpan.FromTicks(MajorTickInterval.Ticks * timeDisplayInterval);


			// get the time of the first tick that is: visible, on a major tick interval, and a multiple of the number of interval ticks
			TimeSpan firstMajor = TimeSpan.FromTicks(VisibleTimeStart.Ticks - (VisibleTimeStart.Ticks % drawnInterval.Ticks) + drawnInterval.Ticks);

			for (TimeSpan curTime = firstMajor; curTime <= firstMajor + VisibleTimeSpan; curTime += drawnInterval) {
				stringSize = graphics.MeasureString(curTime.ToString(timeFormat), f);
				Single posOffset = (stringSize.Width / 2);
				Single curPixelCentre = timeToPixels(curTime);

				// if drawing the string wouldn't overlap the last, then draw it
				if (lastPixel + minPxBetweenTimeLabels + posOffset < curPixelCentre) {
					graphics.DrawString(curTime.ToString(timeFormat), f, b, curPixelCentre - posOffset, (Height / 4) - (stringSize.Height / 2));
					lastPixel = (int)(curPixelCentre + posOffset);
				}
			}
		}
		 */
	}
}
