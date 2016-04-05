using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class IntentHelpers
	{
		public static Color GetAlphaColorForIntent(IIntentState state)
		{

			object value = state.GetValue();
			Color c = Color.Transparent;
			if (value is LightingValue)
			{
				LightingValue lv = (LightingValue)value;
				if (lv.Intensity > 0)
				{
					c = lv.FullColorWithAlpha;
				}
			}
			else if (value is RGBValue)
			{
				RGBValue rv = (RGBValue)value;
				c = rv.FullColorWithAplha;
			}

			return c;
		}
		/// <summary>
		/// Given one or more intent states, this will calculate a Color that is the combination of them all, with an
		/// alpha channel calculated from the 'brightness' of the color. The combination will occur in the RGB space,
		/// and will take the maximum component of each color as the resulting color (ie. max of R, max of G, max of B).
		/// </summary>
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
		/// Given one or more intent states, this will calculate a Color that is the combination of them all, in a 'max
		/// RGB component' fashion (ie. max of R, max of G, max of B).
		/// </summary>
		public static Color GetOpaqueRGBMaxColorForIntents(IIntentStates states)
		{
			byte R = 0;
			byte G = 0;
			byte B = 0;

			foreach (IIntentState intentState in states.AsList())
			{
				object value = intentState.GetValue();

				if (value is LightingValue)
				{
					LightingValue lv = (LightingValue)value;
					if (lv.Intensity > 0)
					{
						Color intentColor = lv.FullColor;
						R = Math.Max(R, intentColor.R);
						G = Math.Max(G, intentColor.G);
						B = Math.Max(B, intentColor.B);
					}
				}
				else if (value is RGBValue)
				{
					RGBValue rv = (RGBValue)value;
					R = Math.Max(R, rv.R);
					G = Math.Max(G, rv.G);
					B = Math.Max(B, rv.B);
				}
			}

			return Color.FromArgb(R, G, B);
		}

		/// <summary>
		/// Returns a list of alpha affected distinct colors from the states, combined so that the brightest of each color is returned
		/// </summary>
		public static IEnumerable<Color> GetAlphaAffectedDiscreteColorsForIntents(IIntentStates states)
		{
			List<Color> colors = new List<Color>();

			IEnumerable<IGrouping<Color, IIntentState>> colorStates = states.GroupBy(
				(x =>
				{
					if (x is IIntentState<DiscreteValue>)
					{
						return ((IIntentState<DiscreteValue>) x).GetValue().Color;
					}
					if (x is IIntentState<LightingValue>)
					{
						return ((IIntentState<LightingValue>) x).GetValue().FullColor;
					}
					if (x is IIntentState<RGBValue>)
					{
						return ((IIntentState<RGBValue>) x).GetValue().FullColor;
					}
					
					return Color.Empty;
				}
				));

			foreach (IGrouping<Color, IIntentState> grouping in colorStates)
			{

				double intensity = grouping.Max(x =>
				{
					if (x is IIntentState<DiscreteValue>)
					{
						return ((IIntentState<DiscreteValue>) x).GetValue().Intensity;
					}
					if (x is IIntentState<LightingValue>)
					{
						return ((IIntentState<LightingValue>) x).GetValue().Intensity;
					}
					if (x is IIntentState<RGBValue>)
					{
						return ((IIntentState<RGBValue>) x).GetValue().Intensity;
					}
					
					return 0;
				});

				Color brightest = Color.FromArgb((byte)(intensity * byte.MaxValue), grouping.Key.R, grouping.Key.G, grouping.Key.B);
				colors.Add(brightest);
			}

			return colors;
		}
	}
}
