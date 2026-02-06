using System.ComponentModel;

using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Mathematics;

using VixenApplication.SetupDisplay.OpenGL.Shapes;

using VixenModules.App.Props.Models.IntellligentFixture;
using VixenModules.Editor.FixtureGraphics;
using VixenModules.Editor.FixtureGraphics.OpenGL;
using VixenModules.Editor.FixtureGraphics.OpenGL.Volumes;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains OpenGL data structures required to draw an intelligent fixture prop.
	/// </summary>
	public class IntelligentFixturePropOpenGLData : PropOpenGLData,
		IIntelligentFixturePropOpenGLData, IOpenGLMovingHeadShape, IDrawStaticPreviewShape
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prop">Intelligent fixture prop model</param>
		public IntelligentFixturePropOpenGLData(IntelligentFixtureModel propModel) : base(propModel)
		{
			// Store off the intelligent fixture prop model
			_propModel = propModel;			
		}

		#endregion

		#region Fields

		/// <summary>
		/// Intelligent fixture prop model.
		/// </summary>
		private IntelligentFixtureModel _propModel;

		/// <summary>
		/// OpenGL implementation of the moving head graphic.
		/// </summary>
		private MovingHeadOpenGL _movingHeadOpenGL;

		/// <summary>
		/// Current moving head settings.  This property aids with determining if the graphics are stale.
		/// </summary>
		private IMovingHead _movingHeadCurrentSettings;

		#endregion

		#region IDrawStaticPreviewShape

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		/// <remarks>
		/// The majority of the moving head drawing is done by the parent form and <c>MovingHeadRenderStrategy</c> class.
		/// The drawing is done by the parent so that if there are multiple moving heads all the graphical volumes that use the same
		/// shader can be collected.  This minimizes the number of shader program transitions per frame.
		/// </remarks>
		public void DrawOpenGL(			
			Matrix4 projectionMatrix,
			Matrix4 viewMatrix,
			int referenceHeight)			
		{
			// If the legned is to be shown then...
			if (_propModel.ShowLegend)
			{				
				// Draw the legend under the moving head			
				_movingHeadOpenGL.DrawLegend(
					0, // Translation not required					
					0, // Translation not required					
					projectionMatrix,
					viewMatrix);
			}
		}

		/// <inheritdoc/>		
		public void DisposeOpenGLResources()
		{
			// If the moving head has not already been disposed then...
			if (_movingHeadOpenGL != null)
			{
				// Dispose the OpenGL resources
				_movingHeadOpenGL.Dispose();
				_movingHeadOpenGL = null;
			}
		}

		#endregion

		#region IOpenGLMovingHeadShape

		/// <inheritdoc/>
		[Browsable(false)]
		public IRenderMovingHeadOpenGL MovingHead { get; private set; }

		/// <inheritdoc/>
		public void Initialize(float height, float referenceHeight, Action redraw)
		{
			// Create the moving head OpenGL implementation
			_movingHeadOpenGL = new MovingHeadOpenGL();

			// Set the moving head settings to the OpenGL settings
			_movingHeadCurrentSettings = _movingHeadOpenGL.MovingHead;

			_movingHeadCurrentSettings.BeamColorLeft = Color.Purple;
			_movingHeadCurrentSettings.BeamColorRight = Color.Purple;
			_movingHeadCurrentSettings.IncludeLegend = false;
			_movingHeadCurrentSettings.TiltAngle = 135.0;
			_movingHeadCurrentSettings.PanAngle = 215.0;
			_movingHeadCurrentSettings.BeamLength = 40;
			_movingHeadCurrentSettings.Focus = 40;

			_movingHeadCurrentSettings.IncludeLegend = true;
			_movingHeadCurrentSettings.Legend = "R1";

			// Initialize the moving head
			_movingHeadOpenGL.Initialize(height, referenceHeight, (100.0 - _propModel.BeamTransparency) / 100.0, _propModel.BeamWidthMultiplier, _propModel.MountingPosition);

			// Expose the moving head as a property
			MovingHead = _movingHeadOpenGL;			
		}

		/// <inheritdoc/>
		public void InitializeSelectionVertices(float height)
		{
			// Get the physical fixture volumes (no beam volumes)
			List<IVolume> fixtureVolumes = _movingHeadOpenGL.GetPhysicalFixtureVolumes().ToList();

			// Create a collection of vertex coordinates
			List<Vector3> coordinates = new();

			// Loop over the fixture volumes
			foreach (IVolume fixtureVolume in fixtureVolumes)
			{
				// Get the vertices of the volume
				Vector3[] verts = fixtureVolume.GetVertices();
				
				// Loop over the vertices
				foreach (Vector3 v in verts)
				{
					// Create a vector4 from the vertice
					Vector4 v4 = new Vector4(v, 1.0f);

					// Update vertice based on the model matrix
					Vector4 vf = Vector4.TransformRow(v4, fixtureVolume.ModelMatrix);					
					
					// Add the vertice to the coordinates collection
					coordinates.Add(vf.Xyz);
				}
			}

			// Initialize the selection vertices
			InitializeSelectionVertices(coordinates, height);
		}
		
		/// <inheritdoc/>
		public void UpdateVolumes(int maxBeamLength, float referenceHeight, bool standardFrame)
		{			
			// Update the volumes on the OpenGL implementation
			_movingHeadOpenGL.UpdateVolumes(
				(int)X,
				(int)Y,
				maxBeamLength);			
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{			
		}

		#endregion		
	}
}
