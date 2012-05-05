using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LongTransitionIntent : Intent<LongTransitionIntent,long> {
		public LongTransitionIntent(long startValue, long endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new LongInterpolator()) {
		}

		//public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
		//    //return new LongTransitionIntentState(this, intentRelativeTime);
		//    return new IntentState<long>(this, intentRelativeTime);
		//}
	}
}
