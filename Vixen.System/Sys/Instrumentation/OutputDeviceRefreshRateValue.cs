using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation
{
	internal class OutputDeviceRefreshRateValue : RateValue
	{
		public OutputDeviceRefreshRateValue(IOutputDevice outputDevice)
			: base("Output device refresh rate [" + outputDevice.Name + "]")
		{
		}

		protected override string _GetFormattedValue()
		{
			return _GetValue().ToString("0.00 Hz");
		}
	}
}