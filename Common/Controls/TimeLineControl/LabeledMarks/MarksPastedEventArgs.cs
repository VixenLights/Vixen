using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksPastedEventArgs:EventArgs
	{
		public MarksPastedEventArgs(List<IMark> marks)
		{
			Marks = marks;
		}

		public List<IMark> Marks { get; }
	}
}
