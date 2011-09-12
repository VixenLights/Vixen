using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class ExecutorEffectEnumerator : TimedChannelEnumerator<CommandNode> {
		public ExecutorEffectEnumerator(IEnumerable<CommandNode> data, ITiming timingSource, TimeSpan sequenceStartTime, TimeSpan sequenceEndTime)
			: base(data, timingSource, sequenceStartTime, sequenceEndTime) {
		}
	}
}
