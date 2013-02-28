using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VixenModules.Preview.DisplayPreview.WPF
{
	[ValueConversion(typeof(BitmapImage), typeof(ImageBrush))]
	public class ImageBrushConverter : IValueConverter
	{
		#region IValueConverter Members
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BitmapImage image = (BitmapImage)value;
			ImageBrush imageBrush = new ImageBrush();
			if (image != null)
			{
				imageBrush.ImageSource = image;
			}
			return imageBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
