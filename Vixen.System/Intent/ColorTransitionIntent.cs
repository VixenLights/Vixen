using System;
using System.Drawing;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class ColorTransitionIntent : TransitionIntent<Color> {
		public ColorTransitionIntent(Color startValue, Color endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new ColorInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new ColorTransitionIntentState(this, intentRelativeTime);
		}

		protected override TransitionIntent<Color> _CreateSegment(Color startValue, Color endValue, TimeSpan timeSpan) {
			return new ColorTransitionIntent(startValue, endValue, timeSpan);
		}
	}
}
