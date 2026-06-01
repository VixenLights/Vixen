using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.Property.State;

[Collection(StateMapperTestCollection.Name)]
public class StateMapperValidationTests
{
	[Fact]
	public void Constructor_ValidatesExistingInvalidData()
	{
		// Arrange
		var source = CreateSource("   ", "   ");

		// Act
		var viewModel = CreateViewModel(source);

		// Assert
		Assert.True(viewModel.HasErrors);
		Assert.True(viewModel.Items[0].HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void Constructor_InitializesAssignmentsBeforeCatelProperties()
	{
		// Arrange
		var source = CreateSource("Operating Mode", "Enabled");

		// Act
		var viewModel = CreateViewModel(source);

		// Assert
		Assert.NotNull(viewModel.Items[0].AssignmentRoots);
		Assert.Equal(0, viewModel.Items[0].AssignmentCount);
	}

	[Fact]
	public void Name_TrimsWhitespaceAndEnablesOkAfterCorrection()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled"));
		viewModel.Name = "   ";

		// Act
		viewModel.Name = "  Running  ";

		// Assert
		Assert.Equal("Running", viewModel.Name);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ShortName_AddsWarningWithoutBlockingOk()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled"));

		// Act
		viewModel.Name = "Go";

		// Assert
		Assert.True(viewModel.HasWarnings);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public async Task AddItem_CreatesValidDefaultName()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled"));

		// Act
		await InvokeAsync(viewModel, "AddItemAsync");

		// Assert
		Assert.Equal("Item Name 1", viewModel.Items[1].Name);
		Assert.False(viewModel.Items[1].HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ItemName_TrimsWhitespaceAndBlocksOkUntilCorrected()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled"));

		// Act
		viewModel.Items[0].Name = "   ";

		// Assert
		Assert.Equal(string.Empty, viewModel.Items[0].Name);
		Assert.True(viewModel.Items[0].HasErrors);
		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
		Assert.True(viewModel.CancelCommand.CanExecute(null));

		// Act
		viewModel.Items[0].Name = "  Disabled  ";

		// Assert
		Assert.Equal("Disabled", viewModel.Items[0].Name);
		Assert.False(viewModel.Items[0].HasErrors);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ExactDuplicateItemNames_AreValid()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled", "Enabled"));

		// Assert
		Assert.False(viewModel.HasWarnings);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void CaseOnlyItemNameDifferences_AddWarningWithoutBlockingOk()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled", "enabled"));

		// Assert
		Assert.True(viewModel.HasWarnings);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public async Task SaveAsync_RejectsInvalidActiveEditAndLeavesSourceUnchanged()
	{
		// Arrange
		var source = CreateSource("Operating Mode", "Enabled");
		var viewModel = CreateViewModel(source);
		viewModel.Items[0].Name = "   ";

		// Act
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.False(result);
		Assert.Equal("Enabled", source.Items[0].Name);
	}

	private static StateData CreateSource(string name, params string[] itemNames)
	{
		return new StateData
		{
			Name = name,
			Items = itemNames
				.Select(itemName => new StateItemData
				{
					Name = itemName,
					Color = Color.Green
				})
				.ToList()
		};
	}

	private static StateMapperViewModel CreateViewModel(StateData source)
	{
		var rootNode = new Mock<IElementNode>();
		rootNode.SetupGet(node => node.Id).Returns(Guid.NewGuid());
		rootNode.SetupGet(node => node.Name).Returns("Root");
		rootNode.SetupGet(node => node.Children).Returns([]);
		rootNode.Setup(node => node.GetNodeEnumerator()).Returns([]);

		return new StateMapperViewModel(rootNode.Object, source, Mock.Of<IStateColorPickerService>());
	}

	private static Task InvokeAsync(StateMapperViewModel viewModel, string methodName)
	{
		var method = typeof(StateMapperViewModel).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		return Assert.IsAssignableFrom<Task>(method.Invoke(viewModel, null));
	}

	private static Task<bool> InvokeSaveAsync(StateMapperViewModel viewModel)
	{
		var method = typeof(StateMapperViewModel).GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		return Assert.IsType<Task<bool>>(method.Invoke(viewModel, null));
	}
}
