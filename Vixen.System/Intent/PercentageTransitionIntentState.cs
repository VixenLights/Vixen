using System;

namespace Vixen.Intent {
	class PercentageTransitionIntentState : IntentState<PercentageTransitionIntentState, double> {
		private PercentageTransitionIntent _intent;

		public PercentageTransitionIntentState(PercentageTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			_intent = intent;
		}

		public override double GetValue() {
			return _intent.GetStateAt(RelativeTime);
		}

		protected override IntentState<PercentageTransitionIntentState, double> _Clone() {
			return new PercentageTransitionIntentState(_intent, RelativeTime);
		}
	}
}
