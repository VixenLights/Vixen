using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class SystemChannelEnumerator : MultiStateEnumerator<Command, Command[]> {
		public SystemChannelEnumerator(IEnumerable<Command> data, ITiming timingSource)
			: base(data, timingSource, TimeSpan.Zero, TimeSpan.MaxValue) {
		}
	}
}
