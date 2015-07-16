using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class ColorGradientToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ColorGradient)
			{
				ColorGradient colorGradient = (ColorGradient) value;
				return BitmapImageConverter.BitmapToMediaImage(colorGradient.GenerateColorGradientImage(new Size(50, 50), false));
			}

			return
				BitmapImageConverter.BitmapToMediaImage(
					new ColorGradient(Color.DimGray).GenerateColorGradientImage(new System.Drawing.Size(50, 50), false));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}