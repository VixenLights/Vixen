using System;
using Vixen.Data.Value;
using Vixen.Sys;
using System.Drawing;

namespace Vixen.Intent
{
	public class ColorIntent : LinearIntent<ColorValue>
	{
		public ColorIntent(ColorValue startValue, ColorValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}

		public static Color GetColorForIntents(IIntentStates states)
		{
			Color c = Color.Empty;

			foreach (IIntentState<LightingValue> intentState in states)
			{
				Color intentColor = ((IIntentState<LightingValue>)intentState).GetValue().GetAlphaChannelIntensityAffectedColor();
				c = Color.FromArgb(Math.Max(c.A, intentColor.A),
								   Math.Max(c.R, intentColor.R),
								   Math.Max(c.G, intentColor.G),
								   Math.Max(c.B, intentColor.B)
								  );
			}

			if (c == Color.Empty || c == Color.Black)
				c = Color.Transparent;

			return c;
		}

	}
}