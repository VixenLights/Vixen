using System;
using System.Collections.Generic;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksDeletedEventArgs : EventArgs
	{
		public MarksDeletedEventArgs(List<IMark> marks)
		{
			Marks = marks;
		}

		public List<IMark> Marks { get; }
	}
}