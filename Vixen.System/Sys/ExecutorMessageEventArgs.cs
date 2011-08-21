using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class ExecutorMessageEventArgs : EventArgs {
		public ExecutorMessageEventArgs(string value) {
			Message = value;
		}

		public string Message { get; private set; }
	}
}
