using Common.AudioPlayer;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Moq;
using System.Drawing;
using System.Reflection;
using VixenModules.Media.Audio;
using VixenModules.Media.Audio.SampleProviders;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class WaveformAlignmentRenderingTests
{
	[Fact]
	public void GetAlignmentInvalidationRectangle_WhenGuideIsAtLeftEdge_ClipsToClientRectangle()
	{
		using var waveform = CreateWaveform();

		var rectangle = waveform.GetAlignmentInvalidationRectangle(TimeSpan.FromSeconds(10));

		Assert.Equal(new Rectangle(0, 0, 3, 60), rectangle);
	}

	[Fact]
	public void GetAlignmentInvalidationRectangle_WhenGuideIsCentered_ReturnsNarrowGuideRegion()
	{
		using var waveform = CreateWaveform();

		var rectangle = waveform.GetAlignmentInvalidationRectangle(TimeSpan.FromSeconds(60));

		Assert.Equal(new Rectangle(48, 0, 5, 60), rectangle);
	}

	[Fact]
	public void GetAlignmentInvalidationRectangle_WhenGuideIsAtRightEdge_ClipsToClientRectangle()
	{
		using var waveform = CreateWaveform();

		var rectangle = waveform.GetAlignmentInvalidationRectangle(TimeSpan.FromSeconds(109));

		Assert.Equal(new Rectangle(97, 0, 3, 60), rectangle);
	}

	[Fact]
	public void GetAlignmentInvalidationRectangle_WhenGuideIsOutsideViewport_ReturnsEmptyRectangle()
	{
		using var waveform = CreateWaveform();

		var rectangle = waveform.GetAlignmentInvalidationRectangle(TimeSpan.FromSeconds(113));

		Assert.Equal(Rectangle.Empty, rectangle);
	}

	[Fact]
	public void GetAlignmentInvalidationRectangle_WhenGuideMoves_CoversPreviousAndCurrentPositions()
	{
		using var waveform = CreateWaveform();

		var rectangle = waveform.GetAlignmentInvalidationRectangle(
			[TimeSpan.FromSeconds(10)],
			[TimeSpan.FromSeconds(20)]);

		Assert.Equal(new Rectangle(0, 0, 13, 60), rectangle);
	}

	[Fact]
	public void WaveFormSelectedTimeLineGlobalMove_WhenInactiveTimesAreNull_ClearsActiveTimesWithoutThrowing()
	{
		using var waveform = CreateWaveform();

		InvokeAlignmentActivity(waveform, new AlignmentEventArgs(true, [TimeSpan.FromSeconds(10)]));
		var exception = Record.Exception(() => InvokeAlignmentActivity(waveform, new AlignmentEventArgs(false, null)));

		Assert.Null(exception);
		Assert.Empty(GetActiveTimes(waveform));
	}

	[Fact]
	public void GetVisibleSampleRange_WhenClipIsAtLeftEdge_ReturnsBoundedRangeWithMargin()
	{
		using var waveform = CreateWaveform();
		ConfigureSampleRange(waveform, 120, TimeSpan.FromSeconds(150));

		var range = waveform.GetVisibleSampleRange(new Rectangle(0, 0, 5, 60));

		Assert.Equal((9, 16), range);
	}

	[Fact]
	public void GetVisibleSampleRange_WhenClipIsAtRightEdge_ReturnsBoundedRangeWithMargin()
	{
		using var waveform = CreateWaveform();
		ConfigureSampleRange(waveform, 120, TimeSpan.FromSeconds(150));

		var range = waveform.GetVisibleSampleRange(new Rectangle(95, 0, 5, 60));

		Assert.Equal((104, 111), range);
	}

	[Fact]
	public void GetVisibleSampleRange_WhenClipIsOutsideSamples_ClampsBothBounds()
	{
		using var waveform = CreateWaveform();
		ConfigureSampleRange(waveform, 120, TimeSpan.FromSeconds(150));

		var range = waveform.GetVisibleSampleRange(new Rectangle(200, 0, 5, 60));

		Assert.Equal((120, 120), range);
	}

	[Fact]
	public void GetVisibleSampleRange_WhenClientIsFullyInvalidated_CoversVisibleSampleColumns()
	{
		using var waveform = CreateWaveform();
		ConfigureSampleRange(waveform, 120, TimeSpan.FromSeconds(150));

		var range = waveform.GetVisibleSampleRange(waveform.ClientRectangle);

		Assert.Equal((9, 111), range);
	}

	private static Waveform CreateWaveform()
	{
		var timeInfo = new TimeInfo
		{
			TimePerPixel = TimeSpan.FromSeconds(1),
			TotalTime = TimeSpan.FromSeconds(200),
			VisibleTimeStart = TimeSpan.FromSeconds(10)
		};
		return new Waveform(timeInfo, Guid.NewGuid())
		{
			Size = new Size(100, 60)
		};
	}

	private static void ConfigureSampleRange(Waveform waveform, int sampleCount, TimeSpan mediaDuration)
	{
		var audio = new Audio();
		var player = new Mock<IPlayer>();
		player.SetupGet(x => x.Duration).Returns(mediaDuration);

		SetField(audio, "_audioSystem", player.Object);
		SetField(waveform, "audio", audio);
		SetField(waveform, "samples", Enumerable.Repeat(new Sample(), sampleCount).ToList());
	}

	private static void InvokeAlignmentActivity(Waveform waveform, AlignmentEventArgs eventArgs)
	{
		var method = typeof(Waveform).GetMethod("WaveFormSelectedTimeLineGlobalMove", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(waveform, [null, eventArgs]);
	}

	private static IReadOnlyList<TimeSpan> GetActiveTimes(Waveform waveform)
	{
		return (IReadOnlyList<TimeSpan>) GetField(waveform, "_activeTimes");
	}

	private static object GetField(object target, string fieldName)
	{
		var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		return field.GetValue(target)!;
	}

	private static void SetField(object target, string fieldName, object value)
	{
		var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		field.SetValue(target, value);
	}
}
