using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Combinator.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.RDSController {
	internal class DataPolicy : ControllerDataPolicy {
		protected override IEvaluator GetEvaluator() {
			return new  DynamicEvaluator();
		}

		protected override ICombinator GetCombinator() {
			return new DynamicCombinator();
		}
	}
}
