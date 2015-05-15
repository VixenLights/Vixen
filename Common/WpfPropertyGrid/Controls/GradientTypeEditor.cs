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
	public class GradientTypeEditor:BaseColorTypeEditor
	{
		public GradientTypeEditor()
		{
			EditedType = KnownTypes.Wpf.ColorGradient;
			InlineTemplate = EditorKeys.GradientEditorKey;
			ExtendedTemplate = null;
		}

		public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null) return;
			if (propertyValue.ParentProperty.IsReadOnly) return;
			ColorGradient gradient = null;
			var value = propertyValue.Value as ColorGradient;
			HashSet<Color> discreteColors = GetDiscreteColors(propertyValue.ParentProperty.Component);
			if (value != null)
			{
				gradient = value;
			}
			else
			{
				gradient=discreteColors.Any()?new ColorGradient(discreteColors.First()):new ColorGradient(Color.White);	
			}
			
			ColorGradientEditor editor = new ColorGradientEditor(gradient, discreteColors.Any(), discreteColors);
			if (editor.ShowDialog() == DialogResult.OK)
			{
				propertyValue.Value = editor.Gradient;
			}
		}
	}
}
