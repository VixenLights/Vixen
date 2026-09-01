using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class MarkCollectionSelectionServiceTests
{
	private readonly MarkCollectionSelectionService _service = new();

	[Fact]
	public void Normalize_OrdinaryActiveSelection_UsesFirstCollection()
	{
		// Arrange
		var first = CreateCollection("First", MarkCollectionType.Generic);
		var second = CreateCollection("Second", MarkCollectionType.State);
		var selection = new TestSelection { IsActive = true, AllowsFirstCollectionFallback = true };

		// Act
		var result = _service.Normalize([first, second], selection);

		// Assert
		Assert.Equal(first.Id, result);
	}

	[Fact]
	public void Normalize_StateSelection_PrefersFirstStateCollection()
	{
		// Arrange
		var generic = CreateCollection("Generic", MarkCollectionType.Generic);
		var firstState = CreateCollection("First State", MarkCollectionType.State);
		var secondState = CreateCollection("Second State", MarkCollectionType.State);
		var selection = new TestSelection
		{
			IsActive = true,
			PreferredCollectionType = MarkCollectionType.State,
			AllowsFirstCollectionFallback = true
		};

		// Act
		var result = _service.Normalize([generic, firstState, secondState], selection);

		// Assert
		Assert.Equal(firstState.Id, result);
	}

	[Fact]
	public void Normalize_PreferredSelectionWithoutMatch_UsesFirstCollectionWhenAllowed()
	{
		// Arrange
		var first = CreateCollection("First", MarkCollectionType.Generic);
		var selection = new TestSelection
		{
			IsActive = true,
			PreferredCollectionType = MarkCollectionType.State,
			AllowsFirstCollectionFallback = true
		};

		// Act
		var result = _service.Normalize([first], selection);

		// Assert
		Assert.Equal(first.Id, result);
	}

	[Fact]
	public void Normalize_PhonemeSelectionWithoutMatch_ReturnsEmpty()
	{
		// Arrange
		var selection = new TestSelection
		{
			IsActive = true,
			PreferredCollectionType = MarkCollectionType.Phoneme,
			AllowsFirstCollectionFallback = false
		};

		// Act
		var result = _service.Normalize([CreateCollection("Generic", MarkCollectionType.Generic)], selection);

		// Assert
		Assert.Equal(Guid.Empty, result);
	}

	[Theory]
	[InlineData(null, true)]
	[InlineData(MarkCollectionType.State, true)]
	[InlineData(MarkCollectionType.Phoneme, false)]
	public void Normalize_ValidSelection_PreservesIdRegardlessOfPolicy(MarkCollectionType? preferredCollectionType, bool allowsFirstCollectionFallback)
	{
		// Arrange
		var selected = CreateCollection("Existing Generic", MarkCollectionType.Generic);
		var preferred = CreateCollection("Preferred", MarkCollectionType.State);
		var selection = new TestSelection
		{
			IsActive = true,
			MarkCollectionId = selected.Id,
			PreferredCollectionType = preferredCollectionType,
			AllowsFirstCollectionFallback = allowsFirstCollectionFallback
		};

		// Act
		var result = _service.Normalize([preferred, selected], selection);

		// Assert
		Assert.Equal(selected.Id, result);
	}

	[Fact]
	public void Normalize_MissingNonEmptyId_RepairsSelection()
	{
		// Arrange
		var first = CreateCollection("First", MarkCollectionType.Generic);
		var selection = new TestSelection
		{
			IsActive = true,
			MarkCollectionId = Guid.NewGuid(),
			AllowsFirstCollectionFallback = true
		};

		// Act
		var result = _service.Normalize([first], selection);

		// Assert
		Assert.Equal(first.Id, result);
	}

	[Fact]
	public void Normalize_NoCollectionsOrInactiveSelection_RetainsRequiredResult()
	{
		// Arrange
		var activeSelection = new TestSelection { IsActive = true, MarkCollectionId = Guid.NewGuid(), AllowsFirstCollectionFallback = true };
		var inactiveId = Guid.NewGuid();
		var inactiveSelection = new TestSelection { MarkCollectionId = inactiveId };

		// Act
		var activeResult = _service.Normalize([], activeSelection);
		var inactiveResult = _service.Normalize([], inactiveSelection);

		// Assert
		Assert.Equal(Guid.Empty, activeResult);
		Assert.Equal(inactiveId, inactiveResult);
	}

	[Fact]
	public void Normalize_DoesNotModifySharedCollections()
	{
		// Arrange
		var first = CreateCollection("First", MarkCollectionType.Generic);
		first.LinkedMarkCollectionId = Guid.NewGuid();
		var second = CreateCollection("Second", MarkCollectionType.State);
		second.LinkedMarkCollectionId = Guid.NewGuid();
		var collections = new ObservableCollection<IMarkCollection> { first, second };
		var snapshot = collections.Select(collection => (collection.Id, collection.Name, collection.CollectionType, collection.LinkedMarkCollectionId, collection.Marks.Count)).ToArray();
		var selection = new TestSelection { IsActive = true, PreferredCollectionType = MarkCollectionType.State, AllowsFirstCollectionFallback = true };

		// Act
		_ = _service.Normalize(collections, selection);

		// Assert
		Assert.Equal(snapshot, collections.Select(collection => (collection.Id, collection.Name, collection.CollectionType, collection.LinkedMarkCollectionId, collection.Marks.Count)).ToArray());
	}

	private static MarkCollection CreateCollection(string name, MarkCollectionType collectionType)
	{
		return new MarkCollection
		{
			Id = Guid.NewGuid(),
			Name = name,
			CollectionType = collectionType
		};
	}

	private sealed class TestSelection : IMarkCollectionSelection
	{
		public bool IsActive { get; init; }
		public Guid MarkCollectionId { get; set; }
		public MarkCollectionType? PreferredCollectionType { get; init; }
		public bool AllowsFirstCollectionFallback { get; init; }
	}
}
