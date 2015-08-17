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

		public override Object ShowDialog(PropertyItem propertyItem, Object propertyValue, IInputElement commandSource)
		{
			HashSet<Color> discreteColors = GetDiscreteColors(propertyItem.Component);

			Color colorValue;
			if (propertyValue != null)
			{
				colorValue = (Color)propertyValue;
			}
			else
			{
				colorValue = discreteColors.Any() ? discreteColors.First() : Color.White;
			}
			DialogResult result;
			if (discreteColors.Any())
			{
				using (DiscreteColorPicker dcp = new DiscreteColorPicker())
				{
					dcp.ValidColors = discreteColors;
					dcp.SingleColorOnly = true;
					dcp.SelectedColors = new List<Color> {colorValue};
					dcp.Text = propertyItem.DisplayName;
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
					cp.LockValue_V = false;
					cp.Color = XYZ.FromRGB(colorValue);
					cp.Text = propertyItem.DisplayName;
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