using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewArch : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _bottomRight;

		private PreviewPoint _topRight = new PreviewPoint(10, 10);
		private PreviewPoint _bottomLeft = new PreviewPoint(10, 10);

		private PreviewPoint p1Start, p2Start;

		public override string TypeName => @"Arch";

		public PreviewArch(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			TopLeft = PointToZoomPoint(point1).ToPoint();
			BottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y).ToPoint();

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_pixels.Clear();
			var lightCount = 25;
			
			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				// is this a single node?
				if (children.Count >= 4)
				{
					StringType = StringTypes.Pixel;
					lightCount = children.Count;
					// Just add the pixels, they will get laid out next
					foreach (ElementNode child in children)
					{
						{
							PreviewPixel pixel = AddPixel(10, 10);
							pixel.Node = child;
							pixel.PixelColor = Color.White;
						}
					}
				}
			}

			if (_pixels.Count == 0)
			{
				// Just add the pixels, they will get laid out next
				for (int lightNum = 0; lightNum < lightCount; lightNum++)
				{
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.PixelColor = Color.White;
					if (node != null && node.IsLeaf)
					{
						pixel.Node = node;
					}
				}
			}

			// Lay out the pixels
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		[Browsable(false)]
		public PreviewPoint TopRight
		{
			get
			{
                if (_topRight == null)
                    _topRight = new PreviewPoint();

				_topRight.X = _bottomRight.X;
                _topRight.Y = _topLeft.Y;
				return _topRight;
			}
            //set
            //{
            //    //if (_topRight == null)
            //    //    _topRight = new PreviewPoint(10, 10);
            //    _topRight = value;
            //}
		}

		[Browsable(false)]
        public PreviewPoint BottomLeft
        {
            get
            {
                if (_bottomLeft == null)
                    _bottomLeft = new PreviewPoint();
                _bottomLeft.X = TopLeft.X;
                _bottomLeft.Y = BottomRight.Y;
                return _bottomLeft;
            }
            //set
            //{
            //    //if (_bottomLeft == null)
            //    //    _bottomLeft = new PreviewPoint(10, 10);
            //    _bottomLeft = value;
            //}
        }

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is point 1.")]
		public Point TopLeft
		{
			get
			{
                if (_topLeft == null) _topLeft = new PreviewPoint(0, 0);
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
		 DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is point 2.")]
		public Point BottomRight
		{
			get
			{
                if (_bottomRight == null) _bottomRight = new PreviewPoint(0, 0);
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

		[CategoryAttribute("Size"),
		 DisplayName("Width"),
		 DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is the width of those points.")]
		public int Width
		{
			get
			{
				return Math.Abs(_bottomRight.X -_topLeft.X);
			}
			set
			{
				_bottomRight.X = _topLeft.X + value;
				Layout();
			}
		}

		[CategoryAttribute("Size"),
		 DisplayName("Height"),
		 DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is the height of those points.")]
		public int Height
		{
			get
			{
				return Math.Abs(_bottomRight.Y - _topLeft.Y);
			}
			set
			{
				_topLeft.Y = _bottomRight.Y - value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Count"),
		 DescriptionAttribute("Number of pixels or lights in the arch.")]
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


        public override int Top
        {
            get
            {
                return Math.Min(_topLeft.Y, _bottomRight.Y);
            }
            set
            {
                int delta = Top - value;

                _topLeft.Y -= delta;
                _bottomRight.Y -= delta;
                Layout();
            }
        }

        public override int Bottom
        {
            get
            {
                return Math.Max(_topLeft.Y, _bottomRight.Y);
            }
        }

        public override int Left
        {
            get
            {
                return Math.Min(_topLeft.X, _bottomRight.X);
            }
            set
            {
                int delta = Left - value;
                _topLeft.X -= delta;
                _bottomRight.X -= delta;
                Layout();
            }
        }

        [Browsable(false)]
        public override int Right
        {
            get
            {
                return Math.Max(_topLeft.X, _bottomRight.X);
            }
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewArch shape = (matchShape as PreviewArch);
            Width = shape.Width;
            Height = shape.Height;
            PixelSize = shape.PixelSize;
            Layout();
        }

		public override void Layout()
		{
            if (BottomRight != null && TopLeft != null)
			{
				int width = BottomRight.X - TopLeft.X;
				int height = BottomRight.Y - TopLeft.Y;
				List<Point> points;
				points = PreviewTools.GetArcPoints(width, height, PixelCount);
				int pointNum = 0;
				foreach (PreviewPixel pixel in _pixels)
				{
					pixel.X = points[pointNum].X + TopLeft.X;
					pixel.Y = points[pointNum].Y + TopLeft.Y;
					pointNum++;
				}

				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));

			// See if we're resizing
			if (_selectedPoint != null) {
				if (_selectedPoint == TopRight) {
					_topLeft.Y = point.Y;
					_bottomRight.X = point.X;
				}
				else if (_selectedPoint == BottomLeft) {
					_topLeft.X = point.X;
					_bottomRight.Y = point.Y;
				}
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				Layout();
			}
				// If we get here, we're moving
			else {
				_topLeft.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_topLeft.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_bottomRight.X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_bottomRight.Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_bottomRight);
            }

			TopRight.X = _bottomRight.X;
			TopRight.Y = _topLeft.Y;
			BottomLeft.X = _topLeft.X;
			BottomLeft.Y = _bottomRight.Y;
			Layout();
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_topLeft);
			points.Add(_bottomRight);
			points.Add(TopRight);
			points.Add(BottomLeft);
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
				p1Start = new PreviewPoint(_topLeft.X, _topLeft.Y);
				p2Start = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
		}

		public override object Clone()
		{
			var newArch =(PreviewArch)MemberwiseClone();
			newArch._topLeft = _topLeft.Copy();
			newArch._bottomRight = _bottomRight.Copy();
			newArch.Pixels = new List<PreviewPixel>();
			foreach (var previewPixel in Pixels)
			{
				newArch.Pixels.Add(previewPixel.Clone());
			}

			return newArch;
		}

		public override void MoveTo(int x, int y)
		{
			Point topLeft = new Point();
			topLeft.X = Math.Min(TopLeft.X, BottomRight.X);
			topLeft.Y = Math.Min(TopLeft.Y, BottomRight.Y);

			int deltaX = x - topLeft.X;
			int deltaY = y - topLeft.Y;

            TopLeft = new Point(TopLeft.X + deltaX, TopLeft.Y + deltaY);
            BottomRight = new Point(BottomRight.X + deltaX, BottomRight.Y + deltaY);

            if (TopRight != null)
            {
                TopRight.X = _bottomRight.X;
                TopRight.Y = _topLeft.Y;
                BottomLeft.X = _topLeft.X;
                BottomLeft.Y = _bottomRight.Y;
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
	}
}