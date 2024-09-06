using System.Windows.Data;
using System.Windows.Media;
using WPFCommon.Extensions;

namespace Common.WPFCommon.Converters
{
	public class ColorToHexStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (null == value)
			{
				return null;
			}
			// For a more sophisticated converter, check also the targetType and react accordingly..
			if (value is Color)
			{
				Color color = (Color)value;
				return color.ToHex();
			}

			if (value is System.Drawing.Color)
			{
				var color = (System.Drawing.Color)value;
				return color.ToHex();
			}

			Type type = value.GetType();
			throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
