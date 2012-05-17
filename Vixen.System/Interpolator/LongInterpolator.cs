namespace Vixen.Interpolator {
	[Vixen.Sys.Attribute.Interpolator(typeof(long))]
	class LongInterpolator : Interpolator<long> {
		protected override long InterpolateValue(double percent, long startValue, long endValue) {
			return (long)(startValue + (endValue - startValue) * percent);
		}
	}
}
