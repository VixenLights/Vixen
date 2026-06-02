using VixenModules.Property.State.Setup.Preview;
using Xunit;

namespace Vixen.Tests.Property.State;

public sealed class StatePreviewCoordinatorTests
{
	[Fact]
	public void Refresh_WithEmptyDesiredSet_DoesNotPublish()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);

		// Act
		coordinator.Refresh([]);

		// Assert
		Assert.Empty(publisher.Operations);
	}

	[Fact]
	public void Refresh_WithNewPairs_PublishesOneTurnOnOperation()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var pair = CreatePair("#FF0000");

		// Act
		coordinator.Refresh([pair]);

		// Assert
		var operation = Assert.Single(publisher.Operations);
		Assert.Equal("TurnOn", operation.Name);
		Assert.Equal([pair], operation.Pairs);
	}

	[Fact]
	public void Refresh_WithDuplicatePairs_ActivatesPairOnce()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var pair = CreatePair("#FF0000");

		// Act
		coordinator.Refresh([pair, pair]);

		// Assert
		var operation = Assert.Single(publisher.Operations);
		Assert.Equal([pair], operation.Pairs);
	}

	[Fact]
	public void Refresh_WithDifferentColorsForSameElement_PreservesBothPairs()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var red = CreatePair("#FF0000");
		var green = red with { Color = "#00FF00" };

		// Act
		coordinator.Refresh([red, green]);

		// Assert
		var operation = Assert.Single(publisher.Operations);
		Assert.Equal([red, green], operation.Pairs);
	}

	[Fact]
	public void Refresh_WhenOneColorIsRemoved_TurnsOffElementAndReactivatesRemainingColor()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var red = CreatePair("#FF0000");
		var green = red with { Color = "#00FF00" };
		coordinator.Refresh([red, green]);
		publisher.Operations.Clear();

		// Act
		coordinator.Refresh([green]);

		// Assert
		Assert.Collection(
			publisher.Operations,
			operation =>
			{
				Assert.Equal("TurnOff", operation.Name);
				Assert.Equal([red.ElementId], operation.ElementIds);
			},
			operation =>
			{
				Assert.Equal("TurnOn", operation.Name);
				Assert.Equal([green], operation.Pairs);
			});
	}

	[Fact]
	public void Refresh_WithRemovalAndAddition_PublishesTurnOffBeforeTurnOn()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var removed = CreatePair("#FF0000");
		var added = CreatePair("#00FF00");
		coordinator.Refresh([removed]);
		publisher.Operations.Clear();

		// Act
		coordinator.Refresh([added]);

		// Assert
		Assert.Collection(
			publisher.Operations,
			operation => Assert.Equal("TurnOff", operation.Name),
			operation => Assert.Equal("TurnOn", operation.Name));
	}

	[Fact]
	public void Refresh_WithRemovedColorsForSameElement_TurnsOffElementOnce()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var red = CreatePair("#FF0000");
		var green = red with { Color = "#00FF00" };
		coordinator.Refresh([red, green]);
		publisher.Operations.Clear();

		// Act
		coordinator.Refresh([]);

		// Assert
		var operation = Assert.Single(publisher.Operations);
		Assert.Equal("TurnOff", operation.Name);
		Assert.Equal([red.ElementId], operation.ElementIds);
	}

	[Fact]
	public void Refresh_WithAlreadyActivePairs_DoesNotPublish()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var pair = CreatePair("#FF0000");
		coordinator.Refresh([pair]);
		publisher.Operations.Clear();

		// Act
		coordinator.Refresh([pair]);

		// Assert
		Assert.Empty(publisher.Operations);
	}

	[Fact]
	public void Clear_PublishesEveryTimeAndResetsActivePairs()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var pair = CreatePair("#FF0000");
		coordinator.Refresh([pair]);
		publisher.Operations.Clear();

		// Act
		coordinator.Clear();
		coordinator.Clear();
		coordinator.Refresh([pair]);

		// Assert
		Assert.Collection(
			publisher.Operations,
			operation => Assert.Equal("Clear", operation.Name),
			operation => Assert.Equal("Clear", operation.Name),
			operation => Assert.Equal("TurnOn", operation.Name));
	}

	[Fact]
	public void Release_PublishesEveryTimeAndResetsActivePairs()
	{
		// Arrange
		var publisher = new RecordingStatePreviewPublisher();
		var coordinator = new StatePreviewCoordinator(publisher);
		var pair = CreatePair("#FF0000");
		coordinator.Refresh([pair]);
		publisher.Operations.Clear();

		// Act
		coordinator.Release();
		coordinator.Release();
		coordinator.Refresh([pair]);

		// Assert
		Assert.Collection(
			publisher.Operations,
			operation => Assert.Equal("Release", operation.Name),
			operation => Assert.Equal("Release", operation.Name),
			operation => Assert.Equal("TurnOn", operation.Name));
	}

	[Fact]
	public void Constructor_WithNullPublisher_Throws()
	{
		// Act and assert
		Assert.Throws<ArgumentNullException>(() => new StatePreviewCoordinator(null!));
	}

	[Fact]
	public void Refresh_WithNullDesiredPairs_Throws()
	{
		// Arrange
		var coordinator = new StatePreviewCoordinator(new RecordingStatePreviewPublisher());

		// Act and assert
		Assert.Throws<ArgumentNullException>(() => coordinator.Refresh(null!));
	}

	private static StatePreviewPair CreatePair(string color) => new(Guid.NewGuid(), color);

	private sealed class RecordingStatePreviewPublisher : IStatePreviewPublisher
	{
		internal List<PreviewOperation> Operations { get; } = [];

		public void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs)
		{
			Operations.Add(new PreviewOperation("TurnOn", pairs.ToArray(), []));
		}

		public void TurnOff(IReadOnlyCollection<Guid> elementIds)
		{
			Operations.Add(new PreviewOperation("TurnOff", [], elementIds.ToArray()));
		}

		public void Clear()
		{
			Operations.Add(new PreviewOperation("Clear", [], []));
		}

		public void Release()
		{
			Operations.Add(new PreviewOperation("Release", [], []));
		}
	}

	private sealed record PreviewOperation(
		string Name,
		IReadOnlyCollection<StatePreviewPair> Pairs,
		IReadOnlyCollection<Guid> ElementIds);
}
