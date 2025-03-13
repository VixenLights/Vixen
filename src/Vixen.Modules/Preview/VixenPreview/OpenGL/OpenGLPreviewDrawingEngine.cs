using System.Diagnostics;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Vixen.Sys;
using Vixen.Sys.Instrumentation;

using VixenModules.Editor.FixtureGraphics.OpenGL;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	/// <summary>
	/// OpenGL logic to draw the preview.
	/// </summary>
	public class OpenGLPreviewDrawingEngine : IDisposable
	{
		#region Private Constants
		
		private const double Fov = 45.0;
		private const float FarDistance = 4000f;
		private const float NearDistance = 1f;

		#endregion

		#region Instrumentation Fields

		private readonly MillisecondsValue _backgroundDraw;
		private readonly MillisecondsValue _pointsUpdate;
		private readonly MillisecondsValue _pointsDraw;
		private readonly MillisecondsValue _previewUpdate;
		//private readonly MillisecondsValue _previewDrawPoints;
		private readonly Stopwatch _sw = Stopwatch.StartNew();
		private readonly Stopwatch _sw2 = Stopwatch.StartNew();
		private readonly Stopwatch _frameRateTimer = Stopwatch.StartNew();
		//private readonly Stopwatch _drawPointsSW = Stopwatch.StartNew();

		#endregion

		#region Static Fields

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		internal static readonly Object ContextLock = new Object();

		#endregion

		#region Fields

		private int _width = 800;
		private int _height = 600;
		private ShaderProgram _program;
		private float _focalDepth = 0;
		private float _aspectRatio;				
		private bool _isRendering;
		private bool _formLoading;		
		private float _pointScaleFactor;
		private long _frameCount;
		private string _programLog = string.Empty;		
		private List<DisplayItem> _lightBasedDisplayItems;
				
		/// <summary>
		/// Moving head render strategy.  This class renders graphical volumes that are associated with an OpenGL shader.
		/// This class exists to improve performance.  We are violating the enapsulation of the display item shapes so that
		/// we can group like shapes together and minimize shader program transitions on the GPU.
		/// </summary>
		private MovingHeadRenderStrategy _movingHeadRenderStrategy;

		private readonly ParallelOptions _parallelOptions = new ParallelOptions()
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};
		
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Preview data</param>		
		public OpenGLPreviewDrawingEngine(VixenPreviewData data)
		{
			Data = data;			
			_formLoading = true;
			
			_backgroundDraw = new MillisecondsValue("OpenGL preview background draw");
			_pointsUpdate = new MillisecondsValue("OpenGL preview points update");
			_pointsDraw = new MillisecondsValue("OpenGL preview points draw");
			_previewUpdate = new MillisecondsValue("OpenGL preview update");
			//_previewDrawPoints = new MillisecondsValue("OpenGL preview DrawPoints");
			VixenSystem.Instrumentation.AddValue(_backgroundDraw);
			VixenSystem.Instrumentation.AddValue(_pointsUpdate);
			VixenSystem.Instrumentation.AddValue(_pointsDraw);
			VixenSystem.Instrumentation.AddValue(_previewUpdate);
			//VixenSystem.Instrumentation.AddValue(_previewDrawPoints);

			// Separate out the light based shapes
			_lightBasedDisplayItems = Data.DisplayItems.Where(item => item.IsLightShape()).ToList();			
		}

		#endregion

		#region Private Moving Head Methods

		/// <summary>
		/// Initializes the moving head render strategy with shapes that are made up of graphical volumes.
		/// </summary>
		/// <param name="redraw">Method to redraw the preview</param>
		private void InitializeMovingHeadRenderStrategy(Action redraw)
		{
			// If the moving head render strategy has not been created then...
			if (_movingHeadRenderStrategy == null)
			{
				// Reset all strobe timers
				MovingHeadIntentHandler.ResetStrobeTimers();

				// Create the moving head render strategy
				_movingHeadRenderStrategy = new MovingHeadRenderStrategy();

				// Loop over the moving heads
				foreach (IOpenGLMovingHeadShape movingHeadVolumes in GetMovingHeadShapes())
				{
					// Initialize the moving head with the reference height and redraw delegate
					int referenceHeight = GetReferenceHeight();
					movingHeadVolumes.Initialize(referenceHeight, redraw);

					// Give the shape to the render strategy
					_movingHeadRenderStrategy.Shapes.Add(movingHeadVolumes.MovingHead);
				}

				// Initialize the moving head render strategy
				_movingHeadRenderStrategy.Initialize();
			}
		}

		/// <summary>
		/// Calculates the reference height of the view.
		/// </summary>
		/// <returns>Reference height of the view</returns>
		private int GetReferenceHeight()
		{
			return _background.HasBackground ? _background.Height : _height; 
		}

		/// <summary>
		/// Calculate the reference width of the view.
		/// </summary>
		/// <returns>Reference width of the view</returns>
		private int GetReferenceWidth()
		{
			return _background.HasBackground ? _background.Width : _width; 
		}

		/// <summary>
		/// Returns the display item shapes that implement <c>IDrawMovingHeadVolumes</c>.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IOpenGLMovingHeadShape> GetMovingHeadShapes()
		{
			// Get the display item shapes that implement IDrawMovingHeadVolumes
			return Data.DisplayItems.Where(displayItem => displayItem.Shape is IOpenGLMovingHeadShape).
				Select(displayItem => displayItem.Shape).
					Cast<IOpenGLMovingHeadShape>().ToList();
		}

		/// <summary>
		/// Reeturns the display item shapes that implement <c>IDrawMovingHeadVolumes</c>.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IDrawStaticPreviewShape> GetIDrawStaticPreviewShapes()
		{
			// Get the display item shapes that implement IDrawMovingHeadVolumes
			return Data.DisplayItems.Where(displayItem => displayItem.Shape is IDrawStaticPreviewShape)
				.Select(displayItem => displayItem.Shape)
					.Cast<IDrawStaticPreviewShape>().ToList();
		}

		/// <summary>
		/// Renders static preview shapes.
		/// </summary>
		/// <param name="perspective">Perspective matrix used to render 3-D onto 2-D</param>
		/// <param name="standardFrame">True when rendering for a standard frame transition</param>
		private void RenderStaticPreviewShapes(Matrix4 perspective, bool standardFrame)
		{
			// Deselect any previous shader programs
			GL.UseProgram(0);

			// Calculate the reference height
			int referenceHeight = GetReferenceHeight();
			int referenceWidth = GetReferenceWidth(); 

			float sizeScale = ((float)_width / referenceWidth + (float)_height / referenceHeight) / 2f;

			// The beam does not seem to cover the background so adding multiplier
			int maxBeamLength = (int)Math.Round(Math.Max(_width, _height) / sizeScale, MidpointRounding.AwayFromZero);

			// Loop over all the moving heads
			Parallel.ForEach(GetMovingHeadShapes(), (movingHead) =>
			{
				// Update the position and rotation uniforms for the volumes
				movingHead.UpdateVolumes(maxBeamLength, referenceHeight, standardFrame);
			});

			// Disable depth checks as we want the static preview shapes to show up on top of the background
			GL.Clear(ClearBufferMask.DepthBufferBit);

			// Render the moving head volumes
			_movingHeadRenderStrategy.RenderVolumes(perspective, _camera.Position, _camera.ViewMatrix, Data.BackgroundAlpha);

			// Loop over the IDrawStaticPreviewShape display item shapes
			foreach (IDrawStaticPreviewShape shape in GetIDrawStaticPreviewShapes())
			{
				// Draw the static preview shapes using OpenGL				
				shape.DrawOpenGL(_camera.Position.Z, _width, _height, perspective, _camera.ViewMatrix, _pointScaleFactor, referenceHeight, _camera.Position);
			}
		}

		/// <summary>
		/// Returns true if a moving head is (still) moving.
		/// </summary>
		/// <returns>True if a moving head is (still) moving</returns>
		private bool MovingHeadsMoving()
		{
			// Default to NOT moving
			bool movingHeadsMoving = false;

			// Loop over the moving heads
			foreach (IOpenGLMovingHeadShape movingHead in GetMovingHeadShapes())
			{
				// If the moving head settings have been initialized then...
				if (movingHead.MovingHead != null)
				{
					// If the command position does NOT match the current unlimited position then...
					if (((MovingHeadOpenGL)movingHead.MovingHead).MovingHead.CommandedPanAngle !=
						((MovingHeadOpenGL)movingHead.MovingHead).MovingHead.UnlimitedPanAngle ||
						((MovingHeadOpenGL)movingHead.MovingHead).MovingHead.CommandedTiltAngle !=
						((MovingHeadOpenGL)movingHead.MovingHead).MovingHead.UnlimitedTiltAngle)
					{
						// At least one moving head is still moving
						movingHeadsMoving = true;

						break;
					}
				}
			}

			return movingHeadsMoving;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Enables the OpenGL features required by the preview.
		/// </summary>
		private void EnableOpenGLFeatures()
		{
			// enable depth testing to ensure correct z-ordering of our fragments
			//GL.Enable(EnableCap.DepthTest);
			lock (ContextLock)
			{
#if DEBUG
				//Logging.Info("Debug Output");
				GL.Enable(EnableCap.DebugOutput);
				//Logging.Info("Debug Output Sync");
				GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
				//Logging.Info("Blend");
				GL.Enable(EnableCap.Blend);
				//Logging.Info("Blend Func");
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
				//Logging.Info("Point Sprite");
				GL.Enable(EnableCap.PointSprite);
				//Logging.Info("Vertex Point Sprite");
				GL.Enable(EnableCap.VertexProgramPointSize); ;
				//Logging.Info("Program Point");
				GL.Enable(EnableCap.ProgramPointSize);
				//Logging.Info("Cull Face");
				GL.Disable(EnableCap.CullFace);

				GL.Enable(EnableCap.DepthTest);
			}
		}
		
		/// <summary>
		/// Updates the frame rate associated with the preview.
		/// </summary>
		private void UpdateFrameRate()
		{
			_frameCount++;

			if (_frameRateTimer.ElapsedMilliseconds > 999)
			{
				// Call the delegate on the parent to update the frame rate
				UpdateFrmRate(_frameCount);

				_frameCount = 0;
				_frameRateTimer.Restart();
			}
		}

		/// <summary>
		/// Converts the specified angle in degrees to radians.
		/// </summary>
		/// <param name="angleIndegrees">Angle in degrees to convert</param>
		/// <returns>Angle in radians</returns>
		private double ConvertDegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees * Math.PI / 180;
		}
		
		/// <summary>
		/// Creates the camera view point.
		/// </summary>		
		/// <param name="cameraX">Camera X axis position</param>
		/// <param name="cameraY">Camera Y axis position</param>
		/// <param name="cameraZ">Camera Z axis position</param>
		private void CreateCamera(			
			float cameraX,
			float cameraY,
			float cameraZ)
		{			
			// Create our camera
			_camera = new Camera(new Vector3(GetReferenceWidth() / 2f, (GetReferenceHeight()) / 2f, _focalDepth), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));
			//_camera.Position = new Vector3(_camera.Position.X, _camera.Position.Y, xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", Name),_camera.Position.Z));
			_camera.Position = new Vector3(
				cameraX,
				cameraY,
				cameraZ);

			// Update the status bar distance 
			UpdateStatusBarDistance();
		}

		/// <summary>
		/// Updates the Z distance on the status bar.
		/// </summary>
		private void UpdateStatusBarDistance()
		{
			// If the parent has assigned a delegate then...
			if (UpdateStatusDistance != null)
			{
				// Notify the parent that the camera Z position has changed
				UpdateStatusDistance(_camera.Position.Z);
			}
		}

		/// <summary>
		/// Updates the scale factor of a light point.
		/// </summary>
		private void CalculatePointScaleFactor()
		{
			if (!_enableLightScaling)
			{
				_pointScaleFactor = 1;
				return;
			}
			float scale = _focalDepth / _camera.Position.Z;
			float sizeScale = 0;
			if (_background.HasBackground)
			{
				sizeScale = ((float)_width / _background.Width + (float)_height / _background.Height) / 2f;
			}
			else
			{
				sizeScale = 1;
			}

			scale *= sizeScale;

			scale = scale >= .1f ? scale : .1f;

			_pointScaleFactor = scale;
		}

		/// <summary>
		/// Creates the OpenGL perspective matrix.
		/// </summary>
		/// <returns></returns>
		private Matrix4 CreatePerspective()
		{
			var perspective = Matrix4.CreatePerspectiveFieldOfView((float)ConvertDegreesToRadians(Fov), _aspectRatio > 0 ? _aspectRatio : 1, 1f, 7500f);
			return perspective;
		}

		/// <summary>
		/// Updates a colletion of points that need to drawn.
		/// </summary>		
		private void UpdateShapePoints()
		{
			//Prepare the points
			//Logging.Debug("Begin Update Shape Points.");
			int height = GetReferenceHeight();
			Parallel.ForEach(_lightBasedDisplayItems, _parallelOptions, d => ((PreviewLightBaseShape)d.Shape).UpdateDrawPoints(height));
		}

		/// <summary>
		/// Clears the drawing canvas.
		/// </summary>
		private void ClearScreen()
		{
			// Set up the OpenGL viewport and clear both the color and depth bits
			GL.Viewport(0, 0, _width, _height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Draws the light points.
		/// </summary>
		/// <param name="mvp">Model view projection</param>
		private void DrawPoints(Matrix4 mvp)
		{
			try
			{
				//_drawPointsSW.Restart();

				//Logging.Debug("Selecting point program.");

				// Active that shader program
				_program.Use();

				// Transfer the Uniforms to the GPU
				// This only needs to be done once after switching shader programs
				_program["mvp"].SetValue(mvp);
				_program["pointScale"].SetValue(_pointScaleFactor);

				// Loop over all the light based display items
				foreach (DisplayItem dataDisplayItem in _lightBasedDisplayItems)
				{
					// Draw the color points					
					dataDisplayItem.LightShape.Draw();
				}

				// Clear the OpenGL program
				GL.UseProgram(0);

				//_drawPointsSW.Stop();
				//_previewDrawPoints.Set(_drawPointsSW.ElapsedTicks);
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred rendering display items.");
			}
			//Logging.Debug("Exiting DrawPoints.");
		}

		/// <summary>
		/// Clears the preview and draws the background.
		/// </summary>
		/// <param name="perspective">Model view projection matrix</param>
		private void DrawBackground(Matrix4 perspective)
		{
			ClearScreen();

			_sw2.Restart();
			_background.Draw(perspective, _camera.ViewMatrix);
			_backgroundDraw.Set(_sw2.ElapsedMilliseconds);
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Renders the preview.
		/// </summary>		
		public void RenderPreview()
		{
			// TODO: Minimized Short Circuit ?
			//Logging.Debug("Entering RenderFrame");
			if (_isRendering || _formLoading) return;

			// Initialize the moving head render strategy with the applicable display item shapes
			InitializeMovingHeadRenderStrategy(OnRenderFrameOnGUIThread);
			
			_isRendering = true;
			_sw.Restart();
			var perspective = CreatePerspective();

			if (IsPreviewStale())
			{
				//Logging.Debug("Elements have state.");
				_sw2.Restart();
				UpdateShapePoints();
				_pointsUpdate.Set(_sw2.ElapsedMilliseconds);
				var mvp = Matrix4.Identity * _camera.ViewMatrix * perspective;
				
				lock (ContextLock)
				{
					//GLControl.Context.MakeCurrent();
					DrawBackground(perspective);
					
					//Logging.Info($"GL Error: {GL.GetError()}");
					_sw2.Restart();

					// Render static preview shapes (moving heads)
					RenderStaticPreviewShapes(perspective, StandardFrame);

					DrawPoints(mvp);

					NeedsUpdate = true;

					_pointsDraw.Set(_sw2.ElapsedMilliseconds);

					//GLControl.Context.SwapBuffers(); // I don't think the OpenTK control requires this method
					//GLControl.Context.MakeCurrent();
				}
			}
			else
			{
				lock (ContextLock)
				{
					//GLControl.Context.MakeCurrent();
					DrawBackground(perspective);

					// Render static preview shapes (moving heads)
					RenderStaticPreviewShapes(perspective, StandardFrame);

					NeedsUpdate = false;

					//GLControl.Context.SwapBuffers(); // I don't think the OpenTK control requires this method
					//GLControl.Context.MakeCurrent();
				}
			}

			// This event can be called by WPF to repaint the window and can also be called by moving head related timers
			if (StandardFrame)
			{
				UpdateFrameRate();
				StandardFrame = false;
			}
			
			_isRendering = false;
			_previewUpdate.Set(_sw.ElapsedMilliseconds);
									
			//Logging.Debug("Exiting RenderFrame");
		}

		/// <summary>
		/// Moves the camera to the center of the preview.
		/// </summary>
		/// <returns>Width and height of the OpenGL drawing area</returns>
		public System.Windows.Size ExecuteCenterPreview()
		{
			// Reposition the camera in the center of the preview				
			_camera.Position = new Vector3(_width / 2f, _height / 2f, _camera.Position.Z);
			_camera.SetDirection(new Vector3(0, 0, -1));
			
			CalculatePointScaleFactor();
			
			return new System.Windows.Size(_width, _height);
		}

		/// <summary>
		/// Resets the size of the preview to match the background.
		/// </summary>
		/// <returns>Size of the preview to match the background</returns>
		public System.Windows.Size ResetSize()
		{
			_width = _background.Width;
			_height = _background.Height;

			_focalDepth = (float)(1 / Math.Tan(ConvertDegreesToRadians(Fov / 2)) * (_height / 2.0));
			_camera.Position = new Vector3(_width / 2f, _height / 2f, _focalDepth);
			_camera.SetDirection(new Vector3(0, 0, -1));
			CalculatePointScaleFactor();

			return new System.Windows.Size(_width, _height);
		}

		/// <summary>
		/// Zooms in and out the preview.
		/// </summary>
		/// <param name="direction">Direction of the zoom</param>
		public void Zoom(int direction)
		{
			float distance = _camera.Position.Z + direction;

			// If the user is attempting to zoom to far out then...
			if (distance > FarDistance)
			{
				// Zoom them back to the edge of the limits
				direction = (int)Math.Round(FarDistance - _camera.Position.Z, MidpointRounding.AwayFromZero);
			}
			// Otherwise if the user attempt to zoom too far in then...
			else if (distance < NearDistance)
			{
				// Zoom them back to the edge of the limits
				direction = (int)(NearDistance - (int)_camera.Position.Z);
			}

			_camera.Move(new Vector3(0, 0, direction));

			CalculatePointScaleFactor();

			UpdateStatusBarDistance();
		}

		/// <summary>
		/// Returns true if the preview has a background image.
		/// </summary>
		/// <returns>True if the preview has a background image</returns>
		public bool HasBackgroundImage()
		{
			return _background.HasBackground;
		}

		/// <summary>
		/// Initializes the drawing engine.
		/// </summary>		
		/// <param name="cameraX">X axis position of the camera</param>
		/// <param name="cameraY">Y axis position of the camera</param>
		/// <param name="cameraZ">Z axis position of the camera</param>
		public void Initialize(			
			float cameraX,
			float cameraY,
			float cameraZ)
		{
			_formLoading = true;

			EnableOpenGLFeatures();

			// compile the shader program
			_program = new ShaderProgram(VertexShader, FragmentShader, false);

			_background = new Background(Data);

			_aspectRatio = (float)_width / _height;

			_focalDepth = (float)(1 / Math.Tan(ConvertDegreesToRadians(Fov / 2)) * (_height / 2.0));

			CreateCamera(				
				cameraX,
				cameraY,
				cameraZ);

			Logging.Info("OpenGL v {0}", GL.GetString(StringName.Version));
			Logging.Info("Vendor {0}, Renderer {1}", GL.GetString(StringName.Vendor), GL.GetString(StringName.Renderer));
			Logging.Info("Shading language version {0}", GL.GetString(StringName.ShadingLanguageVersion));
			Logging.Info("Extensions {0}", GL.GetString(StringName.Extensions));
			_programLog = _program.ProgramLog;
			_programLog += _program.VertexShader.ShaderLog;
			_programLog += _program.FragmentShader.ShaderLog;
			if (!string.IsNullOrEmpty(_programLog))
			{
				Logging.Error($"Shader log output: {_programLog}");
			}
			
			CalculatePointScaleFactor();
			_formLoading = false;
		}

		/// <summary>
		/// Updates the width and height of the drawing area.
		/// </summary>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		public void OpenTKDrawingAreaChanged(double width, double height)
		{
			// This logic existed in the Windows Forms version of the Preview.
			// Suspect it might have been trying to avoid an exception. 
			// Going to exclude this logic for now in the WPF Preview.
			//if (glControl.ClientSize.Height == 0)
			//	glControl.ClientSize = new Size(glControl.ClientSize.Width, 100);

			// Convert the drawing area to integers
			_width = (int)width;
			_height = (int)height;

			// Calculate the focal depth
			_focalDepth = (float)(1 / Math.Tan(ConvertDegreesToRadians(Fov / 2)) * (height / 2.0));

			// Calculate the aspect ratio
			_aspectRatio = (float)_width / (float)_height;			
		}

		/// <summary>
		/// Returns true if the preview is stale.
		/// </summary>
		/// <returns>True if the preview is stale</returns>
		public bool IsPreviewStale()
		{
			// The preview is stale if there is light data or the moving heads are moving
			// Moving head beam color changes are included in ElementsHaveState
			return VixenSystem.Elements.ElementsHaveState || MovingHeadsMoving();
		}
		
		/// <summary>
		/// Initializes the props and calculates the number of light points.
		/// </summary>
		/// <returns>Number of light points that make up the preview</returns>
		public int Setup()
		{
			var pixelCount = 0;
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				dataDisplayItem.Shape.Layout();

				if (dataDisplayItem.IsLightShape())
				{
					pixelCount += dataDisplayItem.LightShape.UpdatePixelCache();
				}
			}
			
			return pixelCount;
		}

		/// <summary>
		/// Moves the camera based on the X and Y positions.
		/// </summary>
		/// <param name="prevX">Previous X position</param>
		/// <param name="prevY">Previous Y position</param>
		/// <param name="eX">X position</param>
		/// <param name="eY">Y position</param>
		public void MoveCamera(int prevX, int prevY, int eX, int eY)
		{
			var moveFactor = (_camera.Position.Z / FarDistance) * 7;
			float yaw = (prevX - eX) * moveFactor;
			float pitch = (prevY - eY) * moveFactor;
			_camera.Move(new Vector3(yaw, -pitch, 0f));

			CalculatePointScaleFactor();
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Flag that indicates if this a normal periodic update the preview vs a moving head update due to a timer.
		/// </summary>
		public bool StandardFrame { get; set; }

		/// <summary>
		/// Delegate call back to update the frame rate.
		/// </summary>
		public Action<long> UpdateFrmRate { get; set; }

		private Camera _camera;

		/// <summary>
		/// OpenGL camera used to view the preview.
		/// </summary>
		public Camera Camera
		{ 
			get 
			{ 
				return _camera; 
			} 
		}	
				
		private Background _background;

		/// <summary>
		/// OpenGL logic used to draw the background.
		/// </summary>
		public Background Background
		{
			get
			{
				return _background;
			}
			set
			{
				_background = value;
			}
		}

		private bool _enableLightScaling = true;

		/// <summary>
		/// Flag to determine if lights are scaled.
		/// </summary>
		public bool EnableLightScaling
		{
			get
			{
				return _enableLightScaling;
			}
			set
			{
				_enableLightScaling = value;
				CalculatePointScaleFactor();
			}
		}

		/// <summary>
		/// Call back delegate to render the preview on the GUI thread.
		/// </summary>
		public Action OnRenderFrameOnGUIThread { get; set; }

		/// <summary>
		/// Call back to update the distance in the status pane.
		/// </summary>
		public Action<float> UpdateStatusDistance { get; set; }

		/// <summary>
		/// Data associated with the preview.
		/// </summary>
		public VixenPreviewData Data { get; set; }

		/// <summary>
		/// Flag that forces the preview to redraw.
		/// This flag ensures that when all lights are turned off that the preview is updated to clear all lights.
		/// </summary>
		public bool NeedsUpdate { get; set; }

		#endregion

		#region Private Constants

		private const string VertexShader = @"
		#version 330

		in vec3 vertexPosition;
		in vec4 vertexColor;
		in float vertexSize;
		
		out vec4 color;
		out float pSize;

		uniform float pointScale;
		uniform mat4 mvp;

		void main(void)
		{
			color = vertexColor;

			gl_Position = mvp * vec4(vertexPosition, 1);

			gl_PointSize = vertexSize * pointScale;
			
			if(vertexSize < 1)
			{
				gl_PointSize = 1;
			}
			
			pSize = gl_PointSize; //pass through
			
		}
		";

		private const string FragmentShader = @"
		#version 330

		in vec4 color;
		in float pSize;
		out vec4 fragColor;

		void main(void)
		{
			if(pSize > 2) //We only need to round points that are bigger than 2
			{
				vec2 circCoord = 2.0 * gl_PointCoord - 1.0;
				if (dot(circCoord, circCoord) > 1)
				{
					discard;
				}
			}
			
			fragColor = vec4(color.r/255.0, color.g/255.0, color.b/255.0, color.a/255.0);

		}
		";

		#endregion

		#region IDisposable

		/// <inheritdoc/>		
		public void Dispose()
		{
			// Loop over the display item shapes that implement IDrawStaticPreviewShape
			foreach (IDrawStaticPreviewShape staticShape in GetIDrawStaticPreviewShapes())
			{
				// Dispose of the OpenGL resources associated with this shape
				staticShape.DisposeOpenGLResources();
			}
		
			// Lock the OpenGL context
			lock (ContextLock)
			{				
				if (_program != null)
				{
					_program.DisposeChildren = true;
					_program.Dispose();
					_program = null;

					_background.Dispose();
					_background = null;
				}				
			}
		}

		#endregion
	}
}
