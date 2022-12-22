using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class CommandsExpiredPercentValue : PercentValue
	{
		public CommandsExpiredPercentValue()
			: base("Commands - Expired (%)")
		{
		}
	}
}