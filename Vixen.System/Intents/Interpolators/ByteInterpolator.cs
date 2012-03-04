namespace Vixen.Intents.Interpolators {
	public class ByteInterpolator : Interpolator<byte> {
		protected override byte InterpolateValue(byte startValue, byte endValue, float percent) {
			return (byte)(startValue + (endValue - startValue) * percent);
		}
	}
}
