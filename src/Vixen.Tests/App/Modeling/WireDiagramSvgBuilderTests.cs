using System.Drawing;
using System.Linq;
using Svg;
using VixenModules.App.Modeling;
using Xunit;

namespace Vixen.Tests.App.Modeling;

public sealed class WireDiagramSvgBuilderTests
{
	[Fact]
	public void Build_ReturnsNullForEmptyInput()
	{
		var document = WireDiagramSvgBuilder.Build([], false, new WireDiagramExportOptions());

		Assert.Null(document);
	}

	[Fact]
	public void Build_ThrowsForNullNodesOrOptions()
	{
		Assert.Throws<ArgumentNullException>(() => WireDiagramSvgBuilder.Build(null!, false, new WireDiagramExportOptions()));
		Assert.Throws<ArgumentNullException>(() => WireDiagramSvgBuilder.Build([], false, null!));
	}

	[Fact]
	public void Build_PreservesLabelsMarkersLinksAndInputOrder()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(10, 20), 17),
			Tuple.Create(new Point(30, 30), 4),
			Tuple.Create(new Point(60, 50), 29)
		};

		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions());
		var group = Assert.IsType<SvgGroup>(Assert.Single(document!.Children));
		var children = group.Children.ToArray();

		Assert.Equal(3, children.OfType<SvgCircle>().Count());
		Assert.Equal(2, children.OfType<SvgLine>().Count());
		Assert.Equal(new[] { "17", "4", "29" }, children.OfType<SvgText>().Select(label => label.Text));
		Assert.Equal(
			new[] { typeof(SvgCircle), typeof(SvgText), typeof(SvgCircle), typeof(SvgText), typeof(SvgLine), typeof(SvgCircle), typeof(SvgText), typeof(SvgLine) },
			children.Select(child => child.GetType()));
	}

	[Fact]
	public void Build_FlipReversesHorizontalNodePositionsWithoutReorderingLabels()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(10, 20), 1),
			Tuple.Create(new Point(30, 30), 2),
			Tuple.Create(new Point(60, 50), 3)
		};
		var options = new WireDiagramExportOptions();
		var front = WireDiagramSvgBuilder.Build(nodes, false, options)!;
		var back = WireDiagramSvgBuilder.Build(nodes, true, options)!;
		var frontGroup = Assert.IsType<SvgGroup>(Assert.Single(front.Children));
		var backGroup = Assert.IsType<SvgGroup>(Assert.Single(back.Children));
		var frontCenters = frontGroup.Children.OfType<SvgCircle>().Select(circle => circle.CenterX.Value).ToArray();
		var backCenters = backGroup.Children.OfType<SvgCircle>().Select(circle => circle.CenterX.Value).ToArray();

		Assert.Equal(new[] { 0f, 320f, 800f }, frontCenters);
		Assert.Equal(new[] { 800f, 480f, 0f }, backCenters);
		Assert.Equal(new[] { "1", "2", "3" }, backGroup.Children.OfType<SvgText>().Select(label => label.Text));
	}

	[Theory]
	[InlineData(123, 50, new[] { "1", "50", "100", "123" })]
	[InlineData(100, 50, new[] { "1", "50", "100" })]
	[InlineData(5, 1, new[] { "1", "2", "3", "4", "5" })]
	[InlineData(5, 10, new[] { "1", "5" })]
	[InlineData(1, 50, new[] { "1" })]
	public void Build_FiltersLabelsBySequencePosition(int count, int labelInterval, string[] expectedLabels)
	{
		var nodes = Enumerable.Range(1, count)
			.Select(index => Tuple.Create(new Point(index, index), index))
			.ToArray();
		var options = new WireDiagramExportOptions { LabelInterval = labelInterval };

		var document = WireDiagramSvgBuilder.Build(nodes, false, options);
		var group = Assert.IsType<SvgGroup>(Assert.Single(document!.Children));
		var children = group.Children.ToArray();

		Assert.Equal(expectedLabels, children.OfType<SvgText>().Select(label => label.Text));
		Assert.Equal(count, children.OfType<SvgCircle>().Count());
		Assert.Equal(count - 1, children.OfType<SvgLine>().Count());
	}

	[Fact]
	public void Build_FiltersRepeatedNonsequentialLabelsByPosition()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(1, 1), 8),
			Tuple.Create(new Point(2, 2), 8),
			Tuple.Create(new Point(3, 3), 4),
			Tuple.Create(new Point(4, 4), 4),
			Tuple.Create(new Point(5, 5), 8)
		};
		var options = new WireDiagramExportOptions { LabelInterval = 3 };

		var document = WireDiagramSvgBuilder.Build(nodes, false, options);
		var group = Assert.IsType<SvgGroup>(Assert.Single(document!.Children));

		Assert.Equal(new[] { "8", "4", "8" }, group.Children.OfType<SvgText>().Select(label => label.Text));
		Assert.Equal(5, group.Children.OfType<SvgCircle>().Count());
		Assert.Equal(4, group.Children.OfType<SvgLine>().Count());
	}
}
