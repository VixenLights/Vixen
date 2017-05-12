using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class ColorGradientIsLibraryToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ColorGradient cg = value as ColorGradient;
			if (value != null)
			{
				return cg.IsLibraryReference?Visibility.Hidden : Visibility.Visible;
			}

			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
