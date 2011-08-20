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
        }

		/// <summary>
		/// The amount of time currently visible. Adjusting this implements zoom along the X (time) axis.
		/// </summary>
		private TimeSpan m_visibleTimeSpan;
		public virtual TimeSpan VisibleTimeSpan
		{
			get { return m_visibleTimeSpan; }
			set { m_visibleTimeSpan = value; Refresh(); }
		}

		private TimeSpan m_visibleTimeStart;
		public virtual TimeSpan VisibleTimeStart
		{
			get { return m_visibleTimeStart; }
			set { m_visibleTimeStart = value; Refresh(); }
		}

		/// <summary>
        /// Gets the amount of time represented by one horizontal pixel.
        /// </summary>
        public TimeSpan TimePerPixel
        {
            get { return TimeSpan.FromTicks(VisibleTimeSpan.Ticks / Width); }
        }

		protected Single timeToPixels(TimeSpan t)
		{
			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		protected TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }
    }
}
