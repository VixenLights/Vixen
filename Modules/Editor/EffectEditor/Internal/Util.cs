using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Property.Color;

namespace VixenModules.Editor.EffectEditor.Internal
{
	public static class Util
	{
		public static HashSet<Color> GetDiscreteColors(object component)
		{
			var validColors = new HashSet<Color>();
			if (component is IEffect)
			{
				var effect = (IEffect)component;
				validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			}
			else if (component is Array)
			{
				foreach (var item in (Array)component)
				{
					if (item is IEffect)
					{
						var effect = (IEffect)item;
						validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
					}
				}
			}

			return validColors;
		}
	}
}
