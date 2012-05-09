using System;
using System.Drawing;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public class ColorLinearIntent : LinearIntent<ColorLinearIntent,Color> {
		public ColorLinearIntent(Color startValue, Color endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new ColorInterpolator()) {
		}
	}
}
