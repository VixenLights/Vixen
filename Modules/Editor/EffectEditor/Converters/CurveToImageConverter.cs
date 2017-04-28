using System;
using System.Globalization;
using System.Windows.Data;
using VixenModules.App.Curves;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class CurveToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{

			var width = 25;
			var height = 25;
			var editable = false;
			
			if (parameter != null)
			{
				editable = System.Convert.ToBoolean(parameter);
			}
			if (editable)
			{
				width = 300;
				height = 30;

			}

			if (value is Curve)
			{
				Curve curve = (Curve) value;
				
				return BitmapImageConverter.BitmapToMediaImage(curve.GenerateGenericCurveImage(new System.Drawing.Size(width, height), false, editable, editable));
			}

			return BitmapImageConverter.BitmapToMediaImage(new Curve().GenerateGenericCurveImage(new System.Drawing.Size(width, height), true));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}