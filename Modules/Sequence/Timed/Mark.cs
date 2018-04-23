using System;
using System.Runtime.Serialization;

namespace VixenModules.Sequence.Timed
{
	public class Mark
	{
		public Mark() : this(TimeSpan.Zero)
		{

		}

		public Mark(TimeSpan startTime)
		{
			StartTime = startTime;
			Duration = TimeSpan.FromMilliseconds(250);
		}

		public string Text { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }

		public TimeSpan EndTime => StartTime + Duration;
	}
}
