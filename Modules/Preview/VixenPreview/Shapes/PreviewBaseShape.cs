using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using Vixen.Sys;
using Vixen.Data.Value;
using Vixen.Execution.Context;
using System.Collections.Concurrent;
using System.Threading;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public abstract class PreviewBaseShape : ICloneable, IDisposable
	{
		public string _name;

		public bool connectStandardStrings = false;
		public StringTypes _stringType = StringTypes.Standard;

		public enum StringTypes
		{
			Standard,
			Pixel,
			//            Flood
		}

		private bool _selected = false;
		[XmlIgnore] public List<PreviewPoint> _selectPoints = null;
		public const int SelectPointSize = 6;
		private Color _pixelColor = Color.White;
		public int _pixelSize = 2;
		public List<PreviewPixel> _pixels = new List<PreviewPixel>();

		[DataMember] public List<PreviewBaseShape> _strings;
		public PreviewPoint _selectedPoint;

		public delegate void OnPropertiesChangedHandler(object sender, PreviewBaseShape shape);

		public event OnPropertiesChangedHandler OnPropertiesChanged;

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			ResizePixels();
		}

		[DataMember,
		 CategoryAttribute("Settings"),
		 DescriptionAttribute("The name of this string. Used in templates to distinguish various strings."),
		 DisplayName("String Name")]
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				FireOnPropertiesChanged(this, this);
			}
		}

		/// <summary>
		/// Need to override if this is anywhere other than the top left in _pixels
		/// </summary>
		[Browsable(false)]
		public virtual int Top
		{
			get
			{
				int y = int.MaxValue;
				foreach (PreviewPixel pixel in Pixels) {
					y = Math.Min(y, pixel.Y);
				}
				return y;
			}
			set
			{
				int y = int.MaxValue;
				foreach (PreviewPixel pixel in Pixels) {
					y = Math.Min(y, pixel.Y);
				}
				int delta = value - y;
				foreach (PreviewPixel pixel in Pixels) {
					pixel.Y += delta;
				}
			}
		}

		/// <summary>
		/// Need to override if this is anywhere other than the bottom in _pixels
		/// </summary>
		[Browsable(false)]
		public virtual int Bottom
		{
			get
			{
				int y = int.MinValue;
				foreach (PreviewPixel pixel in Pixels) {
					y = Math.Max(y, pixel.Y);
				}
				return y;
			}
			set { }
		}

		/// <summary>
		/// Need to override if this is anywhere other than the top left in _pixels
		/// </summary>
		[Browsable(false)]
		public virtual int Left
		{
			get
			{
				int x = int.MaxValue;
				foreach (PreviewPixel pixel in Pixels) {
					x = Math.Min(x, pixel.X);
				}
				return x;
			}
			set
			{
				int x = int.MaxValue;
				foreach (PreviewPixel pixel in Pixels) {
					x = Math.Min(x, pixel.X);
				}
				int delta = value - x;
				foreach (PreviewPixel pixel in Pixels) {
					pixel.X += delta;
				}
			}
		}

		/// <summary>
		/// Need to override if this is anywhere other than the right in _pixels
		/// </summary>
		[Browsable(false)]
		public virtual int Right
		{
			get
			{
				int x = int.MinValue;
				foreach (PreviewPixel pixel in Pixels) {
					x = Math.Max(x, pixel.X);
				}
				return x;
			}
		}

        //[Browsable(false)]
        //public abstract int Width { get; set; }

        //[Browsable(false)]
        //public abstract int Height { get; set; }

        public abstract void Match(PreviewBaseShape matchShape);

		public abstract void Layout();

		[DataMember,
		 CategoryAttribute("Settings"),
		 DisplayName("String Type")]
		public virtual StringTypes StringType
		{
			get { return _stringType; }
			set
			{
				_stringType = value;
				if (_strings != null) {
					foreach (PreviewBaseShape line in _strings) {
						line.StringType = _stringType;
					}
				}
			}
		}

		[DataMember,
		 Browsable(false)]
		public virtual List<PreviewPixel> Pixels
		{
			get
			{
				if (_strings != null && _strings.Count > 0) {
					List<PreviewPixel> outPixels = new List<PreviewPixel>();
					foreach (PreviewBaseShape line in _strings) {
						foreach (PreviewPixel pixel in line.Pixels) {
							outPixels.Add(pixel);
						}
					}
					return outPixels.ToList();
				}
				else {
					return _pixels;
				}
			}
			set { _pixels = value; }
		}

		[Editor(typeof (PreviewSetElementsUIEditor), typeof (UITypeEditor)),
		 CategoryAttribute("Settings"),
		 DisplayName("Linked Elements")]
		public virtual List<PreviewBaseShape> Strings
		{
			get
			{
				if (_strings != null) {
					//Instead of going through the strings multiple times.. do it once
					// set all the sub-strings to match the connection state for elements
					foreach (PreviewBaseShape line in _strings)
						line.connectStandardStrings = this.connectStandardStrings;

					// Set all the StringTypes in the substrings
					foreach (PreviewBaseShape line in _strings) {
						line.StringType = _stringType;
					}
				}

				List<PreviewBaseShape> stringsResult = _strings;
				if (stringsResult == null) {
					stringsResult = new List<PreviewBaseShape>();
					stringsResult.Add(this);
				}
				return stringsResult;
			}
			set { _strings = value; }
		}

		[Browsable(false)]
		public Color PixelColor
		{
			get { return _pixelColor; }
			set
			{
				_pixelColor = value;
				foreach (PreviewPixel pixel in _pixels) {
					pixel.PixelColor = _pixelColor;
				}
			}
		}

		[DataMember,
		 CategoryAttribute("Settings"),
		 DescriptionAttribute("The size of the light point on the preview."),
		 DisplayName("Light Size")]
		public virtual int PixelSize
		{
			get { return _pixelSize; }
			set
			{
				_pixelSize = value;
				ResizePixels();
			}
		}

		public void SetPixelNode(int pixelNum, ElementNode node)
		{
			Pixels[pixelNum].Node = node;
		}

		public void SetPixelColor(int pixelNum, Color color)
		{
			Pixels[pixelNum].PixelColor = color;
		}

		public void SetColor(Color pixelColor)
		{
			foreach (PreviewPixel pixel in Pixels)
				pixel.PixelColor = pixelColor;
		}

		[Browsable(false)]
		public bool Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		public virtual void Select(bool selectDragPoints)
		{
			Selected = true;
			if (selectDragPoints)
				SelectDragPoints();
		}

		public abstract void SelectDragPoints();

		public virtual void Deselect()
		{
			_selected = false;
			if (_selectPoints != null)
				_selectPoints.Clear();
		}

		public void SetSelectPoints(List<PreviewPoint> selectPoints, List<PreviewPoint> skewPoints)
		{
			_selectPoints = selectPoints;
			foreach (PreviewPoint p in _selectPoints)
				if (p != null)
					p.PointType = PreviewPoint.PointTypes.Size;
		}

		// Add a pxiel at a specific location
		public PreviewPixel AddPixel(int x, int y)
		{
			PreviewPixel pixel = new PreviewPixel(x, y, 0, PixelSize);
			pixel.PixelColor = PixelColor;
			Pixels.Add(pixel);
			return pixel;
		}

		public virtual void ResizePixels()
		{
			if (Pixels != null) {
				foreach (PreviewPixel pixel in Pixels) {
					pixel.PixelSize = PixelSize;
				}
			}

			if (_strings != null && _strings.Count > 0) {
				foreach (PreviewBaseShape shape in _strings) {
					shape.PixelSize = PixelSize;
				}
			}
			else {
				if (Pixels != null) {
					foreach (PreviewPixel pixel in Pixels) {
						pixel.PixelSize = PixelSize;
						pixel.Resize();
					}
				}
			}
		}

		public virtual void Draw(Bitmap b, bool editMode, List<ElementNode> highlightedElements)
		{
			throw new NotImplementedException();
		}

		public virtual void DrawPixel(PreviewPixel pixel, FastPixel.FastPixel fp, bool editMode, List<ElementNode> highlightedElements,
		                              bool selected, bool forceDraw)
		{
			if (forceDraw) {
				pixel.Draw(fp, forceDraw);
			}
			else if (selected) {
				pixel.Draw(fp, PreviewTools.SelectedItemColor);
			}
			else if (highlightedElements != null && highlightedElements.Contains(pixel.Node)) {
				pixel.Draw(fp, Color.HotPink);
			}
			else {
				pixel.Draw(fp, Color.White);
			}
		}

		public virtual void Draw(FastPixel.FastPixel fp, bool editMode, List<ElementNode> highlightedElements, bool selected,
		                         bool forceDraw)
		{
			foreach (PreviewPixel pixel in Pixels) {
				DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
			}

			DrawSelectPoints(fp);
		}

		public void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint point in _selectPoints) {
					if (point != null) {
						if (point.PointType == PreviewPoint.PointTypes.Size) {
							fp.DrawRectangle(
								new Rectangle(point.X - (SelectPointSize/2), point.Y - (SelectPointSize/2), SelectPointSize, SelectPointSize),
								Color.White);
						}
					}
				}
			}
		}

		public void FireOnPropertiesChanged(Object sender, PreviewBaseShape shape)
		{
			if (OnPropertiesChanged != null)
				OnPropertiesChanged(sender, shape);
		}

		public abstract void MouseMove(int x, int y, int changeX, int changeY);

		public virtual void MouseUp(object sender, MouseEventArgs e)
		{
			FireOnPropertiesChanged(this, this);
		}

		public abstract bool PointInShape(PreviewPoint point);

		public virtual bool ShapeInRect(Rectangle rect)
		{
			foreach (PreviewPixel pixel in Pixels) {
				if (pixel.X >= rect.X &&
				    pixel.X <= rect.X + rect.Width &&
				    pixel.Y >= rect.Y &&
				    pixel.Y <= rect.Y + rect.Height) {
					return true;
				}
			}
			return false;
		}

		public PreviewPoint PointInSelectPoint(PreviewPoint point)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint selectPoint in _selectPoints) {
					if (selectPoint != null) {
						if (point.X >= selectPoint.X - (SelectPointSize/2) &&
						    point.Y >= selectPoint.Y - (SelectPointSize/2) &&
						    point.X <= selectPoint.X + (SelectPointSize/2) &&
						    point.Y <= selectPoint.Y + (SelectPointSize/2)) {
							return selectPoint;
						}
					}
				}
			}
			return null;
		}

		public abstract void SetSelectPoint(PreviewPoint point = null);

		public abstract void SelectDefaultSelectPoint();

		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}

		public abstract void MoveTo(int x, int y);

		public virtual void Nudge(int x, int y)
		{
			MoveTo(Left + x, Top + y);
		}

		public abstract void Resize(double aspect);

		public abstract void ResizeFromOriginal(double aspect);

		public DisplayItemBaseControl GetSetupControl()
		{
			Shapes.DisplayItemBaseControl setupControl = null;

			if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewSingle") {
				setupControl = new Shapes.PreviewSingleSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewLine") {
				setupControl = new Shapes.PreviewLineSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewRectangle") {
				setupControl = new Shapes.PreviewRectangleSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewEllipse") {
				setupControl = new Shapes.PreviewEllipseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewArch") {
				setupControl = new Shapes.PreviewArchSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMegaTree") {
				setupControl = new Shapes.PreviewMegaTreeSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewTriangle") {
				setupControl = new Shapes.PreviewTriangleSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewFlood") {
				setupControl = new Shapes.PreviewFloodSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCane") {
				setupControl = new Shapes.PreviewCaneSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStar") {
				setupControl = new Shapes.PreviewStarSetupControl(this);
			}
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStarBurst")
            {
                setupControl = new Shapes.PreviewStarBurstSetupControl(this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewNet")
            {
				setupControl = new Shapes.PreviewNetSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustom") {
				setupControl = new Shapes.PreviewCustomSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewPixelGrid") {
				setupControl = new Shapes.PreviewPixelGridSetupControl(this);
			}

			return setupControl;
		}

		~PreviewBaseShape()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_selectPoints != null)
					_selectPoints.Clear();
				_selectPoints = null;
				if (_pixels != null)
					_pixels.ForEach(p => p.Dispose());
				_pixels.Clear();
				_pixels = null;
				if (_strings != null)
					_strings.ForEach(s => s.Dispose());
				_strings = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}