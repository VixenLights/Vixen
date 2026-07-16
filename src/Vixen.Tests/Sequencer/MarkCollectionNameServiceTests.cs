using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class MarkCollectionNameServiceTests
{
	[Fact]
	public void GetUniqueName_NoExistingCollection_ReturnsBaseName()
	{
		// Arrange
		var collections = new ObservableCollection<IMarkCollection>();

		// Act
		var result = MarkCollectionNameService.GetUniqueName("Mark Collection", collections);

		// Assert
		Assert.Equal("Mark Collection", result);
	}

	[Fact]
	public void GetUniqueName_BaseNameExists_ReturnsSuffix2()
	{
		// Arrange
		var collections = CreateCollections("Mark Collection");

		// Act
		var result = MarkCollectionNameService.GetUniqueName("Mark Collection", collections);

		// Assert
		Assert.Equal("Mark Collection - 2", result);
	}

	[Fact]
	public void GetUniqueName_Suffix2Exists_ReturnsSuffix3()
	{
		// Arrange
		var collections = CreateCollections("Mark Collection", "Mark Collection - 2");

		// Act
		var result = MarkCollectionNameService.GetUniqueName("Mark Collection", collections);

		// Assert
		Assert.Equal("Mark Collection - 3", result);
	}

	[Fact]
	public void IsUniqueName_CaseOnlyDuplicate_ReturnsFalse()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks");

		// Act
		var result = MarkCollectionNameService.IsUniqueName(collections, "beat marks");

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void IsUniqueName_WhitespaceOnlyDuplicate_ReturnsFalse()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks");

		// Act
		var result = MarkCollectionNameService.IsUniqueName(collections, " Beat Marks ");

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void IsUniqueName_ExcludingCurrentCollection_ReturnsTrue()
	{
		// Arrange
		var collection = CreateCollection("Beat Marks");
		var collections = new ObservableCollection<IMarkCollection> { collection };

		// Act
		var result = MarkCollectionNameService.IsUniqueName(collections, "Beat Marks", collection.Id);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void RenameDuplicates_DuplicateNames_RenamesLaterDuplicates()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks", "Beat Marks", "Beat Marks");

		// Act
		MarkCollectionNameService.RenameDuplicates(collections);

		// Assert
		Assert.Equal(["Beat Marks", "Beat Marks - 2", "Beat Marks - 3"], collections.Select(collection => collection.Name));
	}

	[Fact]
	public void RenameDuplicates_SuffixAlreadyExists_SkipsOccupiedSuffix()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks", "Beat Marks - 2", "Beat Marks");

		// Act
		MarkCollectionNameService.RenameDuplicates(collections);

		// Assert
		Assert.Equal(["Beat Marks", "Beat Marks - 2", "Beat Marks - 3"], collections.Select(collection => collection.Name));
	}

	[Fact]
	public void RenameDuplicates_DuplicateNames_ReturnsRenameRecords()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks", "Beat Marks", "Beat Marks");

		// Act
		var records = MarkCollectionNameService.RenameDuplicates(collections);

		// Assert
		Assert.Equal(["Beat Marks - 2", "Beat Marks - 3"], records.Select(record => record.NewName));
	}

	[Fact]
	public void RenameDuplicates_AlreadyUniqueNames_ReturnsNoRecords()
	{
		// Arrange
		var collections = CreateCollections("Beat Marks", "Phrase Marks");

		// Act
		var records = MarkCollectionNameService.RenameDuplicates(collections);

		// Assert
		Assert.Empty(records);
	}

	[Fact]
	public void RenameDuplicates_BlankNameUsesAvailableDefaultName()
	{
		// Arrange
		var collections = CreateCollections(string.Empty, "Mark Collection");

		// Act
		MarkCollectionNameService.RenameDuplicates(collections);

		// Assert
		Assert.Equal(["Mark Collection - 2", "Mark Collection"], collections.Select(collection => collection.Name));
	}

	[Fact]
	public void GetUniqueName_NullCollections_ThrowsArgumentNullException()
	{
		// Arrange
		IEnumerable<IMarkCollection> collections = null!;

		// Act
		var exception = Assert.Throws<ArgumentNullException>(() => MarkCollectionNameService.GetUniqueName("Mark Collection", collections));

		// Assert
		Assert.Equal("collections", exception.ParamName);
	}

	private static ObservableCollection<IMarkCollection> CreateCollections(params string[] names)
	{
		return new ObservableCollection<IMarkCollection>(names.Select(CreateCollection));
	}

	private static IMarkCollection CreateCollection(string name)
	{
		return new MarkCollection { Name = name };
	}
}
