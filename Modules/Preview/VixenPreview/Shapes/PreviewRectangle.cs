//
// ToDo: Property Dialog
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewRectangle: PreviewBaseShape
    {
        [DataMember]
        private PreviewPoint _p1;
        [DataMember]
        private PreviewPoint _p2;
        [DataMember]
        private PreviewPoint _p3;
        [DataMember]
        private PreviewPoint _p4;

        private bool lockXY = false;

        [DataMember]
        private int _lightCountX1;
        [DataMember]
        private int _lightCountX2;
        [DataMember]
        private int _lightCountY1;
        [DataMember]
        private int _lightCountY2;


        private PreviewPoint p1Start, p2Start, p3Start, p4Start;

        public PreviewRectangle(PreviewPoint point1)
        {
            _p1 = point1;
            _p2 = new PreviewPoint(_p1.X, _p1.Y);
            _p3 = new PreviewPoint(_p1.X, _p1.Y);
            _p4 = new PreviewPoint(_p1.X, _p1.Y);

            _lightCountX1 = 10;
            _lightCountX2 = 10;
            _lightCountY1 = 10;
            _lightCountY2 = 10;

            int totalLights = _lightCountX1 + _lightCountX2 + _lightCountY1 + _lightCountY2;

            // Just add the pixels, they will get layed out next
            for (int lightNum = 0; lightNum < totalLights; lightNum++)
            {
                PreviewPixel pixel = AddPixel(20, 20);
                pixel.PixelColor = Color.White;
            }
            // Lay out the pixels
            Layout();

            DoResize += new ResizeEvent(OnResize);
        }

        [CategoryAttribute("Position"),
        DisplayName("Top Left"),
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 1.")]
        public Point TopLeftPoint
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

        [CategoryAttribute("Position"),
        DisplayName("Top Right"),
        DescriptionAttribute("Rectangles are defined by 4 points. This is point 2.")]
        public Point TopRightPoint
        {
            get
            {
                Point p = new Point(_p2.X, _p2.Y);
                return p;
            }
            set
            {
                _p2.X = value.X;
                _p2.Y = value.Y;
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
                Point p = new Point(_p3.X, _p3.Y);
                return p;
            }
            set
            {
                _p3.X = value.X;
                _p3.Y = value.Y;
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
                Point p = new Point(_p4.X, _p4.Y);
                return p;
            }
            set
            {
                _p4.X = value.X;
                _p4.Y = value.Y;
                Layout();
            }
        }

        //[CategoryAttribute("Settings"),
        //DisplayName("Left Light Count"),
        //DescriptionAttribute("Number of lights in the left string.")]
        //public int LightCountY1
        //{
        //    get { return _lightCountY1; }
        //    set
        //    {
        //        _lightCountY1 = value;
        //        Layout();
        //    }
        //}

        //[CategoryAttribute("Settings"),
        //DisplayName("Right Light Count"),
        //DescriptionAttribute("Number of lights in the right string.")]
        //public int LightCountY2
        //{
        //    get { return _lightCountY2; }
        //    set
        //    {
        //        _lightCountY2 = value;
        //        Layout();
        //    }
        //}

        //[CategoryAttribute("Settings"),
        //DisplayName("Top Light Count"),
        //DescriptionAttribute("Number of lights in the top string.")]
        //public int LightCountX1
        //{
        //    get { return _lightCountX1; }
        //    set
        //    {
        //        _lightCountX1 = value;
        //        Layout();
        //    }
        //}

        //[CategoryAttribute("Settings"),
        //DisplayName("Bottom Light Count"),
        //DescriptionAttribute("Number of lights in the bottom string.")]
        //public int LightCountX2
        //{
        //    get { return _lightCountX2; }
        //    set
        //    {
        //        _lightCountX2 = value;
        //        Layout();
        //    }
        //}

        public int PixelCount
        {
            get { return Pixels.Count; }
        }

        public void Layout()
        {
            double x, y = 0;
            //Top
            double X1XSpacing = (double)( _p1.X - _p2.X) / (double)(_lightCountX1 + 1);
            double X1YSpacing = (double)( _p1.Y - _p2.Y) / (double)(_lightCountX1 + 1);
            //Bottom
            double X2XSpacing = (double)( _p3.X - _p4.X) / (double)(_lightCountX2 + 1);
            double X2YSpacing = (double)( _p3.Y - _p4.Y) / (double)(_lightCountX1 + 1);
            //Left
            double Y1XSpacing = (double)( _p4.X - _p1.X) / (double)(_lightCountY1 + 1);
            double Y1YSpacing = (double)( _p4.Y - _p1.Y) / (double)(_lightCountY1 + 1);
            //Right
            double Y2XSpacing = (double)( _p2.X - _p3.X) / (double)(_lightCountY2 + 1);
            double Y2YSpacing = (double)( _p2.Y - _p3.Y) / (double)(_lightCountY2 + 1);

            int currentPixel = 0;

            //Top
            x = _p1.X - X1XSpacing;
            y = _p1.Y - X1YSpacing;
            for (int i = 0; i < _lightCountX1; i++)
            {
                Pixels[currentPixel].X = (int)Math.Round(x);
                Pixels[currentPixel].Y = (int)Math.Round(y);
                x -= X1XSpacing;
                y -= X1YSpacing;
                currentPixel++;
            }

            //Right
            x = _p2.X - Y2XSpacing;
            y = _p2.Y - Y2YSpacing;
            for (int i = 0; i < _lightCountY1; i++)
            {
                Pixels[currentPixel].X = (int)Math.Round(x);
                Pixels[currentPixel].Y = (int)Math.Round(y);
                x -= Y2XSpacing;
                y -= Y2YSpacing;
                currentPixel++;
            }

            //Bottom
            x = _p3.X - X2XSpacing;
            y = _p3.Y - X2YSpacing;
            for (int i = 0; i < _lightCountX2; i++)
            {
                Pixels[currentPixel].X = (int)Math.Round(x);
                Pixels[currentPixel].Y = (int)Math.Round(y);
                x -= X2XSpacing;
                y -= X2YSpacing;
                currentPixel++;
            }

            // Left
            x = _p4.X - Y1XSpacing;
            y = _p4.Y - Y1YSpacing;
            for (int i = 0; i < _lightCountY1; i++)
            {
                Pixels[currentPixel].X = (int)Math.Round(x);
                Pixels[currentPixel].Y = (int)Math.Round(y);
                x -= Y1XSpacing;
                y -= Y1YSpacing;
                currentPixel++;
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY) 
        {
            if (_selectedPoint != null)
            {
                _selectedPoint.X = x;
                _selectedPoint.Y = y;

                if (lockXY)
                {
                    _p2.X = x;
                    _p4.Y = y;
                }

                Layout();
                SelectDragPoints();
            }
            // If we get here, we're moving
            else
            {
                _p1.X = p1Start.X + changeX;
                _p1.Y = p1Start.Y + changeY;
                _p2.X = p2Start.X + changeX;
                _p2.Y = p2Start.Y + changeY;
                _p3.X = p3Start.X + changeX;
                _p3.Y = p3Start.Y + changeY;
                _p4.X = p4Start.X + changeX;
                _p4.Y = p4Start.Y + changeY;
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
            SelectDragPoints();
        }

        private void SelectDragPoints()
        {
            List<PreviewPoint> points = new List<PreviewPoint>();
            points.Add( _p1);
            points.Add( _p2);
            points.Add( _p3);
            points.Add( _p4);
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
                p1Start = new PreviewPoint( _p1.X, _p1.Y);
                p2Start = new PreviewPoint( _p2.X, _p2.Y) ;
                p3Start = new PreviewPoint( _p3.X, _p3.Y);
                p4Start = new PreviewPoint( _p4.X, _p4.Y);
            }
            _selectedPoint = point;
        }

        public override void SelectDefaultSelectPoint()
        {
            _selectedPoint = _p3;
            lockXY = true;
        }

        public override void PropertyDialog()
        {
            //PreviewLineProperties f = new PreviewLineProperties(this);
            //if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{

            //}
        }


    }
}
