using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
	public partial class TimelineHeader : UserControl
	{
		public TimelineHeader()
		{
			this.DoubleBuffered = true;
			DisplaySegments = 5;
		}

		// the amount of time that should be displayed in the header.
		public TimeSpan DisplayedTimeSpan { get; set; }

		// the start time that should be displayed in the header.
		public TimeSpan DisplayedTimeStart { get; set; }

		// the number of segments to divide each interval into (each 10 seconds, second, 1/10 second, etc.)
		public int DisplaySegments { get; set; }


		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(Color.Purple), 0, 0, Size.Width, Size.Height);

			if (DisplayedTimeSpan.TotalSeconds <= 0)
				return;

			int minPixelsPerInterval = 60;
			int maxPixelsPerInterval = 300;

			double secondsPerInterval = 1;
			double pixelsPerInterval = (double)Width * secondsPerInterval / DisplayedTimeSpan.TotalSeconds;

			while (pixelsPerInterval < minPixelsPerInterval || pixelsPerInterval > maxPixelsPerInterval) {
				if (pixelsPerInterval < minPixelsPerInterval)
					secondsPerInterval *= 5;
				else if (pixelsPerInterval > maxPixelsPerInterval)
					secondsPerInterval /= 5;

				pixelsPerInterval = (double)Width * secondsPerInterval / DisplayedTimeSpan.TotalSeconds;
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
	}
}
