using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (PositionValue))]
	internal class PositionValueInterpolator : Interpolator<PositionValue>
	{
		protected override PositionValue InterpolateValue(float percent, PositionValue startValue, PositionValue endValue)
		{
			float position = (startValue.Position + (endValue.Position - startValue.Position) * percent);

			return new PositionValue(position);
		}
	}
}