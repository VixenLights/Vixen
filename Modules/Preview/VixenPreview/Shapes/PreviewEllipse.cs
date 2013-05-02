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
    public class PreviewEllipse : PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _topLeft;
        [DataMember]
        private PreviewPoint _bottomRight;

        private PreviewPoint topRight, bottomLeft;

        private PreviewPoint skewXPoint = new PreviewPoint(10, 10);
        private PreviewPoint skewYPoint = new PreviewPoint(10, 10);
        
        private PreviewPoint p1Start, p2Start;

        public PreviewEllipse(PreviewPoint point1, int lightCount, ElementNode selectedNode)
        {
            _topLeft = point1;
            _bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);
            //_lightCount = lightCount;

            if (selectedNode != null)
            {
                //List<ElementNode> children = selectedNode.Children.ToList();
                List<ElementNode> children = PreviewTools.GetLeafNodes(selectedNode);
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
                    if (selectedNode != null && selectedNode.IsLeaf)
                    {
                        pixel.Node = selectedNode;
                        //pixel.NodeId = selectedNode.Id;
                    }
                }
            }

            // Lay out the pixels
            Layout();

            //DoResize += new ResizeEvent(OnResize);
        }

        [OnDeserialized]
        new void OnDeserialized(StreamingContext context)
        {
            Layout();
        }

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("An ellipse is defined by a 2 points of a ellipse. This is point 1.")]
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
        DescriptionAttribute("An ellipse is defined by a 2 points of a ellipse. This is point 2.")]
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
        DescriptionAttribute("Number of pixels or lights in the ellipse.")]
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

        public int Height
        {
            set
            {
                _bottomRight.Y = _topLeft.Y + value;
            }
        }

        public int Width
        {
            set
            {
                _bottomRight.X = _topLeft.X + value;
            }
        }

        public override int Top
        {
            set
            {
                _topLeft.Y = value;
            }
        }

        public override int Left
        {
            set
            {
                _topLeft.X = value;
            }
        }

        public override void Layout()
        {
            if (_bottomRight != null && _topLeft != null)
            {
                int width = _bottomRight.X - _topLeft.X;
                int height = _bottomRight.Y - _topLeft.Y;
                List<Point> points;
                points = PreviewTools.GetEllipsePoints(0, 0, width, height, PixelCount, 360, 0);
                int pointNum = 0;
                foreach (PreviewPixel pixel in _pixels)
                {
                    pixel.X = points[pointNum].X + _topLeft.X;
                    pixel.Y = points[pointNum].Y + _topLeft.Y;
                    pointNum++;
                }
                //Skew();
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY)
        {
            //if (_selectedPoint != null)
            //{
                // See if we're resizing
                if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size)
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
                    //SelectDragPoints();
                }
                // If we get here, we're moving
                else if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.SkewNS)
                {
                } 
                else if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.SkewWE)
                {
                } else
                {
                    _topLeft.X = p1Start.X + changeX;
                    _topLeft.Y = p1Start.Y + changeY;
                    _bottomRight.X = p2Start.X + changeX;
                    _bottomRight.Y = p2Start.Y + changeY;
                }

                topRight.X = _bottomRight.X;
                topRight.Y = _topLeft.Y;
                bottomLeft.X = _topLeft.X;
                bottomLeft.Y = _bottomRight.Y;

                // Layout the standard shape
                Layout();

                // Now, we skew the shape
                int width = topRight.X - _topLeft.X;
                int height = bottomLeft.Y - _topLeft.Y;

                // Finally, add the skew points to the shape
                skewXPoint.X = _topLeft.X + (width / 2);
                skewXPoint.Y = topRight.Y;
                skewYPoint.X = topRight.X;
                skewYPoint.Y = topRight.Y + (height / 2);
            //}
        }

        //private void OnResize(EventArgs e)
        //{
        //    Layout();
        //}

        public override void Select()
        {
            base.Select();
            SelectDragPoints();
        }

        private void SelectDragPoints()
        {
            // Create the size points
            List<PreviewPoint> selectPoints = new List<PreviewPoint>();

            selectPoints.Add(_topLeft);
            selectPoints.Add(_bottomRight);
            topRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
            selectPoints.Add(topRight);
            bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
            selectPoints.Add(bottomLeft);

            // Create the skew points
            List<PreviewPoint> skewPoints = new List<PreviewPoint>();

            int width = topRight.X - _topLeft.X;
            skewXPoint = new PreviewPoint(_topLeft.X + (width / 2), topRight.Y);
            skewXPoint.PointType = PreviewPoint.PointTypes.SkewWE;
            skewPoints.Add(skewXPoint);

            int height = bottomLeft.Y - _topLeft.Y;
            skewYPoint = new PreviewPoint(topRight.X, topRight.Y + (height / 2));
            skewYPoint.PointType = PreviewPoint.PointTypes.SkewNS;
            skewPoints.Add(skewYPoint);

            // Tell the base shape about the newely created points
            SetSelectPoints(selectPoints, skewPoints);
        }

        public override bool PointInShape(PreviewPoint point)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2), SelectPointSize + PixelSize, SelectPointSize + PixelSize);
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

        public override void MoveTo(int x, int y)
        {
            int deltaX = x - TopLeft.X;
            int deltaY = y - TopLeft.Y;

            TopLeft = new Point(x, y);
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
