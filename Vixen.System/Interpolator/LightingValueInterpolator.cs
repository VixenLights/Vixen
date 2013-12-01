using System;
using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			// TODO: this could probably be performance optimized a bit; might not matter next year if we rip the guts out of it
			double closestEndHue;
			if (endValue.hsv.H - startValue.hsv.H > 0.5)
				closestEndHue = endValue.hsv.H - 1.0;
			else if (endValue.hsv.H - startValue.hsv.H < -0.5)
				closestEndHue = endValue.hsv.H + 1.0;
			else
				closestEndHue = endValue.hsv.H;

			double h = (startValue.hsv.H + (closestEndHue - startValue.hsv.H) * percent);
			double s = (startValue.hsv.S + (endValue.hsv.S - startValue.hsv.S) * percent);
			double v = (startValue.hsv.V + (endValue.hsv.V - startValue.hsv.V) * percent);

			return new LightingValue((h + 1.0) % 1.0, s, v);
		}
	}
}