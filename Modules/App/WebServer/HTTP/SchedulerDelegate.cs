using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Kayak;

namespace VixenModules.App.WebServer.HTTP
{
	class SchedulerDelegate : ISchedulerDelegate
	{
		public void OnException(IScheduler scheduler, Exception e)
		{
			Debug.WriteLine("Error on scheduler.");
			e.DebugStackTrace();
		}

		public void OnStop(IScheduler scheduler)
		{

		}
	}
}
