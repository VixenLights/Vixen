using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

using Common.Controls.ColorManagement.ColorModels;

using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

using Color = System.Drawing.Color;

namespace VixenModules.Effect.Pattern
{
	/// <summary>
	/// Contains method specific to the Brick pattern.
	/// </summary>
	public partial class Pattern : PixelEffectBase
	{
		#region Fields

		/// <summary>
		/// Weave horizontal thickness in pixels.
		/// </summary>
		private int _weaveHorizontalThickness;

		/// <summary>
		/// Weave vertical thickness in pixels.
		/// </summary>
		private int _weaveVerticalThickness;

		/// <summary>
		/// Horizontal spacing between the weave in pixels.
		/// </summary>
		private int _weaveHorizontalSpacing;

		/// <summary>
		/// Vertical spacing between the weave in pixels.
		/// </summary>
		private int _weaveVerticalSpacing;

		#endregion

		#region Private Methods

		/// <summary>
		/// Draws the repeating brick tile for the specified frame.
		/// </summary>				
		/// <param name="frame">Current frame within the effect duration</param>		
		private void InitializeBrickTile(int frame)
		{												
			// Calculate the size of the brick and mortar
			int brickWidth = GetBrickWidth(_scaleValue);
			int brickHeight = GetBrickHeight(_scaleValue);
			int mortarHeight = GetMortarHeight(_scaleValue);

			// Calculate the height of the brick repeating tile
			_widthOfTile = CalculateTileWidth(brickWidth, mortarHeight, HorizontalColors.Count()); 

			// Calculate the width of the brick repeating tile
			_heightOfTile = CalculateTileHeight(brickHeight, mortarHeight, HorizontalColors.Count()); 

			// Initialize the repeating tile frame buffer
			_tileFrameBuffer = new PixelFrameBuffer(_widthOfTile, _heightOfTile);

			// Draw the repeating brick tile
			SetupRenderBrick(brickWidth, brickHeight, mortarHeight, frame);

			// If the brick is to be rotated 90 degrees then...
			if (RotateBrick)
			{
				// Transpose the X and Y of the tile
				_tileFrameBuffer = TransposeTile(_tileFrameBuffer);

				// Swap the width and height of the tile
				int temp = _widthOfTile;
				_widthOfTile = _heightOfTile;
				_heightOfTile = temp;
			}

			/*
			Bitmap test = new Bitmap(_widthOfTile, _heightOfTile, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			for (int y = 0; y < _heightOfTile; y++)
			{
				for (int x = 0; x < _widthOfTile; x++)
				{
					test.SetPixel(x, y, _tileFrameBuffer.GetColorAt(x, y));
				}
			}
			test.Save("c:\\Temp\\Tile.bmp");
			*/			
		}

		/// <summary>
		/// Transposes the tile frame buffer.  Swaps X and Y.
		/// </summary>
		/// <param name="original">Original frame buffer to transpose</param>
		/// <returns>Transposed tile frame buffer</returns>
		private IPixelFrameBuffer TransposeTile(IPixelFrameBuffer original)		
		{ 
			// Create a new bitmap with transposed dimensions
			PixelFrameBuffer transposed = new PixelFrameBuffer(_heightOfTile, _widthOfTile); 
			
			// Iterate over each pixel in the original frame buffer
			for (int x = 0; x < _widthOfTile; x++) 
			{ 
				for (int y = 0; y < _heightOfTile; y++) 
				{ 
					// Get the pixel at the current position
					Color originalColor = original.GetColorAt(x, y); 
					
					// Set the pixel in the transposed bitmap
					transposed.SetPixel(_heightOfTile - y - 1, x, originalColor); 
				} 
			} 
			
			return transposed; 
		}

		/// <summary>
		/// Calculates the width of the brick tile.
		/// </summary>
		/// <param name="brickWidth">Brick width</param>
		/// <param name="mortorHeight">Mortar height</param>
		/// <param name="colors">Number of brick colors</param>
		/// <returns>Width of the brick tile</returns>
		private int CalculateTileWidth(
			int brickWidth,
			int mortarHeight,			
			int colors)
		{
			// Initialize the count to the number of colors
			int count = colors;
						
			// Add together all the brick colors with mortar separating them
			return (int)(count * brickWidth) + count * mortarHeight;			
		}

		/// <summary>
		/// Calculates the height of the brick tile.
		/// </summary>
		/// <param name="brickHeight">Brick height</param>
		/// <param name="mortarHeight">Mortar height</param>
		/// <param name="colors">Colors of the bricks</param>
		/// <returns>Height of the brick tile</returns>
		private int CalculateTileHeight(
			int brickHeight,
			int mortorHeight,
			int colors)		
		{
			// The height is always two rows of mortar and two rows of bricks
			int count = 2;
			
			// Need the bar thickness then the bar spacing for each of the brick color bars
			return (int)(count * brickHeight) + count * mortorHeight;			
		}

		/// <summary>
		/// Gets the height of the brick in pixels.
		/// </summary>
		/// <param name="scaleValue">Largest dimension of the prop</param>
		/// <returns>Height of the brick in pixels</returns>
		private int GetBrickHeight(int scaleValue)
		{
			int height = (int)(BrickHeight / 100.0 * scaleValue);

			if (height < 1)
			{
				height = 1;	
			}

			return height;
		}

		/// <summary>
		/// Gets the width of the brick in pixels.
		/// </summary>
		/// <param name="scaleValue">Largest dimension of the prop</param>
		/// <returns>Width of the brick in pixels</returns>
		private int GetBrickWidth(int scaleValue)
		{
			int width = (int)(BrickWidth / 100.0 * scaleValue);

			if (width < 1)
			{
				width = 1;
			}

			return width;
		}

		/// <summary>
		/// Gets the height of the brick mortar in pixels.
		/// </summary>
		/// <param name="scaleValue">Largest dimension of the prop</param>
		/// <returns>Height of the brick mortar in pixels</returns>
		private int GetMortarHeight(int scaleValue)
		{
			int height = (int)(MortarHeight / 100.0 * scaleValue);

			if (height < 1)
			{
				height = 1;
			}

			return height;
		}

		/// <summary>
		/// The design of the brick effect draws a small repeating tile of the bricks.
		/// This tile is then repeated across the display element.  
		/// </summary>       
		/// <param name="brickWidth">Width of the bricks</param>
		/// <param name="brickHeight">Height of the bricks</param>
		/// <param name="mortarHeight">Height of the mortar</param>		
		/// <param name="frame">Current frame within the effect duration</param>		
		private void SetupRenderBrick(int brickWidth, int brickHeight, int mortarHeight, int frame)
		{
			// Create a collection of color gradients
			List<ColorGradient> colors = new List<ColorGradient>();
			
			// If there are less than two gradients then...
			if (HorizontalColors.Count < 2)
			{
				// Add the single color gradient to the collection twice
				colors.Add(HorizontalColors[0]);
				colors.Add(HorizontalColors[0]);
			}
			else
			{
				colors = HorizontalColors.ToList();
			}

			// Y position counter
			int skipY = 0;
			
			// Loop over two rows of bricks
			for (int colorIndex = 0; colorIndex < 2; colorIndex++)
			{
				// X position counter
				int skipX = 0;

				// Determine if we are processing the first row of bricks
				bool firstRow = colorIndex % 2 == 0;

				// If this is the first row of bricks then...
				if (firstRow)
				{					
					// Loop over the colors
					foreach (ColorGradient colorGradient in colors)
					{
						// Draw a full brick of the specified color gradient
						DrawBrickAndMortar(skipX, skipY, brickWidth, brickHeight, colorGradient, mortarHeight, frame);
												
						// Increment the x position counter
						skipX += brickWidth + mortarHeight;
					}
				}
				// Otherwise this is the second row of bricks
				else
				{
					// Calculate the width of a half brick
					int secondHalfBrickWidth = brickWidth / 2;

					// If the brick length is not evenly divisible by 2 then...
					if (brickWidth % 2 != 0)
					{
						// Increase the half brick width by one
						secondHalfBrickWidth++;
					}

					// Retrieve the first brick color
					ColorGradient firstColor = HorizontalColors[0];

					// Draw a half brick
					DrawHalfBrick(0, skipY, brickHeight, brickWidth / 2, firstColor);
					
					// Draw mortar between bricks
					DrawMortar(skipX + brickWidth / 2, skipY, mortarHeight, brickHeight, frame);
					
					// Increment the x position counter
					skipX = brickWidth / 2 + mortarHeight;

					// Loop over the remaining colors
					for (int colorIndex2 = 1; colorIndex2 < colors.Count; colorIndex2++)
					{
						// Draw a full brick and motar
						DrawBrickAndMortar(skipX, skipY, brickWidth, brickHeight, colors[colorIndex2], mortarHeight, frame);

						// Increment the x position												
						skipX += brickWidth + mortarHeight;
					}

					// Draw the last half brick (longer half)
					DrawHalfBrick(skipX, skipY, brickHeight, secondHalfBrickWidth, firstColor);								
				}
				
				// Draw mortar the length of the tile between brick rows
				DrawMortar(0, skipY + brickHeight, _widthOfTile, mortarHeight, frame);

				// Advance to the next row of bricks
				skipY += brickHeight + mortarHeight;
			}
		}

		/// <summary>
		/// Draws a full brick and the mortar between bricks.
		/// </summary>
		/// <param name="x">X starting position</param>
		/// <param name="y">Y starting position</param>
		/// <param name="brickWidth">Width of a brick</param>
		/// <param name="brickHeight">Height of a brick</param>
		/// <param name="colorGradient">Color gradient of the brick</param>
		/// <param name="mortarHeight">Height of the mortar</param>	
		/// <param name="frame">Current frame within the effect duration</param>		
		private void DrawBrickAndMortar(int x, int y, int brickWidth, int brickHeight, ColorGradient colorGradient, int mortarHeight, int frame)
		{
			// Draw a full brick
			DrawFullBrick(x, y, brickWidth, brickHeight, colorGradient);

			// Draw the mortar between bricks
			DrawMortar(x + brickWidth, y, mortarHeight, brickHeight, frame);
		}

		/// <summary>
		/// Draws mortar between bricks.
		/// </summary>
		/// <param name="x">X starting position</param>
		/// <param name="y">Y starting position</param>
		/// <param name="mortarWidth">Width of a brick</param>
		/// <param name="brickHeight">Height of a brick</param>		
		/// <param name="frame">Current frame within the effect duration</param>		
		private void DrawMortar(int x, int y, int mortarWidth, int brickHeight, int frame)
		{
			// Loop over brick height
			for (int h = 0; h < brickHeight; h++)
			{
				// Loop over brick width
				for (int w = 0; w < mortarWidth; w++)
				{
					// Draw a pixel of mortar
					_tileFrameBuffer.SetPixel(x + w, y + h, MortarColor.GetColorAt(GetEffectTimeIntervalPosition(frame)));
				}
			}
		}

		/// <summary>
		/// Draws a full brick.
		/// </summary>
		/// <param name="x">X starting position</param>
		/// <param name="y">Y starting position</param>
		/// <param name="brickWidth">Width of a brick</param>
		/// <param name="brickHeight">Height of a brick</param>
		/// <param name="colorGradient">Color gradient of the brick</param>		
		private void DrawFullBrick(int x, int y, int brickWidth, int brickHeight, ColorGradient colorGradient)
		{
			// Loop over brick height
			for (int h = 0; h < brickHeight; h++)
			{				
				// Loop over brick width
				for (int w = 0; w < brickWidth; w++)
				{
					// Draw a pixel of a brick
					_tileFrameBuffer.SetPixel(x + w, y + h, GetBrickColor(h, brickHeight, colorGradient));					
				}
			}
		}

		/// <summary>
		/// Gets the 3-D color for the specified brick pixel.
		/// </summary>
		/// <param name="currentThicknessCounter">Current row of pixels within the brick being drawn</param>		
		/// <param name="brickThickness">Thickness of the brick</param>
		/// <returns>3-D color for the specified brick pixel</returns>
		private Color Get3DColor(int currentThicknessCounter, int brickThickness, ColorGradient colorGradient)
		{
			// Get the specified color from the color array
			Color color = colorGradient.GetColorAt(currentThicknessCounter / (double)brickThickness);

			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(color);

			// Set the brightness based on the percentage of the brick thickness
			hsv.V *= (float)(brickThickness - currentThicknessCounter) / brickThickness;

			// Convert the color back to RGB format
			return hsv.ToRGB();
		}

		/// <summary>
		/// Gets the color for the brick taking into account the specified pixel position within the brick.
		/// </summary>		
		/// <param name="currentThicknessCounter">Pixel position within the brick</param>
		/// <param name="brickThickness">Thickness of the bricks</param>
		/// <param name="colorGradient">Color gradient of the brick</param>
		/// <returns>Gets the color for the specified brick pixel</returns>
		private Color GetBrickColor(int currentThicknessCounter, int brickThickness, ColorGradient colorGradient)
		{
			Color color;

			// Default the highlight to only the first row of pixels
			int highLightRow = 0;

			// If the effect is in locations mode then...
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Make the first % pixels white
				highLightRow = (int)(brickThickness * HighlightPercentage / 100.0);
			}

			// If we are at the beginning of the bar and
			// highlight is selected then...
			if (Highlight &&
				currentThicknessCounter <= highLightRow)
			{
				// Set the color to the highlight color
				color = GetHighlightColor(colorGradient);
			}
			// Otherwise if the bar is 3-D then...
			else if (Show3D)
			{
				// Set the color to the 3-D color
				color = Get3DColor(currentThicknessCounter, brickThickness, colorGradient);
			}
			else
			{
				// Default to the gradient position based on the position in the bar
				color = colorGradient.GetColorAt(currentThicknessCounter / (double)brickThickness);
			}

			// Return the color of the bar
			return color;
		}

		/// <summary>
		/// Gets the highlight color for the brick.
		/// </summary>		
		/// <param name="colorGradient">Color gradient of the brick</param>
		/// <returns>Highlight color</returns>
		private Color GetHighlightColor(ColorGradient colorGradient)
		{
			// The color gradients run perpendicular to the bricks so the highlight always
			// impacts the beginning of the gradient
			Color highlightColor = colorGradient.GetColorAt(0);

			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(highlightColor);

			// Highlight the weave bar
			hsv.S = 0.0f;

			// Convert the HSB color back to RGB
			highlightColor = hsv.ToRGB();

			return highlightColor;
		}

		/// <summary>
		/// Draws a half brick.
		/// </summary>
		/// <param name="x">X starting position</param>
		/// <param name="y">Y starting position</param>
		/// <param name="brickHeight">Height of a brick</param>
		/// <param name="halfWidth">Half width of a brick</param>
		/// <param name="colorGradient">Color gradient of the brick</param>		
		private void DrawHalfBrick(int x, int y, int brickHeight, int halfWidth, ColorGradient colorGradient)
		{
			// Loop over brick height
			for (int h = 0; h < brickHeight; h++)
			{
				// Loop over half the brick width
				for (int w = 0; w < halfWidth; w++)
				{
					// Draw a pixel of the half brick
					_tileFrameBuffer.SetPixel(x + w, y + h, GetBrickColor(h, brickHeight, colorGradient));
				}
			}
		}

		#endregion
	}
}
