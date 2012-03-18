using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	class ColorCombinator : Dispatchable<ColorCombinator>, ICombinator<Color>, IAnyEvaluatorHandler {
		public void Combine(IEnumerable<IEvaluator> evaluators) {
			Value = Color.Empty;

			foreach(IEvaluator evaluator in evaluators) {
				evaluator.Dispatch(this);
			}
		}

		public void Handle(IEvaluator<float> obj) {
			// Ignored
		}

		public void Handle(IEvaluator<DateTime> obj) {
			// Ignored
		}

		public void Handle(IEvaluator<Color> obj) {
			// Black + White = White
			// Blue + Yellow = Green ([0,255,0] + [255,0,255] = [0,0,255]?)
			// Bah, just going with highest-wins on each component for now.
			Value = Color.FromArgb(
				Math.Max(Value.R, obj.Value.R),
				Math.Max(Value.G, obj.Value.G),
				Math.Max(Value.B, obj.Value.B));
		}

		public void Handle(IEvaluator<long> obj) {
			// Ignored
		}

		public void Handle(IEvaluator<double> obj) {
			// Ignored
		}

		public Color Value { get; private set; }
	}
}
