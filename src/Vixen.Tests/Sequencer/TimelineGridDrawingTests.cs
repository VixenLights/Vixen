using System.Drawing;
using Moq;
using Vixen.Module.Effect;
using Vixen.Sys;
using Xunit;
using TimelineElement = Common.Controls.Timeline.Element;

namespace Vixen.Tests.Sequencer;

public sealed class TimelineGridDrawingTests
{
	[Fact]
	public void Draw_WhenEffectIsRendered_ReusesCachedImageForSameVisibleSlice()
	{
		TestTimelineElement element = CreateElement(isDirty: false);

		using Bitmap firstImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;
		Bitmap secondImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;

		Assert.Same(firstImage, secondImage);
		Assert.Equal(1, element.DrawCanvasContentCallCount);
	}

	[Fact]
	public void Draw_WhenSelectionChanges_InvalidatesRenderedImageCache()
	{
		TestTimelineElement element = CreateElement(isDirty: false);

		Bitmap firstImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;

		element.Selected = true;
		Bitmap secondImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;

		Assert.NotSame(firstImage, secondImage);
		Assert.Equal(2, element.DrawCanvasContentCallCount);
		secondImage.Dispose();
	}

	[Fact]
	public void Draw_WhenEffectIsNotRendered_ReturnsIndependentPlaceholderImages()
	{
		TestTimelineElement element = CreateElement(isDirty: true);

		Bitmap firstImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;
		Bitmap secondImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;

		Assert.NotSame(firstImage, secondImage);
		Assert.Equal(0, element.DrawCanvasContentCallCount);
		firstImage.Dispose();
		secondImage.Dispose();
	}

	[Fact]
	public void Draw_DisposedPlaceholder_DoesNotAffectLaterRenderedImage()
	{
		Mock<IEffectModuleInstance> effect = CreateEffectMock(isDirty: true);
		TestTimelineElement element = CreateElement(effect.Object);
		Bitmap placeholderImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;
		placeholderImage.Dispose();
		effect.SetupGet(instance => instance.IsDirty).Returns(false);

		using Bitmap renderedImage = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false).Image;

		Assert.Equal(1, element.DrawCanvasContentCallCount);
		Assert.Equal(Color.Red.ToArgb(), renderedImage.GetPixel(3, 2).ToArgb());
	}

	[Fact]
	public void DrawV2_WhenEffectIsNotRendered_MarksPlaceholderForDisposal()
	{
		TestTimelineElement element = CreateElement(isDirty: true);

		var drawResult = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false);

		Assert.True(drawResult.DisposeAfterDraw);
		drawResult.Image.Dispose();
	}

	[Fact]
	public void DrawV2_WhenEffectIsRendered_DoesNotMarkCachedImageForDisposal()
	{
		TestTimelineElement element = CreateElement(isDirty: false);

		var drawResult = element.DrawV2(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false);

		Assert.False(drawResult.DisposeAfterDraw);
		drawResult.Image.Dispose();
	}

	[Fact]
	public void Draw_ObsoleteWrapper_ReturnsDrawV2Image()
	{
		TestTimelineElement element = CreateElement(isDirty: false);

#pragma warning disable CS0618 // Verifies compatibility wrapper until Draw is retired.
		using Bitmap image = element.Draw(CreateImageSize(), null, TimeSpan.Zero, element.EndTime, 100, false);
#pragma warning restore CS0618

		Assert.Equal(1, element.DrawCanvasContentCallCount);
		Assert.Equal(Color.Red.ToArgb(), image.GetPixel(3, 2).ToArgb());
	}

	private static TestTimelineElement CreateElement(bool isDirty)
	{
		return CreateElement(CreateEffectMock(isDirty).Object);
	}

	private static TestTimelineElement CreateElement(IEffectModuleInstance effect)
	{
		return new TestTimelineElement
		{
			StartTime = TimeSpan.Zero,
			Duration = TimeSpan.FromSeconds(1),
			EffectNode = new EffectNode(effect, TimeSpan.Zero)
		};
	}

	private static Mock<IEffectModuleInstance> CreateEffectMock(bool isDirty)
	{
		var effect = new Mock<IEffectModuleInstance>();
		effect.SetupGet(instance => instance.IsDirty).Returns(isDirty);
		effect.SetupGet(instance => instance.TimeSpan).Returns(TimeSpan.FromSeconds(1));
		return effect;
	}

	private static Size CreateImageSize()
	{
		return new Size(8, 4);
	}

	private sealed class TestTimelineElement : TimelineElement
	{
		public int DrawCanvasContentCallCount { get; private set; }

		protected override void DrawCanvasContent(Graphics graphics, TimeSpan startTime, TimeSpan endTime, int overallWidth, bool redBorder)
		{
			DrawCanvasContentCallCount++;
			graphics.Clear(Color.Red);
		}
	}
}
