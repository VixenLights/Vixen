using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class ContextUpdateTimeValue : DoubleValue
	{
		public ContextUpdateTimeValue()
			: base("Update time for all contexts")
		{
		}

		protected override string _GetFormattedValue()
		{
			return string.Format("{0} ms", (int) _GetValue());
		}
	}
}