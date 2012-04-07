using System;
using Vixen.Interpolator;

namespace Vixen.Intent {
	class LongTransitionIntentState : TransitionIntentState<LongTransitionIntentState, long, LongInterpolator> {
		private LongTransitionIntent _intent;

		public LongTransitionIntentState(LongTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intent, intentRelativeTime) {
			_intent = intent;
		}

		protected override IntentState<LongTransitionIntentState, long> _Clone() {
			return new LongTransitionIntentState(_intent, RelativeTime);
		}
	}
	//class LongTransitionIntentState : Dispatchable<LongTransitionIntentState>, IIntentState<long> {
	//    private LongTransitionIntent _intent;
	//    private LongInterpolator _interpolator;

	//    public LongTransitionIntentState(LongTransitionIntent intent, TimeSpan intentRelativeTime) {
	//        _intent = intent;
	//        RelativeTime = intentRelativeTime;
	//        _interpolator = new LongInterpolator();
	//        FilterStates = new List<IFilterState>();
	//        SubordinateIntentStates = new List<SubordinateIntentState>();
	//    }

	//    public TimeSpan RelativeTime { get; private set; }

	//    public List<IFilterState> FilterStates { get; private set; }

	//    public long GetValue() {
	//        long value;
	//        _interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
	//        return SubordinateIntentState.Aggregate(value, SubordinateIntentStates);
	//    }

	//    public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }

	//    public IIntentState Clone() {
	//        LongTransitionIntentState newState = new LongTransitionIntentState(_intent, RelativeTime);
	//        newState.FilterStates.AddRange(FilterStates);
	//        newState.SubordinateIntentStates.AddRange(SubordinateIntentStates);
	//        return newState;
	//    }
	//}
}
