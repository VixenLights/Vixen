using System;
using System.Drawing;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator {
	class LongEvaluator : Dispatchable<LongEvaluator>, IEvaluator<long>, IAnyIntentStateHandler {
		public long Value { get; private set; }

		public void Evaluate(IIntentState intentState) {
			intentState.Dispatch(this);
		}

		public void Handle(PercentageTransitionIntentState obj) {
			Value = (long)Evaluator.Default(obj);
		}

		public void Handle(IIntentState<float> obj) {
			Value = (long)Evaluator.Default(obj);
			//Value = (long)obj.FilterStates.Aggregate(obj.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		public void Handle(IIntentState<DateTime> obj) {
			// Ignored
		}

		public void Handle(IIntentState<Color> obj) {
			Value = Evaluator.Default(obj);
		}

		public void Handle(IIntentState<long> obj) {
			Value = Evaluator.Default(obj);
		}

		public void Handle(IIntentState<double> obj) {
			Value = (long)Evaluator.Default(obj);
		}
	}
}
