using System.Globalization;
using System.Windows.Data;
using Common.ValueTypes;

namespace System.Windows.Controls.WpfPropertyGrid.Converters
{
	public class LevelValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value)*100;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value)/100;
		}
	}
}