using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using System.Reflection;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class TimelineCursorSelectionTests
{
	[Fact]
	public void SelectElement_ByDefault_MovesCursorToElementStart()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		Element element = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(12));
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);

		timelineControl.SelectElement(element);

		Assert.Equal(element.StartTime, manager.CursorPosition);
	}

	[Fact]
	public void SelectElement_WhenLegacyCursorEnabled_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		Element element = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(12));
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		manager.CursorPosition = originalCursorPosition;
		timelineControl.grid.LegacyCursorActiveRow = true;

		timelineControl.SelectElement(element);

		Assert.Equal(originalCursorPosition, manager.CursorPosition);
	}

	[Fact]
	public void SelectElements_WhenLegacyCursorDisabled_MovesCursorToEarliestSelectedStart()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2");
		Element laterElement = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(18));
		Element earlierElement = AddElement(timelineControl.Rows.ElementAt(1), TimeSpan.FromSeconds(7));
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		timelineControl.grid.LegacyCursorActiveRow = false;

		timelineControl.grid.SelectElements([laterElement, earlierElement]);

		Assert.Equal(earlierElement.StartTime, manager.CursorPosition);
	}

	[Fact]
	public void SelectElements_WhenLegacyCursorEnabled_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2");
		Element firstElement = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(18));
		Element secondElement = AddElement(timelineControl.Rows.ElementAt(1), TimeSpan.FromSeconds(7));
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		manager.CursorPosition = originalCursorPosition;
		timelineControl.grid.LegacyCursorActiveRow = true;

		timelineControl.grid.SelectElements([firstElement, secondElement]);

		Assert.Equal(originalCursorPosition, manager.CursorPosition);
	}

	[Fact]
	public void SelectElements_WhenNoElementsSelected_DoesNotMoveCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1");
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		manager.CursorPosition = originalCursorPosition;
		timelineControl.grid.LegacyCursorActiveRow = false;

		timelineControl.grid.SelectElements([]);

		Assert.Equal(originalCursorPosition, manager.CursorPosition);
	}

	[Fact]
	public void LassoFinalization_WhenLegacyCursorDisabled_ActivatesOriginRowAndMovesCursorToEarliestSelectedStart()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2", "Row 3");
		Row originRow = timelineControl.Rows.ElementAt(1);
		Row previouslyActiveRow = timelineControl.Rows.ElementAt(2);
		Element laterElement = AddElement(originRow, TimeSpan.FromSeconds(14));
		Element earlierElement = AddElement(timelineControl.Rows.ElementAt(0), TimeSpan.FromSeconds(5));
		laterElement.Selected = true;
		earlierElement.Selected = true;
		previouslyActiveRow.Active = true;
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		timelineControl.grid.LegacyCursorActiveRow = false;

		FinalizeMoveCursorLassoSelection(timelineControl.grid, originRow, [laterElement, earlierElement]);

		Assert.True(originRow.Active);
		Assert.False(previouslyActiveRow.Active);
		Assert.Equal(earlierElement.StartTime, manager.CursorPosition);
	}

	[Fact]
	public void LassoFinalization_WhenLegacyCursorEnabled_DoesNotChangeActiveRowOrCursor()
	{
		TimelineControl timelineControl = CreateTimelineControl(out Guid id, "Row 1", "Row 2");
		Row originRow = timelineControl.Rows.ElementAt(0);
		Row previouslyActiveRow = timelineControl.Rows.ElementAt(1);
		Element selectedElement = AddElement(originRow, TimeSpan.FromSeconds(5));
		TimeSpan originalCursorPosition = TimeSpan.FromSeconds(3);
		selectedElement.Selected = true;
		previouslyActiveRow.Active = true;
		TimeLineGlobalStateManager manager = TimeLineGlobalStateManager.Manager(id);
		manager.CursorPosition = originalCursorPosition;
		timelineControl.grid.LegacyCursorActiveRow = true;

		FinalizeMoveCursorLassoSelection(timelineControl.grid, originRow, [selectedElement]);

		Assert.False(originRow.Active);
		Assert.True(previouslyActiveRow.Active);
		Assert.Equal(originalCursorPosition, manager.CursorPosition);
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

	private static void FinalizeMoveCursorLassoSelection(Grid grid, Row lassoOriginRow, IEnumerable<Element> selectedElements)
	{
		MethodInfo method = typeof(Grid).GetMethod("FinalizeMoveCursorLassoSelection", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(grid, [lassoOriginRow, selectedElements]);
	}
}
