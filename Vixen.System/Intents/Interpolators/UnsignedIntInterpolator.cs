namespace Vixen.Intents.Interpolators {
	public class UnsignedIntInterpolator : Interpolator<uint> {
		protected override uint InterpolateValue(uint startValue, uint endValue, float percent) {
			return (uint)(startValue + (endValue - startValue) * percent);
		}
	}
}
