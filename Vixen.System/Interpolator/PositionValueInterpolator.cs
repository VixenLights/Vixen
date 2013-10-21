using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (PositionValue))]
	internal class PositionValueInterpolator : Interpolator<PositionValue>
	{
		protected override PositionValue InterpolateValue(double percent, PositionValue startValue, PositionValue endValue)
		{
			double position = (startValue.Position + (endValue.Position - startValue.Position) * percent);

			return new PositionValue(position);
		}
	}
}