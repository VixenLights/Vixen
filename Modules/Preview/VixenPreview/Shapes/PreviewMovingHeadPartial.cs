using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Vixen.Sys;
using Point = System.Drawing.Point;


namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Maintains a moving head preview graphic.
	/// </summary>
	[DataContract]
	public partial class PreviewMovingHead : PreviewBaseShape, IDrawStaticPreviewShape, IDisposable, IOpenGLMovingHeadShape  
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _topRight;
		[DataMember] private PreviewPoint _bottomLeft;
		[DataMember] private PreviewPoint _bottomRight;

		public override string TypeName => @"Moving Head";

		public enum Directions
		{
			Clockwise,
			CounterClockwise
		}

		private bool lockXY = false;
		private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;
				
		public PreviewMovingHead(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_topRight = new PreviewPoint(_topLeft);
			_bottomLeft = new PreviewPoint(_topLeft);
			_bottomRight = new PreviewPoint(_topLeft);

			// Default the movement constraints of the moving head
			PanStartPosition = DefaultPanStartPosition;
			PanStopPosition = DefaultPanStopPosition;
			TiltStartPosition = DefaultTiltStartPosition;
			TiltStartPosition = DefaultTiltStopPosition;

			// Default the beam length percentage
			BeamLength = DefaultBeamLength;

			Reconfigure(selectedNode);
		}
				
		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{			
			Layout();						
		}
	
		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("Rectangles are defined by 4 points. This is point 1.")]
		public Point TopLeftPoint
		{
			get
			{
				if (_topLeft == null)
					_topLeft = new PreviewPoint(10, 10);
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
				return p;
			}
			set
			{
				_bottomLeft.X = value.X;
				_bottomLeft.Y = value.Y;
				Layout();
			}
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
			PreviewMovingHead shape = (matchShape as PreviewMovingHead);			
			_topRight.X = _topLeft.X + (shape._topRight.X - shape._topLeft.X);
			_topRight.Y = _topLeft.Y + (shape._topRight.Y - shape._topRight.Y);
			_bottomRight.X = _topLeft.X + (shape._bottomRight.X - shape._topLeft.X);
			_bottomRight.Y = _topLeft.Y + (shape._bottomRight.Y - shape._topRight.Y);
			_bottomLeft.X = _topLeft.X + (shape._bottomLeft.X - shape._topLeft.X);
			_bottomLeft.Y = _topLeft.Y + (shape._bottomLeft.Y - shape._topRight.Y);
			_topRight.X = _topLeft.X + (shape._topRight.X - shape._topLeft.X);
			_topRight.Y = _topLeft.Y + (shape._topRight.Y - shape._topRight.Y);
			Layout();
		}

		public override void Layout()
		{			
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

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(_topLeft);
			points.Add(_topRight);
			points.Add(_bottomLeft);
			points.Add(_bottomRight);
			SetSelectPoints(points, null);
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
			_topLeft.X = (int)(_topLeft.X * aspect);
			_topLeft.Y = (int)(_topLeft.Y * aspect);
			_topRight.X = (int)(_topRight.X * aspect);
			_topRight.Y = (int)(_topRight.Y * aspect);
			_bottomRight.X = (int)(_bottomRight.X * aspect);
			_bottomRight.Y = (int)(_bottomRight.Y * aspect);
			_bottomLeft.X = (int)(_bottomLeft.X * aspect);
			_bottomLeft.Y = (int)(_bottomLeft.Y * aspect);

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
			PreviewMovingHead newRectangle = (PreviewMovingHead)MemberwiseClone();
			newRectangle._topRight = _topRight.Copy();
			newRectangle._topLeft = _topLeft.Copy();
			newRectangle._bottomRight = _bottomRight.Copy();
			newRectangle._bottomLeft = _bottomLeft.Copy();

			newRectangle.InitializeGDI();

			return newRectangle;
		}

		#endregion	
	}
}