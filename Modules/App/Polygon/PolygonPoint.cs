using System;
using System.Runtime.Serialization;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains a shape point.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PolygonPoint
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the X position of the point.
		/// </summary>
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y position of the point.
		/// </summary>
		[DataMember]
		public double Y { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Clones the shape point.
		/// </summary>
		/// <returns></returns>
		public PolygonPoint Clone()
		{
			// Create a new point
			PolygonPoint clone = new PolygonPoint();
			
			// Clone the coordinates
			clone.X = X;
			clone.Y = Y;

			// Return the cloned point
			return clone;
		}
		#endregion
	}
}
