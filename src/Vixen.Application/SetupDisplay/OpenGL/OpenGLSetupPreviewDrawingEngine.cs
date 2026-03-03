using System.ComponentModel;
using System.Windows.Media.Media3D;

using Common.OpenGLCommon;
using Common.OpenGLCommon.Constructs.DrawingEngine;
using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
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
	public class OpenGLSetupPreviewDrawingEngine : OpenGLDrawingEngineBase<ILightPropOpenGLData>, IDisposable, INotifyPropertyChanged
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

		/// <summary>
		/// Previous mouse position in screen coordinates.
		/// This position is used when moving the camera.
		/// </summary>
		private Vector2i _prevMousePosition;

		/// <summary>
		/// Flag that indicates a rubberband selection operation is in progress.
		/// </summary>
		private bool _rubberbandOperationInProgress;

		/// <summary>
		/// Drawing primitive for drawing the rubberband selection box.
		/// </summary>
		private IOpenGLDrawablePrimitive _rubberbandPrimitive;
		
		/// <summary>
		/// Rubberband start position in world coordinates.
		/// </summary>
		private Vector3 _rubberbandStartInWorld;

		/// <summary>
		/// Starting X position of the selection rubberband on mouse down.
		/// </summary>
		private float _rubberBandStartX;

		/// <summary>
		/// Starting Y position of the selection rubberband on mouse down.
		/// </summary>
		private float _rubberBandStartY;

		/// <summary>
		/// Top left X coordinate in screen coordinates of the normalized rubberband selection rectangle.
		/// Normalized such that the ruberband is always oriented left to right, top to bottom.
		/// </summary>
		private float _normalizedRubberbandPt1X;

		/// <summary>
		/// Top left Y coordinate in screen coordinates of the normalized rubberband selection rectangle.
		/// Normalized such that the ruberband is always oriented left to right, top to bottom.
		/// </summary>
		private float _normalizedRubberbandPt1Y;

		/// <summary>
		/// Bottom right X coordinate in screen coordinates of the normalized rubberband selection rectangle.
		/// Normalized such that the ruberband is always oriented left to right, top to bottom.
		/// </summary>
		private float _normalizedRubberbandPt2X;

		/// <summary>
		/// Bottom right Y coordinate in screen coordinates of the normalized rubberband selection rectangle.
		/// Normalized such that the ruberband is always oriented left to right, top to bottom.
		/// </summary>
		private float _normalizedRubberbandPt2Y;

		#endregion

		#region Private Properties

		/// <summary>
		/// Collection of selected props.
		/// </summary>
		private List<IPropOpenGLData> SelectedProps { get; set; }
		
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

			// Create the rubber band drawing primitive
			_rubberbandPrimitive = new OpenGLDrawablePrimitive();

			// Initialize the color line shader program
			LineProgram = new ColorLineShaderProgram();
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
				Background?.Draw(perspective, Camera.ViewMatrix);
				
				// Render static preview shapes (moving heads)
				RenderStaticPreviewShapes(perspective, true, OpenTkControl_Width, OpenTkControl_Height);

				// Loop over the intelligent fixture props
				foreach (IIntelligentFixturePropOpenGLData movingHeadVolumes in GetMovingHeadShapes())
				{
					// Initialize the selection vertices of the intelligent fixture
					movingHeadVolumes.InitializeSelectionVertices(movingHeadVolumes.SizeY);
				}

				// Draw the light points
				DrawPoints(mvp);

				// Draw the prop selection box
				DrawSelectionBox(Matrix4.Identity, Camera.ViewMatrix, perspective);

				// Draw the prop center X move handle
				DrawSelectionIcon(Matrix4.Identity, Camera.ViewMatrix, perspective);

				// Draw Resize Boxes
				DrawResizeHandles(Matrix4.Identity, Camera.ViewMatrix, perspective);

				// Draw rubberband selection box
				DrawRubberband(Matrix4.Identity, Camera.ViewMatrix, perspective);
			}			
		}

		/// <inheritdoc/>		
		protected override float GetReferenceHeight()
		{
			const int DefaultHeight = 600;

			float height = DefaultHeight;

			if (Background != null && Background.HasBackground)
			{
				height = Background.Height;
			}

			return height;
		}

		/// <inheritdoc/>		
		protected override float GetReferenceWidth()
		{
			const int DefaultWidth = 800;
			
			float width = DefaultWidth;	

			if (Background != null && Background.HasBackground)
			{
				width = Background.Width;
			}

			return width;
		}

		/// <inheritdoc/>		
		protected override float GetZoomFactor()
		{
			return 1.0f;
		}

		/// <inheritdoc/>		
		protected override void InitializeBackground()
		{
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
			
			// Store off that the preview has been initialized
			Initialized = true;
		}
		
		/// <summary>
		/// Handles mouse up preview events to select props.
		/// </summary>
		/// <param name="mousePos">Position of the mouse in screen coordinates</param>
		public void MouseUp(Vector2 mousePos)
		{
			// If a rubberband operation is not in progress then...
			if (!_rubberbandOperationInProgress)
			{
				// Clear all selected props
				ClearSelectedProps();
								
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
			}

			// Set a flag to indicate the mouse is no longer down
			_mouseButtonDown = false;

			// Indicate any prop move operations are complete
			_propMoveInProgress = false;

			// End the rubberband operation
			_rubberbandOperationInProgress = false;
			_rubberbandPrimitive.Vertices.Clear();
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
			// Get the mouse position in screen coordinates
			Vector2i mousePos = new Vector2i(mouseX, mouseY);

			// If a rubberband operation is in progress then...
			if (_rubberbandOperationInProgress)
			{
				// Create a vector for the mouse position
				Vector3 RayDir2 = MouseHitTest.CreateRayFromMouse(
					new Vector2(mouseX, mouseY),
					CreatePerspective(),
					Camera.ViewMatrix,
					OpenTkControl_Width,
					OpenTkControl_Height);

				// Determine where the mouse intersects with the X-Y plane
				Vector3 endPoint = IntersectRayWithZ0(Camera.Position, RayDir2);

				// Clear the previous rubberband vertices
				_rubberbandPrimitive.Vertices.Clear();

				// Top Left
				Vector3 p1 = new Vector3(_rubberbandStartInWorld.X, _rubberbandStartInWorld.Y, 0.0f);
				_rubberbandPrimitive.Vertices.Add(_rubberbandStartInWorld.X);
				_rubberbandPrimitive.Vertices.Add(_rubberbandStartInWorld.Y);
				_rubberbandPrimitive.Vertices.Add(0.0f);

				// Top Right
				Vector3 p2 = new Vector3(endPoint.X, _rubberbandStartInWorld.Y, 0.0f);
				_rubberbandPrimitive.Vertices.Add(endPoint.X);
				_rubberbandPrimitive.Vertices.Add(_rubberbandStartInWorld.Y);
				_rubberbandPrimitive.Vertices.Add(0.0f);

				// Bottom Right
				Vector3 p3 = new Vector3(endPoint.X, endPoint.Y, 0.0f);
				_rubberbandPrimitive.Vertices.Add(endPoint.X);
				_rubberbandPrimitive.Vertices.Add(endPoint.Y);
				_rubberbandPrimitive.Vertices.Add(0.0f);

				// Bottom Left
				Vector3 p4 = new Vector3(_rubberbandStartInWorld.X, endPoint.Y, 0.0f);
				_rubberbandPrimitive.Vertices.Add(_rubberbandStartInWorld.X);
				_rubberbandPrimitive.Vertices.Add(endPoint.Y);
				_rubberbandPrimitive.Vertices.Add(0.0f);
				
				// To simplify the logic always assuming the rubberband is 
				// moving from left to right, top to bottom.				
				_normalizedRubberbandPt1X = Math.Min(_rubberBandStartX, mouseX);
				_normalizedRubberbandPt2X = Math.Max(_rubberBandStartX, mouseX);
				_normalizedRubberbandPt1Y = Math.Min(_rubberBandStartY, mouseY);
				_normalizedRubberbandPt2Y = Math.Max(_rubberBandStartY, mouseY);
								
				// Convert the screen coordinates into 3-D world coordinates
				Vector3 leftTop = GetMousePointInWorld(new Vector2(_normalizedRubberbandPt1X, _normalizedRubberbandPt1Y));
				Vector3 rightTop = GetMousePointInWorld(new Vector2(_normalizedRubberbandPt2X, _normalizedRubberbandPt1Y));
				Vector3 bottomRight = GetMousePointInWorld(new Vector2(_normalizedRubberbandPt2X, _normalizedRubberbandPt2Y));
				Vector3 bottomLeft = GetMousePointInWorld(new Vector2(_normalizedRubberbandPt1X, _normalizedRubberbandPt2Y));

				// Loop over all the props
				foreach (IPropOpenGLData prop in Props)
				{
					// If in selection mode then...
					if (IsSelectMode)
					{												
						// If the prop is fully contained in the rubberband then...				
						if (ContainsProp(leftTop, bottomRight, prop.GetMinimum(), prop.GetMaximum()))
						{
							// If the prop is not already selected then...
							if (!SelectedProps.Contains(prop))
							{
								// Select the prop
								SelectedProps.Add(prop);
								prop.Selected = true;								
							}
						}
						else
						{
							// Deselect the prop
							SelectedProps.Remove(prop);
							prop.Selected = false;
						}
					}
					// Otherwise we are in partial selection mode
					else
					{
						// If the prop intersects the rubberband selection or
						// contains the entire prop then...
						if (IntersectsProp(prop.GetMinimum(), prop.GetMaximum()) ||
							ContainsProp(leftTop, bottomRight, prop.GetMinimum(), prop.GetMaximum()))							
						{
							// If the prop is not already selected then...
							if (!SelectedProps.Contains(prop))
							{
								// Select the prop
								SelectedProps.Add(prop);
								prop.Selected = true;								
							}
						}
						else
						{
							// Deselect the prop
							SelectedProps.Remove(prop);
							prop.Selected = false;
						}
					}
				}				
			}
			// If only one props is selected and
			// the mouse is down and
			// over a resize handle then			
			else if (SelectedProps.Count == 1 &&
				_mouseButtonDown && 
				_overResizeHandleOnMouseDown)
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
			// Otherwise if only one props is selected and 
			// the mouse is down and
			// the mouse is over a selected prop then...
			else if (SelectedProps.Count == 1 &&
				_mouseButtonDown && 
				IsOverSelectedProp(mousePos))
			{
				// Loop over the selected props
				foreach (IPropOpenGLData prop in SelectedProps)
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
			// Otherwise if in Pan Mode and 
			// the mouse is down then...
			else if (IsPanMode && _mouseButtonDown)
			{
				// Move the view camera 
				MoveCamera(_prevMousePosition.X, _prevMousePosition.Y, mousePos.X, mousePos.Y);

				// Save off the mouse position
				_prevMousePosition = mousePos;
			}

			// Return whether the mouse is over a resize handle
			return IsMouseOverAResizeHandle(mouseX, mouseY);
		}

		/// <summary>
		/// Returns true if the prop represented by the minimum and maximum vectors intersect with
		/// the rubberband frustum.
		/// </summary>				
		/// <param name="minimum">Minimum values of the prop along each axis</param>
		/// <param name="maximum">Maximum values of the prop along each axis</param>
		/// <returns>True if the prop intersects with the frustum created from the rubberband rectangle</returns>
		private bool IntersectsProp(			
			Vector3 minimum, 
			Vector3 maximum)
		{
			// Check to see if the prop intersects with the rubberband frustum
			return IsBoxIntersectingFrustum(
				CreateSelectionFrustum(), minimum, maximum);				
		}
		
		/// <summary>
		/// Returns true if the specified prop represented by the minimum and maximum vectors
		/// is contained by the rectangle formed by the start and end vectors.
		/// </summary>
		/// <param name="start">Start of the rectangle</param>
		/// <param name="end">End of the rectangle</param>
		/// <param name="minimum">Minimum values of the prop along each axis</param>
		/// <param name="maximum">Maximum values of the prop along each axis</param>
		/// <returns>True if the prop is contained by the rectangle</returns>
		private bool ContainsProp(Vector3 start, Vector3 end, Vector3 minimum, Vector3 maximum)
		{
			// Return whether the prop is contained by the rectangle
			return		
				minimum.X >= start.X && minimum.X <= end.X &&
				minimum.Y <= start.Y && minimum.Y >= end.Y &&
				maximum.X >= start.X && maximum.X <= end.X &&
				maximum.Y <= start.Y && maximum.Y >= end.Y;					
		}

		/// <summary>
		/// Returns true if the mouse is over a selected prop.
		/// </summary>
		/// <param name="mousePos">Position of the mouse</param>
		/// <returns>True if the mouse is over a selected prop</returns>
		private bool IsOverSelectedProp(Vector2 mousePos)
		{
			// Default to NOT being over a selected prop
			bool overSelectedProp = false;

			// Loop over the selected props
			foreach (IPropOpenGLData prop in SelectedProps)
			{
				// If the mouse is over the prop or
				// a prop move is in progress then...
				if (IsMouseOverProp(mousePos, prop) || _propMoveInProgress)
				{
					// Indicate we are over a selected prop
					overSelectedProp = true;
				}
			}

			// Return whether the mouse is over a selected prop
			return overSelectedProp;
		}

		/// <summary>
		/// Handles mouse down preview events to select props and start move operations.
		/// </summary>
		/// <param name="mouseX">Position of the mouse in X screen coordinates</param>
		/// <param name="mouseY">Position of the mouse in Y screen coordinates</param>
		public void MouseDown(int mouseX, int mouseY)
		{
			// Store off the mouse position in support of moving the camera
			_prevMousePosition = new Vector2i(mouseX, mouseY);
			
			// Set a flag that the preview mouse is down
			_mouseButtonDown = true;

			// Default to NOT being over a resize handle
			_overResizeHandleOnMouseDown = false;

			// Default to NOT being in a rubber band operation
			_rubberbandOperationInProgress = false;

			// Check to see if the mouse is over any prop
			bool mouseOverProp = IsMouseOverProp(mouseX, mouseY);
						
			// If only one prop is selected then...
			if (SelectedProps.Count == 1)
			{
				// Retrieve the selected prop
				IPropOpenGLData selectedProp = SelectedProps[0];	

				// Store off mouse position at the beginning of the move operation
				_startMousePosition.X = mouseX;
				_startMousePosition.Y = mouseY;
				
				// If the mouse is over the a resize handle then...
				if (selectedProp.MouseOverResizeHandle(
					Camera.Position,
					CreatePerspective(),
					Camera.ViewMatrix,
					OpenTkControl_Width,
					OpenTkControl_Height,
					new Vector2(mouseX, mouseY),
					out _activeResizeHandle))
				{
					// Store off the scale at the beginning of the resize operation
					_mouseDownInitialSize.X = selectedProp.SizeX;
					_mouseDownInitialSize.Y = selectedProp.SizeY;
					_mouseDownInitialSize.Z = selectedProp.SizeZ;
																	
					// Set a flag that the mouse started over a resize handle
					_overResizeHandleOnMouseDown = true;
				}				
			}
			
			// If in rubberband selection mode and
			// the mouse is not over a prop then...
			if ((IsSelectMode || IsPartialSelectMode) && !mouseOverProp)
			{
				// Remember that a rubberband operation is in progress
				_rubberbandOperationInProgress = true;

				// Store off the starting position of the rubberband
				_rubberbandStartInWorld = GetMousePointInWorld(new Vector2(mouseX, mouseY));

				// Save off the sta+rting positon of the rubberband selection
				_rubberBandStartX = mouseX;
				_rubberBandStartY = mouseY;

				// Clear all selected props
				ClearSelectedProps();
			}
		}

		#endregion

		#region Public Properties

		private bool _isSelectMode;
		
		/// <summary>
		/// True when in rubberband selection mode.
		/// When this flag is set the prop must be completely inside the rubberband selection rectangle.
		/// </summary>
		public bool IsSelectMode 
		{ 
			get
			{
				return _isSelectMode;
			}
			set
			{
				_isSelectMode = value;
				if (_isSelectMode)
				{
					IsPartialSelectMode = false;
				}

				OnPropertyChanged(nameof(IsPartialSelectMode));
				OnPropertyChanged(nameof(IsSelectMode));
				OnPropertyChanged(nameof(IsPanMode));
			}
		}

		private bool _isPartialSelectMode;

		/// <summary>
		/// True when in partial rubberband selection mode.
		/// When this flag is set the prop must intersect the rubberband selection rectangle.
		/// </summary>
		public bool IsPartialSelectMode 
		{ 
			get
			{
				return _isPartialSelectMode;
			}
			set
			{
				_isPartialSelectMode = value;
				if (_isPartialSelectMode)
				{
					IsSelectMode = false;
				}

				OnPropertyChanged(nameof(IsPartialSelectMode));
				OnPropertyChanged(nameof(IsSelectMode));
				OnPropertyChanged(nameof(IsPanMode));
			}
		}

		private bool _isPanMode;

		/// <summary>
		/// True when in panning mode.		
		/// </summary>
		public bool IsPanMode
		{
			get
			{
				return _isPanMode;
			}
			set
			{
				_isPanMode = value;
				if (_isPanMode)
				{
					IsSelectMode = false;
					IsPartialSelectMode = false;
				}

				OnPropertyChanged(nameof(IsPartialSelectMode));
				OnPropertyChanged(nameof(IsSelectMode));
				OnPropertyChanged(nameof(IsPanMode));
			}
		}

		/// <summary>
		/// Called when property value is changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Collection of Props being displayed on the preview.
		/// </summary>
		public List<IPropOpenGLData> Props { get; set; }
				
		/// <summary>
		/// OpenGL logic used to draw the background.
		/// </summary>
		public PropPreviewBackground? Background { get; set; }
		
		/// <summary>
		/// True if the preview has been loaded and initialized.
		/// </summary>
		public bool Initialized { get; set; }

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
				if (Background != null)
				{
					Background.Dispose();
					Background = null;
				}
			}

			// Call the base class implementation
			base.Dispose();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Clears all selected props.
		/// </summary>
		private void ClearSelectedProps()
		{
			// Loop over the selected props
			foreach (IPropOpenGLData prop in SelectedProps)
			{
				// Deselect all props
				prop.Selected = false;
			}

			// Clear the collection of selected props
			SelectedProps.Clear();
		}

		/// <summary>
		/// Returns true if the mouse is over a prop.
		/// </summary>
		/// <param name="mouseX">Mouse X axis screen position</param>
		/// <param name="mouseY">Mouse Y axis screen position</param>
		/// <returns>True if the mouse is over a prop</returns>
		private bool IsMouseOverProp(float mouseX, float mouseY)
		{
			// Default to the mouse not being over a prop
			bool mouseOverProp = false;

			// Loop over all props
			foreach (IPropOpenGLData prp in Props)
			{
				// If the mouse is over the specified prop then...
				if (IsMouseOverProp(new Vector2(mouseX, mouseY), prp))
				{
					// Remember we are over a prop
					mouseOverProp = true;
					break;
				}
			}

			// Return whether the mouse is over a prop
			return mouseOverProp;
		}

		/// <summary>
		/// Draws the rubberband selection rectangle.
		/// </summary>
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		private void DrawRubberband(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix)
		{
			// If a rubberband selection operation is in progress and
			// the rubberband drawing primitive has vertices then...
			if (_rubberbandOperationInProgress && _rubberbandPrimitive.Vertices.Count > 0)
			{
				// Draw the Rubberband Selection Rectangle
				DrawRubberbandSelectionRectangle(
					modelMatrix,
					viewMatrix,
					projectionMatrix,
					DrawLineUtility.DrawLineLoop);
			}
		}

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
			// Draw the front side of the cuboid
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					DrawLineUtility.DrawLineLoop(prop.GetSelectionCuboidFrontSide()));

			// Draw the back side of the cuboid
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					DrawLineUtility.DrawLineLoop(prop.GetSelectionCuboidBackSide()));

			// Draw the left side of the cuboid
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					DrawLineUtility.DrawLineLoop(prop.GetSelectionCuboidLeftSide()));

			// Draw the top side of the cuboid
			DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					DrawLineUtility.DrawLineLoop(prop.GetSelectionCuboidRightSide()));		
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
			// If only one prop is selected then...
			if (SelectedProps.Count == 1)
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
			// If only one prop is selected then...
			if (SelectedProps.Count == 1)
			{
				// Draw the move handle			
				DrawLines(
				modelMatrix,
				viewMatrix,
				projectionMatrix,
				(prop) =>
					DrawLineUtility.DrawLines(prop.GetCenterX()));
			}
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
		/// Draws lines associated with the rubberband selection rectangle.
		/// </summary>
		/// <param name="modelMatrix">Model matrix</param>
		/// <param name="viewMatrix">View matrix</param>
		/// <param name="projectionMatrix">Projection matrix</param>
		/// <param name="drawLines">Delegate to draw lines</param>
		private void DrawRubberbandSelectionRectangle(
			Matrix4 modelMatrix,
			Matrix4 viewMatrix,
			Matrix4 projectionMatrix,
			Action<IOpenGLDrawablePrimitive> drawLines)
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

				// Have the delegate draw lines
				drawLines(_rubberbandPrimitive);
				
				// Clear the OpenGL program
				GL.UseProgram(0);
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred in DrawRubberbandSelectionRectangle");
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
						Camera.Position,
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
			Vector3 RayDir = MouseHitTest.CreateRayFromMouse(
								mousePos,
								CreatePerspective(),
								Camera.ViewMatrix,
								OpenTkControl_Width,
								OpenTkControl_Height);
			
			// Return whether the mouse is over the prop
			return MouseHitTest.RayIntersectsAABB(Camera.Position, RayDir, prop.GetMinimum(), prop.GetMaximum(), out _);			
		}

		/// <summary>
		/// Returns a vector with specified ray intersects with the X-Y plane.
		/// </summary>
		/// <param name="origin">Origin of the ray (vector)</param>
		/// <param name="dir">Direction of the ray (vector)</param>
		/// <returns></returns>
		private Vector3 IntersectRayWithZ0(Vector3 origin, Vector3 dir)
		{
			// Avoid divide-by-zero if ray is parallel to plane
			if (Math.Abs(dir.Z) < 1e-6f)
				return Vector3.Zero;

			float t = -origin.Z / dir.Z;
			return origin + dir * t;
		}

		/// <summary>
		/// Gets the mouse position in world coordinates.
		/// </summary>
		/// <param name="mousePos">Mouse position in screen coordinates</param>
		/// <returns>Mouse position in world coordinates</returns>
		private Vector3 GetMousePointInWorld(Vector2 mousePos)
		{
			// Gets the mouse position vector
			Vector3 RayDir = MouseHitTest.CreateRayFromMouse(
				mousePos,
				CreatePerspective(),
				Camera.ViewMatrix,
				OpenTkControl_Width,
				OpenTkControl_Height);

			// Return where this mouse vector intersects the X-Y plane
			return IntersectRayWithZ0(Camera.Position, RayDir);
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

			Vector3 lastWorld = Unproject(previousMouseX, previousMouseY, depth, Camera.ViewMatrix, CreatePerspective());
			Vector3 currWorld = Unproject(mouseX, mouseY, depth, Camera.ViewMatrix, CreatePerspective());

			propPos = currWorld - lastWorld;

			return propPos;
		}

		/// <summary>
		/// Unprojects a screen coordinate into world coordinates.
		/// </summary>
		/// <param name="screenX">Screen position on the X axis</param>
		/// <param name="screenY">Screen position on the Y axis</param>
		/// <param name="depth">
		/// The depth value in the range [0.0, 1.0], where 0.0 represents 
		/// the Near clipping plane and 1.0 represents the Far clipping plane.
		/// </param>
		/// <param name="view">View matrix</param>
		/// <param name="proj">Project matrix</param>
		/// <returns>Screen position in world coordinates</returns>
		private Vector3 Unproject(float screenX, float screenY, float depth, Matrix4 view, Matrix4 proj)
		{
			// 1. Map Screen Pixels to NDC (-1 to 1)
			float ndcX = (2.0f * screenX / OpenTkControl_Width) - 1.0f;
			float ndcY = 1.0f - (2.0f * screenY / OpenTkControl_Height);

			// 2. Map Depth (0 to 1) to NDC (-1 to 1)
			// This is the missing piece in your second method!
			float ndcZ = (depth * 2.0f) - 1.0f;

			// 3. Transform by Inverse View-Projection
			Matrix4 invVP = Matrix4.Invert(view * proj);
			Vector4 ndc = new Vector4(ndcX, ndcY, ndcZ, 1.0f);

			// Using TransformRow to match OpenTK's Matrix * Vector convention
			Vector4 world = Vector4.TransformRow(ndc, invVP);

			// 4. Perspective Divide (Crucial for 3D depth)
			if (Math.Abs(world.W) > float.Epsilon)
			{
				return world.Xyz / world.W;
			}

			return world.Xyz;
		}

		/// <summary>
		/// Creates the rubberband selection frustum.
		/// </summary>
		/// <returns>Selection frustum as a collection of planes</returns>
		private List<Plane> CreateSelectionFrustum()
		{
			// Top-Left corner on the Near plane
			Vector3 ptTL_Near = Unproject(_normalizedRubberbandPt1X, _normalizedRubberbandPt1Y, 0.0f, Camera.ViewMatrix, CreatePerspective());
			// Bottom-Right corner on the Near plane
			Vector3 ptBR_Near = Unproject(_normalizedRubberbandPt2X, _normalizedRubberbandPt2Y, 0.0f, Camera.ViewMatrix, CreatePerspective());
			Vector3 ptTR_Near = Unproject(_normalizedRubberbandPt2X, _normalizedRubberbandPt1Y, 0.0f, Camera.ViewMatrix, CreatePerspective());
			Vector3 ptBL_Near = Unproject(_normalizedRubberbandPt1X, _normalizedRubberbandPt2Y, 0.0f, Camera.ViewMatrix, CreatePerspective());

			// Top-Left corner on the Far plane
			Vector3 ptTL_Far = Unproject(_normalizedRubberbandPt1X, _normalizedRubberbandPt1Y, 1.0f, Camera.ViewMatrix, CreatePerspective());
			// Bottom-Right corner on the Far plane
			Vector3 ptBR_Far = Unproject(_normalizedRubberbandPt2X, _normalizedRubberbandPt2Y, 1.0f, Camera.ViewMatrix, CreatePerspective());			
			Vector3 ptTR_Far = Unproject(_normalizedRubberbandPt2X, _normalizedRubberbandPt1Y, 1.0f, Camera.ViewMatrix, CreatePerspective());
			Vector3 ptBL_Far = Unproject(_normalizedRubberbandPt1X, _normalizedRubberbandPt2Y, 1.0f, Camera.ViewMatrix, CreatePerspective());
			
			List<Plane> selectionFrustum = new List<Plane>
			{
				new Plane(ptTL_Near, ptBL_Near, ptTR_Near), // Near Plane
				new Plane(ptTL_Far, ptTR_Far, ptBL_Far),    // Far Plane								
				new Plane(ptBL_Near, ptTL_Near, ptBL_Far),  // Left Plane
				new Plane(ptTR_Near, ptBR_Near, ptTR_Far),  // Right Plane
				new Plane(ptTL_Near, ptTR_Near, ptTL_Far),  // Top Plane
				new Plane(ptBL_Near, ptBL_Far, ptBR_Near)   // Bottom Plane
			};

			return selectionFrustum;
		}

		/// <summary>
		/// Returns true if the specified box intersects the frustum.
		/// </summary>
		/// <param name="frustum">Frustum to test</param>
		/// <param name="boxMin">Box minimum point</param>
		/// <param name="boxMax">Box maximum point</param>
		/// <returns>True if the box intersects the frustum</returns>
		private bool IsBoxIntersectingFrustum(List<Plane> frustum, Vector3 boxMin, Vector3 boxMax)
		{
			// Loop over the planes in the frustum
			foreach (Plane plane in frustum)
			{
				// The "Negative Vertex" is the corner of the box furthest OPPOSITE the normal
				Vector3 negativeVertex = new Vector3(
					plane.Normal.X >= 0 ? boxMin.X : boxMax.X,
					plane.Normal.Y >= 0 ? boxMin.Y : boxMax.Y,
					plane.Normal.Z >= 0 ? boxMin.Z : boxMax.Z
				);

				// If the point furthest along the normal is behind the plane, 
				// the entire box is definitely outside this plane.
				if (Vector3.Dot(plane.Normal, negativeVertex) > plane.Distance)
				{
					// Note: If you find ONE plane where the box is entirely on the "outside" 
					// side, there is no intersection.
					return false;
				}
			}
			return true;
		}

		#endregion
	}	
}
