using System;
using System.Drawing;

namespace CommonElements.ColorManagement.ColorModels
{
	/// <summary>
	/// utility functions for colors
	/// </summary>
	public class ColorUtility
	{
		private ColorUtility(){}

		/// <summary>
		/// returns either black or white with the greatest contrast to the original color
		/// </summary>
		public static Color MaxContrastTo(Color value)
		{
			return (value.R+value.G+value.B)>381?Color.Black:Color.White;
		}
		/// <summary>
		/// returns either red or blue with the greatest contrast to the original color
		/// </summary>
		public static Color MaxContrastRBTo(Color value)
		{
			return value.R>128?Color.Blue:Color.Red;
		}
		/// <summary>
		/// returns a grayscale with the same brightness as the original color
		/// </summary>
		public static Color GrayScaleOf(Color value)
		{
			int mid=(value.R+value.G+value.B)/3;
			return Color.FromArgb(value.A, mid, mid, mid);
		}
		/// <summary>
		/// blends two colors together, respecting the alpha channels
		/// </summary>
		public static Color AoverB(Color a, Color b)
		{
			//alphas must bew ranged 0-1
			float alpha_a = (float)a.A / 255f,
				alpha_b = (float)b.A / 255f,
				div=(alpha_a + (1f - alpha_a) * alpha_b),
				FB=(1f - alpha_a) * alpha_b;
			//if divider is zero, return
			if(div==0.0)
				return Color.Transparent;
			//mix using A over B subpixel alpha blending
			return Color.FromArgb(
				(int)(255f - 255f * (1f - alpha_a) * (1f - alpha_b)),
				(int)((alpha_a * (float)a.R + FB * (float)b.R) / div),
				(int)((alpha_a * (float)a.G + FB * (float)b.G) / div),
				(int)((alpha_a * (float)a.B + FB * (float)b.B) / div));
		}
		/// <summary>
		/// blends two colors over, respection the position (0-1).
		/// position=0 means: color a is returned
		/// position=1 means: color b is returned
		/// </summary>
		public static Color BlendOver(Color a, Color b, float position)
		{
			//normalize position
			position=Math.Max(0f,Math.Min(1f,position));

			return Color.FromArgb(
				a.A+(int)(position*(float)(b.A-a.A)),
				a.R+(int)(position*(float)(b.R-a.R)),
				a.G+(int)(position*(float)(b.G-a.G)),
				a.B+(int)(position*(float)(b.B-a.B)));
		}
	}
}
