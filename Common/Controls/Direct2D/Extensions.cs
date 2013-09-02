using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using D2D = Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using DWrite = Microsoft.WindowsAPICodePack.DirectX.DirectWrite;


namespace Common.Controls.Direct2D
{
	public static class Extensions
	{
		public static D2D.ColorF ToColorF(this System.Drawing.Color color)
		{
			return new D2D.ColorF(color.ToArgb());
			//return new D2D.ColorF() { Alpha = color.A, Blue = color.B, Green = color.G, Red = color.R };
		}
		public static D2D.Point2F ToPointF(this Point point) {
			return new D2D.Point2F(point.X, point.Y);
		}
	}
}
