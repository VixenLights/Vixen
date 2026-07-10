using Vixen.Commands;

namespace Vixen.Data.Value
{
	public struct CommandValue : IIntentDataType
	{
		public CommandValue(ICommand command)
		{
			Command = command;
		}

		public ICommand Command;
	}
}