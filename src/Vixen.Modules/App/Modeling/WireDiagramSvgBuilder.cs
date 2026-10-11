#nullable enable

using System.Drawing;
using Svg;

namespace VixenModules.App.Modeling;

/// <summary>
/// Builds an in-memory SVG document for an ordered wire diagram.
/// </summary>
public static class WireDiagramSvgBuilder
{
	private const float CanvasMargin = 24f;
	private const float LabelFontSize = 16f;
	private const string LabelFontFamily = "Arial";

	/// <summary>
	/// Creates an SVG document from the ordered nodes without opening or writing a file.
	/// </summary>
	/// <param name="nodes">The ordered node coordinates and displayed labels.</param>
	/// <param name="flip"><see langword="true" /> to reflect node positions horizontally; otherwise, <see langword="false" />.</param>
	/// <param name="options">The validated options associated with this export.</param>
	/// <returns>An in-memory SVG document, or <see langword="null" /> when there are no nodes.</returns>
	/// <exception cref="ArgumentNullException">The nodes or options are <see langword="null" />.</exception>
	/// <exception cref="ArgumentOutOfRangeException">An option is outside its supported range or is not finite.</exception>
	public static SvgDocument? Build(
		IReadOnlyList<Tuple<Point, int>> nodes,
		bool flip,
		WireDiagramExportOptions options)
	{
		ArgumentNullException.ThrowIfNull(nodes);
		ArgumentNullException.ThrowIfNull(options);
		options.Validate();

		if (nodes.Count == 0)
		{
			return null;
		}

		using var measurementBitmap = new Bitmap(1, 1);
		using var measurementGraphics = Graphics.FromImage(measurementBitmap);
		using var labelFont = new Font(LabelFontFamily, LabelFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
		var layout = CalculateLayout(nodes, options.LabelInterval, options.NodeRadius, measurementGraphics, labelFont);
		var document = new SvgDocument
		{
			Width = new SvgUnit(SvgUnitType.Inch, layout.CanvasWidth / 96f * options.ExportScale),
			Height = new SvgUnit(SvgUnitType.Inch, layout.CanvasHeight / 96f * options.ExportScale)
		};
		document.ViewBox = new SvgViewBox(0, 0, layout.CanvasWidth, layout.CanvasHeight);

		var group = new SvgGroup();
		document.Children.Add(group);

		var points = new PointF[nodes.Count];
		for (var index = 0; index < nodes.Count; index++)
		{
			var source = nodes[index].Item1;
			var x = layout.OriginX + ((double)source.X - layout.MinX) * layout.Scale;
			var y = layout.OriginY + ((double)source.Y - layout.MinY) * layout.Scale;
			if (flip)
			{
				x = layout.OriginX + layout.Width * layout.Scale - (x - layout.OriginX);
			}

			points[index] = new PointF((float)x, (float)y);
		}

		for (var index = 0; index < nodes.Count; index++)
		{
			var point = points[index];
			SvgElement marker = index switch
			{
				0 => CreateStartMarker(point, options.NodeRadius),
				_ when index == nodes.Count - 1 => CreateEndMarker(point, options.NodeRadius),
				_ => CreateMiddleMarker(point, options.NodeRadius)
			};
			group.Children.Add(marker);

			if (index == 0 || (index + 1) % options.LabelInterval == 0 || index == nodes.Count - 1)
			{
				group.Children.Add(CreateLabel(nodes[index].Item2, point, options.NodeRadius));
			}
		}

		for (var index = 1; index < points.Length; index++)
		{
			group.Children.Add(CreateLine(points[index - 1], points[index]));
		}

		return document;
	}

	private static Layout CalculateLayout(
		IReadOnlyList<Tuple<Point, int>> nodes,
		int labelInterval,
		float radius,
		Graphics graphics,
		Font font)
	{
		var firstPoint = nodes[0].Item1;
		var minX = firstPoint.X;
		var maxX = firstPoint.X;
		var minY = firstPoint.Y;
		var maxY = firstPoint.Y;
		var labelWidth = 0f;
		for (var index = 0; index < nodes.Count; index++)
		{
			var node = nodes[index];
			minX = Math.Min(minX, node.Item1.X);
			maxX = Math.Max(maxX, node.Item1.X);
			minY = Math.Min(minY, node.Item1.Y);
			maxY = Math.Max(maxY, node.Item1.Y);
			if (index == 0 || (index + 1) % labelInterval == 0 || index == nodes.Count - 1)
			{
				labelWidth = Math.Max(labelWidth, graphics.MeasureString(node.Item2.ToString(), font).Width);
			}
		}

		var width = (double)maxX - minX;
		var height = (double)maxY - minY;
		labelWidth += 2f;
		var family = font.FontFamily;
		var emHeight = family.GetEmHeight(font.Style);
		var ascent = font.Size * family.GetCellAscent(font.Style) / emHeight + 2f;
		var descent = font.Size * family.GetCellDescent(font.Style) / emHeight + 2f;
		var outerRadius = radius * 1.25f;
		var right = Math.Max(outerRadius, radius + labelWidth);
		var top = Math.Max(outerRadius, radius + ascent);
		var bottom = Math.Max(outerRadius, descent - radius);
		var portraitScale = GetScale(816f, 1056f, width, height, outerRadius, right, top, bottom);
		var landscapeScale = GetScale(1056f, 816f, width, height, outerRadius, right, top, bottom);
		var isPortrait = portraitScale >= landscapeScale;
		var canvasWidth = isPortrait ? 816f : 1056f;
		var canvasHeight = isPortrait ? 1056f : 816f;
		var scale = isPortrait ? portraitScale : landscapeScale;
		var originX = CanvasMargin + (canvasWidth - 2 * CanvasMargin - (width * scale + outerRadius + right)) / 2 + outerRadius;
		var originY = CanvasMargin + (canvasHeight - 2 * CanvasMargin - (height * scale + top + bottom)) / 2 + top;
		return new Layout(minX, minY, width, scale, canvasWidth, canvasHeight, originX, originY);
	}

	private static double GetScale(float canvasWidth, float canvasHeight, double width, double height, float left, float right, float top, float bottom)
	{
		var usableWidth = canvasWidth - 2 * CanvasMargin - left - right;
		var usableHeight = canvasHeight - 2 * CanvasMargin - top - bottom;
		var scaleX = width > 0 ? usableWidth / width : double.PositiveInfinity;
		var scaleY = height > 0 ? usableHeight / height : double.PositiveInfinity;
		return width == 0 && height == 0 ? 1 : Math.Min(scaleX, scaleY);
	}

	private sealed record Layout(
		int MinX,
		int MinY,
		double Width,
		double Scale,
		float CanvasWidth,
		float CanvasHeight,
		double OriginX,
		double OriginY);

	private static SvgLine CreateLine(PointF start, PointF end)
	{
		//TODO figure out which way the line is moving
		return new SvgLine
		{
			StartX = start.X,
			StartY = start.Y,
			EndX = end.X,
			EndY = end.Y,
			Fill = new SvgColourServer(Color.Black),
			StrokeWidth = 1,
			Stroke = new SvgColourServer(Color.Black)
		};
	}

	private static SvgText CreateLabel(int order, PointF point, float radius)
	{
		return new SvgText
		{
			Text = order.ToString(),
			Fill = new SvgColourServer(Color.Black),
			FontSize = 16,
			X = { point.X + radius },
			Y = { point.Y - radius }
		};
	}

	private static SvgPolygon CreateStartMarker(PointF point, float radius)
	{
		var outerRadius = radius * 1.25f;
		var innerRadius = outerRadius / 2f;
		var vertices = Enumerable.Range(0, 10)
			.Select(index =>
			{
				var angle = -Math.PI / 2 + index * Math.PI / 5;
				var vertexRadius = index % 2 == 0 ? outerRadius : innerRadius;
				return new PointF(
					point.X + (float)(Math.Cos(angle) * vertexRadius),
					point.Y + (float)(Math.Sin(angle) * vertexRadius));
			})
			.ToArray();
		var points = new SvgPointCollection();
		foreach (var vertex in vertices)
		{
			points.Add(vertex.X);
			points.Add(vertex.Y);
		}

		return new SvgPolygon
		{
			Points = points,
			Fill = new SvgColourServer(Color.Green)
		};
	}

	private static SvgRectangle CreateEndMarker(PointF point, float radius)
	{
		return new SvgRectangle
		{
			X = point.X - radius,
			Y = point.Y - radius,
			Width = radius * 2,
			Height = radius * 2,
			Fill = new SvgColourServer(Color.Red)
		};
	}

	private static SvgCircle CreateMiddleMarker(PointF point, float radius)
	{
		return new SvgCircle
		{
			Radius = radius,
			Fill = new SvgColourServer(Color.Red),
			CenterX = point.X,
			CenterY = point.Y
		};
	}
}
