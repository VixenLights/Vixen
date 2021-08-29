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
	public class PreviewFlood : PreviewLightBaseShape
	{
		[DataMember] private PreviewPoint _p1;
		private PreviewPoint p1Start;

		public override string TypeName => @"Flood";

		public PreviewFlood(PreviewPoint point, ElementNode selectedNode)
		{
			_p1 = point;

			PixelSize = 40;

			StringType = StringTypes.Standard;

			PreviewPixel pixel = AddPixel(10, 10);
			pixel.PixelColor = Color.White;

			if (selectedNode != null) {
				if (selectedNode.IsLeaf) {
					pixel.Node = selectedNode;
					pixel.NodeId = selectedNode.Id;
				}
			}

			// Lay out the flood
			Layout();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			StringType = StringTypes.Standard;
			Layout();
		}

		[Browsable(false)]
		public override StringTypes StringType
		{
			get { return StringTypes.Standard; }
			set { _stringType = value; }
		}

		[DataMember,
		 CategoryAttribute("Settings"),
		 DescriptionAttribute("The size of the light point on the preview."),
		 DisplayName("Light Size")]
		public override int PixelSize
		{
			get { return _pixelSize; }
			set
			{
				_pixelSize = value;
				if (_pixels != null && _pixels.Count > 0)
					_pixels[0].PixelSize = 3;
			}
		}

		[CategoryAttribute("Position"),
		 DisplayName("Point 1"),
		 DescriptionAttribute("Spot lights are defined by 1 point.")]
		public Point Point1
		{
			get
			{
				Point p = new Point(_p1.X, _p1.Y);
				return p;
			}
			set
			{
				_p1.X = value.X;
				_p1.Y = value.Y;
				Layout();
			}
		}

        public override int Top
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Left
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Right
        {
			get
			{
				throw new NotImplementedException();
			}
        }

        public override int Bottom
        {
			get
			{
				throw new NotImplementedException();
			}
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewFlood shape = (matchShape as PreviewFlood);
            PixelSize = shape.PixelSize;
            Layout();
        }

		public override void Layout()
		{
			if (_p1 == null) {
				_p1 = new PreviewPoint();
				_p1.X = 100;
				_p1.Y = 100;
			}
			PreviewPixel pixel = Pixels[0];
			pixel.X = _p1.X;
			pixel.Y = _p1.Y;
			pixel.PixelSize = 3;

			SetPixelZoom();
		}

		public override void Draw(Bitmap b, bool editMode, HashSet<Guid> highlightedElements)
		{
			Graphics g = Graphics.FromImage(b);
			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

			if (!highlightedElements.Contains(_pixels[0].NodeId)) 
			{
				SolidBrush brush;
				if (editMode) {
					//Image img = Properties.Resources.FloodLight;
					//Rectangle dstRect = new Rectangle(_pixels[0].X - (PixelSize / 2), _pixels[0].Y - (PixelSize / 2), PixelSize, PixelSize);
					//Rectangle srcRect = new Rectangle(0, 0, img.Width, img.Height);
					//g.DrawImage(img, dstRect, srcRect, GraphicsUnit.Pixel);
					brush = new SolidBrush(Color.FromArgb(0, 255, 255, 255));
					g.FillEllipse(brush, _pixels[0].X - (PixelSize/2), _pixels[0].Y - (PixelSize/2), PixelSize, PixelSize);
				}
				else {
					brush = new SolidBrush(_pixels[0].PixelColor);
					g.FillEllipse(brush, _pixels[0].X - (PixelSize/2), _pixels[0].Y - (PixelSize/2), PixelSize, PixelSize);
				}
			}
		}

		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                          bool forceDraw, double zoomLevel)
		{
			foreach (PreviewPixel pixel in Pixels) {
				//if (highlightedElements != null && highlightedElements.Contains(pixel.Node))
				//    pixel.Draw(fp, Color.HotPink);
				//else
				//    pixel.Draw(fp, Color.White);
				DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
			}
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
				_p1.X = p1Start.X + changeX;
				_p1.Y = p1Start.Y + changeY;
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
			points.Add(_p1);
			//_p2.X = _p1.X;
			//_p2.Y = _p1.Y;
			//points.Add(_p2);
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
			//if (PixelSize < 5)
			//{
			Rectangle r = new Rectangle(_p1.X - (PixelSize/2), _p1.Y - (PixelSize/2), PixelSize, PixelSize);
			if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
				return true;
			}
			//}
			//else
			//{
			//    if (_p1 != null)
			//    {
			//        if (point.X >= _p1.X && point.X <= _p1.X + PixelSize && point.Y >= _p1.Y && point.Y <= _p1.Y + PixelSize)
			//        {
			//            return true;
			//        }
			//    }
			//}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(_p1.X, _p1.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _p1;
		}

		public override void MoveTo(int x, int y)
		{
			_p1.X = x;
			_p1.Y = y;
			Layout();
		}

		public override void Resize(double aspect)
		{
			_p1.X = (int) (_p1.X*aspect);
			_p1.Y = (int) (_p1.Y*aspect);
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_p1.X = p1Start.X;
			_p1.Y = p1Start.Y;
			Resize(aspect);
		}
	}
}