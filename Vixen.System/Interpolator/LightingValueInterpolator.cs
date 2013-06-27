using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		private ColorInterpolator _colorInterpolator;
		private FloatInterpolator _doubleInterpolator;

		public LightingValueInterpolator()
		{
			_colorInterpolator = new ColorInterpolator();
			_doubleInterpolator = new FloatInterpolator();
		}

		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			Color newColor;
			float newIntensity;

			_colorInterpolator.Interpolate(percent, startValue.Color, endValue.Color, out newColor);
			_doubleInterpolator.Interpolate(percent, startValue.Intensity, endValue.Intensity, out newIntensity);

			return new LightingValue(newColor, newIntensity);
		}
	}
}