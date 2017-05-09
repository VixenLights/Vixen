using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class BitmapImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Bitmap)
			{
				return BitmapToMediaImage((Bitmap) value);
			}
			
			throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public static object BitmapToMediaImage(Bitmap bitmap)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				bitmap.Save(stream, ImageFormat.Png);

				stream.Position = 0;
				BitmapImage result = new BitmapImage();
				result.BeginInit();
				// According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
				// Force the bitmap to load right now so we can dispose the stream.
				result.CacheOption = BitmapCacheOption.OnLoad;
				result.StreamSource = stream;
				result.EndInit();
				result.Freeze();
				return result;
			}
		}
	}
}