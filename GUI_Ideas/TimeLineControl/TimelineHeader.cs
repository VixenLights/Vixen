using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Timeline
{
	public partial class TimelineHeader : TimelineControlBase
	{
		public TimelineHeader()
		{
            MajorTickInterval = TimeSpan.FromSeconds(1.0);
            MinorTicksPerMajor = 4;
		}

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

		private const int minPxBetweenTimeLabels = 10;






        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, Size.Width, Size.Height);

            // Translate the graphics to work the same way the timeline grid does
            // (ie. Drawing coordinates take into account where we start at in time)
            e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

            drawTicks(e.Graphics, MajorTickInterval, 2, 0.5);
            drawTicks(e.Graphics, MinorTickInterval, 1, 0.25);
			drawTimes(e.Graphics);
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
	}
}
