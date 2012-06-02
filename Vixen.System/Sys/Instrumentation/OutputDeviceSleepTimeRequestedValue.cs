using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation {
	class OutputDeviceSleepTimeRequestedValue : DoubleValue {
		public OutputDeviceSleepTimeRequestedValue(IOutputDevice outputDevice)
			: base("Output device sleep time (requested) [" + outputDevice.Name + "]") {
		}

		protected override string _GetFormattedValue() {
			return ((int)_GetValue()) + " ms";
		}
}
}
