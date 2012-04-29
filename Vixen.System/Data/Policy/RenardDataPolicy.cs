using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Sys;

//THIS DOES NOT REMAIN.  FOR TESTING ONLY.
namespace Vixen.Data.Policy {
	public class RenardDataPolicy : ControllerDataPolicy {
		protected override IEvaluator GetEvaluator() {
			//When testing with effect generating 0-1 % values, produces double value
			//return new PercentEvaluator();
			//When testing with effect generating 0-255 float values; produces float value
			//return new FloatEvaluator();
			//When testing with effect generating 0-255 integer values; produces long value
			//return new LongEvaluator();
			//When testing with effect generating color values; produces color value
			//return new ColorEvaluator();
			return new LightingEvaluator();
		}

		protected override ICombinator GetCombinator() {
			//Combines evaluator values as floats (use with float or long)
			//return new FloatHighestWinsCombinator();
			//Combines evaluator values as % (double 0-1)
			//return new PercentageHighestWinsCombinator();
			//Combines evaluator values as colors
			//return new ColorCombinator();
			return new LightingCombinator();
		}

		//protected override IGenerator GetGenerator() {
		//    //Just works :)
		//    return new ByteCommandGenerator();
		//}
	}
}
