using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Generator;
using Vixen.Sys;

namespace Vixen.Data.Policy {
	public class RenardDataPolicy : StandardDataPolicy {
		protected override IEvaluator GetEvaluator() {
			//When testing with effect generating 0-1 % values
			//return new PercentEvaluator();
			//When testing with effect generating 0-255 float values
			//return new FloatEvaluator();
			//When testing with effect generating 0-255 integer values
			return new LongEvaluator();
		}

		protected override ICombinator GetCombinator() {
			return new FloatHighestWinsCombinator();
			//return new PercentageHighestWinsCombinator();
		}

		protected override IGenerator GetGenerator() {
			return new ByteCommandGenerator();
		}
	}
}
