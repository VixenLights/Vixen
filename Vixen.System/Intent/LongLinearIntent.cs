using System;
using Vixen.Sys;

namespace Vixen.Intent {
	public class LongLinearIntent : LinearIntent<LongLinearIntent,long> {
		public LongLinearIntent(long startValue, long endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
		}

		protected override LinearIntent<LongLinearIntent, long> Spawn(long startValue, long endValue, TimeSpan timeSpan) {
			return new LongLinearIntent(startValue, endValue, timeSpan);
		}
	}
}
