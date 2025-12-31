using Vixen.Sys.Props.Model;

using Common.OpenGLCommon.Constructs.Vertex;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains OpenGL data structures required to draw a light based prop.
	/// </summary>
	public class LightPropOpenGLData : ILightPropOpenGLData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lightPropModel">Model associated with the prop</param>
		public LightPropOpenGLData(ILightPropModel lightPropModel)
		{
			// Store off the light prop model
			_propModel = lightPropModel;	
		}

		#endregion

		#region Fields

		/// <summary>
		/// Collection of vertices to be drawn.
		/// </summary>
		private List<float> _points = new List<float>();

		/// <summary>
		/// Model associated with the light based prop.
		/// </summary>
		private ILightPropModel _propModel;
		
		#endregion

		#region IPropOpenGLData
		
		/// <inheritdoc/>		
		public int PointsBufferSize { get; set; }

		/// <inheritdoc/>		
		public VBO<float> VertexBufferObject { get; set; }

		/// <inheritdoc/>		
		public int VAO { get; set; }

		/// <inheritdoc/>		
		public void UpdateDrawPoints(float referenceHeight)
		{
			_points.Clear();
			CreateFullColorPoints(referenceHeight);
		}

		/// <inheritdoc/>		
		public List<float> GetPoints()
		{
			return _points;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the vertex data required to draw the light points.
		/// </summary>
		/// <param name="referenceHeight">Logical height of the view</param>
		private void CreateFullColorPoints(float referenceHeight)
		{			
			// Loop over the 3-D points associated with the prop
			foreach (NodePoint nodePoint in _propModel.Nodes)
			{				
				// Adjust (scale) the 3-D points for the reference height
				_points.Add((float)(nodePoint.X * referenceHeight));
				_points.Add((float)(nodePoint.Y * referenceHeight));						
				_points.Add((float)(nodePoint.Z * referenceHeight));
						
				// Configure the vertex as Full White
				_points.Add(0xff); // R
				_points.Add(0xff); // G
				_points.Add(0xff); // B
				_points.Add(0xff); // Alpha (Brightness)
					
				// Add the light point size
				_points.Add(nodePoint.Size);				
			}
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>		
		public void Dispose()
		{
			// Dispose of the points Vertex Buffer Object
			if (VertexBufferObject != null)
			{
				(VertexBufferObject).Dispose();
				VertexBufferObject = null;
			}
		}

		#endregion
	}
}
