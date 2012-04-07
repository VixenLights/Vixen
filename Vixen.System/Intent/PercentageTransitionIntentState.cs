using System;
using Vixen.Interpolator;

namespace Vixen.Intent {
	class PercentageTransitionIntentState : TransitionIntentState<PercentageTransitionIntentState, double, DoubleInterpolator> {
		private PercentageTransitionIntent _intent;

		public PercentageTransitionIntentState(PercentageTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intent, intentRelativeTime) {
			_intent = intent;
		}

		protected override IntentState<PercentageTransitionIntentState, double> _Clone() {
			return new PercentageTransitionIntentState(_intent, RelativeTime);
		}
	}

	//class PercentageTransitionIntentState : Dispatchable<PercentageTransitionIntentState>, IIntentState<double> {
	//    private PercentageTransitionIntent _intent;
	//    private DoubleInterpolator _interpolator;

	//    public PercentageTransitionIntentState(PercentageTransitionIntent intent, TimeSpan intentRelativeTime) {
	//        _intent = intent;
	//        RelativeTime = intentRelativeTime;
	//        _interpolator = new DoubleInterpolator();
	//        FilterStates = new List<IFilterState>();
	//        SubordinateIntentStates = new List<SubordinateIntentState>();
	//    }

	//    public TimeSpan RelativeTime { get; private set; }

	//    public List<IFilterState> FilterStates { get; private set; }

	//    public double GetValue() {
	//        double value;
	//        _interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
	//        return SubordinateIntentState.Aggregate(value, SubordinateIntentStates);
	//    }

	//    public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }

	//    public IIntentState Clone() {
	//        PercentageTransitionIntentState newState = new PercentageTransitionIntentState(_intent, RelativeTime);
	//        newState.FilterStates.AddRange(FilterStates);
	//        newState.SubordinateIntentStates.AddRange(SubordinateIntentStates);
	//        return newState;
	//    }
	//}
}
