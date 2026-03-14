using Common.OpenGLCommon;
using Common.OpenGLCommon.Constructs.DrawingEngine;
using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Mathematics;

using Vixen.Sys.Props.Model;

using VixenApplication.SetupDisplay.OpenGL.Shapes;

using VixenModules.App.Props.Models.IntellligentFixture;


namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// OpenGL logic to draw props.
	/// </summary>
	public class OpenGLPropDrawingEngine : OpenGLDrawingEngineBase<ILightPropOpenGLData>, IDisposable
	{
		#region Private Properties

		/// <summary>
		/// Collection of light based props.
		/// </summary>
		private List<ILightPropOpenGLData> LightProps { get; set; }

		/// <summary>
		/// Collection of intelligent fixtures.
		/// </summary>
		private List<IntelligentFixturePropOpenGLData> IntelligentFixtureProps { get; set; }

		/// <summary>
		/// Height of the 3-D world.  It is assumed that the height and width are equal.
		/// </summary>
		private int _referenceHeight;
		
		/// <summary>
		/// Zoom factor adjustment.
		/// </summary>
		/// <remarks>If the world coordinates are 0 - 1 the zoom needs to be adjusted</remarks>
		private float _zoomFactor;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="props">Props to display</param>
		/// <param name="referenceHeight">Height of the world</param>
		/// <param name="zoomFactor">Zoom factor adjustment</param>
		/// <remarks>
		/// For most of the props we are attempting to use a reference height of 1.0.
		/// Exception being the Intelligent Fixture.  The Intelligent Fixture requires a large world
		/// height.  For Intelligent Fixtures intending to use the height of the WPF OpenTK control height.
		/// </remarks>
		public OpenGLPropDrawingEngine(List<IPropModel> props, int referenceHeight, float zoomFactor)
		{		
			// Extract out the light props
			LightProps = props.Where(prp => prp is ILightPropModel).Select(prp => new LightPropOpenGLData((ILightPropModel)prp)).Cast<ILightPropOpenGLData>().ToList();
				
			// Extract out the intelligent fixture props
			IntelligentFixtureProps = props.Where(prp => prp is IntelligentFixtureModel).Select(prp => new IntelligentFixturePropOpenGLData((IntelligentFixtureModel)prp)).Cast<IntelligentFixturePropOpenGLData>().ToList();

			// Save off the height of the world
			_referenceHeight = referenceHeight;

			// Save off the zoom factor
			_zoomFactor = zoomFactor;

			// Loop over the light props
			foreach (IPropOpenGLData prop in LightProps)
			{				
				prop.SizeX = _referenceHeight;
				prop.SizeY = _referenceHeight;
				prop.SizeZ = _referenceHeight;
			}

			// Loop over the intelligent fixtures
			foreach (IntelligentFixturePropOpenGLData prop in IntelligentFixtureProps)
			{				
				prop.SizeY = _referenceHeight;
			}
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
		protected override void UpdateLightShapePoints(float referenceHeight)
		{
			Parallel.ForEach(LightProps, ParallelOptions, d => d.UpdateDrawPoints(referenceHeight));
		}

		/// <inheritdoc/>		
		protected override void DrawPoints(ILightPropOpenGLData prop)
		{
			// Draw the line points associated with the prop
			DrawPointsUtility.DrawPoints(prop);			
		}

		/// <inheritdoc/>		
		protected override void RenderPreviewInternal()
		{			
			// Initialize the moving head render strategy with the applicable display item shapes
			InitializeMovingHeadRenderStrategy(OnRenderFrameOnGUIThread);

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

				//Logging.Info($"GL Error: {GL.GetError()}");
			
				// Render static preview shapes (moving heads)
				RenderStaticPreviewShapes(perspective, true, OpenTkControl_Width, OpenTkControl_Height);

				// Draw the light points
				DrawPoints(mvp);
			}					
		}

		/// <inheritdoc/>		
		protected override float GetReferenceHeight()
		{
			// For light props expect a world view of 1.0 x 1.0
			// For Intelligent Fixtures to display a legend this needs > 1.0 so we are using the OpenTK control height.
			return _referenceHeight;
		}

		/// <inheritdoc/>		
		protected override float GetReferenceWidth()
		{
			// For light props expect a world view of 1.0 x 1.0
			// For Intelligent Fixtures to display a legend this needs > 1.0 so we are using the OpenTK control height.
			return _referenceHeight;
		}

		/// <inheritdoc/>		
		protected override float GetZoomFactor()
		{
			// Light props are being draw on a 1x1 world coordinate system
			// so the zoom factor is expected to be a value of 100.
			// For Intelligent fixture the world coordinate system matches the height of the OpenTK control
			// so the zoom factor would be expected to be a value of 1.
			return _zoomFactor;
		}

		/// <inheritdoc/>		
		protected override void InitializeBackground()
		{
			// Prop Preview does not use a background image
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
	}
}
