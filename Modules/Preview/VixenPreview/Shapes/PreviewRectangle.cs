using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Controls;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewRectangle: PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _topLeft;
        [DataMember]
        private PreviewPoint _topRight;
        [DataMember]
        private PreviewPoint _bottomLeft;
        [DataMember]
        private PreviewPoint _bottomRight;

        private bool lockXY = false;
        private PreviewPoint topLeftStart, topRightStart, bottomLeftStart, bottomRightStart;

        public PreviewRectangle(PreviewPoint point1)
        {
            _topLeft = point1;
            _topRight = new PreviewPoint(point1);
            _bottomLeft = new PreviewPoint(point1);
            _bottomRight = new PreviewPoint(point1);

            // Just add lines, they will be layed out in Layout()
            _strings = new List<PreviewBaseShape>();
            for (int i = 0; i < 4; i++)
            {
                PreviewLine line;
                line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(20, 20), 10);
                line.PixelColor = Color.White;
                _strings.Add(line);
            }
            Layout();

            DoResize += new ResizeEvent(OnResize);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Layout();
        }

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 1.")]
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
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 2.")]
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
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 3.")]
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
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 4.")]
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
        DisplayName("String 1 Light Count"),
        DescriptionAttribute("Number of pixels or lights in string 1 of the rectangle.")]
        public int LightCountString1
        {
            get { return Strings[0].Pixels.Count; }
            set
            {
                (Strings[0] as PreviewLine).PixelCount = value;
                Layout();
            }
        }

        [CategoryAttribute("Settings"),
        DisplayName("String 2 Light Count"),
        DescriptionAttribute("Number of pixels or lights in string 2 of the rectangle.")]
        public int LightCountString2
        {
            get { return Strings[1].Pixels.Count; }
            set
            {
                (Strings[1] as PreviewLine).PixelCount = value;
                Layout();
            }
        }

        [CategoryAttribute("Settings"),
        DisplayName("String 3 Light Count"),
        DescriptionAttribute("Number of pixels or lights in string 3 of the rectangle.")]
        public int LightCountString3
        {
            get { return Strings[2].Pixels.Count; }
            set
            {
                (Strings[2] as PreviewLine).PixelCount = value;
                Layout();
            }
        }

        [CategoryAttribute("Settings"),
        DisplayName("String 4 Light Count"),
        DescriptionAttribute("Number of pixels or lights in string 4 of the rectangle.")]
        public int LightCountString4
        {
            get { return Strings[3].Pixels.Count; }
            set
            {
                (Strings[3] as PreviewLine).PixelCount = value;
                Layout();
            }
        }

        public int PixelCount
        {
            get { return Pixels.Count; }
        }

        public override void Layout()
        {
            (Strings[0] as PreviewLine).Point1 = TopLeftPoint;
            (Strings[0] as PreviewLine).Point2 = TopRightPoint;
            (Strings[0] as PreviewLine).Layout();

            (Strings[1] as PreviewLine).Point1 = TopRightPoint;
            (Strings[1] as PreviewLine).Point2 = BottomRightPoint;
            (Strings[1] as PreviewLine).Layout();

            (Strings[2] as PreviewLine).Point1 = BottomLeftPoint;
            (Strings[2] as PreviewLine).Point2 = BottomRightPoint;
            (Strings[2] as PreviewLine).Layout();

            (Strings[3] as PreviewLine).Point1 = TopLeftPoint;
            (Strings[3] as PreviewLine).Point2 = BottomLeftPoint;
            (Strings[3] as PreviewLine).Layout();
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

    }
}
