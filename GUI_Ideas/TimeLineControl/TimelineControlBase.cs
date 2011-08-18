using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
    public abstract class TimelineControlBase : UserControl
    {

        internal TimelineControlBase()
        {
            this.DoubleBuffered = true;
            this.VisibleTimeSpan = TimeSpan.FromSeconds(10.0);
        }

        /// <summary>
        /// Gets the amount of time represented by one horizontal pixel.
        /// </summary>
        public TimeSpan TimePerPixel
        {
            get { return TimeSpan.FromTicks(VisibleTimeSpan.Ticks / Width); }
        }


        /// <summary>
        /// The amount of time currently visible. Adjusting this implements zoom along the X (time) axis.
        /// </summary>
        public TimeSpan VisibleTimeSpan { get; set; }


		protected int timeToPixels(TimeSpan t)
		{
			return (int)(t.Ticks / this.TimePerPixel.Ticks);
		}

		protected Single timeToPixelsF(TimeSpan t)
		{
			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		protected TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }

        public virtual TimeSpan VisibleTimeStart { get; set; }
    }
}
