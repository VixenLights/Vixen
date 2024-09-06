using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFCommon.Extensions
{
	public static class Extensions
	{
		public static ImageSource ToImageSource(this Icon icon)
		{
			ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
				icon.Handle,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			return imageSource;
		}

		public static string ToHex(this System.Drawing.Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		public static string ToHex(this System.Windows.Media.Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}
	}
}
