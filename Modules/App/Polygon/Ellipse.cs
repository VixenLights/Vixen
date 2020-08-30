using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains an ellipse.
	/// </summary>
	[DataContract]
	public class Ellipse : PointBasedShape
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public Ellipse()
		{
			Center = new PolygonPoint();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clones the line.
		/// </summary>		
		public Ellipse Clone()
		{
			// Make a copy the ellipse
			Ellipse clone = new Ellipse();
			clone.Copy(this);

			// Since it is a clone we should give it a unique ID
			clone.ID = Guid.NewGuid();

			clone.Angle = Angle;
			clone.Width = Width;
			clone.Height = Height;
			clone.Center.X = Center.X;
			clone.Center.Y = Center.Y;
			clone.StartSideRotation = StartSideRotation;

			return clone;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the angle of rotation of the ellipse.
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Gets the center of the ellipse.
		/// </summary>
		public PolygonPoint Center { get; set; }

		/// <summary>
		/// Gets or sets the width of the ellipse.
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Gets or sets the height of the ellipse.
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		/// Gets or sets the start side of the ellipse.
		/// </summary>
		public int StartSideRotation { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// This method is overriden so that the rectangle that bounds the ellipse is distorted.
		/// Trying to keep the points a true rectangle, in other words the lines need to be at right angles.
		/// </summary>
		/// <param name="width">Width of the display element</param>
		/// <param name="height">Height of the display element</param>
		public override void LimitPoints(int width, int height)
		{
			// If the ellipse is outside the limits then...
			if (IsOutsideLimits(width, height))
			{
				// Figure how far the ellipse if off center from the display element
				double deltaX = width / 2.0 - Center.X;
				double deltaY = height / 2.0 - Center.Y;

				// Move all the points to be relative to the center of the display element
				MovePoints((int)deltaX, (int)deltaY);

				// Move the center of the ellipse to the center of the display element
				Center.X += deltaX;
				Center.Y += deltaY;
			}

			// Keep scaling the ellipse to make it smaller until it fits on the display element
			while (IsOutsideLimits(width, height))
			{
				const double scaleFactor = 0.99;
				ScalePoints(scaleFactor, scaleFactor);
			}
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void RoundPoints()
		{
			Debug.Assert(false, "Should not round ellipse points as it will distort the rectangle");

			base.RoundPoints();

			Center.X = Math.Round(Center.X);
			Center.Y = Math.Round(Center.Y);

			Width = Math.Round(Width);
			Height = Math.Round(Height);
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Scale(double xScaleFactor, double yScaleFactor)
		{
			// Create a reverse transform for the ellipse
			RotateTransform reverseRotateTransform = new RotateTransform(-Angle, Center.X, Center.Y);
				
			// Rotate the points that make up the rectangle that surrounds the ellipse
			// such that the ellipse is no longer rotated
			foreach (PolygonPoint pt in Points)
			{
				// Transform the point
				Point transformedPoint = reverseRotateTransform.Transform(new Point(pt.X, pt.Y));

				// Update the point
				pt.X = transformedPoint.X;
				pt.Y = transformedPoint.Y;
			}

			// Call the base class implementation to scale the points
			base.Scale(xScaleFactor, yScaleFactor);

			// Scale the center for the new display element size
			Center.X = Center.X * xScaleFactor;
			Center.Y = Center.Y * yScaleFactor;

			// Scale the width of the ellipse
			Width = Width * xScaleFactor;

			// Scale the height of the ellipse
			Height = Height * yScaleFactor;

			// Create the transform to rotate the ellipse back to the original angle
			RotateTransform rotateTransform = new RotateTransform(Angle, Center.X, Center.Y);

			// Rotate the points that make up the rectangle that surrounds the ellipse
			// such that the ellipse is no longer rotated
			foreach (PolygonPoint pt in Points)
			{
				// Transform the point
				Point transformedPoint = rotateTransform.Transform(new Point(pt.X, pt.Y));

				// Update the point
				pt.X = transformedPoint.X;
				pt.Y = transformedPoint.Y;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns true if the ellipse is outside the limits of the display element.
		/// </summary>
		/// <param name="width">Width of the display element</param>
		/// <param name="height">Height of the display element</param>
		/// <returns>true if the ellipse is outside the limits</returns>
		private bool IsOutsideLimits(int width, int height)
		{
			// Default to being inside the limits
			bool outsideLimits = false;

			// Loop over the ellipse's points
			foreach (PolygonPoint point in Points)
			{
				// If the X or Y coordinate exceeds the limits then...
				if (point.X > width - 1 ||
				    point.Y > height - 1)
				{
					// Remember the shape is outside the limits
					outsideLimits = true;

					break;
				}
			}

			return outsideLimits;
		}

		#endregion
	}
}
