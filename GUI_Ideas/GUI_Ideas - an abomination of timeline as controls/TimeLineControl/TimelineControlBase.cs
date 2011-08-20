using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
    public class TimelineControlBase : UserControl
    {
        internal TimelineControlBase()
        {
            DoubleBuffered = true;
        }

		/// <summary>
		/// The amount of time currently visible.
		/// </summary>
		public virtual TimeSpan VisibleTimeSpan
		{
			get;
			set;
		}

		/// <summary>
        /// Gets the amount of time represented by one horizontal pixel.
        /// </summary>
        public TimeSpan TimePerPixel
        {
            get { return TimeSpan.FromTicks(VisibleTimeSpan.Ticks / Width); }
        }

		public Single timeToPixels(TimeSpan t)
		{
			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		public TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }
    }
}
