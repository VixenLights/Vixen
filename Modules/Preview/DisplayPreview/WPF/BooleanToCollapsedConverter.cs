namespace VixenModules.Preview.DisplayPreview.WPF
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	[ValueConversion(typeof (bool), typeof (Visibility))]
	public class BooleanToCollapsedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is bool
			    && (targetType == typeof (Visibility) || typeof (Visibility).IsSubclassOf(targetType))) {
				return (bool) value ? Visibility.Collapsed : Visibility.Visible;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is Visibility
			    && ((Visibility) value == Visibility.Collapsed || (Visibility) value == Visibility.Visible)
			    && targetType == typeof (bool)) {
				return (Visibility) value == Visibility.Collapsed;
			}

			return DependencyProperty.UnsetValue;
		}
	}
}