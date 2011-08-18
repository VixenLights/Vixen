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
			//this.DoubleBuffered = true;
			DisplaySegments = 5;

            MajorTickInterval = TimeSpan.FromSeconds(1.0);
            MinorTicksPerMajor = 3;

           //this.VisibleTimeSpan = TimeSpan.FromSeconds(10);
		}

		// the amount of time that should be displayed in the header.
        //public TimeSpan DisplayedTimeSpan { get; set; }

		// the start time that should be displayed in the header.
		//public TimeSpan DisplayedTimeStart { get; set; }

		// the number of segments to divide each interval into (each 10 seconds, second, 1/10 second, etc.)
		public int DisplaySegments { get; set; }


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
            get { return TimeSpan.FromTicks(MajorTickInterval.Ticks / (MinorTicksPerMajor + 1)); }
        }






        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, Size.Width, Size.Height);

            // HACK - Compensate for the border in the TimelineGrid
            e.Graphics.TranslateTransform(3, 0);

            // Translate the graphics to work the same way the timeline grid does
            // (ie. Drawing coordinates take into account where we start at in time)
            e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

            drawTicks(e.Graphics, MajorTickInterval, 2, 0.5);
            drawTicks(e.Graphics, MinorTickInterval, 1, 0.25);

        }

        private void drawTicks(Graphics graphics, TimeSpan interval, int width, double height)
        {
            int pxint = timeToPixels(interval);

            // calculate first tick - (it is the first multiple of interval greater than start)
            // believe it or not, this math is correct :-)
            int start = timeToPixels(VisibleTimeStart) / pxint * pxint + pxint;

            for (int x = start; x < start + Width; x += pxint)   
            {
                Pen p = new Pen(Color.Black);
                p.Width = width;
                p.Alignment = PenAlignment.Right;
                graphics.DrawLine(p, x, (int)(Height * (1.0-height)), x, Height);

            }
        }


        private void drawTimes(Graphics graphics)
        {

        }



        /*
		protected override void _OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(Color.Purple), 0, 0, Size.Width, Size.Height);

			if (VisibleTimeSpan.TotalSeconds <= 0)
				return;

			int minPixelsPerInterval = 60;
			int maxPixelsPerInterval = 300;

			double secondsPerInterval = 1;
			double pixelsPerInterval = (double)Width * secondsPerInterval / VisibleTimeSpan.TotalSeconds;

			while (pixelsPerInterval < minPixelsPerInterval || pixelsPerInterval > maxPixelsPerInterval) {
				if (pixelsPerInterval < minPixelsPerInterval)
					secondsPerInterval *= 5;
				else if (pixelsPerInterval > maxPixelsPerInterval)
					secondsPerInterval /= 5;

				pixelsPerInterval = (double)Width * secondsPerInterval / VisibleTimeSpan.TotalSeconds;
			}

			double secondsPerSegment = secondsPerInterval / DisplaySegments;
			double pixelsPerSegment = pixelsPerInterval / DisplaySegments;

			double segmentOffset = secondsPerSegment - (DisplayedTimeStart.TotalSeconds % secondsPerSegment);
			double intervalOffset = secondsPerInterval - (DisplayedTimeStart.TotalSeconds % secondsPerInterval);

			int segmentCounter = DisplaySegments - (int)((intervalOffset - segmentOffset) / (secondsPerInterval / DisplaySegments));

			Pen pen = new Pen(Color.Black);
			for (double px = segmentOffset * pixelsPerSegment; px < Width; px += pixelsPerSegment) {
				segmentCounter %= DisplaySegments;
				if (segmentCounter == 0) {
					pen.Width = 2;
					e.Graphics.DrawLine(pen, (int)px, (int)(Height * 0.5), (int)px, Height);
				} else {
					pen.Width = 1;
					e.Graphics.DrawLine(pen, (int)px, (int)(Height * 0.75), (int)px, Height);
				}

				segmentCounter++;
			}


		}
         */


	}
}
