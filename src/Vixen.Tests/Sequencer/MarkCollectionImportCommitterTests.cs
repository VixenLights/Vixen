using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class MarkCollectionImportCommitterTests
{
	[Fact]
	public void Commit_AppendsCandidatesInOrderWithSequentialUniqueNames()
	{
		// Arrange
		var target = CreateCollections("Beat Marks");
		var first = CreateCollection("Beat Marks");
		var second = CreateCollection("Beat Marks");

		// Act
		MarkCollectionImportCommitter.Commit(target, [first, second]);

		// Assert
		Assert.Equal(["Beat Marks", "Beat Marks - 2", "Beat Marks - 3"], target.Select(collection => collection.Name));
		Assert.Same(first, target[1]);
		Assert.Same(second, target[2]);
	}

	[Fact]
	public void Commit_ExistingDefault_ClearsImportedDefaults()
	{
		// Arrange
		var existingDefault = CreateCollection("Existing", isDefault: true);
		var target = new ObservableCollection<IMarkCollection> { existingDefault };
		var candidate = CreateCollection("Imported", isDefault: true);

		// Act
		MarkCollectionImportCommitter.Commit(target, [candidate]);

		// Assert
		Assert.True(existingDefault.IsDefault);
		Assert.False(candidate.IsDefault);
	}

	[Fact]
	public void Commit_NoExistingDefault_PreservesOnlyFirstImportedDefault()
	{
		// Arrange
		var target = new ObservableCollection<IMarkCollection>();
		var first = CreateCollection("First", isDefault: true);
		var second = CreateCollection("Second", isDefault: true);

		// Act
		MarkCollectionImportCommitter.Commit(target, [first, second]);

		// Assert
		Assert.True(first.IsDefault);
		Assert.False(second.IsDefault);
	}

	[Fact]
	public void Commit_NoDefault_UsesFirstVisibleCandidateThenFirstCandidate()
	{
		// Arrange
		var visibleTarget = new ObservableCollection<IMarkCollection>();
		var hidden = CreateCollection("Hidden");
		var visible = CreateCollection("Visible", isVisible: true);
		var hiddenTarget = new ObservableCollection<IMarkCollection>();
		var firstHidden = CreateCollection("First hidden");
		var secondHidden = CreateCollection("Second hidden");

		// Act
		MarkCollectionImportCommitter.Commit(visibleTarget, [hidden, visible]);
		MarkCollectionImportCommitter.Commit(hiddenTarget, [firstHidden, secondHidden]);

		// Assert
		Assert.False(hidden.IsDefault);
		Assert.True(visible.IsDefault);
		Assert.True(firstHidden.IsDefault);
		Assert.False(secondHidden.IsDefault);
	}

	[Fact]
	public void Commit_PreservesLinksToExistingAndSelectedCollections()
	{
		// Arrange
		var existingParent = CreateCollection("Existing parent");
		var target = new ObservableCollection<IMarkCollection> { existingParent };
		var selectedParent = CreateCollection("Selected parent");
		var childOfExisting = CreateCollection("Existing child", linkedMarkCollectionId: existingParent.Id);
		var childOfSelected = CreateCollection("Selected child", linkedMarkCollectionId: selectedParent.Id);

		// Act
		MarkCollectionImportCommitter.Commit(target, [selectedParent, childOfExisting, childOfSelected]);

		// Assert
		Assert.Equal(existingParent.Id, childOfExisting.LinkedMarkCollectionId);
		Assert.Equal(selectedParent.Id, childOfSelected.LinkedMarkCollectionId);
	}

	[Fact]
	public void Commit_ExcludedLinkedParent_ClearsImportedChildLink()
	{
		// Arrange
		var excludedParent = CreateCollection("Excluded parent");
		var child = CreateCollection("Child", linkedMarkCollectionId: excludedParent.Id);
		var target = new ObservableCollection<IMarkCollection>();

		// Act
		MarkCollectionImportCommitter.Commit(target, [child]);

		// Assert
		Assert.Equal(Guid.Empty, child.LinkedMarkCollectionId);
		Assert.DoesNotContain(excludedParent, target);
	}

	[Fact]
	public void Commit_NoCandidates_DoesNotMutateTarget()
	{
		// Arrange
		var existing = CreateCollection("Existing", isDefault: true);
		var target = new ObservableCollection<IMarkCollection> { existing };

		// Act
		MarkCollectionImportCommitter.Commit(target, []);

		// Assert
		Assert.Same(existing, Assert.Single(target));
		Assert.Equal("Existing", existing.Name);
		Assert.True(existing.IsDefault);
	}

	[Fact]
	public void Commit_NullCandidate_ThrowsBeforeMutatingTarget()
	{
		// Arrange
		var existing = CreateCollection("Existing");
		var target = new ObservableCollection<IMarkCollection> { existing };
		IEnumerable<IMarkCollection> candidates = [null!];

		// Act
		var exception = Assert.Throws<ArgumentException>(() => MarkCollectionImportCommitter.Commit(target, candidates));

		// Assert
		Assert.Equal("candidates", exception.ParamName);
		Assert.Same(existing, Assert.Single(target));
	}

	private static ObservableCollection<IMarkCollection> CreateCollections(params string[] names)
	{
		return new ObservableCollection<IMarkCollection>(names.Select(name => CreateCollection(name)));
	}

	private static MarkCollection CreateCollection(string name, bool isDefault = false, bool isVisible = false, Guid? linkedMarkCollectionId = null)
	{
		return new MarkCollection
		{
			Name = name,
			IsDefault = isDefault,
			ShowGridLines = isVisible,
			ShowMarkBar = isVisible,
			LinkedMarkCollectionId = linkedMarkCollectionId ?? Guid.Empty
		};
	}
}
