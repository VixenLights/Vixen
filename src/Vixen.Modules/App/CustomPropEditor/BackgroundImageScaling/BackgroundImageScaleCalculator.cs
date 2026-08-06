namespace VixenModules.App.CustomPropEditor.BackgroundImageScaling
{
	/// <summary>
	/// Converts, rounds, and validates logical background image dimensions.
	/// </summary>
	internal static class BackgroundImageScaleCalculator
	{
		/// <summary>
		/// Gets the smallest permitted logical canvas dimension.
		/// </summary>
		internal const int MinimumDimension = 1;

		/// <summary>
		/// Gets the largest permitted logical canvas dimension.
		/// </summary>
		internal const int MaximumDimension = 100000;

		/// <summary>
		/// Attempts to convert a user-entered value to a validated editor-pixel dimension.
		/// </summary>
		/// <param name="value">The entered pixel or percentage value.</param>
		/// <param name="unit">The unit used by <paramref name="value" />.</param>
		/// <param name="sourceDimension">The corresponding source bitmap dimension.</param>
		/// <param name="dimension">When this method returns, contains the validated editor-pixel dimension.</param>
		/// <returns><see langword="true" /> if the conversion produces a valid dimension; otherwise, <see langword="false" />.</returns>
		internal static bool TryConvertToPixels(double value, BackgroundImageScaleUnit unit, int sourceDimension, out int dimension)
		{
			dimension = default;
			if (!double.IsFinite(value) || sourceDimension < MinimumDimension)
			{
				return false;
			}

			var pixelValue = unit switch
			{
				BackgroundImageScaleUnit.Pixels => value,
				BackgroundImageScaleUnit.Percent => sourceDimension * value / 100d,
				_ => double.NaN
			};

			return TryRoundDimension(pixelValue, out dimension);
		}

		/// <summary>
		/// Attempts to calculate a target height that preserves the source bitmap aspect ratio.
		/// </summary>
		/// <param name="targetWidth">The validated target width.</param>
		/// <param name="sourceWidth">The source bitmap width.</param>
		/// <param name="sourceHeight">The source bitmap height.</param>
		/// <param name="targetHeight">When this method returns, contains the validated matching height.</param>
		/// <returns><see langword="true" /> if the calculation produces a valid dimension; otherwise, <see langword="false" />.</returns>
		internal static bool TryCalculateLockedHeight(int targetWidth, int sourceWidth, int sourceHeight, out int targetHeight)
		{
			targetHeight = default;
			return targetWidth >= MinimumDimension && sourceWidth >= MinimumDimension && sourceHeight >= MinimumDimension &&
				TryRoundDimension((double)targetWidth * sourceHeight / sourceWidth, out targetHeight);
		}

		/// <summary>
		/// Attempts to calculate a target width that preserves the source bitmap aspect ratio.
		/// </summary>
		/// <param name="targetHeight">The validated target height.</param>
		/// <param name="sourceWidth">The source bitmap width.</param>
		/// <param name="sourceHeight">The source bitmap height.</param>
		/// <param name="targetWidth">When this method returns, contains the validated matching width.</param>
		/// <returns><see langword="true" /> if the calculation produces a valid dimension; otherwise, <see langword="false" />.</returns>
		internal static bool TryCalculateLockedWidth(int targetHeight, int sourceWidth, int sourceHeight, out int targetWidth)
		{
			targetWidth = default;
			return targetHeight >= MinimumDimension && sourceWidth >= MinimumDimension && sourceHeight >= MinimumDimension &&
				TryRoundDimension((double)targetHeight * sourceWidth / sourceHeight, out targetWidth);
		}

		/// <summary>
		/// Gets a value that indicates whether both dimensions are within the supported editor range.
		/// </summary>
		/// <param name="width">The logical canvas width.</param>
		/// <param name="height">The logical canvas height.</param>
		/// <returns><see langword="true" /> if both dimensions are valid; otherwise, <see langword="false" />.</returns>
		internal static bool AreValidDimensions(int width, int height) => IsValidDimension(width) && IsValidDimension(height);

		private static bool TryRoundDimension(double value, out int dimension)
		{
			dimension = default;
			if (!double.IsFinite(value))
			{
				return false;
			}

			var rounded = Math.Round(value, MidpointRounding.AwayFromZero);
			if (rounded < MinimumDimension || rounded > MaximumDimension)
			{
				return false;
			}

			dimension = (int)rounded;
			return true;
		}

		private static bool IsValidDimension(int value) => value is >= MinimumDimension and <= MaximumDimension;
	}
}
