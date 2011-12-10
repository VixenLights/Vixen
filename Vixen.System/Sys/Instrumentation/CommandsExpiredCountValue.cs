using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class CommandsExpiredCountValue : CountValue {
		public CommandsExpiredCountValue()
			: base("Commands - Expired (count)") {
		}
	}
}
