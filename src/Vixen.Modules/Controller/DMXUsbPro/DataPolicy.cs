using Vixen.Data.Combinator._8Bit;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.DmxUsbPro
{
	internal class DataPolicy : ControllerDataPolicy
	{
		protected override ICombinator GetCombinator()
		{
			return new _8BitHighestWinsCombinator();
		}

		protected override IEvaluator GetEvaluator()
		{
			return new _8BitEvaluator();
		}
	}
}