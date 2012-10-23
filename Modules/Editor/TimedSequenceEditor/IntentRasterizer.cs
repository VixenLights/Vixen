using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vixen.Data.Value;
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
		private readonly TimeSpan _oneTick = TimeSpan.FromTicks(1);

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
			LightingValue endValue = obj.GetStateAt(obj.TimeSpan - _oneTick);

			// Why we have to do this? I have no idea, but without it, the gradient rendering gives strange artefacts.
			// (If you want to see what I mean, make a long spin (minutes) across a bunch of channels in a group with
			// a simple pulse down (or up). The ends/starts of the effect flip to the color of the other end briefly,
			// for a single pixel width. I'm guessing it's an issue in the gradient rendering for large shapes where
			// the gradient rectangle is within the same integer range as the rendering rectangle.
			float offset = _rect.X * 0.004F;
			RectangleF gradientRectangle = new RectangleF(
				(_rect.X) - offset,
				_rect.Y,
				(_rect.Width) + (2 * offset),
				_rect.Height
				);
			//(float)Math.Floor(_rect.X) - (_rect.X / 300),   _rect.Y,   (float)Math.Ceiling(_rect.Width) + (_rect.Right / 300) + 1.0F,  _rect.Height

			using (LinearGradientBrush brush = new LinearGradientBrush(gradientRectangle, startValue.GetIntensityAffectedColor(), endValue.GetIntensityAffectedColor(), LinearGradientMode.Horizontal)) {
				_graphics.FillRectangle(brush, _rect);
			}
		}
	}
}
