using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class FloatLinearIntent : LinearIntent<FloatLinearIntent,float> {
		public FloatLinearIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new FloatInterpolator()) {
		}
	}
}
