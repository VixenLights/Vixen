using System;
using System.Drawing;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class ColorTransitionIntent : Intent<ColorTransitionIntent,Color> {
		public ColorTransitionIntent(Color startValue, Color endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new ColorInterpolator()) {
		}

		public override IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new ColorTransitionIntentState(this, intentRelativeTime);
		}
	}
}
