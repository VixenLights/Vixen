using System.Globalization;
using System.Windows.Data;
using Common.ValueTypes;

namespace System.Windows.Controls.WpfPropertyGrid.Converters
{
	public class PercentageValueConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Percentage)
			{
				Percentage p = (Percentage)value;
				return (p.Value * 100);	
			}
			return System.Convert.ToDouble(value)*100;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			float f = System.Convert.ToSingle(value) / 100;
			if (f < 0 || f > 1)
			{
				f = 1;
			}
			return new Percentage(f);	
		}
	}
}
