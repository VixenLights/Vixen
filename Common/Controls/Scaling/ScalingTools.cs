using System;
using System.Drawing;

namespace Common.Controls.Scaling
{
	public static class ScalingTools
	{
		private static double _factor = -1;

		public static double GetScaleFactor()
		{
			if (_factor > -1)
			{
				return _factor;
			}
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				_factor = g.DpiY / 96d;
			}

			return _factor;
		}

		public static SizeF MeasureString(Font f, string s)
		{
			SizeF size = new SizeF(0,0);
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				size = g.MeasureString(s, f);
			}

			return size;
		}

		public static double MeasureHeight(Font f, string s)
		{
			return MeasureString(f, s).Height;
		}
	}
}
