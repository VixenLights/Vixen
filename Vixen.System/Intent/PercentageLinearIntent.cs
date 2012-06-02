using System;
using Vixen.Sys;

namespace Vixen.Intent {
	public class PercentageLinearIntent : LinearIntent<PercentageLinearIntent,double> {
		public PercentageLinearIntent(double startValue, double endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
			if(startValue < 0 || startValue > 1) throw new ArgumentException("startValue is out of range");
			if(endValue < 0 || endValue > 1) throw new ArgumentException("endValue is out of range");
		}

		protected override LinearIntent<PercentageLinearIntent, double> Spawn(double startValue, double endValue, TimeSpan timeSpan) {
			return new PercentageLinearIntent(startValue, endValue, timeSpan);
		}
	}
}
