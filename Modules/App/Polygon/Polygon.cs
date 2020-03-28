using System;
using System.Collections.Generic;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Defines a polygon.
	/// </summary>
	[Serializable]
	public class Polygon
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Polygon()
		{
			Points = new List<PolygonPoint>();
		}

		#endregion

		#region Public Properties

		public List<PolygonPoint> Points
		{
			get;
			private set;
		}

		#endregion
	}	
}
