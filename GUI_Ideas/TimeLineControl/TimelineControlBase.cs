using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
    public abstract class TimelineControlBase : UserControl
    {
		protected TimeSpan m_timePerPixel;
		private TimeSpan m_visibleTimeStart;

        internal TimelineControlBase()
        {
            DoubleBuffered = true;

			TimePerPixel = TimeSpan.FromTicks(100000);
			VisibleTimeStart = TimeSpan.FromSeconds(0);
        }

		#region Virtual Properties
		// These can be overridden in derived classes (namely TimelineControl)

		public virtual TimeSpan VisibleTimeStart
		{
			get { return m_visibleTimeStart; }
			set { m_visibleTimeStart = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the amount of time represented by one horizontal pixel.
		/// </summary>
		public virtual TimeSpan TimePerPixel
		{
			get { return m_timePerPixel; }
			set { m_timePerPixel = value; Invalidate(); }
		}

		#endregion


		/// <summary>
		/// The amount of time currently visible.
		/// </summary> 
		public TimeSpan VisibleTimeSpan
		{
			get { return TimeSpan.FromTicks(Width * TimePerPixel.Ticks); }
		}
		
		public TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
			set { VisibleTimeStart = value - VisibleTimeSpan; }
		}




		protected Single timeToPixels(TimeSpan t)
		{
			if (TimePerPixel.Ticks == 0)
				throw new DivideByZeroException("Time per pixel is zero!");

			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		protected TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }
		
    }
}
