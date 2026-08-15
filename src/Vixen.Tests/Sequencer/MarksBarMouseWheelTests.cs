using Common.Controls.Timeline;
using System.Drawing;
using System.Windows.Forms;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class MarksBarMouseWheelTests
{
	[Fact]
	public void HandleMarksBarMouseWheel_WhenShiftWheelMovesTowardUser_PansRightByTenPercent()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart + timelineControl.VisibleTimeSpan.Scale(0.1);

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Shift);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenShiftWheelMovesAwayFromUser_PansLeftByTenPercent()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart - timelineControl.VisibleTimeSpan.Scale(0.1);

		timelineControl.HandleMarksBarMouseWheel(120, Keys.Shift);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenShiftAndAltArePressed_Pans()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart + timelineControl.VisibleTimeSpan.Scale(0.1);

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Shift | Keys.Alt);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenShiftAndControlArePressed_Pans()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart + timelineControl.VisibleTimeSpan.Scale(0.1);

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Shift | Keys.Control);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenControlIsPressedWithoutShift_ZoomsHorizontally()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		TimeSpan originalTimePerPixel = timelineControl.TimePerPixel;

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Control);

		Assert.Equal(originalTimePerPixel.Scale(1.1), timelineControl.TimePerPixel);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenNoModifierIsPressed_DoesNotNavigate()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan originalVisibleStart = timelineControl.VisibleTimeStart;
		TimeSpan originalTimePerPixel = timelineControl.TimePerPixel;

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.None);

		Assert.Equal(originalVisibleStart, timelineControl.VisibleTimeStart);
		Assert.Equal(originalTimePerPixel, timelineControl.TimePerPixel);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenControlAndAltArePressed_DoesNotNavigate()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan originalVisibleStart = timelineControl.VisibleTimeStart;
		TimeSpan originalTimePerPixel = timelineControl.TimePerPixel;

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Control | Keys.Alt);

		Assert.Equal(originalVisibleStart, timelineControl.VisibleTimeStart);
		Assert.Equal(originalTimePerPixel, timelineControl.TimePerPixel);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenPanningPastLeftBoundary_ClampsToZero()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = timelineControl.VisibleTimeSpan.Scale(0.05);

		timelineControl.HandleMarksBarMouseWheel(120, Keys.Shift);

		Assert.Equal(TimeSpan.Zero, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenPanningPastRightBoundary_ClampsToLatestVisibleStart()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		TimeSpan latestVisibleStart = timelineControl.TotalTime - timelineControl.VisibleTimeSpan;
		timelineControl.VisibleTimeStart = latestVisibleStart - timelineControl.VisibleTimeSpan.Scale(0.05);

		timelineControl.HandleMarksBarMouseWheel(-120, Keys.Shift);

		Assert.Equal(latestVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void HandleMarksBarMouseWheel_WhenDeltaIsPartial_PansProportionally()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart + timelineControl.VisibleTimeSpan.Scale(0.025);

		timelineControl.HandleMarksBarMouseWheel(-30, Keys.Shift);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	[Theory]
	[InlineData(120, 0.10)]
	[InlineData(-120, -0.10)]
	public void HandleMarksBarMouseHWheel_WhenNativeHorizontalWheelMoves_PansLikeGrid(int delta, double scale)
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(20);
		TimeSpan expectedVisibleStart = timelineControl.VisibleTimeStart + timelineControl.VisibleTimeSpan.Scale(scale);

		timelineControl.HandleMarksBarMouseHWheel(delta);

		Assert.Equal(expectedVisibleStart, timelineControl.VisibleTimeStart);
	}

	private static TimelineControl CreateTimelineControl()
	{
		TimelineControl timelineControl = new TimelineControl(Guid.NewGuid())
		{
			Size = new Size(1000, 600),
			TotalTime = TimeSpan.FromMinutes(10),
			TimePerPixel = TimeSpan.FromMilliseconds(100)
		};
		timelineControl.PerformLayout();
		return timelineControl;
	}
}
