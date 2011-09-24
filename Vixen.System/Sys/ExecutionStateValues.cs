using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class ExecutionStateValues : Dictionary<Channel, Command> {
		public ExecutionStateValues() {
		}

		public ExecutionStateValues(TimeSpan time) {
			Time = time;
		}

		public TimeSpan Time { get; private set; }
	}
}
