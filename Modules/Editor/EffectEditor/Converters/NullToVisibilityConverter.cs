using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class NullToVisibilityConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
