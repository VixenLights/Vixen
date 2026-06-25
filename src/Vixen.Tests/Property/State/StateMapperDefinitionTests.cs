using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Preview;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.Property.State;

[Collection(StateMapperTestCollection.Name)]
public class StateMapperDefinitionTests
{
	[Fact]
	public void Constructor_SelectsFirstStateDefinition()
	{
		// Arrange
		var source = CreateSource("Open", "Closed");

		// Act
		var viewModel = CreateViewModel(source);

		// Assert
		Assert.Equal("Open", viewModel.Name);
		Assert.Equal("Description for Open", viewModel.Description);
		Assert.Equal("Open Item", viewModel.Items[0].Name);
		Assert.Same(viewModel.StateDefinitions[0], viewModel.SelectedStateDefinition);
	}

	[Fact]
	public async Task AddStateDefinition_AppendsDefaultDefinitionAndSelectsIt()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService("Blink");
		var viewModel = CreateViewModel(CreateSource("Open"), dialogService);

		// Act
		await InvokeAsync(viewModel, "AddStateDefinitionAsync");

		// Assert
		Assert.Equal(["Open", "Blink"], viewModel.StateDefinitions.Select(definition => definition.Name));
		Assert.Equal("Blink", viewModel.Name);
		Assert.Same(viewModel.StateDefinitions[1], viewModel.SelectedStateDefinition);
		Assert.Single(viewModel.Items);
		Assert.Equal("State Item - 1", viewModel.Items[0].Name);
	}

	[Fact]
	public async Task AddStateDefinition_RejectsExactDuplicateName()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService("Open");
		var viewModel = CreateViewModel(CreateSource("Open"), dialogService);

		// Act
		await InvokeAsync(viewModel, "AddStateDefinitionAsync");

		// Assert
		Assert.Single(viewModel.StateDefinitions);
		Assert.Equal("Open", viewModel.Name);
	}

	[Fact]
	public async Task DeleteStateDefinition_BlocksLastDefinition()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService(confirmDelete: true);
		var viewModel = CreateViewModel(CreateSource("Open"), dialogService);

		// Act
		await InvokeAsync(viewModel, "DeleteStateDefinitionAsync");

		// Assert
		Assert.Single(viewModel.StateDefinitions);
		Assert.False(viewModel.DeleteStateDefinitionCommand.CanExecute(null));
	}

	[Fact]
	public async Task DeleteStateDefinition_SelectsNextOrPreviousDefinition()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService(confirmDelete: true);
		var viewModel = CreateViewModel(CreateSource("Open", "Closed", "Blink"), dialogService);

		// Act
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];
		await InvokeAsync(viewModel, "DeleteStateDefinitionAsync");

		// Assert
		Assert.Equal(["Open", "Blink"], viewModel.StateDefinitions.Select(definition => definition.Name));
		Assert.Equal("Blink", viewModel.Name);

		// Act
		await InvokeAsync(viewModel, "DeleteStateDefinitionAsync");

		// Assert
		Assert.Single(viewModel.StateDefinitions);
		Assert.Equal("Open", viewModel.Name);
	}

	[Fact]
	public async Task RenameStateDefinition_PreservesStableId()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService("Running");
		var source = CreateSource("Open");
		var viewModel = CreateViewModel(source, dialogService);
		var definitionId = viewModel.SelectedStateDefinition!.Definition.Id;

		// Act
		await InvokeAsync(viewModel, "RenameStateDefinitionAsync");

		// Assert
		Assert.Equal("Running", viewModel.Name);
		Assert.Equal(definitionId, viewModel.SelectedStateDefinition.Definition.Id);
	}

	[Fact]
	public async Task CopyStateDefinition_AppendsCopyWithNewStableIds()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService("Open Copy");
		var viewModel = CreateViewModel(CreateSource("Open"), dialogService);
		var sourceDefinitionId = viewModel.SelectedStateDefinition!.Definition.Id;
		var sourceItemId = viewModel.Items[0].Item.Id;

		// Act
		await InvokeAsync(viewModel, "CopyStateDefinitionAsync");

		// Assert
		Assert.Equal(["Open", "Open Copy"], viewModel.StateDefinitions.Select(definition => definition.Name));
		Assert.Equal("Open Copy", viewModel.Name);
		Assert.NotEqual(sourceDefinitionId, viewModel.SelectedStateDefinition!.Definition.Id);
		Assert.NotEqual(sourceItemId, viewModel.Items[0].Item.Id);
		Assert.Equal("Open Item", viewModel.Items[0].Name);
		Assert.Equal(Color.Green, viewModel.Items[0].Color);
	}

	[Fact]
	public void ExactDuplicateDefinitionNames_BlockOk()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Open", "Closed"));

		// Act
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];
		viewModel.Name = "Open";

		// Assert
		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void CaseOnlyDefinitionNameDifferences_AddWarningWithoutBlockingOk()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Open", "open"));

		// Assert
		Assert.True(viewModel.HasWarnings);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void SelectionChange_IsBlockedWhenCurrentDefinitionIsInvalid()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateSource("Open", "Closed"));

		// Act
		viewModel.Name = "   ";
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];

		// Assert
		Assert.Same(viewModel.StateDefinitions[0], viewModel.SelectedStateDefinition);
		Assert.Equal(string.Empty, viewModel.Name);
	}

	[Fact]
	public async Task SaveAsync_PersistsAllDefinitions()
	{
		// Arrange
		var source = CreateSource("Open", "Closed");
		var viewModel = CreateViewModel(source);
		viewModel.Name = "Opened";
		viewModel.SelectedStateDefinition = viewModel.StateDefinitions[1];
		viewModel.Description = "Changed second definition";
		viewModel.Items[0].Name = "Closed Item Changed";

		// Act
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Equal(["Opened", "Closed"], source.StateDefinitions.Select(definition => definition.Name));
		Assert.Equal("Changed second definition", source.StateDefinitions[1].Description);
		Assert.Equal("Closed Item Changed", source.StateDefinitions[1].Items[0].Name);
	}

	[Fact]
	public async Task SynchronizeStateItemOrder_PersistsVisibleOrderAndPreservesItemIdentity()
	{
		// Arrange
		var assignedNodeId = Guid.NewGuid();
		var openId = Guid.NewGuid();
		var closedId = Guid.NewGuid();
		var blinkId = Guid.NewGuid();
		var source = CreateItemOrderSource(assignedNodeId, openId, closedId, blinkId);
		var rootNode = CreateNode(Guid.NewGuid(), "Root", CreateNode(assignedNodeId, "Assigned Leaf"));
		var viewModel = CreateViewModel(source, rootNode);
		var visibleOrder = new[]
		{
			viewModel.Items[2],
			viewModel.Items[1],
			viewModel.Items[0]
		};

		// Act
		viewModel.SynchronizeStateItemOrder(visibleOrder);
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Equal(["Blink", "Closed", "Open"], source.StateDefinitions[0].Items.Select(item => item.Name));
		Assert.Equal([blinkId, closedId, openId], source.StateDefinitions[0].Items.Select(item => item.Id));
		Assert.Equal(Color.Blue, source.StateDefinitions[0].Items[0].Color);
		Assert.Equal(Color.Red, source.StateDefinitions[0].Items[1].Color);
		Assert.Equal(Color.Green, source.StateDefinitions[0].Items[2].Color);
		Assert.Equal([assignedNodeId], source.StateDefinitions[0].Items[2].ElementNodeIds);
	}

	[Fact]
	public void MoveItemCommands_AreAvailableOnlyWhenSelectionCanMove()
	{
		// Arrange
		var viewModel = CreateViewModel(CreateItemOrderSource(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
		viewModel.SelectedItem = viewModel.Items[0];

		// Assert
		Assert.False(viewModel.MoveItemUpCommand.CanExecute(null));
		Assert.True(viewModel.MoveItemDownCommand.CanExecute(null));

		// Act
		viewModel.SelectedItem = viewModel.Items[1];

		// Assert
		Assert.True(viewModel.MoveItemUpCommand.CanExecute(null));
		Assert.True(viewModel.MoveItemDownCommand.CanExecute(null));

		// Act
		viewModel.SelectedItem = viewModel.Items[2];

		// Assert
		Assert.True(viewModel.MoveItemUpCommand.CanExecute(null));
		Assert.False(viewModel.MoveItemDownCommand.CanExecute(null));

		// Act
		viewModel.SelectedItem = null;

		// Assert
		Assert.False(viewModel.MoveItemUpCommand.CanExecute(null));
		Assert.False(viewModel.MoveItemDownCommand.CanExecute(null));
	}

	[Fact]
	public async Task MoveItemUp_PersistsOrderAndPreservesItemIdentity()
	{
		// Arrange
		var assignedNodeId = Guid.NewGuid();
		var openId = Guid.NewGuid();
		var closedId = Guid.NewGuid();
		var blinkId = Guid.NewGuid();
		var source = CreateItemOrderSource(assignedNodeId, openId, closedId, blinkId);
		var rootNode = CreateNode(Guid.NewGuid(), "Root", CreateNode(assignedNodeId, "Assigned Leaf"));
		var viewModel = CreateViewModel(source, rootNode);
		var selectedItem = viewModel.Items[2];
		viewModel.SelectedItem = selectedItem;

		// Act
		viewModel.MoveItemUpCommand.Execute(null);
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Same(selectedItem, viewModel.SelectedItem);
		Assert.Equal(["Open", "Blink", "Closed"], source.StateDefinitions[0].Items.Select(item => item.Name));
		Assert.Equal([openId, blinkId, closedId], source.StateDefinitions[0].Items.Select(item => item.Id));
		Assert.Equal(Color.Green, source.StateDefinitions[0].Items[0].Color);
		Assert.Equal(Color.Blue, source.StateDefinitions[0].Items[1].Color);
		Assert.Equal(Color.Red, source.StateDefinitions[0].Items[2].Color);
		Assert.Equal([assignedNodeId], source.StateDefinitions[0].Items[0].ElementNodeIds);
	}

	[Fact]
	public async Task MoveItemDown_PersistsOrderAndKeepsMovedItemSelected()
	{
		// Arrange
		var openId = Guid.NewGuid();
		var closedId = Guid.NewGuid();
		var blinkId = Guid.NewGuid();
		var source = CreateItemOrderSource(Guid.NewGuid(), openId, closedId, blinkId);
		var viewModel = CreateViewModel(source);
		var selectedItem = viewModel.Items[0];
		viewModel.SelectedItem = selectedItem;

		// Act
		viewModel.MoveItemDownCommand.Execute(null);
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Same(selectedItem, viewModel.SelectedItem);
		Assert.Equal(["Closed", "Open", "Blink"], source.StateDefinitions[0].Items.Select(item => item.Name));
		Assert.Equal([closedId, openId, blinkId], source.StateDefinitions[0].Items.Select(item => item.Id));
	}

	[Fact]
	public async Task MoveItemAfterSort_PersistsOrderAndKeepsMovedItemSelected()
	{
		// Arrange
		var openId = Guid.NewGuid();
		var closedId = Guid.NewGuid();
		var blinkId = Guid.NewGuid();
		var source = CreateItemOrderSource(Guid.NewGuid(), openId, closedId, blinkId);
		var viewModel = CreateViewModel(source);
		viewModel.SynchronizeStateItemOrder(
		[
			viewModel.Items[2],
			viewModel.Items[1],
			viewModel.Items[0]
		]);
		var selectedItem = viewModel.Items[1];
		viewModel.SelectedItem = selectedItem;

		// Act
		viewModel.MoveItemUpCommand.Execute(null);
		var result = await InvokeSaveAsync(viewModel);

		// Assert
		Assert.True(result);
		Assert.Same(selectedItem, viewModel.SelectedItem);
		Assert.Equal(["Closed", "Blink", "Open"], source.StateDefinitions[0].Items.Select(item => item.Name));
		Assert.Equal([closedId, blinkId, openId], source.StateDefinitions[0].Items.Select(item => item.Id));
	}

	[Fact]
	public async Task RemoveItem_RemovesSingleSelectedRowWithSingleConfirmation()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService(confirmDelete: true);
		var viewModel = CreateViewModel(
			CreateItemOrderSource(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
			dialogService);
		viewModel.SelectedItem = viewModel.Items[1];

		// Act
		await InvokeAsync(viewModel, "RemoveItemAsync");

		// Assert
		Assert.Equal(["Open", "Blink"], viewModel.Items.Select(item => item.Name));
		Assert.Equal("Blink", Assert.Single(viewModel.SelectedItems).Name);
		Assert.Equal(["Closed"], dialogService.StateItemDeleteConfirmations);
		Assert.Empty(dialogService.StateItemsDeleteConfirmations);
	}

	[Fact]
	public async Task RemoveItem_RemovesMultipleSelectedRowsWithMultiConfirmation()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService(confirmDelete: true);
		var viewModel = CreateViewModel(
			CreateItemOrderSource(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
			dialogService);
		viewModel.SelectedItems.Add(viewModel.Items[0]);
		viewModel.SelectedItems.Add(viewModel.Items[2]);

		// Act
		await InvokeAsync(viewModel, "RemoveItemAsync");

		// Assert
		Assert.Equal(["Closed"], viewModel.Items.Select(item => item.Name));
		Assert.Empty(viewModel.SelectedItems);
		Assert.Empty(viewModel.AssignedElementRoots);
		Assert.Empty(dialogService.StateItemDeleteConfirmations);
		Assert.Equal([2], dialogService.StateItemsDeleteConfirmations);
	}

	[Fact]
	public async Task RemoveItem_CancelKeepsSelectedRows()
	{
		// Arrange
		var dialogService = new FakeStateDefinitionDialogService(confirmDelete: false);
		var viewModel = CreateViewModel(
			CreateItemOrderSource(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
			dialogService);
		viewModel.SelectedItems.Add(viewModel.Items[0]);
		viewModel.SelectedItems.Add(viewModel.Items[2]);

		// Act
		await InvokeAsync(viewModel, "RemoveItemAsync");

		// Assert
		Assert.Equal(["Open", "Closed", "Blink"], viewModel.Items.Select(item => item.Name));
		Assert.Equal(["Open", "Blink"], viewModel.SelectedItems.Select(item => item.Name));
		Assert.Equal([2], dialogService.StateItemsDeleteConfirmations);
	}

	private static StateData CreateSource(params string[] definitionNames)
	{
		return new StateData
		{
			StateDefinitions = definitionNames
				.Select(name => new StateDefinitionData
				{
					Name = name,
					Description = $"Description for {name}",
					Items =
					[
						new StateItemData
						{
							Name = $"{name} Item",
							Color = Color.Green
						}
					]
				})
				.ToList()
		};
	}

	private static StateData CreateItemOrderSource(
		Guid assignedNodeId,
		Guid openId,
		Guid closedId,
		Guid blinkId)
	{
		return new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Name = "Operating Mode",
					Items =
					[
						new StateItemData
						{
							Id = openId,
							Name = "Open",
							Color = Color.Green,
							ElementNodeIds = [assignedNodeId]
						},
						new StateItemData
						{
							Id = closedId,
							Name = "Closed",
							Color = Color.Red
						},
						new StateItemData
						{
							Id = blinkId,
							Name = "Blink",
							Color = Color.Blue
						}
					]
				}
			]
		};
	}

	private static StateMapperViewModel CreateViewModel(
		StateData source,
		IStateDefinitionDialogService? dialogService = null)
	{
		var rootNode = new Mock<IElementNode>();
		rootNode.SetupGet(node => node.Id).Returns(Guid.NewGuid());
		rootNode.SetupGet(node => node.Name).Returns("Root");
		rootNode.SetupGet(node => node.Children).Returns([]);
		rootNode.Setup(node => node.GetNodeEnumerator()).Returns([]);

		return new StateMapperViewModel(
			rootNode.Object,
			source,
			Mock.Of<IStateColorPickerService>(),
			new NoOpStatePreviewPublisher(),
			dialogService ?? new FakeStateDefinitionDialogService());
	}

	private static StateMapperViewModel CreateViewModel(
		StateData source,
		IElementNode rootNode)
	{
		return new StateMapperViewModel(
			rootNode,
			source,
			Mock.Of<IStateColorPickerService>(),
			new NoOpStatePreviewPublisher(),
			new FakeStateDefinitionDialogService());
	}

	private static IElementNode CreateNode(Guid id, string name, params IElementNode[] children)
	{
		var node = new Mock<IElementNode>();
		node.SetupGet(elementNode => elementNode.Id).Returns(id);
		node.SetupGet(elementNode => elementNode.Name).Returns(name);
		node.SetupGet(elementNode => elementNode.Children).Returns(children);
		node.Setup(elementNode => elementNode.GetNodeEnumerator()).Returns(children);
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

	private sealed class FakeStateDefinitionDialogService : IStateDefinitionDialogService
	{
		private readonly Queue<string?> _names = new();
		private readonly bool _confirmDelete;

		public FakeStateDefinitionDialogService(params string?[] names)
			: this(true, names)
		{
		}

		public FakeStateDefinitionDialogService(bool confirmDelete, params string?[] names)
		{
			_confirmDelete = confirmDelete;
			foreach (var name in names)
			{
				_names.Enqueue(name);
			}
		}

		public List<string> StateItemDeleteConfirmations { get; } = [];

		public List<int> StateItemsDeleteConfirmations { get; } = [];

		public Task<string?> RequestNameAsync(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName)
		{
			return Task.FromResult(_names.Count == 0 ? (string?)null : _names.Dequeue());
		}

		public Task<bool> ConfirmDeleteAsync(string name)
		{
			return Task.FromResult(_confirmDelete);
		}

		public Task<bool> ConfirmDeleteStateItemAsync(string name)
		{
			StateItemDeleteConfirmations.Add(name);
			return Task.FromResult(_confirmDelete);
		}

		public Task<bool> ConfirmDeleteStateItemsAsync(int count)
		{
			StateItemsDeleteConfirmations.Add(count);
			return Task.FromResult(_confirmDelete);
		}
	}

	private sealed class NoOpStatePreviewPublisher : IStatePreviewPublisher
	{
		public void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs)
		{
		}

		public void TurnOff(IReadOnlyCollection<Guid> elementIds)
		{
		}

		public void Clear()
		{
		}

		public void Release()
		{
		}
	}
}
