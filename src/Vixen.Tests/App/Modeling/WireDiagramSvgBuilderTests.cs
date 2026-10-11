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

		Assert.Equal(front.ViewBox.Width, back.ViewBox.Width);
		var mirroredAxis = frontStart.Points[0].Value + backStart.Points[0].Value;
		Assert.Equal(mirroredAxis, frontMiddle.CenterX.Value + backMiddle.CenterX.Value, 0.001f);
		Assert.Equal(mirroredAxis, frontEnd.X.Value + backEnd.X.Value + frontEnd.Width.Value, 0.001f);
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
		var standardStar = Assert.Single(standardGroup.Children.OfType<SvgPolygon>());
		var doubledStar = Assert.Single(doubledGroup.Children.OfType<SvgPolygon>());
		var standardCenterY = standardStar.Points.Where((_, index) => index % 2 == 1).Average(point => point.Value);
		var doubledCenterY = doubledStar.Points.Where((_, index) => index % 2 == 1).Average(point => point.Value);
		Assert.Equal(7.5f, Math.Abs(standardStar.Points[1].Value - standardCenterY), 0.001f);
		Assert.Equal(15f, Math.Abs(doubledStar.Points[1].Value - doubledCenterY), 0.001f);
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
		var expectedDiamondCount = labelInterval > 1
			? Enumerable.Range(1, Math.Max(count - 2, 0)).Count(index => (index + 1) % labelInterval == 0)
			: 0;
		var polygons = children.OfType<SvgPolygon>().ToArray();
		Assert.Single(polygons, polygon => polygon.Points.Count == 20);
		Assert.Equal(expectedDiamondCount, polygons.Count(polygon => polygon.Points.Count == 8));
		Assert.Equal(expectedDiamondCount + 1, polygons.Length);
		Assert.Equal(Math.Max(count - 2, 0) - expectedDiamondCount, children.OfType<SvgCircle>().Count());
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
		var polygons = group.Children.OfType<SvgPolygon>().ToArray();
		Assert.Single(polygons, polygon => polygon.Points.Count == 20);
		Assert.Single(polygons, polygon => polygon.Points.Count == 8);
		Assert.Equal(2, group.Children.OfType<SvgCircle>().Count());
		Assert.Single(group.Children.OfType<SvgRectangle>());
		Assert.Equal(4, group.Children.OfType<SvgLine>().Count());
	}

	[Fact]
	public void Build_UsesDiamondsAtSelectedInteriorPositionsInIntervalMode()
	{
		var nodes = Enumerable.Range(1, 123)
			.Select(index => Tuple.Create(new Point(index, index), index))
			.ToArray();
		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions { LabelInterval = 50 })!;
		var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
		var polygons = group.Children.OfType<SvgPolygon>().ToArray();
		var star = Assert.Single(polygons, polygon => polygon.Points.Count == 20);
		var diamonds = polygons.Where(polygon => polygon.Points.Count == 8).ToArray();
		var labels = group.Children.OfType<SvgText>().ToArray();

		Assert.Equal(2, diamonds.Length);
		Assert.Equal(119, group.Children.OfType<SvgCircle>().Count());
		Assert.Single(group.Children.OfType<SvgRectangle>());
		Assert.Equal(122, group.Children.OfType<SvgLine>().Count());
		Assert.Equal(new[] { "1", "50", "100", "123" }, labels.Select(label => label.Text));
		Assert.Equal(Color.Green, Assert.IsType<SvgColourServer>(star.Fill).Colour);
		Assert.Equal(Color.Red, Assert.IsType<SvgColourServer>(Assert.Single(group.Children.OfType<SvgRectangle>()).Fill).Colour);

		foreach (var label in labels.Where(label => label.Text is "50" or "100"))
		{
			var expectedCenter = (X: label.X[0].Value - 6f, Y: label.Y[0].Value + 6f);
			Assert.Contains(diamonds, diamond =>
			{
				var center = GetPolygonCenter(diamond);
				return Math.Abs(center.X - expectedCenter.X) < 0.001f && Math.Abs(center.Y - expectedCenter.Y) < 0.001f;
			});
		}
	}

	[Fact]
	public void Build_IntervalOneKeepsAllInteriorNodesAsCircles()
	{
		var nodes = Enumerable.Range(1, 123)
			.Select(index => Tuple.Create(new Point(index, index), index))
			.ToArray();
		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions { LabelInterval = 1 })!;
		var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
		var polygons = group.Children.OfType<SvgPolygon>().ToArray();

		Assert.Single(polygons, polygon => polygon.Points.Count == 20);
		Assert.DoesNotContain(polygons, polygon => polygon.Points.Count == 8);
		Assert.Equal(121, group.Children.OfType<SvgCircle>().Count());
		Assert.Single(group.Children.OfType<SvgRectangle>());
		Assert.Equal(122, group.Children.OfType<SvgLine>().Count());
		Assert.Equal(123, group.Children.OfType<SvgText>().Count());
	}

	[Fact]
	public void Build_DiamondVerticesUseSelectedRadius()
	{
		var nodes = Enumerable.Range(1, 5)
			.Select(index => Tuple.Create(new Point(index * 10, index * 20), index))
			.ToArray();
		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions { LabelInterval = 2, NodeRadius = 9f })!;
		var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
		var diamonds = group.Children.OfType<SvgPolygon>().Where(polygon => polygon.Points.Count == 8).ToArray();

		Assert.Equal(2, diamonds.Length);
		foreach (var diamond in diamonds)
		{
			var points = diamond.Points.Select(point => point.Value).ToArray();
			var center = GetPolygonCenter(diamond);
			var expectedPoints = new[]
			{
				center.X, center.Y - 9f,
				center.X + 9f, center.Y,
				center.X, center.Y + 9f,
				center.X - 9f, center.Y
			};
			Assert.Equal(expectedPoints.Length, points.Length);
			for (var index = 0; index < points.Length; index++)
			{
				Assert.InRange(Math.Abs(points[index] - expectedPoints[index]), 0f, 0.001f);
			}
		}
	}

	[Fact]
	public void Build_FlipMirrorsDiamondCentersAndPreservesLabelText()
	{
		var nodes = Enumerable.Range(1, 5)
			.Select(index => Tuple.Create(new Point(index * 10, index * 20), index * 3))
			.ToArray();
		var options = new WireDiagramExportOptions { LabelInterval = 2 };
		var front = WireDiagramSvgBuilder.Build(nodes, false, options)!;
		var back = WireDiagramSvgBuilder.Build(nodes, true, options)!;
		var frontGroup = Assert.IsType<SvgGroup>(Assert.Single(front.Children));
		var backGroup = Assert.IsType<SvgGroup>(Assert.Single(back.Children));
		var frontStar = GetPolygonCenter(Assert.Single(frontGroup.Children.OfType<SvgPolygon>(), polygon => polygon.Points.Count == 20));
		var backStar = GetPolygonCenter(Assert.Single(backGroup.Children.OfType<SvgPolygon>(), polygon => polygon.Points.Count == 20));
		var frontDiamonds = frontGroup.Children.OfType<SvgPolygon>().Where(polygon => polygon.Points.Count == 8).Select(GetPolygonCenter).ToArray();
		var backDiamonds = backGroup.Children.OfType<SvgPolygon>().Where(polygon => polygon.Points.Count == 8).Select(GetPolygonCenter).ToArray();

		Assert.Equal(2, frontDiamonds.Length);
		Assert.Equal(frontDiamonds.Length, backDiamonds.Length);
		for (var index = 0; index < frontDiamonds.Length; index++)
		{
			Assert.Equal(frontStar.X + backStar.X, frontDiamonds[index].X + backDiamonds[index].X, 0.001f);
			Assert.Equal(frontDiamonds[index].Y, backDiamonds[index].Y, 0.001f);
		}
		Assert.Equal(frontGroup.Children.OfType<SvgText>().Select(label => label.Text), backGroup.Children.OfType<SvgText>().Select(label => label.Text));
	}

	[Theory]
	[InlineData(0, 0, 100, 1000, 816f, 1056f)]
	[InlineData(0, 0, 1000, 100, 1056f, 816f)]
	[InlineData(0, 0, 100, 100, 1056f, 816f)]
	public void Build_SelectsOrientationAndViewBoxFromModelExtents(int x, int y, int width, int height, float expectedWidth, float expectedHeight)
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(x, y), 1),
			Tuple.Create(new Point(x + width, y + height), 2)
		};

		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;

		Assert.Equal(expectedWidth, document.ViewBox.Width);
		Assert.Equal(expectedHeight, document.ViewBox.Height);
		Assert.Equal(expectedWidth / 96f, document.Width.Value);
		Assert.Equal(expectedHeight / 96f, document.Height.Value);
		Assert.Equal(SvgUnitType.Inch, document.Width.Type);
		Assert.Equal(SvgUnitType.Inch, document.Height.Type);
	}

	[Fact]
	public void Build_AppliesExportScaleOnlyToSerializedPhysicalDimensions()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(10, 20), 1),
			Tuple.Create(new Point(30, 1020), 2)
		};
		var standard = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;
		var enlarged = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions { ExportScale = 2f })!;

		Assert.Equal(816f, standard.ViewBox.Width);
		Assert.Equal(1056f, standard.ViewBox.Height);
		Assert.Equal(standard.ViewBox.Width, enlarged.ViewBox.Width);
		Assert.Equal(standard.ViewBox.Height, enlarged.ViewBox.Height);
		Assert.Equal(standard.Width.Value * 2, enlarged.Width.Value);
		Assert.Equal(standard.Height.Value * 2, enlarged.Height.Value);
		Assert.Equal(SvgUnitType.Inch, enlarged.Width.Type);
		Assert.Equal(SvgUnitType.Inch, enlarged.Height.Type);
		var standardGroup = Assert.IsType<SvgGroup>(Assert.Single(standard.Children));
		var enlargedGroup = Assert.IsType<SvgGroup>(Assert.Single(enlarged.Children));
		Assert.Equal(standardGroup.Children.Select(element => element.ToString()), enlargedGroup.Children.Select(element => element.ToString()));

		using var standardStream = new MemoryStream();
		using var enlargedStream = new MemoryStream();
		standard.Write(standardStream);
		enlarged.Write(enlargedStream);
		var standardSvg = System.Text.Encoding.UTF8.GetString(standardStream.ToArray());
		var enlargedSvg = System.Text.Encoding.UTF8.GetString(enlargedStream.ToArray());
		Assert.Contains("width=\"8.5in\"", standardSvg);
		Assert.Contains("height=\"11in\"", standardSvg);
		Assert.Contains("width=\"17in\"", enlargedSvg);
		Assert.Contains("height=\"22in\"", enlargedSvg);
	}

	[Fact]
	public void Build_UsesPortraitWhenOrientationScalesTie()
	{
		var nodes = new[] { Tuple.Create(new Point(10, 20), 42) };

		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;

		Assert.Equal(816f, document.ViewBox.Width);
		Assert.Equal(1056f, document.ViewBox.Height);
	}

	[Fact]
	public void Build_UsesOneUniformScaleForCoordinateDifferences()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(-10, -20), 1),
			Tuple.Create(new Point(10, 20), 2),
			Tuple.Create(new Point(30, 60), 3)
		};
		var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;
		var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
		var centers = GetMarkerCenters(group);

		Assert.Equal((centers[1].X - centers[0].X) / 20f, (centers[2].X - centers[1].X) / 20f, 0.001f);
		Assert.Equal((centers[1].Y - centers[0].Y) / 40f, (centers[2].Y - centers[1].Y) / 40f, 0.001f);
		Assert.Equal((centers[1].X - centers[0].X) / 20f, (centers[1].Y - centers[0].Y) / 40f, 0.001f);
	}

	[Fact]
	public void Build_HandlesDegenerateAndExtremeCoordinatesWithoutOverflow()
	{
		var cases = new[]
		{
			new[] { new Point(-100, -50), new Point(100, -50) },
			new[] { new Point(-50, -100), new Point(-50, 100) },
			new[] { new Point(-10, 20) },
			new[] { new Point(5, 5), new Point(5, 5), new Point(5, 5) },
			new[] { new Point(int.MinValue, int.MinValue), new Point(int.MaxValue, int.MaxValue) }
		};

		foreach (var points in cases)
		{
			var nodes = points.Select((point, index) => Tuple.Create(point, index + 1)).ToArray();
			var document = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;
			var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
			foreach (var element in group.Children)
			{
				switch (element)
				{
					case SvgCircle circle:
						Assert.True(float.IsFinite(circle.CenterX.Value) && float.IsFinite(circle.CenterY.Value));
						break;
					case SvgRectangle rectangle:
						Assert.True(float.IsFinite(rectangle.X.Value) && float.IsFinite(rectangle.Y.Value));
						break;
					case SvgPolygon polygon:
						Assert.All(polygon.Points, coordinate => Assert.True(float.IsFinite(coordinate.Value)));
						break;
					case SvgText label:
						Assert.True(float.IsFinite(label.X[0].Value) && float.IsFinite(label.Y[0].Value));
						break;
					case SvgLine line:
						Assert.True(float.IsFinite(line.StartX.Value) && float.IsFinite(line.StartY.Value));
						Assert.True(float.IsFinite(line.EndX.Value) && float.IsFinite(line.EndY.Value));
						break;
				}
			}
		}
	}

	[Theory]
	[InlineData(6f)]
	[InlineData(14f)]
	[InlineData(24f)]
	public void Build_KeepsLongEndpointLabelAndMarkersInsideCanvasMargins(float labelFontSize)
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(-1000, -1000), int.MinValue),
			Tuple.Create(new Point(1000, 1000), int.MaxValue)
		};
		var options = new WireDiagramExportOptions { LabelFontSize = labelFontSize };
		var document = WireDiagramSvgBuilder.Build(nodes, false, options)!;
		var group = Assert.IsType<SvgGroup>(Assert.Single(document.Children));
		var start = Assert.IsType<SvgPolygon>(group.Children.OfType<SvgPolygon>().Single());
		var end = Assert.IsType<SvgRectangle>(group.Children.OfType<SvgRectangle>().Single());
		var labels = group.Children.OfType<SvgText>().ToArray();
		var points = start.Points.Chunk(2).Select(pair => (X: pair[0].Value, Y: pair[1].Value)).ToArray();

		Assert.True(points.Min(point => point.X) >= 23.9f);
		Assert.True(points.Min(point => point.Y) >= 23.9f);
		Assert.True(end.X.Value + end.Width.Value <= document.ViewBox.Width - 23.9f);
		Assert.True(end.Y.Value + end.Height.Value <= document.ViewBox.Height - 23.9f);
		using var measurementBitmap = new System.Drawing.Bitmap(1, 1);
		using var graphics = System.Drawing.Graphics.FromImage(measurementBitmap);
		using var font = new System.Drawing.Font("Arial", labelFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		var emHeight = font.FontFamily.GetEmHeight(font.Style);
		var ascent = font.Size * font.FontFamily.GetCellAscent(font.Style) / emHeight + 2f;
		var descent = font.Size * font.FontFamily.GetCellDescent(font.Style) / emHeight + 2f;
		foreach (var label in labels)
		{
			Assert.Equal(labelFontSize, label.FontSize.Value);
			var labelWidth = graphics.MeasureString(label.Text, font).Width + 2f;
			Assert.True(label.X[0].Value >= 24f);
			Assert.True(label.X[0].Value + labelWidth <= document.ViewBox.Width - 24f);
			Assert.True(label.Y[0].Value - ascent >= 24f);
			Assert.True(label.Y[0].Value + descent <= document.ViewBox.Height - 24f);
		}
		Assert.Equal(new[] { int.MinValue.ToString(), int.MaxValue.ToString() }, labels.Select(label => label.Text));
	}

	[Fact]
	public void Build_FlipMirrorsMarkerCentersAndPreservesLabelText()
	{
		var nodes = new[]
		{
			Tuple.Create(new Point(-20, 10), 100),
			Tuple.Create(new Point(30, 20), 200),
			Tuple.Create(new Point(80, 40), 300)
		};
		var front = WireDiagramSvgBuilder.Build(nodes, false, new WireDiagramExportOptions())!;
		var back = WireDiagramSvgBuilder.Build(nodes, true, new WireDiagramExportOptions())!;
		var frontGroup = Assert.IsType<SvgGroup>(Assert.Single(front.Children));
		var backGroup = Assert.IsType<SvgGroup>(Assert.Single(back.Children));
		var frontCenters = GetMarkerCenters(frontGroup);
		var backCenters = GetMarkerCenters(backGroup);

		Assert.Equal(front.ViewBox.Width, back.ViewBox.Width);
		for (var index = 0; index < frontCenters.Length; index++)
		{
			Assert.Equal(frontCenters[0].X + backCenters[0].X, frontCenters[index].X + backCenters[index].X, 0.01f);
			Assert.Equal(frontCenters[index].Y, backCenters[index].Y, 0.01f);
		}
		Assert.Equal(frontGroup.Children.OfType<SvgText>().Select(label => label.Text), backGroup.Children.OfType<SvgText>().Select(label => label.Text));
	}

	private static (float X, float Y) GetPolygonCenter(SvgPolygon polygon)
	{
		return (
			polygon.Points.Where((_, index) => index % 2 == 0).Average(point => point.Value),
			polygon.Points.Where((_, index) => index % 2 == 1).Average(point => point.Value));
	}

	private static (float X, float Y)[] GetMarkerCenters(SvgGroup group)
	{
		var centers = new List<(float X, float Y)>();
		centers.Add((group.Children.OfType<SvgPolygon>().Single().Points.Where((_, index) => index % 2 == 0).Average(point => point.Value),
			group.Children.OfType<SvgPolygon>().Single().Points.Where((_, index) => index % 2 == 1).Average(point => point.Value)));
		centers.AddRange(group.Children.OfType<SvgCircle>().Select(circle => (circle.CenterX.Value, circle.CenterY.Value)));
		centers.AddRange(group.Children.OfType<SvgRectangle>().Select(rectangle => (rectangle.X.Value + rectangle.Width.Value / 2, rectangle.Y.Value + rectangle.Height.Value / 2)));
		return centers.ToArray();
	}

}
