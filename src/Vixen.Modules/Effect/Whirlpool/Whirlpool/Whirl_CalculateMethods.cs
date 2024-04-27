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
		/// Returns true if the specified length is twice the thickness of the whirl.
		/// </summary>
		/// <param name="length">Length to test</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <returns>True if the specified length is twice the thickness of the whirl</returns>
		private bool DoubleThickness(int length, int thickness)
		{
			return (length - 2 * thickness >= 0);
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the top left corner and rotating clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsTopLeftClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Top
				numberOfPixels += width - 1;
			}
			else
			{
				numberOfPixels += width + thickness + spacing - 1;
			}

			if (height - thickness >= thickness)
			{
				// Right
				numberOfPixels += height - 1;

				if (width - thickness >= thickness && DoubleThickness(height, thickness))
				{
					// Bottom Side
					numberOfPixels += width - 1;

					if (height - thickness - spacing >= thickness && DoubleThickness(width, thickness))
					{
						// Left Side
						numberOfPixels += height - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = false;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;

					_vortexMetadata.DrawLeft = false;
					_vortexMetadata.DrawTop = true;
					_vortexMetadata.DrawRight = true;
					_vortexMetadata.DrawBottom = false;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;


				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = true;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsTopLeftClockwise(
					x + thickness + spacing,
					y + thickness + spacing,
					width,
					height,
					thickness,
					spacing,
					false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);

				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawBottom &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight)
				{
					numberOfPixels++;
				}

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the top right corner and rotating counter-clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsTopRightCounterClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Top
				numberOfPixels += width - 1;
			}
			else
			{
				// Top
				numberOfPixels += width + thickness + spacing - 1;
			}

			if (height - thickness >= thickness)
			{
				// Left
				numberOfPixels += height - 1;

				if (width - thickness >= thickness && DoubleThickness(height, thickness))
				{
					// Bottom Side
					numberOfPixels += width - 1;

					if (height - thickness - spacing >= thickness && DoubleThickness(width, thickness))
					{
						// Right Side
						numberOfPixels += height - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = false;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;


					_vortexMetadata.DrawLeft = true;
					_vortexMetadata.DrawTop = true;
					_vortexMetadata.DrawRight = false;
					_vortexMetadata.DrawBottom = false;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;


				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = true;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsTopRightCounterClockwise(
					x + thickness + spacing,
					y + thickness + spacing,
					width,
					height,
					thickness,
					spacing,
					false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}
				//_lastNumberOfPixels = numberOfPixels;
				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the bottom left corner and rotating counter-clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsBottomLeftCounterClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;
			_vortexMetadata.DrawBottom = true;

			if (firstPass)
			{
				// Bottom
				numberOfPixels += width - 1;
			}
			else
			{
				// Bottom
				numberOfPixels += width + spacing + thickness - 1;
			}

			if (height - thickness >= thickness)
			{
				// Right
				numberOfPixels += height - 1;

				if (width - thickness >= thickness && DoubleThickness(height, thickness))
				{
					// Top
					numberOfPixels += width - 1;

					if (height - thickness - spacing >= thickness && DoubleThickness(width, thickness))
					{
						// Left Side
						numberOfPixels += height - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = false;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;

					_vortexMetadata.DrawLeft = false;
					_vortexMetadata.DrawTop = false;
					_vortexMetadata.DrawRight = true;
					_vortexMetadata.DrawBottom = true;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;

				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = true;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsBottomLeftCounterClockwise(
					x + thickness + spacing,
					y + thickness + spacing,
					width,
					height,
					thickness,
					spacing,
					false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}
				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the bottom left corner and rotating counter-clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		int CalculateNumberOfPixelsBottomRightClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Bottom
				numberOfPixels += width - 1;
			}
			else
			{
				// Bottom
				numberOfPixels += width + thickness + spacing - 1;
			}

			if (height - thickness >= thickness)
			{
				// Left
				numberOfPixels += height - 1;

				if (width - thickness >= thickness && DoubleThickness(height, thickness))
				{
					// Top
					numberOfPixels += width - 1;

					if (height - thickness - spacing >= thickness && DoubleThickness(width, thickness))
					{
						// Right Side
						numberOfPixels += height - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;


						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = false;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;


					_vortexMetadata.DrawLeft = true;
					_vortexMetadata.DrawTop = false;
					_vortexMetadata.DrawRight = false;
					_vortexMetadata.DrawBottom = true;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;

				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = true;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsBottomRightClockwise(
				x + thickness + spacing,
				y + thickness + spacing,
				width,
				height,
				thickness,
				spacing,
				false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}
				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the bottom right corner and rotating counter-clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsBottomRightCounterClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Right
				numberOfPixels += height - 1;
			}
			else
			{
				// Right
				numberOfPixels += height + thickness + spacing - 1;
			}

			if (width - thickness >= thickness)
			{
				// Top
				numberOfPixels += width - 1;

				if (height - thickness >= thickness && DoubleThickness(width, thickness) )
				{
					// Left
					numberOfPixels += height - 1;

					if (width - thickness - spacing >= thickness && DoubleThickness(height, thickness))
					{
						// Bottom Side
						numberOfPixels += width - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = false;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;


					_vortexMetadata.DrawLeft = false;
					_vortexMetadata.DrawTop = true;
					_vortexMetadata.DrawRight = true;
					_vortexMetadata.DrawBottom = false;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;

				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = true;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}
			
			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsBottomRightCounterClockwise(
				x + thickness + spacing,
				y + thickness + spacing,
				width,
				height,
				thickness,
				spacing,
				false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}
				
				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the bottom left corner and rotating clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsBottomLeftClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Left
				numberOfPixels += height - 1;
			}
			else
			{
				// Left
				numberOfPixels += height + thickness + spacing - 1;
			}

			if (width - thickness >= thickness)
			{
				// Top
				numberOfPixels += width - 1;

				if (height - thickness >= thickness && DoubleThickness(width, thickness))
				{
					// Right Side
					numberOfPixels += height - 1;

					if (width - thickness - spacing >= thickness && DoubleThickness(height, thickness))
					{
						// Bottom Side
						numberOfPixels += width - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = true;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = false;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;

					_vortexMetadata.DrawLeft = true;
					_vortexMetadata.DrawTop = true;
					_vortexMetadata.DrawRight = false;
					_vortexMetadata.DrawBottom = false;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;


				_vortexMetadata.DrawLeft = true;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsBottomLeftClockwise(
				x + thickness + spacing,
				y + thickness + spacing,
				width,
				height,
				thickness,
				spacing,
				false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the top right corner and rotating clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsTopRightClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawBottom = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;

			if (firstPass)
			{
				// Right
				numberOfPixels += height - 1;
			}
			else
			{
				// Right
				numberOfPixels += height + thickness + spacing - 1;
			}

			if (width - thickness >= thickness)
			{
				// Bottom
				numberOfPixels += width - 1;

				if (height - thickness >= thickness && DoubleThickness(width, thickness))
				{
					// Left Side
					numberOfPixels += height - 1;

					if (width - thickness - spacing >= thickness && DoubleThickness(height, thickness))
					{
						// Top Side
						numberOfPixels += width - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = false;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;

					_vortexMetadata.DrawLeft = false;
					_vortexMetadata.DrawTop = false;
					_vortexMetadata.DrawRight = true;
					_vortexMetadata.DrawBottom = true;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;

				_vortexMetadata.DrawLeft = false;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = true;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}
			
			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsTopRightClockwise(
				x + thickness + spacing,
				y + thickness + spacing,
				width,
				height,
				thickness,
				spacing,
				false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);

				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		/// <summary>
		/// Calculates the number of pixels in a whirl starting from the top left corner and rotating counter-clockwise.
		/// </summary>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="spacing">Spacing between the whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <returns>Number of pixels in the whirl</returns>
		private int CalculateNumberOfPixelsTopLeftCounterClockwise(int x, int y, int width, int height, int thickness, int spacing, bool firstPass)
		{
			int numberOfPixels = 0;

			_vortexMetadata.DrawLeft = true;
			_vortexMetadata.DrawTop = true;
			_vortexMetadata.DrawRight = true;
			_vortexMetadata.DrawBottom = true;

			if (firstPass)
			{
				// Left Side
				numberOfPixels += height - 1;
			}
			else
			{
				// Left Side
				numberOfPixels += height + spacing + thickness - 1;
			}

			if (width - thickness >= thickness)
			{
				// Bottom
				numberOfPixels += width - 1; // width - thickness

				if (height - thickness >= thickness && DoubleThickness(width, thickness))
				{
					// Right Side
					numberOfPixels += height - 1;

					if (width - thickness - spacing >= thickness && DoubleThickness(height, thickness))
					{
						// Top Side
						numberOfPixels += width - thickness - spacing - 1;
					}
					else
					{
						_vortexMetadata.LastWidth = width;
						_vortexMetadata.LastHeight = height;
						numberOfPixels++;

						_vortexMetadata.DrawLeft = true;
						_vortexMetadata.DrawTop = false;
						_vortexMetadata.DrawRight = true;
						_vortexMetadata.DrawBottom = true;

						_vortexMetadata.LastX = x;
						_vortexMetadata.LastY = y;
					}
				}
				else
				{
					_vortexMetadata.LastWidth = width;
					_vortexMetadata.LastHeight = height;
					numberOfPixels++;

					_vortexMetadata.DrawLeft = true;
					_vortexMetadata.DrawTop = false;
					_vortexMetadata.DrawRight = false;
					_vortexMetadata.DrawBottom = true;

					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}
			else
			{
				_vortexMetadata.LastWidth = width;
				_vortexMetadata.LastHeight = height;
				numberOfPixels++;

				_vortexMetadata.DrawLeft = true;
				_vortexMetadata.DrawTop = false;
				_vortexMetadata.DrawRight = false;
				_vortexMetadata.DrawBottom = false;

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}
			
			width -= 2 * (thickness + spacing);
			height -= 2 * (thickness + spacing);

			if (width >= thickness && height >= thickness)
			{
				numberOfPixels += CalculateNumberOfPixelsTopLeftCounterClockwise(
				x + thickness + spacing,
				y + thickness + spacing,
				width,
				height,
				thickness,
				spacing, 
				false);
			}
			else
			{
				_vortexMetadata.LastWidth = width += 2 * (thickness + spacing);
				_vortexMetadata.LastHeight = height += 2 * (thickness + spacing);
				if (_vortexMetadata.DrawLeft &&
				    _vortexMetadata.DrawTop &&
				    _vortexMetadata.DrawRight &&
				    _vortexMetadata.DrawBottom)
				{
					numberOfPixels++;
				}

				_vortexMetadata.LastX = x;
				_vortexMetadata.LastY = y;
			}

			return numberOfPixels;
		}

		#endregion
	}
}
	

		