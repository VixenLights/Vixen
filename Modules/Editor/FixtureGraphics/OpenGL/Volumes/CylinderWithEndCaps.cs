using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Vector3 = OpenTK.Vector3;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
{
	/// <summary>
	/// Base class for a cylinder with end caps.
	/// </summary>
	public abstract class CylinderWithEndCaps : CylinderBase, ISpecifyVolumeColor, IUpdateCylinder
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="yOffset">Y offset of the volume</param>
		/// <param name="length">Length of the cylinder</param>
		/// <param name="bottomRadius">Radius of the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>
		/// <param name="isDynamic">Whether the cylinder changes shape during execution</param>
		/// <param name="panelIndices">Panel indices to draw</param>		
		public CylinderWithEndCaps(
			float yOffset, 
			float length, 
			float bottomRadius, 
			float topRadius, 
			bool isDynamic,
			List<int> panelIndices) : 
			base(isDynamic)
		{						
			// Create the cylinder with end caps
			CreateCylinderWithEndCaps(yOffset, length, bottomRadius, topRadius, Normals, Vertices, panelIndices);
		}

		#endregion

		#region ISpecifyVolumeColor

		/// <summary>
		/// Refer to interface documenation.
		/// </summary>
		public Color Color
		{
			get;
			set;
		}

		#endregion

		#region Fields
		
		/// <summary>
		/// The following fields determine if the cyclinder geometry is stale and needs to transfered to the GPU.		
		/// </summary>
		private float _previousBottomRadius = -1;
		private float _previousTopRadius = -1;
		private float _previousLength = -1;

		#endregion

		#region IUpdateCyclinder

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Update(
			float yOffset,
			float length,
			float bottomRadius,
			float topRadius,
			List<int> panelIndices)
		{
			// If any of the parameters that make up the cylinder have changed then...
			if (length != _previousLength ||
				bottomRadius != _previousBottomRadius ||
				topRadius != _previousTopRadius)
			{
				// Clear the vertex data
				Vertices.Clear();
				Normals.Clear();

				// Re-create the cylinder
				CreateCylinderWithEndCaps(yOffset, length, bottomRadius, topRadius, Normals, Vertices, panelIndices);

				// Save off the cylinder settings
				_previousLength = length;
				_previousBottomRadius = bottomRadius;
				_previousTopRadius = topRadius;

				// Remember that the vertex data is dirty
				VertexDataIsDirty = true;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates a cylinder with end caps.
		/// </summary>
		/// <param name="yOffset">Y Offset of the cylinder</param>
		/// <param name="length">Length of the cylinder</param>		
		/// <param name="bottomRadius">Radius as the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>						
		/// <param name="normals">Collection of normal</param>
		/// <param name="triangleVertices">Collection of vertices</param>
		/// <param name="panelIndices">Panel indices to draw</param>		
		private void CreateCylinderWithEndCaps(
			float yOffset, 
			float length, 
			float bottomRadius, 
			float topRadius,
			List<Vector3> normals,
			List<Vector3> triangleVertices,
			List<int> panelIndices)
		{
			// Create the circular shell of the cylinder
			List<Vector3> panelVertices = CreateCylinder(
				length,
				GetVerticalCylinderPanelVertices,
				bottomRadius,
				topRadius,
				yOffset,
				true,
				normals,
				triangleVertices,
				panelIndices);
			
			// Create the end caps
			for (int index = 0; index < panelVertices.Count; index += 4)
			{
				// Normals for the bottom triangle
				normals.Add(new Vector3(0, -1, 0)); 
				normals.Add(new Vector3(0, -1, 0)); 
				normals.Add(new Vector3(0, -1, 0)); 
				
				// Get the three points to define a pie wedge on the bottom
				Vector3 v1 = panelVertices[index];      // 0 
				Vector3 v2 = panelVertices[index + 3];  // 1
				Vector3 centerBottom = new Vector3();
				centerBottom.X = 0; 
				centerBottom.Y = -yOffset; 
				centerBottom.Z = 0; 

				// Add the bottom pie wedge points to the vertex collection
				triangleVertices.Add(v1);    
				triangleVertices.Add(v2);    
				triangleVertices.Add(centerBottom);

				// Get the three points to define a pie wedge on the top
				Vector3 vTopPlus1 = panelVertices[index + 1];  // 0 
				Vector3 vTopPlus2 = panelVertices[index + 2];  // 1
				Vector3 centerTop = new Vector3();
				centerTop.X = 0; 
				centerTop.Y = yOffset; 
				centerTop.Z = 0;

				// Add the top pie wedge points to the vertex collection
				triangleVertices.Add(vTopPlus1);
				triangleVertices.Add(vTopPlus2);
				triangleVertices.Add(centerTop);

				// Add the normals for the top triangle
				normals.Add(new Vector3(0, 1, 0)); 
				normals.Add(new Vector3(0, 1, 0)); 
				normals.Add(new Vector3(0, 1, 0)); 
			}			
		}
		
		#endregion
	}
}
