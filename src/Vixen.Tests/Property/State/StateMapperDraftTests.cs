using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateMapperDraftTests
{
	[Fact]
	public void Cancel_DoesNotMutateOriginalStateData()
	{
		// Arrange
		var elementNodeId = Guid.NewGuid();
		var source = CreateSource(elementNodeId);
		var viewModel = CreateViewModel(source, elementNodeId);

		// Act
		viewModel.Name = "Changed";
		viewModel.Description = "Changed description";
		viewModel.Items[0].Name = "Disabled";
		viewModel.Items[0].AssignmentRoots[0].IsChecked = true;

		// Assert
		Assert.Equal("Operating Mode", source.Name);
		Assert.Equal("Available operating modes", source.Description);
		Assert.Equal("Enabled", source.Items[0].Name);
		Assert.Empty(source.Items[0].ElementNodeIds);
	}

	[Fact]
	public async Task Ok_AppliesDraftToOriginalStateData()
	{
		// Arrange
		var elementNodeId = Guid.NewGuid();
		var source = CreateSource(elementNodeId);
		var viewModel = CreateViewModel(source, elementNodeId);
		viewModel.Name = "Changed";
		viewModel.Description = "Changed description";
		viewModel.Items[0].Name = "Disabled";
		viewModel.Items[0].AssignmentRoots[0].IsChecked = true;

		// Act
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Equal("Changed", source.Name);
		Assert.Equal("Changed description", source.Description);
		Assert.Equal("Disabled", source.Items[0].Name);
		Assert.Equal([elementNodeId], source.Items[0].ElementNodeIds);
	}

	private static StateData CreateSource(Guid itemId)
	{
		return new StateData
		{
			Name = "Operating Mode",
			Description = "Available operating modes",
			Items =
			[
				new StateItemData
				{
					Id = itemId,
					Name = "Enabled",
					Color = Color.Green
				}
			]
		};
	}

	private static StateMapperViewModel CreateViewModel(StateData source, Guid elementNodeId)
	{
		var rootNode = new Mock<IElementNode>();
		rootNode.SetupGet(node => node.Id).Returns(elementNodeId);
		rootNode.SetupGet(node => node.Name).Returns("Root");
		rootNode.SetupGet(node => node.Children).Returns([]);
		rootNode.Setup(node => node.GetNodeEnumerator()).Returns([]);

		return new StateMapperViewModel(rootNode.Object, source, Mock.Of<IStateColorPickerService>());
	}

	private static Task<bool> InvokeSaveAsync(StateMapperViewModel viewModel)
	{
		var method = typeof(StateMapperViewModel).GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		return Assert.IsType<Task<bool>>(method.Invoke(viewModel, null));
	}
}
