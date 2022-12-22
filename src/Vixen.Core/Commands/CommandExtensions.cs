namespace Vixen.Commands
{
	public static class CommandExtensions
	{
		public static ICommand Max(this _8BitCommand command, _8BitCommand commandOther)
		{
			if (command.CommandValue > commandOther.CommandValue)
			{
				return command;
			}
			return commandOther;
		}
	}
}
