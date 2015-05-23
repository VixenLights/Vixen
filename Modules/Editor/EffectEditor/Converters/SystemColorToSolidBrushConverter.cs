using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace VixenModules.Editor.EffectEditor.Converters
{
	[ValueConversion(typeof (Color), typeof (SolidColorBrush))]
	public class SystemColorToSolidBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				Color color = (Color) value;
				return new SolidColorBrush(ColorToColor(color));
			}
			return new SolidColorBrush(ColorToColor(Color.DimGray));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static Color RGBToColor(string rgb)
		{
			//Trim to RRGGBB
			if (rgb.Length > 6)
			{
				rgb = rgb.Substring(rgb.Length - 6);
			}

			if (rgb.Length != 6)
				throw new ArgumentException("Invalid rgb value given");

			int red = 0;
			int green = 0;
			int blue = 0;

			red = System.Convert.ToInt32(rgb.Substring(0, 2), 16);
			green = System.Convert.ToInt32(rgb.Substring(2, 2), 16);
			blue = System.Convert.ToInt32(rgb.Substring(4, 2), 16);


			return Color.FromArgb(red, green, blue);
		}

		public static string ColorToRGB(Color color)
		{
			string red = color.R.ToString("X2");
			string green = color.G.ToString("X2");
			string blue = color.B.ToString("X2");
			return String.Format("{0}{1}{2}", red, green, blue);
		}

		public static System.Windows.Media.Color ColorToColor(Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static Color ColorToColor(System.Windows.Media.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
}