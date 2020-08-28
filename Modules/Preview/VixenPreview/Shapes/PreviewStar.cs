using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewStar : PreviewBaseShape
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		[DataMember] private PreviewPoint _topLeftPoint;
		[DataMember] private PreviewPoint _bottomRightPoint;
		[DataMember] private int _pointCount;
		[DataMember] private int _pixelCount;
		[DataMember] private int _insideSize;
		[DataMember] private StringDirections _stringDirection;

		private int pixelsPerPoint;

		private PreviewPoint bottomRightStart, topLeftStart;

		public override string TypeName => @"Star";

		public PreviewStar(PreviewPoint point, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeftPoint = PointToZoomPoint(point);
			_bottomRightPoint = PointToZoomPoint(point);

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_pointCount = 0;
			_pixelCount = 0;
			_insideSize = 40;
			_pixels.Clear();
			if (node != null)
			{

				AddAllChildren(node);
				if (IsPixelStar(node))
					StringType = StringTypes.Pixel;
			}

			if (_pixels.Count >= 5 && _pointCount == 0)
			{
				ConfigurePointCount(_pixels.Count);
			}

			if (PointCount == 0)
			{
				ConfigurePointCount(_pixels.Count + 1);
			}

			if (_pixels.Count < 5)
			{
				_pixelCount = 40;
				_pointCount = 5;
				// Just add the pixels, they will get layed out next
				for (int lightNum = 0; lightNum < _pixelCount; lightNum++)
				{
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.PixelColor = Color.White;
					if (node != null && node.IsLeaf)
					{
						pixel.Node = node;
					}
				}
			}

			//Console.WriteLine("Star Pixel Count: " + _pixelCount + ":" + _pixels.Count());

			// Lay out the pixels
			Layout();
		}

		#endregion

		private void ConfigurePointCount(int lightCount)
		{
			if (lightCount % 5 == 0)
			{
				_pointCount = 5;
			}
			else if (lightCount % 3 == 0)
			{
				_pointCount = 3;
			}
			else if (lightCount % 2 == 0)
			{
				_pointCount = 4;
			}
		}

		private bool IsPixelStar(ElementNode selectedNode)
		{
			return !selectedNode.IsLeaf && selectedNode.GetLeafEnumerator().Count() >= 2;
		}

		private void AddAllChildren(ElementNode selectedNode)
		{
			foreach (ElementNode child in selectedNode.Children)
			{
				if (!child.IsLeaf && child.Children.Count() > 0)
				{
					AddAllChildren(child);
					_pointCount++;
				}
				else
				{
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.Node = child;
					pixel.PixelColor = Color.White;
				}
			}
			_pixelCount = _pixels.Count();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		public override int Bottom
		{
			get
			{
				return (Math.Max(_topLeftPoint.Y, _bottomRightPoint.Y));
			}
		}

		public override int Top
		{
			get
			{
				return (Math.Min(_topLeftPoint.Y, _bottomRightPoint.Y));
			}
			set
			{
				int delta = Top - value;
				if (_topLeftPoint.Y == Top)
				{
					_topLeftPoint.Y = value;
					_bottomRightPoint.Y -= delta;
				}
				else
				{
					_topLeftPoint.Y -= delta;
					_bottomRightPoint.Y = value;
				}
				Layout();
			}
		}

		public override int Right
		{
			get
			{
				return (Math.Max(_topLeftPoint.X, _bottomRightPoint.X));
			}
		}

		public override int Left
		{
			get
			{
				return (Math.Min(_topLeftPoint.X, _bottomRightPoint.X));
			}
			set
			{
				int delta = Left - value;
				if (_topLeftPoint.X == Left)
				{
					_topLeftPoint.X = value;
					_bottomRightPoint.X -= delta;
				}
				else
				{
					_topLeftPoint.X -= delta;
					_bottomRightPoint.X = value;
				}
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String Direction"),
		 DescriptionAttribute("Do the lights rotate around the star clockwise or counter-clockwise?")]
		public StringDirections StringDirection
		{
			get
			{
				return _stringDirection;
			}
			set
			{
				_stringDirection = value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Count"),
		 DescriptionAttribute("The number of lights in the star.")]
		public int PixelCount
		{
			set
			{
				while (_pixelCount < value)
				{
					PreviewPixel pixel = AddPixel(10, 10);
					_pixelCount++;
				}
				while (_pixelCount > value)
				{
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

		[DataMember]
		int _XYRotation = 18;
		[CategoryAttribute("Settings"),
		DescriptionAttribute("The prop can be rotated about the Z axis in the XY plane. This is the rotation angle."),
		DisplayName("XY Rotation")]
		public int XYRotation
		{
			get
			{
				return _XYRotation;
			}
			set
			{
				_XYRotation = value;
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
			get { return Math.Abs(_bottomRightPoint.X - _topLeftPoint.X); }
		}

		private int Height
		{
			get { return Math.Abs(_bottomRightPoint.Y - _topLeftPoint.Y); }
		}

		public override void Select(bool selectDragPoints)
		{
			base.Select(selectDragPoints);
			connectStandardStrings = true;
		}

		private void RecalcPoints()
		{
			pixelsPerPoint = PixelCount / _pointCount;
			while (pixelsPerPoint % 2 != 0)
			{
				pixelsPerPoint--;
			}

			if (pixelsPerPoint < 2)
			{
				pixelsPerPoint = 2;
			}

			int newPixelCount = pixelsPerPoint * _pointCount;
			while (_pixelCount > newPixelCount)
			{
				_pixels.RemoveAt(_pixels.Count - 1);
				_pixelCount--;
			}
			while (_pixelCount < newPixelCount)
			{
				AddPixel(10, 10);
				_pixelCount++;
			}
		}

		public override void Match(PreviewBaseShape matchShape)
		{
			PreviewStar shape = (matchShape as PreviewStar);
			PixelSize = shape.PixelSize;
			PointCount = shape.PointCount;
			InsideSize = shape.InsideSize;
			XYRotation = shape.XYRotation;
			_bottomRightPoint.X = _topLeftPoint.X + (shape._bottomRightPoint.X - shape._topLeftPoint.X);
			_bottomRightPoint.Y = _topLeftPoint.Y + (shape._bottomRightPoint.Y - shape._topLeftPoint.Y);
			Layout();
		}

		public override void Layout()
		{
			if (_topLeftPoint != null && _bottomRightPoint != null)
			{
				if (Width > 10 && Height > 10)
				{
					RecalcPoints();
					int outerWidth = _bottomRightPoint.X - _topLeftPoint.X;
					int outerHeight = _bottomRightPoint.Y - _topLeftPoint.Y;
					int degreeOffset = 360 / _pointCount / 2;
					List<Point> outerEllipse = PreviewTools.GetEllipsePoints(_topLeftPoint.X,
																			 _topLeftPoint.Y,
																			 outerWidth,
																			 outerHeight,
																			 _pointCount,
																			 360,
																			 degreeOffset + XYRotation);

					int innerWidth = (int)(outerWidth * _insideSize * .01);
					if (innerWidth < 4) innerWidth = 4;
					int innerHeight = (int)(outerHeight * _insideSize * .01);
					if (innerHeight < 4) innerHeight = 4;

					int widthOffset = ((outerWidth - innerWidth) / 2);
					int heightOffset = ((outerHeight - innerHeight) / 2);
					int innerLeft = _topLeftPoint.X + widthOffset;
					int innerTop = _topLeftPoint.Y + heightOffset;

					int rot = XYRotation;
					//if (StringDirection == StringDirections.CounterClockwise)
					//    rot = -rot;
					List<Point> innerEllipse = PreviewTools.GetEllipsePoints(innerLeft,
																			 innerTop,
																			 innerWidth,
																			 innerHeight,
																			 _pointCount,
																			 360,
																			 rot);

					int pixelNum = 0;
					for (int i = 0; i < _pointCount; i++)
					{
						Point point1;
						Point point2;
						Point point3;
						var ellipsePointNum = 0;
						if (StringDirection == StringDirections.Clockwise)
						{
							ellipsePointNum = i;
							point1 = innerEllipse[ellipsePointNum];
							point2 = outerEllipse[ellipsePointNum];
							if (ellipsePointNum < _pointCount - 1)
							{
								point3 = innerEllipse[ellipsePointNum + 1];
							}
							else
							{
								point3 = innerEllipse[0];
							}
						}
						else
						{
							ellipsePointNum = (_pointCount) - i;
							point1 = innerEllipse[ellipsePointNum];
							if (ellipsePointNum > 0)
							{
								point2 = outerEllipse[ellipsePointNum - 1];
								point3 = innerEllipse[ellipsePointNum - 1];
							}
							else
							{
								point2 = outerEllipse[_pointCount - 1];
								point3 = innerEllipse[_pointCount - 1];
							}
						}

						var line1PixelCount = pixelsPerPoint / 2d;
						var line2PixelCount = line1PixelCount;
						if (line1PixelCount + line2PixelCount < pixelsPerPoint)
						{
							line1PixelCount++;
						}
						double xSpacing = (point1.X - point2.X) / line1PixelCount;
						double ySpacing = (point1.Y - point2.Y) / line1PixelCount;
						double x = point1.X;
						double y = point1.Y;
						for (int linePointNum = 0; linePointNum < line1PixelCount; linePointNum++)
						{
							if (pixelNum < _pixelCount && pixelNum < _pixels.Count)
							{
								_pixels[pixelNum].X = (int)Math.Round(x);
								_pixels[pixelNum].Y = (int)Math.Round(y);
								x -= xSpacing;
								y -= ySpacing;
							}
							else
							{
								Logging.Error("pixelNum Overrun 1: " + pixelNum);
							}
							pixelNum++;
						}

						xSpacing = (point2.X - point3.X) / line2PixelCount;
						ySpacing = (point2.Y - point3.Y) / line2PixelCount;
						x = point2.X;
						y = point2.Y;
						for (int linePointNum = 0; linePointNum < line2PixelCount; linePointNum++)
						{
							if (pixelNum < _pixelCount && pixelNum < _pixels.Count)
							{
								_pixels[pixelNum].X = (int)Math.Round(x);
								_pixels[pixelNum].Y = (int)Math.Round(y);
								x -= xSpacing;
								y -= ySpacing;
							}
							else
							{
								Logging.Error("pixelNum Overrun 2: " + pixelNum);
							}
							pixelNum++;
						}
					}
					SetPixelZoom();
				}
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null)
			{
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;

				if (_selectedPoint == _bottomRightPoint &&
					System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
				{
					_bottomRightPoint.Y = _topLeftPoint.Y + (_bottomRightPoint.X - _topLeftPoint.X);
				}

				Layout();
				SelectDragPoints();
			}
			// If we get here, we're moving
			else
			{
				_bottomRightPoint.X = (int)(bottomRightStart.X * ZoomLevel + changeX);
				_bottomRightPoint.Y = (int)(bottomRightStart.Y * ZoomLevel + changeY);
				_topLeftPoint.X = (int)(topLeftStart.X * ZoomLevel + changeX);
				_topLeftPoint.Y = (int)(topLeftStart.Y * ZoomLevel + changeY);
				PointToZoomPointRef(_topLeftPoint);
				PointToZoomPointRef(_bottomRightPoint);

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
			foreach (PreviewPixel pixel in Pixels)
			{
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2),
											SelectPointSize + PixelSize, SelectPointSize + PixelSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height)
				{
					return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null)
			{
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
			_topLeftPoint.X = (int)(_topLeftPoint.X * aspect);
			_topLeftPoint.Y = (int)(_topLeftPoint.Y * aspect);
			_bottomRightPoint.X = (int)(_bottomRightPoint.X * aspect);
			_bottomRightPoint.Y = (int)(_bottomRightPoint.Y * aspect);

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

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newStar = (PreviewStar) MemberwiseClone();
			newStar._topLeftPoint = _topLeftPoint.Copy();
			newStar._bottomRightPoint = _bottomRightPoint.Copy();
			newStar.Pixels = new List<PreviewPixel>();
			foreach (var previewPixel in Pixels)
			{
				newStar.Pixels.Add(previewPixel.Clone());
			}

			return newStar;
		}

		#endregion
	}
}