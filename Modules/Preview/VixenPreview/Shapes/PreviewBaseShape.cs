using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex;
using Point = System.Drawing.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public abstract class PreviewBaseShape : ICloneable, IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private List<float> _points = new List<float>();
		public string _name;
		public static int SevenFloatDataSize = 7 * Marshal.SizeOf(typeof(float));
		public static int EightFloatDataSize = 8 * Marshal.SizeOf(typeof(float));
		public static int FloatDataSize = Marshal.SizeOf(typeof(float));
		public bool connectStandardStrings = false;
		public StringTypes _stringType = StringTypes.Standard;
		private bool _isHighPrecision;

		private List<PreviewPixel> _pixelCache = new List<PreviewPixel>();

		public enum StringTypes
		{
			Standard,
			Pixel,
			Custom
			//            Flood
		}

        public enum StringDirections
        {
            Clockwise,
            CounterClockwise
        }

		private bool _selected = false;
		[XmlIgnore] public List<PreviewPoint> _selectPoints = null;
		public const int SelectPointSize = 6;
		private Color _pixelColor = Color.White;
		public int _pixelSize = 3;

		[DataMember(Name = "Pixels")]
		public List<PreviewPixel> _pixels = new List<PreviewPixel>();

		[DataMember(EmitDefaultValue = false)]
		public List<PreviewBaseShape> _strings;

		public PreviewPoint _selectedPoint;

		public delegate void OnPropertiesChangedHandler(object sender, PreviewBaseShape shape);

		public event OnPropertiesChangedHandler OnPropertiesChanged;

		internal virtual void Reconfigure(ElementNode node)
		{

		}

		public void UpdateColorType()
		{
			foreach (var previewPixel in Pixels)
			{
				previewPixel.TestForDiscrete();
			}
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			
			ResizePixels();
		}

		[DataMember(EmitDefaultValue = false), 
		 Category("Settings"),
		 Description("The name of this string. Used in templates to distinguish various strings."),
		 DisplayName("String Name")]
		public string Name
		{
			get
			{
                return _name; 
            }
			set
			{
				_name = string.IsNullOrEmpty(value) ? VixenPreviewSetup3.DrawShape : value;
				FireOnPropertiesChanged(this, this);
			}
		}

		/// <summary>
		/// Display name for the type of shape.
		/// </summary>
		[Browsable(false)]
		public virtual string TypeName => @"Shape";

		/// <summary>
		/// Top most pixel location
		/// </summary>
		[Browsable(false)]
        public abstract int Top { get; set; }

		/// <summary>
		/// Bottom most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Bottom { get; }

		/// <summary>
		/// Left most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Left { get; set; }

		/// <summary>
		/// Right most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Right { get; }

		public abstract void Match(PreviewBaseShape matchShape);

		public abstract void Layout();

		[DataMember,
		 Category("Settings"),
		 DisplayName("String Type")]
		public virtual StringTypes StringType
		{
			get { return _stringType; }
			set
			{
				_stringType = value;
				_isHighPrecision = _stringType == StringTypes.Custom;
				if (_strings != null) {
					foreach (var line in _strings) {
						line.StringType = _stringType;
					}
				}
			}
		}

		[Browsable(false)]
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
					_pixels.Clear();
					return outPixels.ToList();
				}
				
				return _pixels;
			}
			set { _pixels = value; }
		}

        [Browsable(false)]
        public PreviewBaseShape Parent { get; set; }

		[Editor(typeof (PreviewSetElementsUIEditor), typeof (UITypeEditor)),
		 Category("Settings"),
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
		 Category("Settings"),
		 Description("The size of the light point on the preview."),
		 DisplayName("Light Size")]
		public virtual int PixelSize
		{
			get { return _pixelSize; }
			set
			{
				if (value > 0)
				{
					_pixelSize = value;
				}
				else if(_pixelSize <= 0)
				{
					//set a reasonable default.
					_pixelSize = 3;
				}
				ResizePixels();
			}
		}

		public virtual void MatchPixelSize(PreviewBaseShape shape)
		{
			PixelSize = shape.PixelSize;
		}

		public virtual void ResizePixelsBy(int value)
		{
			var newSize = PixelSize + value;
			PixelSize = newSize > 0 ? newSize : 1;
		}

		private double _zoomLevel = 1;
		

		[Browsable(false)]
		public virtual double ZoomLevel
		{
			get
			{
				if (_zoomLevel <= 0)
					_zoomLevel = 1;
				return _zoomLevel;
			}
			set
			{
				_zoomLevel = value;
				if (_strings != null)
				{
					foreach (PreviewBaseShape shape in _strings)
					{
						shape.ZoomLevel = value;
					}
				}

				Layout();
			}
		}

		public void SetPixelZoom() 
		{
			// Zoom
			if (ZoomLevel == 1) return;
			foreach (PreviewPixel pixel in _pixels)
			{
				pixel.X = (int)(pixel.X * ZoomLevel);
				pixel.Y = (int)(pixel.Y * ZoomLevel);
			}
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

		// Add a pixel at a specific location
		public PreviewPixel AddPixel(int x, int y)
		{
			return AddPixel(x, y, 0, PixelSize);
		}

		public PreviewPixel AddPixel(int x, int y, int z, int size)
		{
			PreviewPixel pixel = new PreviewPixel(x, y, z, size);
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


		protected static Rect GetCombinedBounds(IEnumerable<Rect> recs)
		{
			if(!recs.Any()) return new Rect(0,0,0,0);
			double xMin = recs.Min(s => s.X);
			double yMin = recs.Min(s => s.Y);
			double xMax = recs.Max(s => s.X + s.Width);
			double yMax = recs.Max(s => s.Y + s.Height);
			var rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			return rect;
		}

		public virtual void Draw(Bitmap b, bool editMode, HashSet<Guid> highlightedElements)
		{
			throw new NotImplementedException();
		}

		public virtual void DrawPixel(PreviewPixel pixel, FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements,
		                              bool selected, bool forceDraw)
		{
			int origPixelSize = pixel.PixelSize;
            if (forceDraw)
            {
                pixel.Draw(fp, forceDraw);
            }
            else
            {
                Color pixelColor = Color.White;
                if (StringType==StringTypes.Pixel && IsPixelOne(pixel))
	            {
		            pixelColor = Color.Yellow;
		            pixel.PixelSize = PixelSize + 2;
	      
                }
                else
                {
	                if (selected)
	                {
		                pixelColor = PreviewTools.SelectedItemColor;
	                }
	                else
	                {
		                if (pixel.NodeId != Guid.Empty)
		                {
			                if (highlightedElements.Contains(pixel.NodeId))
			                {
				                pixelColor = Color.HotPink;
			                }
			                else
			                {
								pixelColor = Color.Turquoise;
							}
						}
	                } 
                }
                pixel.Draw(fp, pixelColor);
				//Restore the size if we changed it.
	            pixel.PixelSize = origPixelSize;
            }
		}

		private bool IsPixelOne(PreviewPixel pixel)
		{
			if (_pixels.Count > 0 && pixel == _pixels[0] ||
				 _strings != null && _strings.Count > 0 && _strings[0].Pixels != null &&
				 _strings[0].Pixels.Count > 0 && _strings[0].Pixels[0] == pixel)
			{
				return true;
			}

			return false;
		}

		public virtual void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                         bool forceDraw)
		{
			foreach (PreviewPixel pixel in Pixels) {

				DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
			}

			DrawSelectPoints(fp);
		}

		public virtual void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint point in _selectPoints) {
					if (point != null) {
						if (point.PointType == PreviewPoint.PointTypes.Size) {
                            int x = Convert.ToInt32((point.X) * ZoomLevel) - (SelectPointSize / 2);
                            int y = Convert.ToInt32(point.Y * ZoomLevel) - (SelectPointSize / 2);
                            fp.DrawRectangle(
								new Rectangle(x, y, SelectPointSize, SelectPointSize),
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
            foreach (PreviewPixel pixel in Pixels)
            {
                int X1 = Math.Min(rect.X, rect.X + rect.Width);
                int X2 = Math.Max(rect.X, rect.X + rect.Width);
                int Y1 = Math.Min(rect.Y, rect.Y + rect.Height);
                int Y2 = Math.Max(rect.Y, rect.Y + rect.Height);
                if (_isHighPrecision)
                {
	                if (pixel.Location.X >= X1 &&
	                    pixel.Location.X <= X2 &&
	                    pixel.Location.Y >= Y1 &&
	                    pixel.Location.Y <= Y2)
	                {
		                return true;
	                }
                }
                else
                {
	                if (pixel.X >= X1 &&
	                    pixel.X <= X2 &&
	                    pixel.Y >= Y1 &&
	                    pixel.Y <= Y2)
	                {
		                return true;
	                }
                }
                
            }
			return false;
		}

        public virtual bool ShapeAllInRect(Rectangle rect)
        {
            PreviewPoint p1 = PointToZoomPoint(new PreviewPoint(rect.X, rect.Y));
            PreviewPoint p2 = PointToZoomPoint(new PreviewPoint(rect.X + rect.Width, rect.Y + rect.Height));
            int X1 = Math.Min(p1.X, p2.X);
            int X2 = Math.Max(p1.X, p2.X);
            int Y1 = Math.Min(p1.Y, p2.Y);
            int Y2 = Math.Max(p1.Y, p2.Y);
            //Console.WriteLine(Top + ":" + Y1 + "  " + Bottom + ":" + Y2 + "  " + Left + ":" + X1 + "  " + Right + ":" + X2);
            return (Top >= Y1 && Bottom <= Y2 && Left >= X1 && Right <= X2);
        }

		public virtual PreviewPoint PointInSelectPoint(PreviewPoint point)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint selectPoint in _selectPoints) {
					if (selectPoint != null) {
						int selectPointX = Convert.ToInt32(selectPoint.X * ZoomLevel);
						int selectPointY = Convert.ToInt32(selectPoint.Y * ZoomLevel);
						if (point.X >= selectPointX - (SelectPointSize / 2) &&
						    point.Y >= selectPointY - (SelectPointSize/2) &&
						    point.X <= selectPointX + (SelectPointSize/2) &&
						    point.Y <= selectPointY + (SelectPointSize/2)) {
							return selectPoint;
						}
					}
				}
			}
			return null;
		}

		public PreviewPoint PointToZoomPoint(PreviewPoint p)
		{
			PreviewPoint newPoint = new PreviewPoint(p.X, p.Y);
			PointToZoomPointRef(newPoint);
			return newPoint;
		}

		public void PointToZoomPointRef(PreviewPoint p)
		{
			int xDif = p.X - (int)(p.X / ZoomLevel);
			int yDif = p.Y - (int)(p.Y / ZoomLevel);
			p.X = p.X - xDif;
			p.Y = p.Y - yDif;
		}

		public PreviewPoint PointToZoomPointAdd(PreviewPoint p)
		{
			return new PreviewPoint(Convert.ToInt32(p.X * ZoomLevel), Convert.ToInt32(p.Y * ZoomLevel));
		}

		public abstract void SetSelectPoint(PreviewPoint point = null);

		public abstract void SelectDefaultSelectPoint();

		public virtual object Clone()
		{
			var shape = (PreviewBaseShape)this.MemberwiseClone();
			foreach (var previewPixel in Pixels)
			{
				shape.Pixels.Add(previewPixel.Clone());
			}

			return shape;
		}

		public abstract void MoveTo(int x, int y);

		public virtual void Nudge(int x, int y)
		{
			MoveTo(Left + x, Top + y);
		}

		public abstract void Resize(double aspect);

		public abstract void ResizeFromOriginal(double aspect);

        /// <summary>
        /// This will be true if the shape is being created. Only used in multi-point placement objects
        /// </summary>
        [Browsable(false)]
        public virtual bool Creating { get; set; }

		public DisplayItemBaseControl GetSetupControl()
		{
			Shapes.DisplayItemBaseControl setupControl = null;

			if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewSingle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewLine") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewRectangle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewEllipse") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewArch") {
				setupControl = new Shapes.PreviewArchSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMegaTree") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewTriangle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewFlood") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCane") {
				setupControl = new Shapes.PreviewCaneSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStar") {
				setupControl = new Shapes.PreviewStarSetupControl(this);
			}
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStarBurst")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewNet")
            {
				setupControl = new Shapes.PreviewNetSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustom") {
				setupControl = new Shapes.PreviewCustomSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewPixelGrid") {
				setupControl = new PreviewShapeBaseSetupControl(this);
			}
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewIcicle")
            {
                setupControl = new Shapes.PreviewIcicleSetupControl(this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewPolyLine")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMultiString")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
            }
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustomProp")
			{
				setupControl = new PreviewShapeBaseSetupControl(this);
			}

			return setupControl;
		}

		public int UpdatePixelCache()
		{
			_pixelCache = Pixels.Where(x => x.Node != null).ToList();
			_points = new List<float>(_pixelCache.Count * 8);
			return _pixelCache.Count;
		}

		public void UpdateDrawPoints(int referenceHeight)
		{
			//Logging.Debug("Updating Drawing Shape {0}.", ToString());

			_points.Clear();
			if (_pixelCache.Count == 0) return;
			
			if (_pixelCache[0].IsDiscreteColored)
			{
				//Logging.Debug("Standard Type.");
				CreateDiscreteColorPoints(referenceHeight);
			}
			else
			{
				//Logging.Debug("Pixel Type.");
				CreateFullColorPoints(referenceHeight);
			}

			//Logging.Debug("{0} Points generated.", _points.Count() / 7);

		}

		public void Draw(ShaderProgram program)
		{
			//Logging.Debug("Entering Draw.");
			if (_points.Count == 0)
			{
				//Logging.Debug("Exiting Draw.");
				return;
			}

			//program["pointSize"].SetValue((float)PixelSize);
			VBO<float> points = new VBO<float>(_points.ToArray());

			//Logging.Debug("Created VBO.");

			GlUtility.BindBuffer(points);

			//Logging.Debug("Buffer Bound.");
			GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, EightFloatDataSize, IntPtr.Zero);
			GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);

			//Logging.Debug("Point pointer set.");

			GL.VertexAttribPointer(ShaderProgram.VertexColor, 4, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes);
			GL.EnableVertexAttribArray(ShaderProgram.VertexColor);

			GL.VertexAttribPointer(ShaderProgram.VertexSize, 1, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes + Vector4.SizeInBytes);
			GL.EnableVertexAttribArray(ShaderProgram.VertexSize);

			GL.DisableVertexAttribArray(ShaderProgram.TextureCoords);

			//Logging.Debug("Color pointer set.");

			//Logging.Debug("Beginning draw.");

			// draw the points
			GL.DrawArrays(PrimitiveType.Points, 0, points.Count / 8);

			//Logging.Debug("Draw completed for shape.");
			points.Dispose();

			//Logging.Debug("VBO Disposed.");
			//Logging.Debug("Exiting Draw.");
		}

		
		//public void Draw(ShaderProgram program, int referenceHeight)
		//{
		//	Logging.Debug("Drawing Shape {0}.", ToString());
		//	int pointCount = 0;
		//	List<float> p;

		//	if (StringType == StringTypes.Pixel)
		//	{
		//		Logging.Debug("Pixel Type.");
		//		var fullColorPoints = CreateFullColorPoints(referenceHeight);
		//		p = fullColorPoints;

		//	}
		//	else
		//	{
		//		Logging.Debug("Standard Type.");
		//		var discreteColorPoints = CreateDiscreteColorPoints(referenceHeight);
		//		p = discreteColorPoints;
		//	}

		//	Logging.Debug("{0} Points generated.", pointCount);

		//	if (!p.Any()) return;

		//	program["pointSize"].SetValue((float)PixelSize);//>1?PixelSize:2
		//	VBO<float> points = new VBO<float>(p.ToArray());

		//	Logging.Debug("Created VBO.");

		//	GlUtility.BindBuffer(points);

		//	Logging.Debug("Buffer Bound.");
		//	GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, SevenFloatDataSize, IntPtr.Zero);
		//	GL.EnableVertexAttribArray(0);

		//	Logging.Debug("Point pointer set.");

		//	GL.VertexAttribPointer(ShaderProgram.VertexColor, 4, VertexAttribPointerType.Float, true, SevenFloatDataSize, Vector3.SizeInBytes);
		//	GL.EnableVertexAttribArray(1);

		//	Logging.Debug("Color pointer set.");

		//	Logging.Debug("Beginning draw.");

		//	// draw the points
		//	GL.DrawArrays(PrimitiveType.Points, 0, points.Count / 7);

		//	Logging.Debug("Draw completed for shape.");
		//	points.Dispose();

		//	Logging.Debug("VBO Disposed.");
		//}
		private static readonly List<Color> _emptyColors = new List<Color>();

		private void CreateDiscreteColorPoints(int referenceHeight)
		{
			List<Color> colors = _emptyColors;
			Guid nodeId = Guid.Empty;
			IIntentStates state = null;
			foreach (PreviewPixel previewPixel in _pixelCache)
			{
				if (previewPixel.NodeId != nodeId)
				{
					nodeId = previewPixel.NodeId;
					state = previewPixel.Node.Element.State;
					colors = previewPixel.GetDiscreteColors(state);
				}

				if (state?.Count > 0)
				{
					//All points are the same in standard discrete 
					int col = 1;
					Vector2 xy;
					if (_isHighPrecision)
					{
						xy = new Vector2((float)previewPixel.Location.X, (float)previewPixel.Location.Y);
					}
					else
					{
						xy = new Vector2(previewPixel.X, previewPixel.Y);
					}
					foreach (Color c in colors)
					{
						if (c.A > 0)
						{
							_points.Add(xy.X);
							_points.Add(referenceHeight - xy.Y);
							_points.Add(previewPixel.Z);
							_points.Add(c.R);
							_points.Add(c.G);
							_points.Add(c.B);
							_points.Add(c.A);
							_points.Add(previewPixel.PixelSize);

							if (col % 2 == 0)
							{
								xy.Y += previewPixel.PixelSize;
								//xy.X = xy.X;
							}
							else
							{
								xy.X = xy.X + previewPixel.PixelSize;
							}

							col++;
						}
					}
				}
			}
		}

		private void CreateFullColorPoints(int referenceHeight)
		{
			foreach (PreviewPixel previewPixel in _pixelCache)
			{
				var state = previewPixel.Node.Element.State;
				
				if (state.Count > 0)
				{
					Color c = previewPixel.GetFullColor(state);
					if (c.A > 0)
					{
						if (_isHighPrecision)
						{
							_points.Add((float)previewPixel.Location.X);
							_points.Add(referenceHeight - (float)previewPixel.Location.Y);
						}
						else
						{
							_points.Add(previewPixel.X);
							_points.Add(referenceHeight - previewPixel.Y);
						}
						
						_points.Add(previewPixel.Z);
						_points.Add(c.R);
						_points.Add(c.G);
						_points.Add(c.B);
						_points.Add(c.A);
						_points.Add(previewPixel.PixelSize);
					}
				}
			}
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

		protected void AddPixels(ElementNode node, int lightCount)
		{
			if (_pixels.Count == 0)
			{
				// Just add the pixels, they will get laid out next
				for (int lightNum = 0; lightNum < lightCount; lightNum++)
				{
					PreviewPixel pixel = AddPixel(10, 10);
					pixel.PixelColor = Color.White;
					if (node != null && node.IsLeaf)
					{
						pixel.Node = node;
					}
				}
			}
		}
	}
}