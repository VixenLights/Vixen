using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class NumericTransitionIntent : TransitionIntent<float> {
		public NumericTransitionIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new NumericInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new NumericTransitionIntentState(this, intentRelativeTime);
		}
	}
}
