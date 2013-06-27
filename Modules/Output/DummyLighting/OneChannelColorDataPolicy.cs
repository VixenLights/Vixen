using Vixen.Data.Combinator.Color;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting
{
	internal class OneChannelColorDataPolicy : ControllerDataPolicy
	{
		protected override IEvaluator GetEvaluator()
		{
			return new ColorEvaluator();
		}

		protected override ICombinator GetCombinator()
		{
			return new NaiveColorCombinator();
		}
	}
}