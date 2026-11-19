using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Editor.TimedSequenceEditor;
using Xunit;

namespace Vixen.Tests.Sequencer;

/// <summary>
/// Verifies how effect intents are sampled for timeline thumbnails.
/// </summary>
public class EffectRasterizerDrawingTests
{
	/// <summary>
	/// Verifies that sparse targets with overlapping intents are displayed in their target bands.
	/// </summary>
	[Fact]
	public void DrawCanvasContent_DisplaysVisibleTargetsInDenseThumbnail()
	{
		var targets = Enumerable.Range(0, 40).Select(index => new Element($"Target {index}")).ToArray();
		var targetGroup = new Mock<IElementNode>();
		targetGroup.Setup(node => node.GetElementEnumerator()).Returns(targets);

		var visibleStart = TimeSpan.FromMilliseconds(100);
		var visibleEnd = TimeSpan.FromMilliseconds(300);
		var visibleDuration = visibleEnd - visibleStart;
		var intents = new EffectIntents();
		intents.AddIntentForElement(targets[0].Id, CreateIntent(Color.Blue, visibleStart), TimeSpan.Zero);
		var denseIntent = CreateIntent(Color.Red, visibleDuration);
		for (int i = 0; i < 215; i++)
			intents.AddIntentForElement(targets[2].Id, denseIntent, visibleStart);
		intents.AddIntentForElement(targets[^1].Id, CreateIntent(Color.Lime, visibleDuration), visibleStart);

		var effect = new Mock<IEffectModuleInstance>();
		effect.SetupGet(instance => instance.TargetNodes).Returns([targetGroup.Object]);
		effect.SetupGet(instance => instance.TimeSpan).Returns(TimeSpan.FromMilliseconds(400));
		effect.Setup(instance => instance.Render()).Returns(intents);
		var element = new TimedSequenceElement(new EffectNode(effect.Object, TimeSpan.Zero));
		using var bitmap = new Bitmap(100, 19);
		using var graphics = Graphics.FromImage(bitmap);
		var drawCanvasContent = typeof(TimedSequenceElement).GetMethod("DrawCanvasContent", BindingFlags.Instance | BindingFlags.NonPublic)!;

		drawCanvasContent.Invoke(element, [graphics, visibleStart, visibleEnd, 200, false]);

		Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(20, 0).ToArgb());
		Assert.Equal(0, bitmap.GetPixel(20, 9).A);
		Assert.Equal(Color.Lime.ToArgb(), bitmap.GetPixel(20, 18).ToArgb());
	}

	private static StaticArrayIntent<RGBValue> CreateIntent(Color color, TimeSpan duration)
	{
		var value = new RGBValue(color);
		return new StaticArrayIntent<RGBValue>(duration, [value, value], duration);
	}
}
