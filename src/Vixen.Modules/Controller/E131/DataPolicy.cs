using Vixen.Data.Combinator._8Bit;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.E131
{
	internal class DataPolicy : ControllerDataPolicy
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