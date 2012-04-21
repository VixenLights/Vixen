using System;

namespace Vixen.Intent {
	class FloatTransitionIntentState : IntentState<FloatTransitionIntentState, float> {
		private FloatTransitionIntent _intent;

		public FloatTransitionIntentState(FloatTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			_intent = intent;
		}

		public override float GetValue() {
			return _intent.GetStateAt(RelativeTime);
		}

		protected override IntentState<FloatTransitionIntentState, float> _Clone() {
			return new FloatTransitionIntentState(_intent, RelativeTime);
		}
	}
}
