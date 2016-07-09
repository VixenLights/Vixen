using System;
using System.Drawing;

namespace Common.Controls.Scaling
{
	public static class ScalingTools
	{
		public static double GetScaleFactor()
		{
			double scaleFactor = 1;
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				if (g.DpiY > 96)
				{
					 scaleFactor = g.DpiY / 96d;
				}
			}

			return scaleFactor;
		}
	}
}
