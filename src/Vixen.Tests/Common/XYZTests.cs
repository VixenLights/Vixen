using Common.Controls.ColorManagement.ColorModels;
using Xunit;

namespace Vixen.Tests.Common;

public class XYZTests
{
	[Fact]
	public void ClipValue_ValueInRange_ReturnsValue()
	{
		var result = XYZ.ClipValue(50.0, 0.0, 100.0);
		Assert.Equal(50.0, result);
	}

	[Fact]
	public void ClipValue_ValueBelowMin_ReturnsMin()
	{
		var result = XYZ.ClipValue(-1.0, 0.0, 100.0);
		Assert.Equal(0.0, result);
	}

	[Fact]
	public void ClipValue_ValueAboveMax_ReturnsMax()
	{
		var result = XYZ.ClipValue(101.0, 0.0, 100.0);
		Assert.Equal(100.0, result);
	}

	[Fact]
	public void ClipValue_NaN_ReturnsMin()
	{
		var result = XYZ.ClipValue(double.NaN, 0.0, 100.0);
		Assert.Equal(0.0, result);
	}

	[Fact]
	public void ClipValue_NegativeInfinity_ReturnsMin()
	{
		var result = XYZ.ClipValue(double.NegativeInfinity, 0.0, 100.0);
		Assert.Equal(0.0, result);
	}

	[Fact]
	public void ClipValue_PositiveInfinity_ReturnsMax()
	{
		var result = XYZ.ClipValue(double.PositiveInfinity, 0.0, 100.0);
		Assert.Equal(100.0, result);
	}
}