using System;
using System.Collections.Generic;
using System.Linq;
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
				foreach (IIntentState intentState in states)
				{
					// If this color is "off" or has no intensity, no reason to put it in the mix...
					LightingValue lv = (LightingValue)intentState.GetValue();
					if (lv.Intensity > 0)
					{
						Color intentColor = lv.GetOpaqueIntensityAffectedColor();
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
					IIntentState intentState = states[0];
					LightingValue lv = (LightingValue)intentState.GetValue();
					c = lv.GetOpaqueIntensityAffectedColor();
				}
			}

			return c;
		}

		/// <summary>
		/// Returns a distinct list of intensity affected colors from the states in a highest value wins combine method 
		/// </summary>
		/// <param name="states"></param>
		/// <returns></returns>
		public static IEnumerable<Color> GetIntensityAffectedColorForDiscreteStates(IIntentStates states)
		{

			if (states.Count() == 1) return new List<Color> { (states[0] as IIntentState<LightingValue>).GetValue().GetAlphaChannelIntensityAffectedColor() };
			IEnumerable<IGrouping<Color, IIntentState>> colorStates = states.GroupBy(x => (x as IIntentState<LightingValue>).GetValue().Color);
			List<Color> colors = new List<Color>(colorStates.Count());
			foreach (var group in colorStates)
			{
				IIntentState<LightingValue> maxValue = group.FirstOrDefault() as IIntentState<LightingValue>;
				foreach (IIntentState<LightingValue> state in group.Skip(1))
				{
					if (state.GetValue().Intensity > maxValue.GetValue().Intensity)
					{
						maxValue = state;
					}
				}
				//IIntentState<LightingValue> intentState = group.Aggregate((agg, next) =>
					//(next as IntentState<LightingValue>).GetValue().Intensity > (agg as IIntentState<LightingValue>).GetValue().Intensity ? next : agg) as IIntentState<LightingValue>;
				colors.Add(maxValue.GetValue().GetAlphaChannelIntensityAffectedColor());
			}
			
			return colors;
		}

	}
}