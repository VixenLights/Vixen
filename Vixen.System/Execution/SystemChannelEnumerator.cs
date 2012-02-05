using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Module.Timing;
using Vixen.Commands;
using Vixen.Sys.Instrumentation;

namespace Vixen.Execution {
	class SystemChannelEnumerator : MultiStateTimedEnumerator<CommandNode, CommandNode[]> {
		private CommandsQualifiedPercentValue _commandsQualifiedPercentValue;
		private CommandsExpiredPercentValue _commandsExpiredPercentValue;
		private CommandsQualifiedCountValue _commandsQualifiedCountValue;
		private CommandsExpiredCountValue _commandsExpiredCountValue;
		
		public SystemChannelEnumerator(IEnumerable<CommandNode> data, ITiming timingSource)
			: base(data, timingSource, TimeSpan.Zero) {
			_SetupInstrumentation();
		}

		protected override void _ItemQualified(CommandNode value) {
			_commandsQualifiedPercentValue.IncrementQualifying();
			_commandsQualifiedCountValue.Add(1);
		}

		protected override void _ItemExpired(CommandNode value) {
			_commandsExpiredPercentValue.IncrementUnqualifying();
			_commandsExpiredCountValue.Add(1);
		}

		private void _SetupInstrumentation() {
			_commandsQualifiedPercentValue = new CommandsQualifiedPercentValue();
			_commandsExpiredPercentValue = new CommandsExpiredPercentValue();
			_commandsQualifiedCountValue = new CommandsQualifiedCountValue();
			_commandsExpiredCountValue = new CommandsExpiredCountValue();

			VixenSystem.Instrumentation.AddValue(_commandsQualifiedPercentValue);
			VixenSystem.Instrumentation.AddValue(_commandsExpiredPercentValue);
			VixenSystem.Instrumentation.AddValue(_commandsQualifiedCountValue);
			VixenSystem.Instrumentation.AddValue(_commandsExpiredCountValue);
		}
	}
}
