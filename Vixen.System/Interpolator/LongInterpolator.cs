namespace Vixen.Interpolator {
	class LongInterpolator : Interpolator<long> {
		protected override long InterpolateValue(long startValue, long endValue, float percent) {
			return (long)(startValue + (endValue - startValue) * percent);
		}
	}
}
