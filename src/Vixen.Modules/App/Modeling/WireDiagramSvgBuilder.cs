#nullable enable

using System.Drawing;
using Svg;
using Svg.Transforms;

namespace VixenModules.App.Modeling;

/// <summary>
/// Builds an in-memory SVG document for an ordered wire diagram.
/// </summary>
public static class WireDiagramSvgBuilder
{
	private const float CanvasMargin = 24f;
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
		using var labelFont = new Font(LabelFontFamily, options.LabelFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
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
				_ when options.LabelInterval > 1 && IsLabelSelected(index, nodes.Count, options.LabelInterval) => CreateDiamondMarker(point, options.NodeRadius),
				_ => CreateMiddleMarker(point, options.NodeRadius)
			};
			group.Children.Add(marker);
		}

		for (var index = 1; index < points.Length; index++)
		{
			group.Children.Add(CreateLine(points[index - 1], points[index]));
		}

		// Labels are deliberately emitted last so neither later markers nor wires obscure them.
		var labels = MeasureLabels(nodes, options.LabelInterval, measurementGraphics, labelFont);
		var placedLabels = new List<(LabelPlacement Placement, LabelMeasurement Measurement)>();
		foreach (var label in labels)
		{
			var placement = PlaceLabel(points[label.Index], label, points, placedLabels, options.NodeRadius, layout.CanvasWidth, layout.CanvasHeight);
			group.Children.Add(CreateLabel(label.Text, placement, label, labelFont.Size));
			placedLabels.Add((placement, label));
		}

		return document;
	}

	private static LabelMeasurement[] MeasureLabels(
		IReadOnlyList<Tuple<Point, int>> nodes,
		int labelInterval,
		Graphics graphics,
		Font font)
	{
		var family = font.FontFamily;
		var emHeight = family.GetEmHeight(font.Style);
		var ascent = font.Size * family.GetCellAscent(font.Style) / emHeight + 2f;
		var descent = font.Size * family.GetCellDescent(font.Style) / emHeight + 2f;
		return Enumerable.Range(0, nodes.Count)
			.Where(index => IsLabelSelected(index, nodes.Count, labelInterval))
			.Select(index =>
			{
				var text = nodes[index].Item2.ToString();
				return new LabelMeasurement(index, text, graphics.MeasureString(text, font).Width + 2f, ascent, descent);
			})
			.ToArray();
	}

	private static LabelPlacement PlaceLabel(
		PointF point,
		LabelMeasurement label,
		PointF[] points,
		IReadOnlyList<(LabelPlacement Placement, LabelMeasurement Measurement)> placedLabels,
		float radius,
		float canvasWidth,
		float canvasHeight)
	{
		const float gap = 2f;
		const float clearance = 2.5f;
		var halfWidth = label.Width / 2f;
		var halfHeight = (label.Ascent + label.Descent) / 2f;
		var directions = new (float X, float Y)[]
		{
			(1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1)
		};
		var candidates = new List<LabelPlacement>();
		var ordinal = 0;
		foreach (var direction in directions)
		{
			var length = MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
			var unitX = direction.X / length;
			var unitY = direction.Y / length;
			var projectedHalfSize = MathF.Abs(unitX) * halfWidth + MathF.Abs(unitY) * halfHeight;
			foreach (var gapMultiplier in new[] { 1f, 2f })
			{
				var distance = radius + projectedHalfSize + gap * gapMultiplier;
				candidates.Add(new LabelPlacement(point.X + unitX * distance, point.Y + unitY * distance, 0, ordinal++));
			}
		}

		var wires = Enumerable.Range(1, Math.Max(0, points.Length - 1)).Select(index => (points[index - 1], points[index])).ToArray();
		var inBounds = candidates.Where(candidate => IsInsidePage(candidate, label, canvasWidth, canvasHeight, clearance)).ToArray();
		var clear = inBounds.Where(candidate => CountIntersections(candidate, label, wires, clearance) == 0).ToArray();
		var best = clear.Length > 0
			? clear.OrderBy(candidate => CalculateLabelOverlapArea(candidate, label, placedLabels)).ThenBy(candidate => GetLabelStartDistance(candidate, label, point)).ThenBy(candidate => candidate.Ordinal).First()
			: inBounds.OrderBy(candidate => CountIntersections(candidate, label, wires, clearance)).ThenBy(candidate => CalculateLabelOverlapArea(candidate, label, placedLabels)).ThenBy(candidate => GetLabelStartDistance(candidate, label, point)).ThenBy(candidate => candidate.Ordinal).FirstOrDefault();
		return inBounds.Length == 0
			? candidates.OrderBy(candidate => GetLabelStartDistance(candidate, label, point)).ThenBy(candidate => candidate.Ordinal).First()
			: best!;
	}

	private static float GetLabelStartDistance(LabelPlacement placement, LabelMeasurement label, PointF point)
	{
		var deltaX = placement.CenterX - label.Width / 2f - point.X;
		var deltaY = placement.CenterY - point.Y;
		return MathF.Sqrt(deltaX * deltaX + deltaY * deltaY);
	}

	private static float CalculateLabelOverlapArea(
		LabelPlacement placement,
		LabelMeasurement label,
		IReadOnlyList<(LabelPlacement Placement, LabelMeasurement Measurement)> placedLabels)
	{
		const float clearance = 2f;
		var bounds = RectangleF.Inflate(GetLabelBounds(placement, label), clearance, clearance);
		var overlapArea = 0f;
		foreach (var placedLabel in placedLabels)
		{
			var placedBounds = RectangleF.Inflate(GetLabelBounds(placedLabel.Placement, placedLabel.Measurement), clearance, clearance);
			var intersection = RectangleF.Intersect(bounds, placedBounds);
			overlapArea += Math.Max(0, intersection.Width) * Math.Max(0, intersection.Height);
		}

		return overlapArea;
	}

	private static RectangleF GetLabelBounds(LabelPlacement placement, LabelMeasurement label)
	{
		var radians = placement.Angle * MathF.PI / 180f;
		var halfWidth = label.Width / 2f;
		var halfHeight = (label.Ascent + label.Descent) / 2f;
		var absoluteCosine = MathF.Abs(MathF.Cos(radians));
		var absoluteSine = MathF.Abs(MathF.Sin(radians));
		var width = 2 * (absoluteCosine * halfWidth + absoluteSine * halfHeight);
		var height = 2 * (absoluteSine * halfWidth + absoluteCosine * halfHeight);
		return new RectangleF(placement.CenterX - width / 2f, placement.CenterY - height / 2f, width, height);
	}

	private static bool IsInsidePage(LabelPlacement placement, LabelMeasurement label, float width, float height, float margin)
	{
		var halfWidth = label.Width / 2f;
		var halfHeight = (label.Ascent + label.Descent) / 2f;
		var radians = placement.Angle * MathF.PI / 180f;
		var extentX = Math.Abs(MathF.Cos(radians)) * halfWidth + Math.Abs(MathF.Sin(radians)) * halfHeight;
		var extentY = Math.Abs(MathF.Sin(radians)) * halfWidth + Math.Abs(MathF.Cos(radians)) * halfHeight;
		return placement.CenterX - extentX >= margin && placement.CenterX + extentX <= width - margin &&
			placement.CenterY - extentY >= margin && placement.CenterY + extentY <= height - margin;
	}

	private static int CountIntersections(
		LabelPlacement placement,
		LabelMeasurement label,
		(PointF Start, PointF End)[] wires,
		float clearance)
	{
		var count = 0;
		var radians = -placement.Angle * MathF.PI / 180f;
		foreach (var wire in wires)
		{
			var start = ToLocal(wire.Start, placement, radians);
			var end = ToLocal(wire.End, placement, radians);
			if (SegmentIntersectsRectangle(start, end, label.Width / 2f + clearance, (label.Ascent + label.Descent) / 2f + clearance))
			{
				count++;
			}
		}
		return count;
	}

	private static PointF ToLocal(PointF point, LabelPlacement placement, float radians)
	{
		var x = point.X - placement.CenterX;
		var y = point.Y - placement.CenterY;
		return new PointF(x * MathF.Cos(radians) - y * MathF.Sin(radians), x * MathF.Sin(radians) + y * MathF.Cos(radians));
	}

	private static bool SegmentIntersectsRectangle(PointF start, PointF end, float halfWidth, float halfHeight)
	{
		var min = 0f;
		var max = 1f;
		var dx = end.X - start.X;
		var dy = end.Y - start.Y;
		return Clip(-dx, start.X + halfWidth, ref min, ref max) && Clip(dx, halfWidth - start.X, ref min, ref max) &&
			Clip(-dy, start.Y + halfHeight, ref min, ref max) && Clip(dy, halfHeight - start.Y, ref min, ref max);
	}

	private static bool Clip(float direction, float distance, ref float min, ref float max)
	{
		if (direction == 0)
		{
			return distance >= 0;
		}
		var amount = distance / direction;
		if (direction < 0)
		{
			if (amount > max) return false;
			min = Math.Max(min, amount);
		}
		else
		{
			if (amount < min) return false;
			max = Math.Min(max, amount);
		}
		return true;
	}

	private sealed record LabelMeasurement(int Index, string Text, float Width, float Ascent, float Descent);
	private sealed record LabelPlacement(float CenterX, float CenterY, float Angle, int Ordinal);

	private static bool IsLabelSelected(int index, int count, int labelInterval)
	{
		return index == 0 || (index + 1) % labelInterval == 0 || index == count - 1;
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
		var maxHalfDiagonal = 0f;
		for (var index = 0; index < nodes.Count; index++)
		{
			var node = nodes[index];
			minX = Math.Min(minX, node.Item1.X);
			maxX = Math.Max(maxX, node.Item1.X);
			minY = Math.Min(minY, node.Item1.Y);
			maxY = Math.Max(maxY, node.Item1.Y);
			if (IsLabelSelected(index, nodes.Count, labelInterval))
			{
				var measuredWidth = graphics.MeasureString(node.Item2.ToString(), font).Width + 2f;
				var measureFamily = font.FontFamily;
				var measureEmHeight = measureFamily.GetEmHeight(font.Style);
				var labelHeight = font.Size * (measureFamily.GetCellAscent(font.Style) + measureFamily.GetCellDescent(font.Style)) / measureEmHeight + 4f;
				maxHalfDiagonal = Math.Max(maxHalfDiagonal, MathF.Sqrt(measuredWidth * measuredWidth + labelHeight * labelHeight) / 2f);
			}
		}

		var width = (double)maxX - minX;
		var height = (double)maxY - minY;
		var outerRadius = radius * 1.25f;
		const float labelGap = 2f;
		var sideGuard = Math.Max(outerRadius, radius + 2 * maxHalfDiagonal + 2 * labelGap);
		var right = sideGuard;
		var top = sideGuard;
		var bottom = sideGuard;
		var portraitScale = GetScale(816f, 1056f, width, height, sideGuard, right, top, bottom);
		var landscapeScale = GetScale(1056f, 816f, width, height, sideGuard, right, top, bottom);
		var isPortrait = portraitScale >= landscapeScale;
		var canvasWidth = isPortrait ? 816f : 1056f;
		var canvasHeight = isPortrait ? 1056f : 816f;
		var scale = isPortrait ? portraitScale : landscapeScale;
		var originX = CanvasMargin + (canvasWidth - 2 * CanvasMargin - (width * scale + sideGuard + right)) / 2 + sideGuard;
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

	private static SvgText CreateLabel(string text, LabelPlacement placement, LabelMeasurement measurement, float fontSize)
	{
		var centerY = placement.CenterY + (measurement.Ascent - measurement.Descent) / 2f;
		var label = new SvgText
		{
			Text = text,
			Fill = new SvgColourServer(Color.Black),
			FontFamily = LabelFontFamily,
			FontSize = fontSize,
			Transforms = new SvgTransformCollection(),
			X = { placement.CenterX - measurement.Width / 2f },
			Y = { centerY }
		};
		if (placement.Angle != 0)
		{
			label.Transforms.Add(new SvgRotate(placement.Angle, placement.CenterX, placement.CenterY));
		}
		return label;
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

	private static SvgPolygon CreateDiamondMarker(PointF point, float radius)
	{
		var points = new SvgPointCollection
		{
			point.X, point.Y - radius,
			point.X + radius, point.Y,
			point.X, point.Y + radius,
			point.X - radius, point.Y
		};

		return new SvgPolygon
		{
			Points = points,
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
