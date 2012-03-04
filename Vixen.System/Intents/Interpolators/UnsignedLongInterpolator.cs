namespace Vixen.Intents.Interpolators {
	public class UnsignedLongInterpolator : Interpolator<ulong> {
		protected override ulong InterpolateValue(ulong startValue, ulong endValue, float percent) {
			return (ulong)(startValue + (endValue - startValue) * percent);
		}
	}
}
