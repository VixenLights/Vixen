using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace PSC {
	//Out of lack of a real-world need at this point...
	class DataPolicy : ControllerDataPolicy {
		protected override IEvaluator GetEvaluator() {
			return new PercentEvaluator();
		}

		protected override ICombinator GetCombinator() {
			return new PercentageHighestWinsCombinator();
		}
	}
}
