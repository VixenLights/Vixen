using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Data.Policy;
using Vixen.Sys;
using Vixen.Data.Evaluator;
using Vixen.Data.Combinator._8Bit;

namespace VixenModules.Output.HelixController
{
	class HelixDataPolicy : ControllerDataPolicy
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
