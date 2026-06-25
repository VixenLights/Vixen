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
	public void SelectedItem_ExpandsOnlyBranchesWithCheckedDescendants()
	{
		// Arrange
		var checkedLeafId = Guid.NewGuid();
		var rootNode = CreateRootNodeWithSiblingGroups(checkedLeafId);
		var source = CreateSource("Operating Mode", "Enabled", "Disabled");
		source.Items[1].ElementNodeIds = [checkedLeafId];
		var viewModel = CreateViewModel(source, rootNode);
		viewModel.Items[1].AssignmentRoots[0].Children[1].IsExpanded = true;

		// Act
		viewModel.SelectedItem = viewModel.Items[1];

		// Assert
		var root = viewModel.SelectedItem.AssignmentRoots[0];
		Assert.True(root.IsExpanded);
		Assert.True(root.Children[0].IsExpanded);
		Assert.False(root.Children[1].IsExpanded);
		Assert.True(root.Children[0].Children[0].IsChecked);
	}

	[Fact]
	public void SwitchingStateDefinition_ClearsSelectedItemWithoutExpandingAssignmentTree()
	{
		// Arrange
		var checkedLeafId = Guid.NewGuid();
		var rootNode = CreateRootNodeWithSiblingGroups(checkedLeafId);
		var source = new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Name = "First",
					Items =
					[
						new StateItemData
						{
							Name = "Enabled",
							Color = Color.Green
						}
					]
				},
				new StateDefinitionData
				{
					Name = "Second",
					Items =
					[
						new StateItemData
						{
							Name = "Disabled",
							Color = Color.Red,
							ElementNodeIds = [checkedLeafId]
						}
					]
				}
			]
		};
		var viewModel = CreateViewModel(source, rootNode);

		// Act
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];

		// Assert
		Assert.Null(viewModel.SelectedItem);
		var item = viewModel.Items[0];
		var root = item.AssignmentRoots[0];
		Assert.False(root.IsExpanded);
		Assert.False(root.Children[0].IsExpanded);
		Assert.True(root.Children[0].Children[0].IsChecked);

		// Act
		viewModel.SelectedItem = item;

		// Assert
		Assert.True(root.IsExpanded);
		Assert.True(root.Children[0].IsExpanded);
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
		Assert.Equal("State Item - 1", viewModel.Items[1].Name);
		Assert.False(viewModel.Items[1].HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void ToggleSelectedAssignmentsCommand_TogglesSelectedAssignmentNodes()
	{
		// Arrange
		var rootNode = CreateRootNodeWithSiblingGroups(Guid.NewGuid());
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled"), rootNode);
		var item = viewModel.Items[0];
		var leaf = item.AssignmentRoots[0].Children[0].Children[0];

		// Act
		item.AssignmentSelection.SelectSingle(leaf);
		item.ToggleSelectedAssignmentsCommand.Execute(null);

		// Assert
		Assert.True(item.ToggleSelectedAssignmentsCommand.CanExecute(null));
		Assert.True(leaf.IsChecked);
		Assert.Equal(1, item.AssignmentCount);
	}

	[Fact]
	public async Task ToggleSelectedAssignmentsCommand_SavesSourceElementNodeIds()
	{
		// Arrange
		var leafId = Guid.NewGuid();
		var rootNode = CreateRootNodeWithSiblingGroups(leafId);
		var source = CreateSource("Operating Mode", "Enabled");
		var viewModel = CreateViewModel(source, rootNode);
		var item = viewModel.Items[0];
		var leaf = item.AssignmentRoots[0].Children[0].Children[0];

		// Act
		item.AssignmentSelection.SelectSingle(leaf);
		item.ToggleSelectedAssignmentsCommand.Execute(null);
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Equal([leafId], source.Items[0].ElementNodeIds);
		Assert.Equal(1, item.AssignmentCount);
	}

	[Fact]
	public void SelectedItemChange_ClearsPreviousAssignmentSelection()
	{
		// Arrange
		var rootNode = CreateRootNodeWithSiblingGroups(Guid.NewGuid());
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled", "Disabled"), rootNode);
		var firstItem = viewModel.Items[0];
		var selectedLeaf = firstItem.AssignmentRoots[0].Children[0].Children[0];
		viewModel.SelectedItem = firstItem;
		firstItem.AssignmentSelection.SelectSingle(selectedLeaf);

		// Act
		viewModel.SelectedItem = viewModel.Items[1];

		// Assert
		Assert.False(selectedLeaf.IsSelected);
		Assert.Equal(0, firstItem.SelectedAssignmentCount);
	}

	[Fact]
	public void SelectedStateDefinitionChange_ClearsAssignmentSelection()
	{
		// Arrange
		var rootNode = CreateRootNodeWithSiblingGroups(Guid.NewGuid());
		var source = new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Name = "First",
					Items =
					[
						new StateItemData
						{
							Name = "Enabled",
							Color = Color.Green
						}
					]
				},
				new StateDefinitionData
				{
					Name = "Second",
					Items =
					[
						new StateItemData
						{
							Name = "Disabled",
							Color = Color.Red
						}
					]
				}
			]
		};
		var viewModel = CreateViewModel(source, rootNode);
		var selectedLeaf = viewModel.Items[0].AssignmentRoots[0].Children[0].Children[0];
		viewModel.SelectedItem = viewModel.Items[0];
		viewModel.Items[0].AssignmentSelection.SelectSingle(selectedLeaf);

		// Act
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];

		// Assert
		Assert.False(selectedLeaf.IsSelected);
		Assert.Empty(source.StateDefinitions[0].Items[0].ElementNodeIds);
	}

	[Fact]
	public void MultiRowSelection_ClearsAssignmentSelectionAndHidesTree()
	{
		// Arrange
		var rootNode = CreateRootNodeWithSiblingGroups(Guid.NewGuid());
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled", "Disabled"), rootNode);
		var firstItem = viewModel.Items[0];
		var selectedLeaf = firstItem.AssignmentRoots[0].Children[0].Children[0];
		viewModel.SelectedItem = firstItem;
		firstItem.AssignmentSelection.SelectSingle(selectedLeaf);

		// Act
		viewModel.SelectedItems.Add(viewModel.Items[1]);

		// Assert
		Assert.Null(viewModel.SelectedItem);
		Assert.Empty(viewModel.AssignedElementRoots);
		Assert.False(selectedLeaf.IsSelected);
		Assert.Equal(0, firstItem.SelectedAssignmentCount);
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
	public void CaseOnlyItemNameDifferences_DoNotAddWarning()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Operating Mode", "Enabled", "enabled"));

		// Assert
		Assert.False(viewModel.HasWarnings);
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

	private static StateMapperViewModel CreateViewModel(StateData source, IElementNode rootNode)
	{
		return new StateMapperViewModel(rootNode, source, Mock.Of<IStateColorPickerService>());
	}

	private static IElementNode CreateRootNodeWithSiblingGroups(Guid checkedLeafId)
	{
		var checkedLeaf = CreateNode(checkedLeafId, "Checked Leaf");
		var uncheckedLeaf = CreateNode(Guid.NewGuid(), "Unchecked Leaf");
		var checkedGroup = CreateNode(Guid.NewGuid(), "Checked Group", checkedLeaf);
		var uncheckedGroup = CreateNode(Guid.NewGuid(), "Unchecked Group", uncheckedLeaf);
		return CreateNode(Guid.NewGuid(), "Root", checkedGroup, uncheckedGroup);
	}

	private static IElementNode CreateNode(Guid id, string name, params IElementNode[] children)
	{
		var node = new Mock<IElementNode>();
		node.SetupGet(elementNode => elementNode.Id).Returns(id);
		node.SetupGet(elementNode => elementNode.Name).Returns(name);
		node.SetupGet(elementNode => elementNode.Children).Returns(children);
		node.Setup(elementNode => elementNode.GetNodeEnumerator()).Returns([]);
		return node.Object;
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
