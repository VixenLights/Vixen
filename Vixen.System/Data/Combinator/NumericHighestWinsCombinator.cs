using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	class NumericHighestWinsCombinator : Dispatchable<NumericHighestWinsCombinator>, ICombinator<float>, IAnyEvaluatorHandler {
		public void Combine(IEnumerable<IEvaluator> evaluators) {
			Value = 0;

			foreach(IEvaluator evaluator in evaluators) {
				evaluator.Dispatch(this);
			}
		}

		public void Handle(IEvaluator<float> obj) {
			Value = Math.Max(Value, obj.Value);
		}

		public void Handle(IEvaluator<DateTime> obj) {
			// Ignored
		}

		public void Handle(IEvaluator<Color> obj) {
			// Ignored
		}

		public float Value { get; private set; }
	}
}
