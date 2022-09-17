using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.DiscreteColorPicker;
using Common.DiscreteColorPicker.Views;

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
			
			if (discreteColors.Any())
			{
				// Create the discrete single color picker view
				SingleDiscreteColorPickerView discreteColorPickerView = new SingleDiscreteColorPickerView(discreteColors, colorValue);
				
				// Show the single color picker window
				bool? result = discreteColorPickerView.ShowDialog();

				// If the user selected the OK button then...
				if (result.HasValue &&
				    result.Value)
				{
					// Retrieve the selected color
					propertyValue = discreteColorPickerView.GetSelectedColor();
				}
			}
			else
			{
				using (ColorPicker cp = new ColorPicker())
				{
					cp.LockValue_V = false;
					cp.Color = XYZ.FromRGB(colorValue);
					cp.Text = propertyItem.DisplayName;
					DialogResult result = cp.ShowDialog();
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