using System;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class PlayRangeEventArgs : EventArgs
	{
		public PlayRangeEventArgs(TimeSpan startTime, TimeSpan endTime)
		{
			StartTime = startTime;
			EndTime = endTime;
		}

		public TimeSpan StartTime { get; }

		public TimeSpan EndTime { get; }
	}
}
