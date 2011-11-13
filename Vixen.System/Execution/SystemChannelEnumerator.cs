using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;
using Vixen.Commands;
using Vixen.Sys.Instrumentation;

namespace Vixen.Execution {
	class SystemChannelEnumerator : MultiStateTimedEnumerator<CommandNode, CommandNode[]> {
		private CommandsQualifiedValue _commandsQualifiedValue;
		private CommandsExpiredValue _commandsExpiredValue;
		
		public SystemChannelEnumerator(IEnumerable<CommandNode> data, ITiming timingSource)
			: base(data, timingSource, TimeSpan.Zero, TimeSpan.MaxValue) {
			_commandsQualifiedValue = new CommandsQualifiedValue();
			_commandsExpiredValue = new CommandsExpiredValue();
			VixenSystem.Instrumentation.AddValue(_commandsQualifiedValue);
			VixenSystem.Instrumentation.AddValue(_commandsExpiredValue);
		}

		protected override void _ItemQualified(CommandNode value) {
			_commandsQualifiedValue.IncrementQualifying();
		}

		protected override void _ItemExpired(CommandNode value) {
			_commandsExpiredValue.IncrementUnqualifying();
		}
	}
}
