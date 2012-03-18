using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LongTransitionIntent : TransitionIntent<long> {
		public LongTransitionIntent(long startValue, long endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new LongInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new LongTransitionIntentState(this, intentRelativeTime);
		}
	}
}
