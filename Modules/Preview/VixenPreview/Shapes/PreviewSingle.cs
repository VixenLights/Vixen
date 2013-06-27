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
		//[DataMember]
		//private PreviewPoint p2;
		//private int _maxAlpha;

		private PreviewPoint p1Start;
		//, p2Start;

		public PreviewSingle(PreviewPoint point, ElementNode selectedNode)
		{
			p1 = point; // new PreviewPoint(point.X, point.Y);
			//p2 = new PreviewPoint(point.X, point.Y);

			PreviewPixel pixel = AddPixel(10, 10);
			pixel.PixelColor = Color.White;

			if (selectedNode != null) {
				if (selectedNode.IsLeaf) {
					pixel.Node = selectedNode;
					//pixel.NodeId = selectedNode.Id;
				}
			}

			// Lay out the pixels
			Layout();

			//DoResize += new ResizeEvent(OnResize);
		}

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

		public override void Layout()
		{
			PreviewPixel pixel = Pixels[0];
			pixel.X = p1.X;
			pixel.Y = p1.Y;
			//PixelSize = Math.Abs(p2.X - p1.X);
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			if (_selectedPoint != null) {
				_selectedPoint.X = x;
				_selectedPoint.Y = y;
				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
				p1.X = p1Start.X + changeX;
				p1.Y = p1Start.Y + changeY;
				//p2.X = p2Start.X + changeX;
				//p2.Y = p2Start.Y + changeY;
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
			//p2.X = p1.X + PixelSize;
			//p2.Y = p1.Y + PixelSize;
			//points.Add(p2);
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			//foreach (PreviewPixel pixel in Pixels) 
			//{
			//    Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2), SelectPointSize, SelectPointSize);
			//    if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height)
			//    {
			//        return true;
			//    }
			//}
			//if (point == null)
			//    MessageBox.Show("point");
			//if (p1 == null)
			//    MessageBox.Show("p1");
			if (PixelSize < 5) {
				Rectangle r = new Rectangle(p1.X - (SelectPointSize/2), p1.Y - (SelectPointSize/2), SelectPointSize, SelectPointSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
					return true;
				}
			}
			else {
				if (p1 != null) {
					if (point.X >= p1.X && point.X <= p1.X + PixelSize && point.Y >= p1.Y && point.Y <= p1.Y + PixelSize) {
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
				//p2Start = new PreviewPoint(p2.X, p2.Y);
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
	}
}