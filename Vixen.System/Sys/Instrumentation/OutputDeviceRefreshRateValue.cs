using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation {
	internal class OutputDeviceRefreshRateValue : RateValue {
		public OutputDeviceRefreshRateValue(IOutputDevice outputDevice)
			: base(string.Format("Output device refresh rate [{0}]", outputDevice.Name)) {
		}

		protected override string _GetFormattedValue() {
			return _GetValue().ToString("0.00 Hz");
		}
	}
}