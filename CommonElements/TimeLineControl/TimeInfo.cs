using System;

namespace CommonElements.Timeline
{
	/// <summary>
	/// Provides time information to consumers and events to notify when they have been changed.
	/// </summary>
	public class TimeInfo
	{
		private TimeSpan m_timePerPixel;
		private TimeSpan m_visibleTimeStart;


		public TimeSpan TimePerPixel
		{
			get { return m_timePerPixel; }
			set
			{
				if (m_timePerPixel == value)
					return;

				m_timePerPixel = value;
				if (TimePerPixelChanged != null)
					TimePerPixelChanged(this, EventArgs.Empty);
			}
		}

		public TimeSpan VisibleTimeStart
		{
			get { return m_visibleTimeStart; }
			set
			{
				if (m_visibleTimeStart == value)
					return;

				m_visibleTimeStart = value;
				if (VisibleTimeStartChanged != null)
					VisibleTimeStartChanged(this, EventArgs.Empty);
			}
		}


		public event EventHandler TimePerPixelChanged;
		public event EventHandler VisibleTimeStartChanged;
	}
}