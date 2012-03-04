namespace Vixen.Intents.Interpolators {
	public class SignedShortInterpolator : Interpolator<short> {
		protected override short InterpolateValue(short startValue, short endValue, float percent) {
			return (short)(startValue + (endValue - startValue) * percent);
		}
	}
}
