using System;
using System.Collections.Generic;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMovingEventArgs : EventArgs
	{
		public MarksMovingEventArgs(List<Mark> marks)
		{
			Marks = marks;
		}

		public List<Mark> Marks { get; private set; }
	}
}