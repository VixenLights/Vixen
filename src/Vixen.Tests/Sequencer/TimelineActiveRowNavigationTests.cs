using Common.Controls.Timeline;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class TimelineActiveRowNavigationTests
{
	[Fact]
	public void MoveActiveRow_Down_ActivatesNextVisibleRow()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2", "Row 3");
		Row firstRow = timelineControl.Rows.ElementAt(0);
		Row secondRow = timelineControl.Rows.ElementAt(1);
		Row thirdRow = timelineControl.Rows.ElementAt(2);
		firstRow.Active = true;
		thirdRow.Selected = true;

		bool moved = timelineControl.MoveActiveRow(1);

		Assert.True(moved);
		Assert.False(firstRow.Active);
		Assert.True(secondRow.Active);
		Assert.True(thirdRow.Selected);
	}

	[Fact]
	public void MoveActiveRow_Up_ActivatesPreviousVisibleRow()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2", "Row 3");
		Row firstRow = timelineControl.Rows.ElementAt(0);
		Row secondRow = timelineControl.Rows.ElementAt(1);
		secondRow.Active = true;

		bool moved = timelineControl.MoveActiveRow(-1);

		Assert.True(moved);
		Assert.True(firstRow.Active);
		Assert.False(secondRow.Active);
	}

	[Fact]
	public void MoveActiveRow_SkipsRowsThatAreNotVisible()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2", "Row 3");
		Row firstRow = timelineControl.Rows.ElementAt(0);
		Row hiddenRow = timelineControl.Rows.ElementAt(1);
		Row thirdRow = timelineControl.Rows.ElementAt(2);
		firstRow.Active = true;
		hiddenRow.Visible = false;

		bool moved = timelineControl.MoveActiveRow(1);

		Assert.True(moved);
		Assert.False(firstRow.Active);
		Assert.False(hiddenRow.Active);
		Assert.True(thirdRow.Active);
	}

	[Fact]
	public void MoveActiveRow_AtFirstVisibleRow_DoesNotWrap()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2");
		Row firstRow = timelineControl.Rows.ElementAt(0);
		firstRow.Active = true;

		bool moved = timelineControl.MoveActiveRow(-1);

		Assert.False(moved);
		Assert.True(firstRow.Active);
	}

	[Fact]
	public void MoveActiveRow_AtLastVisibleRow_DoesNotWrap()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2");
		Row lastRow = timelineControl.Rows.ElementAt(1);
		lastRow.Active = true;

		bool moved = timelineControl.MoveActiveRow(1);

		Assert.False(moved);
		Assert.True(lastRow.Active);
	}

	[Fact]
	public void MoveActiveRow_WithNoRows_ReturnsFalse()
	{
		using TimelineControl timelineControl = CreateTimelineControl();

		bool moved = timelineControl.MoveActiveRow(1);

		Assert.False(moved);
	}

	[Fact]
	public void MoveActiveRow_WithNoActiveRow_DoesNotActivateFallbackRow()
	{
		using TimelineControl timelineControl = CreateTimelineControl("Row 1", "Row 2");

		bool moved = timelineControl.MoveActiveRow(1);

		Assert.False(moved);
		Assert.All(timelineControl.Rows, row => Assert.False(row.Active));
	}

	private static TimelineControl CreateTimelineControl(params string[] rowNames)
	{
		TimelineControl timelineControl = new TimelineControl(Guid.NewGuid());
		foreach (string rowName in rowNames)
		{
			Row row = timelineControl.AddRow(rowName);
			row.Visible = true;
		}

		timelineControl.grid.ResizeGridHeight();
		timelineControl.LayoutRows();
		return timelineControl;
	}
}
