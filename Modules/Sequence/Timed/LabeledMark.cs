using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class LabeledMark
	{
		public LabeledMark():this(TimeSpan.Zero)
		{
			
		}

		public LabeledMark(TimeSpan startTime)
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

		public int StackIndex { get; set; }
		public int StackCount { get; set; }

	}
}
