using System.Drawing;
using Vixen.Data.Value;
using Xunit;

namespace Vixen.Tests.Data.Value;

public class RGBValueTests
{
	// Grayscale formula: (byte)(R * 0.3 + G * 0.59 + B * 0.11)
	// Cast truncates (does not round):
	//   Black  (0,0,0)       -> 0
	//   White  (255,255,255) -> (byte)(76.5 + 150.45 + 28.05) = 255
	//   Red    (255,0,0)     -> (byte)(76.5) = 76

	[Fact]
	public void GetGrayscaleLevel_Black_Returns0()
	{
		// Arrange
		var color = Color.FromArgb(0, 0, 0);

		// Act
		var level = RGBValue.GetGrayscaleLevel(color);

		// Assert
		Assert.Equal((byte)0, level);
	}

	[Fact]
	public void GetGrayscaleLevel_White_Returns255()
	{
		// Arrange
		var color = Color.FromArgb(255, 255, 255);

		// Act
		var level = RGBValue.GetGrayscaleLevel(color);

		// Assert
		Assert.Equal((byte)255, level);
	}

	[Fact]
	public void GetGrayscaleLevel_Red_Returns76()
	{
		// Arrange — luma = 255*0.3 = 76.5, truncated to byte = 76
		var color = Color.FromArgb(255, 0, 0);

		// Act
		var level = RGBValue.GetGrayscaleLevel(color);

		// Assert
		Assert.Equal((byte)76, level);
	}
}