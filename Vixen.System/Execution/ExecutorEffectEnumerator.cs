using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;
using Vixen.Execution;

namespace Vixen.Execution {
	class ExecutorEffectEnumerator : TimedChannelEnumerator<EffectNode> {
		public ExecutorEffectEnumerator(IEnumerable<EffectNode> data, ITiming timingSource, TimeSpan sequenceStartTime, TimeSpan sequenceEndTime)
			: base(data, timingSource, sequenceStartTime, sequenceEndTime, SingleTimedEnumeratorProgressType.LeadingEdge, true) {
		}
	}
}
