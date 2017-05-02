using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (RGBValue))]
	internal class RGBValueInterpolator : Interpolator<RGBValue>
	{
		protected override RGBValue InterpolateValue(double percent, RGBValue startValue, RGBValue endValue)
		{
			var r = (byte)(startValue.R + (endValue.R - startValue.R)*percent);
			var g = (byte)(startValue.G + (endValue.G - startValue.G)*percent);
			var b = (byte)(startValue.B + (endValue.B - startValue.B)*percent);
			return new RGBValue(r, g, b);
		}
	}
}