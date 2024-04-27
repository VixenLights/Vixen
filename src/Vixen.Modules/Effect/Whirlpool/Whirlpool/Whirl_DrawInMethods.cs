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
		/// Draws a continuous whirl starting on the outside of the matrix and spiraling in with smaller whirls.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		/// <param name="length1">Width or height of the draw area</param>
		/// <param name="length2">Width or height of the draw area</param>
		/// <param name="drawMethod1">Draws side #1</param>
		/// <param name="drawMethod2">Draws side #2</param>
		/// <param name="drawMethod3">Draws side #3</param>
		/// <param name="drawMethod4">Draws side #4</param>
		/// <param name="offset">Offset between whirls.  This is usually a +/- (Spacing + Thickness)</param>
		/// <param name="firstPass">Whether this is the first whirl</param>
		private void DrawIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos,
			int length1,
			int length2,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod1,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod2,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod3,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod4,
			int offset,
			bool firstPass)
		{
			int length;
			int lengthOffset;
			
			// If this is the start of the first whirl then...
			if (firstPass)
			{
				length = length1 - 1;

				// If this is the first pass then we don't need to draw a transition from the previous
				// whirl rectangle to the current smaller rectangle
				lengthOffset = 0;
			}
			else
			{
				// First leg of the whirl needs to be drawn between the previous whirl and the current whirl
				// This space is the thickness plus the spacing.
				length = length1 + thickness + spacing - 1;
				
				// Initialize the offset to transition from the previous rectangle to the current
				// smaller rectangle
				lengthOffset = offset;
			}

			// Draw the first leg of the whirl
			drawMethod1(
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				Math.Min(CalcNumPixels(length, numberOfPixels), numberOfPixels),
				length,
				intervalPos,
				lengthOffset,
				firstPass,
				numberOfPixels);

			// Subtract off the leg pixels 
			numberOfPixels -= Math.Min(CalcNumPixels(length, numberOfPixels), numberOfPixels);

			// If there are more pixels to draw then...
			if (numberOfPixels > 0)
			{
				// Draw the second leg of the whirl
				drawMethod2(
					frameBuffer,
					x,
					y,
					width,
					height,
					thickness,
					Math.Min(CalcNumPixels(length2 - 1, numberOfPixels), numberOfPixels),
					length2 - 1,
					intervalPos,
					0,
					false,
					numberOfPixels);

				// Subtract off the leg pixels 
				numberOfPixels -= Math.Min(CalcNumPixels(length2 - 1, numberOfPixels), numberOfPixels);

				// If there are more pixels to draw then...
				if (numberOfPixels > 0)
				{
					// Draw the third leg of the whirl
					drawMethod3(
						frameBuffer,
						x,
						y,
						width,
						height,
						thickness,
						Math.Min(CalcNumPixels(length1 - 1, numberOfPixels), numberOfPixels),
						length1 - 1,
						intervalPos,
						0, 
						false,
						numberOfPixels);
					numberOfPixels -= Math.Min(CalcNumPixels(length1 - 1, numberOfPixels), numberOfPixels);

					// If there are more pixels to draw then...
					if (numberOfPixels > 0)
					{
						// Draw the fourth leg of the whirl
						drawMethod4(
							frameBuffer,
							x,
							y,
							width,
							height,
							thickness,
							Math.Min(CalcNumPixels(length2 - thickness - spacing - 1, numberOfPixels), numberOfPixels),
							length2 - thickness - spacing - 1,
							intervalPos,
							0, 
							false,
							numberOfPixels);

						// Subtract off the leg pixels 
						numberOfPixels -= Math.Min(CalcNumPixels(length2 - thickness - spacing - 1, numberOfPixels), numberOfPixels);

						// Reduce the drawing area by the thickness and spacing on all sides
						width -= 2 * (thickness + spacing);
						height -= 2 * (thickness + spacing);
						length1 -= 2 * (thickness + spacing);
						length2 -= 2 * (thickness + spacing);	

						// If both the width and height are at least the thickness of the whirl then...
						if (width >= thickness && height >= thickness)
						{
							// Update the color of the whirl
							UpdateColor();

							// Draw another whirl
							DrawIn(
								frameBuffer,
								x + thickness + spacing,
								y + thickness + spacing,
								width,
								height,
								numberOfPixels,
								thickness,
								spacing,
								intervalPos,
								length1,
								length2,
								drawMethod1,
								drawMethod2,
								drawMethod3,
								drawMethod4,
								offset,
								false);
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws a clockwise whirl starting in the upper left hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopLeftClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				width,
				height,
				DrawTopSideClockwise,
				DrawRightSideClockwise,
				DrawBottomSideClockwise,
				DrawLeftSideClockwise,
				-thickness - spacing,
				true);
		}

		/// <summary>
		/// Draws a clockwise whirl starting in the bottom left hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomLeftClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				height,
				width,
				DrawLeftSideClockwise,
				DrawTopSideClockwise,
				DrawRightSideClockwise,
				DrawBottomSideClockwise,
				-thickness - spacing,
				true);
		}

		/// <summary>
		/// Draws a clockwise whirl starting in the bottom right hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomRightCounterClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				height,
				width,
				DrawRightSideCounterClockwise,
				DrawTopSideCounterClockwise,
				DrawLeftSideCounterClockwise,
				DrawBottomSideCounterClockwise,
				-thickness - spacing,
				true);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl starting in the bottom left hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomLeftCounterClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				width,
				height,
				DrawBottomSideCounterClockwise,
				DrawRightSideCounterClockwise,
				DrawTopSideCounterClockwise,
				DrawLeftSideCounterClockwise,
				-thickness - spacing,
				true);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl starting in the bottom right hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomRightClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				width,
				height,
				DrawBottomSideClockwise,
				DrawLeftSideClockwise,
				DrawTopSideClockwise,
				DrawRightSideClockwise,
				thickness + spacing,
				true);

		}

		/// <summary>
		/// Draws a counter-clockwise whirl starting in the top left hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopLeftCounterClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				height,
				width,
				DrawLeftSideCounterClockwise,
				DrawBottomSideCounterClockwise,
				DrawRightSideCounterClockwise,
				DrawTopSideCounterClockwise,
				-thickness - spacing,
				true);
		}

		/// <summary>
		/// Draws a clockwise whirl starting in the top right hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopRightClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				height,
				width,
				DrawRightSideClockwise,
				DrawBottomSideClockwise,
				DrawLeftSideClockwise,
				DrawTopSideClockwise, thickness + spacing,
				true);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl starting in the top right hand corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopRightCounterClockwiseIn(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			double intervalPos)
		{
			DrawIn(
				frameBuffer,
				x,
				y,
				width,
				height,
				numberOfPixels,
				thickness,
				spacing,
				intervalPos,
				width,
				height,
				DrawTopSideCounterClockwise,
				DrawLeftSideCounterClockwise,
				DrawBottomSideCounterClockwise,
				DrawRightSideCounterClockwise,
				thickness + spacing,
				true);
		}

		#endregion
	}
}


