using System.Drawing;

namespace Vixen.Intents.Interpolators {
	class ColorInterpolator : Interpolator<Color> {
		protected override Color InterpolateValue(Color startValue, Color endValue, float percent) {
			int r = (int)(startValue.R + (endValue.R - startValue.R) * percent);
			int g = (int)(startValue.G + (endValue.G - startValue.G) * percent);
			int b = (int)(startValue.B + (endValue.B - startValue.B) * percent);
			return Color.FromArgb(r, g, b);
		}
	}
}
