using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.WPFCommon.Converters
{
	public class ColorToSolidBrushConverter: IValueConverter
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
				return new SolidColorBrush(color);
			}
			
			if (value is System.Drawing.Color)
			{
				var color = (System.Drawing.Color)value;
				return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
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
