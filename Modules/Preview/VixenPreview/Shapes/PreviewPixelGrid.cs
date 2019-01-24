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
using VixenModules.Property.Orientation;
using Orientation = VixenModules.Property.Orientation.Orientation;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewPixelGrid : PreviewBaseShape, ICloneable
	{
		[DataMember] private PreviewPoint _topLeft, _topRight;
		[DataMember] private PreviewPoint _bottomLeft, _bottomRight;
		[DataMember] private int _stringCount;
		[DataMember] private int _lightsPerString;
		[DataMember] private StringOrientations _stringOrientation = StringOrientations.Vertical;

		private PreviewPoint p1Start, p2Start;

		public override string TypeName => @"Pixel Grid";

		public enum StringOrientations 
		{
			Vertical, 
			Horizontal
		}

		public PreviewPixelGrid(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topRight = PointToZoomPoint(point1);
			_topLeft = PointToZoomPoint(point1);
			_bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);
            _bottomLeft = new PreviewPoint(_bottomRight);

			int defaultStringCount = 16;
			int defaultLightsPerString = 50;

			_strings = new List<PreviewBaseShape>();

			int childLightCount;
			if (IsPixelGridSelected(selectedNode, out childLightCount)) {
				StringType = StringTypes.Pixel;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), childLightCount, child, ZoomLevel);
					_strings.Add(line);
				}
				LightsPerString = childLightCount;
			}
			else if (IsStandardGridSelected(selectedNode)) {
				StringType = StringTypes.Standard;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), defaultLightsPerString, child, ZoomLevel);
					_strings.Add(line);
				}
			}
			else {
				// Just add the pixels, we don't care where they go... they get positioned in Layout()
				for (int stringNum = 0; stringNum < defaultStringCount; stringNum++) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), defaultLightsPerString, null, ZoomLevel);
					_strings.Add(line);
				}
			}
			if (childLightCount == -1)
			{
				LightsPerString = defaultLightsPerString;
			}
			StringCount = _strings.Count();

			if (selectedNode!= null && selectedNode.Properties.Contains(OrientationDescriptor._typeId))
			{
				var m = selectedNode.Properties.Get(OrientationDescriptor._typeId) as OrientationModule;
				if (m.Orientation == Orientation.Horizontal)
				{
					_stringOrientation = StringOrientations.Horizontal;
				}
			}
			// Lay out the pixels
			Layout();
		}

		private bool IsPixelGridSelected(ElementNode selectedNode, out int childLightCount)
		{
			int lastChildLightCount = -1;
			childLightCount = -1;
			if (selectedNode != null && selectedNode.Children != null) {
				int parentStringCount = selectedNode.Children.ToList().Count;
				// Selected node has to be a group!
				if (!selectedNode.IsLeaf && parentStringCount >= 2) {
					// Iterate through the strings in the grid
					parentStringCount = selectedNode.Children.ToList().Count;
					foreach (ElementNode parent in selectedNode.Children) {
						int childCount = parent.Children.ToList().Count;
						if (lastChildLightCount == -1) {
							lastChildLightCount = childCount;
						}
							// All the strings have to have the same light count for this to work!
						else if (lastChildLightCount != childCount) {
							return false;
						}
						lastChildLightCount = childCount;

						foreach (ElementNode child in parent.Children) {
							// If there are sub-groups this is not a grid element!
							if (!child.IsLeaf) {
								return false;
							}
						}
					}
				}

				if (lastChildLightCount >= 2 && parentStringCount >= 2) {
					childLightCount = lastChildLightCount;
					return true;
				}
				else {
					return false;
				}
			}
			else {
				return false;
			}
		}

		private bool IsStandardGridSelected(ElementNode selectedNode)
		{
			int parentStringCount = 0;
			// Selected node has to be a group!
			if (selectedNode != null && !selectedNode.IsLeaf) {
				// Iterate through the strings in the grid
				foreach (ElementNode parent in selectedNode.Children) {
					parentStringCount += 1;
					// If there are more groups, this is not a Grid
					if (!parent.IsLeaf)
						return false;
				}
			}
			// Gotta have at least 4 strings to make a Grid!
			return (parentStringCount >= 4);
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			_pixels.Clear();
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

		#region "Properties'

		[CategoryAttribute("Settings"),
		 DisplayName("Lights Per String"),
		 DescriptionAttribute("The number of lights on each string of the Pixel Grid")]
		public int LightsPerString
		{
			set
			{
				_lightsPerString = value;
				foreach (PreviewLine line in _strings) {
					line.PixelCount = _lightsPerString;
				}
			}
			get { return _lightsPerString; }
		}

		[CategoryAttribute("Settings"),
		 DisplayName("String Count"),
		 DescriptionAttribute("Number of strings on Pixel Grid")]
		public int StringCount
		{
			set
			{
				_stringCount = value;
				while (_strings.Count > _stringCount) {
					_strings.RemoveAt(_strings.Count - 1);
				}
				while (_strings.Count < _stringCount) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), LightsPerString, null, ZoomLevel);
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
			set 
            {
                int delta = Top - value;
                //_bottomLeft.Y = value + (_bottomLeft.Y - _topLeft.Y);
                _topLeft.Y = value;
                TopRight.Y = value;
                _bottomRight.Y -= delta;
                _bottomLeft.Y -= delta;
                Layout();
            }
		}

		public override int Left
		{
			get 
            { 
                return _topLeft.X; 
            }
			set 
            {
                int delta = Left - value;
                //bottomRight.X = value + (_bottomRight.X - _topLeft.X);
                _topLeft.X = value;
                _bottomLeft.X = value;
                TopRight.X -= delta;
                _bottomRight.X -= delta;
                Layout();
            }
		}

        public override int Right
        {
            get
            {
                return _bottomRight.X; ;
			}
        }

        public override int Bottom
        {
            get
            {
                return _bottomRight.Y;
			}
        }

		[CategoryAttribute("Position"),
		 DisplayName("Bottom Right"),
		 DescriptionAttribute("Lower right point of Pixel Grid.")]
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

		[Browsable(false)]
		public PreviewPoint BottomLeft
		{
			get { return _bottomLeft; }
			set
			{ _bottomLeft = value; }
		}

        private PreviewPoint TopRight
        {
            get
            {
                if (_topRight == null)
                {
                    _topRight = new PreviewPoint();
                }
                return _topRight;
            }
            set
            {
                _topRight = value;
            }
        }

		[CategoryAttribute("Position"),
		 DisplayName("Top Left"),
		 DescriptionAttribute("Upper left point of Pixel Grid.")]
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

		[CategoryAttribute("Settings"),
		 DisplayName("String Orientation"),
		 DescriptionAttribute("Orientation of strings in pixel grid. Vertical is Up/Down, Horizontal is Left/Right")]
		public StringOrientations StringOrientation 
		{ 
			get 
			{
				return _stringOrientation;
			} 
			set 
			{
				_stringOrientation = value;
				Layout();
			} 
		}

		#endregion

		[Browsable(false)]
		public int PixelCount
		{
			set
			{
				foreach (PreviewLine line in _strings) {
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
				if (_strings != null && _strings.Count > 0) {
					List<PreviewPixel> outPixels = new List<PreviewPixel>();
					for (int i = 0; i < Strings.Count; i++) {
						foreach (PreviewPixel pixel in _strings[i].Pixels) {
							outPixels.Add(pixel);
						}
					}
					return outPixels;
				}
				
				return _pixels;
			}
		}

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewPixelGrid shape = (matchShape as PreviewPixelGrid);
            PixelSize = shape.PixelSize;
            StringOrientation = shape.StringOrientation;

            _bottomRight.X = _topLeft.X + (shape._bottomRight.X - shape._topLeft.X);
            _bottomRight.Y = _topLeft.Y + (shape._bottomRight.Y - shape.TopRight.Y);

            Layout();
        }

		public override void Layout()
		{
			if (_bottomRight != null && _topLeft != null)
			{
				if (StringOrientation == StringOrientations.Vertical)
				{
					int width = _bottomRight.X - _topLeft.X;
					int height = _bottomRight.Y - _topLeft.Y;
					double stringXSpacing = (double)width / (double)(StringCount - 1);
					double x = _topLeft.X;
					int y = _topLeft.Y;
					for (int stringNum = 0; stringNum < StringCount; stringNum++)
					{
						PreviewLine line = _strings[stringNum] as PreviewLine;
						var x1 = (int)Math.Round(x, MidpointRounding.AwayFromZero);
						line.SetPoint0(x1, y + height);
						line.SetPoint1(x1, y);
						line.Layout();
						x += stringXSpacing;
					}
				}
				else
				{
					int width = _bottomRight.X - _bottomLeft.X;
					int height = _bottomLeft.Y - _topLeft.Y;
					double stringYSpacing = height / (double)(StringCount - 1);
					int x = _bottomLeft.X;
					double y = _bottomLeft.Y;
					for (int stringNum = 0; stringNum < StringCount; stringNum++)
					{
						PreviewLine line = _strings[stringNum] as PreviewLine;
						var y1 = (int)Math.Round(y, MidpointRounding.AwayFromZero);
						line.SetPoint0(x, y1);
						line.SetPoint1((x + width), y1);
						line.Layout();
						y -= stringYSpacing;
					}
				}
				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size) {
				if (_selectedPoint == TopRight) {
					_topLeft.Y = point.Y;
					_bottomRight.X = point.X;
				}
				else if (_selectedPoint == _bottomLeft) {
					_topLeft.X = point.X;
					_bottomRight.Y = point.Y;
				}
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
			}
				// If we get here, we're moving
			else {
				//_topLeft.X = p1Start.X + changeX;
				//_topLeft.Y = p1Start.Y + changeY;
				//_bottomRight.X = p2Start.X + changeX;
				//_bottomRight.Y = p2Start.Y + changeY;
				_topLeft.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_topLeft.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_bottomRight.X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_bottomRight.Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_bottomRight);
			}

			TopRight.X = _bottomRight.X;
			TopRight.Y = _topLeft.Y;
			_bottomLeft.X = _topLeft.X;
			_bottomLeft.Y = _bottomRight.Y;

			// Layout the standard shape
			Layout();
		}

		public override void SelectDragPoints()
		{
			// Create the size points
			List<PreviewPoint> selectPoints = new List<PreviewPoint>();

			selectPoints.Add(_topLeft);
			selectPoints.Add(_bottomRight);
			TopRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
			selectPoints.Add(TopRight);
			_bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
			selectPoints.Add(_bottomLeft);

			// Tell the base shape about the newely created points            
			SetSelectPoints(selectPoints, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			if (_strings != null) {
				foreach (PreviewLine line in _strings) {
					if (line.PointInShape(point))
						return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(_topLeft.X, _topLeft.Y);
				p2Start = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
		}

		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                          bool forceDraw)
		{
			if (_strings != null) {
				for (int i = 0; i < _strings.Count; i++) {
					foreach (PreviewPixel pixel in _strings[i]._pixels) {
						DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
					}
				}
			}

			base.Draw(fp, editMode, highlightedElements, selected, forceDraw);
		}

		public override object Clone()
		{
			PreviewPixelGrid newGrid = (PreviewPixelGrid) this.MemberwiseClone();

			newGrid._strings = new List<PreviewBaseShape>();
			foreach (PreviewBaseShape line in _strings) {
				PreviewBaseShape newLine = (PreviewLine) line.Clone();
				newGrid._strings.Add(newLine);
			}
			newGrid._topLeft = new PreviewPoint(_topLeft);
			newGrid._bottomRight = new PreviewPoint(_bottomRight);

			return newGrid;
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

			TopRight.X = _bottomRight.X;
			TopRight.Y = _topLeft.Y;
			_bottomLeft.X = _topLeft.X;
			_bottomLeft.Y = _bottomRight.Y;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeft.X = (int) (_topLeft.X*aspect);
			_topLeft.Y = (int) (_topLeft.Y*aspect);
			_bottomRight.X = (int) (_bottomRight.X*aspect);
			_bottomRight.Y = (int) (_bottomRight.Y*aspect);

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
