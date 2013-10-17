namespace Vixen.Interpolator
{
	internal class StaticValueInterpolator<T> : Interpolator<T>
	{
		protected override T InterpolateValue(float percent, T startValue, T endValue)
		{
			return startValue;
		}
	}
}