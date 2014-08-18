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
        [DataMember] private int _verticalSpacing;

        private PreviewPoint p1Start, p2Start;
        //const int InitialStringSpacing = 10;
        //const int InitialLightsPerString = 10;
        const int InitialPixelSpacing = 10;
        PreviewTools previewTools = new PreviewTools();

        public PreviewPolyLine(PreviewPoint point1, PreviewPoint point2, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			AddPoint(PointToZoomPoint(point1));

            if (selectedNode != null)
            {
                List<ElementNode> children = PreviewTools.GetLeafNodes(selectedNode);
                // is this a single node?
                if (children.Count == 0)
                {
                    StringType = StringTypes.Standard;
                    PixelCount = 2;
                    _pixels[0].PixelColor = Color.White;
                    if (selectedNode.IsLeaf)
                        _pixels[0].Node = selectedNode;
                    Creating = true;
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
                    Creating = true;
                }
            }
            else
            {
                Creating = true;
                CreateDefaultPixels = true;
                PixelCount = 2;
            }

			Layout();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

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
                Layout();
            }
        }

        public bool Creating { get; set; }
        public bool CreateDefaultPixels { get; set; }

        public override int Top
        {
            get
            {
                return (Math.Min(_points[0].Y, _points[1].Y));
            }
            set
            {
                if (_points[0].Y < _points[1].Y)
                {
                    int delta = _points[0].Y - value;
                    _points[0].Y = value;
                    _points[1].Y -= delta;
                }
                else
                {
                    int delta = _points[1].Y - value;
                    _points[0].Y -= delta;
                    _points[1].Y = value;
                }
                Layout();
            }
        }

        public override int Left
        {
            get
            {
                return (Math.Min(_points[0].X, _points[1].X));
            }
            set
            {
                if (_points[0].X < _points[1].X)
                {
                    int delta = _points[0].X - value;
                    _points[0].X = value;
                    _points[1].X -= delta;
                }
                else
                {
                    int delta = _points[1].X - value;
                    _points[0].X -= delta;
                    _points[1].X = value;
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
                //Console.WriteLine(">>>>>LineLength<<<<");
                double lineLength = 0;
                for (int pointNum = 0; pointNum < _points.Count - 1; pointNum++)
                {
                    //Console.WriteLine("pointNum: " + pointNum);
                    if (pointNum < _points.Count - 1)
                    {
                        //Console.WriteLine("Here1");
                        // Is this a horizontal line?
                        if (_points[pointNum].X == _points[pointNum + 1].X)
                        {
                            //Console.WriteLine("Here2");
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
                            //Console.WriteLine("Else: " + lineLength);
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
                //Console.WriteLine("-----> LineLength: " + lineLength);
                if (lineLength > 0)
                {
                    int currentPixelNum = 0;
                    double tailLength = PixelSpacing;
                    for (int pointNum = 0; pointNum < _points.Count - 1; pointNum++)
                    {
                        //Console.WriteLine("-------------------------------");
                        //Console.WriteLine("LineLength: " + lineLength + "   PixelCount: " + PixelCount);
                        // Length of the entire segment
                        double thisFullLineLength = SegmentLength(pointNum);
                        //Console.WriteLine("thisFullLineLength: " + thisFullLineLength);
                        // Empty portion of the start of the line segment 
                        double thisEmptyStartLength = PixelSpacing - tailLength;
                        //Console.WriteLine("thisEmptyStartLength: " + thisEmptyStartLength);
                        // Active length of the segment without the start
                        double thisActiveLineLength = thisFullLineLength - thisEmptyStartLength;
                        //Console.WriteLine("thisActiveLineLength: " + thisActiveLineLength);
                        // Get the pixels in this line. No hangers on the end.
                        double pixelSpacesInThisLine = Math.Truncate(thisActiveLineLength / PixelSpacing);
                        double pixelsInThisLine = pixelSpacesInThisLine + 1;
                        // Re-calcuate the active line length
                        thisActiveLineLength = pixelSpacesInThisLine * PixelSpacing;
                        //Console.WriteLine("thisActiveLineLength: " + thisActiveLineLength);
                        // Calculate the empty tail
                        tailLength = thisFullLineLength - thisEmptyStartLength - thisActiveLineLength;
                        if (tailLength < 0) tailLength = 0;

                        Point lineStartPoint = PreviewTools.CalculatePointOnLine(
                                                new PreviewTools.Vector2(_points[pointNum].ToPoint()),
                                                new PreviewTools.Vector2(_points[pointNum + 1].ToPoint()),
                                                Convert.ToInt32(thisEmptyStartLength));

                        Point lineEndPoint = PreviewTools.CalculatePointOnLine(
                                                new PreviewTools.Vector2(_points[pointNum].ToPoint()),
                                                new PreviewTools.Vector2(_points[pointNum + 1].ToPoint()),
                                                Convert.ToInt32(thisEmptyStartLength + thisActiveLineLength));

                        if (pixelSpacesInThisLine > 0)
                        {
                            double xSpacing = (double)(lineStartPoint.X - lineEndPoint.X) / (double)(pixelSpacesInThisLine);
                            double ySpacing = (double)(lineStartPoint.Y - lineEndPoint.Y) / (double)(pixelSpacesInThisLine);
                            double x = lineStartPoint.X;
                            double y = lineStartPoint.Y;

                            //double slope = ySpacing / xSpacing;

                            for (int pixelNum = 0; pixelNum < pixelsInThisLine; pixelNum++)
                            {
                                if (currentPixelNum < Pixels.Count-1)
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
                    }
                    // We're on the last pixel... need to adjust for rounding
                    Pixels[PixelCount-1].X = _points[_points.Count - 1].X;
                    Pixels[PixelCount-1].Y = _points[_points.Count - 1].Y;
                }

				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
            //Console.WriteLine("MouseMove: " + x.ToString() + ":" + y.ToString());
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
                //_points[0].X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
                //_points[0].Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
                //_points[1].X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
                //_points[1].Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

                //PointToZoomPointRef(_points[0]);
                //PointToZoomPointRef(_points[1]);

				Layout();
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
			if (point == null) {
				p1Start = new PreviewPoint(_points[0].X, _points[0].Y);
				p2Start = new PreviewPoint(_points[1].X, _points[1].Y);
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

			//newLine._pixels = new List<Preview Pixel>();

			foreach (PreviewPixel pixel in _pixels) {
				newLine.AddPixel(pixel.X, pixel.Y);
			}
			return newLine;
		}

		public override void MoveTo(int x, int y)
		{
			Point topLeft = new Point();
			topLeft.X = Math.Min(_points[0].X, _points[1].X);
			topLeft.Y = Math.Min(_points[0].Y, _points[1].Y);

			int deltaX = x - topLeft.X;
			int deltaY = y - topLeft.Y;

			_points[0].X += deltaX;
			_points[0].Y += deltaY;
			_points[1].X += deltaX;
			_points[1].Y += deltaY;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_points[0].X = (int) (_points[0].X*aspect);
			_points[0].Y = (int) (_points[0].Y*aspect);
			_points[1].X = (int) (_points[1].X*aspect);
			_points[1].Y = (int) (_points[1].Y*aspect);
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_points[0].X = p1Start.X;
			_points[0].Y = p1Start.Y;
			_points[1].X = p2Start.X;
			_points[1].Y = p2Start.Y;
			Resize(aspect);
		}
	}
}