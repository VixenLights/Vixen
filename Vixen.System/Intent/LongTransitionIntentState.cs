using System;

namespace Vixen.Intent {
	class LongTransitionIntentState : IntentState<LongTransitionIntentState, long> {
		private LongTransitionIntent _intent;

		public LongTransitionIntentState(LongTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			_intent = intent;
		}

		public override long GetValue() {
			return _intent.GetStateAt(RelativeTime);
		}

		protected override IntentState<LongTransitionIntentState, long> _Clone() {
			return new LongTransitionIntentState(_intent, RelativeTime);
		}
	}
}
