using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class TimelineCursorSelectionTests
{
	[Fact]
	public void SelectElement_WhenMoveCursorEnabled_MovesCursorToElementStart()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		Element element = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(12));
		timelineControl.grid.MoveCursorToSelectedEffect = true;

		try
		{
			timelineControl.SelectElement(element);

			Assert.Equal(element.StartTime, TimeLineGlobalStateManager.Manager(id).CursorPosition);
		}
		finally
		{
			TimeLineGlobalStateManager.CloseManager(id);
		}
	}

	[Fact]
	public void SelectElement_WhenMoveCursorDisabled_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		Element element = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(12));
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager.Manager(id).CursorPosition = originalCursorPosition;
		timelineControl.grid.MoveCursorToSelectedEffect = false;

		try
		{
			timelineControl.SelectElement(element);

			Assert.Equal(originalCursorPosition, TimeLineGlobalStateManager.Manager(id).CursorPosition);
		}
		finally
		{
			TimeLineGlobalStateManager.CloseManager(id);
		}
	}

	[Fact]
	public void SelectElements_WhenMoveCursorEnabled_MovesCursorToEarliestSelectedStart()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2");
		Element laterElement = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(18));
		Element earlierElement = AddElement(timelineControl.Rows.ElementAt(1), TimeSpan.FromSeconds(7));
		timelineControl.grid.MoveCursorToSelectedEffect = true;

		try
		{
			timelineControl.grid.SelectElements([laterElement, earlierElement]);

			Assert.Equal(earlierElement.StartTime, TimeLineGlobalStateManager.Manager(id).CursorPosition);
		}
		finally
		{
			TimeLineGlobalStateManager.CloseManager(id);
		}
	}

	[Fact]
	public void SelectElements_WhenMoveCursorDisabled_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2");
		Element firstElement = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(18));
		Element secondElement = AddElement(timelineControl.Rows.ElementAt(1), TimeSpan.FromSeconds(7));
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager.Manager(id).CursorPosition = originalCursorPosition;
		timelineControl.grid.MoveCursorToSelectedEffect = false;

		try
		{
			timelineControl.grid.SelectElements([firstElement, secondElement]);

			Assert.Equal(originalCursorPosition, TimeLineGlobalStateManager.Manager(id).CursorPosition);
		}
		finally
		{
			TimeLineGlobalStateManager.CloseManager(id);
		}
	}

	[Fact]
	public void SelectElements_WhenNoElementsSelected_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager.Manager(id).CursorPosition = originalCursorPosition;
		timelineControl.grid.MoveCursorToSelectedEffect = true;

		try
		{
			timelineControl.grid.SelectElements([]);

			Assert.Equal(originalCursorPosition, TimeLineGlobalStateManager.Manager(id).CursorPosition);
		}
		finally
		{
			TimeLineGlobalStateManager.CloseManager(id);
		}
	}

	private static TimelineControl CreateTimelineControl(out Guid id, params string[] rowNames)
	{
		id = Guid.NewGuid();
		TimelineControl timelineControl = new TimelineControl(id);
		foreach (string rowName in rowNames)
		{
			Row row = timelineControl.AddRow(rowName);
			row.Visible = true;
		}

		timelineControl.grid.ResizeGridHeight();
		timelineControl.LayoutRows();
		return timelineControl;
	}

	private static Element AddElement(Row row, TimeSpan startTime)
	{
		Element element = new Element
		{
			StartTime = startTime,
			Duration = TimeSpan.FromSeconds(2)
		};
		row.AddElement(element);
		return element;
	}
}
