using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	class LightingIntent : Intent<LightingIntent, LightingValue> {
		public LightingIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new LightingInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new LightingIntentState(this, intentRelativeTime);
		}
	}
}
