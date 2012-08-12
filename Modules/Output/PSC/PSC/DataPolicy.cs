using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Controller.PSC {
	class DataPolicy : ControllerDataPolicy {
		protected override IEvaluator GetEvaluator() {
			return new Evaluator();
		}

		protected override ICombinator GetCombinator() {
			return new Combinator();
		}
	}
}
