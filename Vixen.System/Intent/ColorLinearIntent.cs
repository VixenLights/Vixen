using System;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Intent {
	public class ColorLinearIntent : LinearIntent<ColorLinearIntent,Color> {
		public ColorLinearIntent(Color startValue, Color endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan) {
		}

		protected override LinearIntent<ColorLinearIntent, Color> Spawn(Color startValue, Color endValue, TimeSpan timeSpan) {
			return new ColorLinearIntent(startValue, endValue, timeSpan);
		}
	}
}
