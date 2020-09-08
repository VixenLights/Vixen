using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Controls;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewNet : PreviewBaseShape
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _topRight;
		[DataMember] private PreviewPoint _bottomLeft;
		[DataMember] private PreviewPoint _bottomRight;

		[DataMember] private int _pixelSpacing = 8;

		private ElementNode initiallyAssignedNode = null;
		private bool lockXY = false;
		private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;

		public override string TypeName => @"Net";

		public PreviewNet(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_topRight = new PreviewPoint(_topLeft);
			_bottomLeft = new PreviewPoint(_topLeft);
			_bottomRight = new PreviewPoint(_topLeft);

			Reconfigure(selectedNode);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			_pixels.Clear();
			initiallyAssignedNode = node;
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			if (_pixelSpacing == 0)
				_pixelSpacing = 8;
		}

		#region "Properties"

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("Nets are defined by 4 points. This is point 1.")]
		public Point TopLeftPoint
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
		 DisplayName("Top Right"),
		 DescriptionAttribute("Nets are defined by 4 points. This is point 2.")]
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
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("Nets are defined by 4 points. This is point 3.")]
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
				Layout();
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Botom Left"),
		 DescriptionAttribute("Nets are defined by 4 points. This is point 4.")]
		public Point BottomLeftPoint
		{
			get
			{
				Point p = new Point(_bottomLeft.X, _bottomLeft.Y);
				return p;
			}
			set
			{
				_bottomLeft.X = value.X;
				_bottomLeft.Y = value.Y;
				Layout();
			}
		}

		[CategoryAttribute("Settings"),
		 DisplayName("Light Spacing"),
		 DescriptionAttribute("This is the spacing between each light in the net.")]
		public int PixelSpacing
		{
			get { return _pixelSpacing; }
			set
			{
				_pixelSpacing = value;
				Layout();
			}
		}

		[Browsable(false)]
		public override StringTypes StringType
		{
			get { return StringTypes.Standard; }
			set { _stringType = value; }
		}

		[Browsable(false)]
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

        public override int Right
        {
            get
            {
                return Math.Max(_topLeft.X, Math.Max(_topRight.X, Math.Max(_bottomLeft.X, _bottomRight.X)));
			}
        }

        public override int Bottom
        {
            get
            {
                return (Math.Max(_topLeft.Y, Math.Max(_topRight.Y, Math.Max(_bottomLeft.Y, _bottomRight.Y))));
			}
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewNet shape = (matchShape as PreviewNet);
            PixelSize = shape.PixelSize;
            PixelSpacing = shape.PixelSpacing;
            
            _bottomRight.X = _topLeft.X + (shape._bottomRight.X - shape._topLeft.X);
            _bottomRight.Y = _topLeft.Y + (shape._bottomRight.Y - shape._topLeft.Y);
            
            _bottomLeft.X = _topLeft.X + (shape._bottomLeft.X - shape._topLeft.X);
            _bottomLeft.Y = _topLeft.Y + (shape._bottomLeft.Y - shape._topLeft.Y);

            _topRight.X = _topLeft.X + (shape._topRight.X - shape._topLeft.X);
            _topRight.Y = _topLeft.Y + (shape._topRight.Y - shape._topLeft.Y);
            Layout();
        }

		#endregion

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
				Rectangle rect = new Rectangle(boundsTopLeft,
											   new Size(bottomRight.X - boundsTopLeft.X, bottomRight.Y - boundsTopLeft.Y));

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
							int xCount = 1;
							int spacingY = _pixelSpacing;
							for (int y = 0; y < rect.Height; y++)
							{
								if (spacingY % _pixelSpacing == 0)
								{
									int xDiv;
									if (xCount % 2 == 0)
										xDiv = _pixelSpacing;
									else
										xDiv = _pixelSpacing / 2;

									for (int x = 0; x < rect.Width; x++)
									{
										if ((x + xDiv) % _pixelSpacing == 0)
										{
											Color newColor = fp.GetPixel(x, y);
											if (newColor.A != 0)
											{
												PreviewPixel pixel = new PreviewPixel(x + boundsTopLeft.X, y + boundsTopLeft.Y, 0, PixelSize);
												pixel.Node = node;
												_pixels.Add(pixel);
											}
										}
									}
									xCount += 1;
								}
								spacingY += 1;
							}
							fp.Unlock(false);
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
				if (lockXY ||
				    (_selectedPoint == _bottomRight &&
				     System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)) {
					_topRight.X = point.X;
					_bottomLeft.Y = point.Y;
				}
				Layout();
			}
				// If we get here, we're moving
			else {
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
			if (point == null) {
				topLeftStart = new PreviewPoint(_topLeft.X, _topLeft.Y);
				topRightStart = new PreviewPoint(_topRight.X, _topRight.Y);
				bottomLeftStart = new PreviewPoint(_bottomLeft.X, _bottomLeft.Y);
				bottomRightStart = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
			lockXY = true;
		}

		public override void MoveTo(int x, int y)
		{
			Point boundsTopLeft = new Point();
			boundsTopLeft.X = Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
			boundsTopLeft.Y = Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));

			int changeX = x - boundsTopLeft.X;
			int changeY = y - boundsTopLeft.Y;

			_topLeft.X += changeX;
			_topLeft.Y += changeY;
			_topRight.X += changeX;
			_topRight.Y += changeY;
			_bottomRight.X += changeX;
			_bottomRight.Y += changeY;
			_bottomLeft.X += changeX;
			_bottomLeft.Y += changeY;

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
			var newNet = (PreviewNet) MemberwiseClone();
			newNet._topLeft = _topLeft.Copy();
			newNet._topRight = _topRight.Copy();
			newNet._bottomRight = _bottomRight.Copy();
			newNet._bottomLeft = _bottomLeft.Copy();

			newNet.Reconfigure(initiallyAssignedNode);

			return newNet;
		}

		#endregion
	}
}