using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class CommandIntent : StaticIntent<CommandValue>
	{
		public CommandIntent(CommandValue command, TimeSpan timeSpan)
			: base(command, timeSpan)
		{
		}
	}
}