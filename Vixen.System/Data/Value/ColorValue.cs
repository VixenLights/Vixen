using System.Drawing;

namespace Vixen.Data.Value
{
	public class ColorValue : IIntentDataType
	{
		public ColorValue(Color color)
		{
			ARGB = color.ToArgb();
		}
		public int ARGB { get; set; }
		public Color Color { get {return System.Drawing.Color.FromArgb(ARGB); } }

		public static Color ConvertToGrayscale(Color color)
		{
			return _BasicGrayscaleLuma(color);
		}

		public static byte GetGrayscaleLevel(Color color)
		{
			return ConvertToGrayscale(color).R;
		}

		private static Color _BasicGrayscaleLuma(Color color)
		{
			byte grayLevel = (byte) (color.R*0.3 + color.G*0.59 + color.B*0.11);
			return Color.FromArgb(grayLevel, grayLevel, grayLevel);
		}
	}
}