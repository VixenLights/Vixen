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
	}
}
