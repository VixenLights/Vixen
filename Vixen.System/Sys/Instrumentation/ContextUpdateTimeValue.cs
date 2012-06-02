using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class ContextUpdateTimeValue : DoubleValue {
		public ContextUpdateTimeValue()
			: base("Update time for all contexts") {
		}

		protected override string _GetFormattedValue() {
			return ((int)_GetValue()) + " ms";
		}
	}
}
