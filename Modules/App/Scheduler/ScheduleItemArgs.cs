using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler
{
	internal class ScheduleItemArgs : EventArgs
	{
		public ScheduleItemArgs(IScheduleItem item)
		{
			Item = item;
		}

		public IScheduleItem Item { get; private set; }
	}
}