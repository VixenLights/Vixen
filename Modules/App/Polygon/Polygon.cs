using System;
using System.Runtime.Serialization;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains a polygon.
	/// </summary>
	[DataContract]
	[Serializable]
	public class Polygon : PointBasedShape
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Polygon()
		{						
		}

		#endregion

		#region Public Methods
		
		/// <summary>
		/// Clones the polygon.
		/// </summary>		
		public Polygon Clone()
		{
			// Make a copy the polygon 
			Polygon copy = new Polygon();
			copy.Copy(this);
			
			// Since it is a clone we should give it a unique ID
			copy.ID = Guid.NewGuid();

			return copy;
		}

		#endregion
	}
}
