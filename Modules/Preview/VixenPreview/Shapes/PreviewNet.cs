using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Controls;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewNet : PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _topLeft;
        [DataMember]
        private PreviewPoint _topRight;
        [DataMember]
        private PreviewPoint _bottomLeft;
        [DataMember]
        private PreviewPoint _bottomRight;

        [DataMember]
        private int _pixelSpacing = 8;

        private ElementNode initiallyAssignedNode = null;

        private bool lockXY = false;
        private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;

        public PreviewNet(PreviewPoint point1, ElementNode selectedNode)
        {
            _topLeft = point1;
            _topRight = new PreviewPoint(point1);
            _bottomLeft = new PreviewPoint(point1);
            _bottomRight = new PreviewPoint(point1);

            initiallyAssignedNode = selectedNode;

            Layout();

            DoResize += new ResizeEvent(OnResize);
        }

        [OnDeserialized]
        new void OnDeserialized(StreamingContext context)
        {
            if (_pixelSpacing == 0)
                _pixelSpacing = 8;
            Layout();
        }

        #region "Properties"

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("Nets are defined by 4 points. This is point 1.")]
        public Point TopLeftPoint
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
        DisplayName("Top Right"),
        DescriptionAttribute("Nets are defined by 4 points. This is point 2.")]
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
        DescriptionAttribute("Nets are defined by 4 points. This is point 3.")]
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
        DescriptionAttribute("Nets are defined by 4 points. This is point 4.")]
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

        [CategoryAttribute("Settings"),
        DisplayName("Light Spacing"),
        DescriptionAttribute("This is the spacing between each light in the net.")]
        public int PixelSpacing
        {
            get { return _pixelSpacing; }
            set
            {
                _pixelSpacing = value;
                Layout();
            }
        }

        [Browsable(false)]
        public override StringTypes StringType
        {
            get { return StringTypes.Standard; }
            set
            {
                _stringType = value;
            }
        }

        [Browsable(false)]
        public int PixelCount
        {
            get { return Pixels.Count; }
        }

        [Browsable(false)]
        public override int Left
        {
            get 
            {
                return Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
            }
        }

        [Browsable(false)]
        public override int Top
        {
            get 
            {
                return Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));
            }
        }
        
#endregion

        public override void Layout()
        {
            ElementNode node = null;
            Guid nodeId = Guid.Empty;
            if (PixelCount > 0)
            {
                node = _pixels[0].Node;
                nodeId = _pixels[0].NodeId;
                Console.WriteLine(_pixels[0].NodeId);
                _pixels.Clear();
            }
            else if (initiallyAssignedNode != null)
            {
                if (initiallyAssignedNode.IsLeaf)
                {
                    node = initiallyAssignedNode;
                    nodeId = initiallyAssignedNode.Id;
                }
            }

            Point boundsTopLeft = new Point();
            boundsTopLeft.X = Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
            boundsTopLeft.Y = Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));
            Point bottomRight = new Point();
            bottomRight.X = Math.Max(_topLeft.X, Math.Max(Math.Max(_topRight.X, _bottomRight.X), _bottomLeft.X));
            bottomRight.Y = Math.Max(_topLeft.Y, Math.Max(Math.Max(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));
            Rectangle rect = new Rectangle(boundsTopLeft, new Size(bottomRight.X - boundsTopLeft.X, bottomRight.Y - boundsTopLeft.Y));

            Point tL = new Point(_topLeft.X - boundsTopLeft.X, _topLeft.Y - boundsTopLeft.Y);
            Point tR = new Point(_topRight.X - boundsTopLeft.X, _topRight.Y - boundsTopLeft.Y);
            Point bL = new Point(_bottomLeft.X - boundsTopLeft.X, _bottomLeft.Y - boundsTopLeft.Y);
            Point bR = new Point(_bottomRight.X - boundsTopLeft.X, _bottomRight.Y - boundsTopLeft.Y);
            Point[] points = { tL, tR, bR, bL };

            if (rect.Width > 0 && rect.Height > 0)
            {
                Bitmap b;
                FastPixel fp;

                b = new Bitmap(rect.Width, rect.Height);
                Graphics g = Graphics.FromImage(b);
                g.Clear(Color.Transparent);
                g.FillPolygon(Brushes.White, points);
                fp = new FastPixel(b);
                fp.Lock();
                int xCount = 1;
                int spacingY = _pixelSpacing;
                for (int y = 0; y < rect.Height; y++)
                {
                    if (spacingY % _pixelSpacing == 0)
                    {
                        int xDiv;
                        if (xCount % 2 == 0)
                            xDiv = _pixelSpacing;
                        else
                            xDiv = _pixelSpacing/2;

                        for (int x = 0; x < rect.Width; x++)
                        {
                            if ((x+xDiv) % _pixelSpacing == 0) {
                                Color newColor = fp.GetPixel(x, y);
                                if (newColor.A != 0)
                                {
                                    PreviewPixel pixel = new PreviewPixel(x + boundsTopLeft.X, y + boundsTopLeft.Y, PixelSize);
                                    pixel.NodeId = nodeId;
                                    _pixels.Add(pixel);
                                }
                            }
                        }
                        xCount += 1;
                    }
                    spacingY += 1;
                }
                fp.Unlock(false);
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY)
        {
            if (_selectedPoint != null)
            {
                _selectedPoint.X = x;
                _selectedPoint.Y = y;
                if (lockXY || (_selectedPoint == _bottomRight && System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control))
                {
                    _topRight.X = x;
                    _bottomLeft.Y = y;
                }
                Layout();
            }
            // If we get here, we're moving
            else
            {
                _topLeft.X = topLeftStart.X + changeX;
                _topLeft.Y = topLeftStart.Y + changeY;
                _topRight.X = topRightStart.X + changeX;
                _topRight.Y = topRightStart.Y + changeY;
                _bottomLeft.X = bottomLeftStart.X + changeX;
                _bottomLeft.Y = bottomLeftStart.Y + changeY;
                _bottomRight.X = bottomRightStart.X + changeX;
                _bottomRight.Y = bottomRightStart.Y + changeY;
                Layout();
            }
        }

        private void OnResize(EventArgs e)
        {
            Layout();
        }

        public override void Select()
        {
            base.Select();
            connectStandardStrings = true;
            SelectDragPoints();
        }

        private void SelectDragPoints()
        {
            List<PreviewPoint> points = new List<PreviewPoint>();
            points.Add(_topLeft);
            points.Add(_topRight);
            points.Add(_bottomLeft);
            points.Add(_bottomRight);
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
            lockXY = false;
            if (point == null)
            {
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
            Point boundsTopLeft = new Point();
            boundsTopLeft.X = Math.Min(_topLeft.X, Math.Min(Math.Min(_topRight.X, _bottomRight.X), _bottomLeft.X));
            boundsTopLeft.Y = Math.Min(_topLeft.Y, Math.Min(Math.Min(_topRight.Y, _bottomRight.Y), _bottomLeft.Y));

            int changeX = x - boundsTopLeft.X;
            int changeY = y - boundsTopLeft.Y;

            _topLeft.X += changeX;
            _topLeft.Y += changeY;
            _topRight.X += changeX;
            _topRight.Y += changeY;
            _bottomRight.X += changeX;
            _bottomRight.Y += changeY;
            _bottomLeft.X += changeX;
            _bottomLeft.Y += changeY;

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

    }
}
