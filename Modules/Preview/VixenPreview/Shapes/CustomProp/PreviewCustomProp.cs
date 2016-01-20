using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class PreviewCustomProp : PreviewBaseShape
	{
		[DataMember]
		private List<PreviewPoint> _points = new List<PreviewPoint>();
		[DataMember]
		private PreviewPoint _topLeftPoint;
		[DataMember]
		private PreviewPoint _bottomRightPoint;
		private PreviewPoint bottomRightStart, topLeftStart;

		[DataMember]
		public List<PreviewBaseShape> _channels;

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

		public override void Layout()
		{
			throw new NotImplementedException();
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_bottomRightPoint);
			points.Add(_topLeftPoint);
			SetSelectPoints(points, null);
		}


		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null)
			{
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;

				if (_selectedPoint == _bottomRightPoint &&
					System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
				{
					_bottomRightPoint.Y = _topLeftPoint.Y + (_bottomRightPoint.X - _topLeftPoint.X);
				}

				Layout();
				SelectDragPoints();
			}
			// If we get here, we're moving
			else
			{
				_bottomRightPoint.X = Convert.ToInt32(bottomRightStart.X * ZoomLevel) + changeX;
				_bottomRightPoint.Y = Convert.ToInt32(bottomRightStart.Y * ZoomLevel) + changeY;
				_topLeftPoint.X = Convert.ToInt32(topLeftStart.X * ZoomLevel) + changeX;
				_topLeftPoint.Y = Convert.ToInt32(topLeftStart.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeftPoint);
				PointToZoomPointRef(_bottomRightPoint);

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
				topLeftStart = new PreviewPoint(_topLeftPoint.X, _topLeftPoint.Y);
				bottomRightStart = new PreviewPoint(_bottomRightPoint.X, _bottomRightPoint.Y);
			}

			_selectedPoint = point;
		}


		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRightPoint;
		}

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - _topLeftPoint.X;
			int deltaY = y - _topLeftPoint.Y;

			_topLeftPoint.X += deltaX;
			_topLeftPoint.Y += deltaY;
			_bottomRightPoint.X += deltaX;
			_bottomRightPoint.Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeftPoint.X = (int)(_topLeftPoint.X * aspect);
			_topLeftPoint.Y = (int)(_topLeftPoint.Y * aspect);
			_bottomRightPoint.X = (int)(_bottomRightPoint.X * aspect);
			_bottomRightPoint.Y = (int)(_bottomRightPoint.Y * aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeftPoint.X = topLeftStart.X;
			_topLeftPoint.Y = topLeftStart.Y;
			_bottomRightPoint.X = bottomRightStart.X;
			_bottomRightPoint.Y = bottomRightStart.Y;
			Resize(aspect);
		}
	}
}
