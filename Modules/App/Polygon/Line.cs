using System;
using System.Runtime.Serialization;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains a line.
	/// </summary>
	[DataContract]
	[Serializable]
	public class Line : PointBasedShape
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Line()
		{	
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clones the line.
		/// </summary>		
		public Line Clone()
		{
			// Make a copy the line
			Line clone = new Line();
			clone.Copy(this);

			// Since it is a clone we should give it a unique ID
			clone.ID = Guid.NewGuid();

			return clone;
		}

		#endregion
	}
}
