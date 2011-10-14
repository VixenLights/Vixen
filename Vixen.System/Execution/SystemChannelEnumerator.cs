using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;
using Vixen.Commands;

namespace Vixen.Execution {
	class SystemChannelEnumerator : MultiStateTimedEnumerator<CommandNode, CommandNode[]> {
		public SystemChannelEnumerator(IEnumerable<CommandNode> data, ITiming timingSource)
			: base(data, timingSource, TimeSpan.Zero, TimeSpan.MaxValue) {
		}
	}
}
