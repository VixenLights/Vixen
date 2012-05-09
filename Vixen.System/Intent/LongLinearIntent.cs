using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LongLinearIntent : LinearIntent<LongLinearIntent,long> {
		public LongLinearIntent(long startValue, long endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new LongInterpolator()) {
		}
	}
}
