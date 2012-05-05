using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class FloatTransitionIntent : Intent<FloatTransitionIntent,float> {
		public FloatTransitionIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new FloatInterpolator()) {
		}

		//public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
		//    //return new FloatTransitionIntentState(this, intentRelativeTime);
		//    return new IntentState<float>(this, intentRelativeTime);
		//}
	}
}
