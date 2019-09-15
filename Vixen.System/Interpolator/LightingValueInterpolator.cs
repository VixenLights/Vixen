using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			var r = (byte)(startValue.Color.R + (endValue.Color.R - startValue.Color.R) * percent);
			var g = (byte)(startValue.Color.G + (endValue.Color.G - startValue.Color.G) * percent);
			var b = (byte)(startValue.Color.B + (endValue.Color.B - startValue.Color.B) * percent);
			var i = (startValue.Intensity + (endValue.Intensity - startValue.Intensity) * percent);
			return new LightingValue(Color.FromArgb(r, g, b), i);
		}
	
	}
}