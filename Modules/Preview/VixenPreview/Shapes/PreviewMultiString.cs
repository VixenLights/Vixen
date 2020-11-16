using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewMultiString: PreviewBaseShape
    {
        [DataMember] private int _stringCount;
        [DataMember] private List<PreviewPoint> _points = new List<PreviewPoint>();

        private ElementNode inputElements;
        private List<PreviewPoint> pStart = new List<PreviewPoint>();
        const int InitialLightsPerString = 10;

	    public override string TypeName => @"Multi-String";

		public PreviewMultiString(PreviewPoint point1, PreviewPoint point2, ElementNode selectedNode, double zoomLevel)
		{ 
			ZoomLevel = zoomLevel;
            AddPoint(PointToZoomPoint(point1));
            _strings = new List<PreviewBaseShape>();
            Creating = true;
            inputElements = selectedNode;
        }

        [OnDeserialized]
        private new void OnDeserialized(StreamingContext context)
        {
            Layout();
        }

        [Browsable(false)]
        public int StringCount
        {
            set
            {
                _stringCount = value;
                while (_strings.Count > _stringCount)
                {
                    _strings.RemoveAt(Strings.Count - 1);
                }

                bool first = true;
                while (_strings.Count < _stringCount)
                {
                    PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), InitialLightsPerString, null, ZoomLevel);
                    line.Parent = this;
                    line.AddStartPadding = !first;
                    _strings.Add(line);
                    first = false;
                }
            }
            get { return _stringCount; }
        }

        [Editor(typeof(PreviewSetElementsUIEditor), typeof(UITypeEditor)),
         CategoryAttribute("Settings"),
         DisplayName("Linked Elements")]
        public override List<PreviewBaseShape> Strings
        {
            get
            {
                if (_strings == null)
                {
                    _strings = new List<PreviewBaseShape>();
                }
                List<PreviewBaseShape> stringsResult;
                if (_strings.Count != StringCount && _strings.Count > 0)
                {
                    stringsResult = new List<PreviewBaseShape>();
                    for (int i = 0; i < StringCount; i++)
                    {
                        stringsResult.Add(_strings[i]);
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
                foreach (PreviewLine line in stringsResult)
                {
                    line.Parent = this;
                }
                return stringsResult;
            }
            set { }
        }
        [Browsable(false)]
        public override StringTypes StringType
        {
            get
            {
                base.StringType = StringTypes.Pixel;
                return StringTypes.Pixel;
            }
            set
            {
                base.StringType = StringTypes.Pixel;
            }
        }

        [Browsable(false)]
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
                    PreviewPixel pixel = new PreviewPixel(10, 10, 0, PixelSize);
                    Pixels.Add(pixel);
                }
                Layout();
            }
        }

        [Browsable(false)]
        public override int Right
        {
            get
            {
                int rMax = 0;
                foreach (PreviewLine line in Strings) {
                    rMax = Math.Max(rMax, line.Right);
                }
                return rMax;

                //int r = 0;
                //foreach (PreviewPoint p in _points)
                //{
                //    r = Math.Min(r, p.X);
                //}
                //return r;
			}
        }

        [Browsable(false)]
        public override int Bottom
        {
            get
            {
                int bMax = 0;
                foreach (PreviewLine line in Strings)
                {
                    bMax = Math.Max(bMax, line.Bottom);
                }
                return bMax;
                //int b = 0;
                //foreach (PreviewPoint p in _points)
                //{
                //    b = Math.Min(b, p.Y);
                //}
                //return b;
			}
        }

        public override int Top
        {
            get
            {
                int m = int.MaxValue;
                foreach (PreviewLine line in Strings)
                {
                    m = Math.Min(m, line.Top);
                }
                return m;
                //int t = int.MaxValue;
                //foreach (PreviewPoint p in _points)
                //{
                //    t = Math.Min(t, p.Y);
                //}
                //return t;
            }
            set
            {
                int currentTop = Top;
                int delta = currentTop - value;
                foreach (PreviewPoint p in _points)
                {
                    p.Y = p.Y - delta;
                }
                Layout();
            }
        }

        public override int Left
        {
            get
            {
                //int l = int.MaxValue;
                //foreach (PreviewPoint p in _points)
                //{
                //    l = Math.Min(l, p.X);
                //}
                //return l;
                int m = int.MaxValue;
                foreach (PreviewLine line in Strings)
                {
                    m = Math.Min(m, line.Left);
                }
                return m;
            }
            set
            {
                int currentLeft = Left;
                int delta = currentLeft - value;
                foreach (PreviewPoint p in _points)
                {
                    p.X = p.X - delta;
                }
                Layout();
            }
        }

        public void AddPoint(PreviewPoint point)
        {
            _points.Add(point);
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewMultiString shape = (matchShape as PreviewMultiString);
            PixelSize = shape.PixelSize;
            Layout();
        }

        public override void Select(bool selectDragPoints)
        {
            StringType = StringTypes.Pixel;
            base.Select(selectDragPoints);
        }

        public override void Layout()
        {
            if (_points != null && _points.Count >= 2 && _strings.Count() > 0)
            {
                if (StringCount < _points.Count - 1 || StringCount > _points.Count - 1)
                {
                    StringCount = _points.Count - 1;
                }
                for (int stringNum = 0; stringNum < StringCount; stringNum++)
                {
                    var line = Strings[stringNum] as PreviewLine;
                    line.AddStartPadding = stringNum > 0;
                    line.Point1 = _points[stringNum].ToPoint();
                    line.Point2 = _points[stringNum + 1].ToPoint();
                }

                SetPixelZoom();
            }
        }

        public override void MouseMove(int x, int y, int changeX, int changeY)
        {
            PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
            // See if we're resizing
            if (_selectedPoint != null)
            {
                _selectedPoint.X = point.X;
                _selectedPoint.Y = point.Y;
                Layout();
                SelectDragPoints();
            }
            // If we get here, we're moving
            else
            {
                if (pStart.Count() == _points.Count())
                {
                    for (int pNum = 0; pNum < _points.Count(); pNum++)
                    {
                        _points[pNum].X = Convert.ToInt32(pStart[pNum].X * ZoomLevel) + changeX;
                        _points[pNum].Y = Convert.ToInt32(pStart[pNum].Y * ZoomLevel) + changeY;
                        PointToZoomPointRef(_points[pNum]);
                    }

                    //PointToZoomPointRef(_points[0]);
                    //PointToZoomPointRef(_points[1]);

                    //Left = x;

                    Layout();
                }
            }
        }

        public override void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Creating)
                {
                    PreviewPoint newPoint = PointToZoomPoint(new PreviewPoint(e.X, e.Y));
                    AddPoint(newPoint);
                    CreateString();
                    _selectedPoint = newPoint;
                }
            }
            SelectDragPoints();
            Layout();
        }

        private void CreateString()
        {
            if (Creating)
            {
                // There is no ElementNode selected in the tree
                if (inputElements == null)
                {
                    StringCount++;
                }
                // We've got an ElementNode selected in the tree, now figure out what to do with it.
                else
                {
                    int inputStringCount = 0;
                    int inputPixelCount = 0;
                    PreviewTools.CountPixelsAndStrings(inputElements, out inputPixelCount, out inputStringCount);
                    int stringNum = _strings.Count();

                    // is this a single node with no children?
                    if (inputPixelCount == 0 && inputStringCount == 0)
                    {
                        PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), 10, inputElements, ZoomLevel);
                        line.Parent = this;
                        _strings.Add(line);
                        _stringCount = _strings.Count();
                    }
                    // If we're here, we've got a group selected
                    else
                    {
                        // Do we have multiple child strings in this group and no individual pixels selected?
                        if (stringNum <= inputStringCount - 1 && inputPixelCount == 0)
                        {
                            StringType = StringTypes.Pixel;

                            ElementNode child = null;
                            if (inputStringCount > 0)
                            {
                                child = inputElements.Children.ToList()[stringNum];
                            }
                            else
                            {
                                child = inputElements;
                            }
                            int pixelCount = child.Count();
                            PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), child.Count(), child, ZoomLevel);
                            line.Parent = this;
                            _strings.Add(line);
                            _stringCount = _strings.Count();
                        }
                        // If we're here, we have multiple pixels in a single string and no strings in our parent node
                        else if (inputPixelCount > 2 && inputStringCount == 0 && stringNum == 0)
                        {
                            StringType = StringTypes.Pixel;
                            PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), inputElements.Count(), inputElements, ZoomLevel);
                            line.Parent = this;
                            _strings.Add(line);
                            _stringCount = _strings.Count();
                        }
                        // If we get here, there is nothing valid selected so add a string and move on
                        else
                        {
                            PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), 10, null, ZoomLevel);
                            line.StringType = StringType;
                            line.Parent = this;
                            _strings.Add(line);
                            _stringCount = _strings.Count();
                        }
                    }
                }
            }
        }

        public void EndCreation()
        {
            if (Creating)
            {
                Creating = false;
                _points.Remove(_selectedPoint);
                _strings.RemoveAt(_strings.Count() - 1);
                SelectDragPoints();
                Layout();
            }
        }

        public override void SelectDragPoints()
        {
            List<PreviewPoint> selectPoints = new List<PreviewPoint>();
            foreach (PreviewPoint point in _points)
            {
                selectPoints.Add(point);
            }
            SetSelectPoints(selectPoints, null);
        }

        public override bool PointInShape(PreviewPoint point)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2),
                                            SelectPointSize + PixelSize, SelectPointSize + PixelSize);
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
                if (pStart == null) pStart = new List<PreviewPoint>();
                pStart.Clear();
                foreach (PreviewPoint p in _points)
                {
                    pStart.Add(new PreviewPoint(p));
                }
            }

            _selectedPoint = point;
        }

        public override void SelectDefaultSelectPoint()
        {
            //_selectedPoint = _points[_points.Count-1];
            // The second point (default) gets set in MouseDown
        }

        public override object Clone()
        {
            PreviewMultiString newMultiString = (PreviewMultiString)this.MemberwiseClone();

			foreach (PreviewPixel pixel in _pixels)
			{
				newMultiString.AddPixel(pixel.X, pixel.Y);
			}

			newMultiString._points = new List<PreviewPoint>();
			foreach (var previewPoint in _points)
			{
				newMultiString._points.Add(previewPoint.Copy());
			}

			//Console.WriteLine("Clone");
			return newMultiString;
        }

        public override void MoveTo(int x, int y)
        {
            //PreviewPoint p = new PreviewPoint(x, y);
            //PointToZoomPoint(p);

            Top = y;
            Left = x;

            Layout();
        }

        public override void Resize(double aspect)
        {
            //_points[0].X = (int)(_points[0].X * aspect);
            //_points[0].Y = (int)(_points[0].Y * aspect);
            //_points[1].X = (int)(_points[1].X * aspect);
            //_points[1].Y = (int)(_points[1].Y * aspect);
            foreach (PreviewPoint point in _points)
            {
                point.X = Convert.ToInt32(Convert.ToDouble(point.X) * aspect);
                point.Y = Convert.ToInt32(Convert.ToDouble(point.Y) * aspect);
            }
            Layout();
        }

        public override void ResizeFromOriginal(double aspect)
        {
            //_points[0].X = p1Start.X;
            //_points[0].Y = p1Start.Y;
            //_points[1].X = p2Start.X;
            //_points[1].Y = p2Start.Y;
            for (int pNum = 0; pNum < pStart.Count(); pNum++) 
            {
                _points[pNum].X = pStart[pNum].X;
                _points[pNum].Y = pStart[pNum].Y;
            }
            Resize(aspect);
        }
    }
}