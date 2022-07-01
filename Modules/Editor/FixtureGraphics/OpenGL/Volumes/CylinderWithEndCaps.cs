using OpenTK;
using System.Collections.Generic;
using System.Drawing;

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
		public CylinderWithEndCaps(float yOffset, float length, float bottomRadius, float topRadius, bool isDynamic) : base(isDynamic)
		{						
			// Create the cylinder with end caps
			CreateCylinderWithEndCaps(yOffset, length, bottomRadius, topRadius, Normals, Vertices);
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
		float _previousBottomRadius = -1;
		float _previousTopRadius = -1;
		float _previousLength = -1;

		#endregion

		#region IUpdateCyclinder

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Update(
			float yOffset,
			float length,
			float bottomRadius,
			float topRadius)		
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
				CreateCylinderWithEndCaps(yOffset, length, bottomRadius, topRadius, Normals, Vertices);

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
		private void CreateCylinderWithEndCaps(
			float yOffset, 
			float length, 
			float bottomRadius, 
			float topRadius,
			List<Vector3> normals,
			List<Vector3> triangleVertices)
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
				triangleVertices);

			// Create the end caps
			for (int index = 0; index < panelVertices.Count / 2; index += 4)
			{
				normals.Add(new Vector3(0, -1, 0)); // 1
				normals.Add(new Vector3(0, -1, 0)); // 2
				normals.Add(new Vector3(0, -1, 0)); // 3
				normals.Add(new Vector3(0, -1, 0)); // 4
				normals.Add(new Vector3(0, -1, 0)); // 5
				normals.Add(new Vector3(0, -1, 0)); // 6

				triangleVertices.Add(panelVertices[index]);                           // 0 
				triangleVertices.Add(panelVertices[index + 3]);                       // 1
				triangleVertices.Add(panelVertices[index + panelVertices.Count / 2]); // 2

				triangleVertices.Add(panelVertices[index]);                               // 0
				triangleVertices.Add(panelVertices[index + panelVertices.Count / 2]);     // 2
				triangleVertices.Add(panelVertices[index + 3 + panelVertices.Count / 2]); // 3

				normals.Add(new Vector3(0, 1, 0)); // 1
				normals.Add(new Vector3(0, 1, 0)); // 2
				normals.Add(new Vector3(0, 1, 0)); // 3
				normals.Add(new Vector3(0, 1, 0)); // 4
				normals.Add(new Vector3(0, 1, 0)); // 5
				normals.Add(new Vector3(0, 1, 0)); // 6
		
				triangleVertices.Add(panelVertices[index + 1]);                           // 0
				triangleVertices.Add(panelVertices[index + 2]);                           // 1
				triangleVertices.Add(panelVertices[index + 1 + panelVertices.Count / 2]); // 2

				triangleVertices.Add(panelVertices[index + 1]);                           // 0
				triangleVertices.Add(panelVertices[index + 1 + panelVertices.Count / 2]); // 2
				triangleVertices.Add(panelVertices[index + 2 + panelVertices.Count / 2]); // 3
			}			
		}
		
		#endregion
	}
}
