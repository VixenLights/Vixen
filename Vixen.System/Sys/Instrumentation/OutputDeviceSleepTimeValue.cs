using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation {
	class OutputDeviceSleepTimeValue : DoubleValue {
		public OutputDeviceSleepTimeValue(IOutputDevice outputDevice)
			: base("Output device sleep time [" + outputDevice.Name + "]") {
		}

		protected override string _GetFormattedValue() {
			return ((int)_GetValue()) + " ms";
		}
	}
}
