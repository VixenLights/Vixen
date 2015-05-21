using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VixenModules.App.ColorGradients;

namespace System.Windows.Controls.WpfPropertyGrid.Converters
{
	public class ColorGradientToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ColorGradient)
			{
				ColorGradient colorGradient = (ColorGradient) value;
				return BitmapImageConverter.BitmapToMediaImage(colorGradient.GenericImage);
			}

			return
				BitmapImageConverter.BitmapToMediaImage(
					new ColorGradient(Color.DimGray).GenerateColorGradientImage(new Drawing.Size(50, 50), false));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}