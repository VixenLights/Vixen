using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewArch: PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _topLeft;
        [DataMember]
        private PreviewPoint _bottomRight;

        private PreviewPoint topRight = new PreviewPoint(10, 10);
        private PreviewPoint bottomLeft = new PreviewPoint(10, 10);

        private PreviewPoint p1Start, p2Start;

        public PreviewArch()
        {
        }

        public PreviewArch(PreviewPoint point1, ElementNode selectedNode)
        {
            _topLeft = point1;
            _bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

            int lightCount = 25;

            //// Just add the pixels, they will get layed out next
            //for (int lightNum = 0; lightNum < 25; lightNum++)
            //{
            //    PreviewPixel pixel = AddPixel(10, 10);
            //    pixel.PixelColor = Color.White;
            //}
            
            if (selectedNode != null)
            {
                List<ElementNode> children = selectedNode.Children.ToList();
                // is this a single node?
                if (children.Count >= 4)
                {
                    StringType = StringTypes.Pixel;
                    lightCount = children.Count;
                    // Just add the pixels, they will get layed out next
                    foreach (ElementNode child in children)
                    {
                        {
                            PreviewPixel pixel = AddPixel(10, 10);
                            pixel.Node = child;
                            pixel.NodeId = child.Id;
                            pixel.PixelColor = Color.White;
                        }
                    }
                }
            }

            if (_pixels.Count == 0)
            {
                // Just add the pixels, they will get layed out next
                for (int lightNum = 0; lightNum < lightCount; lightNum++)
                {
                    PreviewPixel pixel = AddPixel(10, 10);
                    pixel.PixelColor = Color.White;
                    if (selectedNode != null)
                    {
                        pixel.Node = selectedNode;
                        pixel.NodeId = selectedNode.Id;
                    }
                }
            }




            // Lay out the pixels
            Layout();

            DoResize += new ResizeEvent(OnResize);
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            Layout();
        }

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is point 1.")]
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
        DescriptionAttribute("An arch is defined by a 2 points of a rectangle. This is point 2.")]
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
        DisplayName("Light Count"),
        DescriptionAttribute("Number of pixels or lights in the arch.")]
        public int PixelCount
        {
            get { return Pixels.Count; }
            set
            {
                while (Pixels.Count > value)
                {
                    Pixels.RemoveAt(Pixels.Count - 1);
                }
                while (Pixels.Count < value)
                {
                    PreviewPixel pixel = new PreviewPixel(10, 10, PixelSize);
                    Pixels.Add(pixel);
                }
                Layout();
            }
        }

        public override void Layout()
        {
            int width = _bottomRight.X - _topLeft.X;
            int height = _bottomRight.Y - _topLeft.Y;
            List<Point> points;
            points = PreviewTools.GetArcPoints(width, height, PixelCount);
            //points = PreviewTools.GetEllipsePoints(0, 0, width/2, height/2, PixelCount*2, 180);
            int pointNum = 0;
            foreach (PreviewPixel pixel in _pixels)
            {
                //if (pointNum < points.Count)
                //{
                    pixel.X = points[pointNum].X + _topLeft.X;
                    pixel.Y = points[pointNum].Y + _topLeft.Y;
                    pointNum++;
                //}
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY) 
        {
            // See if we're resizing
            if (_selectedPoint != null)
            {
                if (_selectedPoint == topRight)
                {
                    _topLeft.Y = y;
                    _bottomRight.X = x;
                }
                else if (_selectedPoint == bottomLeft)
                {
                    _topLeft.X = x;
                    _bottomRight.Y = y;
                }
                _selectedPoint.X = x;
                _selectedPoint.Y = y;
                Layout();
                //SelectDragPoints();
            }
            // If we get here, we're moving
            else
            {
                _topLeft.X = p1Start.X + changeX;
                _topLeft.Y = p1Start.Y + changeY;
                _bottomRight.X = p2Start.X + changeX;
                _bottomRight.Y = p2Start.Y + changeY;
                Layout();
            }

            topRight.X = _bottomRight.X;
            topRight.Y = _topLeft.Y;
            bottomLeft.X = _topLeft.X;
            bottomLeft.Y = _bottomRight.Y;
        }

        private void OnResize(EventArgs e)
        {
            Layout();
        }

        public override void Select() 
        {
            base.Select();
            SelectDragPoints();
        }

        private void SelectDragPoints()
        {
            List<PreviewPoint> points = new List<PreviewPoint>();
            points.Add(_topLeft);
            points.Add(_bottomRight);
            topRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
            points.Add(topRight);
            bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
            points.Add(bottomLeft);
            SetSelectPoints(points, null);
        }

        public override bool PointInShape(PreviewPoint point)
        {
            foreach (PreviewPixel pixel in Pixels) 
            {
                Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2), SelectPointSize, SelectPointSize);
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
                p1Start = new PreviewPoint(_topLeft.X, _topLeft.Y);
                p2Start = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
            }
            _selectedPoint = point;
        }

        public override void SelectDefaultSelectPoint()
        {
            _selectedPoint = _bottomRight;
        }

        public override object Clone()
        {
            //PreviewArch arch = (PreviewArch)this.MemberwiseClone();
            //arch.Pixels = new List<PreviewPixel>();
            //foreach (PreviewPixel pixel in Pixels)
            //{
            //}
            //return arch;
            return this.MemberwiseClone();
        }

        public override void MoveTo(int x, int y)
        {
            Point topLeft = new Point();
            topLeft.X = Math.Min(TopLeft.X, BottomRight.X);
            topLeft.Y = Math.Min(TopLeft.Y, BottomRight.Y);

            int deltaX = x - topLeft.X;
            int deltaY = y - topLeft.Y;

            TopLeft = new Point(TopLeft.X + deltaX, TopLeft.Y + deltaY);
            BottomRight = new Point(BottomRight.X + deltaX, BottomRight.Y + deltaY);

            topRight.X = _bottomRight.X;
            topRight.Y = _topLeft.Y;
            bottomLeft.X = _topLeft.X;
            bottomLeft.Y = _bottomRight.Y;
            
            Layout();
        }

        public override void Resize(double aspect)
        {
            TopLeft = new Point((int)(TopLeft.X * aspect), (int)(TopLeft.Y * aspect));
            BottomRight = new Point((int)(BottomRight.X * aspect), (int)(BottomRight.Y * aspect));
            //topRight.X = _bottomRight.X;
            //topRight.Y = _topLeft.Y;
            //bottomLeft.X = _topLeft.X;
            //bottomLeft.Y = _bottomRight.Y;
            Layout();
        }

    }
}
