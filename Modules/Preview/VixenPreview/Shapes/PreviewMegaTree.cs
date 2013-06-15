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

        public PreviewMegaTree(PreviewPoint point1, ElementNode selectedNode)
        {
            _topLeft = point1;
            _bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

            _stringCount = 16;
            _topWidth = 20;
            _topHeight = _topWidth/2;
            _baseHeight = 40;
            _lightsPerString = 50;
            _degrees = 360;

            _strings = new List<PreviewBaseShape>();

            int childLightCount;
            if (IsPixelTreeSelected(selectedNode, out childLightCount)) 
            {
                StringType = StringTypes.Pixel;
                _lightsPerString = childLightCount;
                foreach (ElementNode child in selectedNode.Children) 
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, child);
                    _strings.Add(line);
                }
                _stringCount = _strings.Count;
            }
            else if (IsStandardTreeSelected(selectedNode))
            {
                StringType = StringTypes.Standard;
                foreach (ElementNode child in selectedNode.Children)
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, child);
                    _strings.Add(line);
                }
                _stringCount = _strings.Count;
            }
            else
            {
                // Just add the pixels, we don't care where they go... they get positioned in Layout()
                for (int stringNum = 0; stringNum < _stringCount; stringNum++)
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, null);
                    _strings.Add(line);
                }
            }

            // Lay out the pixels
            Layout();

            //DoResize += new ResizeEvent(OnResize);
        }

        private bool IsPixelTreeSelected(ElementNode selectedNode, out int childLightCount)
        {
            int lastChildLightCount = -1;
            childLightCount = -1;
            if (selectedNode != null && selectedNode.Children != null)
            {
                int parentStringCount = selectedNode.Children.ToList().Count;
                // Selected node has to be a group!
                if (!selectedNode.IsLeaf && parentStringCount >= 4)
                {
                    // Iterate through the strings in the tree
                    parentStringCount = selectedNode.Children.ToList().Count;
                    foreach (ElementNode parent in selectedNode.Children)
                    {
                        int childCount = parent.Children.ToList().Count;
                        if (lastChildLightCount == -1)
                        {
                            lastChildLightCount = childCount;
                        }
                        // All the strings have to have the same light count for this to work!
                        else if (lastChildLightCount != childCount)
                        {
                            return false;
                        }
                        lastChildLightCount = childCount;

                        foreach (ElementNode child in parent.Children)
                        {
                            // If there are sub-groups this is not a mega tree element!
                            if (!child.IsLeaf)
                            {
                                return false;
                            }
                        }
                    }
                }

                if (lastChildLightCount > 4 && parentStringCount >= 4)
                {
                    childLightCount = lastChildLightCount;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }

        private bool IsStandardTreeSelected(ElementNode selectedNode)
        {
            int parentStringCount = 0;
            // Selected node has to be a group!
            if (selectedNode != null && !selectedNode.IsLeaf)
            {
                // Iterate through the strings in the tree
                foreach (ElementNode parent in selectedNode.Children)
                {
                    parentStringCount += 1;
                    // If there are more groups, this is not a Mega Tree
                    if (!parent.IsLeaf)
                        return false;
                }
            }
            // Gotta have at least 4 strings to make a Mega Tree!
            return (parentStringCount >= 4);
        }

        [OnDeserialized]
        new void OnDeserialized(StreamingContext context)
        {
            Layout();
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

        public int VisibleStringCount()
        {
            int count = Convert.ToInt32((double)_stringCount * ((double)Degrees / 360.0));
            return count;
        }

#region "Properties'

        public int TopHeight
        {
            set 
            { 
                _topHeight = value;
                Layout();
            }
            get { return _topHeight; }
        }

        public int TopWidth
        {
            set 
            { 
                _topWidth = value;
                Layout();
            }
            get { return _topWidth; }
        }

        public int BaseHeight
        {
            set 
            { 
                _baseHeight = value;
                Layout();
            }
            get { return _baseHeight; }
        }

        public int Degrees
        {
            set 
            { 
                _degrees = value;
                Layout();
            }
            get { return _degrees; }
        }

        public int LightsPerString
        {
            set
            {
                _lightsPerString = value;
                foreach (PreviewLine line in _strings)
                {
                    line.PixelCount = _lightsPerString;
                }
            }
            get { return _lightsPerString; }
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
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, null);
                    _strings.Add(line);
                }
                Layout();
            }
            get { return _stringCount; }
        }

        public override int Top
        {
            get
            {
                return _topLeft.Y;
            }
            set { }
        }

        public override int Left
        {
            get
            {
                return _topLeft.X;
            }
            set
            { }
        }

        public PreviewPoint BottomRight
        {
            get { return _bottomRight; }
            set { _bottomRight = value; }
        }
#endregion

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


        public int PixelCount
        {
            set 
            {
                foreach (PreviewLine line in _strings)
                {
                    line.PixelCount = value;
                }
            }
            get { return Pixels.Count; }
        }

        
        [Browsable(false)]
        public override List<PreviewPixel> Pixels
        {
            get
            {
                if (_strings != null && _strings.Count > 0)
                {
                    _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);
                    List<PreviewPixel> outPixels = new List<PreviewPixel>();
                    for (int i = 0; i < _stringsInDegrees; i++)
                    {
                        foreach (PreviewPixel pixel in _strings[i].Pixels)
                        {
                            outPixels.Add(pixel);
                        }
                    }

                    //foreach (PreviewBaseShape line in _strings)
                    //{
                    //    foreach (PreviewPixel pixel in line.Pixels)
                    //    {
                    //        outPixels.Add(pixel);
                    //    }
                    //}
                    return outPixels;
                }
                else
                {
                    return _pixels;
                }
            }
            set
            {
                _pixels = value;
                //ResetNodeToPixelDictionary();
            }
        }

        public override void Layout()
        {
            int width = _bottomRight.X - _topLeft.X;
            int height = _bottomRight.Y - _topLeft.Y;

            List<Point> _topEllipsePoints;
            List<Point> _baseEllipsePoints;

            // First we'll get the top and bottom ellipses
            //double _topLeftOffset = _topLeft.X + (width / 2) - (_topWidth / 2);
            //_topEllipsePoints = PreviewTools.GetEllipsePoints(_topLeftOffset, _topLeft.Y, _topWidth, _topHeight, _stringCount, _degrees);
            //double bottomTopOffset = _bottomRight.Y - _baseHeight;
            //_baseEllipsePoints = PreviewTools.GetEllipsePoints(_topLeft.X, bottomTopOffset, width, _baseHeight, _stringCount, _degrees);
            double _topLeftOffset = _topLeft.X + (width / 2) - (_topWidth / 2);
            double bottomTopOffset = _bottomRight.Y - _baseHeight;
            //if (_degrees < 360)
            //{
            //    _topEllipsePoints = PreviewTools.GetEllipsePoints(_topLeftOffset, _topLeft.Y, _topWidth, _topHeight, _stringCount+1, 360, 0);
            //    _baseEllipsePoints = PreviewTools.GetEllipsePoints(_topLeft.X, bottomTopOffset, width, _baseHeight, _stringCount+1, 360, 0);
            //}
            //else
            //{
            _topEllipsePoints = PreviewTools.GetEllipsePoints(_topLeftOffset, _topLeft.Y, _topWidth, _topHeight, _stringCount, _degrees, 0);
            _baseEllipsePoints = PreviewTools.GetEllipsePoints(_topLeft.X, bottomTopOffset, width, _baseHeight, _stringCount, _degrees, 0);
            //}

            _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);

            for (int stringNum = 0; stringNum < (int)_stringCount; stringNum++)
            {
                if (stringNum < (int)_stringsInDegrees)
                {
                    if (stringNum < _topEllipsePoints.Count())
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

        //private void OnResize(EventArgs e)
        //{
        //    Layout();
        //}

        public override void SelectDragPoints()
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

        //public override void Draw(Graphics graphics, Color color)
        //{
        //    if (_strings != null)
        //    {
        //        for (int stringNum = 0; stringNum < _stringCount; stringNum++)
        //        {
        //            PreviewLine line = (PreviewLine)_strings[stringNum];
        //            if (stringNum < (int)_stringsInDegrees)
        //            {
        //                line.Draw(graphics, color);
        //            }
        //            else
        //            {
        //                line.Draw(graphics, Color.Transparent);
        //            }
        //        }
        //    }
        //    base.Draw(graphics, color);
        //}

        //public override void Draw(FastPixel fp, Color color)
        //{
        //    if (_strings != null) {
        //        _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);
        //        for (int i = 0; i < (int)_stringsInDegrees; i++)
        //        {
        //            _strings[i].Draw(fp, color);
        //        }
        //    }
        //    base.Draw(fp, color);
        //}

        //public override void Draw(FastPixel fp)
        //{
        //    if (_strings != null) {
        //        _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);
        //        for (int i = 0; i < _stringsInDegrees; i++)
        //        {
        //            _strings[i].Draw(fp);
        //        }
        //    }
        //    base.Draw(fp);
        //}

        public override void Draw(FastPixel fp, bool editMode, List<ElementNode> highlightedElements, bool selected, bool forceDraw)
        {
            if (_strings != null)
            {
                _stringsInDegrees = (double)_stringCount * ((double)_degrees / 360);
                for (int i = 0; i < _stringsInDegrees; i++)
                {
                    foreach (PreviewPixel pixel in _strings[i]._pixels)
                    {
                        //Console.WriteLine(pixel.X + ":" + pixel.Y);
                        DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);

                        //if (highlightedElements.Contains(pixel.Node))
                        //    pixel.Draw(fp, PreviewTools.HighlightedElementColor);
                        //else
                        //    pixel.Draw(fp, Color.White);
                    }
                }
            }

            base.Draw(fp, editMode, highlightedElements, selected, forceDraw);
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

        [Editor(typeof(PreviewSetElementsUIEditor), typeof(UITypeEditor)),
        CategoryAttribute("Settings"),
        DisplayName("Linked Elements")]
        public override List<PreviewBaseShape> Strings
        {
            get
            {
                Layout();
                List<PreviewBaseShape> stringsResult;
                if (_strings.Count != _stringsInDegrees)
                {
                    stringsResult = new List<PreviewBaseShape>();
                    for (int i = 0; i < _stringsInDegrees; i++)
                    {
                        stringsResult.Add(_strings[i]);
                        Console.WriteLine("Added String: " + i.ToString());
                    }
                }
                else
                {
                    stringsResult = _strings;
                    if (stringsResult == null)
                    {
                        stringsResult = new List<PreviewBaseShape>();
                        stringsResult.Add(this);
                    }
                }
                return stringsResult;
            }
            set { }
        }

        public override void MoveTo(int x, int y)
        {
            Point newTopLeft = new Point();
            newTopLeft.X = Math.Min(_topLeft.X, _bottomRight.X);
            newTopLeft.Y = Math.Min(_topLeft.Y, _bottomRight.Y);

            int deltaX = x - newTopLeft.X;
            int deltaY = y - newTopLeft.Y;

            _topLeft.X += deltaX;
            _topLeft.Y += deltaY;
            _bottomRight.X += deltaX;
            _bottomRight.Y += + deltaY;

            _topRight.X = _bottomRight.X;
            _topRight.Y = _topLeft.Y;
            _bottomLeft.X = _topLeft.X;
            _bottomLeft.Y = _bottomRight.Y;

            Layout();
        }

        public override void Resize(double aspect)
        {
            _topLeft.X = (int)(_topLeft.X * aspect);
            _topLeft.Y = (int)(_topLeft.Y * aspect);
            _bottomRight.X = (int)(_bottomRight.X * aspect);
            _bottomRight.Y = (int)(_bottomRight.Y * aspect);

            Layout();
        }

        public override void ResizeFromOriginal(double aspect)
        {
            _topLeft.X = p1Start.X;
            _topLeft.Y = p1Start.Y;
            _bottomRight.X = p2Start.X;
            _bottomRight.Y = p2Start.Y;
            Resize(aspect);
        }

    }
}
