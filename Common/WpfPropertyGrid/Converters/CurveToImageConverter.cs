using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VixenModules.App.Curves;

namespace System.Windows.Controls.WpfPropertyGrid.Converters
{
	public class CurveToImageConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Curve)
			{
				Curve curve = (Curve) value;
				return BitmapImageConverter.BitmapToMediaImage(curve.GenerateGenericCurveImage(new Drawing.Size(50, 50)));
			}

			return BitmapImageConverter.BitmapToMediaImage(new Curve().GenerateGenericCurveImage(new Drawing.Size(50,50),true));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
