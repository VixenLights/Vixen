using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.DmxUsbPro {
	class DataPolicy : ControllerDataPolicy {
		protected override ICombinator GetCombinator() {
			return new LightingCombinator();
		}

		protected override IEvaluator GetEvaluator() {
			return new LightingEvaluator();
		}
	}
}
