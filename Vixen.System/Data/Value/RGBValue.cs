using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct RGBValue : IIntentDataType
	{
		public byte R;
		public byte G;
		public byte B;

		public RGBValue(Color color)
		{
			if (color.A != 0)
			{
				R = color.R;
				G = color.G;
				B = color.B;
			}
			else
			{
				R = G = B = 0;
			}
		}

		/// <summary>
		/// The RGB value as a intensity appplied color with a 100% alpha channel. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor
		{
			get { return Color.FromArgb(R, G, B); }
			set
			{
				R = value.R;
				G = value.G;
				B = value.B;
			}
		}

		/// <summary>
		/// Gets the RGB value as a full brightness color with the intensity value applied to the alpha channel. 
		/// Results in an non opaque color ranging from transparent (0,0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColorWithAlpha
		{
			get
			{
				//return Color.FromArgb((byte) (Intensity * Byte.MaxValue), R, G, B);
				//This is already a brightness compensated color so applying the intensity to the alpha will dim it further.
				//Need to convert it to it's full brightness color and then apply the alpha.
				var hsv = HSV.FromRGB(R / 255d, G / 255d, B / 255d);
				var v = hsv.V;
				hsv.V = 1;
				var c = hsv.ToRGB().ToArgb();
				return Color.FromArgb((int)(255d * v), c.R, c.G, c.B);
			}
		}

		/// <summary>
		/// The Intensity or brightness of this color in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Intensity
		{
			get
			{
				return HSV.VFromRgb(FullColor);
			}
		}


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
