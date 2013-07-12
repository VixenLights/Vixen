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

			if (states.Count > 1)
			{
				foreach (IIntentState<LightingValue> intentState in states)
				{
					// If this color is "off" or has no intensity, no reason to put it in the mix...
					if (intentState.GetValue().Intensity > 0)
					{
						Color intentColor = ((IIntentState<LightingValue>)intentState).GetValue().GetOpaqueIntensityAffectedColor();
						c = Color.FromArgb(Math.Max(c.R, intentColor.R),
										   Math.Max(c.G, intentColor.G),
										   Math.Max(c.B, intentColor.B)
										   );
					}
				}

				c = Color.FromArgb((c.R + c.G + c.B) / 3, c.R, c.G, c.B);
			}
			else
			{
				if (states.Count > 0)
				{
					IIntentState<LightingValue> intentState = states[0] as IIntentState<LightingValue>;
					c = ((IIntentState<LightingValue>)intentState).GetValue().GetAlphaChannelIntensityAffectedColor();
				}
			}
			return c;

		}

	}
}