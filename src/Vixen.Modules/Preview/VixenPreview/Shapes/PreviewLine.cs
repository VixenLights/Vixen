using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows;
using Vixen.Sys;
using Point = System.Drawing.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewLine : PreviewLightBaseShape
	{
		[DataMember] private List<PreviewPoint> _points = new List<PreviewPoint>();

		private PreviewPoint p1Start, p2Start;
		private int _lightCount = 50;

		public override string TypeName => @"Line";

		public PreviewLine(PreviewPoint point1, PreviewPoint point2, int lightCount, ElementNode selectedNode, double zoomLevel)
		{
			AddStartPadding = false;
			AddEndPadding = false;
			ZoomLevel = zoomLevel;
			AddPoint(PointToZoomPoint(point1));
			AddPoint(PointToZoomPoint(point2));

			_lightCount = lightCount;

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_pixels.Clear();
			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				// is this a single node?
				if (children.Count == 0)
				{
					StringType = StringTypes.Standard;
					// Just add the pixels, they will get layed out next
					for (int lightNum = 0; lightNum < _lightCount; lightNum++)
					{
						PreviewPixel pixel = AddPixel(10, 10);
						pixel.PixelColor = Color.White;
						if (node.IsLeaf)
							pixel.Node = node;
					}
				}
				else
				{
					StringType = StringTypes.Pixel;
					// Just add the pixels, they will get layed out next
					foreach (ElementNode child in children)
					{
						PreviewPixel pixel = AddPixel(10, 10);
						pixel.Node = child;
						pixel.NodeId = child.Id;
						pixel.PixelColor = Color.White;
					}
				}
			}
			else
			{
				// Just add the pixels, they will get layed out next
				for (int lightNum = 0; lightNum < _lightCount; lightNum++)
				{
					//Console.WriteLine("Added: " + lightNum.ToString());
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.PixelColor = Color.White;
				}
			}
			// Lay out the pixels
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			_points[0].PointType = PreviewPoint.PointTypes.SizeTopLeft;
			_points[1].PointType = PreviewPoint.PointTypes.SizeBottomRight;
			Layout();
		}

		public void SetPoint0(int X, int Y)
		{
			_points[0].X = X;
			_points[0].Y = Y;
		}

		public void SetPoint1(int X, int Y)
		{
			_points[1].X = X;
			_points[1].Y = Y;
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 1"),
		 DescriptionAttribute("Lines are defined by 2 points. This is point 1.")]
		public Point Point1
		{
			get
			{
				Point p = new Point(_points[0].X, _points[0].Y);
				_points[0].PointType = PreviewPoint.PointTypes.SizeTopLeft;
				return p;
			}
			set
			{
				_points[0].X = value.X;
				_points[0].Y = value.Y;
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 2"),
		 DescriptionAttribute("Lines are defined by 2 points. This is point 2.")]
		public Point Point2
		{
			get
			{
				Point p = new Point(_points[1].X, _points[1].Y);
				_points[1].PointType = PreviewPoint.PointTypes.SizeBottomRight;
				return p;
			}
			set
			{
				_points[1].X = value.X;
				_points[1].Y = value.Y;
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		public void AddPoint(PreviewPoint point)
		{
			_points.Add(point);
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Count"),
		 DescriptionAttribute("Number of pixels or lights in the string.")]
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

        public override int Bottom
        {
            get
            {
                return Math.Max(_points[0].Y, _points[1].Y);
			}
        }

        public override int Top
        {
            get
            {
                return (Math.Min(_points[0].Y, _points[1].Y));
            }
	        set
	        {
				if (_points[0].Y < _points[1].Y)
				{
					int delta = _points[0].Y - value;
					_points[0].Y = value;
					_points[1].Y -= delta;
				}
				else
				{
					int delta = _points[1].Y - value;
					_points[0].Y -= delta;
					_points[1].Y = value;
				}
				Layout();
	        }
        }

        public override int Right
        {
            get
            {
                return Math.Max(_points[0].X, _points[1].X);
			}
        }

        public override int Left
        {
            get
            {
                return (Math.Min(_points[0].X, _points[1].X));
            }
	        set
	        {
				if (_points[0].X < _points[1].X)
				{
					int delta = _points[0].X - value;
					_points[0].X = value;
					_points[1].X -= delta;
				}
				else
				{
					int delta = _points[1].X - value;
					_points[0].X -= delta;
					_points[1].X = value;
				}
				Layout();
	        }
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewLine shape = (matchShape as PreviewLine);
            PixelSize = shape.PixelSize;
            _points[1].X = _points[0].X + (shape._points[1].X - shape._points[0].X);
            _points[1].Y = _points[0].Y + (shape._points[1].Y - shape._points[0].Y);
			base.Match(shape);
            Layout();
        }

		public override void Layout()
		{
			if (_points != null && _points.Count > 0)
			{
			    var count = AddStartPadding ? PixelCount : PixelCount - 1;
				count = AddEndPadding ? count + 1 : count;
			    double xSpacing = (double)(_points[0].X - _points[1].X) / count;
				double ySpacing = (double)(_points[0].Y - _points[1].Y) / count;
				double x = _points[0].X;
				double y = _points[0].Y;
			    if (AddStartPadding)
			    {
			        x -= xSpacing;
			        y -= ySpacing;
			    }
				foreach (PreviewPixel pixel in Pixels)
				{
					pixel.X = (int)Math.Round(x);
					pixel.Y = (int)Math.Round(y);
					x -= xSpacing;
					y -= ySpacing;
				}

				SetPixelZoomRotate();
			}
		}

        [Browsable(false)]
        public bool AddStartPadding { get; set; }
		[Browsable(false)]
		public bool AddEndPadding { get; set; }

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			// See if we're resizing
			if (_selectedPoint != null) {
				var point = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(x, y), -ZoomLevel, PreviewTools.RotateTypes.Counterclockwise);
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
			}
				// If we get here, we're moving
			else {
				changeX = Convert.ToInt32(p1Start.X + changeX / ZoomLevel) - _points[0].X;
				changeY = Convert.ToInt32(p1Start.Y + changeY / ZoomLevel) - _points[0].Y;
				_points[0].X += changeX;
				_points[0].Y += changeY;
				_points[1].X += changeX;
				_points[1].Y += changeY;

			}
			base.MouseMove(x, y, changeX, changeY);

			Layout();
		}

		public override void SelectDragPoints()
		{
			if (_points.Count >= 2) {
				List<PreviewPoint> selectPoints = new List<PreviewPoint>();
				selectPoints.Add(_points[0]);
				selectPoints.Add(_points[1]);
				SetSelectPoints(selectPoints, null);
			}
		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels) {
				Rect r = new Rect(pixel.X - (SelectPointSize/2), pixel.Y - (SelectPointSize/2),
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
				p1Start = new PreviewPoint(_points[0].X, _points[0].Y);
				p2Start = new PreviewPoint(_points[1].X, _points[1].Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _points[1];
		}

		public override object Clone()
		{
			PreviewLine newLine = (PreviewLine) this.MemberwiseClone();

			newLine._pixels = new List<PreviewPixel>();

			foreach (PreviewPixel pixel in _pixels) {
				newLine._pixels.Add(pixel.Clone());
			}

			newLine._points = new List<PreviewPoint>();
			foreach (var previewPoint in _points)
			{
				newLine._points.Add(previewPoint.Copy());
			}
			return newLine;
		}

		public override void MoveTo(int x, int y)
		{
			Point topLeft = new Point();
			topLeft.X = Math.Min(_points[0].X, _points[1].X);
			topLeft.Y = Math.Min(_points[0].Y, _points[1].Y);

			int deltaX = x - topLeft.X;
			int deltaY = y - topLeft.Y;

			_points[0].X += deltaX;
			_points[0].Y += deltaY;
			_points[1].X += deltaX;
			_points[1].Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_points[0].X = (int) (_points[0].X*aspect);
			_points[0].Y = (int) (_points[0].Y*aspect);
			_points[1].X = (int) (_points[1].X*aspect);
			_points[1].Y = (int) (_points[1].Y*aspect);
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_points[0].X = p1Start.X;
			_points[0].Y = p1Start.Y;
			_points[1].X = p2Start.X;
			_points[1].Y = p2Start.Y;
			Resize(aspect);
		}
	}
}