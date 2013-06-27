using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Trigger
{
	public class TriggerSetEventArgs : EventArgs
	{
		public TriggerSetEventArgs(ITriggerInput trigger)
		{
			Trigger = trigger;
		}

		public ITriggerInput Trigger { get; private set; }
	}
}