using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewRectangle : PreviewLightBaseShape
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _topRight;
		[DataMember] private PreviewPoint _bottomLeft;
		[DataMember] private PreviewPoint _bottomRight;

		public override string TypeName => @"Rectangle";

		public enum Directions
		{
			Clockwise,
			CounterClockwise
		}

		private bool lockXY = false;
		private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;

		public PreviewRectangle(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_topLeft.PointType = PreviewPoint.PointTypes.SizeTopLeft;

			_topRight = new PreviewPoint(_topLeft);
			_topRight.PointType = PreviewPoint.PointTypes.SizeTopRight;

			_bottomLeft = new PreviewPoint(_topLeft);
			_bottomLeft.PointType = PreviewPoint.PointTypes.SizeBottomLeft;

			_bottomRight = new PreviewPoint(_topLeft);
			_bottomRight.PointType = PreviewPoint.PointTypes.SizeBottomRight;

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_strings = new List<PreviewBaseShape>();

			if (node != null)
			{
				List<ElementNode> parents = PreviewTools.GetParentNodes(node);
				// Do we have the 4 sides of the rectangle defined in our elements?
				if (parents.Count() == 4)
				{
					foreach (ElementNode pixelString in parents)
					{
						PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(20, 20), pixelString.Children.Count(), pixelString, ZoomLevel);
						line.PixelColor = Color.White;
						_strings.Add(line);
					}
				}
				else
				{
					List<ElementNode> children = PreviewTools.GetLeafNodes(node);
					if (children.Count >= 8)
					{
						int increment = children.Count / 4;
						int pixelsLeft = children.Count;

						StringType = StringTypes.Pixel;

						// Just add lines, they will be layed out in Layout()
						for (int i = 0; i < 4; i++)
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
			}

			if (_strings.Count == 0)
			{
				// Just add lines, they will be layed out in Layout()
				for (int i = 0; i < 4; i++)
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
			_topLeft.PointType = PreviewPoint.PointTypes.SizeTopLeft;
			_bottomRight.PointType = PreviewPoint.PointTypes.SizeBottomRight;
			_topRight.PointType = PreviewPoint.PointTypes.SizeTopRight;
			_bottomLeft.PointType = PreviewPoint.PointTypes.SizeBottomLeft;
			Layout();
		}

		public override List<PreviewPixel> Pixels
		{
			get
			{
				List<PreviewPixel> pixels = new List<PreviewPixel>();
				if (_strings != null) 
				{ 
					foreach (PreviewLightBaseShape lightString in LightStrings)
					{
						foreach (PreviewPixel pixel in lightString._pixels)
						{
							pixels.Add(pixel);
						}
					}
				}
				return pixels;
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("Rectangles are defined by 4 points. This is point 1.")]
		public Point TopLeftPoint
		{
			get
			{
				if (_topLeft == null)
				{
					_topLeft = new PreviewPoint(10, 10);
					_topLeft.PointType = PreviewPoint.PointTypes.SizeTopLeft;
				}
					Point p = new Point(_topLeft.X, _topLeft.Y);
				return p;
			}
			set
			{
				_topLeft.X = value.X;
				_topLeft.Y = value.Y;
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Top Right"),
		 DescriptionAttribute("Rectangles are defined by 4 points. This is point 2.")]
		public Point TopRightPoint
		{
			get
			{
				Point p = new Point(_topRight.X, _topRight.Y);
				return p;
			}
			set
			{
				_topRight.X = value.X;
				_topRight.Y = value.Y;
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("Rectangles are defined by 4 points. This is point 3.")]
		public Point BottomRightPoint
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
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Botom Left"),
		 DescriptionAttribute("Rectangles are defined by 4 points. This is point 4.")]
		public Point BottomLeftPoint
		{
			get
			{
				Point p = new Point(_bottomLeft.X, _bottomLeft.Y);
//				_bottomLeft.PointType = PreviewPoint.PointTypes.SizeBottomLeft;
				return p;
			}
			set
			{
				_bottomLeft.X = value.X;
				_bottomLeft.Y = value.Y;
				PreviewTools.TransformPreviewPoint(this);
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String 1 Light Count"),
		 DescriptionAttribute("Number of pixels or lights in string 1 of the rectangle.")]
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
		 DescriptionAttribute("Number of pixels or lights in string 2 of the rectangle.")]
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
		 DescriptionAttribute("Number of pixels or lights in string 3 of the rectangle.")]
		public int LightCountString3
		{
			get { return Strings[2].Pixels.Count; }
			set
			{
				(Strings[2] as PreviewLine).PixelCount = value;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String 4 Light Count"),
		 DescriptionAttribute("Number of pixels or lights in string 4 of the rectangle.")]
		public int LightCountString4
		{
			get { return Strings[3].Pixels.Count; }
			set
			{
				(Strings[3] as PreviewLine).PixelCount = value;
				Layout();
			}
		}

        private Directions _direction = Directions.Clockwise;

		[CategoryAttribute("Settings"),
		 DisplayName("Direction"),
		 DescriptionAttribute("Wrap direction."),
		 DataMember]
		public Directions Direction 
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                Layout();
            }
        }

		public int PixelCount
		{
			get { return Pixels.Count; }
		}

        public override int Top
        {
            get
            {
                return (Math.Min(_topLeft.Y, Math.Min(_topRight.Y, Math.Min(_bottomLeft.Y, _bottomRight.Y))));
            }
            set
            {
                int delta = Top - value;
                if (_topLeft.Y == Top)
                {
                    _topLeft.Y = value;
                    _topRight.Y -= delta;
                    _bottomLeft.Y -= delta;
                    _bottomRight.Y -= delta;
                }
                else if (_topRight.Y == Top)
                {
                    _topLeft.Y -= delta;
                    _topRight.Y = value;
                    _bottomLeft.Y -= delta;
                    _bottomRight.Y -= delta;
                }
                else if (_bottomLeft.Y == Top)
                {
                    _topLeft.Y -= delta;
                    _topRight.Y -= delta;
                    _bottomLeft.Y = value;
                    _bottomRight.Y -= delta;
                }
                else
                {
                    _topLeft.Y -= delta;
                    _topRight.Y -= delta;
                    _bottomLeft.Y -= delta;
                    _bottomRight.Y = value;
                }
                Layout();
            }
        }

        public override int Bottom
        {
            get
            {
                return (Math.Max(_topLeft.Y, Math.Max(_topRight.Y, Math.Max(_bottomLeft.Y, _bottomRight.Y))));
			}
        }

        public override int Right
        {
            get
            {
                return (Math.Max(_topLeft.X, Math.Max(_topRight.X, Math.Max(_bottomLeft.X, _bottomRight.X))));
			}
        }

        public override int Left
        {
            get
            {
                return (Math.Min(_topLeft.X, Math.Min(_topRight.X, Math.Min(_bottomLeft.X, _bottomRight.X))));
            }
            set
            {
                int delta = Left - value;
                if (_topLeft.X == Left)
                {
                    _topLeft.X = value;
                    _topRight.X -= delta;
                    _bottomLeft.X -= delta;
                    _bottomRight.X -= delta;
                }
                else if (_topRight.X == Left)
                {
                    _topLeft.X -= delta;
                    _topRight.X = value;
                    _bottomLeft.X -= delta;
                    _bottomRight.X -= delta;
                }
                else if (_bottomLeft.X == Left)
                {
                    _topLeft.X -= delta;
                    _topRight.X -= delta;
                    _bottomLeft.X = value;
                    _bottomRight.X -= delta;
                }
                else
                {
                    _topLeft.X -= delta;
                    _topRight.X -= delta;
                    _bottomLeft.X -= delta;
                    _bottomRight.X = value;
                }
                Layout();
            }
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewRectangle shape = (matchShape as PreviewRectangle);
            PixelSize = shape.PixelSize;
            _topRight.X = _topLeft.X + (shape._topRight.X - shape._topLeft.X);
            _topRight.Y = _topLeft.Y + (shape._topRight.Y - shape._topRight.Y);
            _bottomRight.X = _topLeft.X + (shape._bottomRight.X - shape._topLeft.X);
            _bottomRight.Y = _topLeft.Y + (shape._bottomRight.Y - shape._topRight.Y);
            _bottomLeft.X = _topLeft.X + (shape._bottomLeft.X - shape._topLeft.X);
            _bottomLeft.Y = _topLeft.Y + (shape._bottomLeft.Y - shape._topRight.Y);
            _topRight.X = _topLeft.X + (shape._topRight.X - shape._topLeft.X);
            _topRight.Y = _topLeft.Y + (shape._topRight.Y - shape._topRight.Y);
			base.Match(shape);
            Layout();
        }

		public override void Layout()
		{
			if (_topLeft != null && _bottomRight != null)
			{
				// Calculate the relative position of all vertices
				var _bottomLeft = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(BottomLeftPoint), ZoomLevel);
				var _bottomRight = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(BottomRightPoint), ZoomLevel);
				var _topRight = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(TopRightPoint), ZoomLevel);
				var _topLeft = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(TopLeftPoint), ZoomLevel);

				// Paint 4 lines based upon the adjusted vertices. Set the Zoom Level to 1, since the vertices themselves have
				// already been zoomed.
				if (Direction == Directions.CounterClockwise)
				{
                    // Start in the lower left corner and move counter clockwise around the rectangle.
                    PreviewLine line = Strings[0] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = false;
					line.AddEndPadding = false;
					line.Point1 = _bottomLeft.ToPoint();
                    line.Point2 = _bottomRight.ToPoint();
                    line.Layout();

                    line = Strings[1] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = true;
					line.AddEndPadding = true;
					line.Point1 = new Point(_bottomRight.X, _bottomRight.Y - 1);
                    line.Point2 = new Point(_topRight.X, _topRight.Y + 1);
                    line.Layout();

                    line = Strings[2] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = false;
					line.AddEndPadding = false;
					line.Point1 = _topRight.ToPoint();
                    line.Point2 = _topLeft.ToPoint();
                    line.Layout();

                    line = Strings[3] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = true;
					line.AddEndPadding = true;
					line.Point1 = new Point(_topLeft.X, _topLeft.Y + 1);
                    line.Point2 = new Point(_bottomLeft.X, _bottomLeft.Y - 1);
                    line.Layout();
				}
				else
				{
					// Start in the lower left corner and move clockwise around the rectangle.
					PreviewLine line = Strings[0] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = false;
					line.AddEndPadding = false;
					line.Point1 = _bottomLeft.ToPoint();
					line.Point2 = _topLeft.ToPoint();
					line.Layout();

					line = Strings[1] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = true;
					line.AddEndPadding = true;
					line.Point1 = new Point(_topLeft.X + 1, _topLeft.Y);
					line.Point2 = new Point(_topRight.X - 1, _topRight.Y);
					line.Layout();


					line = Strings[2] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = false;
					line.AddEndPadding = false;
					line.Point1 = _topRight.ToPoint();
					line.Point2 = _bottomRight.ToPoint();
					line.Layout();

					line = Strings[3] as PreviewLine;
					line.ZoomLevel = 1;
					line.AddStartPadding = true;
					line.AddEndPadding = true;
					line.Point1 = new Point(_bottomRight.X - 1, _bottomRight.Y);
					line.Point2 = new Point(_bottomLeft.X + 1, _bottomLeft.Y);
					line.Layout();
				}
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			if (_selectedPoint != null)
			{
				var point = PreviewTools.TransformPreviewPoint(this, new PreviewPoint(x, y), -ZoomLevel, PreviewTools.RotateTypes.Counterclockwise);
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				if (lockXY ||
				    (_selectedPoint == _bottomRight &&
				     System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)) {
					_topRight.X = point.X;
					_bottomLeft.Y = point.Y;
				}
				base.MouseMove(x, y, changeX, changeY);
				Layout();
			}
				// If we get here, we're moving
			else {
				changeX = Convert.ToInt32(topLeftStart.X + changeX / ZoomLevel) - _topLeft.X;
				changeY = Convert.ToInt32(topLeftStart.Y + changeY / ZoomLevel) - _topLeft.Y;

				_topLeft.X += changeX;
				_topLeft.Y += changeY;
				_topRight.X += changeX;
				_topRight.Y += changeY;
				_bottomLeft.X += changeX;
				_bottomLeft.Y += changeY;
				_bottomRight.X += changeX;
				_bottomRight.Y += changeY;

				base.MouseMove(x, y, changeX, changeY);
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
			points.Add(_topLeft);
			points.Add(_topRight);
			points.Add(_bottomLeft);
			points.Add(_bottomRight);
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
			lockXY = false;
			if (point == null || (point.X == 0 && point.Y == 0)) {
				topLeftStart = new PreviewPoint(_topLeft.X, _topLeft.Y);
				topRightStart = new PreviewPoint(_topRight.X, _topRight.Y);
				bottomLeftStart = new PreviewPoint(_bottomLeft.X, _bottomLeft.Y);
				bottomRightStart = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
			base.SetSelectPoint(point);
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
			lockXY = true;
		}

		public override void MoveTo(int x, int y)
		{
			Point topLeft = new Point();
			topLeft.X = Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
			topLeft.Y = Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));

			int deltaX = x - topLeft.X;
			int deltaY = y - topLeft.Y;

			_topLeft.X += deltaX;
			_topLeft.Y += deltaY;
			_topRight.X += deltaX;
			_topRight.Y += deltaY;
			_bottomRight.X += deltaX;
			_bottomRight.Y += deltaY;
			_bottomLeft.X += deltaX;
			_bottomLeft.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeft.X = (int) (_topLeft.X*aspect);
			_topLeft.Y = (int) (_topLeft.Y*aspect);
			_topRight.X = (int) (_topRight.X*aspect);
			_topRight.Y = (int) (_topRight.Y*aspect);
			_bottomRight.X = (int) (_bottomRight.X*aspect);
			_bottomRight.Y = (int) (_bottomRight.Y*aspect);
			_bottomLeft.X = (int) (_bottomLeft.X*aspect);
			_bottomLeft.Y = (int) (_bottomLeft.Y*aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeft.X = topLeftStart.X;
			_topLeft.Y = topLeftStart.Y;
			_bottomRight.X = bottomRightStart.X;
			_bottomRight.Y = bottomRightStart.Y;
			_topRight.X = topRightStart.X;
			_topRight.Y = topRightStart.Y;
			_bottomLeft.X = bottomLeftStart.X;
			_bottomLeft.Y = bottomLeftStart.Y;

			Resize(aspect);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newRectangle = (PreviewRectangle) MemberwiseClone();
			newRectangle._topRight = _topRight.Copy();
			newRectangle._topLeft = _topLeft.Copy();
			newRectangle._bottomRight = _bottomRight.Copy();
			newRectangle._bottomLeft = _bottomLeft.Copy();

			newRectangle.Pixels = new List<PreviewPixel>();
			foreach (var previewPixel in Pixels)
			{
				newRectangle.Pixels.Add(previewPixel.Clone());
			}

			return newRectangle;
		}

		#endregion
	}
}