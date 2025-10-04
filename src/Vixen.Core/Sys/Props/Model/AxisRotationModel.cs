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
	}
}
