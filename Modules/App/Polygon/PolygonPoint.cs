using System;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains a polygon point.
	/// </summary>
	[Serializable]
	public class PolygonPoint
	{
		#region Public Properties
		
		/// <summary>
		/// Gets or sets the X position of the point.
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y position of the point.
		/// </summary>
		public double Y { get; set; }

		#endregion
	}
}
