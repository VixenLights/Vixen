using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview
{
	public static class GraphicsExtensions
	{
		public static void DrawCircle(this Graphics g, Pen pen, Point center, float radius)
		{
			g.DrawEllipse(pen, (float)center.X - radius, (float)center.Y - radius, radius * 2, radius * 2);
		}

		public static void FillCircle(this Graphics g, Brush brush, Point center, float radius)
		{
			g.FillEllipse(brush, (float)center.X - radius, (float)center.Y - radius, radius * 2, radius - 2);
		}
	}
}
