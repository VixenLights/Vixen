using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys
{
	public class LatchedEventArgs : EventArgs
	{
		public LatchedEventArgs(bool checkedState)
		{
			CheckedState = checkedState;
		}

		public bool CheckedState { get; private set; }
	}
}