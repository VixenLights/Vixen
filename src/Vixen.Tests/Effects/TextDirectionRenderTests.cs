using System.Drawing;
using System.Reflection;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Text;
using Xunit;

namespace Vixen.Tests.Effects;

/// <summary>
/// Verifies that animated Text directions render stacked literal-space entries without exhausting direction state.
/// </summary>
public sealed class TextDirectionRenderTests
{
	private const int BufferWidth = 200;
	private const int BufferHeight = 100;

	[Theory]
	[InlineData(TextDirection.Fall, "A B")]
	[InlineData(TextDirection.Fall, " A")]
	[InlineData(TextDirection.Fall, "A ")]
	[InlineData(TextDirection.Fall, "A  B")]
	[InlineData(TextDirection.Fall, " ")]
	[InlineData(TextDirection.Explode, "A B")]
	[InlineData(TextDirection.Explode, " A")]
	[InlineData(TextDirection.Explode, "A ")]
	[InlineData(TextDirection.Explode, "A  B")]
	[InlineData(TextDirection.Explode, " ")]
	public void TextRendering_StackedLiteralSpaces_DoNotThrow(TextDirection direction, string text)
	{
		// Arrange
		using var font = new Font("Arial", 16);
		var effect = new Text
		{
			TextSource = TextSource.None,
			TextMode = TextMode.Rotated,
			Direction = direction,
			TextLines = [text],
			Font = font,
			Colors = [new ColorGradient(Color.White)],
			TimeSpan = TimeSpan.FromSeconds(1)
		};
		SetVirtualBuffer(effect, BufferWidth, BufferHeight);
		InvokeSetupRender(effect);
		var frameBuffer = new PixelFrameBuffer(BufferWidth, BufferHeight);

		// Act
		var exception = Record.Exception(() => InvokeRenderEffect(effect, 0, frameBuffer));

		// Assert
		Assert.True(exception is null, $"Rendering {direction} text '{text}' threw {exception?.InnerException ?? exception}.");
	}

	private static void SetVirtualBuffer(Text effect, int width, int height)
	{
		SetPixelEffectBaseField(effect, "_bufferHt", height);
		SetPixelEffectBaseField(effect, "_bufferWi", width);
	}

	private static void SetPixelEffectBaseField(Text effect, string fieldName, int value)
	{
		var field = typeof(PixelEffectBase).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

		Assert.NotNull(field);
		field.SetValue(effect, value);
	}

	private static void InvokeSetupRender(Text effect)
	{
		var setupRender = typeof(Text).GetMethod("SetupRender", BindingFlags.Instance | BindingFlags.NonPublic);

		Assert.NotNull(setupRender);
		setupRender.Invoke(effect, []);
	}

	private static void InvokeRenderEffect(Text effect, int frame, IPixelFrameBuffer frameBuffer)
	{
		var renderEffect = typeof(Text).GetMethod(
			"RenderEffect",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(IPixelFrameBuffer)],
			null);

		Assert.NotNull(renderEffect);
		renderEffect.Invoke(effect, [frame, frameBuffer]);
	}
}
