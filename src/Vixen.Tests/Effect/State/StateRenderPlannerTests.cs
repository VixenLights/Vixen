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
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, openDuplicate.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(2)], intervals.Select(interval => interval.Start));
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
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Start));
		Assert.Equal([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)], intervals.Select(interval => interval.Duration));
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
			TimeSpan.FromSeconds(10),
			TimeSpan.FromSeconds(4));

		// Assert
		Assert.Equal([open.Id, closed.Id], intervals.Select(interval => interval.Item.Id));
		Assert.Equal([TimeSpan.Zero, TimeSpan.FromSeconds(1)], intervals.Select(interval => interval.Start));
		Assert.Equal([TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3)], intervals.Select(interval => interval.Duration));
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
		return new StateItemData
		{
			Id = id,
			Name = name,
			Color = Color.White
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
