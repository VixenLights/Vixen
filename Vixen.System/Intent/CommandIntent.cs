using System;
using Vixen.Data.Value;
using Vixen.Interpolator;

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