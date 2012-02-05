using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class ContextUpdateTimeValue : DoubleValue {
		public ContextUpdateTimeValue()
			: base("Context update time") {
		}

		protected override string _GetFormattedValue() {
			return ((int)_GetValue()) + " ms";
		}
	}
}
