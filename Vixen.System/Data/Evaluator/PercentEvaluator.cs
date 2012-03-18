using System;
using System.Drawing;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator {
	class PercentEvaluator : Dispatchable<PercentEvaluator>, IEvaluator<double>, IAnyIntentStateHandler {
		public double Value { get; private set; }

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
			// Ignored
		}

		public void Handle(IIntentState<long> obj) {
			// Ignored
		}

		public void Handle(IIntentState<double> obj) {
			Value = Evaluator.Default(obj);
		}
	}
}
