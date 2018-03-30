using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Win32.SafeHandles;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Model;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewCustomProp : PreviewBaseShape
	{
		private List<PreviewPoint> _dragPoints = new List<PreviewPoint>();
		private PreviewPoint p1Start, p2Start;
		private double _zoomLevel;

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
		public Rectangle Bounds { get; private set; }

		internal void UpdateBounds()
		{
			var rects = Pixels.Select(p => p.Bounds);
			Bounds = GetCombinedBounds(rects);
			if (Selected)
			{
				ConfigureDragPoints();
			}
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		[Browsable(true)]
		[Category("Location")]
		[DisplayName("Y")]
		public override int Top
		{
			get { return Bounds.Top; }
			set
			{
				MoveTo(Bounds.X, value);
			}

		}

		/// <inheritdoc />
		public override int Bottom => Bounds.Bottom;

		/// <inheritdoc />
		[Browsable(true)]
		[Category("Location")]
		[DisplayName("X")]
		public override int Left
		{
			get { return Bounds.Left; }
			set
			{
				MoveTo(value, Bounds.Y);
			}
		}

		/// <inheritdoc />
		public override int Right => Bounds.Right;

		/// <inheritdoc />
		public override void Match(PreviewBaseShape matchShape)
		{
			//Match size for now
			var yAspect =  (matchShape.Bottom - matchShape.Top) / (double)(Bottom - Top);
			var xAspect = (matchShape.Right - matchShape.Left) / (double)(Right - Left);
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
				if (PropPixels.Any())
				{
					Zoom();
				}
			}
		}

		/// <inheritdoc />
		public override void Layout()
		{
			if (Pixels == null)
			{
				Zoom();
			}
			else
			{
				UpdateBounds();
			}
		}

		/// <inheritdoc />
		public override void SelectDragPoints()
		{   
			ConfigureDragPoints();
			SetSelectPoints(_dragPoints, null);
		}

		private void ConfigureDragPoints()
		{
			_dragPoints.Clear();
			_dragPoints.Add(new PreviewPoint(Bounds.Left, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Right, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Right, Bounds.Bottom, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Left, Bounds.Bottom, PreviewPoint.PointTypes.Size));
		}

		public override void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_selectPoints != null)
			{
				foreach (PreviewPoint point in _selectPoints)
				{
					if (point?.PointType == PreviewPoint.PointTypes.Size)
					{
						int x = Convert.ToInt32(point.X - SelectPointSize / 2);
						int y = Convert.ToInt32(point.Y - SelectPointSize / 2);
						fp.DrawRectangle(
							new Rectangle(x, y, SelectPointSize, SelectPointSize),
							Color.White);
					}
				}
			}
		}

		public override PreviewPoint PointInSelectPoint(PreviewPoint point)
		{
			if (_selectPoints != null)
			{
				foreach (PreviewPoint selectPoint in _selectPoints)
				{
					if (selectPoint != null)
					{
						if (point.X >= selectPoint.X - (SelectPointSize / 2) &&
						    point.Y >= selectPoint.Y - (SelectPointSize / 2) &&
						    point.X <= selectPoint.X + (SelectPointSize / 2) &&
						    point.Y <= selectPoint.Y + (SelectPointSize / 2))
						{
							return selectPoint;
						}
					}
				}
			}
			return null;
		}

		/// <inheritdoc />
		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = new PreviewPoint(x, y);
			//See if we're resizing
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size)
			{
				//We are resizing, so figure our change and scale the points.
				if (_selectedPoint == _dragPoints[0])
				{
					//Top Left
					var scaleY = -(point.Y - Bounds.Top) / (double)Bounds.Height;
					var scaleX = -(point.X - Bounds.Left) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = Math.Max(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, Bounds.Right, Bounds.Bottom);
					
					_selectedPoint = _dragPoints[0];

				}else if (_selectedPoint == _dragPoints[1])
				{
					//Top Right
					var scaleY = -(point.Y - Bounds.Top)  / (double)Bounds.Height;
					var scaleX = (point.X - Bounds.Right) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = Math.Max(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, Bounds.Left, Bounds.Bottom);
					
					_selectedPoint = _dragPoints[1];

				}
				else if (_selectedPoint == _dragPoints[2])
				{
					//Bottom Right
					var scaleY = (point.Y - Bounds.Bottom) / (double)Bounds.Height;
					var scaleX = (point.X - Bounds.Right) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					if(Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = Math.Max(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, Bounds.Left, Bounds.Top);
					
					_selectedPoint = _dragPoints[2];
				}
				else if (_selectedPoint == _dragPoints[3])
				{
					//Bottom Left
					var scaleY = (point.Y - Bounds.Bottom) / (double)Bounds.Height;
					var scaleX = -(point.X - Bounds.Left) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					if (Control.ModifierKeys == Keys.Shift)
					{
						scaleX = scaleY = Math.Max(scaleX, scaleY);
					}

					Scale(scaleX, scaleY, Bounds.Right, Bounds.Top);
					
					_selectedPoint = _dragPoints[3];
				}
				
			}
			// If we get here, we're moving
			else
			{
				var newX = p1Start.X + changeX;
				var newY = p1Start.Y + changeY;

				MoveTo(newX, newY);
			}
		}

		private void Zoom()
		{
			if (ZoomLevel == 1)
			{
				Pixels = PropPixels;
			}
			else
			{
				Pixels = PropPixels.Select(x => x.Clone()).ToList();
				foreach (PreviewPixel pixel in Pixels)
				{
					var x = pixel.Location.X * ZoomLevel;
					var y = pixel.Location.Y * ZoomLevel;
					pixel.Location = new Point(x, y);
				}
			}

			UpdateBounds();
		}

		private void Scale(double scaleX, double scaleY, int centerX, int centerY)
		{
			var t = new ScaleTransform(scaleX, scaleY, centerX, centerY);
			foreach (var previewPixel in PropPixels)
			{
				var point = t.Transform(new Point(previewPixel.Location.X, previewPixel.Location.Y));
				previewPixel.Location = point;
			}

			if (ZoomLevel != 1)
			{
				foreach (var previewPixel in Pixels)
				{
					var point = t.Transform(new Point(previewPixel.Location.X, previewPixel.Location.Y));
					previewPixel.Location = point;
				}
			}
			
			UpdateBounds();
		}

		/// <inheritdoc />
		public override bool PointInShape(PreviewPoint point)
		{
			return Bounds.Contains(point.ToPoint());
		}

		/// <inheritdoc />
		public override void SetSelectPoint(PreviewPoint point = null)
		{
			if (point == null)
			{
				p1Start = new PreviewPoint(Bounds.X, Bounds.Y);
				p2Start = new PreviewPoint(Bounds.Right, Bounds.Bottom);
			}

			_selectedPoint = point;
		}

		/// <inheritdoc />
		public override void SelectDefaultSelectPoint()
		{
			
		}

		/// <inheritdoc />
		public override void MoveTo(int x, int y)
		{
			int xOffset = x - Bounds.X;
			int yOffset = y - Bounds.Y;
			foreach (var previewPixel in PropPixels)
			{
				var p = new Point(previewPixel.Location.X + xOffset, previewPixel.Location.Y + yOffset);
				previewPixel.Location = p;
			}

			if (ZoomLevel != 1)
			{
				foreach (var previewPixel in Pixels)
				{
					var p = new Point(previewPixel.Location.X + xOffset, previewPixel.Location.Y + yOffset);
					previewPixel.Location = p;
				}
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

			if (ZoomLevel != 1)
			{
				foreach (var previewPixel in Pixels)
				{
					var p = new Point(previewPixel.Location.X * aspect, previewPixel.Location.Y * aspect);
					previewPixel.Location = p;
				}
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
				_dragPoints = new List<PreviewPoint>();
			}

			_zoomLevel = 1;
			Pixels = PropPixels;

			Layout();
		}
	}
}
