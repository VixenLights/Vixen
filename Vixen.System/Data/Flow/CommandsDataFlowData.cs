using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Flow {
	public class CommandsDataFlowData : Dispatchable<CommandsDataFlowData>, IDataFlowData<IEnumerable<ICommand>> {
		public CommandsDataFlowData(IEnumerable<ICommand> commands) {
			Value = commands;
		}

		public IEnumerable<ICommand> Value { get; private set; }

		object IDataFlowData.Value {
			get { return Value; }
		}
	}
}
