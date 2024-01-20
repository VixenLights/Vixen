using Common.Controls.ColorManagement.ColorModels;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Shockwave
{
	/// <summary>
	/// This file contains the diamond specific routines of the shock-wave effect.
	/// </summary>
	public partial class Shockwave
	{
		#region Private Location Methods

		/// <summary>
		/// Returns true if the specified location point is within the diamond.
		/// </summary>
		/// <param name="x">X position of the pixel</param>
		/// <param name="y">Y position of the pixel</param>
		/// <param name="outerLength">Length the diamond from top to bottom on the Y-axis</param>
		/// <param name="centerX">Center of the diamond in the X axis</param>
		/// <param name="centerY">Center of the diamond in the Y axis</param>
		/// <param name="width">Thickness of the diamond</param>
		/// <param name="intensity">Intensity of the point if blending is applicable</param>
		/// <returns>True if the point is within the diamond</returns>
		/// <remarks>In locations mode the Y coordinates increase as you move down the screen or prop.</remarks>
		private bool ContainsPoint(int x, int y, double outerLength, int centerX, int centerY, int width, out double intensity)
		{  
			// Default to full intensity
			intensity = 1.0;

			// Determine where the diamond should start
			int yStart = (int)(centerY - outerLength / 2);

			// Default to the point not being part of the diamond
			bool containsPoint = false;

			// If the Y coordinate is within the diamond then...
			if (y >= yStart && y < yStart + outerLength)
			{
				// Calculate the row width
				int rowWidth = CalculateRowWidth(yStart, y, centerY);

				// If the thickness of the diamond is greater than the row width then...
				if (width > rowWidth)
				{
					// Set the width to the row width
					width = rowWidth;
				}

				// If the x coordinate is on the left side of the diamond edge OR
				// the x coordinate is on the right side of the diamond then...
				if ((x > centerX - rowWidth / 2 && x < centerX - rowWidth / 2 + width) ||
					(x > centerX + rowWidth / 2 - width && x < centerX + rowWidth / 2))
				{
					// Indicate that the point is contained within the diamond
					containsPoint = true;

					// Calculate the pixel intensity of the point
					intensity = CalculateIntensity(centerX, x, width, rowWidth);
				}
			}

			return containsPoint;
		}

		/// <summary>
		/// Calculates the intensity of the location based pixel.
		/// </summary>
		/// <param name="centerX">Center of the diamond</param>
		/// <param name="x">X location of the pixel</param>
		/// <param name="width">Thickness of the diamond edge</param>
		/// <param name="rowWidth">Width of the row</param>
		/// <returns>Intensity of the pixel (0-1)</returns>
		private double CalculateIntensity(int centerX, int x, int width, int rowWidth)
		{
			// Declare the return value
			double intensity; 

			// If the pixel is to the right of center then...
			if (x >= centerX)
			{
				// Calculate the center of the right edge of the diamond
				double center = centerX + rowWidth / 2 - width / 2;

				// Calculate the intensity of the pixel based on the distance from the center of the diamond edge
				intensity = CalculateIntensityBasedOnDistanceFromCenter(center, x, width);
			}
			// Otherwise the pixel is to the left
			else
			{
				// Calculate the center of the left edge of the diamond
				double center = centerX - rowWidth / 2 + width / 2;

				// Calculate the intensity of the pixel based on the distance from the center of the diamond edge
				intensity = CalculateIntensityBasedOnDistanceFromCenter(center, x, width);
			}

			return intensity;
		}

		/// <summary>
		/// Calculates the intensity of the location based pixel based on the X axis location.
		/// </summary>
		/// <param name="centerX">Center of the diamond</param>
		/// <param name="x">X location of the pixel</param>
		/// <param name="width">Thickness of the diamond edge</param>
		/// <returns>Intensity of the pixel (0-1)</returns>
		private double CalculateIntensityBasedOnDistanceFromCenter(double center, int x, int width)
		{
			// Calculate the distance from the center of the diamond edge
			double distanceAwayFromCenter = Math.Abs((double)x - center);
			
			// If the distance is greater than the width then...
			if (distanceAwayFromCenter > width)
			{
				// Cap the distance at the width
				distanceAwayFromCenter = width;
			}

			// Calculate the intensity of the pixel
			return 1.0 - distanceAwayFromCenter / width;
		}

		/// <summary>
		/// Calculate the length of the specified row of the diamond.
		/// </summary>
		/// <param name="yStart">The Y location coordinate of where the diamond starts</param>
		/// <param name="y">Y location of candidate pixel</param>
		/// <param name="centerY">Center of the diamond</param>
		/// <returns>The length of the row for the specified Y coordinate</returns>
		private int CalculateRowWidth(int yStart, int y, int centerY)
		{
			// Default the row width to one pixel
			int rowWidth = 1;

			// Loop until reaching the specified Y coordinate
			for (int h = yStart; h <= y; h++)
			{
				// If the current row is less than the center Y coordinate then...
				if (h < centerY)
				{
					// Increase the row width by two pixels
					rowWidth += 2;
				}
				// Otherwise we are greater than the center Y coordinate
				else
				{
					// Decrease the row width by two pixels
					rowWidth -= 2;
				}
			}

			return rowWidth;
		}

		#endregion

		#region Private String Methods

		/// <summary>
		/// Draws the diamond shock-wave for string mode.
		/// </summary>
		/// <param name="effectPositionAdjust">Effect position taking into account acceleration</param>
		/// <param name="frameBuffer">Current frame buffer</param>
		/// <param name="posX">Center X position of the diamond</param>
		/// <param name="posY">Center Y position of the diamond</param>
		/// <param name="color">Color of the diamond</param>
		/// <param name="width">Thickness of the diamond edge</param>
		/// <param name="outerRadius">Outer radius of the diamond</param>
		private void DrawDiamondShockwave(
			double effectPositionAdjust,
			IPixelFrameBuffer frameBuffer,
			double posX,
			double posY,
			Color color,
			double width,
			double outerRadius)
		{
			// Calculate the diameter for the shock-wave
			double outsideDiameter = 2.0 * outerRadius;

			// Draw the diamond
			DrawDiamond(
				(int)outsideDiameter,
				(int)width,
				frameBuffer,
				(int)posX,
				(int)posY,
				color);
		}

		/// <summary>
		/// Draws the diamond shock-wave for string mode.
		/// </summary>
		/// <param name="outerLength">Outer length of the diamond</param>
		/// <param name="thickness">Thickness of the diamond edge</param>
		/// <param name="frameBuffer">Current frame buffer</param>
		/// <param name="centerX">Center X coordinate of the diamond</param>
		/// <param name="centerY">Center Y coordinate of the diamond</param>
		/// <param name="color">Color of the diamond</param>
		private void DrawDiamond(
		  int outerLength,
		  int thickness,
		  IPixelFrameBuffer frameBuffer,
		  int centerX,
		  int centerY,
		  Color color)
		{
			// Determine how much to increment the intensity
			double intensityIncrement = 1.0 / thickness;

			// Initialize the start to the center of the prop plus the length of the diamond
			int yStart = (int)(centerY + outerLength / 2);

			// Start the diamond with a thickness of 1 pixel
			int rowWidth = 1;

			// Loop over the height of the diamond.
			// This loop draws the diamond from the top down.
			for (int h = 0; h < outerLength; h++)
			{
				//
				// Draw the left side of the diamond
				//

				// Reset the 1st intensity to the first intensity increment
				double intensity = intensityIncrement;

				// Loop over the thickness of the diamond shock-wave edge
				for (int rw = 0; rw < thickness; rw++)
				{
					// Make sure we are only lighting up pixels on the left side
					if ((centerX - rowWidth / 2 + rw) <= centerX)
					{
						// Get color for the diamond pixel
						HSV hsv = GetColor(color, BlendEdges ? intensity : 1.0);

						// Set the color intensity on the diamond pixel
						frameBuffer.SetPixel(centerX - rowWidth / 2 + rw, yStart, hsv);
						
						// Update the intensity of the next diamond pixel
						intensity = UpdatePixelIntensity(rw, thickness, intensity, intensityIncrement);
					}
				}

				// 
				// Draw the right side of the diamond
				//

				// Reset the 1st intensity to the first intensity increment
				double intensity2 = intensityIncrement;

				// Loop over the thickness of the diamond shock-wave edge
				for (int r = 0; r < thickness; r++)
				{
					if (centerX + rowWidth / 2 - r > centerX)
					{
						// Get color for the diamond pixel
						HSV hsv = GetColor(color, BlendEdges ? intensity2 : 1.0);

						// Set the color intensity on the diamond
						frameBuffer.SetPixel(centerX + rowWidth / 2 - r, yStart, hsv);

						// Update the intensity of the next diamond pixel
						intensity2 = UpdatePixelIntensity(r, thickness, intensity2, intensityIncrement);
					}
				}

				// Update the width of the diamond row
				rowWidth = UpdateRowWidth(h, rowWidth, (int)outerLength);

				// Move to the next row in the diamond
				yStart -= 1;
			}
		}

		/// <summary>
		/// Gets the specified color using the specified intensity.
		/// </summary>
		/// <param name="color">Base color to apply the intensity too</param>
		/// <param name="intensity">Intensity to apply to the color</param>
		/// <returns>Color of the diamond</returns>
		private HSV GetColor(Color color, double intensity)
		{
			HSV hsv = HSV.FromRGB(color);
			hsv.V = hsv.V * intensity;

			return hsv;
		}

		/// <summary>
		/// Updates the row width for the specified Y position.
		/// </summary>
		/// <param name="y">Y position of the row</param>
		/// <param name="rowWidth">Previous row width</param>
		/// <param name="height">Total height of the diamond</param>
		/// <returns>Updated row width</returns>
		private int UpdateRowWidth(int y, int rowWidth, int height)
		{
			// If the Y coordinate is in the top half of the diamond then...
			if (y < height / 2)
			{
				// Increase the row width by two pixels
				rowWidth += 2;
			}
			// Otherwise the Y coordinate is in the lower half of the diamond
			else
			{
				// Decrease the row width by two pixels
				rowWidth -= 2;
			}

			return rowWidth;
		}

		/// <summary>
		/// Calculates the pixel intensity.
		/// </summary>
		/// <param name="x">X Position within the diamond edge</param>
		/// <param name="thickness">Thickness of the diamond edge</param>
		/// <param name="intensity">Previous intensity</param>
		/// <param name="intensityIncrement">Intensity increment</param>
		/// <returns></returns>
		private double UpdatePixelIntensity(int r, int thickness, double intensity, double intensityIncrement)
		{
			// If the x position is to the left of the center of the diamond edge then... 
			if (r <= thickness / 2)
			{
				// Increase the intensity
				intensity += intensityIncrement;
			}
			// Otherwise we are to the right of center of the diamond edge
			else
			{
				// Decrease the intensity
				intensity -= intensityIncrement;
			}

			return intensity;
		}

		#endregion
	}
}
