namespace Vixen.Interpolator {
	class DoubleInterpolator : Interpolator<double> {
		protected override double InterpolateValue(double startValue, double endValue, float percent) {
			return startValue + (endValue - startValue) * percent;
		}
	}
}
