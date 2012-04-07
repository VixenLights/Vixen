using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Combinator;
using Vixen.Data.Evaluator;
using Vixen.Data.Generator;
using Vixen.Data.Policy;
using Vixen.Sys;

namespace TestPreview {
	class TestPreviewDataPolicy : StandardDataPolicy {
		protected override IEvaluator GetEvaluator() {
			return new ColorEvaluator();
		}

		protected override ICombinator GetCombinator() {
			return new ColorCombinator();
		}

		protected override IGenerator GetGenerator() {
			return new ColorCommandGenerator();
		}
	}
}
