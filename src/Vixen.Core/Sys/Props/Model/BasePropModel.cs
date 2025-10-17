using System.Collections.ObjectModel;

using OpenTK.Mathematics;

using Vixen.Model;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Abstract base class for prop model.
	/// </summary>
	public abstract class BasePropModel : BindableBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		protected BasePropModel()
		{
		}

		#endregion

		#region IPropModel

		/// <inheritdoc/>		
		public Guid Id { get; init; } = Guid.NewGuid();

		#endregion

		#region Protected Methods

		/// <summary>
		/// Rotates the specified vertices by up to three axis rotations.
		/// </summary>
		/// <param name="vertices">Vertices to rotate</param>
		protected void RotatePoints(List<NodePoint> vertices, ObservableCollection<AxisRotationModel>rotations)
		{
			// Loop over the rotations because order matters
			foreach (AxisRotationModel rm in rotations)
			{
				// If there is a rotation angle then...
				if (rm.RotationAngle != 0.0)
				{
					// Create the applicable rotation matrix
					Matrix4 rotation = GetRotationMatrix(rm.Axis, rm.RotationAngle);

					// Loop over the vertices
					foreach (NodePoint pt in vertices)
					{
						// Convert the vertice to an OpenTK Vector
						Vector4 point4 = new Vector4((float)pt.X, (float)pt.Y, (float)pt.Z, 1.0f); // w = 1 for position

						// Rotate the vertex
						Vector4 rotatedPoint4 = rotation * point4;

						// Push the data back into the NodePoint
						pt.X = rotatedPoint4.X;
						pt.Y = rotatedPoint4.Y;
						pt.Z = rotatedPoint4.Z;
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets a rotation matrix for the specified axis and degrees of rotation.
		/// </summary>
		/// <param name="axis">Axis to rotate around</param>
		/// <param name="degrees">Amount to rotate in degrees</param>
		/// <returns>Rotation matrix for the specified axis</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Matrix4 GetRotationMatrix(Axis axis, float degrees)
		{
			float radians = MathHelper.DegreesToRadians(degrees);
			return axis switch
			{
				Axis.XAxis => Matrix4.CreateRotationX(radians),
				Axis.YAxis => Matrix4.CreateRotationY(radians),
				Axis.ZAxis => Matrix4.CreateRotationZ(radians),
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		#endregion
	}
}
