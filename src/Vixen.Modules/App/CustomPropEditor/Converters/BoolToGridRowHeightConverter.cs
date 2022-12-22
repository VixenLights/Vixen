﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VixenModules.App.CustomPropEditor.Converters
{
	[ValueConversion(typeof(bool), typeof(GridLength))]
	public class BoolToGridRowHeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{    // Don't need any convert back
			return null;
		}
	}
}
