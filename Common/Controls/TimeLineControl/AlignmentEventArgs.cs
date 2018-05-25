using System;
using System.Collections.Generic;

namespace Common.Controls.TimelineControl
{
	public class AlignmentEventArgs
	{
		public AlignmentEventArgs(bool active, IEnumerable<TimeSpan> times)
		{
			Active = active;
			Times = times;
		}

		public bool Active { get; }

		public IEnumerable<TimeSpan> Times { get; }

	}

}
