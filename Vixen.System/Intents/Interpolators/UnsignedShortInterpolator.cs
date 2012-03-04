namespace Vixen.Intents.Interpolators {
	public class UnsignedShortInterpolator : Interpolator<ushort> {
		protected override ushort InterpolateValue(ushort startValue, ushort endValue, float percent) {
			return (ushort)(startValue + (endValue - startValue) * percent);
		}
	}
}
