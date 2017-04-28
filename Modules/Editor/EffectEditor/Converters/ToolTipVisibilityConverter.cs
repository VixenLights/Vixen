using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class ToolTipVisibilityConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var text = TypeDescriptor.GetConverter(value.GetType()).ConvertToString(value);
			return string.IsNullOrEmpty(text) ? Visibility.Hidden : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
