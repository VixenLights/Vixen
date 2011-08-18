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
            DoubleBuffered = true;
            VisibleTimeSpan = TimeSpan.FromSeconds(10.0);
			Rows = new TimelineRowCollection();
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


		protected Single timeToPixels(TimeSpan t)
		{
			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		protected TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }

        public virtual TimeSpan VisibleTimeStart { get; set; }

		// Rows are now obtained from the main TimelineControl, which should set up the
		// reference when it initializes itself and the grid.
		public TimelineRowCollection Rows { get; set; }
    }
}
