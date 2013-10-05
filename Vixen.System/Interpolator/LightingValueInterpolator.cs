using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue)
		{
			double h = (startValue.hsv.H + (endValue.hsv.H - startValue.hsv.H) * percent);
			double s = (startValue.hsv.S + (endValue.hsv.S - startValue.hsv.S) * percent);
			double v = (startValue.hsv.V + (endValue.hsv.V - startValue.hsv.V) * percent);
			return new LightingValue(h, s, v);
		}
	}
}