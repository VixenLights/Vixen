using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.EffectPropertyEditors
{
	public class GradientTypeEditor : System.Windows.Controls.WpfPropertyGrid.Editors.BaseColorTypeEditor
	{
		public GradientTypeEditor(object inlineTemplate): base(typeof(ColorGradient), inlineTemplate)
		{
		}

		public override Object ShowDialog(Object effect, Object propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null)
			{
				return propertyValue;
			}
			
			ColorGradient gradient = null;
			var value = propertyValue as ColorGradient;
			HashSet<Color> discreteColors = GetDiscreteColors(effect);
			if (value != null)
			{
				gradient = value;
			}
			else
			{
				gradient = discreteColors.Any() ? new ColorGradient(discreteColors.First()) : new ColorGradient(Color.White);
			}

			ColorGradientEditor editor = new ColorGradientEditor(gradient, discreteColors.Any(), discreteColors);
			if (editor.ShowDialog() == DialogResult.OK)
			{
				propertyValue = editor.Gradient;
			}

			return propertyValue;
		}
	}
}