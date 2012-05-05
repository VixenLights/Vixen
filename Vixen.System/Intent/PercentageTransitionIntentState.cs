using System;
using Vixen.Sys;

namespace Vixen.Intent {
	class PercentageTransitionIntentState : IntentState<PercentageTransitionIntentState, double> {
		public PercentageTransitionIntentState(IIntent<double> intent, TimeSpan intentRelativeTime)
			: base(intent, intentRelativeTime) {
		}

		protected override IntentState<PercentageTransitionIntentState, double> _Clone() {
			return new PercentageTransitionIntentState(Intent, RelativeTime);
		}
		//private PercentageTransitionIntent _intent;

		//public PercentageTransitionIntentState(PercentageTransitionIntent intent, TimeSpan intentRelativeTime)
		//    : base(intentRelativeTime) {
		//    _intent = intent;
		//}

		//public override double GetValue() {
		//    return _intent.GetStateAt(RelativeTime);
		//}

		//protected override IntentState<PercentageTransitionIntentState, double> _Clone() {
		//    return new PercentageTransitionIntentState(_intent, RelativeTime);
		//}
	}
}
