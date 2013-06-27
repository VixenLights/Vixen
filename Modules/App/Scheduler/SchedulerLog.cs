using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler
{
	internal class SchedulerLog : Vixen.Sys.Log
	{
		public SchedulerLog()
			: base("Schedule")
		{
		}
	}
}