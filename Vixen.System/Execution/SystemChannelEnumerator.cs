using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class SystemChannelEnumerator : TimedChannelEnumerator<CommandData> {
		public SystemChannelEnumerator(IEnumerable<CommandData> data, ITiming timingSource)
			: base(data, timingSource, 0, long.MaxValue) {
		}
	}
}
