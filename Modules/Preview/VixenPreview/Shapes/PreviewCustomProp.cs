using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Common.Controls.Timeline;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.Preview.VixenPreview.Converter;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	[TypeConverter(typeof(PropertySorter))]
	public class PreviewCustomProp : PreviewBaseShape
	{
		private List<PreviewDoublePoint> _dragPoints = new List<PreviewDoublePoint>();
		private PreviewPoint _p1Start;
		private double _zoomLevel;
		private Point _rotationCenter;
		private PreviewDoublePoint _selectionPoint;
		private int _rotationAngle;

		public override string TypeName => @"Custom Prop";

		public PreviewCustomProp(double zoomLevel)
		{
			PropPixels = new List<PreviewPixel>();
			StringType = StringTypes.Custom;
			ZoomLevel = zoomLevel;
		}

		public void AddLightNodes(ElementModel model, ElementNode node)
		{
			if (!model.IsLightNode) return;

			foreach (var light in model.Lights)
			{
				PreviewPixel p = AddHighPrecisionPixel(new Point(light.X, light.Y), model.LightSize);
				p.PixelColor = Color.White;
				p.Node = node;
			}
		}

		private PreviewPixel AddHighPrecisionPixel(Point location, int size)
		{
			PreviewPixel pixel = new PreviewPixel(location, size);
			pixel.PixelColor = PixelColor;
			PropPixels.Add(pixel);
			return pixel;
		}

		[DataMember]
		[Browsable(false)]
		public List<PreviewPixel> PropPixels { get; set; }

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override List<PreviewPixel> Pixels { get; set; }

		#endregion
		[Browsable(false)]
		public Rect Bounds { get; private set; }

		internal void UpdateBounds()
		{
			var rects = Pixels.Select(p => new Rect(p.Location.X, p.Location.Y, p.PixelSize, p.PixelSize));
			Bounds = GetCombinedBounds(rects);
			_rotationCenter = Center(Bounds);
			if (Selected)
			{
				UpdateDragPoints();
			}
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override int Bottom => (int)Bounds.Bottom;

		/// <inheritdoc />
		[Browsable(true)]
		[Category("Position")]
		[PropertyOrder(1)]
		[DisplayName("X")]
		public override int Left
		{
			get { return (int)Bounds.Left; }
			set
			{
				MoveTo(value, (int)Bounds.Y);
			}
		}

		/// <inheritdoc />
		[Browsable(true)]
		[Category("Position")]
		[PropertyOrder(2)]
		[DisplayName("Y")]
		public override int Top
		{
			get { return (int)Bounds.Top; }
			set
			{
				MoveTo((int)Bounds.X, value);
			}

		}

		/// <inheritdoc />
		public override int Right => (int)Bounds.Right;

		[DataMember]
		[DisplayName("Rotation Angle")]
		[PropertyOrder(3)]
		[Category("Position")]
		public int RotationAngle
		{
			get { return _rotationAngle; }
			set
			{
				if (value < 0 || value > 360) return;
				_rotationAngle = value;
				Layout();
			}
		}


		private static Point Center(Rect rect)
		{
			return new Point(rect.Left + rect.Width / 2,
				rect.Top + rect.Height / 2);
		}

		/// <inheritdoc />
		public override void Match(PreviewBaseShape matchShape)
		{
			//Match size for now
			var yAspect =  (matchShape.Bottom - matchShape.Top) / (double)(Bottom - Top);
			var xAspect = (matchShape.Right - matchShape.Left) / (double)(Right - Left);

			var customProp = matchShape as PreviewCustomProp;
			if (customProp != null)
			{
				//match up the rotation angle and set it directly
				//the scale below will call layout, so no need to do it twice.
				_rotationAngle = customProp.RotationAngle;
			}

			Scale(xAspect, yAspect, Left, Top);
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override int PixelSize
		{
			set;
			get;
		}

		/// <inheritdoc />
		public override void MatchPixelSize(PreviewBaseShape shape)
		{
			if (!shape.Pixels.Any()) return;

			var newsize = shape.Pixels[0].PixelSize;

			foreach (var previewPixel in PropPixels)
			{
				previewPixel.PixelSize = newsize;
			}

			foreach (var previewPixel in Pixels)
			{
				previewPixel.PixelSize = newsize;
			}
		}

		public override void ResizePixelsBy(int value)
		{
			foreach (var previewPixel in PropPixels)
			{
				var newSize = previewPixel.PixelSize + value;
				previewPixel.PixelSize = newSize > 0 ? newSize : 1;
			}

			foreach (var previewPixel in Pixels)
			{
				var newSize = previewPixel.PixelSize + value;
				previewPixel.PixelSize = newSize > 0 ? newSize : 1;
			}
		}

		[Browsable(false)]
		public override double ZoomLevel
		{
			get
			{
				return _zoomLevel;
			}
			set
			{
				if (value == _zoomLevel) return;
				_zoomLevel = value <= 0 ? 1 : value;
				Layout();
			}
		}

		public override void Deselect()
		{
			Selected = false;
			_selectionPoint = null;
			_dragPoints?.Clear();
		}

		/// <inheritdoc />
		public override void SelectDragPoints()
		{   
			ConfigureDragPoints();
		}

		private void UpdateDragPoints()
		{
			_dragPoints[0].X = Bounds.X;
			_dragPoints[0].Y = Bounds.Y;

			_dragPoints[1].X = Bounds.Right;
			_dragPoints[1].Y = Bounds.Top;

			_dragPoints[2].X = Bounds.Right;
			_dragPoints[2].Y = Bounds.Bottom;

			_dragPoints[3].X = Bounds.Left;
			_dragPoints[3].Y = Bounds.Bottom;

			_dragPoints[4].X = Bounds.Left + (Bounds.Right - Bounds.Left) / 2;
			_dragPoints[4].Y = Bounds.Top - 12;

			if (RotationAngle != 0)
			{
				RotateDragPoints();
			}
		}

		private void ConfigureDragPoints()
		{
			_dragPoints.Clear();
			_dragPoints.Add(new PreviewDoublePoint(Bounds.Left, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewDoublePoint(Bounds.Right, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewDoublePoint(Bounds.Right, Bounds.Bottom, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewDoublePoint(Bounds.Left, Bounds.Bottom, PreviewPoint.PointTypes.Size));

			_dragPoints.Add(new PreviewDoublePoint(Bounds.Left+(Bounds.Right-Bounds.Left)/2, Bounds.Top - 10, PreviewPoint.PointTypes.Rotate));

			if (RotationAngle != 0)
			{
				RotateDragPoints();
			}
		}

		public override void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_dragPoints != null)
			{
				foreach (PreviewDoublePoint point in _dragPoints)
				{
					if (point?.PointType == PreviewPoint.PointTypes.Size)
					{
						int x = Convert.ToInt32(point.X - (double)SelectPointSize / 2);
						int y = Convert.ToInt32(point.Y - (double)SelectPointSize / 2);
						fp.DrawRectangle(
							new Rectangle(x, y, SelectPointSize, SelectPointSize),
							Color.White);
					}
					else
					{
						if (point?.PointType == PreviewPoint.PointTypes.Rotate)
						{
							int x = Convert.ToInt32(point.X - (double)SelectPointSize / 2);
							int y = Convert.ToInt32(point.Y - (double)SelectPointSize / 2);
							fp.DrawCircle(
								new Rectangle(x, y, SelectPointSize, SelectPointSize),
								Color.White);
						}
					}
				}
			}
		}

		/// <inheritdoc />
		public override void SetSelectPoint(PreviewPoint point = null)
		{
			if (point == null)
			{

				_p1Start = new PreviewPoint((int)Bounds.X, (int)Bounds.Y);
			}

			_selectionPoint = PointInDragPoints(point);
		}


		public override PreviewPoint PointInSelectPoint(PreviewPoint point)
		{
			if (_dragPoints != null)
			{
				var p = PointInDragPoints(point);
				return p == null ? null : point;
			}
			return null;
		}

		private PreviewDoublePoint PointInDragPoints(PreviewPoint point)
		{
			if (_dragPoints != null && point != null)
			{
				var halfSize = SelectPointSize / 2d;
				foreach (PreviewDoublePoint selectPoint in _dragPoints)
				{
					if (point.X >= selectPoint.X - halfSize &&
						point.Y >= selectPoint.Y - halfSize &&
						point.X <= selectPoint.X + halfSize &&
						point.Y <= selectPoint.Y + halfSize)
					{
						return selectPoint;
					}		
				}
			}
			return null;
		}

		/// <inheritdoc />
		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			var rt = new RotateTransform(-RotationAngle, _rotationCenter.X, _rotationCenter.Y);
			var point = rt.TransformPoint(x, y);
			
			//See if we're resizing
			if (_selectionPoint != null && (_selectionPoint.PointType == PreviewPoint.PointTypes.Size || _selectionPoint.PointType == PreviewPoint.PointTypes.Rotate))
			{
				//We are resizing, so figure our change and scale the points.
				if (_selectionPoint == _dragPoints[0])
				{
					//Top Left
					var newHeight = Bounds.Height + (Bounds.TopLeft.Y - point.Y);
					var newWidth = Bounds.Width + (Bounds.TopLeft.X - point.X);

					var minSize = ClampSize();
					var scaleY = newHeight > minSize ? newHeight / Bounds.Height : 1;
					var scaleX = newWidth > minSize ? newWidth / Bounds.Width : 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = MaxValue(scaleX, scaleY);
					}
					Layout();

					Scale(scaleX, scaleY, ZoomCoordToOriginal(_dragPoints[2].X), ZoomCoordToOriginal(_dragPoints[2].Y));

					_selectionPoint = _dragPoints[0];

				}else if (_selectionPoint == _dragPoints[1])
				{
					//Top Right
					var newHeight = Bounds.Height + (Bounds.TopRight.Y - point.Y);
					var newWidth = Bounds.Width + (point.X - Bounds.TopRight.X);

					var minSize = ClampSize();
					var scaleY = newHeight > minSize ? newHeight / Bounds.Height : 1;
					var scaleX = newWidth > minSize ? newWidth / Bounds.Width : 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = MaxValue(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, ZoomCoordToOriginal(_dragPoints[3].X), ZoomCoordToOriginal(_dragPoints[3].Y));
					Layout();

					_selectionPoint = _dragPoints[1];

				}
				else if (_selectionPoint == _dragPoints[2])
				{
					//Bottom Right
					var newHeight = Bounds.Height + (point.Y - Bounds.BottomRight.Y);
					var newWidth = Bounds.Width + (point.X - Bounds.BottomRight.X);

					var minSize = ClampSize();
					var scaleY = newHeight > minSize ? newHeight / Bounds.Height:1;
					var scaleX = newWidth > minSize ? newWidth / Bounds.Width:1;
					
					if(Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = MaxValue(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, ZoomCoordToOriginal(_dragPoints[0].X), ZoomCoordToOriginal(_dragPoints[0].Y));
					Layout();

					_selectionPoint = _dragPoints[2];
				}
				else if (_selectionPoint == _dragPoints[3])
				{
					//Bottom Left
					var newHeight = Bounds.Height + (point.Y - Bounds.BottomLeft.Y);
					var newWidth = Bounds.Width + (Bounds.BottomLeft.X - point.X);

					var minSize = ClampSize();
					var scaleY = newHeight > minSize ? newHeight / Bounds.Height : 1;
					var scaleX = newWidth > minSize ? newWidth / Bounds.Width : 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = MaxValue(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, ZoomCoordToOriginal(_dragPoints[1].X), ZoomCoordToOriginal(_dragPoints[1].Y));
					Layout();

					_selectionPoint = _dragPoints[3];
				}else if (_selectionPoint == _dragPoints[4])
				{
					//We are rotating!
					double angle = GetAngle(_rotationCenter, new Point(x, y));

					// Use Detents of 0, 45, 90, 135, 180, 225, 270 and 315 when holding the Shift modifier key down.
					if (Control.ModifierKeys == Keys.Control)
					{
						if (angle >= 22.5 && angle < 67.5) angle = 45;
						else if (angle >= 67.5 && angle < 112.5) angle = 90;
						else if (angle >= 112.5 && angle < 157.5) angle = 135;
						else if (angle >= 157.5 && angle < 202.5) angle = 180;
						else if (angle >= 202.5 && angle < 247.5) angle = 225;
						else if (angle >= 247.5 && angle < 292.5) angle = 270;
						else if (angle >= 292.5 && angle < 337.5) angle = 315;
						else if (angle >= 337.5 || angle < 22.5) angle = 0;
					}

					RotationAngle = (int)Math.Round(angle, MidpointRounding.AwayFromZero);
					_selectionPoint = _dragPoints[4];
				}
			}
			else
			{
				var newX = _p1Start.X + changeX;
				var newY = _p1Start.Y + changeY;
				MoveTo(newX, newY);
			}
		}

		private double ClampSize()
		{
			if (ZoomLevel > 1)
			{
				return 20 * ZoomLevel;
			}

			return 20;
		}

		private Point ZoomPointToOriginal(Point p)
		{
			return new Point(p.X / ZoomLevel, p.Y/ZoomLevel);
		}

		private double ZoomCoordToOriginal(double c)
		{
			return c / ZoomLevel;
		}

		private static double MaxValue(double a, double b)
		{
			if (a >= 0 && b >= 0)
			{
				return Math.Max(a, b);
			}
			if(a < 0 && b < 0)
			{
				return Math.Min(a, b);
			}
			if(Math.Abs(a) > Math.Abs(b))
			{
				return a;
			}

			return b;
		}

		/// <summary>
		/// Fetches angle relative to screen center point
		/// </summary>
		/// <param name="screenPoint"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		public double GetAngle(Point screenPoint, Point center)
		{
			double dx = screenPoint.X - center.X;
			double dy = screenPoint.Y - center.Y;

			double inRads = Math.Atan2(dy, dx);
			
			// Convert from radians and adjust the angle from the 0 at 9:00 positon.
			return (inRads * (180 / Math.PI) + 270) % 360;
		}

		/// <inheritdoc />
		public override void Layout()
		{
			if (ZoomLevel != 1 || RotationAngle != 0)
			{
				Pixels = PropPixels.Select(x => x.Clone()).ToList();
				if (ZoomLevel != 1)
				{
					Zoom();
				}

				UpdateBounds();
				
				if (RotationAngle != 0)
				{
					Rotate();
				}
			}
			else
			{
				Pixels = PropPixels;
				UpdateBounds();
			}
		}

		private void Zoom()
		{
			if (ZoomLevel != 1)
			{
				foreach (PreviewPixel pixel in Pixels)
				{
					var x = pixel.Location.X * ZoomLevel;
					var y = pixel.Location.Y * ZoomLevel;
					pixel.Location = new Point(x, y);
				}
			}
			
		}

		private void Rotate()
		{
			var t = new RotateTransform(RotationAngle, _rotationCenter.X, _rotationCenter.Y);
			foreach (var previewPixel in Pixels)
			{
				var point = t.TransformPoint(previewPixel.Location.X, previewPixel.Location.Y);
				previewPixel.Location = point;
			}
		}

		private void RotateDragPoints()
		{
			RotateTransform t = new RotateTransform(RotationAngle, _rotationCenter.X, _rotationCenter.Y);
			for (int i = 0; i < _dragPoints.Count; i++)
			{
				var p = t.TransformPoint(_dragPoints[i].X, _dragPoints[i].Y);
				_dragPoints[i].X = p.X;
				_dragPoints[i].Y = p.Y;
			}
		}

		private void Scale(double scaleX, double scaleY, double centerX, double centerY)
		{
			//Adjust for the zoom level.
			var t = new ScaleTransform(scaleX, scaleY, centerX, centerY);
			foreach (var previewPixel in PropPixels)
			{
				var point = t.Transform(new Point(previewPixel.Location.X, previewPixel.Location.Y));
				previewPixel.Location = point;
			}

			Layout();
		}

		/// <inheritdoc />
		public override bool PointInShape(PreviewPoint point)
		{
			var rt = new RotateTransform(-RotationAngle, _rotationCenter.X, _rotationCenter.Y);
			var tp = rt.TransformPreviewPoint(point);
			return Bounds.Contains(tp);
		}

		/// <inheritdoc />
		public override void SelectDefaultSelectPoint()
		{
			
		}

		public override bool ShapeAllInRect(Rectangle rect)
		{
			PreviewPoint p1 = PointToZoomPoint(new PreviewPoint(rect.X, rect.Y));
			PreviewPoint p2 = PointToZoomPoint(new PreviewPoint(rect.X + rect.Width, rect.Y + rect.Height));
			int X1 = Math.Min(p1.X, p2.X);
			int X2 = Math.Max(p1.X, p2.X);
			int Y1 = Math.Min(p1.Y, p2.Y);
			int Y2 = Math.Max(p1.Y, p2.Y);
			
			var rt = new RotateTransform(RotationAngle, _rotationCenter.X, _rotationCenter.Y);
			var b = rt.TransformBounds(Bounds);

			return (b.Top >= Y1 && b.Bottom <= Y2 && b.Left >= X1 && b.Right <= X2);
		}

		/// <inheritdoc />
		public override void MoveTo(int x, int y)
		{
			int xOffset = x - (int)Bounds.X;
			int yOffset = y - (int)Bounds.Y;
			foreach (var previewPixel in PropPixels)
			{
				var p = new Point(previewPixel.Location.X + xOffset, previewPixel.Location.Y + yOffset);
				previewPixel.Location = p;
			}

			Layout();
		}

		/// <inheritdoc />
		public override void Resize(double aspect)
		{
			foreach (var previewPixel in PropPixels)
			{
				var p = new Point(previewPixel.Location.X * aspect, previewPixel.Location.Y * aspect);
				previewPixel.Location = p;
			}

			Layout();
		}

		/// <inheritdoc />
		public override void ResizeFromOriginal(double aspect)
		{
			throw new NotImplementedException();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			if (_dragPoints == null)
			{
				_dragPoints = new List<PreviewDoublePoint>();
			}

			_zoomLevel = 1;
			Pixels = PropPixels;

			Layout();
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newProp = (PreviewCustomProp) MemberwiseClone();
			newProp.PropPixels = new List<PreviewPixel>();

			foreach (PreviewPixel pixel in PropPixels)
			{
				var p = pixel.Clone();
				newProp.PropPixels.Add(p);
			}

			newProp.Pixels = PropPixels;

			return newProp;
		}

		#endregion
	}

	public static class Extensions
	{
		public static Point TransformPreviewPoint(this RotateTransform @this, PreviewPoint point)
		{
			var p = new Point(point.X, point.Y);
		
			return @this.Transform(p);
		}

		public static Point TransformPoint(this RotateTransform @this, double x, double y)
		{
			var p = new Point(x, y);

			return @this.Transform(p);
		}

	}
}
