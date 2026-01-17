using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using VixenModules.Editor.FixtureGraphics.OpenGL;
using Common.OpenGLCommon.Constructs.Shaders;


namespace Common.OpenGLCommon.Constructs.DrawingEngine
{
	/// <summary>
	/// Base clas for drawing shapes or props in OpenGL.
	/// </summary>
	/// <typeparam name="TLightShape">Type of shape or prop</typeparam>
	public abstract class OpenGLDrawingEngineBase<TLightShape> where TLightShape : IDisposable
	{
		#region Private Constants

		protected const double Fov = 45.0;
		private const float FarDistance = 4000f;
		private const float NearDistance = 1f;

		#endregion

		#region Static Fields

		protected static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		
		/// <summary>
		/// Lock to prevent more than one thread from performing OpenGL operations.
		/// </summary>
		protected static readonly Object ContextLock = new Object();

		#endregion

		#region Fields
						
		private float _aspectRatio;				
		private string _programLog = string.Empty;
		
		/// <summary>
		/// Moving head render strategy.  This class renders graphical volumes that are associated with an OpenGL shader.
		/// This class exists to improve performance.  We are violating the enapsulation of the display item shapes so that
		/// we can group like shapes together and minimize shader program transitions on the GPU.
		/// </summary>
		private MovingHeadRenderStrategy _movingHeadRenderStrategy;

		#endregion

		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		protected OpenGLDrawingEngineBase()
		{
			FormLoading = true;
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Options to use with Parallel.ForEach.
		/// </summary>
		protected ParallelOptions ParallelOptions { get; set; } = new ParallelOptions()
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};

		/// <summary>
		/// Flag that is true while the form is loading.
		/// </summary>
		protected bool FormLoading { get; set; }

		/// <summary>
		/// Flag that is true while rendering a frame.
		/// </summary>
		protected bool IsRendering { get; set; }

		/// <summary>
		/// Light point shader program.
		/// </summary>
		protected ShaderProgram LightPointProgram { get; set; }
		
		/// <summary>
		/// Scale factor for drawing light points.
		/// </summary>
		protected float PointScaleFactor { get; set; }

		/// <summary>
		/// Focal depth of the preview.
		/// </summary>
		protected float FocalDepth { get; set; }

		/// <summary>
		/// Logical size of the view.
		/// </summary>
		protected float ProjectionHeight { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Width of the OpenTK control.
		/// </summary>
		public int OpenTkControl_Width { get; set; }

		/// <summary>
		/// Height of the OpenTK control.
		/// </summary>
		public int OpenTkControl_Height { get; set; }

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

		/// <summary>
		/// Call back to update the distance in the status pane.
		/// </summary>
		public Action<float> UpdateStatusDistance { get; set; }

		/// <summary>
		/// Call back delegate to render the preview on the GUI thread.
		/// </summary>
		public System.Action OnRenderFrameOnGUIThread { get; set; }

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

		#endregion

		#region Protected Abstract Methods
		
		/// <summary>
		/// Resets timers associated with the preview.
		/// </summary>
		protected abstract void ResetTimers();

		/// <summary>
		/// Gets a collection of light based preview shapes.
		/// </summary>
		/// <returns>Light shapes</returns>
		protected abstract IEnumerable<TLightShape> GetLightShapes();

		/// <summary>
		/// Returns the display item shapes that implement <c>IOpenGLMovingHeadShape</c>.
		/// </summary>
		/// <returns>Moving head shapes</returns>
		protected abstract IEnumerable<IOpenGLMovingHeadShape> GetMovingHeadShapes();

		/// <summary>
		/// Reeturns the display item shapes that implement <c>IDrawStaticPreviewShape</c>.
		/// </summary>
		/// <returns>Static preview shapes </returns>
		protected abstract IEnumerable<IDrawStaticPreviewShape> GetIDrawStaticPreviewShapes();

		/// <summary>
		/// Gets the brightness of the background.
		/// </summary>
		/// <returns>Brightness of the background</returns>
		protected abstract int GetBackgroundAlpha();

		/// <summary>
		/// Calculates the scale between the logical size of the preview and the size of the OpenTK viewport.
		/// </summary>
		/// <returns>Scale of the viewport</returns>
		protected abstract float CalculatePointScale();

		/// <summary>
		/// Determines which light points need to be drawn and with what color.
		/// </summary>
		/// <param name="referenceHeightY">Height of the logical view</param>
		protected abstract void UpdateLightShapePoints(float referenceHeightY);

		/// <summary>
		/// Draws the light points associated with the specified shape.
		/// </summary>
		/// <param name="shape">Shape to draw</param>
		protected abstract void DrawPoints(TLightShape shape);
		
		/// <summary>
		/// Top-level rendering algorithm.
		/// </summary>
		protected abstract void RenderPreviewInternal();

		/// <summary>
		/// Calculates the logical reference height of the view.
		/// </summary>
		/// <returns>Logical reference height of the view</returns>
		protected abstract float GetReferenceHeight();

		/// <summary>
		/// Calculates the logical reference width of the view.
		/// </summary>
		/// <returns>Logical reference height of the view</returns>
		protected abstract float GetReferenceWidth();

		/// <summary>
		/// Initializes the background of the preview.
		/// </summary>
		protected abstract void InitializeBackground();

		/// <summary>
		/// Gets a zoom factor relative to the reference height.
		/// </summary>
		/// <returns>Zoom scale factor</returns>
		protected abstract float GetZoomFactor();

		/// <summary>
		/// Gets the default position for the camera.
		/// </summary>
		/// <returns>Default position of the camera</returns>
		protected abstract Vector3 GetCameraCenterPosition();

		/// <summary>
		/// Returns a focal depth multiplier.
		/// </summary>
		/// <returns>Focal depth multiplier</returns>
		protected abstract float GetFocalDepthMultiplier();

		#endregion

		#region Protected Moving Head Methods

		/// <summary>
		/// Initializes the moving head render strategy with shapes that are made up of graphical volumes.
		/// </summary>
		/// <param name="redraw">Method to redraw the preview</param>
		/// <param name="recreateRenderStrategy">Flag to have the drawing engine recreate the moving head render strategy class</param>
		protected void InitializeMovingHeadRenderStrategy(Action redraw, bool recreateRenderStrategy = false)
		{
			// If the moving head render strategy has not been created or
			// the caller requested a new instance then...
			if (_movingHeadRenderStrategy == null || recreateRenderStrategy)
			{
				// Reset all strobe timers
				ResetTimers();

				// Create the moving head render strategy
				_movingHeadRenderStrategy = new MovingHeadRenderStrategy();

				// Loop over the moving heads
				foreach (IOpenGLMovingHeadShape movingHeadVolumes in GetMovingHeadShapes())
				{					
					// Initialize the moving head with the reference height and redraw delegate
					float referenceHeight = GetReferenceHeight();
					movingHeadVolumes.Initialize(movingHeadVolumes.SizeY, referenceHeight, redraw);

					// Give the shape to the render strategy
					_movingHeadRenderStrategy.Shapes.Add(movingHeadVolumes.MovingHead);
				}

				// Initialize the moving head render strategy
				_movingHeadRenderStrategy.Initialize();				
			}
		}
		
		/// <summary>
		/// Renders static preview shapes.
		/// </summary>
		/// <param name="perspective">Perspective matrix used to render 3-D onto 2-D</param>
		/// <param name="standardFrame">True when rendering for a standard frame transition</param>
		/// <param name="previewWidth">Total width of the preview</param>
		/// <param name="previewHeight">Total heigh of the preview</param>
		protected void RenderStaticPreviewShapes(Matrix4 perspective, bool standardFrame, float previewWidth, float previewHeight)
		{
			// Deselect any previous shader programs
			GL.UseProgram(0);

			// Get reference height and width of the view
			float referenceHeight = GetReferenceHeight();
			float referenceWidth = GetReferenceWidth();

			float sizeScale = (previewWidth / referenceWidth + previewHeight / referenceHeight) / 2f;

			// The beam does not seem to cover the background so adding multiplier
			int maxBeamLength = (int)Math.Round(Math.Max(previewWidth, previewHeight) / sizeScale, MidpointRounding.AwayFromZero);

			// Loop over all the moving heads
			Parallel.ForEach(GetMovingHeadShapes(), (movingHead) =>
			{
				// Update the position and rotation uniforms for the volumes
				movingHead.UpdateVolumes(maxBeamLength, referenceHeight, standardFrame);
			});

			// Disable depth checks as we want the static preview shapes to show up on top of the background
			GL.Clear(ClearBufferMask.DepthBufferBit);

			// Render the moving head volumes
			_movingHeadRenderStrategy.RenderVolumes(perspective, _camera.Position, _camera.ViewMatrix, GetBackgroundAlpha());

			// Loop over the IDrawStaticPreviewShape display item shapes
			foreach (IDrawStaticPreviewShape shape in GetIDrawStaticPreviewShapes())
			{
				// Draw the static preview shapes using OpenGL				
				shape.DrawOpenGL(perspective, _camera.ViewMatrix, (int)previewHeight);
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Converts the specified angle in degrees to radians.
		/// </summary>
		/// <param name="angleIndegrees">Angle in degrees to convert</param>
		/// <returns>Angle in radians</returns>
		protected double ConvertDegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees * Math.PI / 180;
		}

		/// <summary>
		/// Updates the scale factor of a light point.
		/// </summary>
		protected void CalculatePointScaleFactor()
		{
			if (!_enableLightScaling)
			{
				PointScaleFactor = 1;

				return;
			}

			// Camera needs to be created before calculating the point scale factor
			if (_camera != null)
			{
				float scale = FocalDepth / _camera.Position.Z;

				float sizeScale = CalculatePointScale();

				scale *= sizeScale;

				scale = scale >= .1f ? scale : .1f;

				PointScaleFactor = scale;
			}
		}

		/// <summary>
		/// Creates the OpenGL perspective matrix.
		/// </summary>
		/// <returns>OpenGL perspective matrix</returns>
		protected Matrix4 CreatePerspective()
		{
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView((float)ConvertDegreesToRadians(Fov), _aspectRatio > 0 ? _aspectRatio : 1, 1f, 7500f);
			return perspective;
		}

		/// <summary>
		/// Clears the drawing canvas.
		/// </summary>
		protected void ClearScreen()
		{
			// Set up the OpenGL viewport and clear both the color and depth bits
			GL.Viewport(0, 0, OpenTkControl_Width, OpenTkControl_Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Draws the light points.
		/// </summary>
		/// <param name="mvp">Model view projection</param>
		protected void DrawPoints(Matrix4 mvp)
		{
			try
			{
				//_drawPointsSW.Restart();

				//Logging.Debug("Selecting point program.");

				// Active that shader program
				LightPointProgram.Use();

				// Transfer the Uniforms to the GPU
				// This only needs to be done once after switching shader programs
				LightPointProgram["mvp"].SetValue(mvp);
				LightPointProgram["pointScale"].SetValue(PointScaleFactor);

				// Loop over all the light based display items
				foreach (TLightShape shape in GetLightShapes())
				{
					// Draw the color points					
					DrawPoints(shape);
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
			_camera = new Camera(new Vector3(cameraX, cameraY, cameraZ), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));
						
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
		
		#endregion

		#region public Methods

		/// <summary>
		/// Main entry point from the OpenTK control.
		/// </summary>
		public void RenderPreview()
		{
			// TODO: Minimized Short Circuit ?
			if (IsRendering || FormLoading) return;

			IsRendering = true;

			RenderPreviewInternal();

			IsRendering = false;
		}

		/// <summary>
		/// Zooms in and out the preview.
		/// </summary>
		/// <param name="direction">Direction of the zoom</param>
		public void Zoom(int direction)
		{
			float distance = _camera.Position.Z + direction / GetZoomFactor();

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

			_camera.Move(new Vector3(0, 0, direction / GetZoomFactor()));

			CalculatePointScaleFactor();

			UpdateStatusBarDistance();
		}

		/// <summary>
		/// Initializes the drawing engine.
		/// </summary>		
		/// <param name="cameraX">X axis position of the camera</param>
		/// <param name="cameraY">Y axis position of the camera</param>
		/// <param name="cameraZ">Z axis position of the camera</param>
		public virtual void Initialize(
			float cameraX,
			float cameraY,
			float cameraZ)
		{			
			EnableOpenGLFeatures();

			// compile the shader program
			LightPointProgram = new ShaderProgram(VertexShader, FragmentShader, false);

			InitializeBackground();

			_aspectRatio = (float)OpenTkControl_Width / OpenTkControl_Height;
			
			ProjectionHeight = GetReferenceHeight();

			// TODO: REVISIT THIS; SHOULD WE BE DIVIDING PROJECTION HEIGHT BY 2 ??
			FocalDepth = (float)((ProjectionHeight / 2.0f) / Math.Tan(ConvertDegreesToRadians(Fov) / 2.0f));

			cameraX = 0f;
			cameraY = 0f;
			cameraZ = FocalDepth * GetFocalDepthMultiplier();

			CreateCamera(
				cameraX,
				cameraY,
				cameraZ);

			Logging.Info("OpenGL v {0}", GL.GetString(StringName.Version));
			Logging.Info("Vendor {0}, Renderer {1}", GL.GetString(StringName.Vendor), GL.GetString(StringName.Renderer));
			Logging.Info("Shading language version {0}", GL.GetString(StringName.ShadingLanguageVersion));
			Logging.Info("Extensions {0}", GL.GetString(StringName.Extensions));
			_programLog = LightPointProgram.ProgramLog;
			_programLog += LightPointProgram.VertexShader.ShaderLog;
			_programLog += LightPointProgram.FragmentShader.ShaderLog;
			if (!string.IsNullOrEmpty(_programLog))
			{
				Logging.Error($"Shader log output: {_programLog}");
			}

			CalculatePointScaleFactor();
			FormLoading = false;
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
			OpenTkControl_Width = (int)width;
			OpenTkControl_Height = (int)height;

			// Calculate the focal depth
			FocalDepth = (float)(1 / Math.Tan(ConvertDegreesToRadians(Fov / 2)) * (height / 2.0));

			//_projectionHeight = (int)(2.0 * _camera.Position.Z * Math.Tan(ConvertDegreesToRadians(Fov / 2.0)));

			// Calculate the aspect ratio
			_aspectRatio = (float)OpenTkControl_Width / (float)OpenTkControl_Height;

			// Focal depth changed to the point scale factor needs to be recalculated
			CalculatePointScaleFactor();
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

		/// <summary>
		/// Moves the camera to the center of the preview.
		/// </summary>
		/// <returns>Width and height of the OpenGL drawing area</returns>
		public System.Windows.Size ExecuteCenterPreview()
		{
			// Reposition the camera in the center of the preview				
			_camera.Position = GetCameraCenterPosition();
			// new Vector3(OpenTkControl_Width / 2f, OpenTkControl_Height / 2f, _camera.Position.Z);
			_camera.SetDirection(new Vector3(0, 0, -1));

			CalculatePointScaleFactor();

			return new System.Windows.Size(OpenTkControl_Width, OpenTkControl_Height);
		}

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
		public virtual void Dispose()
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
				foreach(TLightShape shape in GetLightShapes())
				{
					shape.Dispose();
				}

				if (LightPointProgram != null)
				{
					LightPointProgram.DisposeChildren = true;
					LightPointProgram.Dispose();
					LightPointProgram = null;					
				}
			}			
		}
		
		#endregion
	}
}
