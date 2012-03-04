namespace Vixen.Intents.Interpolators {
	public class SignedIntInterpolator : Interpolator<int> {
		protected override int InterpolateValue(int startValue, int endValue, float percent) {
			return (int)(startValue + (endValue - startValue) * percent);
		}
	}
}
