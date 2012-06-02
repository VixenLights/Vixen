using System;
using Vixen.Sys;

namespace Vixen.Intent {
	public class FloatLinearIntent : LinearIntent<FloatLinearIntent,float> {
		public FloatLinearIntent(float startValue, float endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
		}

		protected override LinearIntent<FloatLinearIntent, float> Spawn(float startValue, float endValue, TimeSpan timeSpan) {
			return new FloatLinearIntent(startValue, endValue, timeSpan);
		}
	}
}
