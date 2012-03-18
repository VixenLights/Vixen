using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator {
	class ColorEvaluator : Dispatchable<ColorEvaluator>, IEvaluator<Color>, IAnyIntentStateHandler {
		public Color Value { get; private set; }

		public void Evaluate(IIntentState intentState) {
			intentState.Dispatch(this);
		}

		public void Handle(IIntentState<float> obj) {
			// Ignored
		}

		public void Handle(IIntentState<DateTime> obj) {
			// Ignored
		}

		public void Handle(IIntentState<Color> obj) {
			Value = Evaluator.Default(obj);
		}

		public void Handle(IIntentState<long> obj) {
			// Ignored
		}

		public void Handle(IIntentState<double> obj) {
			// Ignored
		}
	}
}
