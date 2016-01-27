using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes.CustomProp;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewCustomProp : PreviewBaseShape
	{
		[DataMember]
		private PreviewPoint _topLeft;
		[DataMember]
		private PreviewPoint _bottomRight;

		[DataMember]
		private int _pixelSpacing = 8;

		[DataMember]
		internal Prop _prop = null;

		private ElementNode initiallyAssignedNode = null;
		private bool lockXY = false;
		private PreviewPoint topLeftStart, bottomRightStart;

		public PreviewCustomProp(PreviewPoint point1, ElementNode selectedNode, double zoomLevel, Prop prop)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_bottomRight = new PreviewPoint(_topLeft.X + prop.Width, _topLeft.Y + prop.Height);
			_prop = prop;

			initiallyAssignedNode = selectedNode;

			Layout();
		}

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
		 DescriptionAttribute("Nets are defined by 4 points. This is point 3.")]
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
			get { return StringTypes.Pixel; }
			set { _stringType = value; }
		}

		//[Browsable(false)]
		public int PixelCount
		{
			get { return Pixels.Count; }
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
			PreviewCustomProp shape = (matchShape as PreviewCustomProp);
			PixelSize = shape.PixelSize;
			_bottomRight.X = TopLeft.X + (shape.BottomRight.X - shape.TopLeft.X);
			_bottomRight.Y = TopLeft.Y + (shape.BottomRight.Y - shape.TopLeft.Y);
			Layout();
		}

		#endregion
		public override List<PreviewBaseShape> Strings
		{
			get
			{
				return _strings;
			}
			set
			{
				_strings = value;
			}
		}


		public override void Layout()
		{
			if (_topLeft != null && _bottomRight != null)
			{
				ElementNode node = null;

				if (PixelCount > 0 && _pixels.Any())
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
				boundsTopLeft.X = Math.Min(_topLeft.X, _bottomRight.X);
				boundsTopLeft.Y = Math.Min(_topLeft.Y, _bottomRight.Y);

				Point bottomRight = new Point();
				bottomRight.X = Math.Max(_topLeft.X, _bottomRight.X);
				bottomRight.Y = Math.Max(_topLeft.Y, _bottomRight.Y);

				Rectangle rect = new Rectangle(boundsTopLeft, new Size(bottomRight.X - boundsTopLeft.X, bottomRight.Y - boundsTopLeft.Y));

				var wRatio = rect.Width / _prop.Width;
				var hRatio = rect.Height / _prop.Height;

				Point tL = new Point(boundsTopLeft.X, boundsTopLeft.Y);
				Point tR = new Point(boundsTopLeft.X, boundsTopLeft.Y);
				Point bL = new Point(boundsTopLeft.X, boundsTopLeft.Y);
				Point bR = new Point(boundsTopLeft.X, boundsTopLeft.Y);

				Point[] points = { tL, tR, bR, bL };

				if (Strings == null)
					Strings = new List<PreviewBaseShape>();

				if (rect.Width > 0 && rect.Height > 0)
				{
					var xRatio = rect.Width / _prop.Width;
					var yRatio = rect.Height / _prop.Height;

					int spacingY = _pixelSpacing;

					for (int y = 0; y < _prop.Height; y++)
					{
						for (int x = 0; x < _prop.Width; x++)
						{

							var channel = _prop.Data.Rows[y][x];
							if (!string.IsNullOrWhiteSpace(channel as string))
							{

								int iChannel = Convert.ToInt32(channel);
								var ch = _prop.Channels.Where(c => c.ID == iChannel).FirstOrDefault();
								if (ch == null) 
									continue;
								
								var str = _strings.Where(s => s.Name.Equals(ch.Text)).FirstOrDefault();
								if (str == null)
									_strings.Add(new CustomPropBaseShape() { Name = ch.Text });


								PreviewPixel pixel = new PreviewPixel((x * xRatio) + boundsTopLeft.X, (y * yRatio) + boundsTopLeft.Y, 0, PixelSize);
								pixel.InternalId = string.Format("{0}.{1}", x, y);
								//PreviewPixel pixel = AddPixel((x * xRatio) + boundsTopLeft.X, (y * yRatio) + boundsTopLeft.Y);


								var stringPixel = _strings.Where(s => s.Name.Equals(ch.Text)).First().Pixels.Where(w => w.InternalId.Equals(pixel.InternalId)).FirstOrDefault();

								if (stringPixel == null)
								{
									//_pixels.Add(pixel);
									_strings.Where(s => s.Name.Equals(ch.Text)).First().Pixels.Add(pixel);
								}
								else
								{
									stringPixel.X = pixel.X;
									stringPixel.Y = pixel.Y;
								}

							}


						}
					}

					SetPixelZoom();
				}
			}
			CleanUpStrings();
		}
		private void CleanUpStrings()
		{
			//Remove pixels from strings that are no longer valid....

		}
		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size)
			{

				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				// If we get here, we're moving
			}
			// If we get here, we're moving
			else
			{

				_topLeft.X = Convert.ToInt32(topLeftStart.X * ZoomLevel) + changeX;
				_bottomRight.X = Convert.ToInt32(bottomRightStart.X * ZoomLevel) + changeX;


				_topLeft.Y = Convert.ToInt32(topLeftStart.Y * ZoomLevel) + changeY;
				_bottomRight.Y = Convert.ToInt32(bottomRightStart.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_bottomRight);


			}


			Layout();
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
			points.Add(_bottomRight);
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels)
			{
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2), SelectPointSize,
											SelectPointSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height)
				{
					return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			lockXY = false;
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
			lockXY = true;
		}

		public override void MoveTo(int x, int y)
		{
			Point boundsTopLeft = new Point();
			boundsTopLeft.X = Math.Min(_topLeft.X, _bottomRight.X);
			boundsTopLeft.Y = Math.Min(_topLeft.Y, _bottomRight.Y);

			int changeX = x - boundsTopLeft.X;
			int changeY = y - boundsTopLeft.Y;

			_topLeft.X += changeX;
			_topLeft.Y += changeY;

			_bottomRight.X += changeX;
			_bottomRight.Y += changeY;
			Strings.AsParallel().ForAll(str =>
			{
				str.Pixels.AsParallel().ForAll(pix =>
				{
					pix.X += changeX;
					pix.Y += changeY;
				});
			});

			//Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeft.X = (int)(_topLeft.X * aspect);
			_topLeft.Y = (int)(_topLeft.Y * aspect);
			_bottomRight.X = (int)(_bottomRight.X * aspect);
			_bottomRight.Y = (int)(_bottomRight.Y * aspect);
			Strings.AsParallel().ForAll(str =>
			{
				str.Pixels.AsParallel().ForAll(pix =>
				{
					pix.X = (int)(pix.X * aspect);
					pix.Y += (int)(pix.Y * aspect);
				});
			});

			//Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeft.X = topLeftStart.X;
			_topLeft.Y = topLeftStart.Y;
			_bottomRight.X = bottomRightStart.X;
			_bottomRight.Y = bottomRightStart.Y;
			Resize(aspect);
		}
	}

}