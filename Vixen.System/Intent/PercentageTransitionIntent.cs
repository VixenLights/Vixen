using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class PercentageTransitionIntent : TransitionIntent<double> {
		public PercentageTransitionIntent(double startValue, double endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new DoubleInterpolator()) {
			if(startValue < 0 || startValue > 1) throw new ArgumentException("startValue is out of range");
			if(endValue < 0 || endValue > 1) throw new ArgumentException("endValue is out of range");
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new PercentageTransitionIntentState(this, intentRelativeTime);
		}
	}
}
