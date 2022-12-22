using OpenTK;
using VixenModules.Preview.VixenPreview.OpenGL.Extensions;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	public class Camera
	{
		private Vector3 position;
		private Quaternion _orientation;
		private Matrix4 _viewMatrix; // a cached version of the view matrix
		private bool dirty = true;  // true if the viewMatrix must be recalculated

		/// <summary>
		/// Modify the position of the eye of the camera.
		/// Triggers the view matrix to be recalculated.
		/// </summary>
		public Vector3 Position
		{
			get { return position; }
			set
			{
				position = value;
				dirty = true;
			}
		}

		/// <summary>
		/// Modify the orientation of the camera.
		/// Triggers the view matrix to be recalculated.
		/// </summary>
		public Quaternion Orientation
		{
			get { return _orientation; }
			set
			{
				_orientation = value;
				dirty = true;
			}
		}

		/// <summary>
		/// Requests the view matrix of the camera (taking into account position and orientation).
		/// This property will recalculate the matrix automatically if necessary,
		/// otherwise it will use a cached copy (which is useful if your camera isn't moving often).
		/// </summary>
		public Matrix4 ViewMatrix
		{
			get
			{
				if (dirty)
				{
					_viewMatrix = Matrix4.CreateTranslation(-position) * Matrix4.CreateFromQuaternion(_orientation); ;
					dirty = false;
				}
				return _viewMatrix;
			}
		}

		/// <summary>
		/// Create a new camera object with a certain eye position and orientation.
		/// </summary>
		/// <param name="position">The eye position of the camera.</param>
		/// <param name="orientation">The orientation of the camera.</param>
		public Camera(Vector3 position, Quaternion orientation)
		{
			this.Position = position;
			this.Orientation = orientation;
		}

		/// <summary>
		/// Modifies the orientation of the camera to get the camera to look in a particular direction.
		/// </summary>
		/// <param name="direction">The direction to have the camera look.</param>
		public void SetDirection(Vector3 direction)
		{
			if (direction == Vector3.Zero) return;

			Vector3 zvec = -direction.Normalized();
			Vector3 xvec = Vector3.Cross(Vector3.UnitY, zvec).Normalized();
			Vector3 yvec = Vector3.Cross(zvec, xvec).Normalized();
			Orientation = GlExtensions.FromAxis(xvec, yvec, zvec);
		}

		/// <summary>
		/// Moves the camera by modifying the position directly.
		/// </summary>
		/// <param name="by">The amount to move the position by.</param>
		public void Move(Vector3 by)
		{
			Position += by;
		}

		/// <summary>
		/// Moves the camera taking into account the orientation of the camera.
		/// This is useful if you want to move in the direction that the camera is facing.
		/// </summary>
		/// <param name="by">The amount to move the position by, relative to the camera.</param>
		public void MoveRelative(Vector3 by)
		{
			Position += Orientation * by;
		}

		/// <summary>
		/// Rotates the camera by a supplied rotation quaternion.
		/// </summary>
		/// <param name="rotation">The amount to rotate the camera by.</param>
		public void Rotate(Quaternion rotation)
		{
			Orientation = rotation * _orientation;
		}

		/// <summary>
		/// Rotates the camera around a specific axis.
		/// </summary>
		/// <param name="angle">The amount to rotate the camera.</param>
		/// <param name="axis">The axis about which the rotation occurs.</param>
		public void Rotate(float angle, Vector3 axis)
		{
			Rotate(Quaternion.FromAxisAngle(axis, angle));
		}

		/// <summary>
		/// Rotates the camera about the Z axis.
		/// </summary>
		/// <param name="angle">The amount to rotate the camera by.</param>
		public void Roll(float angle)
		{
			Vector3 axis = _orientation * Vector3.UnitZ;
			Rotate(angle, axis);
		}

		/// <summary>
		/// Rotates the camera about the Y axis.  Assumes a fixed Y axis of Vector3.Up.
		/// </summary>
		/// <param name="angle">The amount to rotate the camera by.</param>
		public void Yaw(float angle)
		{
			// this method assumes that the y direction will always be 'up', so we've fixed the yaw
			// which is more useful for FPS games, etc.  For flight simulators, or other applications
			// of an unfixed yaw, simply replace Vector3.Up with (orientation * Vector3.UnitY)
			Rotate(angle, Vector3.UnitY);
		}

		/// <summary>
		/// Rotates the camera about the X axis.
		/// </summary>
		/// <param name="angle">The amount to rotate the camera by.</param>
		public void Pitch(float angle)
		{
			Vector3 axis = _orientation * Vector3.UnitX;
			Rotate(angle, axis);
		}
	}
}
