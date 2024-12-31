using Common.Controls.ColorManagement.ColorModels;

using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

using Color = System.Drawing.Color;

namespace VixenModules.Effect.Pattern
{
	/// <summary>
	/// Contains method specific to the Weave pattern.
	/// </summary>
	public partial class Pattern : PixelEffectBase
	{
		#region Private Methods

		/// <summary>
		/// Gets the spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(WeaveSpacing / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(WeaveThickness / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;
		}

		/// <summary>
		/// Gets the horizontal spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetHorizontalWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(WeaveHorizontalSpacing / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the horizontal thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetHorizontalWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(WeaveHorizontalThickness / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;
		}

		/// <summary>
		/// Gets the vertical spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetVerticalWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(WeaveVerticalSpacing / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the vertical thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetVerticalWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(WeaveVerticalThickness / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;
		}

		/// <summary>
		/// Draws the repeating tile for the specified frame.
		/// </summary>		
		private void InitializeWeaveTile()
		{			
			// If advanced sizing has been selected then...
			if (AdvancedSizing)
			{
				// Calculate the weave bar spacing
				_weaveHorizontalSpacing = GetHorizontalWeaveSpacing(_scaleValue);

				// Calculate the weave bar spacing
				_weaveVerticalSpacing = GetVerticalWeaveSpacing(_scaleValue);

				// Calculate the weave bar thickness
				_weaveHorizontalThickness = GetHorizontalWeaveThickness(_scaleValue);

				// Calculate the weave bar thickness
				_weaveVerticalThickness = GetVerticalWeaveThickness(_scaleValue);
			}
			// Otherwise both the vertical and horizontal bars will be sized the same
			else
			{
				// Calculate the weave bar spacing
				_weaveHorizontalSpacing = GetWeaveSpacing(_scaleValue);
				_weaveVerticalSpacing = _weaveHorizontalSpacing;

				// Calculate the weave bar thickness
				_weaveHorizontalThickness = GetWeaveThickness(_scaleValue);
				_weaveVerticalThickness = _weaveHorizontalThickness;
			}

			// Calculate the height of the weave repeating tile
			_heightOfTile = CalculateTileWidthHeight(_weaveHorizontalThickness, _weaveHorizontalSpacing, HorizontalColors.Count());

			// Calculate the width of the weave repeating tile
			_widthOfTile = CalculateTileWidthHeight(_weaveVerticalThickness, _weaveVerticalSpacing, VerticalColors.Count());

			// Initialize the repeating tile frame buffer
			_tileFrameBuffer = new PixelFrameBuffer(_widthOfTile, _heightOfTile);

			// Draw the repeating weave tile
			SetupRenderWeave();
		}

		/// <summary>
		/// The design of the weave effect draws a small repeating tile of the weave bars.
		/// This tile is then repeated across the display element.  
		/// </summary>       
		private void SetupRenderWeave()
		{
			// Draw the weave on the tile frame buffer
			DrawWeave(_tileFrameBuffer,
				_heightOfTile,
				_widthOfTile,
				HorizontalColors,
				VerticalColors,
				_weaveHorizontalThickness,
				_weaveVerticalThickness,
				_weaveHorizontalSpacing,
			_weaveVerticalSpacing);
		}

		/// <summary>
		/// Draws the weave pattern on the repeating tile.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the weave onto</param>
		/// <param name="height">Height of the repeating tile</param>
		/// <param name="width">Width of the repeating tile</param>
		/// <param name="horizontalColors">Horizontal gradients associated with the horizontal bars</param>
		/// <param name="verticalColors">vertical gradients associated with the vertical bars</param>
		/// <param name="barHorizontalThickness">Horizontal thickness of the weave bar</param>
		/// <param name="barVerticalThickness">Vertical thickness of the weave bar</param>
		/// <param name="barHorizontalSpacing">Horizontal spacing in-between the bars</param>
		/// /// <param name="barVerticalSpacing">Horizontal spacing in-between the bars</param>
		private void DrawWeave(
			IPixelFrameBuffer frameBuffer,
			int height,
			int width,
			List<ColorGradient> horizontalColors,
			List<ColorGradient> verticalColors,
			int barHorizontalThickness,
			int barVerticalThickness,
			int barHorizontalSpacing,
			int barVerticalSpacing)
		{
			// Draw the vertical bars
			DrawBars(
				width,
				barVerticalThickness,
				barHorizontalThickness,
				barVerticalSpacing,
				barHorizontalSpacing,
				height,
				frameBuffer,
				verticalColors,
				DrawVerticalLineBottomToTop,
				Show3D,
				Highlight,
				false);

			// Draw the horizontal bars
			DrawBars(
				height,
				barHorizontalThickness,
				barVerticalThickness,
				barHorizontalSpacing,
				barVerticalSpacing,
				width,
				frameBuffer,
				horizontalColors,
				DrawHorizontalLineLeftRight,
				Show3D,
				Highlight,
				true);
		}

		/// <summary>
		/// Draws the weave bars on the tile frame buffer.
		/// </summary>
		/// <param name="dimensionLength">The length of the dimension being looped over ( x or y)</param>
		/// <param name="barThickness">Thickness of the weave bar</param>
		/// <param name="otherBarThickness">Thickness of the weave bar perpendicular to the ones being drawn</param>
		/// <param name="barSpacing">Spacing in-between the weave bars</param>
		/// <param name="otherBarSpacing">Spacing in-between the weave bars perpendicular to the ones being drawn</param>
		/// <param name="barLength">Length of the weave bar</param>
		/// <param name="frameBuffer">Frame buffer to draw the weave bars on</param>
		/// <param name="colors">Color gradients to draw the weave bars</param>
		/// <param name="drawMethod">Delegate to draw an individual bar</param>
		/// <param name="show3D">Flag to control whether the weave bars appear 3-D</param>
		/// <param name="highlight">Flag to control whether the weave bars have a white highlight</param>
		/// <param name="odd">This flag helps to initialize the method such that half the weave bars go under and the other half go over</param>
		private void DrawBars(
			int dimensionLength,
			int barThickness,
			int otherBarThickness,
			int barSpacing,
			int otherBarSpacing,
			int barLength,
			IPixelFrameBuffer frameBuffer,
			List<ColorGradient> colors,
			Action<int, int, int, int, IPixelFrameBuffer, List<ColorGradient>, bool, bool, int, int> drawMethod,
			bool show3D,
			bool highlight,
			bool odd)
		{
			// Axis counter, using this counter to determine where to start drawing a bar
			int d = 0;

			// Bar counter keeps track of how many bars have been drawn.
			// Used to determine if the bar is an even or an odd bar.
			int barCounter = 0;

			// Bar counter comparision value to help determine if the current
			// bar is an odd or even bar.
			int barCounterCheck = 0;

			// Counter to determine which color to use
			int colorIndex = 0;

			// To get the weave pattern some bars need to go over vs under other bars
			if (odd)
			{
				// This counter check variable helps determine whether to go over or under
				barCounterCheck = 1;
			}

			// Loop over the dimension (x-axis or y-axis)
			while (d < dimensionLength)
			{
				// If another complete bar fits on the axis then...
				if ((d + barThickness) <= dimensionLength)
				{
					// Draw the bar
					drawMethod(
						d,
						barThickness,
						otherBarThickness,
						barLength,
						frameBuffer,
						colors,
						barCounter % 2 == barCounterCheck,
						barCounter % 2 == barCounterCheck,
						otherBarSpacing,
						colorIndex);

					// Keep track of the number bars drawn
					barCounter++;

					// Iterate to the next color
					colorIndex++;

					// If there are no more colors then...
					if (colorIndex > colors.Count() - 1)
					{
						// Wrap back around to the first color
						colorIndex = 0;
					}
				}

				// Position where the next bar should be drawn
				d = d + barThickness + barSpacing;
			}
		}
		
		/// <summary>
		/// Calculates the total width/height of the repeating tile.
		/// </summary>
		/// <param name="barThickness">Thickness of the weave bar</param>
		/// <param name="barSpacing">Spacing in-between the bars</param>
		/// <param name="colors">Collection of colors associated with the bars</param>
		/// <returns></returns>
		int CalculateTileWidthHeight(
			int barThickness,
			int barSpacing,
			int colors)
		{
			// Need the bar thickness then the bar spacing for each of the weave color bars
			return (2 * barThickness + 2 * barSpacing) * colors;
		}

		/// <summary>
		/// Sets a pixel on the weave bar.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the line on</param>
		/// <param name="currentThicknessCounter">Number of pixels drawn for the bar so far</param>
		/// <param name="d2">Position of dimension 2</param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <param name="dStart">Starting position of the line</param>
		/// <param name="colors">Collection of applicable bar colors</param>
		/// <param name="colorIndex">Current bar color index</param>
		private void SetPixelX(
			IPixelFrameBuffer frameBuffer,
			int currentThicknessCounter,
			int d2,
			int barThickness,
			int dStart,
			List<ColorGradient> colors,
			int colorIndex)
		{
			// Set the specified pixel with the specified bar color
			frameBuffer.SetPixel(currentThicknessCounter + dStart, d2, GetBarColor(colorIndex, currentThicknessCounter, barThickness, colors));
		}

		/// <summary>
		/// Sets a pixel on the weave bar.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to draw the pixel on</param>
		/// <param name="currentThicknessCounter">Number of pixels drawn for the bar so far</param>
		/// <param name="d2">Position of dimension 2</param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <param name="dStart">Starting position of the line</param>
		/// <param name="colors">Collection of applicable bar colors</param>
		/// <param name="colorIndex">Current bar color index</param>
		private void SetPixelY(
			IPixelFrameBuffer frameBuffer,
			int currentThicknessCounter,
			int d2,
			int barThickness,
			int dStart,
			List<ColorGradient> colors,
			int colorIndex)
		{
			// Set the specified pixel with the specified bar color
			frameBuffer.SetPixel(d2, currentThicknessCounter + dStart, GetBarColor(colorIndex, currentThicknessCounter, barThickness, colors));
		}

		/// <summary>
		/// Draws a weave line.
		/// </summary>
		/// <param name="dStart">Initial position in the repeating tile</param>
		/// <param name="d2Length">Length of the second dimension</param>
		/// <param name="frameBuffer">Frame buffer to draw the line on</param>
		/// <param name="colors">Colors associated with the weave bars</param>
		/// <param name="overOrUnder">Initial value of whether the line should go over or under the next perpendicular line</param>
		/// <param name="onOff">Initial value of whether the logical pen is down</param>
		/// <param name="barThickness">Thickness of the weave bar</param>
		/// <param name="otherBarThickness">Thickness of the weave bars perpendicular to the ones being drawn</param>
		/// <param name="barSpacing">Spacing in-between weave bars</param>
		/// <param name="colorIndex">Current color index into the colors collection</param>
		/// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
		private void DrawLine(
			int dStart,
			int d2Length,
			IPixelFrameBuffer frameBuffer,
			List<ColorGradient> colors,
			bool overOrUnder,
			bool onOff,
			int barThickness,
			int otherBarThickness,
			int barSpacing,
			int colorIndex,
			Action<IPixelFrameBuffer, int, int, int, int, List<ColorGradient>, int> setPixel)
		{
			// Loop over the first dimension (bar thickness)
			for (int d1 = 0; d1 < barThickness; d1++)
			{
				// Counter to keep track of how many pixels have been processed while
				// not drawing the line
				int offCounter = 0;

				// Initialize the flag that determines if the logical pixel pen is On.
				bool penOn = onOff;

				// Initialize the flag that determines if this line will go over the
				// next perpendicular line.
				bool over = overOrUnder;

				// Loop over the second dimension
				for (int d2 = 0; d2 < d2Length; d2++)
				{
					// If drawing a pixel then...
					if (penOn)
					{
						// Set the pixel the appropriate color
						setPixel(
							frameBuffer,
							d1,
							d2,
							barThickness,
							dStart,
							colors,
							colorIndex);
					}
					// Otherwise the pen is Off
					else
					{
						// Keep track how pixels have been skipped
						offCounter++;
					}

					// If we have skipped enough pixels to cover the thickness of a bar then...
					if (offCounter == otherBarThickness)
					{
						// Toggle the logical pen to On
						penOn = true;
					}

					// If the position within the bar is an even
					// multiple of a bar and the spacing between bars then...
					if ((d2 + 1) % (otherBarThickness + barSpacing) == 0)
					{
						// Toggle the over/under flag so that the next intersection
						// with a perpendicular bar is handled opposite
						over = !over;

						// Reset the off counter
						offCounter = 0;

						// If the weave bar is going under then...
						if (!over)
						{
							// Turn the pen off
							penOn = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws a horizontal line that makes up the weave pattern.
		/// </summary>
		/// <param name="yStart"></param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <param name= "otherBarThickness">Thickness of the bar perpendicular to the ones being drawn</param>
		/// <param name="width">Width of the display element</param>
		/// <param name="frameBuffer">Frame buffer to draw on</param>
		/// <param name="colors">Colors associated with the horizontal bars</param>
		/// <param name="overOrUnder"></param>
		/// <param name="onOff"></param>
		/// <param name="barSpacing">Spacing in-between the weave bars</param>
		/// <param name="colorIndex">Color index into the horizontal color collection</param>
		private void DrawHorizontalLineLeftRight(
			int yStart,
			int barThickness,
			int otherBarThickness,
			int width,
			IPixelFrameBuffer frameBuffer,
			List<ColorGradient> colors,
			bool overOrUnder,
			bool onOff,
			int barSpacing,
			int colorIndex)
		{
			// Draw the horizontal line
			DrawLine(
				yStart,
				width,
				frameBuffer,
				colors,
				overOrUnder,
				onOff,
				barThickness,
				otherBarThickness,
				barSpacing,
				colorIndex,
				SetPixelY);
		}

		/// <summary>
		/// Draws a vertical line that makes up the weave pattern.
		/// </summary>
		/// <param name="xStart">Initial X position in the tile</param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <param name="otherBarThickness">Thickness of the bar perpendicular to the ones being drawn</param>
		/// <param name="frameBuffer">Frame buffer to draw on</param>
		/// <param name="colors">Colors associated with the horizontal bars</param>
		/// <param name="overOrUnder"></param>
		/// <param name="onOff"></param>
		/// <param name="colorIndex">Color index into the horizontal color collection</param>
		private void DrawVerticalLineBottomToTop(
			int xStart,
			int barThickness,
			int otherBarThickness,
			int height,
			IPixelFrameBuffer frameBuffer,
			List<ColorGradient> colors,
			bool overOrUnder,
			bool onOff,
			int blankBarHeight,
			int colorIndex)
		{
			// Draw vertical line
			DrawLine(
				xStart,
				height,
				frameBuffer,
				colors,
				overOrUnder,
				onOff,
				barThickness,
				otherBarThickness,
				blankBarHeight,
				colorIndex,
				SetPixelX);
		}

		/// <summary>
		/// Gets the highlight color for the weave color index.
		/// </summary>
		/// <returns>Highlight color for the specified color index</returns>
		private Color GetHighlightColor(int colorIndex, List<ColorGradient> colors)
		{
			// The color gradients run perpendicular to the bars so the highlight always
			// impacts the beginning of the gradient
			Color highlightColor = colors[colorIndex].GetColorAt(0);

			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(highlightColor);

			// Highlight the weave bar
			hsv.S = 0.0f;

			// Convert the HSB color back to RGB
			highlightColor = hsv.ToRGB();

			return highlightColor;
		}

		/// <summary>
		/// Gets the color for the bar taking into account the specified pixel position within the bar.
		/// </summary>
		/// <param name="colorIndex">Index of the current color</param>       
		/// <param name="currentThicknessCounter">Pixel position within the bar</param>
		/// <param name="barThickness">Thickness of the bars</param>
		/// <returns>Gets the color for the specified bar pixel</returns>
		private Color GetBarColor(int colorIndex, int currentThicknessCounter, int barThickness, List<ColorGradient> colors)
		{
			Color color;

			// Default the highlight to only the first row of pixels
			int highLightRow = 0;

			// If the effect is in locations mode then...
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Make the first % pixels white
				highLightRow = (int)(barThickness * HighlightPercentage / 100.0);
			}

			// If we are at the beginning of the bar and
			// highlight is selected then...
			if (Highlight &&
				currentThicknessCounter <= highLightRow)
			{
				// Set the color to the highlight color
				color = GetHighlightColor(colorIndex, colors);
			}
			// Otherwise if the bar is 3-D then...
			else if (Show3D)
			{
				// Set the color to the 3-D color
				color = Get3DColor(colorIndex, currentThicknessCounter, barThickness, colors);
			}
			else
			{
				// Default to the gradient position based on the position in the bar
				color = colors[colorIndex].GetColorAt(currentThicknessCounter / (double)barThickness);
			}

			// Return the color of the bar
			return color;
		}

		/// <summary>
		/// Gets the 3-D color for the specified bar pixel.
		/// </summary>
		/// <param name="colorIndex">Index into the color array</param>
		/// <param name="currentThicknessCounter">Current row of the bar being drawn</param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <returns>3-D color for the specified bar pixel</returns>
		private Color Get3DColor(int colorIndex, int currentThicknessCounter, int barThickness, List<ColorGradient> colors)
		{
			// Get the specified color from the color array
			Color color = colors[colorIndex].GetColorAt(currentThicknessCounter / (double)barThickness);

			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(color);

			// Set the brightness based on the percentage of the bar thickness
			hsv.V *= (float)(barThickness - currentThicknessCounter) / barThickness;

			// Convert the color back to RGB format
			return hsv.ToRGB();
		}

		#endregion
	}
}
