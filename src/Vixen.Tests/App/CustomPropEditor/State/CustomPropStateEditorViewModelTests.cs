using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.App.CustomPropEditor.ViewModels;
using VixenModules.App.CustomPropEditor.ViewModels.State;
using VixenModules.Property.State.Setup.Services;
using WpfPoint = System.Windows.Point;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.State;

[Collection("CustomPropEditor")]
public sealed class CustomPropStateEditorViewModelTests
{
	[Fact]
	public void Constructor_MigratesLegacyStateDataToModelElement()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		leaf.StateDefinition = new StateDefinition
		{
			StateDefinitionName = "Wave",
			Name = "Hand",
			DefaultColor = Color.Red
		};

		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		Assert.True(viewModel.IsDirty);
		Assert.Same(model, viewModel.ModelElement);
		var definition = Assert.Single(viewModel.StateDefinitions);
		Assert.Equal("Wave", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Hand", item.Name);
		Assert.Equal([leaf.Id], item.StateItem.ElementModelIds);
	}

	[Fact]
	public void Commands_AddCopyDeleteDefinitions_UpdateModelCollection()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		Execute(viewModel.AddStateDefinitionCommand);
		var originalDefinition = Assert.Single(viewModel.StateDefinitions);
		originalDefinition.Name = "Wave";

		Execute(viewModel.CopyStateDefinitionCommand);

		Assert.Equal(2, model.StateDefinitionModels.Count);
		Assert.Equal("Wave Copy", viewModel.SelectedStateDefinition.Name);
		Assert.NotEqual(originalDefinition.StateDefinition.Id, viewModel.SelectedStateDefinition.StateDefinition.Id);

		Execute(viewModel.DeleteStateDefinitionCommand);

		var remainingDefinition = Assert.Single(viewModel.StateDefinitions);
		Assert.Same(originalDefinition, remainingDefinition);
		Assert.Single(model.StateDefinitionModels);
	}

	[Fact]
	public async Task RenameStateDefinition_PromptsForNewName()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var dialogService = new FakeStateDefinitionDialogService("Pulse");
		var viewModel = new StateDefinitionEditorViewModel(prop, dialogService);

		await InvokeAsync(viewModel, "RenameStateDefinitionAsync");

		Assert.Equal("Rename State Definition", dialogService.LastTitle);
		Assert.Equal("Wave", dialogService.LastInitialName);
		Assert.Equal("Wave", dialogService.LastCurrentName);
		Assert.Equal("Pulse", viewModel.SelectedStateDefinition.Name);
		Assert.Equal("Pulse", definition.Name);
		Assert.True(viewModel.IsDirty);
	}

	[Fact]
	public void Commands_AddRemoveAndMoveItems_UpdateModelOrder()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		viewModel.ConfirmStateItemDelete = (_, _) => true;

		Execute(viewModel.AddStateItemCommand);
		viewModel.SelectedStateItem.Name = "Arm";
		Execute(viewModel.MoveStateItemUpCommand);

		Assert.Equal("Arm", definition.Items[0].Name);

		Execute(viewModel.RemoveStateItemCommand);

		Assert.Single(definition.Items);
		Assert.Equal(StateItemModel.DefaultName, definition.Items[0].Name);
	}

	[Fact]
	public void RemoveStateItemCommand_DeletesAllSelectedItems()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = new StateDefinitionModel
		{
			Name = "Wave",
			Items =
			[
				new StateItemModel { Name = "Arm" },
				new StateItemModel { Name = "Leg" },
				new StateItemModel { Name = "Hat" }
			]
		};
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var confirmedQuestion = string.Empty;
		viewModel.ConfirmStateItemDelete = (question, _) =>
		{
			confirmedQuestion = question;
			return true;
		};
		viewModel.SelectedStateItems.Clear();
		viewModel.SelectedStateItems.Add(viewModel.SelectedStateDefinition.Items[0]);
		viewModel.SelectedStateItems.Add(viewModel.SelectedStateDefinition.Items[2]);

		Execute(viewModel.RemoveStateItemCommand);

		Assert.Equal("Delete 2 State items?", confirmedQuestion);
		Assert.Equal(["Leg"], definition.Items.Select(item => item.Name).ToList());
		Assert.Equal(["Leg"], viewModel.SelectedStateDefinition.Items.Select(item => item.Name).ToList());
		Assert.Equal("Leg", viewModel.SelectedStateItem.Name);
		Assert.True(viewModel.IsDirty);
	}

	[Fact]
	public void RemoveStateItemCommand_CancelKeepsSelectedItems()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		definition.Items.Add(new StateItemModel { Name = "Arm" });
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		viewModel.SelectedStateItems.Add(viewModel.SelectedStateDefinition.Items[1]);
		viewModel.ConfirmStateItemDelete = (_, _) => false;

		Execute(viewModel.RemoveStateItemCommand);

		Assert.Equal([StateItemModel.DefaultName, "Arm"], definition.Items.Select(item => item.Name).ToList());
		Assert.Equal(2, viewModel.SelectedStateItems.Count);
	}

	[Fact]
	public void PersistStateItemSortCommand_UpdatesModelItemOrder()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = new StateDefinitionModel
		{
			Name = "Wave",
			Items =
			[
				new StateItemModel { Name = "Charlie" },
				new StateItemModel { Name = "Alpha" },
				new StateItemModel { Name = "Bravo" }
			]
		};
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var sortedItems = viewModel.SelectedStateDefinition.Items
			.OrderBy(item => item.Name)
			.ToList();

		Execute(viewModel.PersistStateItemSortCommand, sortedItems);

		Assert.Equal(["Alpha", "Bravo", "Charlie"], definition.Items.Select(item => item.Name).ToList());
		Assert.Equal(["Alpha", "Bravo", "Charlie"], viewModel.SelectedStateDefinition.Items.Select(item => item.Name).ToList());
		Assert.True(viewModel.IsDirty);
	}

	[Fact]
	public void Validation_DetectsDuplicateDefinitionNamesAndMissingAssignments()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		model.StateDefinitionModels =
		[
			new StateDefinitionModel
			{
				Name = "Wave",
				Items = new ObservableCollection<StateItemModel>
				{
					new() { Name = "Hand" }
				}
			},
			new StateDefinitionModel
			{
				Name = "wave",
				Items = new ObservableCollection<StateItemModel>
				{
					new() { Name = "Hand" },
					new() { Name = "hand" }
				}
			}
		];
		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		Assert.True(viewModel.HasValidationErrors);
		Assert.Contains("State definition name \"Wave\" is duplicated.", viewModel.ValidationMessages);
		Assert.Contains(viewModel.ValidationMessages, message => message.Contains("must assign at least one element"));
	}

	[Fact]
	public void Validation_AllowsDuplicateStateItemNames()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		model.StateDefinitionModels =
		[
			new StateDefinitionModel
			{
				Name = "Wave",
				Items = new ObservableCollection<StateItemModel>
				{
					new()
					{
						Name = "Hand",
						ElementModelIds = new ObservableCollection<Guid> { leaf.Id }
					},
					new()
					{
						Name = "Hand",
						ElementModelIds = new ObservableCollection<Guid> { leaf.Id }
					}
				}
			}
		];

		var viewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		Assert.False(viewModel.HasValidationErrors);
	}

	[Fact]
	public void ElementModelViewModel_ModelTypeMarksEditorDirty()
	{
		var elementModel = new ElementModel("Model");
		var viewModel = new ElementModelViewModel(elementModel, null);

		viewModel.ModelType = ElementModelType.Model;

		Assert.Equal(ElementModelType.Model, elementModel.ModelType);
		Assert.True(viewModel.IsDirty);
	}

	[Fact]
	public void AssignElementModelIds_AddsDistinctAssignmentsAndMarksDirty()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var item = editorViewModel.SelectedStateItem;

		var changed = item.AssignElementModelIds([leaf.Id, leaf.Id, Guid.Empty]);

		Assert.True(changed);
		Assert.Equal([leaf.Id], item.StateItem.ElementModelIds);
		Assert.True(item.HasAssignments);
		Assert.Equal(1, item.AssignmentCount);
		Assert.True(item.IsDirty);
		Assert.True(editorViewModel.IsDirty);
	}

	[Fact]
	public void RemoveElementModelIds_RemovesAssignmentsAndMarksDirty()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		var secondLeaf = new ElementModel("Leaf 2", model);
		model.AddChild(secondLeaf);
		var definition = StateDefinitionModel.CreateDefault("Wave");
		definition.Items[0].ElementModelIds = new ObservableCollection<Guid> { leaf.Id, secondLeaf.Id };
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var item = editorViewModel.SelectedStateItem;

		var changed = item.RemoveElementModelIds([leaf.Id, leaf.Id, Guid.Empty]);

		Assert.True(changed);
		Assert.Equal([secondLeaf.Id], item.StateItem.ElementModelIds);
		Assert.True(item.HasAssignments);
		Assert.Equal(1, item.AssignmentCount);
		Assert.True(item.IsDirty);
		Assert.True(editorViewModel.IsDirty);
	}

	[Fact]
	public void AssignmentCount_IgnoresAssignmentsForMissingElements()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		definition.Items[0].ElementModelIds = new ObservableCollection<Guid> { leaf.Id, Guid.NewGuid() };
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		Assert.Equal(1, editorViewModel.SelectedStateItem.AssignmentCount);
		Assert.True(editorViewModel.SelectedStateItem.HasAssignments);
	}

	[Fact]
	public void RefreshAssignments_RemovesAssignmentsForDeletedElements()
	{
		var prop = CreatePropWithModel(out var model, out var firstLeaf);
		model.ModelType = ElementModelType.Model;
		var secondLeaf = new ElementModel("Leaf 2", model);
		model.AddChild(secondLeaf);
		var definition = StateDefinitionModel.CreateDefault("Wave");
		definition.Items[0].ElementModelIds = new ObservableCollection<Guid> { firstLeaf.Id, secondLeaf.Id };
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());

		model.RemoveChild(firstLeaf);
		editorViewModel.RefreshAssignments();

		Assert.Equal([secondLeaf.Id], definition.Items[0].ElementModelIds);
		Assert.Equal(1, editorViewModel.SelectedStateItem.AssignmentCount);
		Assert.True(editorViewModel.IsDirty);
	}

	[Fact]
	public void SelectedStateItems_MultipleRowsDisablesCanvasAssignments()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		definition.Items.Add(new StateItemModel { Name = "Arm" });
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var firstItem = editorViewModel.SelectedStateDefinition.Items[0];
		var secondItem = editorViewModel.SelectedStateDefinition.Items[1];

		editorViewModel.SelectedStateItems.Clear();
		editorViewModel.SelectedStateItems.Add(firstItem);

		Assert.True(editorViewModel.CanEditCanvasAssignments);

		editorViewModel.SelectedStateItems.Add(secondItem);

		Assert.False(editorViewModel.CanEditCanvasAssignments);
	}

	[Fact]
	public void ToggleElementModelId_AddsAndRemovesAssignment()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop, new StateDefinitionDialogService());
		var item = editorViewModel.SelectedStateItem;

		Assert.True(item.ToggleElementModelId(leaf.Id));
		Assert.Equal([leaf.Id], item.StateItem.ElementModelIds);
		Assert.Equal(1, item.AssignmentCount);

		Assert.True(item.ToggleElementModelId(leaf.Id));
		Assert.Empty(item.StateItem.ElementModelIds);
		Assert.False(item.HasAssignments);
		Assert.Equal(0, item.AssignmentCount);
		Assert.True(editorViewModel.IsDirty);
	}

	[Fact]
	public void DrawingPanel_ApplyStatePreviewColorsAssignedLightsAndGreysUnassignedLights()
	{
		var prop = CreateServicePropWithLights(out var model, out var firstLeaf, out var secondLeaf);
		model.ModelType = ElementModelType.Model;
		var itemModel = new StateItemModel
		{
			Name = "Arm",
			Color = Color.Red,
			ElementModelIds = new ObservableCollection<Guid> { firstLeaf.Id }
		};
		var itemViewModel = new CustomPropStateItemViewModel(itemModel, prop, () => { });
		var drawingPanelViewModel = new DrawingPanelViewModel(new ElementTreeViewModel(prop));

		drawingPanelViewModel.ApplyStatePreview([itemViewModel]);

		Assert.True(drawingPanelViewModel.IsStatePreviewActive);
		Assert.Equal(Color.Red.ToArgb(), drawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == firstLeaf.Id).DisplayColor.ToArgb());
		Assert.Equal(Color.FromArgb(25, 25, 25).ToArgb(), drawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == secondLeaf.Id).DisplayColor.ToArgb());
	}

	[Fact]
	public void DrawingPanel_ApplyStatePreviewColorsMultipleStateItems()
	{
		var prop = CreateServicePropWithLights(out var model, out var firstLeaf, out var secondLeaf);
		model.ModelType = ElementModelType.Model;
		var firstItemViewModel = new CustomPropStateItemViewModel(
			new StateItemModel
			{
				Name = "Arm",
				Color = Color.Red,
				ElementModelIds = new ObservableCollection<Guid> { firstLeaf.Id }
			},
			prop,
			() => { });
		var secondItemViewModel = new CustomPropStateItemViewModel(
			new StateItemModel
			{
				Name = "Leg",
				Color = Color.Green,
				ElementModelIds = new ObservableCollection<Guid> { secondLeaf.Id }
			},
			prop,
			() => { });
		var drawingPanelViewModel = new DrawingPanelViewModel(new ElementTreeViewModel(prop));

		drawingPanelViewModel.ApplyStatePreview([firstItemViewModel, secondItemViewModel]);

		Assert.Equal(Color.Red.ToArgb(), drawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == firstLeaf.Id).DisplayColor.ToArgb());
		Assert.Equal(Color.Green.ToArgb(), drawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == secondLeaf.Id).DisplayColor.ToArgb());
	}

	[Fact]
	public void DrawingPanel_ClearStatePreviewRestoresNormalLightColors()
	{
		var prop = CreateServicePropWithLights(out var model, out var firstLeaf, out _);
		model.ModelType = ElementModelType.Model;
		var itemModel = new StateItemModel
		{
			Name = "Arm",
			Color = Color.Red,
			ElementModelIds = new ObservableCollection<Guid> { firstLeaf.Id }
		};
		var itemViewModel = new CustomPropStateItemViewModel(itemModel, prop, () => { });
		var drawingPanelViewModel = new DrawingPanelViewModel(new ElementTreeViewModel(prop));

		drawingPanelViewModel.ApplyStatePreview([itemViewModel]);
		drawingPanelViewModel.ClearStatePreview();

		Assert.False(drawingPanelViewModel.IsStatePreviewActive);
		Assert.All(drawingPanelViewModel.LightNodes, light => Assert.Equal(drawingPanelViewModel.LightColor, light.DisplayColor));
	}

	[Fact]
	public void PropEditor_ToggleStateItemAssignment_RequiresStateDefinitionTabAndSingleSelectedItem()
	{
		var viewModel = CreatePropEditorWithState(out var firstLeaf, out var secondLeaf, out var firstLight, out var secondLight);
		var stateEditor = viewModel.StateDefinitionEditorViewModel;
		var firstItem = stateEditor.SelectedStateDefinition.Items[0];
		var secondItem = stateEditor.SelectedStateDefinition.Items[1];

		Assert.False(viewModel.ToggleStateItemAssignment(firstLight));
		Assert.Empty(firstItem.StateItem.ElementModelIds);

		viewModel.SelectedTabIndex = 2;

		Assert.True(viewModel.ToggleStateItemAssignment(firstLight));
		Assert.Equal([firstLeaf.Id], firstItem.StateItem.ElementModelIds);

		stateEditor.SelectedStateItems.Add(secondItem);

		Assert.False(viewModel.ToggleStateItemAssignment(secondLight));
		Assert.DoesNotContain(secondLeaf.Id, firstItem.StateItem.ElementModelIds);
		Assert.Empty(secondItem.StateItem.ElementModelIds);
	}

	[Fact]
	public void PropEditor_AssignAndRemoveStateItemAssignment_RefreshesPreview()
	{
		var viewModel = CreatePropEditorWithState(out var firstLeaf, out var secondLeaf, out var firstLight, out _);
		var item = viewModel.StateDefinitionEditorViewModel.SelectedStateItem;
		viewModel.SelectedTabIndex = 2;

		Assert.True(viewModel.AssignStateItemAssignment(firstLight));

		Assert.Equal([firstLeaf.Id], item.StateItem.ElementModelIds);
		Assert.Equal(Color.Red.ToArgb(), viewModel.DrawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == firstLeaf.Id).DisplayColor.ToArgb());
		Assert.Equal(Color.FromArgb(25, 25, 25).ToArgb(), viewModel.DrawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == secondLeaf.Id).DisplayColor.ToArgb());

		Assert.True(viewModel.RemoveStateItemAssignment(firstLight));

		Assert.Empty(item.StateItem.ElementModelIds);
		Assert.All(viewModel.DrawingPanelViewModel.LightNodes, light => Assert.Equal(Color.FromArgb(25, 25, 25).ToArgb(), light.DisplayColor.ToArgb()));
	}

	[Fact]
	public void PropEditor_DeleteCommand_RemovesDeletedElementFromStateAssignments()
	{
		var viewModel = CreatePropEditorWithState(out var firstLeaf, out var secondLeaf, out _, out _);
		var item = viewModel.StateDefinitionEditorViewModel.SelectedStateItem;
		item.AssignElementModelIds([firstLeaf.Id, secondLeaf.Id]);
		viewModel.SelectedTabIndex = 2;
		var firstLeafViewModel = viewModel.ElementTreeViewModel.RootNodesViewModels
			.Single()
			.ChildrenViewModels
			.Single(child => child.ElementModel.ModelType == ElementModelType.Model)
			.ChildrenViewModels
			.Single(child => child.ElementModel.Id == firstLeaf.Id);
		viewModel.ElementTreeViewModel.SelectedItems.Add(firstLeafViewModel);

		Execute(viewModel.DeleteCommand);

		Assert.Equal([secondLeaf.Id], item.StateItem.ElementModelIds);
		Assert.DoesNotContain(viewModel.DrawingPanelViewModel.LightNodes, light => light.Light.ParentModelId == firstLeaf.Id);
		Assert.Contains(viewModel.DrawingPanelViewModel.LightNodes, light => light.Light.ParentModelId == secondLeaf.Id);
	}

	private static Prop CreatePropWithModel(out ElementModel model, out ElementModel leaf)
	{
		var prop = new Prop("State Prop");
		model = new ElementModel("Model", prop.RootNode);
		leaf = new ElementModel("Leaf 1", model);
		model.AddChild(leaf);
		prop.RootNode.AddChild(model);
		return prop;
	}

	private static PropEditorViewModel CreatePropEditorWithState(
		out ElementModel firstLeaf,
		out ElementModel secondLeaf,
		out LightViewModel firstLight,
		out LightViewModel secondLight)
	{
		var viewModel = new PropEditorViewModel();
		var model = new ElementModel("Model", viewModel.Prop.RootNode)
		{
			ModelType = ElementModelType.Model
		};
		firstLeaf = new ElementModel("Leaf 1", model);
		firstLeaf.Lights.Add(new Light(new WpfPoint(10, 10), ElementModel.DefaultLightSize, firstLeaf.Id));
		secondLeaf = new ElementModel("Leaf 2", model);
		secondLeaf.Lights.Add(new Light(new WpfPoint(20, 20), ElementModel.DefaultLightSize, secondLeaf.Id));
		model.AddChild(firstLeaf);
		model.AddChild(secondLeaf);
		model.StateDefinitionModels.Add(new StateDefinitionModel
		{
			Name = "Wave",
			Items = new ObservableCollection<StateItemModel>
			{
				new()
				{
					Name = "Arm",
					Color = Color.Red
				},
				new()
				{
					Name = "Leg",
					Color = Color.Green
				}
			}
		});
		viewModel.Prop.RootNode.AddChild(model);
		viewModel.StateDefinitionEditorViewModel.SetProp(viewModel.Prop);
		viewModel.DrawingPanelViewModel.RefreshLightViewModels();
		var firstLeafId = firstLeaf.Id;
		var secondLeafId = secondLeaf.Id;
		firstLight = viewModel.DrawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == firstLeafId);
		secondLight = viewModel.DrawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == secondLeafId);
		return viewModel;
	}

	private static Prop CreateServicePropWithLights(
		out ElementModel model,
		out ElementModel firstLeaf,
		out ElementModel secondLeaf)
	{
		var prop = PropModelServices.Instance().CreateProp("State Prop");
		model = new ElementModel("Model", prop.RootNode);
		firstLeaf = new ElementModel("Leaf 1", model);
		firstLeaf.Lights.Add(new Light(new WpfPoint(10, 10), ElementModel.DefaultLightSize, firstLeaf.Id));
		secondLeaf = new ElementModel("Leaf 2", model);
		secondLeaf.Lights.Add(new Light(new WpfPoint(20, 20), ElementModel.DefaultLightSize, secondLeaf.Id));
		model.AddChild(firstLeaf);
		model.AddChild(secondLeaf);
		prop.RootNode.AddChild(model);
		return prop;
	}

	private static void Execute(ICommand command)
	{
		Assert.True(command.CanExecute(null));
		command.Execute(null);
	}

	private static void Execute(ICommand command, object parameter)
	{
		Assert.True(command.CanExecute(parameter));
		command.Execute(parameter);
	}

	private static async Task InvokeAsync(object target, string methodName)
	{
		var method = target.GetType().GetMethod(
			methodName,
			System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		Assert.NotNull(method);
		var task = Assert.IsAssignableFrom<Task>(method.Invoke(target, null));
		await task;
	}

	private sealed class FakeStateDefinitionDialogService : IStateDefinitionDialogService
	{
		private readonly string _name;

		public FakeStateDefinitionDialogService(string name)
		{
			_name = name;
		}

		public string LastTitle { get; private set; } = string.Empty;

		public string LastInitialName { get; private set; } = string.Empty;

		public string? LastCurrentName { get; private set; }

		public Task<string?> RequestNameAsync(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName)
		{
			LastTitle = title;
			LastInitialName = initialName;
			LastCurrentName = currentName;
			return Task.FromResult<string?>(_name);
		}

		public Task<bool> ConfirmDeleteAsync(string name)
		{
			return Task.FromResult(false);
		}

		public Task<bool> ConfirmDeleteStateItemAsync(string name)
		{
			return Task.FromResult(false);
		}

		public Task<bool> ConfirmDeleteStateItemsAsync(int count)
		{
			return Task.FromResult(false);
		}
	}
}
