using Vixen.Sys;

namespace Vixen.Commands
{
	public class StringCommand : Dispatchable<StringCommand>, ICommand
	{
		public StringCommand(string value)
		{
			CommandValue = value;
		}

		public string CommandValue { get; set; }

		object ICommand.CommandValue
		{
			get { return CommandValue; }
			set { CommandValue = (string)value; }
		}
	}
}
