using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class CommandsExpiredPercentValue : PercentValue {
		public CommandsExpiredPercentValue()
			: base("Commands - Expired (%)") {
		}
	}
}
