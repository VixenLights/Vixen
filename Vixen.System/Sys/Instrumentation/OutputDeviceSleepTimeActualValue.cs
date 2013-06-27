using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation
{
	internal class OutputDeviceSleepTimeActualValue : DoubleValue
	{
		public OutputDeviceSleepTimeActualValue(IOutputDevice outputDevice)
			: base("Output device sleep time (actual) [" + outputDevice.Name + "]")
		{
		}

		protected override string _GetFormattedValue()
		{
			return ((int) _GetValue()) + " ms";
		}
	}
}