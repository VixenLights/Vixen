#nullable enable

using System.Drawing;
using Svg;

namespace VixenModules.App.Modeling;

/// <summary>
/// Builds an in-memory SVG document for an ordered wire diagram.
/// </summary>
public static class WireDiagramSvgBuilder
{
	private const int ExtraHeight = 50;
	private const int ExtraWidth = 50;

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

		var nodePoints = nodes.Select(node => node.Item1);
		var rect = CalculateModelRectangle(nodePoints);
		var scale = 1f;
		if (rect.Width < 800)
		{
			scale = 800f / rect.Width;
		}

		var document = new SvgDocument { Width = rect.Width * scale, Height = rect.Height * scale };
		document.ViewBox = new SvgViewBox(-20, -20, document.Width + ExtraWidth, document.Height + ExtraHeight);

		var group = new SvgGroup();
		document.Children.Add(group);

		var points = new PointF[nodes.Count];
		for (var index = 0; index < nodes.Count; index++)
		{
			var point = ScalePoint(nodes[index].Item1, scale, rect.Location);
			points[index] = flip
				? new PointF(document.ViewBox.Width - point.X - ExtraWidth, point.Y)
				: point;
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

	private static PointF ScalePoint(Point point, float scale, Point offset)
	{
		return new PointF((point.X - offset.X) * scale, (point.Y - offset.Y) * scale);
	}

	private static Rectangle CalculateModelRectangle(IEnumerable<Point> points)
	{
		using var enumerator = points.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			throw new InvalidOperationException("Sequence contains no elements.");
		}

		var firstPoint = enumerator.Current;
		var xMin = firstPoint.X;
		var xMax = firstPoint.X;
		var yMin = firstPoint.Y;
		var yMax = firstPoint.Y;
		while (enumerator.MoveNext())
		{
			var point = enumerator.Current;
			xMin = Math.Min(xMin, point.X);
			xMax = Math.Max(xMax, point.X);
			yMin = Math.Min(yMin, point.Y);
			yMax = Math.Max(yMax, point.Y);
		}

		var width = xMax - xMin;
		var height = yMax - yMin;
		return new Rectangle(xMin, yMin, width > 0 ? width : 50, height > 0 ? height : 50);
	}

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
