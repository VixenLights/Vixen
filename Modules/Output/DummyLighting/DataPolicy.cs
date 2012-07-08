using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace VixenModules.Output.DummyLighting {
	class DataPolicy : ControllerDataPolicy {
		protected override IEvaluator GetEvaluator() {
			return new LightingEvaluator();
		}

		protected override ICombinator GetCombinator() {
			return new LightingCombinator();
		}
	}
}
