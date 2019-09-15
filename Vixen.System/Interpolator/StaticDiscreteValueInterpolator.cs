using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	internal class StaticDiscreteValueInterpolator: StaticValueInterpolator<DiscreteValue>
	{
		#region Overrides of Interpolator<DiscreteValue>

		/// <inheritdoc />
		protected override DiscreteValue InterpolateValue(double percent, DiscreteValue startValue, DiscreteValue endValue)
		{
			return new DiscreteValue(startValue.Color, startValue.Intensity);
		}

		#endregion
	}
}
