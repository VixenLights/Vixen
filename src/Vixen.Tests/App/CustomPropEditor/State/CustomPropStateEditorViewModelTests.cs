using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.App.CustomPropEditor.ViewModels;
using VixenModules.App.CustomPropEditor.ViewModels.State;
using WpfPoint = System.Windows.Point;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.State;

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

		var viewModel = new StateDefinitionEditorViewModel(prop);

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
		var viewModel = new StateDefinitionEditorViewModel(prop);

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
	public void Commands_AddRemoveAndMoveItems_UpdateModelOrder()
	{
		var prop = CreatePropWithModel(out var model, out _);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop);

		Execute(viewModel.AddStateItemCommand);
		viewModel.SelectedStateItem.Name = "Arm";
		Execute(viewModel.MoveStateItemUpCommand);

		Assert.Equal("Arm", definition.Items[0].Name);

		Execute(viewModel.RemoveStateItemCommand);

		Assert.Single(definition.Items);
		Assert.Equal(StateItemModel.DefaultName, definition.Items[0].Name);
	}

	[Fact]
	public void AssignmentTree_GroupAssignmentAssignsLeafElements()
	{
		var prop = CreatePropWithModel(out var model, out var firstLeaf);
		model.ModelType = ElementModelType.Model;
		var secondLeaf = new ElementModel("Leaf 2", model);
		model.AddChild(secondLeaf);
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var viewModel = new StateDefinitionEditorViewModel(prop);
		var item = viewModel.SelectedStateItem;

		item.AssignmentTree.Single().IsAssigned = true;

		Assert.Equal([firstLeaf.Id, secondLeaf.Id], item.StateItem.ElementModelIds);
		Assert.True(item.HasAssignments);
		Assert.False(viewModel.HasValidationErrors);
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
		var viewModel = new StateDefinitionEditorViewModel(prop);

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

		var viewModel = new StateDefinitionEditorViewModel(prop);

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
		var editorViewModel = new StateDefinitionEditorViewModel(prop);
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
	public void ToggleElementModelId_AddsAndRemovesAssignment()
	{
		var prop = CreatePropWithModel(out var model, out var leaf);
		model.ModelType = ElementModelType.Model;
		var definition = StateDefinitionModel.CreateDefault("Wave");
		model.StateDefinitionModels.Add(definition);
		var editorViewModel = new StateDefinitionEditorViewModel(prop);
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
		Assert.Equal(Color.LightGray.ToArgb(), drawingPanelViewModel.LightNodes.Single(light => light.Light.ParentModelId == secondLeaf.Id).DisplayColor.ToArgb());
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

	private static Prop CreatePropWithModel(out ElementModel model, out ElementModel leaf)
	{
		var prop = new Prop("State Prop");
		model = new ElementModel("Model", prop.RootNode);
		leaf = new ElementModel("Leaf 1", model);
		model.AddChild(leaf);
		prop.RootNode.AddChild(model);
		return prop;
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
}
