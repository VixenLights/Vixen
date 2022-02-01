using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (RangeValue))]
	internal class RangeValueInterpolator : Interpolator<RangeValue>
	{
		protected override RangeValue InterpolateValue(double percent, RangeValue startValue, RangeValue endValue)
		{
			double position = (startValue.Value + (endValue.Value - startValue.Value) * percent);

			return new RangeValue(startValue.TagType, startValue.Tag, position);
		}
	}
}