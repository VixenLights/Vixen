using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation
{
	internal class OutputDeviceUpdateTimeValue : DoubleValue
	{
		public OutputDeviceUpdateTimeValue(IOutputDevice outputDevice)
			: base("Output device update time [" + outputDevice.Name + "]")
		{
		}

		protected override string _GetFormattedValue()
		{
			return ((int) _GetValue()) + " ms";
		}
	}
}