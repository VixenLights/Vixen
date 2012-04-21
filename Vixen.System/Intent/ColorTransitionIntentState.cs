using System;
using System.Drawing;

namespace Vixen.Intent {
	class ColorTransitionIntentState : IntentState<ColorTransitionIntentState, Color> {
		private ColorTransitionIntent _intent;

		public ColorTransitionIntentState(ColorTransitionIntent intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			_intent = intent;
		}

		public override Color GetValue() {
			return _intent.GetStateAt(RelativeTime);
		}

		protected override IntentState<ColorTransitionIntentState, Color> _Clone() {
			return new ColorTransitionIntentState(_intent, RelativeTime);
		}
	}
}
