using System.Collections.Generic;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class PhonemeBreakdownEventArgs
	{
		public PhonemeBreakdownEventArgs(List<IMark> marks, BreakdownType type)
		{
			Marks = marks;
			BreakdownType = type;
		}

		public List<IMark> Marks { get; }

		public BreakdownType BreakdownType { get;}
	}

	public enum BreakdownType
	{
		Phrase,
		Word
	}
}
