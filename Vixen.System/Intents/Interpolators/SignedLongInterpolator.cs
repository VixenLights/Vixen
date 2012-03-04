namespace Vixen.Intents.Interpolators {
	public class SignedLongInterpolator : Interpolator<long> {
		protected override long InterpolateValue(long startValue, long endValue, float percent) {
			return (long)(startValue + (endValue - startValue) * percent);
		}
	}
}
