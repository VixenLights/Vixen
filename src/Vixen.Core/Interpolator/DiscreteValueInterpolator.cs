using Vixen.Data.Value;
using Vixen.Sys.Attribute;

namespace Vixen.Interpolator
{
	[Interpolator(typeof(DiscreteValue))]
	public class DiscreteValueInterpolator: Interpolator<DiscreteValue>
	{
		protected override DiscreteValue InterpolateValue(double percent, DiscreteValue startValue, DiscreteValue endValue)
		{
			double v = (startValue.Intensity + (endValue.Intensity - startValue.Intensity) * percent);
			return new DiscreteValue(startValue.Color, v);
		}
	}
}
