using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VixenModules.Editor.EffectEditor.Converters
{
	class WidthToVisibilityConverter : IMultiValueConverter
	{

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double? containerWidth = values[0] as double?;
			if (containerWidth.HasValue && containerWidth.Value < System.Convert.ToDouble(parameter))
			{
				return Visibility.Collapsed;
			}

			return Visibility.Visible;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
