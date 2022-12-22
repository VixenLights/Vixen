using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class CommandsExpiredCountValue : CountValue
	{
		public CommandsExpiredCountValue()
			: base("Commands - Expired (count)")
		{
		}
	}
}