using VixenModules.Property.State.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.Property.State;

public sealed class StateDefinitionNameDialogViewModelTests
{
	[Fact]
	public void EmptyName_DisablesOkCommand()
	{
		// Arrange
		var viewModel = new StateDefinitionNameDialogViewModel(
			"Rename",
			"Open",
			["Open"],
			"Open");

		// Act
		viewModel.Name = "   ";

		// Assert
		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ExactDuplicateName_DisablesOkCommand()
	{
		// Arrange
		var viewModel = new StateDefinitionNameDialogViewModel(
			"Rename",
			"Closed",
			["Open", "Closed"],
			"Closed");

		// Act
		viewModel.Name = "Open";

		// Assert
		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void CaseOnlyDuplicateName_ShowsWarningWithoutBlockingOkCommand()
	{
		// Arrange
		var viewModel = new StateDefinitionNameDialogViewModel(
			"Rename",
			"Closed",
			["Open", "Closed"],
			"Closed");

		// Act
		viewModel.Name = "open";

		// Assert
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
		Assert.NotEmpty(viewModel.Warning);
	}
}
