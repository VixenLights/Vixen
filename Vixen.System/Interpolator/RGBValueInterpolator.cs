using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (RGBValue))]
	internal class RGBValueInterpolator : Interpolator<RGBValue>
	{
		protected override RGBValue InterpolateValue(double percent, RGBValue startValue, RGBValue endValue)
		{
			RGBValue rv;
			rv.R = (byte)(startValue.R + (endValue.R - startValue.R)*percent);
			rv.G = (byte)(startValue.G + (endValue.G - startValue.G)*percent);
			rv.B = (byte)(startValue.B + (endValue.B - startValue.B)*percent);
			return rv;
		}
	}
}