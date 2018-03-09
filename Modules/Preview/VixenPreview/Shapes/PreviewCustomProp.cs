using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Model;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewCustomProp : PreviewBaseShape
	{

		public Rectangle Bounds { get; private set; }
		private List<PreviewPoint> _dragPoints = new List<PreviewPoint>();
		private PreviewPoint p1Start, p2Start;

		public void AddLightNodes(ElementModel model, ElementNode node)
		{
			if (!model.IsLightNode) return;

			foreach (var light in model.Lights)
			{
				PreviewPixel p = AddHighPrecisionPixel(new Point(light.X, light.Y), model.LightSize);
				p.PixelColor = Color.White;
				p.Node = node;
				p.SerializeCoordinates = true;
			}
		}

		internal void UpdateBounds()
		{
			var rects = Pixels.Select(p => p.Bounds);
			Bounds = GetCombinedBounds(rects);
			SelectDragPoints();
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
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
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override void Layout()
		{
			UpdateBounds();
			//SetPixelZoom();
		}

		/// <inheritdoc />
		public override void SelectDragPoints()
		{
			_dragPoints.Clear();
			_dragPoints.Add(new PreviewPoint(Bounds.Left, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Right, Bounds.Top, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Right, Bounds.Bottom, PreviewPoint.PointTypes.Size));
			_dragPoints.Add(new PreviewPoint(Bounds.Left, Bounds.Bottom, PreviewPoint.PointTypes.Size));
			// Tell the base shape about the newely created points            
			SetSelectPoints(_dragPoints, null);
		}

		/// <inheritdoc />
		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
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

					Scale(scaleX, scaleY, Bounds.Right, Bounds.Bottom);
					Layout();

					_selectedPoint = _dragPoints[0];

				}else if (_selectedPoint == _dragPoints[1])
				{
					//Top Right
					var scaleY = -(point.Y - Bounds.Top)  / (double)Bounds.Height;
					var scaleX = (point.X - Bounds.Right) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					Scale(scaleX, scaleY, Bounds.Left, Bounds.Bottom);
					Layout();

					_selectedPoint = _dragPoints[1];

				}
				else if (_selectedPoint == _dragPoints[2])
				{
					//Bottom Right
					var scaleY = (point.Y - Bounds.Bottom) / (double)Bounds.Height;
					var scaleX = (point.X - Bounds.Right) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					Scale(scaleX, scaleY, Bounds.Left, Bounds.Top);
					Layout();

					_selectedPoint = _dragPoints[2];
				}
				else if (_selectedPoint == _dragPoints[3])
				{
					//Bottom Left
					var scaleY = (point.Y - Bounds.Bottom) / (double)Bounds.Height;
					var scaleX = -(point.X - Bounds.Left) / (double)Bounds.Width;

					scaleY += 1;
					scaleX += 1;

					Scale(scaleX, scaleY, Bounds.Right, Bounds.Top);
					Layout();

					_selectedPoint = _dragPoints[3];
				}
				
			}
			// If we get here, we're moving
			else
			{
				var newX = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				var newY = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;

				MoveTo(newX, newY);
				
				PointToZoomPointRef(new PreviewPoint(Bounds.Location));
				PointToZoomPointRef(new PreviewPoint(Bounds.Right, Bounds.Bottom));
				
				Layout();

			}

			
		}

		private void Scale(double scaleX, double scaleY, int centerX, int centerY)
		{
			var t = new ScaleTransform(scaleX, scaleY, centerX, centerY);
			foreach (var previewPixel in Pixels)
			{
				var point = t.Transform(new Point(previewPixel.Location.X, previewPixel.Location.Y));
				previewPixel.Location = point;
			}
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
			foreach (var previewPixel in Pixels)
			{
				var p = new Point(previewPixel.Location.X + xOffset, previewPixel.Location.Y + yOffset);
				previewPixel.Location = p;
			}

			UpdateBounds();
		}

		/// <inheritdoc />
		public override void Resize(double aspect)
		{
			foreach (var previewPixel in Pixels)
			{
				var p = new Point(previewPixel.Location.X * aspect, previewPixel.Location.Y * aspect);
				previewPixel.Location = p;
			}
			UpdateBounds();
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
			Layout();
		}
	}
}
