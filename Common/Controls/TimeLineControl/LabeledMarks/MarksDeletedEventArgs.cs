using System;
using System.Collections.Generic;
using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksDeletedEventArgs : EventArgs
	{
		public MarksDeletedEventArgs(SortedDictionary<TimeSpan, SnapDetails>.KeyCollection marks)
		{
			Marks = marks;
		}

		public SortedDictionary<TimeSpan, SnapDetails>.KeyCollection Marks { get; private set; }
	}
}