using OpenTK;
using System;
using System.Collections.Generic;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines a cylinder base class with configurable bottom and top radius.
	/// </summary>
	public abstract class CylinderBase : VolumeBase
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>		
		/// <param name="isDynamic">Whether the cylinder changes shape during execution</param>
		public CylinderBase(bool isDynamic) : base(isDynamic)
		{			
		}

		#endregion
		
		#region IVolume
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override void UpdateModelMatrix()
		{
			// Initialize the model matrix
			ModelMatrix = Matrix4.CreateRotationX(Rotation.X) *
			              Matrix4.CreateRotationY(Rotation.Y) *
			              Matrix4.CreateRotationZ(Rotation.Z) *
			              Matrix4.CreateTranslation(Position) *
			              Matrix4.CreateRotationX(GroupRotation.X) *
			              Matrix4.CreateRotationY(GroupRotation.Y) *
			              Matrix4.CreateRotationZ(GroupRotation.Z) *
			              Matrix4.CreateTranslation(GroupTranslation);
		}

		#endregion

		#region Protected Types

		/// <summary>
		/// Delegate for retrieving the cylinder panel vertices.
		/// </summary>
		/// <param name="length">Length of the panel</param>
		/// <param name="theta">Angle at the start of the panel</param>
		/// <param name="nextPanelAngle">Angle of the next panel</param>
		/// <param name="bottomRadius">Radius as the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>		
		/// <param name="offset">Optional Y offset of the cylinder</param>		
		/// <returns>Vertices that make up the panel</returns>
		protected delegate List<Vector3> GetCylinderPanelVertices(float length, double theta, double nextPanelAngle, float bottomRadius, float topRadius, float offset);

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets horizontal panel vertices.
		/// </summary>
		/// <param name="length">Length of the panel</param>
		/// <param name="theta">Angle at the start of the panel</param>
		/// <param name="nextPanelAngle">Angle of the next panel</param>
		/// <param name="bottomRadius">Radius as the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>		
		/// <param name="offset">Optional Y offset of the cylinder</param>		
		/// <returns>Vertices that make up the panel</returns>
		protected List<Vector3> GetHorizontalCylinderPanelVertices(
			float length, 
			double theta, 
			double nextPanelAngle, 
			float topRadius, 
			float bottomRadius, 
			float offset)
		{			
			// Define the first point that makes up the cylinder panel
			Vector3 v1 = new Vector3()
			{
				X = -length / 2.0f,
				Y = (float)(topRadius * Math.Cos(theta)),
				Z = (float)(topRadius * Math.Sin(theta)),
			};

			// Define the second point that makes up the cylinder panel
			Vector3 v2 = new Vector3()
			{
				X = length / 2.0f,
				Y = (float)(topRadius * Math.Cos(theta)),
				Z = (float)(topRadius * Math.Sin(theta)),
			};

			// Define the third point that makes up the cylinder panel
			Vector3 v3 = new Vector3()
			{
				X = length / 2.0f,
				Y = (float)(topRadius * Math.Cos(nextPanelAngle)),
				Z = (float)(topRadius * Math.Sin(nextPanelAngle)),
			};

			// Define the fourth point that makes up the cylinder panel
			Vector3 v4 = new Vector3()
			{
				X = -length / 2.0f,
				Y = (float)(topRadius * Math.Cos(nextPanelAngle)),
				Z = (float)(topRadius * Math.Sin(nextPanelAngle)),
			};

			// Add the vertices to a collection
			List<Vector3> vertices = new List<Vector3>();
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			vertices.Add(v4);

			// Return the collection of vertices
			return vertices;
		}

		/// <summary>
		/// Gets vertical cylinder panel vertices.
		/// </summary>
		/// <param name="length">Length of the panel</param>
		/// <param name="theta">Angle at the start of the panel</param>
		/// <param name="nextPanelAngle">Angle of the next panel</param>
		/// <param name="bottomRadius">Radius as the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>		
		/// <param name="offset">Optional Y offset of the cylinder</param>		
		/// <returns>Vertices that make up the panel</returns>				
		protected List<Vector3> GetVerticalCylinderPanelVertices(
			float length,
			double theta,
			double nextPanelAngle,
			float bottomRadius,
			float topRadius,
			float offset)
		{
			// Bottom Left
			Vector3 v1 = new Vector3()
			{
				X = (float)(bottomRadius * Math.Cos(theta)),
				Y = (float)-offset,
				Z = (float)(bottomRadius * Math.Sin(theta)),
			};

			// Top Left
			Vector3 v2 = new Vector3()
			{
				X = (float)(topRadius * Math.Cos(theta)),
				Y = length - offset,
				Z = (float)(topRadius * Math.Sin(theta)),
			};

			// Top Right
			Vector3 v3 = new Vector3()
			{
				X = (float)(topRadius * Math.Cos(nextPanelAngle)),
				Y = (float)length - offset,
				Z = (float)(topRadius * Math.Sin(nextPanelAngle)),
			};

			// Bottom Right
			Vector3 v4 = new Vector3()
			{
				X = (float)(bottomRadius * Math.Cos(nextPanelAngle)),
				Y = (float)-offset,
				Z = (float)(bottomRadius * Math.Sin(nextPanelAngle)),
			};

			// Add the vertices to a collection
			List<Vector3> vertices = new List<Vector3>();
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			vertices.Add(v4);

			// Return the collection of vertices
			return vertices;
		}

		/// <summary>
		/// Creates a cylinder with the specified parameters.
		/// </summary>
		/// <param name="length">Length of the cylinder</param>
		/// <param name="getVertices">Delegate to create either horizontal or vertical oriented cylinder</param>
		/// <param name="bottomRadius">Radius as the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>		
		/// <param name="offset">Optional Y offset of the cylinder</param>		
		/// <param name="negateNormals">Flag to control if the normals are negated</param>
		/// <param name="normals">Collection of normal</param>
		/// <param name="triangleVertices">Collection of vertices</param>
		/// <returns>All vertices that make up the cylinder</returns>
		protected List<Vector3> CreateCylinder(
			float length,
			GetCylinderPanelVertices getVertices,
			float bottomRadius,
			float topRadius,
			float offset,
			bool negateNormals,
			List<Vector3> normals,
			List<Vector3> triangleVertices)
		{
			// Create a collection to hold all the cylinder vertices
			List<Vector3> allVertices = new List<Vector3>();

			// Define the number panels (Higher number improves smoothness quality) 
			int segments = 20; 
						
			// Loop over the segments 
			for (double x = 0; x < segments; x++)
			{
				// Determine an angle for the panel by dividing 360 degrees into panels
				double theta = (((double) x) / (segments - 1)) * 2 * Math.PI;

				// Determine the angle of the next panel
				double theta1 = (((double) (x + 1)) / (segments - 1)) * 2 * Math.PI;

				// Get the vertices that make up the panel
				List<Vector3> vertices = getVertices(length, theta, theta1, bottomRadius, topRadius, offset);
				Vector3 v1 = vertices[0];
				Vector3 v2 = vertices[1];
				Vector3 v3 = vertices[2];
				Vector3 v4 = vertices[3];

				// Add all the vertices of the panel to the collection of all vertices that make up the cylinder
				allVertices.AddRange(vertices);
				
				// Determine which direction to point the normals
				if (negateNormals)
				{
					// Set the normal vector for the first point
					normals.Add(Vector3.Cross(v2 - v1, v4 - v1).Normalized());

					// Set the normal vector for the second point
					normals.Add(-1 * Vector3.Cross(v1 - v2, v3 - v2).Normalized());

					// Set the normal vector for the third point
					normals.Add(-1 * Vector3.Cross(v2 - v3, v4 - v3).Normalized());

					// Set the normal vector for the first point
					normals.Add(Vector3.Cross(v2 - v1, v4 - v1).Normalized());

					// Set the normal vector for the second point
					normals.Add((-1 * Vector3.Cross(v1 - v2, v3 - v2).Normalized()));

					// Set the normal vector for the fourth point
					normals.Add(Vector3.Cross(v1 - v4, v3 - v4).Normalized());
				}
				else
				{
					// Set the normal vector for the first point
					normals.Add(-1 * Vector3.Cross(v2 - v1, v4 - v1).Normalized());

					// Set the normal vector for the second point
					normals.Add(Vector3.Cross(v1 - v2, v3 - v2).Normalized());

					// Set the normal vector for the third point
					normals.Add(Vector3.Cross(v2 - v3, v4 - v3).Normalized());

					// Set the normal vector for the first point
					normals.Add((-1 * Vector3.Cross(v2 - v1, v4 - v1).Normalized()));

					// Set the normal vector for the second point
					normals.Add(Vector3.Cross(v1 - v2, v3 - v2).Normalized());
					
					// Set the normal vector for the fourth point
					normals.Add(-1 * Vector3.Cross(v1 - v4, v3 - v4).Normalized());
				}

				// Define triangle #1
				triangleVertices.Add(v1); // 0				
				triangleVertices.Add(v2); // 1				
				triangleVertices.Add(v3); // 2
				
				// Define triangle #2
				triangleVertices.Add(v1); // 0
				triangleVertices.Add(v3); // 2				
				triangleVertices.Add(v4); // 3
			}

			return allVertices;
		}

		#endregion
	}
}
