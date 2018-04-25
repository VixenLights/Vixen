using System;
using System.Collections.Generic;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMovedEventArgs : EventArgs
	{
		public MarksMovedEventArgs(TimeSpan offsetTime, List<Mark> marks)
		{
			OffsetTime = offsetTime; 
			Marks = marks;
		}

		public TimeSpan OffsetTime { get; private set; }
		public List<Mark> Marks { get; private set; }
		
	}
}