using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewTriangle : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _point1 = new PreviewPoint(10, 10);
		[DataMember] private PreviewPoint _point2 = new PreviewPoint(10, 10);
		[DataMember] private PreviewPoint _point3 = new PreviewPoint(10, 10);
		private PreviewPoint _bottomRightPoint; 

		private PreviewPoint p1Start, p2Start, p3Start, pBottomRightStart;

		public override string TypeName => @"Triangle";

		public PreviewTriangle(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_bottomRightPoint = PointToZoomPoint(point1);
			_point1 = new PreviewPoint(_bottomRightPoint);
			_point2 = new PreviewPoint(_bottomRightPoint);
			_point3 = new PreviewPoint(_bottomRightPoint);

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_strings = new List<PreviewBaseShape>();
			_pixels.Clear();

			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				if (children.Count >= 6)
					//
				{
					int increment = children.Count / 3;
					int pixelsLeft = children.Count;

					StringType = StringTypes.Pixel;

					// Just add lines, they will be layed out in Layout()
					for (int i = 0; i < 3; i++)
					{
						PreviewLine line;
						if (pixelsLeft >= increment)
						{
							line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(20, 20), increment, null, ZoomLevel);
						}
						else
						{
							line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(20, 20), pixelsLeft, null, ZoomLevel);
						}
						line.PixelColor = Color.White;
						_strings.Add(line);

						pixelsLeft -= increment;
					}

					int pixelNum = 0;
					foreach (PreviewPixel pixel in Pixels)
					{
						pixel.Node = children[pixelNum];
						pixel.NodeId = children[pixelNum].Id;
						pixelNum++;
					}
				}
			}

			if (_strings.Count == 0)
			{
				// Just add lines, they will be layed out in Layout()
				for (int i = 0; i < 3; i++)
				{
					PreviewLine line;
					line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(20, 20), 10, node, ZoomLevel);
					line.PixelColor = Color.White;
					_strings.Add(line);
				}
			}

			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			_pixels.Clear();
			Layout();
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 1"),
		 DescriptionAttribute("An triangle is defined by a 3 points of a rectangle. This is point 1.")]
		public Point Point1
		{
			get
			{
				Point p = new Point(_point1.X, _point1.Y);
				return p;
			}
			set
			{
				_point1.X = value.X;
				_point1.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 2"),
		 DescriptionAttribute("An triangle is defined by a 3 points of a rectangle. This is point 2.")]
		public Point Point2
		{
			get
			{
				Point p = new Point(_point2.X, _point2.Y);
				return p;
			}
			set
			{
				_point2.X = value.X;
				_point2.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 3"),
		 DescriptionAttribute("An triangle is defined by a 3 points of a rectangle. This is point 3.")]
		public Point Point3
		{
			get
			{
				Point p = new Point(_point3.X, _point3.Y);
				return p;
			}
			set
			{
				_point3.X = value.X;
				_point3.Y = value.Y;
				Layout();
			}
		}

		public override List<PreviewPixel> Pixels
		{
			get
			{
				List<PreviewPixel> pixels = new List<PreviewPixel>();
				if (_strings != null) {
					for (int i = 0; i < 3; i++) {
						foreach (PreviewPixel pixel in _strings[i]._pixels) {
							pixels.Add(pixel);
						}
					}
				}
				return pixels;
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String 1 Light Count"),
		 DescriptionAttribute("Number of pixels or lights in string 1 of the triangle.")]
		public int LightCountString1
		{
			get { return Strings[0].Pixels.Count; }
			set
			{
				(Strings[0] as PreviewLine).PixelCount = value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String 2 Light Count"),
		 DescriptionAttribute("Number of pixels or lights in string 2 of the triangle.")]
		public int LightCountString2
		{
			get { return Strings[1].Pixels.Count; }
			set
			{
				(Strings[1] as PreviewLine).PixelCount = value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String 3 Light Count"),
		 DescriptionAttribute("Number of pixels or lights in string 3 of the triangle.")]
		public int LightCountString3
		{
			get { return Strings[2].Pixels.Count; }
			set
			{
				(Strings[2] as PreviewLine).PixelCount = value;
				Layout();
			}
		}

		public int Width
		{
			get
			{
				int x = _point1.X;
				x = Math.Max(x, _point2.X);
				x = Math.Max(x, _point3.X);
				return x;
			}
		}

		public int Height
		{
			get
			{
				int y = _point1.Y;
				y = Math.Max(y, _point2.Y);
				y = Math.Max(y, _point3.Y);
				return y;
			}
		}

        public override int Top
        {
            get
            {
                return Math.Min(_point1.Y, Math.Min(Point1.Y, Point3.Y));
            }
            set
            {
                if (_point1.Y < _point2.Y)
                {
                    // Point 1 is the smallest
                    int delta = _point1.Y - value;
                    _point1.Y = value;
                    _point2.Y -= delta;
                    _point3.Y -= delta;
                }
                else
                {
                    // Point 2 or 3 is the smallest
                    int delta = _point2.Y - value;
                    _point1.Y -= delta;
                    _point2.Y = value;
                    _point3.Y = value;
                }
                Layout();
            }
        }

        public override int Bottom
        {
            get
            {
                return Math.Max(_point1.Y, Math.Max(_point2.Y, _point3.Y));
			}
        }

        public override int Right
        {
            get
            {
                return Math.Max(_point2.X, Point3.X);
			}
        }

        public override int Left
        {
            get
            {
                return Math.Min(_point2.X, Point3.X);
            }
            set
            {
                if (_point2.X < _point3.X)
                {
                    // Point 2 is the smallest
                    int delta = _point1.X - value;
                    _point1.X -= delta;
                    _point2.X -= value;
                    _point3.X -= delta;
                }
                else
                {
                    // Point 3 is the smallest
                    int delta = _point3.X - value;
                    _point1.X -= delta;
                    _point2.X -= delta;
                    _point3.X = value;
                }
                Layout();
            }
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewTriangle shape = (matchShape as PreviewTriangle);
            PixelSize = shape.PixelSize;
            _point2.X = _point1.X + (shape._point2.X - shape._point1.X);
            _point2.Y = _point1.Y + (shape._point2.Y - shape._point1.Y);
            _point3.X = _point1.X + (shape._point3.X - shape._point1.X);
            _point3.Y = _point1.Y + (shape._point3.Y - shape._point1.Y);
            //_point1.X = _topLeft.X + (shape._bottomRight.X - shape._topLeft.X);
            //_bottomRight.Y = _topLeft.Y + (shape._bottomRight.Y - shape._topLeft.Y);
            Layout();
        }

		public override void Layout()
		{
            //if (_bottomRightPoint != null)
            //{
            if (Strings.Count == 3) { 
				(Strings[0] as PreviewLine).Point1 = Point1;
				(Strings[0] as PreviewLine).Point2 = Point2;
				(Strings[0] as PreviewLine).Layout();

				(Strings[1] as PreviewLine).Point1 = Point2;
				(Strings[1] as PreviewLine).Point2 = Point3;
				(Strings[1] as PreviewLine).Layout();

				(Strings[2] as PreviewLine).Point1 = Point3;
				(Strings[2] as PreviewLine).Point2 = Point1;
				(Strings[2] as PreviewLine).Layout();

				SetPixelZoom();
            }
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null && _selectedPoint == _bottomRightPoint)
			{
				int left = pBottomRightStart.X; // OK
				int right = Convert.ToInt32(left + (changeX / ZoomLevel)); // OK
				int width = right - left;
				int top = pBottomRightStart.Y; // OK
				int bottom = Convert.ToInt32(top + (changeY / ZoomLevel));
				int height = bottom - top;

				_point1.X = left + (width / 2);
				_point1.Y = top;
				_point2.X = right;
				_point2.Y = bottom;
				_point3.X = left;
				_point3.Y = bottom;

				Layout();
			}
			else if (_selectedPoint != null) {
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				Layout();
			}
				// If we get here, we're moving
			else {
				_point1.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_point1.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_point2.X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_point2.Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;
				_point3.X = Convert.ToInt32(p3Start.X * ZoomLevel) + changeX;
				_point3.Y = Convert.ToInt32(p3Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_point1);
				PointToZoomPointRef(_point2);
				PointToZoomPointRef(_point3);

				Layout();
			}
		}

		public override void Select(bool selectDragPoints)
		{
			base.Select(selectDragPoints);
			connectStandardStrings = true;
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_point1);
			points.Add(_point2);
			points.Add(_point3);
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels) {
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize/2), pixel.Y - (SelectPointSize/2), SelectPointSize,
				                            SelectPointSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
					return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(_point1.X, _point1.Y);
				p2Start = new PreviewPoint(_point2.X, _point2.Y);
				p3Start = new PreviewPoint(_point3.X, _point3.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			// The triangle is strange. When first drawn, the default select point should be the bottom
			// right corner. This point will not be avalable again...
			_bottomRightPoint = new PreviewPoint(10, 10);
			_bottomRightPoint.X = Math.Max(_point1.X, _point2.X);
			_bottomRightPoint.X = Math.Max(_bottomRightPoint.X, _point3.X);
			_bottomRightPoint.Y = Math.Max(_point1.Y, _point2.Y);
			_bottomRightPoint.Y = Math.Max(_bottomRightPoint.Y, _point3.Y);
			_bottomRightPoint.PointType = PreviewPoint.PointTypes.None;
			_selectPoints.Add(_bottomRightPoint);
			_selectedPoint = _bottomRightPoint;
			pBottomRightStart = new PreviewPoint(_bottomRightPoint.X, _bottomRightPoint.Y);
		}

		public override void MoveTo(int x, int y)
		{
			Point topLeft = new Point();
			topLeft.X = Math.Min(_point1.X, Math.Min(_point2.X, _point3.X));
			topLeft.Y = Math.Min(_point1.Y, Math.Min(_point2.Y, _point3.Y));

			int deltaX = x - topLeft.X;
			int deltaY = y - topLeft.Y;

			_point1.X += deltaX;
			_point1.Y += deltaY;
			_point2.X += deltaX;
			_point2.Y += deltaY;
			_point3.X += deltaX;
			_point3.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_point1.X = (int) (_point1.X*aspect);
			_point1.Y = (int) (_point1.Y*aspect);
			_point2.X = (int) (_point2.X*aspect);
			_point2.Y = (int) (_point2.Y*aspect);
			_point3.X = (int) (_point3.X*aspect);
			_point3.Y = (int) (_point3.Y*aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_point1.X = p1Start.X;
			_point1.Y = p1Start.Y;
			_point2.X = p2Start.X;
			_point2.Y = p2Start.Y;
			_point3.X = p3Start.X;
			_point3.Y = p3Start.Y;
			Resize(aspect);
		}

		/// <inheritdoc />
		public override object Clone()
		{
			var newEllipse = (PreviewTriangle) MemberwiseClone();
			newEllipse._point1 = _point1.Copy();
			newEllipse._point2 = _point2.Copy();
			newEllipse._point3 = _point3.Copy();
			newEllipse.Pixels = new List<PreviewPixel>();
			foreach (var previewPixel in Pixels)
			{
				newEllipse.Pixels.Add(previewPixel.Clone());
			}
			
			return newEllipse;
		}
	}
}