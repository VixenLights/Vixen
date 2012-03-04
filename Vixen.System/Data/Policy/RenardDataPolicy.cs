using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Generator;
using Vixen.Sys;

namespace Vixen.Data.Policy {
	public class RenardDataPolicy : StandardDataPolicy {
		protected override IEvaluator GetEvaluator() {
			//modified to accommodate color, revert later
			return new NumericEvaluator();
		}

		protected override ICombinator GetCombinator() {
			//modified to accommodate color, revert later
			return new NumericHighestWinsCombinator();
		}

		protected override IGenerator GetGenerator() {
			return new ByteCommandGenerator();
		}
	}
}
