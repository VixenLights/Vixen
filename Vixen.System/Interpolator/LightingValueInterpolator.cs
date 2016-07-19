using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			// TODO: this could probably be performance optimized a bit; might not matter next year if we rip the guts out of it
			double closestEndHue;
			if (endValue.Hue - startValue.Hue > 0.5)
				closestEndHue = endValue.Hue - 1.0;
			else if (endValue.Hue - startValue.Hue < -0.5)
				closestEndHue = endValue.Hue + 1.0;
			else
				closestEndHue = endValue.Hue;

			double h = (startValue.Hue + (closestEndHue - startValue.Hue) * percent);
			double s = (startValue.Saturation + (endValue.Saturation - startValue.Saturation) * percent);
			double v = (startValue.Value + (endValue.Value - startValue.Value) * percent);
			double i = (startValue.Intensity + (endValue.Intensity - startValue.Intensity) * percent);

			return new LightingValue((h + 1.0) % 1.0, s, v, i);
		}
	
	}
}