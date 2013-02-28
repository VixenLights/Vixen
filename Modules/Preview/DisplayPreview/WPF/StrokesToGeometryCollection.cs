using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Ink;

namespace VixenModules.Preview.DisplayPreview.WPF
{
	[ValueConversion(typeof(StrokeCollection), typeof(GeometryCollection))]
	public class StrokesToGeometryCollection : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			StrokeCollection strokes = value as StrokeCollection;
			GeometryCollection gc = new GeometryCollection();
			foreach (Stroke stroke in strokes)
			{
				Path path = StrokeToPath(stroke);
				gc.Add(path.Data);
			}
			
			return gc;

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		private Path StrokeToPath(Stroke oStroke)
		{
			PathFigure myPathFigure = null;
			LineSegment myLineSegment = null;
			PathFigureCollection myPathFigureCollection = new PathFigureCollection();
			PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

			if (oStroke == null) return null;

			// Number of points.
			int n = oStroke.StylusPoints.Count;
			if (n == 0) return null;

			// Start point is first point from sytluspoints collection (M, item 0).
			myPathFigure = new PathFigure();
			myPathFigure.StartPoint =
				new Point(oStroke.StylusPoints[0].X, oStroke.StylusPoints[0].Y);
			myPathFigureCollection.Add(myPathFigure);

			// Make small line segment L if there is only one point in the Stroke (workaround).
			// Data with only M is not shown.
			if (n == 1)
			{
				myLineSegment = new LineSegment();
				myLineSegment.Point =
					new Point(oStroke.StylusPoints[0].X + 1, oStroke.StylusPoints[0].Y + 1);
				myPathSegmentCollection.Add(myLineSegment);
			}

			// The other points are line segments (L, items 1..n-1).
			for (int i = 1; i < n; i++)
			{
				myLineSegment = new LineSegment();
				myLineSegment.Point = new Point(oStroke.StylusPoints[i].X, oStroke.StylusPoints[i].Y);
				myPathSegmentCollection.Add(myLineSegment);
			}

			myPathFigure.Segments = myPathSegmentCollection;

			PathGeometry myPathGeometry = new PathGeometry();
			myPathGeometry.Figures = myPathFigureCollection;

			Path oPath = new Path();

			// Add the data to the Path.
			oPath.Data = myPathGeometry;                    // <-|

			// Copy Stroke properties to Path.
			// ----------------------------------------
			// Stroke color.
			Color oC = oStroke.DrawingAttributes.Color;     // Stroke color.
			SolidColorBrush oBr = new SolidColorBrush();
			oBr.Color = oC;
			oPath.Stroke = oBr;

			// Stroke thickness.
			double dW = oStroke.DrawingAttributes.Width;    // Width of stylus.
			double dH = oStroke.DrawingAttributes.Height;   // Height of stylus.
			oPath.StrokeThickness = dW;

			// Attribute FitToCurve.
			// FitToCurve has no effect on the points, oStroke.StylusPoints is used. 
			// See also oStroke.GetBezierStylusPoints().
			// See also test with GetAllPoints().

			// Stroke has no Fill property in attributes.
			// oPath.Fill = mySolidColorBrush;
			// ----------------------------------------

			return oPath;
		}
	}
}
