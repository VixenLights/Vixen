using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Flow
{
	public class CommandDataFlowData : Dispatchable<CommandDataFlowData>, IDataFlowData<ICommand>
	{
		public CommandDataFlowData(ICommand command)
		{
			Value = command;
		}

		public ICommand Value { get; set; }

		object IDataFlowData.Value
		{
			get { return Value; }
		}
	}
}