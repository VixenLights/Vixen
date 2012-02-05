using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class ChannelUpdateTimeValue : DoubleValue {
		public ChannelUpdateTimeValue()
			: base("Update time for all channels") {
		}

		protected override string _GetFormattedValue() {
			return ((int)_GetValue()) + " ms";
		}
	}
}
