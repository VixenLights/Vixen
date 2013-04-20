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
    public class PreviewCane: PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _topLeftPoint;
        [DataMember]
        private PreviewPoint _bottomRightPoint;
        [DataMember]
        private PreviewPoint _archLeftPoint;
        [DataMember]
        private int _archPixelCount;
        [DataMember]
        private int _linePixelCount;

        private bool justPlaced = false;
        private PreviewPoint bottomRightStart, topLeftStart, archStart;

        public PreviewCane(PreviewPoint point, ElementNode selectedNode)
        {
            _topLeftPoint = point; 
            _bottomRightPoint = new PreviewPoint(point.X, point.Y);
            _archLeftPoint = new PreviewPoint(point.X, point.Y);

            _archPixelCount = 8;
            _linePixelCount = 8;

            int lightCount = _archPixelCount + _linePixelCount;

            if (selectedNode != null)
            {
                List<ElementNode> children = selectedNode.Children.ToList();
                // is this a single node?
                if (children.Count >= 8)
                {
                    StringType = StringTypes.Pixel;
                    _archPixelCount = children.Count / 2;
                    _linePixelCount = children.Count / 2;
                    if (_archPixelCount + _linePixelCount > children.Count)
                    {
                        _archPixelCount -= 1;
                    }
                    else if (_archPixelCount + _linePixelCount < children.Count)
                    {
                        _linePixelCount -= 1;
                    }
                    lightCount = children.Count;
                    // Just add the pixels, they will get layed out next
                    foreach (ElementNode child in children)
                    {
                        PreviewPixel pixel = AddPixel(10, 10);
                        pixel.Node = child;
                        pixel.NodeId = child.Id;
                        pixel.PixelColor = Color.White;
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

            //DoResize += new ResizeEvent(OnResize);
        }

        [OnDeserialized]
        new void OnDeserialized(StreamingContext context)
        {
            Layout();
        }

        [CategoryAttribute("Settings"),
        DisplayName("Line Light Count"),
        DescriptionAttribute("The number of lights in the vertical string of the candy cane.")]
        public int LinePixelCount
        {
            set
            {
                while (_linePixelCount < value)
                {
                    PreviewPixel pixel = AddPixel(10, 10);
                    _linePixelCount++;
                }
                while (_linePixelCount > value)
                {
                    _pixels.RemoveAt(0);
                    _linePixelCount--;
                }
                Layout();
            }
            get
            {
                return _linePixelCount;
            }
        }

        [CategoryAttribute("Settings"),
        DisplayName("Arch Light Count"),
        DescriptionAttribute("The number of lights in the arch of the candy cane.")]
        public int ArchPixelCount
        {
            set
            {
                // Todo: change the number of pixels when this changes
                while (_archPixelCount < value)
                {
                    PreviewPixel pixel = AddPixel(10, 10);
                    _archPixelCount++;
                }
                while (_archPixelCount > value)
                {
                    _pixels.RemoveAt(0);
                    _archPixelCount--;
                }
                Layout();
            }
            get
            {
                return _archPixelCount;
            }
        }

        [CategoryAttribute("Position"),
        DisplayName("Bottom Right"),
        DescriptionAttribute("The bottom right point of bounding box of the candy cane.")]
        public Point BottomRight
        {
            get
            {
                Point p = new Point(_bottomRightPoint.X, _bottomRightPoint.Y);
                return p;
            }
            set
            {
                _bottomRightPoint.X = value.X;
                _bottomRightPoint.Y = value.Y;
                Layout();
            }
        }

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("The top left opint of the bounding box of the candy cane.")]
        public Point TopLeft
        {
            get
            {
                Point p = new Point(_topLeftPoint.X, _topLeftPoint.Y);
                return p;
            }
            set
            {
                _topLeftPoint.X = value.X;
                _topLeftPoint.Y = value.Y;
                Layout();
            }
        }

        [CategoryAttribute("Position"),
        DisplayName("Arch Start"),
        DescriptionAttribute("The point on the line where the arch starts.")]
        public Point ArchLeft
        {
            get
            {
                Point p = new Point(_archLeftPoint.X, _archLeftPoint.Y);
                return p;
            }
            set
            {
                _archLeftPoint.X = value.X;
                _archLeftPoint.Y = value.Y;
                Layout();
            }
        }
        
        // For now we don't want this to show. We might delete it later if it is not used
        [Browsable(false)]
        public int MaxAlpha
        {
            get { return _pixels[0].MaxAlpha; }
            set { 
                _pixels[0].MaxAlpha = value;
            }
        }

        public override void Layout()
        {
            double pixelSpacing = (double)(_bottomRightPoint.Y - _archLeftPoint.Y) / (double)_linePixelCount;
            for (int i = 0; i < _linePixelCount; i++)
            {
                PreviewPixel pixel = _pixels[i];
                pixel.X = _topLeftPoint.X;
                pixel.Y = _bottomRightPoint.Y - (int)(i * pixelSpacing);;
            }

            int arcWidth = _bottomRightPoint.X - _topLeftPoint.X;
            int arcHeight = _archLeftPoint.Y - _topLeftPoint.Y;
            List<Point> points = PreviewTools.GetArcPoints(arcWidth, arcHeight, _archPixelCount);
            for (int i = 0; i < points.Count; i++)
            {
                PreviewPixel pixel = _pixels[i + _linePixelCount];
                pixel.X = points[i].X + _topLeftPoint.X;
                pixel.Y = points[i].Y + _topLeftPoint.Y;
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY) 
        {
            if (_selectedPoint != null)
            {
                _selectedPoint.X = x;
                _selectedPoint.Y = y;
                
                _archLeftPoint.X = _topLeftPoint.X;

                if (justPlaced)
                {
                    _archLeftPoint.Y = _topLeftPoint.Y + ((_bottomRightPoint.Y - _topLeftPoint.Y) / 4);
                }

                Layout();
                SelectDragPoints();
            }
            // If we get here, we're moving
            else
            {
                _bottomRightPoint.X = bottomRightStart.X + changeX;
                _bottomRightPoint.Y = bottomRightStart.Y + changeY;
                _topLeftPoint.X = topLeftStart.X + changeX;
                _topLeftPoint.Y = topLeftStart.Y + changeY;
                _archLeftPoint.X = archStart.X + changeX;
                _archLeftPoint.Y = archStart.Y + changeY;
                Layout();
            }
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
            List<PreviewPoint> points = new List<PreviewPoint>();
            points.Add(_bottomRightPoint);
            points.Add(_topLeftPoint);
            points.Add(_archLeftPoint);
            SetSelectPoints(points, null);
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
            justPlaced = false;
            if (point == null)
            {
                topLeftStart = new PreviewPoint(_topLeftPoint.X, _topLeftPoint.Y);
                bottomRightStart = new PreviewPoint(_bottomRightPoint.X, _bottomRightPoint.Y);
                archStart = new PreviewPoint(_archLeftPoint.X, _archLeftPoint.Y);
            }
            _selectedPoint = point;
        }

        public override void SelectDefaultSelectPoint()
        {
            _selectedPoint = _bottomRightPoint;
            justPlaced = true;
        }

        public override void MoveTo(int x, int y)
        {
            int deltaX = x - _topLeftPoint.X;
            int deltaY = y - _topLeftPoint.Y;

            _topLeftPoint.X += deltaX;
            _topLeftPoint.Y += deltaY;
            _bottomRightPoint.X += deltaX;
            _bottomRightPoint.Y += deltaY;
            _archLeftPoint.X += deltaX;
            _archLeftPoint.Y += deltaY;

            Layout();
        }

        public override void Resize(double aspect)
        {
            _topLeftPoint.X = (int)(_topLeftPoint.X * aspect);
            _topLeftPoint.Y = (int)(_topLeftPoint.Y * aspect);
            _bottomRightPoint.X = (int)(_bottomRightPoint.X * aspect);
            _bottomRightPoint.Y = (int)(_bottomRightPoint.Y * aspect);
            _archLeftPoint.X = (int)(_archLeftPoint.X * aspect);
            _archLeftPoint.Y = (int)(_archLeftPoint.Y * aspect);

            Layout();
        }

    }
}
