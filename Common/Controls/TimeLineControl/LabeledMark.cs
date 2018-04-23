using System;
using System.Runtime.Serialization;

namespace Common.Controls.TimelineControl
{
	[DataContract]
	public class LabeledMark
	{
		public LabeledMark()
		{
			
		}

		

		public int StackIndex { get; set; }
		public int StackCount { get; set; }

	}
}
