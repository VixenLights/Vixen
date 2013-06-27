using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (ColorValue))]
	internal class ColorValueInterpolator : Interpolator<ColorValue>
	{
		private ColorInterpolator _colorInterpolator;

		public ColorValueInterpolator()
		{
			_colorInterpolator = new ColorInterpolator();
		}

		protected override ColorValue InterpolateValue(double percent, ColorValue startValue, ColorValue endValue)
		{
			Color interpolatedColor;
			_colorInterpolator.Interpolate(percent, startValue.Color, endValue.Color, out interpolatedColor);
			return new ColorValue(interpolatedColor);
		}
	}
}