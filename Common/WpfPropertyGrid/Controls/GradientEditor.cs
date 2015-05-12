using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.Property.Color;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class GradientEditor:TypeEditor
	{
		public GradientEditor()
		{
			EditedType = KnownTypes.Wpf.ColorGradient;
			InlineTemplate = EditorKeys.GradientEditorKey;
			ExtendedTemplate = null;
		}

		public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null) return;
			if (propertyValue.ParentProperty.IsReadOnly) return;
			if (propertyValue.Value is ColorGradient)
			{

				HashSet<Color> discreteColors = GetDiscreteColors(propertyValue.ParentProperty.Component);
				ColorGradient curveValue = (ColorGradient) propertyValue.Value;
				ColorGradientEditor editor = new ColorGradientEditor(curveValue, discreteColors.Any(), discreteColors);
				if (editor.ShowDialog() == DialogResult.OK)
				{
					propertyValue.Value = editor.Gradient;
				}
			}
		}

		private HashSet<Color> GetDiscreteColors(Object component)
		{
			HashSet<Color> validColors = new HashSet<Color>();
			if (component is IEffect)
			{
				IEffect effect = (IEffect) component;
				validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));	
			}

			return validColors;
		}
	}
}
