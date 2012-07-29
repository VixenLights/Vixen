using System;
using Vixen.Data.Value;

namespace Vixen.Intent {
	public class ColorIntent : LinearIntent<ColorValue> {
		public ColorIntent(ColorValue startValue, ColorValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
		}
	}
}
