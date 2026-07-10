using Vixen.Data.Combinator._8Bit;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.Renard
{
	public class RenardDataPolicy : ControllerDataPolicy
	{
		protected override IEvaluator GetEvaluator()
		{
			return new _8BitEvaluator();
		}

		protected override ICombinator GetCombinator()
		{
			return new _8BitHighestWinsCombinator();
		}
	}
}