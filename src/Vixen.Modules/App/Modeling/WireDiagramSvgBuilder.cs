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

		var points = nodes.Select(node => node.Item1);
		var rect = CalculateModelRectangle(points);
		var scale = 1f;
		if (rect.Width < 800)
		{
			scale = 800f / rect.Width;
		}

		var document = new SvgDocument { Width = rect.Width * scale, Height = rect.Height * scale };
		document.ViewBox = new SvgViewBox(-20, -20, document.Width + ExtraWidth, document.Height + ExtraHeight);

		var group = new SvgGroup();
		document.Children.Add(group);

		var lastPoint = new PointF();
		var first = true;
		const int radius = 6;

		for (var index = 0; index < nodes.Count; index++)
		{
			var node = nodes[index];
			var point = ScalePoint(node.Item1, scale, rect.Location);
			if (flip)
			{
				point = new PointF(document.ViewBox.Width - point.X - ExtraWidth, point.Y);
			}

			group.Children.Add(CreateLightNode(point, radius));
			if (index == 0 || (index + 1) % options.LabelInterval == 0 || index == nodes.Count - 1)
			{
				group.Children.Add(CreateLabel(node.Item2, point, radius));
			}

			if (!first)
			{
				group.Children.Add(CreateLine(lastPoint, point));
			}

			first = false;
			lastPoint = point;
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

	private static SvgText CreateLabel(int order, PointF point, int radius)
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

	private static SvgCircle CreateLightNode(PointF point, int radius)
	{
		return new SvgCircle
		{
			Radius = radius,
			Fill = new SvgColourServer(Color.Red),
			//Stroke = new SvgColourServer(Color.Black),
			//StrokeWidth = 1,
			CenterX = point.X,
			CenterY = point.Y
		};
	}
}
