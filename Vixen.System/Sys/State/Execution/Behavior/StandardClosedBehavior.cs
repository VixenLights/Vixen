using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys.State.Execution.Behavior {
	class StandardClosedBehavior {
		static public void Run() {
			Vixen.Sys.Execution.SystemTime.Stop();
		}
	}
}
