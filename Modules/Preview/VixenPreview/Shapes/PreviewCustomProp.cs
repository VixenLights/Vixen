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
			base.Name = prop.Name;
			initiallyAssignedNode = selectedNode;

			Layout();
		}


		#region "Properties"

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("This is point 1.")]
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
	DescriptionAttribute("This is point 2.")]
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





		public override StringTypes StringType
		{
			get { return _stringType; }
			set
			{
				_stringType = value;
				if (_strings != null)
					foreach (var item in _strings)
					{
						item.StringType = _stringType;
					}
			}
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
				foreach (var item in _strings)
				{
					item.StringType = _stringType;
					item.Parent = this;
				}
				return _strings;
			}
			set
			{
				_strings = value;
				if (_strings != null)
					foreach (var item in _strings)
					{
						item.StringType = _stringType;
						item.Parent = this;
					}
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

				var wRatio = (float)rect.Width / (float)_prop.Width;
				var hRatio = (float)rect.Height / (float)_prop.Height;

				var w = wRatio;

				Strings = new List<PreviewBaseShape>();

				//var channels = _prop.GetRepositionedChannels();
				var channels = _prop.Channels;
				channels.ForEach(c =>
				{
					Strings.Add(new CustomPropBaseShape(_topLeft, c));
				});

				AdjustStringPixelsWithRatio(Strings, wRatio, hRatio);
				Strings.ForEach(s => s.Layout());

				SetPixelZoom();

			}

		}


		private void AdjustStringPixelsWithRatio(List<PreviewBaseShape> strings, float wRatio, float hRatio)
		{
			if (strings == null) return;

			foreach (CustomPropBaseShape s in strings)
			{
				AdjustStringPixelsWithRatio(s.Strings, wRatio, hRatio);

				if (s._pixels != null && s._pixels.Any())
				{
					s._pixels.ForEach(p =>
					{
						var xF = (float)p.X * wRatio;
						var yF = (float)p.Y * hRatio;
						p.X = (int)xF + _topLeft.X;
						p.Y = (int)yF + _topLeft.Y;
					});
				}
			}
		}
		private void MovePixels(List<PreviewBaseShape> strings, int changeX, int changeY)
		{
			if (strings == null) return;

			foreach (CustomPropBaseShape s in strings)
			{
				MovePixels(s.Strings, changeX, changeY);

				if (s._pixels != null && s._pixels.Any())
				{
					s._pixels.ForEach(p =>
					{
						p.X += changeX;
						p.Y += changeY;
					});
				}
			}
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
			MovePixels(Strings, changeX, changeY);


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