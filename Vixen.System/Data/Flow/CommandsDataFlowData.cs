using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Flow
{
	public class CommandsDataFlowData : Dispatchable<CommandsDataFlowData>, IDataFlowData<List<ICommand>>
	{
		public CommandsDataFlowData(List<ICommand> commands)
		{
			Value = commands;
		}

		public List<ICommand> Value { get; private set; }

		object IDataFlowData.Value
		{
			get { return Value; }
		}
	}
}