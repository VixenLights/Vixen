using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class CommandsExpiredValue : PercentValue {
		public CommandsExpiredValue()
			: base("Commands - Expired") {
		}
	}
}
