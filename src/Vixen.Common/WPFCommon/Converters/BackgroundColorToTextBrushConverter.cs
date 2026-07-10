using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.WPFCommon.Converters
{
	public class BackgroundColorToTextBrushConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (null == value)
			{
				return null;
			}
			// For a more sophisticated converter, check also the targetType and react accordingly..
			if (value is Color)
			{
				Color color = (Color)value;
				return IdealMediaTextColor(color);
			}

			if (value is System.Drawing.Color)
			{
				var color = (System.Drawing.Color)value;
				return IdealDrawingTextColor(color);
			}

			Type type = value.GetType();
			throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public SolidColorBrush IdealDrawingTextColor(System.Drawing.Color bg)
		{
			int bgDelta = System.Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) +
			                              (bg.B * 0.114));

			return GetIdealBrush(bgDelta);
		}

		public SolidColorBrush IdealMediaTextColor(Color bg)
		{
			
			int bgDelta = System.Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) +
			                                     (bg.B * 0.114));

			return GetIdealBrush(bgDelta);
		}

		public SolidColorBrush GetIdealBrush(int bgDelta)
		{
			int nThreshold = 105;
			return (255 - bgDelta < nThreshold) ? Brushes.Black : Brushes.White;
		}
	}
}
