using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Generator;
using Vixen.Sys;

namespace Vixen.Data.Policy {
	public class RenardDataPolicy : StandardDataPolicy {
		protected override IEvaluator GetEvaluator() {
			//When testing with effect generating 0-1 % values, produces double
			//return new PercentEvaluator();
			//When testing with effect generating 0-255 float values; produces float
			//return new FloatEvaluator();
			//When testing with effect generating 0-255 integer values; produces long
			//return new LongEvaluator();
			//When tetsting with effect generating color values; produces color
			return new ColorEvaluator();
		}

		protected override ICombinator GetCombinator() {
			//Combines evaluator values as floats
			//return new FloatHighestWinsCombinator();
			//Combines evaluator values as % (double 0-1)
			//return new PercentageHighestWinsCombinator();
			//Combines evaluator values as colors
			return new ColorCombinator();
		}

		protected override IGenerator GetGenerator() {
			//Just works :)
			return new ByteCommandGenerator();
		}
	}
}
