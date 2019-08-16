using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	internal class StaticLightingValueInterpolator: StaticValueInterpolator<LightingValue>
	{
		#region Overrides of StaticValueInterpolator<LightingValue>

		/// <inheritdoc />
		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			return new LightingValue(startValue);
		}

		#endregion
	}
}
