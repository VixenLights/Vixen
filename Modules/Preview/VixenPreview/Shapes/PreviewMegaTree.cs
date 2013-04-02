using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewMegaTree : PreviewBaseShape, ICloneable
    {
        [DataMember]
        private PreviewPoint _topLeft;
        [DataMember]
        private PreviewPoint _bottomRight;

        [DataMember]
        private int _stringCount;
        [DataMember]
        private int _topHeight;
        [DataMember]
        private int _topWidth;
        [DataMember]
        private int _baseHeight;
        [DataMember]
        private int _lightsPerString;
        [DataMember]
        private int _degrees;

        [DataMember]
        private PreviewPoint _topRight, _bottomLeft;

        double _stringsInDegrees = 0;

        private PreviewPoint p1Start, p2Start;

        public PreviewMegaTree(PreviewPoint point1)
        {
            _topLeft = point1;
            _bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

            _stringCount = 16;
            _topWidth = 20;
            _topHeight = _topWidth/2;
            _baseHeight = 40;
            _lightsPerString = 50;
            _degrees = 180;

            _strings = new List<PreviewBaseShape>();

            // Just add the pixels, we don't care where they go... they get positioned in Layout()
            for (int stringNum = 0; stringNum < _stringCount; stringNum++)
            {
                PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString);
                _strings.Add(line);
            }

            // Lay out the pixels
            Layout();

            DoResize += new ResizeEvent(OnResize);
        }

        public void SetTopLeft(int X, int Y)
        {
            _topLeft.X = X;
            _topLeft.Y = Y;
        }

        public void SetBottomRight(int X, int Y)
        {
            _bottomRight.X = X;
            _bottomRight.Y = Y;
        }

        public int TopHeight
        {
            set { _topHeight = value; }
            get { return _topHeight; }
        }

        public int TopWidth
        {
            set { _topWidth = value; }
            get { return _topWidth; }
        }

        public int BaseHeight
        {
            set { _baseHeight = value; }
            get { return _baseHeight; }
        }

        public int Degrees
        {
            set { _degrees = value; }
            get { return _degrees; }
        }

        public int LightsPerString
        {
            set
            {
                _lightsPerString = value;
            }
            get { return _lightsPerString; }
        }

        public void SetStrings(List<PreviewBaseShape> strings)
        {
            _strings = new List<PreviewBaseShape>();
            foreach (PreviewBaseShape line in strings)
            {
                PreviewBaseShape newLine = (PreviewLine)line.Clone();
                _strings.Add(newLine);
            }
            _stringCount = _strings.Count();
        }

        public int StringCount
        {
            set
            {
                _stringCount = value;
                while (_strings.Count > _stringCount)
                {
                    _strings.RemoveAt(_strings.Count - 1);
                }
                while (_strings.Count < _stringCount)
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString);
                    _strings.Add(line);
                }
            }
            get { return _stringCount; }
        }

        public int PixelCount
        {
            get { return Pixels.Count; }
        }

        public void Layout()
        {
            int width = _bottomRight.X - _topLeft.X;
            int height = _bottomRight.Y - _topLeft.Y;

            List<Point> _topEllipsePoints;
            List<Point> _baseEllipsePoints;

            // First we'll get the top and bottom ellipses
            double _topLeftOffset = _topLeft.X + (width / 2) - (_topWidth / 2);
            _topEllipsePoints = PreviewTools.GetEllipsePoints(_topLeftOffset, _topLeft.Y, _topWidth, _topHeight, _stringCount, _degrees);
            double bottomTopOffset = _bottomRight.Y - _baseHeight;
            _baseEllipsePoints = PreviewTools.GetEllipsePoints(_topLeft.X, bottomTopOffset, width, _baseHeight, _stringCount, _degrees);

            _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);

            for (int stringNum = 0; stringNum < (int)_stringsInDegrees; stringNum++)
            {
                if (stringNum < _topEllipsePoints.Count)
                {
                    Point topPixel = _topEllipsePoints[stringNum];
                    Point basePixel = _baseEllipsePoints[stringNum];

                    PreviewLine line = _strings[stringNum] as PreviewLine;
                    line.SetPoint0(basePixel.X, basePixel.Y);
                    line.SetPoint1(topPixel.X, topPixel.Y);
                    line.Layout();
                }
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY)
        {
            // See if we're resizing
            if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size)
            {
                if (_selectedPoint == _topRight)
                {
                    _topLeft.Y = y;
                    _bottomRight.X = x;
                }
                else if (_selectedPoint == _bottomLeft)
                {
                    _topLeft.X = x;
                    _bottomRight.Y = y;
                }
                _selectedPoint.X = x;
                _selectedPoint.Y = y;
            }
            // If we get here, we're moving
            else 
            {
                _topLeft.X = p1Start.X + changeX;
                _topLeft.Y = p1Start.Y + changeY;
                _bottomRight.X = p2Start.X + changeX;
                _bottomRight.Y = p2Start.Y + changeY;
            }

            _topRight.X = _bottomRight.X;
            _topRight.Y = _topLeft.Y;
            _bottomLeft.X = _topLeft.X;
            _bottomLeft.Y = _bottomRight.Y;

            // Layout the standard shape
            Layout();
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
            // Create the size points
            List<PreviewPoint> selectPoints = new List<PreviewPoint>();
            
            selectPoints.Add(_topLeft);
            selectPoints.Add(_bottomRight);
            _topRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
            selectPoints.Add(_topRight);
            _bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
            selectPoints.Add(_bottomLeft);

            // Tell the base shape about the newely created points
            
            SetSelectPoints(selectPoints, null);
        }

        public override bool PointInShape(PreviewPoint point)
        {
            if (_strings != null)
            {
                foreach (PreviewLine line in _strings)
                {
                    if (line.PointInShape(point))
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

        public override void PropertyDialog()
        {
            PreviewMegaTree originalTree = (PreviewMegaTree)this.Clone();
            PreviewMegaTreeProperties f = new PreviewMegaTreeProperties(originalTree);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //SetTopLeft(f.Tree._topLeft.X, f.Tree._topRight.Y);
                //SetBottomRight(f.Tree._bottomRight.X, f.Tree._bottomRight.Y);
                StringCount = f.Tree.StringCount;
                TopHeight = f.Tree.TopHeight;
                TopWidth = f.Tree.TopWidth;
                BaseHeight = f.Tree.BaseHeight;
                LightsPerString = f.Tree.LightsPerString;
                Degrees = f.Tree.Degrees;
                SetStrings(_strings);
                Deselect();
                Select();
                Layout();
            }
        }

        public override void Draw(Graphics graphics, Color color)
        {
            if (_strings != null)
            {
                for (int stringNum = 0; stringNum < _stringCount; stringNum++)
                {
                    PreviewLine line = (PreviewLine)_strings[stringNum];
                    if (stringNum < (int)_stringsInDegrees)
                    {
                        line.Draw(graphics, color);
                    }
                    else
                    {
                        line.Draw(graphics, Color.Transparent);
                    }
                }
            }
            base.Draw(graphics, color);
        }

        public override void Draw(FastPixel fp, Color color)
        {
            if (_strings != null) {
                foreach (PreviewBaseShape line in _strings)
                    line.Draw(fp, color);
            }
            base.Draw(fp, color);
        }

        public override void Draw(FastPixel fp)
        {
            if (_strings != null) {
                foreach (PreviewBaseShape line in _strings)
                    line.Draw(fp);
            }
            base.Draw(fp);
        }

        public override object Clone()
        {
            PreviewMegaTree newTree = (PreviewMegaTree)this.MemberwiseClone();

            newTree._strings = new List<PreviewBaseShape>();
            foreach (PreviewBaseShape line in _strings)
            {
                PreviewBaseShape newLine = (PreviewLine)line.Clone();
                newTree._strings.Add(newLine);
            }
            newTree._topLeft = new PreviewPoint(_topLeft);
            newTree._bottomRight = new PreviewPoint(_bottomRight);
            //newTree.LightsPerString = LightsPerString;
            //newTree.Degrees = Degrees;

            return newTree;
        }
    }
}
