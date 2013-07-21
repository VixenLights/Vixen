using Vixen.Sys;

namespace Vixen.Commands
{
	public class UnknownValueCommand : Dispatchable<UnknownValueCommand>, ICommand
	{
		public UnknownValueCommand(object value)
		{
			CommandValue = value;
		}

		public object CommandValue { get; set; }
	}
}