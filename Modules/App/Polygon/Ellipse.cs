using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains an ellipse.
	/// </summary>
	[DataContract]
	[Serializable]
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
		[DataMember]
		public double Angle { get; set; }

		/// <summary>
		/// Gets the center of the ellipse.
		/// </summary>
		[DataMember]
		public PolygonPoint Center { get; set; }

		/// <summary>
		/// Gets or sets the width of the ellipse.
		/// </summary>
		[DataMember]
		public double Width { get; set; }

		/// <summary>
		/// Gets or sets the height of the ellipse.
		/// </summary>
		[DataMember]
		public double Height { get; set; }

		/// <summary>
		/// Gets or sets the start side of the ellipse.
		/// </summary>
		[DataMember]
		public int StartSideRotation { get; set; }

		#endregion

		#region Public Methods

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
		public override void Scale(double xScaleFactor, double yScaleFactor, int width, int height)
		{
			// Scale the ellipse using the specified scale factor
			ScaleInternal(xScaleFactor, yScaleFactor, width, height);

			// Because of rotation it is possible that the rectangle surrounding the ellipse
			// might be outside the limits.
			// Keep scaling down by one pixel until the ellipse fits on the display element
			while (IsOutsideLimits(width, height))
			{
				// Scale the ellipse down by 1 pixel in both the x and y axis
				ScaleInternal((width - 1.0) / width, (height - 1) / height, width, height);
			}
		}

		private void ScaleInternal(double xScaleFactor, double yScaleFactor, int width, int height)
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
			base.Scale(xScaleFactor, yScaleFactor, width, height);

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
				    point.Y > height - 1 ||
					point.X < 0 ||
				    point.Y < 0)
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
