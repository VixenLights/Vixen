using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class EffectsPerSecondValue : RateValue {
		public EffectsPerSecondValue()
			: base("Effects Written Per Second") {
		}
	}
}
