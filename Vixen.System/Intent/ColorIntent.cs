using System;
//using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Sys;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Intent
{
	public class ColorIntent : LinearIntent<ColorValue>
	{
		public ColorIntent(ColorValue startValue, ColorValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}

		public static Color GetAlphaColorForIntents(IIntentStates states)
		{
			Color result = GetOpaqueColorForIntents(states);

			// have calculated the desired hue/saturation from combining the color components above (in a
			// 'highest-wins in each of R/G/B' fashion). Now we need to figure out the appropriate alpha channel
			// value for the given color. To do that, convert the RGB color to HSL, get the L value to use as
			// our intensity, and apply that to the alpha channel.
			// TODO: using the RGB class is bad. It's an external module which means the Vixen DLL is dependent on it;
			// if there's anything added to the external module which depends on Vixen.System, we'll have dependency fun.
			// to fix it, we really should strip out color models and management and use them within the Vixen system itself.
			HSV hsv = HSV.FromRGB(result);

			result = Color.FromArgb((byte)(hsv.V * byte.MaxValue), result.R, result.G, result.B);
			return result;
		}

		public static Color GetOpaqueColorForIntents(IIntentStates states)
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