using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Combinator._8Bit;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.K8055_Controller
{
	internal class K8055DataPolicy : ControllerDataPolicy
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
