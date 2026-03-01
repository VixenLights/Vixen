using OpenTK.Mathematics;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains a plane in 3-D space.
	/// </summary>
	/// <remarks>
	/// This class is used to support creating a frustrum.
	/// </remarks>
	public class Plane
	{		
		#region Constructor

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="p1">First point in plane</param>
		/// <param name="p2">Second point in plane</param>
		/// <param name="p3">Third point in plane</param>
		public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 v1 = p2 - p1;
			Vector3 v2 = p3 - p1;
			Normal = Vector3.Normalize(Vector3.Cross(v1, v2));
			Distance = Vector3.Dot(Normal, p1);
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Defines the normal vector for the plane.
		/// </summary>
		public Vector3 Normal { get; set; }

		/// <summary>
		/// Maintains the signed distance from the world origin (0, 0, 0) to the plane, 
		/// measured along the plane's normal vector. 
		/// </summary>
		public float Distance { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns true if the specified point is in front of the plane.
		/// In front is determined by the plane normal vector.
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the point is in front of the plane.</returns>
		public bool IsPointInFront(Vector3 point)
		{
			return Vector3.Dot(Normal, point) >= Distance;
		}

		#endregion
	}
}
