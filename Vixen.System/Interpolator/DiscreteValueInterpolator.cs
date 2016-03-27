using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Sys.Attribute.Interpolator(typeof(DiscreteValue))]
	public class DiscreteValueInterpolator: Interpolator<DiscreteValue>
	{
		protected override DiscreteValue InterpolateValue(double percent, DiscreteValue startValue, DiscreteValue endValue)
		{
			// TODO: this could probably be performance optimized a bit; might not matter next year if we rip the guts out of it
			//double closestEndHue;
			//if (endValue.Hue - startValue.Hue > 0.5)
			//	closestEndHue = endValue.Hue - 1.0;
			//else if (endValue.Hue - startValue.Hue < -0.5)
			//	closestEndHue = endValue.Hue + 1.0;
			//else
			//	closestEndHue = endValue.Hue;

			//double h = (startValue.Hue + (closestEndHue - startValue.Hue) * percent);
			//double s = (startValue.Saturation + (endValue.Saturation - startValue.Saturation) * percent);
			//double v = (startValue.Intensity + (endValue.Intensity - startValue.Intensity) * percent);

			//return new LightingValue((h + 1.0) % 1.0, s, v);

			return new DiscreteValue(Linear(startValue.Hue, endValue.Hue, percent),
				Linear(startValue.Saturation, endValue.Saturation, percent),
				Linear(startValue.Intensity, endValue.Intensity, percent));
		}

		private double Linear(double a, double b, double t)
		{
			return a * (1 - t) + b * t;
		}
	}
}
