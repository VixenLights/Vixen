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
	public class PreviewEllipse : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _bottomRight;

		private PreviewPoint topRight, bottomLeft;

		private PreviewPoint skewXPoint = new PreviewPoint(10, 10);
		private PreviewPoint skewYPoint = new PreviewPoint(10, 10);

		private PreviewPoint p1Start, p2Start;

		public override string TypeName => @"Ellipse";

		public PreviewEllipse(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			var lightCount = 50;
			_pixels.Clear();
			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				// is this a single node?
				if (children.Count >= 4)
				{
					StringType = StringTypes.Pixel;
					lightCount = children.Count;
					// Just add the pixels, they will get layed out next
					foreach (ElementNode child in children)
					{
						{
							PreviewPixel pixel = AddPixel(10, 10);
							pixel.Node = child;
							pixel.NodeId = child.Id;
							pixel.PixelColor = Color.White;
						}
					}
				}
			}

			AddPixels(node, lightCount);

			// Lay out the pixels
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("An ellipse is defined by a 2 points of a ellipse. This is point 1.")]
		public Point TopLeft
		{
			get
			{
				Point p = new Point(_topLeft.X, _topLeft.Y);
				return p;
			}
			set
			{
				_topLeft.X = value.X;
				_topLeft.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("An ellipse is defined by a 2 points of a ellipse. This is point 2.")]
		public Point BottomRight
		{
			get
			{
				Point p = new Point(_bottomRight.X, _bottomRight.Y);
				return p;
			}
			set
			{
				_bottomRight.X = value.X;
				_bottomRight.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Count"),
		 DescriptionAttribute("Number of pixels or lights in the ellipse.")]
		public int PixelCount
		{
			get { return Pixels.Count; }
			set
			{
				while (Pixels.Count > value) {
					Pixels.RemoveAt(Pixels.Count - 1);
				}
				while (Pixels.Count < value) {
					PreviewPixel pixel = new PreviewPixel(10, 10, 0, PixelSize);
					Pixels.Add(pixel);
				}
				Layout();
			}
		}

		[DataMember]
		int _XYRotation = 0;
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
		public override int Top
		{
            get
            {
                return (Math.Min(_topLeft.Y, _bottomRight.Y));
            }
			set
			{
				int delta = Top - value;
				if (_topLeft.Y == Top)
				{
					_topLeft.Y = value;
					_bottomRight.Y -= delta;
				}
				else
				{
					_topLeft.Y -= delta;
					_bottomRight.Y = value;
				}

				Layout();
			}
		}

		public override int Left
		{
            get
            {
                return (Math.Min(_topLeft.X, _bottomRight.X));
            }
			set
			{
				int delta = Left - value;
				if (_topLeft.X == Left)
				{
					_topLeft.X = value;
					_bottomRight.X -= delta;
				}
				else
				{
					_topLeft.X -= delta;
					_bottomRight.X = value;
				}
				_topLeft.X = value;

				Layout();
			}
		}

        public override int Right
        {
            get
            {
                return (Math.Max(_topLeft.X, _bottomRight.X));
            }
        }

        public override int Bottom
        {
            get
            {
                return (Math.Max(_topLeft.Y, _bottomRight.Y));
			}
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewEllipse shape = (matchShape as PreviewEllipse);
            PixelSize = shape.PixelSize;
            _bottomRight.X = TopLeft.X + (shape.BottomRight.X - shape.TopLeft.X);
            _bottomRight.Y = TopLeft.Y + (shape.BottomRight.Y - shape.TopLeft.Y);
            Layout();
        }

		public override void Layout()
		{
			if (_bottomRight != null && _topLeft != null) {
				int width = _bottomRight.X - _topLeft.X;
				int height = _bottomRight.Y - _topLeft.Y;
				List<Point> points;
				points = PreviewTools.GetEllipsePoints(0, 0, width, height, PixelCount, 360, XYRotation);
				int pointNum = 0;
				foreach (PreviewPixel pixel in _pixels) {
					pixel.X = points[pointNum].X + _topLeft.X;
					pixel.Y = points[pointNum].Y + _topLeft.Y;
					pointNum++;
				}

				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size) {
				if (_selectedPoint == topRight) {
					_topLeft.Y = point.Y;
					_bottomRight.X = point.X;
				}
				else if (_selectedPoint == bottomLeft) {
					_topLeft.X = point.X;
					_bottomRight.Y = point.Y;
				}
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				// If we get here, we're moving
			}
			else {
				_topLeft.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_topLeft.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_bottomRight.X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_bottomRight.Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_bottomRight);
			}

			if (_selectedPoint == _bottomRight &&
					System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
			{
				_bottomRight.Y = _topLeft.Y + (_bottomRight.X - _topLeft.X);
			}
			
			if (topRight != null) {
				topRight.X = _bottomRight.X;
				topRight.Y = _topLeft.Y;
				bottomLeft.X = _topLeft.X;
				bottomLeft.Y = _bottomRight.Y;
			}

			// Layout the standard shape
			Layout();
		}

		public override void SelectDragPoints()
		{
			// Create the size points
			List<PreviewPoint> selectPoints = new List<PreviewPoint>();

			selectPoints.Add(_topLeft);
			selectPoints.Add(_bottomRight);
			topRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
			selectPoints.Add(topRight);
			bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
			selectPoints.Add(bottomLeft);

			// Create the skew points
			List<PreviewPoint> skewPoints = new List<PreviewPoint>();

			int width = topRight.X - _topLeft.X;
			skewXPoint = new PreviewPoint(_topLeft.X + (width/2), topRight.Y);
			skewXPoint.PointType = PreviewPoint.PointTypes.SkewWE;
			skewPoints.Add(skewXPoint);

			int height = bottomLeft.Y - _topLeft.Y;
			skewYPoint = new PreviewPoint(topRight.X, topRight.Y + (height/2));
			skewYPoint.PointType = PreviewPoint.PointTypes.SkewNS;
			skewPoints.Add(skewYPoint);

			// Tell the base shape about the newely created points
			SetSelectPoints(selectPoints, skewPoints);
		}

		public override bool PointInShape(PreviewPoint inPoint)
		{
			PreviewPoint point = PointToZoomPointAdd(inPoint);
			foreach (PreviewPixel pixel in Pixels) {
				int pixelX = Convert.ToInt32(pixel.X * ZoomLevel);
				int pixelY = Convert.ToInt32(pixel.Y * ZoomLevel);
				Rectangle r = new Rectangle(pixelX - (SelectPointSize / 2), pixelY - (SelectPointSize / 2),
											SelectPointSize + PixelSize, SelectPointSize + PixelSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height)
				{
					return true;
				//Rectangle r = new Rectangle(pixel.X - (SelectPointSize/2), pixel.Y - (SelectPointSize/2),
				//							SelectPointSize + PixelSize, SelectPointSize + PixelSize);
				//if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
				//	return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(_topLeft.X, _topLeft.Y);
				p2Start = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
		}

        public override void Select(bool selectDragPoints)
        {
            base.Select(selectDragPoints);
            //connectStandardStrings = true;
        }

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - TopLeft.X;
			int deltaY = y - TopLeft.Y;

			TopLeft = new Point(x, y);
			BottomRight = new Point(BottomRight.X + deltaX, BottomRight.Y + deltaY);

			if (topRight != null) {
				topRight.X = _bottomRight.X;
				topRight.Y = _topLeft.Y;
				bottomLeft.X = _topLeft.X;
				bottomLeft.Y = _bottomRight.Y;
			}

			Layout();
		}

		public override void Resize(double aspect)
		{
			TopLeft = new Point((int) (TopLeft.X*aspect), (int) (TopLeft.Y*aspect));
			BottomRight = new Point((int) (BottomRight.X*aspect), (int) (BottomRight.Y*aspect));
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeft.X = p1Start.X;
			_topLeft.Y = p1Start.Y;
			_bottomRight.X = p2Start.X;
			_bottomRight.Y = p2Start.Y;
			Resize(aspect);
		}

		/// <inheritdoc />
		public override object Clone()
		{
			var newEllipse = (PreviewEllipse) MemberwiseClone();
			newEllipse._topLeft = _topLeft.Copy();
			newEllipse._bottomRight = _bottomRight.Copy();

			newEllipse.Pixels = new List<PreviewPixel>();
			foreach (var previewPixel in Pixels)
			{
				newEllipse.Pixels.Add(previewPixel.Clone());
			}
			
			return newEllipse;
		}
	}
}