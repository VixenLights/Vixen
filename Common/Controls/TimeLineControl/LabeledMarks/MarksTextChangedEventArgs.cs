using System;
using System.Collections.Generic;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksTextChangedEventArgs : EventArgs
	{
		public MarksTextChangedEventArgs(IEnumerable<MarksTextInfo> changedMarks)
		{
			ChangedMarks = changedMarks;
		}

		public IEnumerable<MarksTextInfo> ChangedMarks { get;}

		
	}

	public class MarksTextInfo
	{
		public MarksTextInfo(IMark mark, string newText, string oldText)
		{
			NewText = newText;
			OldText = oldText;
			Mark = mark;
		}

		public IMark Mark { get;}

		public string NewText { get; }

		public string OldText { get; }
	}
}