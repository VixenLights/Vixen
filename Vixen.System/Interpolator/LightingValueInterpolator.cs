using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof (LightingValue))]
	internal class LightingValueInterpolator : Interpolator<LightingValue>
	{
		protected override LightingValue InterpolateValue(float percent, LightingValue startValue, LightingValue endValue)
		{
			float h = (startValue.hsv.H + (endValue.hsv.H - startValue.hsv.H) * percent);
			float s = (startValue.hsv.S + (endValue.hsv.S - startValue.hsv.S) * percent);
			float v = (startValue.hsv.V + (endValue.hsv.V - startValue.hsv.V) * percent);
			return new LightingValue(h, s, v);
		}
	}
}