using System.Drawing;

using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains properties and methods of a Whirl.
	/// </summary>
	public partial class Whirl : ExpandoObjectBase, IWhirl
	{
		#region Private Methods

		/// <summary>
		/// Draws the left side of the symmetrical whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void DrawLeftSideSymmetryWhirl(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			double intervalPos)
		{
			DrawSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos,
				height,
				WhirlpoolSideType.LeftSide);
		}

		/// <summary>
		/// Draws the bottom side of the symmetrical whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void DrawBottomSideSymmetryWhirl(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int thickness,
			double intervalPos)
		{
			DrawSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos,
				width,
				WhirlpoolSideType.BottomSide);
		}

		/// <summary>
		/// Draws the right side of the symmetrical whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void DrawRightSideSymmetryWhirl(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			double intervalPos)
		{
			DrawSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos,
				height,
				WhirlpoolSideType.RightSide);
		}

		/// <summary>
		/// Draws the top side of the symmetrical whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void DrawTopSideSymmetryWhirl(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int thickness,
			double intervalPos)
		{
			DrawSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos,
				width,
				WhirlpoolSideType.TopSide);
		}

		/// <summary>
		/// Draws a side of the symmetrical whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		/// <param name="length">Length of the leg</param>
		/// <param name="side">Side that being drawn</param>
		private void DrawSideSymmetryWhirl(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int thickness,
			double intervalPos,
			int length,
			WhirlpoolSideType side)
		{
			// Safe off the color index
			int colorIndex = _colorIndex;

			// Save off the pixel per band counter
			int bandPixelCount = _bandPixelCount;

			int nextColorIndex = 0;
			int nextBandPixelCount = 0;

			// Loop over the thickness of the whirl
			for (int index = 0; index < thickness; index++)
			{
				// With each row of pixels that makes up the thickness we need to reset
				// the color band the position within the band
				_colorIndex = colorIndex;
				_bandPixelCount = bandPixelCount;

				// Since the rows of pixel get shorter with each row
				// We need to adjust the color indices
				for (int cIndex = 0; cIndex < index; cIndex++)
				{
					// Increment the color indices
					UpdateSelectedColorBand();
				}

				// Loop over the length of the side
				for (int pixel = index; pixel < length - index - 1; pixel++)
				{
					// Determine the color of the pixel
					Color color = GetColor(side, intervalPos, index, 0);
					
					// Get the position of the pixel
					(int XPos, int YPos) position = GetSymmetricalPixelPosition(side, x, y, pixel, index, width);

					// Draw a pixel on the side
					SetPixel(frameBuffer, position.XPos, position.YPos, color,  0);
				}

				// At the end of first row we need to save off the color indices because these
				// are the values that need to be assigned when we exit the method
				if (index == 0)
				{
					// Save off the color indices so we exit the method on the correct color
					nextColorIndex = _colorIndex;
					nextBandPixelCount = _bandPixelCount;
				}
			}

			// Return the color indices as they were at the end of the first row
			_colorIndex = nextColorIndex;
			_bandPixelCount = nextBandPixelCount;
		}

		/// <summary>
		/// Determines the position of the next symmetrical side pixel.
		/// </summary>
		/// <param name="side">Side being drawn</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="pixel">Current whirl pixel being drawn</param>
		/// <param name="index">Row of the whirl's thickness being drawn</param>
		/// <param name="width">Width of the whirl</param>
		/// <returns>Position of the next whirl pixel</returns>
		private (int XPos, int YPos) GetSymmetricalPixelPosition(WhirlpoolSideType side, int x, int y, int pixel, int index, int width)
		{
			// Initialize the pixel position
			int xPos = 0;
			int yPos = 0;

			// Determine the position of the pixel based on the side being drawn
			switch (side)
			{
				case WhirlpoolSideType.BottomSide:
					xPos = x + pixel;
					yPos = y + index;
					break;
				case WhirlpoolSideType.TopSide:
					xPos = Width - 1 - x - pixel;
					yPos = Height - 1 - y - index;
					break;
				case WhirlpoolSideType.LeftSide:
					xPos = x + index;
					yPos = Height - 1 - (y + pixel);
					break;
				case WhirlpoolSideType.RightSide:
					xPos = x + width - 1 - index;
					yPos = y + pixel;
					break;
			}

			return (xPos, yPos);
		}

		/// <summary>
		/// Draws symmetrical rectangles emanating out.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="levels">Rectangle level counter</param>
		private void DrawWhirlSymmetricalOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int spacing,
			double intervalPos,
			int levels)
		{
			DrawLeftSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				intervalPos);

			DrawBottomSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos);

			DrawRightSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				intervalPos);

			DrawTopSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos);

			// Increase the drawing area by the thickness and spacing
			width += 2 * (thickness + spacing);
			height += 2 * (thickness + spacing);

			// If there are more rectangles to draw then...
			if (width <= Width && height <= Height && levels > 0)
			{
				// Update the color of the next whirl rectangle
				UpdateColor();
				
				// Keep track we have drawn another whirl
				levels = levels - 1;

				// Recursively draw another rectangular whirl
				DrawWhirlSymmetricalOut(
					frameBuffer,
					x - thickness - spacing,
					y - thickness - spacing,
					width,
					height,
					thickness,
					spacing,
					intervalPos,
					levels);
			}
		}

		/// <summary>
		/// Draws symmetrical rectangles moving in.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="levels">Rectangle level counter</param>
		private void DrawWhirlSymmetricalIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int spacing,
			double intervalPos,
			int levels)
		{
			DrawLeftSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				intervalPos);

			DrawBottomSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos);

			DrawRightSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				intervalPos);

			DrawTopSideSymmetryWhirl(
				frameBuffer,
				x,
				y,
				width,
				thickness,
				intervalPos);

			// Decrease the drawing area by the thickness and spacing
			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			// If there more rectangle to draw then...
			if (width >= thickness && height >= thickness && levels > 0)
			{
				// Update the color of the next whirl rectangle
				UpdateColor();

				// Keep track we have drawn another whirl
				levels = levels - 1;

				// Recursively draw another rectangular whirl
				DrawWhirlSymmetricalIn(
					frameBuffer,
					x + thickness + spacing,
					y + thickness + spacing,
					width,
					height,
					thickness,
					spacing,
					intervalPos,
					levels);
			}
		}

		#endregion
	}
}
	
