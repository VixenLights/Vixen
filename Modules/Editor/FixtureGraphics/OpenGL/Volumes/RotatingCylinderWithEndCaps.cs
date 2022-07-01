using OpenTK;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
{
	/// <summary>
	/// Defines a rotating cylinder with end caps.
	/// </summary>
	public class RotatingCylinderWithEndCaps : CylinderWithEndCaps, IRotatableCylinder
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
		public RotatingCylinderWithEndCaps(float yOffset, float length, float bottomRadius, float topRadius, bool isDynamic) :
			base(yOffset, length, bottomRadius, topRadius, isDynamic)
		{
		}

		#endregion

		#region IRotatableCylinder
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Vector3 TiltRotation { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Vector3 TiltTranslation { get; set; }

		#endregion

		#region IVolume

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override void UpdateModelMatrix()
		{
			ModelMatrix =
				Matrix4.CreateRotationX(Rotation.X) *
				Matrix4.CreateRotationY(Rotation.Y) *
				Matrix4.CreateRotationZ(Rotation.Z) *
				Matrix4.CreateTranslation(Position) *

				Matrix4.CreateRotationX(TiltRotation.X) *
				Matrix4.CreateRotationY(TiltRotation.Y) *
				Matrix4.CreateRotationZ(TiltRotation.Z) *
				Matrix4.CreateTranslation(TiltTranslation) *

				Matrix4.CreateRotationX(GroupRotation.X) *
				Matrix4.CreateRotationY(GroupRotation.Y) *
				Matrix4.CreateRotationZ(GroupRotation.Z) *
				Matrix4.CreateTranslation(GroupTranslation);
		}

		#endregion
	}
}
