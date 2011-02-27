using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
	public class ExecutorMessageEventArgs : EventArgs {
		public ExecutorMessageEventArgs(string value) {
			Message = value;
		}

		public string Message { get; private set; }
	}
}
