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
	public class PreviewCane : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _topLeftPoint;
		[DataMember] private PreviewPoint _bottomRightPoint;
		[DataMember] private PreviewPoint _archLeftPoint;
		[DataMember] private int _archPixelCount;
		[DataMember] private int _linePixelCount;

		private bool justPlaced = false;
		private PreviewPoint bottomRightStart, topLeftStart, archStart;

		public override string TypeName => @"Cane";

		public PreviewCane(PreviewPoint point, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			PreviewPoint newPoint = PointToZoomPoint(new PreviewPoint(point.X, point.Y));
			_topLeftPoint = newPoint;
			_bottomRightPoint = new PreviewPoint(newPoint.X, newPoint.Y);
			_archLeftPoint = new PreviewPoint(newPoint.X, newPoint.Y);

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_archPixelCount = 8;
			_linePixelCount = 8;
			_pixels.Clear();
			int lightCount = _archPixelCount + _linePixelCount;

			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				// is this a single node?
				if (children.Count >= 8)
				{
					StringType = StringTypes.Pixel;
					_archPixelCount = children.Count / 2;
					_linePixelCount = children.Count - _archPixelCount;
					if (_archPixelCount + _linePixelCount > children.Count)
					{
						_archPixelCount -= 1;
					}
					else if (_archPixelCount + _linePixelCount < children.Count)
					{
						_linePixelCount -= 1;
					}
					lightCount = children.Count;
					// Just add the pixels, they will get layed out next
					foreach (ElementNode child in children)
					{
						PreviewPixel pixel = AddPixel(10, 10);
						pixel.Node = child;
						pixel.PixelColor = Color.White;
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

		[CategoryAttribute("Settings"),
		 DisplayName("Line Light Count"),
		 DescriptionAttribute("The number of lights in the vertical string of the candy cane.")]
		public int LinePixelCount
		{
			set
			{
				while (_linePixelCount < value) {
					PreviewPixel pixel = AddPixel(10, 10);
					_linePixelCount++;
				}
				while (_linePixelCount > value) {
					_pixels.RemoveAt(0);
					_linePixelCount--;
				}
				Layout();
			}
			get { return _linePixelCount; }
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Arch Light Count"),
		 DescriptionAttribute("The number of lights in the arch of the candy cane.")]
		public int ArchPixelCount
		{
			set
			{
				// Todo: change the number of pixels when this changes
				while (_archPixelCount < value) {
					PreviewPixel pixel = AddPixel(10, 10);
					_archPixelCount++;
				}
				while (_archPixelCount > value) {
					_pixels.RemoveAt(0);
					_archPixelCount--;
				}
				Layout();
			}
			get { return _archPixelCount; }
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("The bottom right point of bounding box of the candy cane.")]
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
		 DescriptionAttribute("The top left point of the bounding box of the candy cane.")]
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

		[CategoryAttribute("Position"),
		 DisplayName("Arch Start"),
		 DescriptionAttribute("The point on the line where the arch starts.")]
		public Point ArchLeft
		{
			get
			{
				Point p = new Point(_archLeftPoint.X, _archLeftPoint.Y);
				return p;
			}
			set
			{
				_archLeftPoint.X = value.X;
				_archLeftPoint.Y = value.Y;
				Layout();
			}
		}

		// For now we don't want this to show. We might delete it later if it is not used
		[Browsable(false)]
		public int MaxAlpha
		{
			get { return _pixels[0].MaxAlpha; }
			set { _pixels[0].MaxAlpha = value; }
		}

        public override int Top
        {
            get
            {
                return Math.Min(_topLeftPoint.Y, _bottomRightPoint.Y);
            }
            set
            {
                int delta = Top - value;
                if (_topLeftPoint.Y == Top)
                {
                    _topLeftPoint.Y = value;
                    _bottomRightPoint.Y -= delta;
                    _archLeftPoint.Y -= delta;
                }
                else
                {
                    _topLeftPoint.Y -= delta;
                    _bottomRightPoint.Y = value;
                    _archLeftPoint.Y -= delta;
                }
                Layout();
            }
        }

        public override int Bottom
        {
            get
            {
                return Math.Max(_topLeftPoint.Y, _bottomRightPoint.Y);
            }
        }

        public override int Right
        {
            get
            {
                return Math.Max(_topLeftPoint.X, _bottomRightPoint.X);
            }
        }

        public override int Left
        {
            get
            {
                return Math.Min(_topLeftPoint.X, _bottomRightPoint.X);
            }
            set
            {
                int delta = Left - value;
                if (_topLeftPoint.X == Left)
                {
                    _topLeftPoint.X = value;
                    _bottomRightPoint.X -= delta;
                    _archLeftPoint.X -= delta;
                }
                else
                {
                    _topLeftPoint.X -= delta;
                    _bottomRightPoint.X = value;
                    _archLeftPoint.X -= delta;
                }
                Layout();
            }
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewCane shape = (matchShape as PreviewCane);
            PixelSize = shape.PixelSize;
            _bottomRightPoint.X = TopLeft.X + (shape.BottomRight.X - shape.TopLeft.X);
            _bottomRightPoint.Y = TopLeft.Y + (shape.BottomRight.Y - shape.TopLeft.Y);
            //_archLeftPoint.X = shape.X;
            _archLeftPoint.Y = _bottomRightPoint.Y - (shape._bottomRightPoint.Y - shape._archLeftPoint.Y);
            Layout();
        }

		public override void Layout()
		{
			if (_bottomRightPoint != null && _archLeftPoint != null)
			{
				double pixelSpacing = (double)(_bottomRightPoint.Y - _archLeftPoint.Y) / (double)_linePixelCount;
				for (int i = 0; i < _linePixelCount; i++)
				{
					PreviewPixel pixel = _pixels[i];
					pixel.X = _topLeftPoint.X;
					pixel.Y = _bottomRightPoint.Y - (int)(i * pixelSpacing);
					;
				}

				int arcWidth = _bottomRightPoint.X - _topLeftPoint.X;
				int arcHeight = _archLeftPoint.Y - _topLeftPoint.Y;
				List<Point> points = PreviewTools.GetArcPoints(arcWidth, arcHeight, _archPixelCount);
				for (int i = 0; i < points.Count; i++)
				{
					PreviewPixel pixel = _pixels[i + _linePixelCount];
					pixel.X = points[i].X + _topLeftPoint.X;
					pixel.Y = points[i].Y + _topLeftPoint.Y;
				}

				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null)
			{
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;

				_archLeftPoint.X = _topLeftPoint.X;

				if (justPlaced) {
					_archLeftPoint.Y = _topLeftPoint.Y + ((_bottomRightPoint.Y - _topLeftPoint.Y)/4);
				}

				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
				//_bottomRightPoint.X = bottomRightStart.X + changeX;
				//_bottomRightPoint.Y = bottomRightStart.Y + changeY;
				//_topLeftPoint.X = topLeftStart.X + changeX;
				//_topLeftPoint.Y = topLeftStart.Y + changeY;
				//_archLeftPoint.X = archStart.X + changeX;
				//_archLeftPoint.Y = archStart.Y + changeY;

				_bottomRightPoint.X = Convert.ToInt32(bottomRightStart.X * ZoomLevel) + changeX;
				_bottomRightPoint.Y = Convert.ToInt32(bottomRightStart.Y * ZoomLevel) + changeY;
				_topLeftPoint.X = Convert.ToInt32(topLeftStart.X * ZoomLevel) + changeX;
				_topLeftPoint.Y = Convert.ToInt32(topLeftStart.Y * ZoomLevel) + changeY;
				_archLeftPoint.X = Convert.ToInt32(archStart.X * ZoomLevel) + changeX;
				_archLeftPoint.Y = Convert.ToInt32(archStart.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_bottomRightPoint);
				PointToZoomPointRef(_topLeftPoint);
				PointToZoomPointRef(_archLeftPoint);

				Layout();
			}
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_bottomRightPoint);
			points.Add(_topLeftPoint);
			points.Add(_archLeftPoint);
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
			justPlaced = false;
			if (point == null) {
				topLeftStart = new PreviewPoint(_topLeftPoint.X, _topLeftPoint.Y);
				bottomRightStart = new PreviewPoint(_bottomRightPoint.X, _bottomRightPoint.Y);
				archStart = new PreviewPoint(_archLeftPoint.X, _archLeftPoint.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRightPoint;
			justPlaced = true;
		}

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - _topLeftPoint.X;
			int deltaY = y - _topLeftPoint.Y;

			_topLeftPoint.X += deltaX;
			_topLeftPoint.Y += deltaY;
			_bottomRightPoint.X += deltaX;
			_bottomRightPoint.Y += deltaY;
			_archLeftPoint.X += deltaX;
			_archLeftPoint.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeftPoint.X = (int) (_topLeftPoint.X*aspect);
			_topLeftPoint.Y = (int) (_topLeftPoint.Y*aspect);
			_bottomRightPoint.X = (int) (_bottomRightPoint.X*aspect);
			_bottomRightPoint.Y = (int) (_bottomRightPoint.Y*aspect);
			_archLeftPoint.X = (int) (_archLeftPoint.X*aspect);
			_archLeftPoint.Y = (int) (_archLeftPoint.Y*aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeftPoint.X = topLeftStart.X;
			_topLeftPoint.Y = topLeftStart.Y;
			_bottomRightPoint.X = bottomRightStart.X;
			_bottomRightPoint.Y = bottomRightStart.Y;
			_archLeftPoint.X = archStart.X;
			_archLeftPoint.Y = archStart.Y;
			Resize(aspect);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newCane = (PreviewCane) MemberwiseClone();

			newCane._topLeftPoint = _topLeftPoint.Copy();
			newCane._bottomRightPoint = _bottomRightPoint.Copy();
			newCane._archLeftPoint = _archLeftPoint.Copy();
			newCane._pixels = new List<PreviewPixel>();

			foreach (PreviewPixel pixel in _pixels) {
				newCane._pixels.Add(pixel.Clone());
			}

			
			return newCane;
		}

		#endregion
	}
}