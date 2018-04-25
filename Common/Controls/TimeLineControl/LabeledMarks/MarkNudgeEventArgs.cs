using System;
using System.Collections.Generic;
using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarkNudgeEventArgs : EventArgs
	{
		public MarkNudgeEventArgs(SortedDictionary<TimeSpan, SnapDetails> selectedMarks, TimeSpan offset)
		{
			SelectedMarks = selectedMarks; //This is effectivlly the previous Mark time as moving the generate many adds and removes so the MArk can be displayed during the move.
			Offset = offset;
		}

		public SortedDictionary<TimeSpan, SnapDetails> SelectedMarks { get; set; }
		public TimeSpan Offset { get; private set; }
	}
}