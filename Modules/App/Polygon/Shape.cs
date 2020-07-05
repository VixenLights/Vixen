using System;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains properties of a shape.
	/// </summary>
	[Serializable]
	public class Shape 
	{
		#region Constructor 
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Shape()
		{
			// Initialize the unique ID of the shape
			ID = Guid.NewGuid();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the unique ID of the shape.
		/// </summary>
		public Guid ID { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Copies the specified source shape's properties into this destination shape.
		/// </summary>		
		public virtual void Copy(Shape sourceShape)
		{
			// Copy the ID of the source shape
			ID = sourceShape.ID;
		}

		#endregion
	}
}
