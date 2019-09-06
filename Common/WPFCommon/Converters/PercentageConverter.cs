using System;
using System.Windows.Data;

namespace Common.WPFCommon.Converters
{
	public class PercentageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (int)(System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
