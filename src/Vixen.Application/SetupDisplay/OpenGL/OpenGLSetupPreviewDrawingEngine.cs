using System.Windows.Media.Media3D;

using Common.OpenGLCommon;
using Common.OpenGLCommon.Constructs.DrawingEngine;
using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;
using Common.OpenGLCommon.Constructs.Shaders;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Vixen.Sys.Props.Model;

using VixenApplication.SetupDisplay.OpenGL.Shapes;

using VixenModules.App.Props.Models.IntellligentFixture;
using VixenModules.Preview.VixenPreview.OpenGL;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Drawing engine for an OpenGL Preview Setup.
	/// </summary>
	public class OpenGLSetupPreviewDrawingEngine : OpenGLDrawingEngineBase<ILightPropOpenGLData>, IDisposable
	{
		#region Fields

		/// <summary>
		/// Stores position of the mouse in screen coordinates at the beginning of a prop move operation.
		/// </summary>		
		private Vector2 _startMousePosition;
		
		/// <summary>
		/// The resize handle the mouse clicked on.
		/// </summary>
		private ResizeHandles _activeResizeHandle;

		/// <summary>
		/// Flag indicating that left mouse button is down.
		/// </summary>
		private bool _mouseButtonDown;

		/// <summary>
		/// Flag that indciates a prop move operation is in progress.
		/// </summary>
		private bool _propMoveInProgress;
		
		/// <summary>
		/// Flag indicating that the mouse is over a resize handle on mouse down.
		/// </summary>
		private bool _overResizeHandleOnMouseDown;

		/// <summary>
		/// Position of a selected prop on mouse down.
		/// </summary>
		private Vector3 _mouseDownInitialSize;

		/// <summary>
		/// Moving head logic wasn't originally designed to be moved around the 
		/// preview.  This flag triggers the render strategy class to be recreated.
		/// </summary>
		private bool _recreateMovingHeadRenderStrategy;

		#endregion

		#region Private Properties

		/// <summary>
		/// Collection of selected props.
		/// </summary>
		private List<IPropOpenGLData> SelectedProps { get; set; }

		/// <summary>
		/// Collection of Props being displayed on the preview.
		/// </summary>
		private List<IPropOpenGLData> Props { get; set; }

		/// <summary>
		/// Collection of light based props.
		/// </summary>
		private List<ILightPropOpenGLData> LightProps { get; set; }

		/// <summary>
		/// Collection of intelligent fixtures.
		/// </summary>
		private List<IntelligentFixturePropOpenGLData> IntelligentFixtureProps { get; set; }

		/// <summary>
		/// Line shader program.
		/// </summary>
		private ColorLineShaderProgram LineProgram { get; set; }
		
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Preview data</param>		
		public OpenGLSetupPreviewDrawingEngine(List<IPropModel> props)
		{
			// Initialize the selected props collection
			SelectedProps = new List<IPropOpenGLData>();

			// Extract out the light props
			LightProps = props.Where(prp => prp is ILightPropModel).Select(prp => new LightPropOpenGLData((ILightPropModel)prp)).Cast<ILightPropOpenGLData>().ToList();

			// Extract out the intelligent fixture props
			IntelligentFixtureProps = props.Where(prp => prp is IntelligentFixtureModel).Select(prp => new IntelligentFixturePropOpenGLData((IntelligentFixtureModel)prp)).Cast<IntelligentFixturePropOpenGLData>().ToList();

			// Create the collection of ALL props
			Props = new List<IPropOpenGLData>();
			Props.AddRange(LightProps);
			Props.AddRange(IntelligentFixtureProps);
		}

		#endregion
		
		#region Protected Abstract Methods

		/// <inheritdoc/>
		protected override void ResetTimers()
		{
			// Prop Preview does not require any timers
		}
		
		/// <inheritdoc/>
		protected override IEnumerable<ILightPropOpenGLData> GetLightShapes()
		{
			return LightProps;
		}

		/// <inheritdoc/>		
		protected override IEnumerable<IOpenGLMovingHeadShape> GetMovingHeadShapes()
		{
			return IntelligentFixtureProps;
		}

		/// <inheritdoc/>
		protected override IEnumerable<IDrawStaticPreviewShape> GetIDrawStaticPreviewShapes()
		{
			return IntelligentFixtureProps;
		}

		/// <inheritdoc/>		
		protected override int GetBackgroundAlpha()
		{
			// Draw with full intensity
			return 255;
		}

		/// <inheritdoc/>		
		protected override float CalculatePointScale()
		{
			// Use a light point size of 1
			return 1.0f;
		}

		/// <inheritdoc/>		
		protected override void UpdateLightShapePoints(float referenceHeightY)
		{
			Parallel.ForEach(LightProps, ParallelOptions, d => d.UpdateDrawPoints(referenceHeightY));
		}

		/// <inheritdoc/>		
		protected override void DrawPoints(ILightPropOpenGLData prop)
		{
			DrawPointsUtility.DrawPoints(prop);			
		}
		
		/// <inheritdoc/>		
		protected override void RenderPreviewInternal()
		{			
			// Initialize the moving head render strategy with the applicable display item shapes
			InitializeMovingHeadRenderStrategy(OnRenderFrameOnGUIThread, _recreateMovingHeadRenderStrategy);

			// Indicate that the render strategy was just updated
			_recreateMovingHeadRenderStrategy = false;

			// Determine which points on the shape need to drawn and what color to draw them
			UpdateLightShapePoints(ProjectionHeight);

			// Create the perspective from the camera
			Matrix4 perspective = CreatePerspective();

			// Create the model-view-project matrix
			Matrix4 mvp = Matrix4.Identity * Camera.ViewMatrix * perspective;

			// Activate the OpenGL lock
			lock (ContextLock)
			{
				// Clear the view port
				ClearScreen();

				// Draw the background
				_background.Draw(perspective, Camera.ViewMatrix);
				
				// Render static preview shapes (moving heads)
				RenderStaticPreviewShapes(perspective, true, OpenTkControl_Width, OpenTkControl_Height);

				// Draw the light points
				DrawPoints(mvp);

				// Draw the prop selection box
				DrawSelectionBox(Matrix4.Identity, Camera.ViewMatrix, perspective);

				// Draw the prop center X move handle
				DrawSelectionIcon(Matrix4.Identity, Camera.ViewMatrix, perspective);

				// Draw Resize Boxes
				DrawResizeHandles(Matrix4.Identity, Camera.ViewMatrix, perspective);			
			}			
		}

		/// <inheritdoc/>		
		protected override float GetReferenceHeight()
		{
			const int DefaultHeight = 600;
			return _background.HasBackground ? _background.Height : DefaultHeight;
		}

		/// <inheritdoc/>		
		protected override float GetReferenceWidth()
		{
			const int DefaultWidth = 800;
			return _background.HasBackground ? _background.Width : DefaultWidth;
		}

		/// <inheritdoc/>		
		protected override float GetZoomFactor()
		{
			// TODO: THIS NEEDS TO BE REVISITED FOR THE PREVIEW SETUP
			//
			// Since the props are being draw on a 1x1 coordinate system
			// The zoom needs to divided by a factor of 100
			return 100;
		}

		/// <inheritdoc/>		
		protected override void InitializeBackground()
		{
			// Initialize the preview background
			_background = new PropPreviewBackground("a469f44f-a21e-485e-af05-95b9b69a3fd0.jpg", 1.0f);			
		}

		/// <inheritdoc/>		
		protected override Vector3 GetCameraCenterPosition()
		{
			// Prop Preview positions the camera on the origin
			return new Vector3(0, 0, Camera.Position.Z);
		}

		/// <inheritdoc/>		
		protected override float GetFocalDepthMultiplier()
		{
			return 1.5f;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the specified props to the preview.
		/// </summary>
		/// <param name="props">Props to add</param>
		public void AddProps(List<IPropModel> props)
		{
			// Separate out the light props
			List<ILightPropOpenGLData> lightProps = props.Where(prp => prp is ILightPropModel).Select(prp => new LightPropOpenGLData((ILightPropModel)prp)).Cast<ILightPropOpenGLData>().ToList();

			foreach (ILightPropOpenGLData prop in lightProps)
			{
				// Make the initial size of the prop 20% of the preview
				prop.SizeX = 0.2f * GetReferenceWidth();
				prop.SizeY = 0.2f * GetReferenceHeight();
				prop.SizeZ = 0.2f * GetReferenceWidth();
			}

			// Extract out the light props
			LightProps.AddRange(lightProps);

			// Separate out the intelligent fixture props
			List<IntelligentFixturePropOpenGLData> fixtureProps = props.Where(prp => prp is IntelligentFixtureModel).Select(prp => new IntelligentFixturePropOpenGLData((IntelligentFixtureModel)prp)).Cast<IntelligentFixturePropOpenGLData>().ToList();
			
			// Loop over the intelligent fixture props
			foreach (IntelligentFixturePropOpenGLData prop in fixtureProps)
			{
				// Make the initial size of the prop 20% of the preview				
				prop.SizeY = 0.2f * GetReferenceHeight();
			}

			// Indicate that the render strategy needs to be recreated
			_recreateMovingHeadRenderStrategy = true;

			// Extract out the intelligent fixture props
			IntelligentFixtureProps.AddRange(fixtureProps);

			// Add the light and fixture props to the prop collection
			Props.AddRange(lightProps);
			Props.AddRange(fixtureProps);
		}

		/// <inheritdoc>/>		
		public override void Initialize(
			float cameraX,
			float cameraY,
			float cameraZ)
		{
			// Call the base class
			base.Initialize(cameraX, cameraY, cameraZ);

			// Initialize the color line shader program
			LineProgram = new ColorLineShaderProgram();			
		}
		
		/// <summary>
		/// Handles mouse up preview events to select props.
		/// </summary>
		/// <param name="mousePos">Position of the mouse in screen coordinates</param>
		public void MouseUp(Vector2 mousePos)
		{
			// Loop over the selected props
			foreach (IPropOpenGLData prop in SelectedProps)
			{
				// Deselect all props
				prop.Selected = false;
			}

			// Clear the collection of selected props
			SelectedProps.Clear();

			// Loop over all the props
			foreach (IPropOpenGLData prop in Props)
			{
				// If the mouse if over the specified prop then...
				if (IsMouseOverProp(mousePos, prop))
				{
					// Indicate the prop is selected
					prop.Selected = true;

					// Add the prop to the collection of selected props
					SelectedProps.Add(prop);					
				}
			}

			// Set a flag to indicate the mouse is no longer down
			_mouseButtonDown = false;

			// Indicate any prop move operations are complete
			_propMoveInProgress = false;
		}

		/// <summary>
		/// Preview mouse event for resizing and moving props.
		/// </summary>
		/// <param name="previousMouseX">Previous mouse X screen position</param>
		/// <param name="previousMouseY">Previous mouse Y screen position</param>
		/// <param name="mouseX">Current mouse X screen position</param>
		/// <param name="mouseY">Current mouse Y screen position</param>
		/// <returns>True when the mouse is over a resize handle</returns>
		public bool MouseMove(int previousMouseX, int previousMouseY, int mouseX, int mouseY)
		{
			// If the mouse is down and was over a resize handle then			
			if (_mouseButtonDown && _overResizeHandleOnMouseDown)
			{
				// Initialize direction signs
				int dirXSign = 0;
				int dirYSign = 0;

				// Determine (signs) how the prop should be resized as the mouse is moved				
				switch (_activeResizeHandle)
				{
					case ResizeHandles.TopLeft:
						dirXSign = -1;
						dirYSign = 1;
						break;
					case ResizeHandles.TopRight:
						dirXSign = +1;
						dirYSign = 1;
						break;
					case ResizeHandles.BottomLeft:
						dirXSign = -1;
						dirYSign = -1;
						break;
					case ResizeHandles.BottomRight:
						dirXSign = +1;
						dirYSign = -1;
						break;
				}

				// Loop over the selected prop
				foreach (IPropOpenGLData prop in SelectedProps)
				{
					// Create a Vector2 with the mouse cursor position
					Vector2 mousePos = new Vector2(mouseX, mouseY);
					
					// Get the mouse position in world coordinates
					Vector3 mouseWorldPos = GetMouseMovementInWorld((int)_startMousePosition.X, (int)_startMousePosition.Y, mouseX, mouseY, prop);
					
					// Resize the prop
					prop.SizeX = Math.Max(1, _mouseDownInitialSize.X + 2 * mouseWorldPos.X * dirXSign);
					prop.SizeY = Math.Max(1, _mouseDownInitialSize.Y + 2 * mouseWorldPos.Y * dirYSign);
					prop.SizeZ = Math.Max(1, _mouseDownInitialSize.Z + 2 * mouseWorldPos.X * dirXSign); // if circular

					// Re-create the moving head render strategy if the prop is a moving head
					_recreateMovingHeadRenderStrategy = (prop is IIntelligentFixturePropOpenGLData);					
				}
			}
			// Otherwise if the mouse is down then...
			else if (_mouseButtonDown)
			{				
				// Get the mouse position in screen coordinates
				Vector2 mousePos = new Vector2(mouseX, mouseY);

				// Loop over the selected props
				foreach (IPropOpenGLData prop in SelectedProps)
				{
					// If the mouse is over the prop or
					// a prop move is in progress then...
					if (IsMouseOverProp(mousePos, prop) || _propMoveInProgress)
					{
						// Remember we started a prop move operation
						_propMoveInProgress = true;

						// Get the mouse position in world coordinates
						Vector3 mouseWorldPosition = GetMouseMovementInWorld(previousMouseX, previousMouseY, mouseX, mouseY, prop);
		
						// Move the prop
						prop.X += mouseWorldPosition.X;
						prop.Y += mouseWorldPosition.Y;

						// Re-create the moving head render strategy if the prop is a moving head
						_recreateMovingHeadRenderStrategy = (prop is IIntelligentFixturePropOpenGLData);						
					}
				}
			}

			// Return whether the mouse is over a resize handle
			return IsMouseOverAResizeHandle(mouseX, mouseY);
		}

		/// <summary>
		/// Handles mouse down preview events to select props and start move operations.
		/// </summary>
		/// <param name="mouseX">Position of the mouse in X screen coordinates</param>
		/// <param name="mouseY">Position of the mouse in Y screen coordinates</param>
		public void MouseDown(int mouseX, int mouseY)
		{
			// Set a flag that the preview mouse is down
			_mouseButtonDown = true;

			// Default to NOT being over a resize handle
			_overResizeHandleOnMouseDown = false;

			ResizeHandles activeHandle = ResizeHandles.BottomRight;

			// Loop over the selected props
			foreach (IPropOpenGLData prop in SelectedProps)
			{
				// If the prop is selected then...
				if (prop.Selected)
				{
					// Store off mouse position at the beginning of the move operation
					_startMousePosition.X = mouseX;
					_startMousePosition.Y = mouseY;

					// If the mouse is over the a resize handle then...
					if (prop.MouseOverResizeHandle(
						CreatePerspective(),
						Camera.ViewMatrix,
						OpenTkControl_Width,
						OpenTkControl_Height,
						new Vector2(mouseX, mouseY),
						out activeHandle))
					{
						// Store off the scale at the beginning of the resize operation
						_mouseDownInitialSize.X = prop.SizeX;
						_mouseDownInitialSize.Y = prop.SizeY;
						_mouseDownInitialSize.Z = prop.SizeZ;
												
						// Remember which resize handle is active
						_activeResizeHandle = activeHandle;

						// Set a flag that the mouse started over a resize handle
						_overResizeHandleOnMouseDown = true;
					}
				}
			}
		}

		#endregion

		#region Public Properties

		private PropPreviewBackground _background;

		/// <summary>
		/// OpenGL logic used to draw the background.
		/// </summary>
		public PropPreviewBackground Background
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

		#endregion

		#region IDisposable
		
		/// <inheritdoc/>
		public override void Dispose()
		{
			// TODO: INVESTIGATE IF THIS LOCK IS NECESSARY
			// Lock the OpenGL context
			lock (ContextLock)
			{
				// Dispose of the class managing the background 
				if (_background != null)
				{
					_background.Dispose();
					_background = null;
				}
			}

			// Call the base class implementation
			base.Dispose();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Draws the selection box of a prop.
		/// </summary>		
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		private void DrawSelectionBox(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix)
		{
			// Draw the Selection Rectangle
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>				
					DrawLineUtility.DrawLineLoop(prop.GetSelectionRectangle()));									
		}

		/// <summary>
		/// Draws the resize handles of a prop.
		/// </summary>
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		private void DrawResizeHandles(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix)
		{
			// Draw the resize handles
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					{
						DrawLineUtility.DrawLineLoop(prop.GetUpperLeftCornerResizeBox());
						DrawLineUtility.DrawLineLoop(prop.GetUpperRightCornerResizeBox());
						DrawLineUtility.DrawLineLoop(prop.GetLowerRightCornerResizeBox());
						DrawLineUtility.DrawLineLoop(prop.GetLowerLeftCornerResizeBox());
					});								
		}

		/// <summary>
		/// Draws the move handle of a prop.
		/// </summary>
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		private void DrawSelectionIcon(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix)
		{
			// Draw the Selection handle			
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>				
					DrawLineUtility.DrawLines(prop.GetCenterX()));			
		}

		/// <summary>
		/// Draws selection lines associated with a prop.
		/// </summary>
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		/// <param name="drawLines">Delegate to draw lines</param>
		private void DrawLines(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix,
			Action<IPropOpenGLData> drawLines)
		{
			try
			{
				// Activate the line shader program
				LineProgram.Use();

				// Give the line program the projection and view matrices
				LineProgram.TransferGlobalUniforms(
					projectionMatrix,
					viewMatrix);

				// Transfer the Uniforms to the GPU
				LineProgram.TransferModelMatrixUniform(modelMatrix);

				// Give the line shader program the line color
				LineProgram.TransferColorUniform(Color4.HotPink);

				// Loop over all the selected props
				foreach (IPropOpenGLData prop in SelectedProps)
				{
					// Have the delegate draw lines
					drawLines(prop); 					
				}

				// Clear the OpenGL program
				GL.UseProgram(0);
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred in DrawLines");
			}
		}

		/// <summary>
		/// Returns true if the mouse is over a prop resize handle
		/// </summary>
		/// <param name="mouseX">X position of the mouse in screen coordinates</param>
		/// <param name="mouseY">Y position of the mouse in screen coordinates</param>
		/// <returns>True if the is over a prop resize handle</returns>
		private bool IsMouseOverAResizeHandle(int mouseX, int mouseY)
		{
			// Default to NOT being over a resize handle
			bool overResizeHandle = false;

			// Loop over the selected props
			foreach (IPropOpenGLData prop in SelectedProps)
			{	
				// If the mouse is over a resize handle then...
				if (prop.MouseOverResizeHandle(
							CreatePerspective(),
							Camera.ViewMatrix,
							OpenTkControl_Width,
							OpenTkControl_Height,
							new Vector2(mouseX, mouseY),
							out _))
				{
					// Indicate the mouse is over a resize handle
					overResizeHandle = true;
					break;
				}				
			}

			// Return whether the mouse is over a resize handle
			return overResizeHandle;
		}
																
		/// <summary>
		/// Returns true if the mouse is over a prop.
		/// </summary>
		/// <param name="mousePos">Mouse position in screen coordinates</param>
		/// <param name="prop">Prop to analyze</param>
		/// <returns>Returns true if the mouse is over a prop</returns>
		private bool IsMouseOverProp(Vector2 mousePos, IPropOpenGLData prop)
		{
			// Create a ray where the mouse is pointing
			(Vector3 RayOrigin, Vector3 RayDir) = MouseHitTest.CreateRayFromMouse(
								mousePos,
								CreatePerspective(),
								Camera.ViewMatrix,
								OpenTkControl_Width,
								OpenTkControl_Height);
			
			// Return whether the mouse is over the prop
			return MouseHitTest.RayIntersectsAABB(RayOrigin, RayDir, prop.GetMinimum(), prop.GetMaximum(), out _);			
		}
		
		/// <summary>
		/// Gets the mouse position in world coordinates.
		/// </summary>
		/// <param name="previousMouseX">Previous mouse X position in screen coordinates</param>
		/// <param name="previousMouseY">Previous mouse Y position in screen coordinates</param>
		/// <param name="mouseX">Mouse X position in screen coordinates</param>
		/// <param name="mouseY">Mouse Y position in screen coordinates</param>
		/// <param name="prop">Prop to relate mouse movement to</param>
		/// <returns>Mouse position in world coordinates</returns>
		private Vector3 GetMouseMovementInWorld(int previousMouseX, int previousMouseY, int mouseX, int mouseY, IPropOpenGLData prop)
		{
			Vector3 propPos = new Vector3(prop.X, prop.Y, 0.0f);
			Vector4 clip = new Vector4(propPos, 1.0f) * Camera.ViewMatrix * CreatePerspective();
			clip /= clip.W; // convert to NDC

			float depth = (clip.Z + 1.0f) * 0.5f; // convert NDC Z → depth buffer range

			var lastWorld = Unproject(new Vector3(previousMouseX, previousMouseY, depth), Camera.ViewMatrix, CreatePerspective(), new Size(OpenTkControl_Width, OpenTkControl_Height));
			var currWorld = Unproject(new Vector3(mouseX, mouseY, depth), Camera.ViewMatrix, CreatePerspective(), new Size(OpenTkControl_Width, OpenTkControl_Height));

			propPos = currWorld - lastWorld;

			return propPos;
		}

		/// <summary>
		/// Unprojects a screen coordinate into world coordinates.
		/// </summary>
		/// <param name="screenPos">Screen position to unproject</param>
		/// <param name="view">View matrix</param>
		/// <param name="proj">Project matrix</param>
		/// <param name="viewport">Size of the viewport</param>
		/// <returns>Screen position in world coordinates</returns>
		private Vector3 Unproject(Vector3 screenPos, Matrix4 view, Matrix4 proj, Size viewport)
		{
			Vector4 vec;

			vec.X = 2.0f * screenPos.X / viewport.Width - 1.0f;
			vec.Y = 1.0f - 2.0f * screenPos.Y / viewport.Height;
			vec.Z = screenPos.Z * 2.0f - 1.0f;
			vec.W = 1.0f;

			Matrix4 inv = Matrix4.Invert(view * proj);
			// Vector4 result = OpenTK.Mathematics.Vector4.Transform(vec, inv);
			Vector4 result = new Vector4(
			vec.X * inv.M11 + vec.Y * inv.M21 + vec.Z * inv.M31 + vec.W * inv.M41,
			vec.X * inv.M12 + vec.Y * inv.M22 + vec.Z * inv.M32 + vec.W * inv.M42,
			vec.X * inv.M13 + vec.Y * inv.M23 + vec.Z * inv.M33 + vec.W * inv.M43,
			vec.X * inv.M14 + vec.Y * inv.M24 + vec.Z * inv.M34 + vec.W * inv.M44);

			if (result.W > float.Epsilon)
				result /= result.W;

			return new Vector3(result.X, result.Y, result.Z);
		}

		#endregion
	}
}
