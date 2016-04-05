using System;
using System.Drawing;

namespace Vixen.Extensions
{
	public static class Extensions
	{
		public static Color Mix(this Color c, Color other)
		{
			return Color.FromArgb(Math.Max(c.R, other.R), Math.Max(c.G, other.G), Math.Max(c.B, other.B));
		}
	}
}
