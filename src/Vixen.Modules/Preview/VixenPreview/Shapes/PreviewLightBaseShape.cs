using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using OpenTK.Mathematics;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Base class for preview shapes that are based on lights.
	/// </summary>
	[DataContract]
	public abstract class PreviewLightBaseShape : PreviewBaseShape, ICloneable, IDisposable
	{
		#region Fields

		private List<float> _points = new List<float>();
		public static int EightFloatDataSize = 8 * Marshal.SizeOf(typeof(float));
		public bool connectStandardStrings = false;
		public StringTypes _stringType = StringTypes.Standard;
		private bool _isHighPrecision;

		private List<PreviewPixel> _pixelCache = new List<PreviewPixel>();
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Vertex Buffer Object for the points that make up the light based shape.
		/// </summary>
		private VBO<float> _pointsVBO;
		
		/// <summary>
		/// Keeps track of the point buffer size so we know when to expand the buffer.
		/// </summary>
		private int _pointsBufferSize;

		#endregion

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
						
		private Color _pixelColor = Color.White;
		public int _pixelSize = 3;
		
		/// <summary>
		/// Vertex Array Object for OpenGL drawing of the light shape.
		/// </summary>
		[IgnoreDataMember]
		public int VAO { get; set; }

		[DataMember(Name = "Pixels")]
		public List<PreviewPixel> _pixels = new List<PreviewPixel>();

		[DataMember(EmitDefaultValue = false)]
		public List<PreviewBaseShape> _strings;

		protected IEnumerable<PreviewLightBaseShape> LightStrings
		{
			get
			{
				return _strings.Cast<PreviewLightBaseShape>();
			}
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
						((PreviewLightBaseShape)line).StringType = _stringType;
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
					foreach (PreviewLightBaseShape line in _strings) {
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
        
		[Editor(typeof (PreviewSetElementsUIEditor), typeof (UITypeEditor)),
		 Category("Settings"),
		 DisplayName("Linked Elements")]
		public virtual List<PreviewLightBaseShape> Strings
		{
			get
			{
				if (_strings != null) {
					//Instead of going through the strings multiple times.. do it once
					// set all the sub-strings to match the connection state for elements
					foreach (PreviewLightBaseShape line in _strings)
						line.connectStandardStrings = this.connectStandardStrings;

					// Set all the StringTypes in the substrings
					foreach (PreviewLightBaseShape line in _strings) {
						line.StringType = _stringType;
					}
				}

				List<PreviewBaseShape> stringsResult = _strings; 

				if (stringsResult == null) {
					stringsResult = new List<PreviewBaseShape>();
					stringsResult.Add(this);
				}
				return stringsResult.Cast<PreviewLightBaseShape>().ToList();
			}
			set 
			{ 
				if (value == null)
				{
					_strings = null;
				}
				else
				{
					_strings = value.Cast<PreviewBaseShape>().ToList();
				}				
			}
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

		public virtual void MatchPixelSize(PreviewLightBaseShape shape)
		{
			PixelSize = shape.PixelSize;
		}

		public virtual void ResizePixelsBy(int value)
		{
			var newSize = PixelSize + value;
			PixelSize = newSize > 0 ? newSize : 1;
		}

		[Browsable(false)]
		public override double ZoomLevel
		{
			get
			{				
				return base.ZoomLevel;
			}
			set
			{
				base.ZoomLevel = value;
				if (_strings != null)
				{
					foreach (PreviewLightBaseShape shape in _strings)
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
				foreach (PreviewLightBaseShape shape in _strings) {
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
			if (_pixels.Count > 0 && 
			pixel == _pixels[0] || _strings != null && 
			    _strings.Count > 0 && 
				_strings[0] is PreviewLightBaseShape &&
			    ((PreviewLightBaseShape)_strings[0]).Pixels != null &&
			    ((PreviewLightBaseShape)_strings[0]).Pixels.Count > 0 && 
				((PreviewLightBaseShape)_strings[0]).Pixels[0] == pixel)
			{
				return true;
			}

			return false;
		}

		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                         bool forceDraw, double zoomLevel)
		{
			foreach (PreviewPixel pixel in Pixels) {

				DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
			}

			DrawSelectPoints(fp);
		}
       				
		public override bool ShapeInRect(Rectangle rect)
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
		
		public override object Clone()
		{
			var shape = (PreviewLightBaseShape)this.MemberwiseClone();
			foreach (var previewPixel in Pixels)
			{
				shape.Pixels.Add(previewPixel.Clone());
			}

			return shape;
		}
		        				
		public int UpdatePixelCache()
		{
			_pixelCache = Pixels.Where(x => x.Node != null && x.Node.Element != null).ToList();
			_points = new List<float>(_pixelCache.Count * 8);
			return _pixelCache.Count;
		}

		public bool HasInValidElementMapping()
		{
			var badLinks = Pixels.FirstOrDefault(x => x.Node != null && x.Node.Element == null);
			if (badLinks != null)
			{
				Logging.Error($"Preview shape mapped to Node {badLinks.Node.Name} without Element");
			}

			return badLinks != null;
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
		
		/// <summary>
		/// Draws the points that make up the light based shape.
		/// </summary>		
		public void Draw()
		{
			//Logging.Debug("Entering Draw.");
			if (_points.Count == 0)
			{
				//Logging.Debug("Exiting Draw.");
				return;
			}
			
			// If the points Vertex Buffer Object has not been created then
			// this is the first time drawing this shape and additional configuration is required.
			if (_pointsVBO == null)
			{
				// Create the Vertex Array Object 
				GL.GenVertexArrays(1, out int vao);
				VAO = vao;

				// Bind the Vertex Array Object
				// Any Vertex Array configuration will be associated with this VAO.
				GL.BindVertexArray(VAO);

				// Create the Vertex Buffer Object passing the light points
				_pointsVBO = new VBO<float>(_points.ToArray());

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer(_pointsVBO);

				// Store off the size of the buffer
				_pointsBufferSize = _pointsVBO.Count;

				// Specify the format and location of the vertex attribute data in the VBO
				GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, EightFloatDataSize, IntPtr.Zero);
				GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);				
				GL.VertexAttribPointer(ShaderProgram.VertexColor, 4, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexColor);
				GL.VertexAttribPointer(ShaderProgram.VertexSize, 1, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes + Vector4.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexSize);
				
				GL.DisableVertexAttribArray(ShaderProgram.TextureCoords);
			}
			else 
			{
				// Bind the Vertex Array Object
				GL.BindVertexArray(VAO);
								
				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer(_pointsVBO);

				// If the buffer is already of sufficient size then...
				if (_pointsBufferSize >= _points.Count)
				{
					// Update the point data in the GPU
					GL.BufferSubData<float>(BufferTarget.ArrayBuffer, 0, _points.Count * sizeof(float), _points.ToArray());
				}
				else
				{
					// Create and initialize a buffer object's data store. Allocate memory for the buffer and initialize the buffer with data
					// This only needs to done when increasing the size of the buffer
					GL.BufferData<float>(BufferTarget.ArrayBuffer, _points.Count * sizeof(float), _points.ToArray(), BufferUsageHint.StreamDraw);

					// Update the size of the buffer
					_pointsBufferSize = _points.Count;
				}							
			}
			
			// Draw the points
			GL.DrawArrays(PrimitiveType.Points, 0, _points.Count / 8);												
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
		
		protected override void Dispose(bool disposing)
		{
		    base.Dispose(disposing);
			 
			if (disposing) 
			{							
				if (_pixels != null)
					_pixels.ForEach(p => p.Dispose());
				_pixels.Clear();
				_pixels = null;
				if (_strings != null)
					_strings.ForEach(s => s.Dispose());
				_strings = null;

				// Dispose of the points Vertex Buffer Object
				if (_pointsVBO != null)
				{
					_pointsVBO.Dispose();	
					_pointsVBO = null;	
				}
			}
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