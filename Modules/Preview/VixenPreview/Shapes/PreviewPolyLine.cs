using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Sys;
using System.Drawing.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewPolyLine : PreviewBaseShape
	{
        [DataMember] private List<PreviewPoint> _points = new List<PreviewPoint>();
        //[DataMember] private int _verticalSpacing;

        //private PreviewPoint p1Start, p2Start;
        private List<PreviewPoint> pStart = new List<PreviewPoint>();
        const int InitialPixelSpacing = 10;
		public override string TypeName => @"Poly Line";

		public PreviewPolyLine(PreviewPoint point1, PreviewPoint point2, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			AddPoint(PointToZoomPoint(point1));
			Reconfigure(selectedNode);
			Creating = true;
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		internal sealed override void Reconfigure(ElementNode node)
		{
			if (node != null)
			{
				List<ElementNode> children = PreviewTools.GetLeafNodes(node);
				// is this a single node?
				if (children.Count == 0)
				{
					StringType = StringTypes.Standard;
					PixelCount = 2;
					_pixels[0].PixelColor = Color.White;
					if (node.IsLeaf)
						_pixels[0].Node = node;
					//Creating = true;
					CreateDefaultPixels = true;
				}
				else
				{
					StringType = StringTypes.Pixel;
					PixelCount = children.Count;
					// Just add the pixels, they will get layed out next
					int pixelNum = 0;
					foreach (ElementNode child in children)
					{
						{
							PreviewPixel pixel = _pixels[pixelNum];
							pixel.Node = child;
							pixel.NodeId = child.Id;
							pixel.PixelColor = Color.White;
						}
						pixelNum++;
					}
					//Creating = true;
				}
			}
			else
			{
				//Creating = true;
				CreateDefaultPixels = true;
				PixelCount = 2;
			}

			if (Creating == true)
			{
				EndCreation();
			}
			Layout();
		}

		#endregion

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

        //[DataMember,
        // CategoryAttribute("Settings"),
        // DisplayName("String Type")]
        //public override StringTypes StringType
        //{
        //    get
        //    {
        //        return _stringType;
        //    }
        //    set
        //    {
        //        _stringType = value;
        //        //Console.WriteLine(StringType);
        //        AssignStandardPixels();
        //    }
        //}

        [CategoryAttribute("Settings"),
         DisplayName("Light Count"),
         DescriptionAttribute("Number of pixels or lights in the string.")]
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

                AssignStandardPixels();

                Layout();
            }
        }

        //[Editor(typeof(PreviewSetElementsUIEditor), typeof(UITypeEditor)),
        // CategoryAttribute("Settings"),
        // DisplayName("Linked Elements")]
        //public virtual List<PreviewBaseShape> Strings
        //{
        //    get
        //    {
        //        if (_strings != null)
        //        {
        //            //Instead of going through the strings multiple times.. do it once
        //            // set all the sub-strings to match the connection state for elements
        //            foreach (PreviewBaseShape line in _strings)
        //                line.connectStandardStrings = this.connectStandardStrings;

        //            // Set all the StringTypes in the substrings
        //            foreach (PreviewBaseShape line in _strings)
        //            {
        //                line.StringType = _stringType;
        //            }
        //        }

        //        List<PreviewBaseShape> stringsResult = _strings;
        //        if (stringsResult == null)
        //        {
        //            stringsResult = new List<PreviewBaseShape>();
        //            stringsResult.Add(this);
        //        }
        //        return stringsResult;
        //    }
        //    set { _strings = value; }
        //}

        [Browsable(false)]
        public bool CreateDefaultPixels { get; set; }

        public override int Top
        {
            get
            {
                int t = int.MaxValue;
                foreach (PreviewPoint p in _points)
                {
                    t = Math.Min(t, p.Y);
                }
                return t;
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
                int l = int.MaxValue;
                foreach (PreviewPoint p in _points)
                {
                    l = Math.Min(l, p.X);
                }
                return l;
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

        public override int Right
        {
            get
            {
                int xMax = 0;
                foreach (PreviewPoint p in _points)
                {
                    xMax = Math.Max(xMax, p.X);
                }
                return xMax;
			}
        }

        public override int Bottom
        {
            get
            {
                int yMax = 0;
                foreach (PreviewPoint p in _points)
                {
                    yMax = Math.Max(yMax, p.Y);
                }
                return yMax;
			}
        }

        public void AddPoint(PreviewPoint point)
        {
            _points.Add(point);
        }

        public override void Match(PreviewBaseShape matchShape)
        {
            //PreviewPolyLine shape = (matchShape as PreviewPolyLine);
            //PixelSize = shape.PixelSize;
            //_points[1].X = _points[0].X + (shape._points[1].X - shape._points[0].X);
            //_points[1].Y = _points[0].Y + (shape._points[1].Y - shape._points[0].Y);
            //Layout();
        }

        /// <summary>
        /// Provide the length of the entire PolyLine
        /// </summary>
        private double LineLength
        {
            get
            {
                double lineLength = 0;
                for (int pointNum = 0; pointNum < _points.Count - 1; pointNum++)
                {
                    if (pointNum < _points.Count - 1)
                    {
                        // Is this a horizontal line?
                        if (_points[pointNum].X == _points[pointNum + 1].X)
                        {
                            lineLength += Math.Abs(_points[pointNum].Y - _points[pointNum + 1].Y);
                        }
                        // Is this a vertical line?
                        else if (_points[pointNum].Y == _points[pointNum + 1].Y)
                        {
                            lineLength += Math.Abs(_points[pointNum].X - _points[pointNum + 1].X);
                        }
                        else
                        {
                            // Find the length of the Hypotenuse of the right triangle
                            double a = Math.Abs(_points[pointNum].X - _points[pointNum + 1].X);
                            double b = Math.Abs(_points[pointNum].Y - _points[pointNum + 1].Y);
                            lineLength += Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                        }
                    }
                }
                return lineLength;
            }
        }

        private double PixelSpacing
        {
            get
            {
                return LineLength / Convert.ToDouble(PixelCount - 1);
            }
        }

        public override void Select(bool selectDragPoints)
        {
            base.Select(selectDragPoints);
            connectStandardStrings = true;
        }

        private double SegmentLength(int segmentNum)
        {
            // Get the length of just this line
            double thisLineLength = 0;
            // Is this a horizontal line?
            if (_points[segmentNum].X == _points[segmentNum + 1].X)
            {
                thisLineLength = Math.Abs(_points[segmentNum].Y - _points[segmentNum + 1].Y);
            }
            // Is this a vertical line?
            else if (_points[segmentNum].Y == _points[segmentNum + 1].Y)
            {
                thisLineLength = Math.Abs(_points[segmentNum].X - _points[segmentNum + 1].X);
            }
            else
            {
                // Find the length of the Hypotenuse of the right triangle
                double a = Math.Abs(_points[segmentNum].X - _points[segmentNum + 1].X);
                double b = Math.Abs(_points[segmentNum].Y - _points[segmentNum + 1].Y);
                thisLineLength = PreviewTools.TriangleHypotenuse(a, b);
            }
            return thisLineLength;
        }

		public override void Layout()
		{
			if (_points != null && _points.Count > 0)
			{
                double lineLength = LineLength;
                int currentPixelNum = 0;
                if (lineLength > 0)
                {
                    // Tracks the spacing between each of the pixels without worrying about the points.
                    double currentEmptySpace = 0;
                    // Length of the entire segment
                    for (int pointNum = 0; pointNum < _points.Count - 1; pointNum++)
                    {
                        double thisFullLineLength = SegmentLength(pointNum);
                        // First pixel is a special case
                        if (currentEmptySpace + thisFullLineLength > PixelSpacing || pointNum == 0)
                        {
                            double thisEmptyStartLength = PixelSpacing - currentEmptySpace;;
                            if (pointNum == 0)
                            {
                                thisEmptyStartLength = 0;
                            }
                            // Active length of the segment without the start
                            double thisActiveLineLength = thisFullLineLength - thisEmptyStartLength;
                            // Get the pixels in this line. No hangers on the end.
                            double pixelSpacesInThisLine = Math.Truncate(thisActiveLineLength / PixelSpacing);
                            //if (pixelSpacesInThisLine < 0) pixelSpacesInThisLine = 0;
                            double pixelsInThisLine = pixelSpacesInThisLine + 1;
                            // Re-calcuate the active line length
                            thisActiveLineLength = pixelSpacesInThisLine * PixelSpacing;
                            
                            currentEmptySpace = thisFullLineLength - thisEmptyStartLength - thisActiveLineLength;

                            Point lineStartPoint = PreviewTools.CalculatePointOnLine(
                                                    new PreviewTools.Vector2(_points[pointNum].ToPoint()),
                                                    new PreviewTools.Vector2(_points[pointNum + 1].ToPoint()),
                                                    Convert.ToInt32(thisEmptyStartLength));

                            Point lineEndPoint = PreviewTools.CalculatePointOnLine(
                                                    new PreviewTools.Vector2(_points[pointNum].ToPoint()),
                                                    new PreviewTools.Vector2(_points[pointNum + 1].ToPoint()),
                                                    Convert.ToInt32(thisEmptyStartLength + thisActiveLineLength));

                            double xSpacing = (double)(lineStartPoint.X - lineEndPoint.X) / (double)(pixelSpacesInThisLine);
                            double ySpacing = (double)(lineStartPoint.Y - lineEndPoint.Y) / (double)(pixelSpacesInThisLine);
                            double x = lineStartPoint.X;
                            double y = lineStartPoint.Y;

                            for (int pixelNum = 0; pixelNum < pixelsInThisLine; pixelNum++)
                            {
                                if (currentPixelNum < Pixels.Count - 1)
                                {
                                    Pixels[currentPixelNum].X = Convert.ToInt32(x);
                                    Pixels[currentPixelNum].Y = Convert.ToInt32(y);
                                    x -= xSpacing;
                                    y -= ySpacing;
                                }

                                currentPixelNum++;
                            }
                            double a1 = _points[pointNum + 1].X - x;
                            double b1 = _points[pointNum + 1].Y - y;
                        }
                        else
                        {
                            currentEmptySpace += thisFullLineLength;
                        }

                        // Finally, put the last dot on the line
                        Pixels[PixelCount - 1].X = _points[_points.Count - 1].X;
                        Pixels[PixelCount - 1].Y = _points[_points.Count - 1].Y;
                    }
                }

                SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null) {
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
                if (CreateDefaultPixels)
                {
                    PixelCount = Math.Max(2, Convert.ToInt32(LineLength) / InitialPixelSpacing);
                }
				Layout();
				SelectDragPoints();
			}
				// If we get here, we're moving
			else {
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
                    _selectedPoint = newPoint;
                }
                if (CreateDefaultPixels)
                {
                    PixelCount = Math.Max(2, Convert.ToInt32(LineLength) / InitialPixelSpacing);
                }
            }
            SelectDragPoints();
            Layout();
        }

        public void EndCreation() 
        {
            _points.Remove(_selectedPoint);
            if (CreateDefaultPixels) 
                PixelCount = Math.Max(2, Convert.ToInt32(LineLength) / InitialPixelSpacing);
            Creating = false;
            CreateDefaultPixels = false;
            SelectDragPoints();
            Layout();
        }

        //public void MouseDown(object sender, System.Windows.Forms.MouseButtons e)
        //{

        //}

		public override void SelectDragPoints()
		{
            List<PreviewPoint> selectPoints = new List<PreviewPoint>();
            foreach (PreviewPoint point in _points)
            {
                selectPoints.Add(point);
            }
            SetSelectPoints(selectPoints, null);

            //if (_points.Count >= 2) {
            //    List<PreviewPoint> selectPoints = new List<PreviewPoint>();
            //    selectPoints.Add(_points[0]);
            //    selectPoints.Add(_points[1]);
            //    SetSelectPoints(selectPoints, null);
            //}
		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels) {
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize/2), pixel.Y - (SelectPointSize/2),
				                            SelectPointSize + PixelSize, SelectPointSize + PixelSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height) {
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
            PreviewPolyLine newLine = (PreviewPolyLine)this.MemberwiseClone();

			newLine._pixels = new List<PreviewPixel>();

			foreach (PreviewPixel pixel in _pixels) {
				newLine._pixels.Add(pixel.Clone());
			}

            newLine._points = new List<PreviewPoint>();
            foreach (var previewPoint in _points)
            {
	            newLine._points.Add(previewPoint.Copy());
            }
			return newLine;
		}

		public override void MoveTo(int x, int y)
		{
            PreviewPoint p = new PreviewPoint(x, y);
            PointToZoomPoint(p);

            Top = p.Y;
            Left = p.X;

            Layout();
		}

		public override void Resize(double aspect)
		{
            foreach (PreviewPoint point in _points)
            {
                point.X = Convert.ToInt32(Convert.ToDouble(point.X) * aspect);
                point.Y = Convert.ToInt32(Convert.ToDouble(point.Y) * aspect);
            } 
            Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
            for (int pNum = 0; pNum < pStart.Count(); pNum++)
            {
                _points[pNum].X = pStart[pNum].X;
                _points[pNum].Y = pStart[pNum].Y;
            }
            Resize(aspect);
		}

        private void AssignStandardPixels()
        {
            if (StringType == StringTypes.Standard)
            {
                foreach (PreviewPixel p in _pixels)
                {
                    p.Node = _pixels[0].Node;
                }
            }
        }
	}
}