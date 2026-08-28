using System.Collections.ObjectModel;
using TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels;
using Vixen.Marks;
using VixenModules.App.Marks;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class MarkCollectionImportSelectionViewModelTests
{
	[Fact]
	public void Constructor_SelectsAllCandidatesAndTheFirstRow()
	{
		var first = CreateCollection("First");
		var second = CreateCollection("Second");

		var viewModel = CreateViewModel([], [first, second]);

		Assert.Equal([first, second], viewModel.Options.Select(option => option.Candidate));
		Assert.All(viewModel.Options, option => Assert.True(option.IsIncluded));
		Assert.Same(viewModel.Options[0], viewModel.SelectedOption);
		Assert.True(viewModel.HasIncludedOptions);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void Constructor_IndicatesCaseAndTrimDuplicateWithoutRenamingCandidate()
	{
		var existing = CreateCollection(" Existing Marks ");
		var candidate = CreateCollection("existing marks");

		var viewModel = CreateViewModel([existing], [candidate]);
		var option = Assert.Single(viewModel.Options);

		Assert.True(option.HasDuplicateName);
		Assert.Equal("Name already exists in this sequence.", option.DuplicateNameMessage);
		Assert.Equal("existing marks", candidate.Name);
	}

	[Fact]
	public void ToggleSelectedOption_TogglesSelectedRowAndUpdatesOkState()
	{
		var first = CreateCollection("First");
		var second = CreateCollection("Second");
		var viewModel = CreateViewModel([], [first, second]);
		viewModel.SelectedOption = viewModel.Options[1];
		viewModel.Options[0].IsIncluded = false;

		viewModel.ToggleSelectedOptionCommand.Execute(null);

		Assert.False(viewModel.Options[1].IsIncluded);
		Assert.False(viewModel.HasIncludedOptions);
		Assert.False(viewModel.OkCommand.CanExecute(null));

		viewModel.ToggleSelectedOptionCommand.Execute(null);

		Assert.True(viewModel.Options[1].IsIncluded);
		Assert.True(viewModel.HasIncludedOptions);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ToggleSelectedOption_WithNoSelectedRow_DoesNothing()
	{
		var candidate = CreateCollection("Candidate");
		var viewModel = CreateViewModel([], [candidate]);
		viewModel.SelectedOption = null;

		viewModel.ToggleSelectedOptionCommand.Execute(null);

		Assert.True(viewModel.Options[0].IsIncluded);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void OkCommand_CapturesOnlyIncludedCandidatesWithoutMutation()
	{
		var live = CreateCollection("Live");
		var first = CreateCollection("First");
		var second = CreateCollection("Second");
		var target = new ObservableCollection<IMarkCollection> { live };
		var viewModel = CreateViewModel(target, [first, second]);
		viewModel.Options[1].IsIncluded = false;

		viewModel.OkCommand.Execute(null);

		Assert.Equal([first], viewModel.SelectedCollections);
		Assert.Same(live, Assert.Single(target));
		Assert.Equal("First", first.Name);
		Assert.Equal("Second", second.Name);
	}

	[Fact]
	public void CancelCommand_LeavesTargetAndCandidatesUnchanged()
	{
		var live = CreateCollection("Live");
		var candidate = CreateCollection("Candidate");
		var target = new ObservableCollection<IMarkCollection> { live };
		var viewModel = CreateViewModel(target, [candidate]);

		viewModel.CancelCommand.Execute(null);

		Assert.Empty(viewModel.SelectedCollections);
		Assert.Same(live, Assert.Single(target));
		Assert.Equal("Candidate", candidate.Name);
	}

	private static MarkCollectionImportSelectionViewModel CreateViewModel(
		IEnumerable<IMarkCollection> existing,
		IEnumerable<IMarkCollection> candidates)
	{
		return new MarkCollectionImportSelectionViewModel(existing, candidates);
	}

	private static MarkCollection CreateCollection(string name)
	{
		return new MarkCollection { Name = name };
	}
}
