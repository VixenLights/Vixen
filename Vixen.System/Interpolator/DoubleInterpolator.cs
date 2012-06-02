namespace Vixen.Interpolator {
	[Vixen.Sys.Attribute.Interpolator(typeof(double))]
	class DoubleInterpolator : Interpolator<double> {
		protected override double InterpolateValue(double percent, double startValue, double endValue) {
			return startValue + (endValue - startValue) * percent;
		}
	}
}
