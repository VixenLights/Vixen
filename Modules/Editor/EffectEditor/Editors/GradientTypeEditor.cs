using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class GradientTypeEditor : BaseColorTypeEditor
	{
		public GradientTypeEditor():base(typeof(ColorGradient), EditorKeys.GradientEditorKey)
		{
		}

		public override Object ShowDialog(PropertyItem propertyItem, Object propertyValue, IInputElement commandSource)
		{
			ColorGradient gradient;
			var value = propertyValue as ColorGradient;
			HashSet<Color> discreteColors = GetDiscreteColors(propertyItem.Component);
			if (value != null)
			{
				gradient = value;
			}
			else
			{
				gradient = discreteColors.Any() ? new ColorGradient(discreteColors.First()) : new ColorGradient(Color.White);
			}

			ColorGradientEditor editor = new ColorGradientEditor(gradient, discreteColors.Any(), discreteColors);
			editor.Text = propertyItem.DisplayName;
			if (editor.ShowDialog() == DialogResult.OK)
			{
				propertyValue = editor.Gradient;
			}

			return propertyValue;
		}
	}
}