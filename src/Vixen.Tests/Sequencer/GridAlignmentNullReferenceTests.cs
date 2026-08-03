using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class GridAlignmentNullReferenceTests
{
	[Fact]
	public void AlignElementMethods_WhenReferenceElementIsNull_DoNotChangeElementTiming()
	{
		TimelineControl timelineControl = CreateTimelineControl();
		Row row = timelineControl.Rows.Single();
		Element firstElement = AddElement(row, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(2));
		Element secondElement = AddElement(row, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
		Element[] selectedElements = [firstElement, secondElement];
		var originalTimings = selectedElements
			.Select(element => (Element: element, element.StartTime, element.EndTime, element.Duration))
			.ToArray();

		Exception? exception = Record.Exception(() =>
		{
			timelineControl.grid.AlignElementStartTimes(selectedElements, null, false);
			timelineControl.grid.AlignElementEndTimes(selectedElements, null, false);
			timelineControl.grid.AlignElementDurations(selectedElements, null, false);
			timelineControl.grid.AlignElementStartEndTimes(selectedElements, null);
			timelineControl.grid.AlignElementStartToEndTimes(selectedElements, null, false);
			timelineControl.grid.AlignElementEndToStartTime(selectedElements, null, false);
			timelineControl.grid.AlignElementCenters(selectedElements, null);
		});

		Assert.Null(exception);
		foreach (var originalTiming in originalTimings)
		{
			Assert.Equal(originalTiming.StartTime, originalTiming.Element.StartTime);
			Assert.Equal(originalTiming.EndTime, originalTiming.Element.EndTime);
			Assert.Equal(originalTiming.Duration, originalTiming.Element.Duration);
		}
	}

	private static TimelineControl CreateTimelineControl()
	{
		TimelineControl timelineControl = new TimelineControl(Guid.NewGuid());
		Row row = timelineControl.AddRow("Row 1");
		row.Visible = true;
		timelineControl.grid.ResizeGridHeight();
		timelineControl.LayoutRows();
		return timelineControl;
	}

	private static Element AddElement(Row row, TimeSpan startTime, TimeSpan duration)
	{
		Element element = new Element
		{
			StartTime = startTime,
			Duration = duration
		};
		row.AddElement(element);
		element.Selected = true;
		return element;
	}
}
