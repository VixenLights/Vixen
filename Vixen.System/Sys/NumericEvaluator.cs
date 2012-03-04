using System;
using System.Drawing;
using System.Linq;

namespace Vixen.Sys {
	class NumericEvaluator : Dispatchable<NumericEvaluator>, IEvaluator<float>, IAnyIntentStateHandler {
		private IIntentState _intentState;

		public float Value { get; private set; }

		public void Evaluate(IIntentState intentState) {
			_intentState = intentState;
			intentState.Dispatch(this);
		}

		public void Handle(IIntentState<float> obj) {
			Value = obj.GetValue();
			foreach(IFilterState filterState in _intentState.FilterStates) {
				Value = filterState.Affect(Value);
			}
		}

		public void Handle(IIntentState<DateTime> obj) {
			// Ignored
		}

		public void Handle(IIntentState<Color> obj) {
			Color color = obj.GetValue();

			color = _intentState.FilterStates.Aggregate(color, (current, filterState) => filterState.Affect(current));

			// Stripping the alpha quantity to keep it from going negative.
			Value = color.ToArgb() & 0xffffff;
		}
	}
}
