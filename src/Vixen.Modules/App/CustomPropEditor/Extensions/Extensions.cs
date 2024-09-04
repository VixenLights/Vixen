using System.Drawing;
using System.Text.RegularExpressions;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Extensions
{
	public static class Extensions
	{
		public static System.Windows.Media.SolidColorBrush ToBrush(this Color color)
		{
			return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		public static string ToHex(this Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		public static Color ToColor(this System.Windows.Media.SolidColorBrush brush)
		{
			return brush.Color.ToDrawingColor();
		}

		public static Color ToDrawingColor(this System.Windows.Media.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static bool IsNumeric(this string value)
		{
			return Regex.IsMatch(value, "^\\d*$");
		}
	}
}
