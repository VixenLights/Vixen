using System;
using Vixen.Sys;

namespace Vixen.Intent {
	class LightingIntentState : IntentState<LightingIntentState, LightingValue> {
		private LightingIntent _intent;

		public LightingIntentState(LightingIntent intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			if(intent == null) throw new ArgumentNullException("intent");
			_intent = intent;
		}

		public override LightingValue GetValue() {
			return _intent.GetStateAt(RelativeTime);
		}

		protected override IntentState<LightingIntentState, LightingValue> _Clone() {
			return new LightingIntentState(_intent, RelativeTime);
		}
	}
}
