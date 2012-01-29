using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class ExecutorEffectEnumerator : SingleTimedEnumerator<EffectNode> {
		public ExecutorEffectEnumerator(IEnumerable<EffectNode> data, ITiming timingSource, TimeSpan sequenceStartTime)
			: base(data, timingSource, sequenceStartTime, SingleTimedEnumeratorProgressType.LeadingEdge, true) {
		}
	}
}
