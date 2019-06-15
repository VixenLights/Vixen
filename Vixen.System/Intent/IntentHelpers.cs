using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class IntentHelpers
	{


		/// <summary>
		/// Deprecated DO NOT USE!!
		/// Given one or more intent states, this will calculate a Color that is the combination of them all, with an
		/// alpha channel calculated from the 'brightness' of the color. The combination will occur in the RGB space,
		/// and will take the maximum component of each color as the resulting color (ie. max of R, max of G, max of B).
		/// </summary>
		[Obsolete("GetAlphaRGBMaxColorForIntents is deprecated")]
		public static Color GetAlphaRGBMaxColorForIntents(IIntentStates states)
		{
			Color result = GetOpaqueRGBMaxColorForIntents(states);

			// have calculated the desired hue/saturation from combining the color components above (in a
			// 'highest-wins in each of R/G/B' fashion). Now we need to figure out the appropriate alpha channel
			// value for the given color. To do that, convert the RGB color to HSV, get the V value to use as
			// our intensity, and apply that to the alpha channel.
			result = Color.FromArgb((byte)(HSV.VFromRgb(result) * Byte.MaxValue), result.R, result.G, result.B);
			return result;
		}

		/// <summary>
		/// Deprecated DO NOT USE!! This method uses casting to obtain the intent value which causes boxing and is not memory efficient
		/// Given one or more intent states, this will calculate a Color that is the combination of them all, in a 'max
		/// RGB component' fashion (ie. max of R, max of G, max of B).
		/// </summary>
		[Obsolete("GetOpaqueRGBMaxColorForIntents is deprecated")]
		public static Color GetOpaqueRGBMaxColorForIntents(IIntentStates states)
		{
			byte R = 0;
			byte G = 0;
			byte B = 0;

			foreach (IIntentState intentState in states.AsList())
			{
				if (intentState is IIntentState<LightingValue> state)
				{
					var lv = state.GetValue();
					if (lv.Intensity > 0)
					{
						Color intentColor = lv.FullColor;
						R = Math.Max(R, intentColor.R);
						G = Math.Max(G, intentColor.G);
						B = Math.Max(B, intentColor.B);
					}
				}
				else if (intentState is IIntentState<RGBValue> rgbState)
				{
					var rv = rgbState.GetValue();
					R = Math.Max(R, rv.R);
					G = Math.Max(G, rv.G);
					B = Math.Max(B, rv.B);
				}
			}

			return Color.FromArgb(R, G, B);
		}

	}
}
