using System.Drawing;
using Moq;
using Vixen.Marks;
using VixenModules.Effect.State;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Effect.State;

public class StateRenderPlannerTests
{
	[Fact]
	public void CreateStateItemIntervals_SelectedItemRendersAllItemsWithSameName()
	{
		// Arrange
		var selectedItemId = Guid.NewGuid();
		var matchingItem = CreateItem(selectedItemId, "Open");
		var duplicateNameItem = CreateItem(Guid.NewGuid(), "Open");
		var otherItem = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(matchingItem, duplicateNameItem, otherItem);

		// Act
		var intervals = StateRenderPlanner.CreateStateItemIntervals(
			definition,
			selectedItemId,
			PlaybackMode.Iterate,
			2,
			TimeSpan.FromSeconds(3));

		// Assert
		Assert.Equal([matchingItem.Id, duplicateNameItem.Id], intervals.Select(interval => interval.Item.Id));
		Assert.All(intervals, interval =>
		{
			Assert.Equal(TimeSpan.Zero, interval.Start);
			Assert.Equal(TimeSpan.FromSeconds(3), interval.Duration);
		});
	}

	[Fact]
	public void CreateStateItemIntervals_AllIterateUsesUniqueNamesInFirstRowOrder()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var openDuplicate = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, openDuplicate, closed);

		// Act
		var intervals = StateRenderPlanner.CreateStateItemIntervals(
			definition,
			Guid.Empty,
			PlaybackMode.Iterate,
			1,
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, openDuplicate.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(2)], intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(2), interval.Duration));
	}

	[Fact]
	public void CreateStateItemIntervals_AllIterateRepeatsUniqueNamesForIterations()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var openDuplicate = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, openDuplicate, closed);

		// Act
		var intervals = StateRenderPlanner.CreateStateItemIntervals(
			definition,
			Guid.Empty,
			PlaybackMode.Iterate,
			2,
			TimeSpan.FromSeconds(8));

		// Assert
		Assert.Equal(
			[open.Id, openDuplicate.Id, closed.Id, open.Id, openDuplicate.Id, closed.Id],
			intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[
				TimeSpan.Zero,
				TimeSpan.Zero,
				TimeSpan.FromSeconds(2),
				TimeSpan.FromSeconds(4),
				TimeSpan.FromSeconds(4),
				TimeSpan.FromSeconds(6)
			],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(2), interval.Duration));
	}

	[Fact]
	public void CreateMarkCollectionIntervals_DefaultClipsAndDeduplicatesRecognizedNames()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var openDuplicate = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var lowerCaseOpen = CreateItem(Guid.NewGuid(), "open");
		var definition = CreateDefinition(open, openDuplicate, closed, lowerCaseOpen);
		var marks = new[]
		{
			CreateMark(TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(4), " Open,Missing,Open,Closed, ,open ")
		};

		// Act
		var intervals = StateRenderPlanner.CreateMarkCollectionIntervals(
			definition,
			marks,
			PlaybackMode.Default,
			1,
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, openDuplicate.Id, closed.Id, lowerCaseOpen.Id], intervals.Select(interval => interval.Item.Id));
		Assert.All(intervals, interval =>
		{
			Assert.Equal(TimeSpan.Zero, interval.Start);
			Assert.Equal(TimeSpan.FromSeconds(3), interval.Duration);
		});
	}

	[Fact]
	public void CreateMarkCollectionIntervals_IteratePreservesUnknownAndEmptySegmentTiming()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var marks = new[]
		{
			CreateMark(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(4), "Open,Missing,,Closed")
		};

		// Act
		var intervals = StateRenderPlanner.CreateMarkCollectionIntervals(
			definition,
			marks,
			PlaybackMode.Iterate,
			1,
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Start));
		Assert.Equal([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)], intervals.Select(interval => interval.Duration));
	}

	[Fact]
	public void CreateMarkCollectionIntervals_IterateRepeatsSegmentsForIterations()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var marks = new[]
		{
			CreateMark(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(8), "Open,Closed")
		};

		// Act
		var intervals = StateRenderPlanner.CreateMarkCollectionIntervals(
			definition,
			marks,
			PlaybackMode.Iterate,
			2,
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(8));

		// Assert
		Assert.Equal([open.Id, closed.Id, open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6)],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(2), interval.Duration));
	}

	[Fact]
	public void CreateMarkCollectionIntervals_IterateRepeatedUnknownAndEmptySegmentsConsumeTiming()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var definition = CreateDefinition(open);
		var marks = new[]
		{
			CreateMark(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(6), "Open,Missing,")
		};

		// Act
		var intervals = StateRenderPlanner.CreateMarkCollectionIntervals(
			definition,
			marks,
			PlaybackMode.Iterate,
			2,
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(6));

		// Assert
		Assert.Equal([open.Id, open.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	[Fact]
	public void CreateMarkCollectionIntervals_OverlappingMarksBothGenerateIntervals()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var marks = new[]
		{
			CreateMark(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3), "Open"),
			CreateMark(TimeSpan.FromSeconds(11), TimeSpan.FromSeconds(3), "Closed")
		};

		// Act
		var intervals = StateRenderPlanner.CreateMarkCollectionIntervals(
			definition,
			marks,
			PlaybackMode.Default,
			1,
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(1)], intervals.Select(interval => interval.Start));
		Assert.Equal([TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_DefaultRendersValidRowsForFullDurationWithRowColors()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open", Color.Green);
		var closed = CreateItem(Guid.NewGuid(), "Closed", Color.Red);
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Default,
			1,
			TimeSpan.FromSeconds(5));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([Color.Blue, Color.Yellow], intervals.Select(interval => interval.ColorOverride));
		Assert.All(intervals, interval =>
		{
			Assert.Equal(TimeSpan.Zero, interval.Start);
			Assert.Equal(TimeSpan.FromSeconds(5), interval.Duration);
		});
	}

	[Fact]
	public void CreateCustomIntervals_NoDefinition_ReturnsEmpty()
	{
		// Arrange
		var customRows = new[]
		{
			CreateCustomRow(Guid.NewGuid(), Color.Blue)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			null,
			customRows,
			PlaybackMode.Default,
			1,
			TimeSpan.FromSeconds(5));

		// Assert
		Assert.Empty(intervals);
	}

	[Fact]
	public void CreateCustomIntervals_DefaultSkipsNoneMissingAndDuplicateRows()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.NewGuid(), Color.Red),
			CreateCustomRow(open.Id, Color.Yellow),
			CreateCustomRow(closed.Id, Color.Purple)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Default,
			1,
			TimeSpan.FromSeconds(5));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([Color.Blue, Color.Purple], intervals.Select(interval => interval.ColorOverride));
	}

	[Fact]
	public void CreateCustomIntervals_DefaultIgnoresCycleIndividually()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.NewGuid(), Color.Red),
			CreateCustomRow(open.Id, Color.Yellow),
			CreateCustomRow(closed.Id, Color.Purple)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Default,
			1,
			cycleIndividually: false,
			TimeSpan.FromSeconds(5));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([Color.Blue, Color.Purple], intervals.Select(interval => interval.ColorOverride));
		Assert.All(intervals, interval =>
		{
			Assert.Equal(TimeSpan.Zero, interval.Start);
			Assert.Equal(TimeSpan.FromSeconds(5), interval.Duration);
		});
	}

	[Fact]
	public void CreateCustomIntervals_IteratePreservesNoneAndMissingRowTiming()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(Guid.NewGuid(), Color.Red),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_IterateRepeatsRowsForIterations()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			2,
			TimeSpan.FromSeconds(8));

		// Assert
		Assert.Equal([open.Id, closed.Id, open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6)],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(2), interval.Duration));
		Assert.Equal([Color.Blue, Color.Yellow, Color.Blue, Color.Yellow], intervals.Select(interval => interval.ColorOverride));
	}

	[Fact]
	public void CreateCustomIntervals_IterateCycleIndividuallyPreservesRowTiming()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(Guid.NewGuid(), Color.Red),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			cycleIndividually: true,
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_GroupedIterateCollapsesConsecutiveNamesAndNone()
	{
		// Arrange
		var item2First = CreateItem(Guid.NewGuid(), "Item 2");
		var item2Second = CreateItem(Guid.NewGuid(), "Item 2");
		var item2Third = CreateItem(Guid.NewGuid(), "Item 2");
		var item4First = CreateItem(Guid.NewGuid(), "Item 4");
		var item4Second = CreateItem(Guid.NewGuid(), "Item 4");
		var item4Third = CreateItem(Guid.NewGuid(), "Item 4");
		var definition = CreateDefinition(item2First, item2Second, item2Third, item4First, item4Second, item4Third);
		var customRows = new[]
		{
			CreateCustomRow(item2First.Id, Color.Blue),
			CreateCustomRow(item2Second.Id, Color.Green),
			CreateCustomRow(item2Third.Id, Color.Red),
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(item4First.Id, Color.Yellow),
			CreateCustomRow(item4Second.Id, Color.Purple),
			CreateCustomRow(item4Third.Id, Color.Orange)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			cycleIndividually: false,
			TimeSpan.FromSeconds(6));

		// Assert
		Assert.Equal(
			[item2First.Id, item2Second.Id, item2Third.Id, item4First.Id, item4Second.Id, item4Third.Id],
			intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[Color.Blue, Color.Green, Color.Red, Color.Yellow, Color.Purple, Color.Orange],
			intervals.Select(interval => interval.ColorOverride));
		Assert.Equal(
			[
				TimeSpan.Zero,
				TimeSpan.Zero,
				TimeSpan.Zero,
				TimeSpan.FromSeconds(4),
				TimeSpan.FromSeconds(4),
				TimeSpan.FromSeconds(4)
			],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(2), interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_GroupedIterateDoesNotMergeNonConsecutiveNames()
	{
		// Arrange
		var item2First = CreateItem(Guid.NewGuid(), "Item 2");
		var item2Second = CreateItem(Guid.NewGuid(), "Item 2");
		var item5First = CreateItem(Guid.NewGuid(), "Item 5");
		var item2Third = CreateItem(Guid.NewGuid(), "Item 2");
		var item5Second = CreateItem(Guid.NewGuid(), "Item 5");
		var definition = CreateDefinition(item2First, item2Second, item5First, item2Third, item5Second);
		var customRows = new[]
		{
			CreateCustomRow(item2First.Id, Color.Blue),
			CreateCustomRow(item2Second.Id, Color.Green),
			CreateCustomRow(item5First.Id, Color.Red),
			CreateCustomRow(item2Third.Id, Color.Yellow),
			CreateCustomRow(item5Second.Id, Color.Purple)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			cycleIndividually: false,
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal(
			[item2First.Id, item2Second.Id, item5First.Id, item2Third.Id, item5Second.Id],
			intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_GroupedIterateDoesNotExpandToDefinitionItemsWithSameName()
	{
		// Arrange
		var selected = CreateItem(Guid.NewGuid(), "Open");
		var notSelected = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(selected, notSelected, closed);
		var customRows = new[]
		{
			CreateCustomRow(selected.Id, Color.Blue),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			cycleIndividually: false,
			TimeSpan.FromSeconds(2));

		// Assert
		Assert.Equal([selected.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.DoesNotContain(intervals, interval => interval.Item.Id == notSelected.Id);
	}

	[Fact]
	public void CreateCustomIntervals_GroupedIterateNoneAndMissingRowsConsumeTiming()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var missingId = Guid.NewGuid();
		var otherMissingId = Guid.NewGuid();
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(Guid.Empty, Color.Red),
			CreateCustomRow(missingId, Color.Green),
			CreateCustomRow(missingId, Color.Yellow),
			CreateCustomRow(otherMissingId, Color.Purple),
			CreateCustomRow(closed.Id, Color.Orange)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			1,
			cycleIndividually: false,
			TimeSpan.FromSeconds(5));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(4)], intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	[Fact]
	public void CreateCustomIntervals_GroupedIterateRepeatsGroupsForIterations()
	{
		// Arrange
		var open = CreateItem(Guid.NewGuid(), "Open");
		var closed = CreateItem(Guid.NewGuid(), "Closed");
		var definition = CreateDefinition(open, closed);
		var customRows = new[]
		{
			CreateCustomRow(open.Id, Color.Blue),
			CreateCustomRow(Guid.Empty, Color.White),
			CreateCustomRow(closed.Id, Color.Yellow)
		};

		// Act
		var intervals = StateRenderPlanner.CreateCustomIntervals(
			definition,
			customRows,
			PlaybackMode.Iterate,
			2,
			cycleIndividually: false,
			TimeSpan.FromSeconds(6));

		// Assert
		Assert.Equal([open.Id, closed.Id, open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal(
			[TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)],
			intervals.Select(interval => interval.Start));
		Assert.All(intervals, interval => Assert.Equal(TimeSpan.FromSeconds(1), interval.Duration));
	}

	private static StateDefinitionData CreateDefinition(params StateItemData[] items)
	{
		return new StateDefinitionData
		{
			Items = items.ToList()
		};
	}

	private static StateItemData CreateItem(Guid id, string name)
	{
		return CreateItem(id, name, Color.White);
	}

	private static StateItemData CreateItem(Guid id, string name, Color color)
	{
		return new StateItemData
		{
			Id = id,
			Name = name,
			Color = color
		};
	}

	private static CustomStateItemData CreateCustomRow(Guid stateItemId, Color color)
	{
		return new CustomStateItemData
		{
			StateItemId = stateItemId,
			Color = color
		};
	}

	private static IMark CreateMark(TimeSpan start, TimeSpan duration, string text)
	{
		var mark = new Mock<IMark>();
		mark.SetupGet(item => item.StartTime).Returns(start);
		mark.SetupGet(item => item.Duration).Returns(duration);
		mark.SetupGet(item => item.EndTime).Returns(start + duration);
		mark.SetupGet(item => item.Text).Returns(text);
		return mark.Object;
	}
}
