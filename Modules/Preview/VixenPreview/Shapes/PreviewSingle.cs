using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewSingle : PreviewBaseShape
	{
		[DataMember] private PreviewPoint p1;

		private PreviewPoint p1Start;

		public PreviewSingle(PreviewPoint point, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			p1 = PointToZoomPoint(point); 

			PreviewPixel pixel = AddPixel(10, 10);
			pixel.PixelColor = Color.White;

			if (selectedNode != null) {
				if (selectedNode.IsLeaf) {
					pixel.Node = selectedNode;
				}
			}

			// Lay out the pixels
			Layout();
		}

		public override string TypeName => @"Single";

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		[CategoryAttribute("Position"),
		 DisplayName("Position"),
		 DescriptionAttribute("An point is defined by a point on the screen.")]
		public Point Point1
		{
			get
			{
				Point p = new Point(p1.X, p1.Y);
				return p;
			}
			set
			{
				p1.X = value.X;
				p1.Y = value.Y;
				Layout();
			}
		}

		// For now we don't want this to show. We might delete it later if it is not used
		[Browsable(false)]
		public int MaxAlpha
		{
			get { return _pixels[0].MaxAlpha; }
			set { _pixels[0].MaxAlpha = value; }
		}

        public override int Top
        {
            get
            {
                return p1.Y;
            }
            set
            {
                p1.Y = value;
            }
        }

        public override int Left
        {
            get
            {
                return p1.X;
            }
            set
            {
                p1.X = value;
            }
        }

        public override int Right
        {
            get 
            {
                return p1.X;
			}
        }

        public override int Bottom
        {
            get 
            { 
                return p1.Y;
			}
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewSingle shape = (matchShape as PreviewSingle);
            PixelSize = shape.PixelSize;
            Layout();
        }

		public override void Layout()
		{
			if (Pixels != null && Pixels.Count > 0) {
			PreviewPixel pixel = Pixels[0];
			pixel.X = p1.X;
			pixel.Y = p1.Y;

			SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			if (_selectedPoint != null)
			{
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
				//p1.X = p1Start.X + changeX;
				//p1.Y = p1Start.Y + changeY;

				p1.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				p1.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(p1);

				Layout();
			}
		}

		//private void OnResize(EventArgs e)
		//{
		//    Layout();
		//}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			points.Add(p1);
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			var p = PointToZoomPoint(point);
			if (PixelSize < 5)
			{
				Rectangle r = new Rectangle(p1.X - SelectPointSize / 2, p1.Y - SelectPointSize / 2, SelectPointSize, SelectPointSize);
				if (p.X >= r.X && p.X <= r.X + r.Width && p.Y >= r.Y && p.Y <= r.Y + r.Height) {
					return true;
				}
			}
			else {
				if (p1 != null) {
					if (p.X >= p1.X-PixelSize/2 && p.X <= p1.X + PixelSize/2 && p.Y >= p1.Y-PixelSize/2 && p.Y <= p1.Y + PixelSize/2) {
						return true;
					}
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(p1.X, p1.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = p1;
		}

		public override void MoveTo(int x, int y)
		{
			p1.X = x;
			p1.Y = y;
			Layout();
		}

		public override void Resize(double aspect)
		{
			p1.X = (int) (p1.X*aspect);
			p1.Y = (int) (p1.Y*aspect);
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			p1.X = p1Start.X;
			p1.Y = p1Start.Y;
			Resize(aspect);
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newSingle = (PreviewSingle) MemberwiseClone();
			newSingle.p1 = p1.Copy();
			return newSingle;
		}

		#endregion
	}
}