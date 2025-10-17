namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Defines the coordinate system axis.
	/// </summary>
	public enum Axis
	{
		XAxis,
		YAxis, 
		ZAxis	
	};

	/// <summary>
	/// Maintains a rotation around an axis.
	/// </summary>
	public class AxisRotationModel
	{
		/// <summary>
		/// Axis to rotate about.
		/// </summary>
		public Axis Axis { get; set; }

		/// <summary>
		/// Rotation angle in degrees.
		/// </summary>
		public int RotationAngle { get; set; }

		/// <summary>
		/// Converts from axis string to enumeration.
		/// </summary>
		/// <param name="axis">String to convert</param>
		/// <returns>Equivalent enumeration of the string</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ConvertAxis(string axis)
		{
			Axis = axis switch
			{
				"X" => Axis.XAxis,
				"Y" => Axis.YAxis,
				"Z" => Axis.ZAxis,
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		public override string ToString()
		{
			return $"{RotationAngle}";
		}
	}
}
