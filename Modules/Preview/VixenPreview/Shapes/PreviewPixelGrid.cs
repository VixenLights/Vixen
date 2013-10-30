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
	public class PreviewPixelGrid : PreviewBaseShape, ICloneable
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _bottomRight;
		[DataMember] private int _stringCount;
		[DataMember] private int _lightsPerString;
		[DataMember] private PreviewPoint _topRight, _bottomLeft;
		[DataMember] private StringOrientations _stringOrientation = StringOrientations.Vertical;

		private PreviewPoint p1Start, p2Start;

		public enum StringOrientations 
		{
			Vertical, 
			Horizontal
		}

		public PreviewPixelGrid(PreviewPoint point1, ElementNode selectedNode)
		{
			_topLeft = point1;
			_bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

			int defaultStringCount = 16;
			int defaultLightsPerString = 50;

			_strings = new List<PreviewBaseShape>();

			int childLightCount;
			if (IsPixelGridSelected(selectedNode, out childLightCount)) {
				StringType = StringTypes.Pixel;
				//_lightsPerString = childLightCount;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), childLightCount, child);
					_strings.Add(line);
				}
				//StringCount = _strings.Count;
				LightsPerString = childLightCount;
			}
			else if (IsStandardGridSelected(selectedNode)) {
				StringType = StringTypes.Standard;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), defaultLightsPerString, child);
					_strings.Add(line);
				}
				//StringCount = _strings.Count;
			}
			else {
				// Just add the pixels, we don't care where they go... they get positioned in Layout()
				for (int stringNum = 0; stringNum < defaultStringCount; stringNum++) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), defaultLightsPerString, null);
					_strings.Add(line);
				}
			}

			StringCount = _strings.Count();

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
				if (!selectedNode.IsLeaf && parentStringCount >= 4) {
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

				if (lastChildLightCount > 4 && parentStringCount >= 4) {
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

		public int StringCount
		{
			set
			{
				_stringCount = value;
				while (_strings.Count > _stringCount) {
					_strings.RemoveAt(_strings.Count - 1);
				}
				while (_strings.Count < _stringCount) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), LightsPerString, null);
					_strings.Add(line);
				}
				Layout();
			}
			get { return _stringCount; }
		}

		public override int Top
		{
			get { return _topLeft.Y; }
			set { }
		}

		public override int Left
		{
			get { return _topLeft.X; }
			set { }
		}

		public PreviewPoint BottomRight
		{
			get { return _bottomRight; }
			set { _bottomRight = value; }
		}

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
				else {
					return _pixels;
				}
			}
			set
			{
				_pixels = value;
			}
		}

		public override void Layout()
		{
			if (StringOrientation == StringOrientations.Vertical)
			{
				int width = _bottomRight.X - _topLeft.X;
				int height = _bottomRight.Y - _topLeft.Y;
				double stringXSpacing = (double)width / (double)(StringCount-1);
				int x = _topLeft.X;
				int y = _topLeft.Y;
				for (int stringNum = 0; stringNum < StringCount; stringNum++)
				{
					PreviewLine line = _strings[stringNum] as PreviewLine;
					line.SetPoint0(x, y + height);
					line.SetPoint1(x, y);
					line.Layout();
					x += (int)stringXSpacing;
				}
			}
			else
			{
				int width = _bottomRight.X - _bottomLeft.X;
				int height = _bottomLeft.Y - _topLeft.Y;
				double stringYSpacing = (double)height / (double)(StringCount-1);
				int x = _bottomLeft.X;
				int y = _bottomLeft.Y;
				for (int stringNum = 0; stringNum < StringCount; stringNum++)
				{
					PreviewLine line = _strings[stringNum] as PreviewLine;
					line.SetPoint0(x, y);
					line.SetPoint1(x + width, y);
					line.Layout();
					y -= (int)stringYSpacing;
				}
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			// See if we're resizing
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size) {
				if (_selectedPoint == _topRight) {
					_topLeft.Y = y;
					_bottomRight.X = x;
				}
				else if (_selectedPoint == _bottomLeft) {
					_topLeft.X = x;
					_bottomRight.Y = y;
				}
				_selectedPoint.X = x;
				_selectedPoint.Y = y;
			}
				// If we get here, we're moving
			else {
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

		public override void Draw(FastPixel.FastPixel fp, bool editMode, List<ElementNode> highlightedElements, bool selected,
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

		//[Editor(typeof (PreviewSetElementsUIEditor), typeof (UITypeEditor)),
		// CategoryAttribute("Settings"),
		// DisplayName("Linked Elements")]
		//public override List<PreviewBaseShape> Strings
		//{
		//	get
		//	{
		//		Layout();
		//		List<PreviewBaseShape> stringsResult;
		//		stringsResult = _strings;
		//		if (stringsResult == null) {
		//			stringsResult = new List<PreviewBaseShape>();
		//			stringsResult.Add(this);
		//		}
		//		return stringsResult;
		//	}
		//	set { }
		//}

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