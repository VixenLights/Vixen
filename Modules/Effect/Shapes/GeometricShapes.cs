using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Svg;
using Svg.Pathing;

namespace VixenModules.Effect.Shapes
{
	public class GetGeometricShape
	{
		private static readonly int _radius = 100; //Radius set to 100 as the SvgDocument size is set at 200.

		#region Create Shape

		public static SvgCircle CreateSvgCircle(int radius)
		{
			SvgCircle svg = new SvgCircle
			{
				CenterX = _radius,
				CenterY = _radius,
				Radius = radius
			};
			return svg;
		}

		public static SvgEllipse CreateSvgEllipse(int radius, float ratio)
		{
			SvgEllipse svg = new SvgEllipse
			{
				CenterX = _radius,
				CenterY = _radius,
				RadiusY = radius * ratio,
				RadiusX = radius
			};
			return svg;
		}

		public static SvgRectangle CreateSvgRectangle(int size, float ratio)
		{
			int width = size;
			int height = (int)(size * ratio);

			SvgRectangle svg = new SvgRectangle
			{
				X = (_radius - width),
				Y = (_radius - height),
				Width = width * 2,
				Height = height * 2
			};
			return svg;
		}

		public static SvgPolygon CreateSvgTriangle(int radius)
		{
			PointF[] points = CalculateVertices(radius, 3);
			return ConvertFloatSVGPoints(points);
		}

		public static SvgPolygon CreateSvgPolygon(int radius,int nPolygonSides)
		{
			PointF[] points = CalculateVertices(radius, nPolygonSides);
			return ConvertFloatSVGPoints(points);
		}

		public static SvgPolygon CreateSvgMultiStar(int radius, int starPoints, int SkipPoints)
		{
			PointF[] points = MakeStarPoints(radius, starPoints, SkipPoints);
			return ConvertFloatSVGPoints(points);
		}

		public static SvgPolygon CreateSvgNonIntersectingStar(int radius, int starPoints, int insideDistance, bool northStar)
		{
			PointF[] points = NonIntersectingStarPoints(radius, starPoints, insideDistance, northStar);
			return ConvertFloatSVGPoints(points);
		}
		
		public static SvgPolygon CreateSvgCross(float ratio)
		{
			double[] points = {
				70 , 130, 199 - 199 * ratio, 130, 199 - 199 * ratio, 70, 70, 70, 70, 1, 130, 1, 130, 70, 199 * ratio, 70, 199 * ratio, 130, 130, 130, 130, 199, 70, 199, 70, 199, 70, 130
			};
			return ConvertDoubleToSVGPoints(points);
		}

		public static SvgPolygon CreateSvgArrow()
		{
			double[] points = {
				1, 70, 110, 70, 110, 1, 199, 100, 110, 199, 110, 130, 1, 130, 1, 70
			};
			return ConvertDoubleToSVGPoints(points);
		}

		#endregion

		#region Helpers

		public static SvgPolygon ConvertFloatSVGPoints(PointF[] points)
		{
			SvgPolygon svg = new SvgPolygon { Points = new SvgPointCollection() };
			foreach (var point in points)
			{
				svg.Points.Add((int)point.X);
				svg.Points.Add((int)point.Y);
			}
			return svg;
		}

		public static SvgPolygon ConvertDoubleToSVGPoints(double[] points)
		{
			SvgPolygon svg = new SvgPolygon { Points = new SvgPointCollection() };
			foreach (var point in points)
			{
				svg.Points.Add((int)point);
			}
			return svg;
		}

		private static PointF[] CalculateVertices(int radius, int sides)
		{
			List<PointF> points = new List<PointF>();
			float step = 360.0f / sides;
			float angle = 0; //starting angle
			for (double i = 0; i < 360.0; i += step)
			{
				points.Add(DegreesToXY(angle, radius));
				angle += step;
			}
			
			return points.ToArray();
		}

		private static PointF DegreesToXY(float degrees, float radius)
		{
			PointF xy = new PointF();
			double radians = degrees * Math.PI / 180.0;
			xy.X = (float) Math.Cos(radians) * radius + _radius;
			xy.Y = (float) Math.Sin(-radians) * radius + _radius;
			return xy;
		}

		private static PointF[] NonIntersectingStarPoints(int radius, int numPoints, int insideDistance, bool northStar)
		{
			PointF[] pts = new PointF[2 * numPoints];
			double rad2 = radius * ((double)insideDistance / (radius * 2));

			double theta = -Math.PI / 2;
			double dtheta = Math.PI / numPoints;
			var newRadius = radius;
			for (int i = 0; i < 2 * numPoints; i += 2)
			{
				if (northStar)
				{
					switch (i)
					{
						case 2:
						case 6:
						case 10:
						case 14:
							newRadius = (int) (radius * 0.5);
							break;
						case 4:
						case 12:
							newRadius = (int) (radius * 0.75);
							break;
						default:
							newRadius = radius;
							break;
					}
				}
				pts[i] = new PointF((float) (_radius + newRadius * Math.Cos(theta)), (float) (_radius + newRadius * Math.Sin(theta)));
				theta += dtheta;
				pts[i + 1] = new PointF((float)(_radius + rad2 * Math.Cos(theta)), (float)(_radius + rad2 * Math.Sin(theta)));
				theta += dtheta;
			}
			return pts;
		}

		private static PointF[] MakeStarPoints(int radius, int numPoints, int skip)
		{
			Double concaveRadius = CalculateConcaveRadius(numPoints, skip);
			var result = new PointF[2 * numPoints];
			var theta = -Math.PI / 2;
			var dtheta = Math.PI / numPoints;
			for (int i = 0; i < numPoints; i++)
			{
				result[2 * i] = new PointF((float) (_radius + radius * Math.Cos(theta)), (float) (_radius + radius * Math.Sin(theta)));
				theta += dtheta;
				result[2 * i + 1] = new PointF((float) (_radius + radius * Math.Cos(theta) * concaveRadius),
					(float) (_radius + radius * Math.Sin(theta) * concaveRadius));
				theta += dtheta;
			}
			return result;
		}

		private static double CalculateConcaveRadius(int numPoints, int skip)
		{
			if (numPoints < 5) return 0.33f;
			
			double dtheta = 2 * Math.PI / numPoints;
			double theta00 = -Math.PI / 2;
			double theta01 = theta00 + dtheta * skip;
			double theta10 = theta00 + dtheta;
			double theta11 = theta10 - dtheta * skip;

			PointF pt00 = new PointF((float)Math.Cos(theta00),(float)Math.Sin(theta00));
			PointF pt01 = new PointF((float)Math.Cos(theta01), (float)Math.Sin(theta01));
			PointF pt10 = new PointF((float)Math.Cos(theta10), (float)Math.Sin(theta10));
			PointF pt11 = new PointF((float)Math.Cos(theta11), (float)Math.Sin(theta11));

			bool lines_intersect, segments_intersect;
			PointF intersection, close_p1, close_P2;

			FindIntersection(pt00, pt01, pt10, pt11, out lines_intersect, out segments_intersect, out intersection, out close_p1,
				out close_P2);

			return Math.Sqrt(intersection.X * intersection.X + intersection.Y * intersection.Y);
		}

		private static void FindIntersection(PointF p1, PointF p2, PointF p3, PointF p4, out bool linesIntersect, out bool segmentsIntersect, out PointF intersection, out PointF closeP1, out PointF closeP2)
		{
			float dx12 = p2.X - p1.X;
			float dy12 = p2.Y - p1.Y;
			float dx34 = p4.X - p3.X;
			float dy34 = p4.Y - p3.Y;

			float denominator = (dy12 * dx34 - dx12 * dy34);
			float t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

			if (float.IsInfinity(t1))
			{
				linesIntersect = false;
				segmentsIntersect = false;
				intersection = new PointF(float.NaN, float.NaN);
				closeP1 = new PointF(float.NaN, float.NaN);
				closeP2 = new PointF(float.NaN, float.NaN);
				return;
			}
			linesIntersect = true;

			float t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

			intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

			segmentsIntersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));
			if (t1 < 0)
			{
				t1 = 0;
			}
			else if (t1 > 1)
			{
				t1 = 1;
			}

			if (t2 < 0)
			{
				t2 = 0;
			}
			else if (t2 > 1)
			{
				t2 = 1;
			}

			closeP1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
			closeP2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);

		}

		#endregion
	}
}