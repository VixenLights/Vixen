using System;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarkTimeInfo
	{
		public MarkTimeInfo(IMark mark)
		{
			StartTime = mark.StartTime;
			EndTime = mark.EndTime;
			Duration = mark.Duration;
			MarkCollection = mark.Parent;
		}

		public TimeSpan StartTime { get; private set; }

		public TimeSpan EndTime { get; }

		public TimeSpan Duration { get; private set; }

		public IMarkCollection MarkCollection { get; }

		public static void SwapPlaces(IMark lhs, MarkTimeInfo rhs)
		{
			TimeSpan temp = lhs.StartTime;
			lhs.StartTime = rhs.StartTime;
			rhs.StartTime = temp;

			temp = lhs.Duration;
			lhs.Duration = rhs.Duration;
			rhs.Duration = temp;

		}
	}
	
}
