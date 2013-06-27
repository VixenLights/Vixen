using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys.State.Execution.Behavior
{
	internal class StandardOpenBehavior
	{
		public static void Run()
		{
			Vixen.Sys.Execution.SystemTime.Start();
		}
	}
}