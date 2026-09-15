using Common.Controls.Timeline;
using System.Drawing;
using Xunit;
using FactAttribute = Xunit.WinFormsFactAttribute;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class TimelineZoomTests
{
	[Fact]
	public void ZoomTime_WhenZoomInReachesMinimumResolution_DoesNotChangeTimeline()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.TimePerPixel = TimeSpan.FromTicks(2222);
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(10);
		TimeSpan originalTimePerPixel = timelineControl.TimePerPixel;
		TimeSpan originalVisibleTimeStart = timelineControl.VisibleTimeStart;

		timelineControl.ZoomTime(0.9, Point.Empty);

		Assert.Equal(originalTimePerPixel, timelineControl.TimePerPixel);
		Assert.Equal(originalVisibleTimeStart, timelineControl.VisibleTimeStart);
	}

	[Fact]
	public void ZoomTime_WhenZoomOutExceedsSequence_MatchesToolbarAndShowsFullSequenceFromZero()
	{
		using TimelineControl toolbarTimelineControl = CreateTimelineControl();
		using TimelineControl mouseTimelineControl = CreateTimelineControl();
		toolbarTimelineControl.TimePerPixel = TimeSpan.FromSeconds(1);
		mouseTimelineControl.TimePerPixel = TimeSpan.FromSeconds(1);

		toolbarTimelineControl.Zoom(1.1);
		mouseTimelineControl.ZoomTime(1.1, new Point(400, 0));

		Assert.Equal(toolbarTimelineControl.TimePerPixel, mouseTimelineControl.TimePerPixel);
		Assert.Equal(toolbarTimelineControl.VisibleTimeSpan, mouseTimelineControl.VisibleTimeSpan);
		Assert.Equal(TimeSpan.Zero, mouseTimelineControl.VisibleTimeStart);
	}

	[Fact]
	public void ZoomTime_WhenMouseFocusedZoomIsValid_PreservesMouseFocusedMovement()
	{
		using TimelineControl timelineControl = CreateTimelineControl();
		timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(100);
		TimeSpan originalTimePerPixel = timelineControl.TimePerPixel;
		TimeSpan originalVisibleTimeStart = timelineControl.VisibleTimeStart;
		Point mousePosition = new(timelineControl.splitContainer.SplitterDistance + 400, 0);

		timelineControl.ZoomTime(0.9, mousePosition);

		Assert.Equal(originalTimePerPixel.Scale(0.9), timelineControl.TimePerPixel);
		Assert.True(timelineControl.VisibleTimeStart > originalVisibleTimeStart);
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
