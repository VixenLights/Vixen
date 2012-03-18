using System;
using System.Collections.Generic;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	class LongTransitionIntentState : Dispatchable<LongTransitionIntentState>, IIntentState<long> {
		private LongTransitionIntent _intent;
		private LongInterpolator _interpolator;

		public LongTransitionIntentState(LongTransitionIntent intent, TimeSpan intentRelativeTime) {
			_intent = intent;
			RelativeTime = intentRelativeTime;
			_interpolator = new LongInterpolator();
			FilterStates = new List<IFilterState>();
		}

		public TimeSpan RelativeTime { get; private set; }

		public List<IFilterState> FilterStates { get; private set; }

		public long GetValue() {
			long value;
			_interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
			return value;
		}

		public IIntentState Clone() {
			LongTransitionIntentState newState = new LongTransitionIntentState(_intent, RelativeTime);
			newState.FilterStates.AddRange(FilterStates);
			return newState;
		}
	}
}
