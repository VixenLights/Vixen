using System;
using Vixen.Intents.Interpolators;
using Vixen.Sys;

namespace Vixen.Intents {
	public class NumericTransitionIntent : TransitionIntent<float> {
		public NumericTransitionIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new NumericInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new NumericTransitionIntentState(this, intentRelativeTime);
		}
	}
}
