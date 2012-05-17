using System;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LightingLinearIntent : LinearIntent<LightingLinearIntent, LightingValue> {
		public LightingLinearIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
		}

		protected override LinearIntent<LightingLinearIntent, LightingValue> Spawn(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan) {
			return new LightingLinearIntent(startValue, endValue, timeSpan);
		}
	}
}
