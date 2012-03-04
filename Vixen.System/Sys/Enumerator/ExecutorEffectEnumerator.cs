using System;
using System.Collections.Generic;
using Vixen.Module.Timing;

namespace Vixen.Sys.Enumerator {
	class ExecutorEffectEnumerator : SingleTimedEnumerator<EffectNode> {
		public ExecutorEffectEnumerator(IEnumerable<EffectNode> data, ITiming timingSource, TimeSpan sequenceStartTime)
			: base(data, timingSource, sequenceStartTime, SingleTimedEnumeratorProgressType.LeadingEdge, true) {
		}
	}
}
