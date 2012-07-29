using Vixen.Data.Value;

namespace Vixen.Interpolator {
	[Vixen.Sys.Attribute.Interpolator(typeof(PositionValue))]
	class PositionValueInterpolator : Interpolator<PositionValue> {
		private FloatInterpolator _floatInterpolator;

		public PositionValueInterpolator() {
			_floatInterpolator = new FloatInterpolator();
		}

		protected override PositionValue InterpolateValue(double percent, PositionValue startValue, PositionValue endValue) {
			float interpolatedValue;
			_floatInterpolator.Interpolate(percent, startValue.Position, endValue.Position, out interpolatedValue);

			return new PositionValue(interpolatedValue);
		}
	}
}
