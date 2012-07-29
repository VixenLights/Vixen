using System.Drawing;
using Vixen.Sys;

namespace Vixen.Data.Value {
	public struct ColorValue : IIntentDataType {
		public ColorValue(Color color) {
			Color = color;
		}

		public Color Color;

		static public Color ConvertToGrayscale(Color color) {
			return _BasicGrayscaleLuma(color);
		}

		static public byte GetGrayscaleLevel(Color color) {
			return ConvertToGrayscale(color).R;
		}

		static private Color _BasicGrayscaleLuma(Color color) {
			byte grayLevel = (byte)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
			return Color.FromArgb(grayLevel, grayLevel, grayLevel);
		}
	}
}
