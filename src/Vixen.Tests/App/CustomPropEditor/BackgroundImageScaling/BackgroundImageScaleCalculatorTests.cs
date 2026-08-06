using VixenModules.App.CustomPropEditor.BackgroundImageScaling;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.BackgroundImageScaling;

public sealed class BackgroundImageScaleCalculatorTests
{
	[Theory]
	[InlineData(640.4, 640)]
	[InlineData(640.5, 641)]
	[InlineData(640.6, 641)]
	public void TryConvertToPixels_RoundsPixelValuesAwayFromZero(double input, int expected)
	{
		var converted = BackgroundImageScaleCalculator.TryConvertToPixels(input, BackgroundImageScaleUnit.Pixels, 100, out var actual);

		Assert.True(converted);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TryConvertToPixels_ConvertsPercentageFromSourceDimension()
	{
		var widthConverted = BackgroundImageScaleCalculator.TryConvertToPixels(25, BackgroundImageScaleUnit.Percent, 4032, out var width);
		var heightConverted = BackgroundImageScaleCalculator.TryConvertToPixels(25, BackgroundImageScaleUnit.Percent, 3024, out var height);

		Assert.True(widthConverted);
		Assert.True(heightConverted);
		Assert.Equal(1008, width);
		Assert.Equal(756, height);
	}

	[Theory]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(100000.5)]
	public void TryConvertToPixels_RejectsInvalidDimensions(double input)
	{
		var converted = BackgroundImageScaleCalculator.TryConvertToPixels(input, BackgroundImageScaleUnit.Pixels, 100, out _);

		Assert.False(converted);
	}

	[Fact]
	public void TryCalculateLockedHeight_CalculatesFourByThreeDimensions()
	{
		var calculated = BackgroundImageScaleCalculator.TryCalculateLockedHeight(640, 4, 3, out var height);

		Assert.True(calculated);
		Assert.Equal(480, height);
	}

	[Fact]
	public void TryCalculateLockedWidth_RoundsAwayFromZero()
	{
		var calculated = BackgroundImageScaleCalculator.TryCalculateLockedWidth(1, 3, 2, out var width);

		Assert.True(calculated);
		Assert.Equal(2, width);
	}
}
