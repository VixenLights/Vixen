using System;
using System.Collections.Generic;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	class PercentageTransitionIntentState : Dispatchable<PercentageTransitionIntentState>, IIntentState<double> {
		private PercentageTransitionIntent _intent;
		private DoubleInterpolator _interpolator;

		public PercentageTransitionIntentState(PercentageTransitionIntent intent, TimeSpan intentRelativeTime) {
			_intent = intent;
			RelativeTime = intentRelativeTime;
			_interpolator = new DoubleInterpolator();
			FilterStates = new List<IFilterState>();
		}

		public TimeSpan RelativeTime { get; private set; }

		public List<IFilterState> FilterStates { get; private set; }

		public double GetValue() {
			double value;
			_interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
			return value;
		}

		public IIntentState Clone() {
			PercentageTransitionIntentState newState = new PercentageTransitionIntentState(_intent, RelativeTime);
			newState.FilterStates.AddRange(FilterStates);
			return newState;
		}
	}
}
