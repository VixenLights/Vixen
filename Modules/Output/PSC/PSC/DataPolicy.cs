using Vixen.Data.Combinator._16Bit;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Controller.PSC
{
	internal class DataPolicy : ControllerDataPolicy
	{
		protected override IEvaluator GetEvaluator()
		{
			return new PositionEvaluator();
		}

		protected override ICombinator GetCombinator()
		{
			return new _16BitAverageCombinator();
		}
	}
}