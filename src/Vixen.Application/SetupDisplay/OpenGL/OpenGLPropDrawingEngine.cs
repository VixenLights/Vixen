using Common.OpenGLCommon;
using Common.OpenGLCommon.Constructs.DrawingEngine;
using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Mathematics;

using Vixen.Sys.Props.Model;

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

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Preview data</param>		
		public OpenGLPropDrawingEngine(List<IPropModel> props)
		{		
			// Extract out the light props
			LightProps = props.Where(prp => prp is ILightPropModel).Select(prp => new LightPropOpenGLData((ILightPropModel)prp)).Cast<ILightPropOpenGLData>().ToList();

			// Extract out the intelligent fixture props
			IntelligentFixtureProps = props.Where(prp => prp is IntelligentFixtureModel).Select(prp => new IntelligentFixturePropOpenGLData((IntelligentFixtureModel)prp)).Cast<IntelligentFixturePropOpenGLData>().ToList();			
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
		protected override void UpdateLightShapePoints(float projectionHeight)
		{			
			Parallel.ForEach(LightProps, ParallelOptions, d => d.UpdateDrawPoints(projectionHeight));
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
			// Draw in a world view of 1.0 x 1.0
			// Note for Intelligent Fixtures to display a legend this needs to be changed to something like 400x400
			return 1.0f;
		}

		/// <inheritdoc/>		
		protected override float GetReferenceWidth()
		{
			// Draw in a world view of 1.0 x 1.0
			// Note for Intelligent Fixtures to display a legend this needs to be changed to something like 400x400
			return 1.0f;
		}

		/// <inheritdoc/>		
		protected override float GetZoomFactor()
		{
			// Since the props are being draw on a 1x1 coordinate system
			// The zoom needs to divided by a factor of 100
			return 100;
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
