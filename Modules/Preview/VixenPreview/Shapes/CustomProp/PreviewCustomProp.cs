using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class PreviewCustomProp : PreviewBaseShape
	{
		[DataMember]
		private List<PreviewPoint> _points = new List<PreviewPoint>();

		[DataMember]
		private PreviewPoint _topLeft;
		[DataMember]
		private PreviewPoint _topRight;
		[DataMember]
		private PreviewPoint _bottomLeft;
		[DataMember]
		private PreviewPoint _bottomRight;
		[DataMember]
		private List<Square> _squares;
		[DataMember]
		private Point _gridSize { get; set; }

		private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;
		private ElementNode initiallyAssignedNode = null;
		[DataMember]
		public List<PreviewBaseShape> _channels;
		[DataMember]
		private Prop _prop;

		public PreviewCustomProp(PreviewPoint point1, ElementNode selectedNode, double zoomLevel, Prop prop)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_topRight = new PreviewPoint(_topLeft);
			_bottomLeft = new PreviewPoint(_topLeft);
			_bottomRight = new PreviewPoint(_topLeft);
			initiallyAssignedNode = selectedNode;
			_prop = prop;
			if (_bottomRight.X - _bottomLeft.X < _prop.Width) _bottomRight.X = _bottomLeft.X + _prop.Width;
			if (_topLeft.Y - _bottomLeft.X < _prop.Height) _topLeft.Y = _topRight.Y = _topLeft.Y + _bottomLeft.X + _prop.Height;
			_squares = _prop.GetSelectedSquares();
			Layout();
		}



		[Browsable(false)]
		public int PixelCount
		{
			get { return Pixels.Count; }
		}

		[Browsable(false)]
		public override StringTypes StringType
		{
			get { return StringTypes.Standard; }
			set { _stringType = value; }
		}

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("Custom Props are defined by 2 points. This is point 1.")]
		public Point TopLeftPoint
		{
			get
			{
				Point p = new Point(_topLeft.X, _topLeft.Y);
				return p;
			}
			set
			{
				_bottomLeft.X = _topLeft.X = value.X;
				_topLeft.Y = value.Y;
				if (_bottomRight.X - _bottomLeft.X < _gridSize.X) _bottomRight.X = _gridSize.X + _bottomLeft.X;
				if (_topLeft.Y - _bottomLeft.X < _gridSize.Y) _topLeft.X = _topLeft.Y = _topRight.Y = _bottomLeft.X + _gridSize.X;

				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("Custom Props are defined by 2 points. This is point 2.")]
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
				_topRight.Y = _bottomRight.Y = value.Y;
				//Size cannot be smaller than the prop width/height
				if (_bottomRight.X - _bottomLeft.X < _gridSize.X) _bottomRight.X = _gridSize.X + _bottomLeft.X;

				if (_topLeft.Y - _bottomLeft.X < _gridSize.Y) _topLeft.X = _topLeft.Y = _topRight.Y = _bottomLeft.X + _gridSize.X;

				Layout();
			}
		}


		private bool lockXY = false;

		public override int Top
		{
			get;
			set;
		}

		public override int Bottom
		{
			get
			{
				int yMax = 0;
				foreach (PreviewPoint p in _points)
				{
					yMax = Math.Max(yMax, p.Y);
				}
				return yMax;
			}
		}

		public override int Left
		{
			get
			{
				int l = int.MaxValue;
				foreach (PreviewPoint p in _points)
				{
					l = Math.Min(l, p.X);
				}
				return l;
			}
			set
			{
				int currentLeft = Left;
				int delta = currentLeft - value;
				foreach (PreviewPoint p in _points)
				{
					p.X = p.X - delta;
				}
				Layout();
			}
		}

		public override int Right
		{
			get
			{
				int xMax = 0;
				foreach (PreviewPoint p in _points)
				{
					xMax = Math.Max(xMax, p.X);
				}
				return xMax;
			}
		}

		[DataMember, Browsable(false)]
		public virtual List<PreviewPixel> Pixels
		{
			get
			{
				if (_strings != null && _strings.Count > 0)
				{
					List<PreviewPixel> outPixels = new List<PreviewPixel>();
					foreach (PreviewBaseShape channel in _channels)
					{
						foreach (PreviewPixel pixel in channel.Pixels)
						{
							outPixels.Add(pixel);
						}
					}
					return outPixels.ToList();
				}
				else
				{
					return _pixels;
				}
			}
			set { _pixels = value; }
		}

		public override void Match(PreviewBaseShape matchShape)
		{
			//throw new NotImplementedException();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		public override void Layout()
		{
			if (_topLeft != null && _bottomRight != null)
			{
				ElementNode node = null;

				if (PixelCount > 0)
				{
					node = _pixels[0].Node;
					_pixels.Clear();
				}
				else if (initiallyAssignedNode != null)
				{
					if (initiallyAssignedNode.IsLeaf)
					{
						node = initiallyAssignedNode;
					}
				}

				Point boundsTopLeft = new Point();
				boundsTopLeft.X = Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
				boundsTopLeft.Y = Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));
				Point bottomRight = new Point();
				bottomRight.X = Math.Max(_topLeft.X, Math.Max(Math.Max(_topRight.X, _bottomRight.X), _bottomLeft.X));
				bottomRight.Y = Math.Max(_topLeft.Y, Math.Max(Math.Max(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));
				Rectangle rect = new Rectangle(boundsTopLeft, new Size(bottomRight.X - boundsTopLeft.X, bottomRight.Y - boundsTopLeft.Y));

				Point tL = new Point(_topLeft.X - boundsTopLeft.X, _topLeft.Y - boundsTopLeft.Y);
				Point tR = new Point(_topRight.X - boundsTopLeft.X, _topRight.Y - boundsTopLeft.Y);
				Point bL = new Point(_bottomLeft.X - boundsTopLeft.X, _bottomLeft.Y - boundsTopLeft.Y);
				Point bR = new Point(_bottomRight.X - boundsTopLeft.X, _bottomRight.Y - boundsTopLeft.Y);
				Point[] points = { tL, tR, bR, bL };

				if (rect.Width > 0 && rect.Height > 0)
				{
					using (var b = new Bitmap(rect.Width, rect.Height))
					{
						Graphics g = Graphics.FromImage(b);
						g.Clear(Color.Transparent);
						g.FillPolygon(Brushes.White, points);
						using (FastPixel.FastPixel fp = new FastPixel.FastPixel(b))
						{
							fp.Lock();
							var xSpacing = Math.Round((double)rect.Width / (double)_prop.Squares.GetLength(0), 0);
							var ySpacing = Math.Round((double)rect.Height / (double)_prop.Squares.GetLength(1), 0);
							if (xSpacing < 1) xSpacing = 1;
							if (ySpacing < 1) ySpacing = 1;

							_squares.ForEach(sq =>
							{
								Color newColor = fp.GetPixel((int)xSpacing * sq.X, (int)ySpacing * sq.Y);
								if (newColor.A != 0)
								{
									PreviewPixel pixel = new PreviewPixel(sq.X, sq.Y , 0, PixelSize);
									pixel.Node = node;
									_pixels.Add(pixel);
								}
							});

							fp.Unlock(false);
						}
					}
					SetPixelZoom();
				}

			}
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


		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null)
			{
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				if (lockXY ||
					(_selectedPoint == _bottomRight &&
					 System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control))
				{
					_topRight.X = point.X;
					_bottomLeft.Y = point.Y;
				}
				Layout();
			}
			// If we get here, we're moving
			else
			{
				//_topLeft.X = topLeftStart.X + changeX;
				//_topLeft.Y = topLeftStart.Y + changeY;
				//_topRight.X = topRightStart.X + changeX;
				//_topRight.Y = topRightStart.Y + changeY;
				//_bottomLeft.X = bottomLeftStart.X + changeX;
				//_bottomLeft.Y = bottomLeftStart.Y + changeY;
				//_bottomRight.X = bottomRightStart.X + changeX;
				//_bottomRight.Y = bottomRightStart.Y + changeY;

				_topLeft.X = Convert.ToInt32(topLeftStart.X * ZoomLevel) + changeX;
				_topLeft.Y = Convert.ToInt32(topLeftStart.Y * ZoomLevel) + changeY;
				_topRight.X = Convert.ToInt32(topRightStart.X * ZoomLevel) + changeX;
				_topRight.Y = Convert.ToInt32(topRightStart.Y * ZoomLevel) + changeY;
				_bottomLeft.X = Convert.ToInt32(bottomLeftStart.X * ZoomLevel) + changeX;
				_bottomLeft.Y = Convert.ToInt32(bottomLeftStart.Y * ZoomLevel) + changeY;
				_bottomRight.X = Convert.ToInt32(bottomRightStart.X * ZoomLevel) + changeX;
				_bottomRight.Y = Convert.ToInt32(bottomRightStart.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_topRight);
				PointToZoomPointRef(_bottomLeft);
				PointToZoomPointRef(_bottomRight);

				Layout();
			}
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
				topLeftStart = new PreviewPoint(_topLeft.X, _topLeft.Y);
				bottomRightStart = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}


		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
		}

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - _topLeft.X;
			int deltaY = y - _topLeft.Y;

			_topLeft.X += deltaX;
			_topLeft.Y += deltaY;
			_bottomRight.X += deltaX;
			_bottomRight.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{

			_topLeft.X = (int)(_topLeft.X * aspect);
			_topLeft.Y = (int)(_topLeft.Y * aspect);
			_bottomRight.X = (int)(_bottomRight.X * aspect);
			_bottomRight.Y = (int)(_bottomRight.Y * aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeft.X = _topLeft.X;
			_topLeft.Y = _topLeft.Y;
			_bottomRight.X = _bottomRight.X;
			_bottomRight.Y = _bottomRight.Y;

			Resize(aspect);
		}

	}
}
