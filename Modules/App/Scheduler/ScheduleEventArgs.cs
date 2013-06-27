using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler
{
	internal class ScheduleEventArgs : EventArgs
	{
		public ScheduleEventArgs(int dayOffset, TimeSpan timeOffset)
		{
			DayOffset = dayOffset;
			TimeOffset = timeOffset;
		}

		public int DayOffset { get; private set; }
		public TimeSpan TimeOffset { get; private set; }
	}
}