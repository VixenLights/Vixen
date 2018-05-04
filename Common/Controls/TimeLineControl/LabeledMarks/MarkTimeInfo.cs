using System;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarkTimeInfo
	{
		public MarkTimeInfo(Mark mark)
		{
			StartTime = mark.StartTime;
			EndTime = mark.EndTime;
			Duration = mark.Duration;
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		public TimeSpan Duration { get; set; }

	}
	
}
