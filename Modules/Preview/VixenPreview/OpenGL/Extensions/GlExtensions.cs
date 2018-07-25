using System;
using OpenTK;

namespace VixenModules.Preview.VixenPreview.OpenGL.Extensions
{
	public static class GlExtensions
	{
		/// <summary>
		/// Creates an orientation Quaternion given the 3 axis.
		/// </summary>
		/// <param name="Axis">An array of 3 axis.</param>
		public static Quaternion FromAxis(Vector3 xvec, Vector3 yvec, Vector3 zvec)
		{
			Matrix4 Rotation = new Matrix4(
				new Vector4(xvec.X, yvec.X, zvec.X, 0),
				new Vector4(xvec.Y, yvec.Y, zvec.Y, 0),
				new Vector4(xvec.Z, yvec.Z, zvec.Z, 0),
				Vector4.Zero);
			return FromRotationMatrix(Rotation);
		}

		public static void SetAt(this Quaternion q, int a, float value)
		{
			if (a == 0) q.X = value; else if (a == 1) q.Y = value; else if (a == 2) q.Z = value; else q.W = value;
		}

		private static readonly int[] RotationLookup = new int[] { 1, 2, 0 };

		/// <summary>
		/// Creates an orientation Quaternion from a target Matrix4 rotational matrix.
		/// </summary>
		public static Quaternion FromRotationMatrix(Matrix4 Rotation)
		{
			// Algorithm from Ken Shoemake's article in 1987 SIGGRAPH course notes
			// "Quaternion Calculus and Fast Animation"

			float t_trace = Rotation.Row0.X + Rotation.Row1.Y + Rotation.Row2.Z;
			float t_root = 0.0f;

			if (t_trace > 0.0)
			{   // |w| > 1/2
				Quaternion t_return = new Quaternion(0, 0, 0, 0);
				t_root = (float)Math.Sqrt(t_trace + 1.0);
				t_return.W = 0.5f * t_root;
				t_root = 0.5f / t_root;
				t_return.X = (Rotation.Row2.Y - Rotation.Row1.Z) * t_root;
				t_return.Y = (Rotation.Row0.Z - Rotation.Row2.X) * t_root;
				t_return.Z = (Rotation.Row1.X - Rotation.Row0.Y) * t_root;
				return t_return;
			}
			else
			{   // |w| <= 1/2
				Quaternion t_return = new Quaternion(0, 0, 0, 0);

				int i = 0;
				if (Rotation.Row1.Y > Rotation.Row0.X) i = 1;
				if (Rotation.Row2.Z > Rotation[i,i]) i = 2;
				int j = RotationLookup[i];
				int k = RotationLookup[j];

				t_root = (float)Math.Sqrt(Rotation[i,i] - Rotation[j,j] - Rotation[k,k] + 1.0f);
				t_return.SetAt(i, 0.5f * t_root); 
				t_root = 0.5f / t_root;
				t_return.W = (Rotation[k,j] - Rotation[j,k]) * t_root;
				t_return.SetAt(j, (Rotation[j,i] + Rotation[i,j]) * t_root);
				t_return.SetAt(k,(Rotation[k,i] + Rotation[i,k]) * t_root);
				return t_return;
			}
		}
	}
}
