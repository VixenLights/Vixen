using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.DiscreteColorPicker.Views;
using Vixen.Sys;
using ColorProperty = VixenModules.Property.Color;

namespace VixenModules.Property.State.Setup.Services
{
	internal sealed class StateColorPickerService : IStateColorPickerService
	{
		public System.Drawing.Color? ChooseColor(IEnumerable<IElementNode> nodes, System.Drawing.Color initialColor)
		{
			ArgumentNullException.ThrowIfNull(nodes);

			var colors = nodes
				.SelectMany(node => ColorProperty.ColorModule.getValidColorsForElementNode(node, true))
				.ToHashSet();
			if (colors.Count > 0)
			{
				var colorPickerView = new SingleDiscreteColorPickerView(colors, initialColor);
				return colorPickerView.ShowDialog() == true
					? colorPickerView.GetSelectedColor()
					: null;
			}

			using var colorPicker = new ColorPicker
			{
				LockValue_V = false,
				Color = XYZ.FromRGB(initialColor)
			};

			return colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK
				? colorPicker.Color.ToRGB()
				: null;
		}
	}
}
