using System;
using System.Collections.Generic;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Defines the fill type of the shape.
	/// </summary>
	public enum PolygonFillType
	{
		Wipe,
		Solid,
		Outline,
	};

	/// <summary>
	/// Maintains a point based shape.
	/// </summary>
	[Serializable]
	public class PointBasedShape : Shape
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PointBasedShape()
		{
			// Create the collection of points
			Points = new List<PolygonPoint>();

			// Default the fill type to a Wipe
			FillType = PolygonFillType.Wipe;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the collection of points that make up the shape.
		/// </summary>
		public IList<PolygonPoint> Points
		{
			get;
			private set;
		}

		/// <summary>
		/// Determines how the shape is filled(Wipe, Solid or Outline).
		/// </summary>
		public PolygonFillType FillType { get; set; }
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Makes a copy of the specified source shape.
		/// </summary>		
		public override void Copy(Shape sourceShape)
		{
			// Call the base class implementation
			base.Copy(sourceShape);

			// Copy the shape fill type
			FillType = ((PointBasedShape)sourceShape).FillType;

			// Clear the existing points on the destination
			Points.Clear();

			// Loop over the points on the source shape
			foreach (PolygonPoint pt in ((PointBasedShape)sourceShape).Points)
			{
				// Create a new point
				PolygonPoint newPoint = new PolygonPoint();
				newPoint.X = pt.X;
				newPoint.Y = pt.Y;

				// Add the point to the destination
				Points.Add(newPoint);
			}
		}

		/// <summary>
		/// Limts the shape points to the specified width and height.
		/// </summary>		
		public virtual void LimitPoints(int width, int height)
		{
			// Loop over the shape's points
			foreach (PolygonPoint point in Points)
			{
				// If the X coordinate exceeds the width then...
				if (point.X > width - 1)
				{
					// Limit the X coordinate
					point.X = width - 1;
				}

				// If the Y coordinate exceeds the height then...
				if (point.Y > height - 1)
				{
					// Limit the Y coordinate
					point.Y = height - 1;
				}
			}
		}

		/// <summary>
		/// Moves the shape's points by the specified x and y offset.
		/// </summary>		
		public void MovePoints(int xOffset, int yOffset)
		{
			// Loop over the shape's points
			foreach (PolygonPoint pt in Points)
			{
				// Offset the point
				pt.X += xOffset;
				pt.Y += yOffset;
			}
		}

		/// <summary>
		/// Scales the points on the shape by the specified scale factors.
		/// </summary>
		/// <param name="xScaleFactor">X axis scale factor</param>
		/// <param name="yScaleFactor">Y axis scale factor</param>
		public void ScalePoints(double xScaleFactor, double yScaleFactor)
		{			
			foreach (PolygonPoint pt in Points)
			{
				// Scale the points for the editor canvas size
				pt.X = pt.X * xScaleFactor;
				pt.Y = pt.Y * yScaleFactor;
			}
		}

		/// <summary>
		/// Rounds the points to the nearest integer;
		/// </summary>
		public virtual void RoundPoints()
		{
			foreach (PolygonPoint pt in Points)
			{
				// Scale the points for the editor canvas size
				pt.X = Math.Round(pt.X);
				pt.Y = Math.Round(pt.Y);
			}
		}

		#endregion
	}
}
