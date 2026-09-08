using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Effect.LipSync;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class LipSyncMarkCollectionNameConverterTests
{
	[Fact]
	public void GetAllowedMarkCollectionNames_NoSelectedCollection_ReturnsOnlyPhonemeCollections()
	{
		// Arrange
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			CreateCollection("Words", MarkCollectionType.Word),
			CreateCollection("Phonemes", MarkCollectionType.Phoneme)
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, Guid.Empty);

		// Assert
		Assert.Equal(["Phonemes"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_SelectedGenericCollection_IncludesSelectedCollectionInSequenceOrder()
	{
		// Arrange
		var selectedCollection = CreateCollection("Old Manual Track", MarkCollectionType.Generic);
		var collections = new[]
		{
			selectedCollection,
			CreateCollection("Phonemes", MarkCollectionType.Phoneme),
			CreateCollection("Words", MarkCollectionType.Word)
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, selectedCollection.Id);

		// Assert
		Assert.Equal(["Old Manual Track", "Phonemes"], result);
	}

	[Fact]
	public void LipSync_ImplementsSelectorContractAndPreservesRenamedLegacySelection()
	{
		// Arrange
		var selectedCollection = CreateCollection("Old Manual Track", MarkCollectionType.Generic);
		var collections = new ObservableCollection<IMarkCollection>
		{
			selectedCollection,
			CreateCollection("Phonemes", MarkCollectionType.Phoneme),
			CreateCollection("Words", MarkCollectionType.Word)
		};
		var effect = new LipSync { MarkCollections = collections };
		effect.MarkCollectionId = selectedCollection.Name;
		var propertyNames = new List<string>();
		effect.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName ?? string.Empty);

		// Act
		selectedCollection.Name = "Renamed Manual Track";
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, selectedCollection.Id);

		// Assert
		Assert.True(typeof(IMarkCollectionSelector).IsAssignableFrom(typeof(LipSync)));
		Assert.Contains(nameof(IMarkCollectionSelector.MarkCollectionId), propertyNames);
		Assert.Equal("Renamed Manual Track", effect.MarkCollectionId);
		Assert.Equal(["Renamed Manual Track", "Phonemes"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_SelectedWordCollection_IncludesSelectedCollection()
	{
		// Arrange
		var selectedCollection = CreateCollection("Words", MarkCollectionType.Word);
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			selectedCollection,
			CreateCollection("Phonemes", MarkCollectionType.Phoneme)
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, selectedCollection.Id);

		// Assert
		Assert.Equal(["Words", "Phonemes"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_SelectedPhraseCollection_IncludesSelectedCollection()
	{
		// Arrange
		var selectedCollection = CreateCollection("Phrases", MarkCollectionType.Phrase);
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			CreateCollection("Phonemes", MarkCollectionType.Phoneme),
			selectedCollection
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, selectedCollection.Id);

		// Assert
		Assert.Equal(["Phonemes", "Phrases"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_SelectedPhonemeCollection_DoesNotDuplicateSelectedCollection()
	{
		// Arrange
		var selectedCollection = CreateCollection("Phonemes", MarkCollectionType.Phoneme);
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			selectedCollection
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, selectedCollection.Id);

		// Assert
		Assert.Equal(["Phonemes"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_NoPhonemeCollectionsAndNoSelectedCollection_ReturnsEmptyList()
	{
		// Arrange
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			CreateCollection("Words", MarkCollectionType.Word),
			CreateCollection("Phrases", MarkCollectionType.Phrase)
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, Guid.Empty);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_MissingSelectedCollection_DoesNotAddSyntheticValue()
	{
		// Arrange
		var collections = new[]
		{
			CreateCollection("Beat Marks", MarkCollectionType.Generic),
			CreateCollection("Phonemes", MarkCollectionType.Phoneme)
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, Guid.NewGuid());

		// Assert
		Assert.Equal(["Phonemes"], result);
	}

	[Fact]
	public void GetAllowedMarkCollectionNames_AfterSelectingPhonemeCollection_RemovesLegacyException()
	{
		// Arrange
		var phonemeCollection = CreateCollection("Phonemes", MarkCollectionType.Phoneme);
		var collections = new[]
		{
			CreateCollection("Old Manual Track", MarkCollectionType.Generic),
			phonemeCollection
		};

		// Act
		var result = LipSyncMarkCollectionNameConverter.GetAllowedMarkCollectionNames(collections, phonemeCollection.Id);

		// Assert
		Assert.Equal(["Phonemes"], result);
	}

	private static IMarkCollection CreateCollection(string name, MarkCollectionType collectionType)
	{
		return new MarkCollection
		{
			Name = name,
			CollectionType = collectionType
		};
	}
}
