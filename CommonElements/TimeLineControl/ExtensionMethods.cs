using System;
using System.Drawing;

namespace CommonElements.Timeline
{
	public static class ExtensionMethods
	{
		public static TimeSpan Scale(this TimeSpan ts, double scale)
		{
			return TimeSpan.FromTicks((long)(ts.Ticks * scale));
		}

		public static TimeSpan Scale(this TimeSpan ts, long scale)
		{
			return TimeSpan.FromTicks(ts.Ticks * scale);
		}


		public static Point BottomRight(this Rectangle r)
		{
			return new Point(r.Right, r.Bottom);
		}
	}

	public static class Util
	{
		public static Rectangle RectangleFromPoints(Point topLeft, Point bottomRight)
		{
			return new Rectangle(topLeft.X, topLeft.Y,
				(bottomRight.X - topLeft.X),
				(bottomRight.Y - topLeft.Y));
		}
	}
}
