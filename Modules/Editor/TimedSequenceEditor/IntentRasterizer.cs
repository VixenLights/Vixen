using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

// By no means does this need to be implemented this way.
// By all means, go ahead and do casting.
// They've so drilled into our heads at my job a fear of conditional constructs
//  that I've actually grown fearful of using them, as if I will break a constant
//  in the universe if I go too far with them.

namespace VixenModules.Editor.TimedSequenceEditor {
	class IntentRasterizer : IntentDispatch {
		RectangleF _rect;
		private Graphics _graphics;
		private readonly TimeSpan _oneMillisecond = TimeSpan.FromMilliseconds(1);

		public void Rasterize(IIntent intent, RectangleF rect, Graphics g) {
			// As recommended by R#
			if(Math.Abs(rect.Width - 0) < float.Epsilon || Math.Abs(rect.Height - 0) < float.Epsilon) return;

			_rect = rect;
			_graphics = g;

			intent.Dispatch(this);
		}

		public override void Handle(IIntent<LightingValue> obj) {
			LightingValue startValue = obj.GetStateAt(TimeSpan.Zero);
			// This is gross, but it's because when you get a value from an intent, the time
			// is used in an exclusive manner for reasons.  So this is trying to backup
			// the end time without affecting the the resulting value too much.
			LightingValue endValue = obj.GetStateAt(obj.TimeSpan - _oneMillisecond);
			using(LinearGradientBrush brush = new LinearGradientBrush(_rect, startValue.GetIntensityAffectedColor(), endValue.GetIntensityAffectedColor(), LinearGradientMode.Horizontal)) {
				_graphics.FillRectangle(brush, _rect);
			}
		}
	}
}
