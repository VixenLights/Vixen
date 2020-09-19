using System;
using System.Runtime.Serialization;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains properties of a shape.
	/// </summary>
	[DataContract]
	[Serializable]
	public abstract class Shape 
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
		[DataMember]
		public Guid ID { get; set; }

		/// <summary>
		/// Gets or sets the label of the shape.
		/// </summary>
		[DataMember]
		public string Label { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Copies the specified source shape's properties into this destination shape.
		/// </summary>		
		public virtual void Copy(Shape sourceShape)
		{
			// Copy the ID of the source shape
			ID = sourceShape.ID;

			// Copy the label of the shape
			Label = sourceShape.Label;
		}

		/// <summary>
		/// Scales the shape using the specified scale factors.
		/// </summary>
		/// <param name="xScaleFactor">X-axis scale factor</param>
		/// <param name="yScaleFactor">Y-axis scale factor</param>
		/// <param name="width">Width of display element</param>
		/// <param name="height">Height of display element</param>
		public abstract void Scale(double xScaleFactor, double yScaleFactor, int width, int height);

		#endregion
	}
}
