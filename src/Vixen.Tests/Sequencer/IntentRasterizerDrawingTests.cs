using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using Vixen.Data.Value;
using Vixen.Intent;
using VixenModules.Editor.TimedSequenceEditor;
using Xunit;

namespace Vixen.Tests.Sequencer;

/// <summary>
/// Verifies gradient rendering for timeline intent thumbnails.
/// </summary>
public sealed class IntentRasterizerDrawingTests
{
	private static readonly Type RasterizerType = typeof(TimedSequenceElement).Assembly.GetType("VixenModules.Editor.TimedSequenceEditor.IntentRasterizer", true)!;
	private static readonly MethodInfo RasterizeMethod = RasterizerType.GetMethod("Rasterize")!;
	private static readonly TimeSpan FrameTime = TimeSpan.FromMilliseconds(10);

	/// <summary>
	/// Verifies the captured Butterfly rectangle draws without preventing a subsequent gradient.
	/// </summary>
	[Fact]
	public void Rasterize_CapturedButterflyBounds_ContinuesDrawing()
	{
		using var bitmap = new Bitmap(320, 24);
		using var graphics = Graphics.FromImage(bitmap);
		var rasterizer = Activator.CreateInstance(RasterizerType, true)!;
		var captured = new RectangleF(297.225464f, 7.57701015f, 4.71786451f, 8.1524071e-5f);
		var start = Color.FromArgb(0x5B0505FE);
		var end = Color.FromArgb(0x580505FF);

		// RGBValue(Color) does not retain alpha; exercise the captured colors at the brush boundary too.
		Draw(rasterizer, graphics, captured, Color.Red, Color.Blue);
		RasterizerType.GetMethod("DrawGradient", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(rasterizer, [start, end, captured]);
		Draw(rasterizer, graphics, new RectangleF(10, 10, 100, 8), Color.Red, Color.Blue);

		Assert.NotEqual(0, bitmap.GetPixel(30, 13).A);
		Assert.NotEqual(bitmap.GetPixel(20, 13).ToArgb(), bitmap.GetPixel(90, 13).ToArgb());
	}

	/// <summary>
	/// Verifies that the original rectangle-based brush rejects the captured geometry.
	/// </summary>
	[Fact]
	public void RectangleBrush_CapturedButterflyBounds_ReproducesFailure()
	{
		var bounds = new RectangleF(296.03656f, 7.57701015f, 7.09566832f, 8.1524071e-5f);
		var start = Color.FromArgb(0x5B0505FE);
		var end = Color.FromArgb(0x580505FF);

		var exception = Assert.Throws<ExternalException>(() =>
		{
			using var brush = new LinearGradientBrush(bounds, start, end, LinearGradientMode.Horizontal);
		});
		Assert.Equal(unchecked((int)0x8000FFFF), exception.HResult);
	}

	/// <summary>
	/// Verifies ordinary and fractional gradients preserve the rectangle brush's visible pixels.
	/// </summary>
	[Fact]
	public void Rasterize_VisibleGradients_MatchOriginalBrush()
	{
		using var actual = new Bitmap(160, 30);
		using var expected = new Bitmap(160, 30);
		using var graphics = Graphics.FromImage(actual);
		using var referenceGraphics = Graphics.FromImage(expected);
		var rasterizer = Activator.CreateInstance(RasterizerType, true)!;
		var cases = new[]
		{
			(new RectangleF(5.25f, 2, 70.5f, 8), Color.Red, Color.Blue),
			(new RectangleF(10, 14, 120, 8), Color.Blue, Color.Red),
			(new RectangleF(140, 2, 12, 8), Color.Lime, Color.Lime)
		};

		foreach (var (rectangle, start, end) in cases)
		{
			Draw(rasterizer, graphics, rectangle, start, end);
			if (start == end)
			{
				using var solid = new SolidBrush(start);
				referenceGraphics.FillRectangle(solid, rectangle);
				continue;
			}

			float offset = rectangle.X * 0.004f;
			var padded = new RectangleF(rectangle.X - offset, rectangle.Y, rectangle.Width + 2 * offset, rectangle.Height);
			using var brush = new LinearGradientBrush(padded, start, end, LinearGradientMode.Horizontal);
			brush.GammaCorrection = true;
			referenceGraphics.FillRectangle(brush, rectangle);
		}

		for (int y = 0; y < actual.Height; y++)
			for (int x = 0; x < actual.Width; x++)
				Assert.Equal(expected.GetPixel(x, y).ToArgb(), actual.GetPixel(x, y).ToArgb());
	}

	/// <summary>
	/// Verifies tiny and invalid rectangles do not prevent later valid rendering.
	/// </summary>
	[Fact]
	public void Rasterize_TinyAndInvalidBounds_ContinuesDrawing()
	{
		using var bitmap = new Bitmap(40, 20);
		using var graphics = Graphics.FromImage(bitmap);
		var rasterizer = Activator.CreateInstance(RasterizerType, true)!;

		Draw(rasterizer, graphics, new RectangleF(0, 0, 1e-8f, 4), Color.Red, Color.Blue);
		Draw(rasterizer, graphics, new RectangleF(float.NaN, 0, 10, 4), Color.Red, Color.Blue);
		Draw(rasterizer, graphics, new RectangleF(0, 0, -1, 4), Color.Red, Color.Blue);
		Draw(rasterizer, graphics, new RectangleF(5, 8, 20, 5), Color.Lime, Color.Lime);

		Assert.Equal(0, bitmap.GetPixel(30, 1).A);
		Assert.Equal(Color.Lime.ToArgb(), bitmap.GetPixel(10, 10).ToArgb());
	}

	private static void Draw(object rasterizer, Graphics graphics, RectangleF rectangle, Color start, Color end)
	{
		var intent = new StaticArrayIntent<RGBValue>(FrameTime, [new RGBValue(start), new RGBValue(end), new RGBValue(end)], FrameTime * 3);
		Assert.Equal(start.ToArgb(), intent.GetStateAt(TimeSpan.Zero).FullColorWithAlpha.ToArgb());
		Assert.Equal(end.ToArgb(), intent.GetStateAt(FrameTime).FullColorWithAlpha.ToArgb());
		RasterizeMethod.Invoke(rasterizer, [intent, rectangle, graphics, TimeSpan.Zero, FrameTime]);
	}
}
