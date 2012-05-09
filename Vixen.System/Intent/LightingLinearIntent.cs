using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LightingLinearIntent : LinearIntent<LightingLinearIntent, LightingValue> {
		public LightingLinearIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new LightingInterpolator()) {
		}
	}
}
