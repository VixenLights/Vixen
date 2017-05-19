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
			var width = 300;
			var height = 30;
			var isDiscrete = false;

			if (parameter != null)
			{
				isDiscrete = System.Convert.ToBoolean(parameter);
			}
			//if (editable)
			//{
			//	width = 300;
			//	height = 30;

			//}

			if (value is ColorGradient)
			{
				ColorGradient colorGradient = (ColorGradient) value;
				return BitmapImageConverter.BitmapToMediaImage(colorGradient.GenerateColorGradientImage(new Size(width, height), isDiscrete));
			}

			return
				BitmapImageConverter.BitmapToMediaImage(
					new ColorGradient(Color.DimGray).GenerateColorGradientImage(new Size(width, height), false));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}