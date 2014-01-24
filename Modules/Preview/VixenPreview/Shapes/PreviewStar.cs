using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewStar : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _topLeftPoint;
		[DataMember] private PreviewPoint _bottomRightPoint;
		[DataMember] private int _pointCount;
		[DataMember] private int _pixelCount;
		[DataMember] private int _insideSize;

		private int pixelsPerPoint;
		private int lineCount;

		private PreviewPoint bottomRightStart, topLeftStart;

		public PreviewStar(PreviewPoint point, ElementNode selectedNode)
		{
			_topLeftPoint = point;
			_bottomRightPoint = new PreviewPoint(point.X, point.Y);

			_pixelCount = 40;
			_pointCount = 5;
			_insideSize = 40;

			if (selectedNode != null) {
				List<ElementNode> children = PreviewTools.GetLeafNodes(selectedNode);
				// is this a single node?
				if (children.Count >= 10) {
					StringType = StringTypes.Pixel;
					// Just add the pixels, they will get layed out next
					foreach (ElementNode child in children) {
						PreviewPixel pixel = AddPixel(10, 10);
						pixel.Node = child;
						pixel.PixelColor = Color.White;
					}
					_pixelCount = children.Count;
				}
			}

			if (_pixels.Count == 0) {
				// Just add the pixels, they will get layed out next
				for (int lightNum = 0; lightNum < _pixelCount; lightNum++) {
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.PixelColor = Color.White;
					if (selectedNode != null && selectedNode.IsLeaf) {
						pixel.Node = selectedNode;
					}
				}
			}

			// Lay out the pixels
			Layout();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}


		public override int Top
		{
			get { return _topLeftPoint.Y; }
		}

		public override int Left
		{
			get { return _topLeftPoint.X; }
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Count"),
		 DescriptionAttribute("The number of lights in the star.")]
		public int PixelCount
		{
			set
			{
				while (_pixelCount < value) {
					PreviewPixel pixel = AddPixel(10, 10);
					_pixelCount++;
				}
				while (_pixelCount > value) {
					_pixels.RemoveAt(_pixels.Count - 1);
					_pixelCount--;
				}
				Layout();
			}
			get { return _pixelCount; }
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("The bottom right point of bounding box of the star.")]
		public Point BottomRight
		{
			get
			{
				Point p = new Point(_bottomRightPoint.X, _bottomRightPoint.Y);
				return p;
			}
			set
			{
				_bottomRightPoint.X = value.X;
				_bottomRightPoint.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("The top left point of the bounding box of the star.")]
		public Point TopLeft
		{
			get
			{
				Point p = new Point(_topLeftPoint.X, _topLeftPoint.Y);
				return p;
			}
			set
			{
				_topLeftPoint.X = value.X;
				_topLeftPoint.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Inside Size"),
		 DescriptionAttribute("The size, in percent of the inside star diameter in relation to the outside diameter.")]
		public int InsideSize
		{
			get { return _insideSize; }
			set
			{
				_insideSize = value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Star Points"),
		 DescriptionAttribute("The number of points on the star.")]
		public int PointCount
		{
			get { return _pointCount; }
			set
			{
				_pointCount = value;
				Layout();
			}
		}

		private int Width
		{
			get { return Math.Abs(_bottomRightPoint.X - _topLeftPoint.Y); }
		}

		private int Height
		{
			get { return Math.Abs(_bottomRightPoint.Y - _topLeftPoint.Y); }
		}

		private void RecalcPoints()
		{
			lineCount = _pointCount*2;
			pixelsPerPoint = PixelCount/_pointCount;
			while (
				((pixelsPerPoint%2) != 0)
				) {
				pixelsPerPoint--;
			}
			if (pixelsPerPoint < 2)
				pixelsPerPoint = 2;

			int newPixelCount = pixelsPerPoint*_pointCount;
			while (_pixelCount > newPixelCount) {
				_pixels.RemoveAt(_pixels.Count - 1);
				_pixelCount--;
			}
			while (_pixelCount < newPixelCount) {
				PreviewPixel pixel = AddPixel(10, 10);
				_pixelCount++;
			}
		}

		public override void Layout()
		{
			if (Width > 10 && Height > 10) {
				RecalcPoints();
				int outerWidth = _bottomRightPoint.X - _topLeftPoint.X;
				int outerHeight = _bottomRightPoint.Y - _topLeftPoint.Y;
				List<Point> outerEllipse = PreviewTools.GetEllipsePoints(_topLeftPoint.X,
				                                                         _topLeftPoint.Y,
				                                                         outerWidth,
				                                                         outerHeight,
				                                                         _pointCount,
				                                                         360,
				                                                         0);

				int innerWidth = (int) (outerWidth*_insideSize*.01);
				if (innerWidth < 4) innerWidth = 4;
				int innerHeight = (int) (outerHeight*_insideSize*.01);
				if (innerHeight < 4) innerHeight = 4;

				int degreeOffset = 360/_pointCount/2;
				int widthOffset = ((outerWidth - innerWidth)/2);
				int heightOffset = ((outerHeight - innerHeight)/2);
				int innerLeft = _topLeftPoint.X + widthOffset;
				int innerTop = _topLeftPoint.Y + heightOffset;
				List<Point> innerEllipse = PreviewTools.GetEllipsePoints(innerLeft,
				                                                         innerTop,
				                                                         innerWidth,
				                                                         innerHeight,
				                                                         _pointCount,
				                                                         360,
				                                                         degreeOffset);


				int pixelNum = 0;
				for (int ellipsePointNum = 0; ellipsePointNum < _pointCount; ellipsePointNum++) {
					Point point1 = outerEllipse[ellipsePointNum];
					Point point2 = innerEllipse[ellipsePointNum];
					Point point3;
					if (ellipsePointNum < _pointCount - 1) {
						point3 = outerEllipse[ellipsePointNum + 1];
					}
					else {
						point3 = outerEllipse[0];
					}
					int line1PixelCount = (int) (pixelsPerPoint/2);
					int line2PixelCount = line1PixelCount - 1;
					if (line1PixelCount + line2PixelCount < pixelsPerPoint) {
						line1PixelCount++;
					}
					double xSpacing = (double) (point1.X - point2.X)/(double) (line1PixelCount - 1);
					double ySpacing = (double) (point1.Y - point2.Y)/(double) (line1PixelCount - 1);
					double x = point1.X;
					double y = point1.Y;
					for (int linePointNum = 0; linePointNum < line1PixelCount; linePointNum++) {
						if (pixelNum < _pixelCount) {
							_pixels[pixelNum].X = (int) Math.Round(x);
							_pixels[pixelNum].Y = (int) Math.Round(y);
							x -= xSpacing;
							y -= ySpacing;
						}
						else {
							Console.WriteLine("pixelNum Overrun 1: " + pixelNum);
						}
						pixelNum++;
					}

					xSpacing = (double) (point2.X - point3.X)/(double) (line1PixelCount - 1);
					ySpacing = (double) (point2.Y - point3.Y)/(double) (line1PixelCount - 1);
					x = point2.X - xSpacing;
					y = point2.Y - ySpacing;
					for (int linePointNum = 0; linePointNum < line2PixelCount; linePointNum++) {
						if (pixelNum < _pixelCount) {
							_pixels[pixelNum].X = (int) Math.Round(x);
							_pixels[pixelNum].Y = (int) Math.Round(y);
							x -= xSpacing;
							y -= ySpacing;
						}
						else {
							Console.WriteLine("pixelNum Overrun 2: " + pixelNum);
						}
						pixelNum++;
					}
				}
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			if (_selectedPoint != null) {
				_selectedPoint.X = x;
				_selectedPoint.Y = y;

				if (_selectedPoint == _bottomRightPoint &&
				    System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control) {
					_bottomRightPoint.Y = _topLeftPoint.Y + (_bottomRightPoint.X - _topLeftPoint.X);
				}

				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
				_bottomRightPoint.X = bottomRightStart.X + changeX;
				_bottomRightPoint.Y = bottomRightStart.Y + changeY;
				_topLeftPoint.X = topLeftStart.X + changeX;
				_topLeftPoint.Y = topLeftStart.Y + changeY;
				Layout();
			}
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_bottomRightPoint);
			points.Add(_topLeftPoint);
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels) {
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize/2), pixel.Y - (SelectPointSize/2),
				                            SelectPointSize + PixelSize, SelectPointSize + PixelSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
					return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				topLeftStart = new PreviewPoint(_topLeftPoint.X, _topLeftPoint.Y);
				bottomRightStart = new PreviewPoint(_bottomRightPoint.X, _bottomRightPoint.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRightPoint;
		}

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - _topLeftPoint.X;
			int deltaY = y - _topLeftPoint.Y;

			_topLeftPoint.X += deltaX;
			_topLeftPoint.Y += deltaY;
			_bottomRightPoint.X += deltaX;
			_bottomRightPoint.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeftPoint.X = (int) (_topLeftPoint.X*aspect);
			_topLeftPoint.Y = (int) (_topLeftPoint.Y*aspect);
			_bottomRightPoint.X = (int) (_bottomRightPoint.X*aspect);
			_bottomRightPoint.Y = (int) (_bottomRightPoint.Y*aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeftPoint.X = topLeftStart.X;
			_topLeftPoint.Y = topLeftStart.Y;
			_bottomRightPoint.X = bottomRightStart.X;
			_bottomRightPoint.Y = bottomRightStart.Y;
			Resize(aspect);
		}
	}
}