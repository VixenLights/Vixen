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






        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, Size.Width, Size.Height);

            // Translate the graphics to work the same way the timeline grid does
            // (ie. Drawing coordinates take into account where we start at in time)
            e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

            drawTicks(e.Graphics, MajorTickInterval, 2, 0.5);
            drawTicks(e.Graphics, MinorTickInterval, 1, 0.25);

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

        }


	}
}
