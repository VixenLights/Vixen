using System;
using VixenModules.App.Modeling;
using Xunit;

namespace Vixen.Tests.App.Modeling;

public sealed class WireDiagramExportOptionsTests
{
	[Fact]
	public void Defaults_AreAllLabelsCurrentRadiusAndNormalScale()
	{
		var options = new WireDiagramExportOptions();

		Assert.Equal(1, options.LabelInterval);
		Assert.Equal(6f, options.NodeRadius);
		Assert.Equal(14f, options.LabelFontSize);
		Assert.Equal(1f, options.ExportScale);
		options.Validate();
	}

	[Theory]
	[InlineData(1)]
	[InlineData(1000)]
	public void Validate_AcceptsLabelIntervalBounds(int labelInterval)
	{
		new WireDiagramExportOptions { LabelInterval = labelInterval }.Validate();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1001)]
	public void Validate_RejectsLabelIntervalOutsideBounds(int labelInterval)
	{
		var options = new WireDiagramExportOptions { LabelInterval = labelInterval };

		Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
	}

	[Fact]
	public void Validate_AcceptsNodeRadiusBounds()
	{
		new WireDiagramExportOptions { NodeRadius = 1f }.Validate();
		new WireDiagramExportOptions { NodeRadius = 30f }.Validate();
	}

	[Theory]
	[InlineData(0f)]
	[InlineData(30.01f)]
	[InlineData(float.NaN)]
	[InlineData(float.PositiveInfinity)]
	public void Validate_RejectsNodeRadiusOutsideBoundsOrNonFinite(float nodeRadius)
	{
		var options = new WireDiagramExportOptions { NodeRadius = nodeRadius };

		Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
	}

	[Theory]
	[InlineData(6f)]
	[InlineData(14f)]
	[InlineData(24f)]
	public void Validate_AcceptsLabelFontSizeBounds(float labelFontSize)
	{
		new WireDiagramExportOptions { LabelFontSize = labelFontSize }.Validate();
	}

	[Theory]
	[InlineData(0f)]
	[InlineData(5f)]
	[InlineData(25f)]
	[InlineData(14.5f)]
	[InlineData(float.NaN)]
	[InlineData(float.PositiveInfinity)]
	[InlineData(float.NegativeInfinity)]
	public void Validate_RejectsLabelFontSizeOutsideBoundsNonWholeOrNonFinite(float labelFontSize)
	{
		var options = new WireDiagramExportOptions { LabelFontSize = labelFontSize };

		Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
	}

	[Fact]
	public void Validate_AcceptsExportScaleBounds()
	{
		new WireDiagramExportOptions { ExportScale = 1f }.Validate();
		new WireDiagramExportOptions { ExportScale = 4f }.Validate();
	}

	[Theory]
	[InlineData(0.99f)]
	[InlineData(4.01f)]
	[InlineData(float.NaN)]
	[InlineData(float.PositiveInfinity)]
	public void Validate_RejectsExportScaleOutsideBoundsOrNonFinite(float exportScale)
	{
		var options = new WireDiagramExportOptions { ExportScale = exportScale };

		Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
	}
}
