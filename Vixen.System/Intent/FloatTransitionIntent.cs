using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class FloatTransitionIntent : TransitionIntent<float> {
		public FloatTransitionIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new FloatInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new FloatTransitionIntentState(this, intentRelativeTime);
		}

		protected override TransitionIntent<float> _CreateSegment(float startValue, float endValue, TimeSpan timeSpan) {
			return new FloatTransitionIntent(startValue, endValue, timeSpan);
		}
	}
}
