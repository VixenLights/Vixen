using System;

namespace Common.Controls.Timeline
{
	/// <summary>
	/// Provides time information to consumers and events to notify when they have been changed.
	/// </summary>
	public class TimeInfo
	{
		public TimeInfo() {
			 TimePerPixel = TimeSpan.FromTicks(100000);
			 VisibleTimeStart = TimeSpan.Zero;
		}
		private TimeSpan m_timePerPixel;
		private TimeSpan m_visibleTimeStart;
		private TimeSpan m_totalTime;

		private TimeSpan? m_playbackStart = null;
		private TimeSpan? m_playbackEnd = null;
		private TimeSpan? m_playbackCur = null;


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

		public TimeSpan TotalTime
		{
			get { return m_totalTime; }
			set
			{
				if (m_totalTime == value)
					return;

				m_totalTime = value;
				if (TotalTimeChanged != null)
					TotalTimeChanged(this, EventArgs.Empty);
			}
		}


		public TimeSpan? PlaybackStartTime
		{
			get { return m_playbackStart; }
			set
			{
				if (m_playbackStart == value)
					return;
				m_playbackStart = value;
				if (PlaybackStartTimeChanged != null)
					PlaybackStartTimeChanged(this, EventArgs.Empty);
			}
		}

		public TimeSpan? PlaybackEndTime
		{
			get { return m_playbackEnd; }
			set
			{
				if (m_playbackEnd == value)
					return;
				m_playbackEnd = value;
				if (PlaybackEndTimeChanged != null)
					PlaybackEndTimeChanged(this, EventArgs.Empty);
			}
		}

		public TimeSpan? PlaybackCurrentTime
		{
			get { return m_playbackCur; }
			set
			{
				if (m_playbackCur == value)
					return;
				m_playbackCur = value;
				if (PlaybackCurrentTimeChanged != null)
					PlaybackCurrentTimeChanged(this, EventArgs.Empty);
			}
		}


		public event EventHandler TimePerPixelChanged;
		public event EventHandler VisibleTimeStartChanged;
		public event EventHandler TotalTimeChanged;

		public event EventHandler PlaybackStartTimeChanged;
		public event EventHandler PlaybackEndTimeChanged;
		public event EventHandler PlaybackCurrentTimeChanged;
	}
}