using System.Drawing;
using VixenModules.Preview.VixenPreview;
using Xunit;

namespace Vixen.Tests.Preview.VixenPreview
{
	/// <summary>
	/// Verifies recovery decisions for saved Preview window bounds.
	/// </summary>
	public sealed class PreviewWindowBoundsTests
	{
		/// <summary>
		/// Verifies that a window wholly outside the primary working area is not recoverable.
		/// </summary>
		[Theory]
		[InlineData(-800, 0, 800, 600)]
		[InlineData(1920, 0, 800, 600)]
		[InlineData(0, -600, 800, 600)]
		[InlineData(0, 1080, 800, 600)]
		public void IsRecoverable_WhenWindowIsWhollyOutsidePrimaryWorkingArea_ReturnsFalse(int x, int y, int width, int height)
		{
			// Arrange
			var windowBounds = new Rectangle(x, y, width, height);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.False(result);
		}

		/// <summary>
		/// Verifies that the final pixel inside a working area is recoverable.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenUpperRightPixelIsAtLastWorkingAreaPixel_ReturnsTrue()
		{
			// Arrange
			var windowBounds = new Rectangle(1919, 0, 1, 1);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.True(result);
		}

		/// <summary>
		/// Verifies that the exclusive right edge of a working area is not recoverable.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenWindowStartsAtExclusiveWorkingAreaRightEdge_ReturnsFalse()
		{
			// Arrange
			var windowBounds = new Rectangle(1920, 0, 1, 1);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.False(result);
		}

		/// <summary>
		/// Verifies that a window is recoverable when only its actual upper-right pixel is visible.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenOnlyUpperRightPixelIsVisible_ReturnsTrue()
		{
			// Arrange
			var windowBounds = new Rectangle(-20, 0, 21, 50);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.True(result);
		}

		/// <summary>
		/// Verifies that a window on an active negative-coordinate monitor is recoverable.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenWindowIsOnNegativeCoordinateMonitor_ReturnsTrue()
		{
			// Arrange
			var windowBounds = new Rectangle(-100, 50, 100, 200);
			var workingAreas = new[] { new Rectangle(-1280, 0, 1280, 1024) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.True(result);
		}

		/// <summary>
		/// Verifies that a window on a removed monitor is not recoverable.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenWindowWasSavedOnRemovedMonitor_ReturnsFalse()
		{
			// Arrange
			var windowBounds = new Rectangle(2000, 0, 800, 600);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.False(result);
		}

		/// <summary>
		/// Verifies that an empty saved window is not recoverable.
		/// </summary>
		[Theory]
		[InlineData(0, 100)]
		[InlineData(100, 0)]
		public void IsRecoverable_WhenWindowHasZeroSize_ReturnsFalse(int width, int height)
		{
			// Arrange
			var windowBounds = new Rectangle(0, 0, width, height);
			var workingAreas = new[] { new Rectangle(0, 0, 1920, 1080) };

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.False(result);
		}

		/// <summary>
		/// Verifies that a window with both upper corners in a monitor gap is not recoverable.
		/// </summary>
		[Fact]
		public void IsRecoverable_WhenWindowCornersAreInMultiMonitorGap_ReturnsFalse()
		{
			// Arrange
			var windowBounds = new Rectangle(1000, 0, 200, 100);
			var workingAreas = new[]
			{
				new Rectangle(0, 0, 1000, 800),
				new Rectangle(1200, 0, 1000, 800)
			};

			// Act
			var result = PreviewWindowBounds.IsRecoverable(windowBounds, workingAreas);

			// Assert
			Assert.False(result);
		}
	}
}
