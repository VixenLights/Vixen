using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;
using System.Drawing.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewIcicle : PreviewBaseShape
	{
        [DataMember] private int _stringCount;
        [DataMember] private List<PreviewPoint> _points = new List<PreviewPoint>();
        [DataMember] private int _verticalSpacing;

        private PreviewPoint p1Start, p2Start;
        private Boolean creating = false;
        const int InitialStringSpacing = 10;
        const int InitialLightsPerString = 10;
        private ElementNode initialNode;
		//const int InitialStringLength = 20;

		public override string TypeName => @"Icicle";

		public PreviewIcicle(PreviewPoint point1, PreviewPoint point2, ElementNode selectedNode, double zoomLevel)
		{
			// If we are creating this fresh, we need to know so we can add strings, etc. as drawn.
			creating = true;
            initialNode = selectedNode;

			ZoomLevel = zoomLevel;
			AddPoint(PointToZoomPoint(point1));
			AddPoint(PointToZoomPoint(point2));

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			if (node != null)
			{
				if (node.Children.Count() > 0 && PreviewTools.GetLeafNodes(node).Count == 0)
				{
					StringType = StringTypes.Pixel;
					_strings = new List<PreviewBaseShape>();
					foreach (ElementNode child in node.Children)
					{
						int pixelCount = child.Children.Count();
						PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), pixelCount, child, ZoomLevel);
						line.Parent = this;
						_strings.Add(line);
					}
					_stringCount = _strings.Count;
					creating = false;
				}
				else
				{
					StringType = StringTypes.Standard;
				}
			}
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

        [CategoryAttribute("Settings"),
         DisplayName("Vertical Spacing"),
         DescriptionAttribute("This is the spacing between each light in a vertical icicle strand.")]
        public int VerticalSpacing
        {
            get
            {
                _verticalSpacing = Math.Max(2, _verticalSpacing);
                return _verticalSpacing;
            }
            set
            {
                _verticalSpacing = value;
                Layout();
            }
        }

		[CategoryAttribute("Position"),
		 DisplayName("Point 1"),
		 DescriptionAttribute("Icicles are defined by 2 points. This is point 1.")]
		public Point Point1
		{
			get
			{
				Point p = new Point(_points[0].X, _points[0].Y);
				return p;
			}
			set
			{
				_points[0].X = value.X;
				_points[0].Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 2"),
		 DescriptionAttribute("Icicles are defined by 2 points. This is point 2.")]
		public Point Point2
		{
			get
			{
				Point p = new Point(_points[1].X, _points[1].Y);
				return p;
			}
			set
			{
				_points[1].X = value.X;
				_points[1].Y = value.Y;
				Layout();
			}
		}

        [CategoryAttribute("Settings"),
         DisplayName("Icicle Count"),
         DescriptionAttribute("Number of icicles in the string.")]
        public int StringCount
        {
            set
            {
                _stringCount = value;
                while (_strings.Count > _stringCount)
                {
                    _strings.RemoveAt(_strings.Count - 1);
                }
                while (_strings.Count < _stringCount)
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), InitialLightsPerString, null, ZoomLevel);
                    line.Parent = this;
                    _strings.Add(line);
                    if (StringType == StringTypes.Standard)
                    {
                        foreach (PreviewLine standardString in _strings)
                        {
                            if (creating)
                            {
                                _strings[0].Pixels[0].Node = initialNode;
                            }
                            standardString.connectStandardStrings = true;
                            standardString.StringType = StringTypes.Standard;
                            foreach (PreviewPixel pixel in standardString.Pixels)
                            {
                                pixel.Node = _strings[0].Pixels[0].Node;
                            }
                        }
                    }
                }
                Layout();
            }
            get { return _stringCount; }
        }


        [Editor(typeof(PreviewSetElementsUIEditor), typeof(UITypeEditor)),
         CategoryAttribute("Settings"),
         DisplayName("Linked Elements")]
        public override List<PreviewBaseShape> Strings
        {
            get
            {
                //Layout();
                if (_strings == null)
                {
                    _strings = new List<PreviewBaseShape>();
                }
                List<PreviewBaseShape> stringsResult;
                if (_strings.Count != StringCount)
                {
                    stringsResult = new List<PreviewBaseShape>();
                    for (int i = 0; i < StringCount; i++)
                    {
                        stringsResult.Add(_strings[i]);
                    }
                }
                else
                {
                    stringsResult = _strings;
                    if (stringsResult == null)
                    {
                        stringsResult = new List<PreviewBaseShape>();
                        stringsResult.Add(this);
                    }
                }
                foreach (PreviewLine line in stringsResult)
                {
                    line.Parent = this;
                }
                return stringsResult;
            }
            set { }
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

        public override int Right
        {
            get
            {
                return (Math.Max(_points[0].X, _points[1].X));
			}
        }

        public override int Bottom
        {
            get
            {
                return (Math.Max(_points[0].Y, _points[1].Y));
			}
        }

        public void AddPoint(PreviewPoint point)
        {
            _points.Add(point);
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewIcicle shape = (matchShape as PreviewIcicle);
            PixelSize = shape.PixelSize;
            _points[1].X = _points[0].X + (shape._points[1].X - shape._points[0].X);
            _points[1].Y = _points[0].Y + (shape._points[1].Y - shape._points[0].Y);
            Layout();
        }

		public override void Layout()
		{
			if (_points != null && _points.Count > 0)
			{
				double xSpacing = (double)(_points[0].X - _points[1].X) / (double)(StringCount - 1);
				double ySpacing = (double)(_points[0].Y - _points[1].Y) / (double)(StringCount - 1);
				double x = _points[0].X;
				double y = _points[0].Y;
				foreach (PreviewLine line in Strings)
				{
                    int lineLength = (VerticalSpacing * line.Pixels.Count);
					line.Point1 = new Point((int)Math.Round(x), (int)Math.Round(y));
                    line.Point2 = new Point((int)Math.Round(x), (int)Math.Round(y) + lineLength);
					x -= xSpacing;
					y -= ySpacing;
				}

				SetPixelZoom();
			}
		}

        private void SetupInitialString()
        {
            int currentXSpacing = Math.Abs(_selectedPoint.X - _points[0].X);
            int stringCount = 2;
            if (currentXSpacing > 0)
            {
                stringCount = (currentXSpacing / InitialStringSpacing) + 1;
                // The stringCount needs to be at least 2!
                stringCount = Math.Max(stringCount, 2);
            }
            StringCount = stringCount;
        }

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null) {
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
                if (creating)
                {
                    SetupInitialString();
                }
				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
				_points[0].X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_points[0].Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_points[1].X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_points[1].Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_points[0]);
				PointToZoomPointRef(_points[1]);

				Layout();
			}
		}

        public override void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(sender, e);
            creating = false;
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
				p1Start = new PreviewPoint(_points[0].X, _points[0].Y);
				p2Start = new PreviewPoint(_points[1].X, _points[1].Y);
			}

			_selectedPoint = point;
		}

        public override void Select(bool selectDragPoints)
        {
            base.Select(selectDragPoints);
            connectStandardStrings = true;
            foreach (PreviewLine line in _strings)
            {
                line.connectStandardStrings = true;
            }
        }

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _points[1];
		}

		public override object Clone()
		{
            PreviewIcicle newIcicle = (PreviewIcicle)this.MemberwiseClone();

			newIcicle._points = new List<PreviewPoint>();

			foreach (var previewPoint in _points)
			{
				newIcicle._points.Add(previewPoint.Copy());
			}

			newIcicle._pixels = new List<PreviewPixel>();

			foreach (PreviewPixel pixel in _pixels) {
				newIcicle.Pixels.Add(pixel.Clone());
			}
			return newIcicle;
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