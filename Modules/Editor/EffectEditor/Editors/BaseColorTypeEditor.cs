using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Property.Color;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public abstract class BaseColorTypeEditor : TypeEditor
	{
		protected BaseColorTypeEditor(Type editedType, object inlineTemplate)
			: base(editedType, inlineTemplate, null)
		{
		}

		protected HashSet<Color> GetDiscreteColors(Object component)
		{
			HashSet<Color> validColors = new HashSet<Color>();
			if (component is IEffect)
			{
				IEffect effect = (IEffect) component;
				validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			}
			else if (component is Array)
			{
				foreach (var item in (Array) component)
				{
					if (item is IEffect)
					{
						IEffect effect = (IEffect) item;
						validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
					}
				}
			}

			return validColors;
		}
	}
}