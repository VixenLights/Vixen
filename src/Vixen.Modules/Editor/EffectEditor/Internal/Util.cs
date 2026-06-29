using System.Drawing;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Effect.Effect;
using VixenModules.Property.Color;

namespace VixenModules.Editor.EffectEditor.Internal
{
	/// <summary>
	/// Provides shared Effect Editor helper methods.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Gets the discrete colors that should constrain color editing for the specified component.
		/// </summary>
		/// <param name="component">The component that owns the edited color property.</param>
		/// <returns>The valid discrete colors, or an empty set when full color editing is allowed.</returns>
		public static HashSet<Color> GetDiscreteColors(object component)
		{
			var validColors = new HashSet<Color>();
			if (component is IDiscreteColorProvider discreteColorProvider)
			{
				validColors.AddRange(discreteColorProvider.GetDiscreteColors());
			}
			else if (component is IEffect)
			{
				var effect = (IEffect)component;
				validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			}
			else if (component is Array)
			{
				foreach (var item in (Array)component)
				{
					if (item is IDiscreteColorProvider itemDiscreteColorProvider)
					{
						validColors.AddRange(itemDiscreteColorProvider.GetDiscreteColors());
					}
					else if (item is IEffect)
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
