using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class ColorTypeEditor : BaseColorTypeEditor
	{
		public ColorTypeEditor()
			: base(typeof(Color), EditorKeys.ColorEditorKey)
		{
		}

		public override Object ShowDialog(Object effect, Object propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null)
			{
				return propertyValue;
			}
			
			HashSet<Color> discreteColors = GetDiscreteColors(effect);

			Color colorValue = Color.Black;
			if (propertyValue != null)
			{
				colorValue = (Color)propertyValue;
			}
			DialogResult result;
			if (discreteColors.Any())
			{
				using (DiscreteColorPicker dcp = new DiscreteColorPicker())
				{
					dcp.ValidColors = discreteColors;
					dcp.SingleColorOnly = true;
					dcp.SelectedColors = new List<Color> {colorValue};
					result = dcp.ShowDialog();
					if (result == DialogResult.OK)
					{
						propertyValue = !dcp.SelectedColors.Any() ? discreteColors.First() : dcp.SelectedColors.First();
					}
				}
			}
			else
			{
				using (ColorPicker cp = new ColorPicker())
				{
					cp.LockValue_V = true;
					cp.Color = XYZ.FromRGB(colorValue);
					result = cp.ShowDialog();
					if (result == DialogResult.OK)
					{
						propertyValue = cp.Color.ToRGB().ToArgb();
					}
				}
			}

			return propertyValue;
		}
	}
}