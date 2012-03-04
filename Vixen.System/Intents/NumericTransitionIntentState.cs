using System;
using System.Collections.Generic;
using Vixen.Intents.Interpolators;
using Vixen.Sys;

namespace Vixen.Intents {
	class NumericTransitionIntentState : Dispatchable<NumericTransitionIntentState>, IIntentState<float> {
		private NumericTransitionIntent _intent;
		private FloatInterpolator _interpolator;

		public NumericTransitionIntentState(NumericTransitionIntent intent, TimeSpan intentRelativeTime) {
			_intent = intent;
			RelativeTime = intentRelativeTime;
			_interpolator = new FloatInterpolator();
			FilterStates = new List<IFilterState>();
		}

		public TimeSpan RelativeTime { get; private set; }

		public List<IFilterState> FilterStates { get; private set; }

		public float GetValue() {
			float value;
			_interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
			return value;
		}

		public IIntentState Clone() {
			NumericTransitionIntentState newState = new NumericTransitionIntentState(_intent, RelativeTime);
			newState.FilterStates.AddRange(FilterStates);
			return newState;
		}
	}
}
