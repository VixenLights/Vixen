using System;
using System.Collections.Generic;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMovingEventArgs : EventArgs
	{
		public MarksMovingEventArgs(List<IMark> marks)
		{
			Marks = marks;
		}

		public List<IMark> Marks { get; private set; }
	}
}