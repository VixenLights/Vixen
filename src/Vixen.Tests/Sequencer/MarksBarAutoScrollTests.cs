using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using System.Drawing;
using System.Reflection;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class MarksBarAutoScrollTests
{
	[Fact]
	public void GetHorizontalAutoScrollDistance_WhenMouseInLeftMargin_ReturnsNegativeDistance()
	{
		using MarksBar marksBar = CreateMarksBar();

		int distance = GetHorizontalAutoScrollDistance(marksBar, 10);

		Assert.Equal(-14, distance);
	}

	[Fact]
	public void GetHorizontalAutoScrollDistance_WhenMouseInRightMargin_ReturnsPositiveDistance()
	{
		using MarksBar marksBar = CreateMarksBar();

		int distance = GetHorizontalAutoScrollDistance(marksBar, 90);

		Assert.Equal(14, distance);
	}

	[Fact]
	public void GetHorizontalAutoScrollDistance_WhenMouseOutsideMargin_ReturnsZero()
	{
		using MarksBar marksBar = CreateMarksBar();

		int distance = GetHorizontalAutoScrollDistance(marksBar, 50);

		Assert.Equal(0, distance);
	}

	[Fact]
	public void CalculateAutoScrollVisibleTimeStart_WhenScrollingLeftPastStart_ClampsToZero()
	{
		using MarksBar marksBar = CreateMarksBar();
		marksBar.TotalTime = TimeSpan.FromSeconds(60);
		TimeSpan currentVisibleStart = TimeSpan.FromMilliseconds(500);

		TimeSpan visibleStart = CalculateAutoScrollVisibleTimeStart(marksBar, currentVisibleStart, -400);

		Assert.Equal(TimeSpan.Zero, visibleStart);
	}

	[Fact]
	public void CalculateAutoScrollVisibleTimeStart_WhenScrollingRightPastEnd_ClampsToLatestVisibleStart()
	{
		using MarksBar marksBar = CreateMarksBar();
		marksBar.TotalTime = TimeSpan.FromSeconds(60);
		TimeSpan currentVisibleStart = TimeSpan.FromSeconds(49);

		TimeSpan visibleStart = CalculateAutoScrollVisibleTimeStart(marksBar, currentVisibleStart, 80);

		Assert.Equal(TimeSpan.FromSeconds(50), visibleStart);
	}

	[Fact]
	public void CalculateAutoScrollVisibleTimeStart_WhenWholeSequenceVisible_ReturnsZero()
	{
		using MarksBar marksBar = CreateMarksBar();
		marksBar.TotalTime = TimeSpan.FromSeconds(5);

		TimeSpan visibleStart = CalculateAutoScrollVisibleTimeStart(marksBar, TimeSpan.FromSeconds(2), 80);

		Assert.Equal(TimeSpan.Zero, visibleStart);
	}

	[Fact]
	public void CalculateAutoScrollVisibleTimeStart_WhenScrollingRight_AdvancesVisibleStart()
	{
		using MarksBar marksBar = CreateMarksBar();
		marksBar.TotalTime = TimeSpan.FromSeconds(60);
		TimeSpan currentVisibleStart = TimeSpan.FromSeconds(10);

		TimeSpan visibleStart = CalculateAutoScrollVisibleTimeStart(marksBar, currentVisibleStart, 80);

		Assert.Equal(TimeSpan.FromSeconds(12), visibleStart);
	}

	[Fact]
	public void CalculateAutoScrollVisibleTimeStart_WhenScrollingLeft_MovesVisibleStartBackward()
	{
		using MarksBar marksBar = CreateMarksBar();
		marksBar.TotalTime = TimeSpan.FromSeconds(60);
		TimeSpan currentVisibleStart = TimeSpan.FromSeconds(10);

		TimeSpan visibleStart = CalculateAutoScrollVisibleTimeStart(marksBar, currentVisibleStart, -80);

		Assert.Equal(TimeSpan.FromSeconds(8), visibleStart);
	}

	private static MarksBar CreateMarksBar()
	{
		TimeInfo timeInfo = new TimeInfo
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		MarksBar marksBar = new MarksBar(timeInfo, Guid.NewGuid())
		{
			Size = new Size(100, 20)
		};
		return marksBar;
	}

	private static int GetHorizontalAutoScrollDistance(MarksBar marksBar, int x)
	{
		MethodInfo method = typeof(MarksBar).GetMethod("GetHorizontalAutoScrollDistance", BindingFlags.Instance | BindingFlags.NonPublic)!;
		return (int) method.Invoke(marksBar, [x])!;
	}

	private static TimeSpan CalculateAutoScrollVisibleTimeStart(MarksBar marksBar, TimeSpan currentVisibleStart, int mouseOutsideX)
	{
		MethodInfo method = typeof(MarksBar).GetMethod("CalculateAutoScrollVisibleTimeStart", BindingFlags.Instance | BindingFlags.NonPublic)!;
		return (TimeSpan) method.Invoke(marksBar, [currentVisibleStart, mouseOutsideX])!;
	}
}
