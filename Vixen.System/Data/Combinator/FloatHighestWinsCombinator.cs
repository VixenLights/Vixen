using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	class FloatHighestWinsCombinator : Dispatchable<FloatHighestWinsCombinator>, ICombinator<float>, IAnyEvaluatorHandler {
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

		public void Handle(IEvaluator<long> obj) {
			Value = Math.Max(Value, obj.Value);
		}

		public void Handle(IEvaluator<double> obj) {
			// Ignored
			// Would turn the 0-1 double into a 0-1 float which would
			// cause it to be interpreted as an absolute value instead of
			// a % value.
		}

		public float Value { get; private set; }
	}
}
