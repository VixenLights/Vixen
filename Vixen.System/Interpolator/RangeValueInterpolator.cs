using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (RangeValue<FunctionIdentity>))]
	internal class RangeValueInterpolator : Interpolator<RangeValue<FunctionIdentity>>
	{
		protected override RangeValue<FunctionIdentity> InterpolateValue(double percent, RangeValue<FunctionIdentity> startValue, RangeValue<FunctionIdentity> endValue)
		{
			double position = (startValue.Value + (endValue.Value - startValue.Value) * percent);

			return new RangeValue<FunctionIdentity>(startValue.TagType, startValue.Tag, position);
		}
	}
}