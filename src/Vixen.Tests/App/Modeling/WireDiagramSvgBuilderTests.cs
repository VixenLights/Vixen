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

		Assert.Single(children.OfType<SvgPolygon>());
		Assert.Single(children.OfType<SvgCircle>());
		Assert.Single(children.OfType<SvgRectangle>());
		Assert.Equal(2, children.OfType<SvgLine>().Count());
		Assert.Equal(new[] { "17", "4", "29" }, children.OfType<SvgText>().Select(label => label.Text));
		Assert.Equal(
			new[] { typeof(SvgPolygon), typeof(SvgText), typeof(SvgCircle), typeof(SvgText), typeof(SvgRectangle), typeof(SvgText), typeof(SvgLine), typeof(SvgLine) },
			children.Select(child => child.GetType()));
		var lines = children.OfType<SvgLine>().ToArray();
		var start = Assert.Single(children.OfType<SvgPolygon>());
		var middle = Assert.Single(children.OfType<SvgCircle>());
		var end = Assert.Single(children.OfType<SvgRectangle>());
		Assert.InRange(Math.Abs(start.Points[0].Value - lines[0].StartX.Value), 0f, 0.001f);
		Assert.Equal(start.Points[1].Value + 7.5f, lines[0].StartY.Value);
		Assert.Equal(middle.CenterX.Value, lines[0].EndX.Value);
		Assert.Equal(middle.CenterY.Value, lines[0].EndY.Value);
		Assert.Equal(middle.CenterX.Value, lines[1].StartX.Value);
		Assert.Equal(middle.CenterY.Value, lines[1].StartY.Value);
		Assert.Equal(end.X.Value + end.Width.Value / 2, lines[1].EndX.Value);
		Assert.Equal(end.Y.Value + end.Height.Value / 2, lines[1].EndY.Value);
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
		var frontStart = Assert.IsType<SvgPolygon>(frontGroup.Children.OfType<SvgPolygon>().Single());
		var backStart = Assert.IsType<SvgPolygon>(backGroup.Children.OfType<SvgPolygon>().Single());
		var frontMiddle = Assert.IsType<SvgCircle>(frontGroup.Children.OfType<SvgCircle>().Single());
		var backMiddle = Assert.IsType<SvgCircle>(backGroup.Children.OfType<SvgCircle>().Single());
		var frontEnd = Assert.IsType<SvgRectangle>(frontGroup.Children.OfType<SvgRectangle>().Single());
		var backEnd = Assert.IsType<SvgRectangle>(backGroup.Children.OfType<SvgRectangle>().Single());

		Assert.InRange(Math.Abs(frontStart.Points[0].Value), 0f, 0.001f);
		Assert.InRange(Math.Abs(backStart.Points[0].Value - 800f), 0f, 0.001f);
		Assert.Equal(320f, frontMiddle.CenterX.Value);
		Assert.Equal(480f, backMiddle.CenterX.Value);
		Assert.Equal(794f, frontEnd.X.Value);
		Assert.Equal(-6f, backEnd.X.Value);
		Assert.Equal(new[] { "1", "2", "3" }, backGroup.Children.OfType<SvgText>().Select(label => label.Text));
	}

	[Fact]
	public void Build_UsesStartMarkerForSingleNodeWithoutWireOrEndMarker()
	{
		var nodes = new[] { Tuple.Create(new Point(10, 20), 42) };

		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions());
		var group = Assert.IsType<SvgGroup>(Assert.Single(document!.Children));

		Assert.Single(group.Children.OfType<SvgPolygon>());
		Assert.Empty(group.Children.OfType<SvgCircle>());
		Assert.Empty(group.Children.OfType<SvgRectangle>());
		Assert.Empty(group.Children.OfType<SvgLine>());
		Assert.Equal("42", Assert.Single(group.Children.OfType<SvgText>()).Text);
	}

	[Fact]
	public void Build_ScalesMarkerDimensionsAndLabelOffsetWithRadius()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(10, 20), 1),
			Tuple.Create(new Point(30, 30), 2),
			Tuple.Create(new Point(60, 50), 3)
		};
		var standard = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;
		var doubled = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions { NodeRadius = 12 })!;
		var standardGroup = Assert.IsType<SvgGroup>(Assert.Single(standard.Children));
		var doubledGroup = Assert.IsType<SvgGroup>(Assert.Single(doubled.Children));
		var standardCircle = Assert.IsType<SvgCircle>(standardGroup.Children.OfType<SvgCircle>().Single());
		var doubledCircle = Assert.IsType<SvgCircle>(doubledGroup.Children.OfType<SvgCircle>().Single());
		var standardSquare = Assert.IsType<SvgRectangle>(standardGroup.Children.OfType<SvgRectangle>().Single());
		var doubledSquare = Assert.IsType<SvgRectangle>(doubledGroup.Children.OfType<SvgRectangle>().Single());
		var standardMiddleLabel = standardGroup.Children.OfType<SvgText>().ElementAt(1);
		var doubledMiddleLabel = doubledGroup.Children.OfType<SvgText>().ElementAt(1);

		Assert.Equal(12f, doubledCircle.Radius.Value);
		Assert.Equal(standardCircle.Radius.Value * 2, doubledCircle.Radius.Value);
		Assert.Equal(12f, standardSquare.Width.Value);
		Assert.Equal(24f, doubledSquare.Width.Value);
		Assert.Equal(Color.Green, Assert.IsType<SvgColourServer>(Assert.Single(standardGroup.Children.OfType<SvgPolygon>()).Fill).Colour);
		Assert.Equal(Color.Red, Assert.IsType<SvgColourServer>(standardCircle.Fill).Colour);
		Assert.Equal(Color.Red, Assert.IsType<SvgColourServer>(standardSquare.Fill).Colour);
		Assert.Equal(standardCircle.CenterX.Value + standardCircle.Radius.Value, standardMiddleLabel.X[0].Value);
		Assert.Equal(doubledCircle.CenterX.Value + doubledCircle.Radius.Value, doubledMiddleLabel.X[0].Value);
		Assert.Equal(7.5f, Math.Abs(Assert.Single(standardGroup.Children.OfType<SvgPolygon>()).Points[1].Value));
		Assert.Equal(15f, Math.Abs(Assert.Single(doubledGroup.Children.OfType<SvgPolygon>()).Points[1].Value));
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
		Assert.Single(children.OfType<SvgPolygon>());
		Assert.Equal(Math.Max(count - 2, 0), children.OfType<SvgCircle>().Count());
		Assert.Equal(count > 1 ? 1 : 0, children.OfType<SvgRectangle>().Count());
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
		Assert.Single(group.Children.OfType<SvgPolygon>());
		Assert.Equal(3, group.Children.OfType<SvgCircle>().Count());
		Assert.Single(group.Children.OfType<SvgRectangle>());
		Assert.Equal(4, group.Children.OfType<SvgLine>().Count());
	}

}
