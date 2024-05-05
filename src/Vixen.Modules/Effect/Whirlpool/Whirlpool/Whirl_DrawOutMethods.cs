using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains properties and methods of a Whirl.
	/// </summary>
	public partial class Whirl : ExpandoObjectBase, IWhirl
	{
		/// <summary>
		/// Draws a continuous whirl starting the center and spiraling out.
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
		private void DrawOut(
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
			bool firstPass,
			bool flag1,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod1,
			bool flag2,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod2,
			bool flag3,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod3,
			bool flag4,
			Action<IPixelFrameBuffer, int, int, int, int, int, int, int, double, int, bool, int> drawMethod4,
			int offset)
		{
			// If drawing the first leg then...
			if (flag1)
			{
				// Draw the first leg of the whirl
				drawMethod1(
					frameBuffer,
					x,
					y,
					width,
					height,
					thickness,
					Math.Min(length1 - thickness - spacing - 1, numberOfPixels),
					length1 - thickness - spacing - 1,
					intervalPos,
					offset,
					firstPass,
					numberOfPixels);
				numberOfPixels -= Math.Min(length1 - thickness - spacing - 1, numberOfPixels);
			}

			// If there are more pixels to draw then...
			if (numberOfPixels > 0)
			{
				// If drawing the second leg then...
				if (flag2)
				{
					// Draw the second leg of the whirl
					drawMethod2(
						frameBuffer,
						x,
						y,
						width,
						height,
						thickness,
						Math.Min(length2 - 1, numberOfPixels),
						length2 - 1,
						intervalPos,
						0,
						firstPass && !flag1,
						numberOfPixels);
					numberOfPixels -= Math.Min(length2 - 1, numberOfPixels);
				}

				// If there are more pixels to draw then...
				if (numberOfPixels > 0)
				{
					// If drawing the third leg then...
					if (flag3)
					{
						// Draw the third leg of the whirl
						drawMethod3(
							frameBuffer,
							x,
							y,
							width,
							height,
							thickness,
							Math.Min(length1 - 1, numberOfPixels),
							length1 - 1,
							intervalPos,
							0,
							firstPass && !flag1 && !flag2,
							numberOfPixels);
						numberOfPixels -= Math.Min(length1 - 1, numberOfPixels);
					}

					// If there are more pixels to draw then...
					if (numberOfPixels > 0)
					{
						// If drawing the fourth leg then...
						if (flag4)
						{
							// Draw the fourth leg of the whirl
							drawMethod4(
								frameBuffer,
								x,
								y,
								width,
								height,
								thickness,
								Math.Min(numberOfPixels, length2 + thickness + spacing - 1),
								length2 + thickness + spacing - 1,
								intervalPos,
								0,
								firstPass && !flag1 && !flag2 && !flag3,
								numberOfPixels);
							numberOfPixels -= Math.Min(numberOfPixels, length2 + thickness + spacing - 1);
						}
					}

					// If there are more pixels to draw then...
					if (numberOfPixels > 0)
					{
						// Increase the drawing area by the thickness + spacing on all sides
						width += 2 * (thickness + spacing);
						height += 2 * (thickness + spacing);
						length1 += 2 * (thickness + spacing);
						length2 += 2 * (thickness + spacing);

						// If still within the bounds of the maximum drawing area then....
						if (width <= Width && height <= Height)
						{
							// Update the color of the whirl
							UpdateColor();

							// Draw another whirl level
							DrawOut(
								frameBuffer,
								x - thickness - spacing,
								y - thickness - spacing,
								width,
								height,
								numberOfPixels,
								thickness,
								spacing,
								intervalPos,
								length1,
								length2,
								false,
								true,
								drawMethod1,
								true,
								drawMethod2,
								true,
								drawMethod3,
								true,
								drawMethod4,
								offset);
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws a counter-clockwise whirl ending in the bottom right corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomRightCounterClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawBottom,
				DrawBottomSideClockwise,
				drawLeft,
				DrawLeftSideClockwise,
				drawTop,
				DrawTopSideClockwise,
				drawRight,
				DrawRightSideClockwise,
				-spacing - thickness);
		}

		/// <summary>
		/// Draws a clockwise whirl ending in the top right corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopRightClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawTop,
				DrawTopSideCounterClockwise,
				drawLeft,
				DrawLeftSideCounterClockwise,
				drawBottom,
				DrawBottomSideCounterClockwise,
				drawRight,
				DrawRightSideCounterClockwise,
				-spacing - thickness);
		}

		/// <summary>
		/// Draws a clockwise whirl ending in the top left corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void DrawTopLeftClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawLeft,
				DrawLeftSideCounterClockwise,
				drawBottom,
				DrawBottomSideCounterClockwise,
				drawRight,
				DrawRightSideCounterClockwise,
				drawTop,
				DrawTopSideCounterClockwise,
				spacing + thickness);
		}

		/// <summary>
		/// Draws a clockwise whirl ending in the bottom left corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomLeftClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawBottom,
				DrawBottomSideCounterClockwise,
				drawRight,
				DrawRightSideCounterClockwise,
				drawTop,
				DrawTopSideCounterClockwise,
				drawLeft,
				DrawLeftSideCounterClockwise,
				spacing + thickness);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl ending in the top right corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopRightCounterClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawRight,
				DrawRightSideClockwise,
				drawBottom,
				DrawBottomSideClockwise,
				drawLeft,
				DrawLeftSideClockwise,
				drawTop,
				DrawTopSideClockwise,
				-spacing - thickness);
		}

		/// <summary>
		/// Draws a clockwise whirl ending in the bottom right corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomRightClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawRight,
				DrawRightSideCounterClockwise,
				drawTop,
				DrawTopSideCounterClockwise,
				drawLeft,
				DrawLeftSideCounterClockwise,
				drawBottom,
				DrawBottomSideCounterClockwise,
				spacing + thickness);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl ending in the bottom left corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawBottomLeftCounterClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawLeft,
				DrawLeftSideClockwise,
				drawTop,
				DrawTopSideClockwise,
				drawRight,
				DrawRightSideClockwise,
				drawBottom,
				DrawBottomSideClockwise,
				spacing + thickness);
		}

		/// <summary>
		/// Draws a counter-clockwise whirl ending in the top left corner.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the whirl on</param>
		/// <param name="x">X position of the whirl</param>
		/// <param name="y">Y position of the whirl</param>
		/// <param name="width">Width of the whirl draw area</param>
		/// <param name="height">Height of the whirl draw area</param>
		/// <param name="numberOfPixels">Number of pixels to draw</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing of the whirl</param>
		/// <param name="drawTop">Flag to determine whether to draw the top leg</param>
		/// <param name="drawRight">Flag to determine whether to draw the right leg</param>
		/// <param name="drawBottom">Flag to determine whether to draw the bottom leg</param>
		/// <param name="drawLeft">Flag to determine whether to draw the left leg</param>
		/// <param name="intervalPos">Position within the effect</param>
		private void DrawTopLeftCounterClockwiseOut(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int numberOfPixels,
			int thickness,
			int spacing,
			bool drawTop,
			bool drawRight,
			bool drawBottom,
			bool drawLeft,
			double intervalPos)
		{
			DrawOut(
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
				true,
				drawTop,
				DrawTopSideClockwise,
				drawRight,
				DrawRightSideClockwise,
				drawBottom,
				DrawBottomSideClockwise,
				drawLeft,
				DrawLeftSideClockwise,
				spacing + thickness);
		}
	}
}
