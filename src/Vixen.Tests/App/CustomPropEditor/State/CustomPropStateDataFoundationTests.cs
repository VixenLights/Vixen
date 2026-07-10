using System.Collections.ObjectModel;
using System.Drawing;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.State;

public sealed class CustomPropStateDataFoundationTests
{
	[Fact]
	public void Mapper_ToStateData_PreservesStableIdsAndAssignments()
	{
		var assignedElementId = Guid.NewGuid();
		var model = new ElementModel("Model")
		{
			StatePropertyId = Guid.NewGuid(),
			StateDefinitionModels =
			[
				new StateDefinitionModel
				{
					Id = Guid.NewGuid(),
					Name = "Wave",
					Description = "Arm positions",
					Items =
					[
						new StateItemModel
						{
							Id = Guid.NewGuid(),
							Name = "Hand",
							Color = Color.Red,
							ElementModelIds = new ObservableCollection<Guid> { assignedElementId, assignedElementId }
						}
					]
				}
			]
		};

		var stateData = CustomPropStateDataMapper.ToStateData(model);

		Assert.Equal(model.StatePropertyId, stateData.Id);
		var definition = Assert.Single(stateData.StateDefinitions);
		Assert.Equal(model.StateDefinitionModels[0].Id, definition.Id);
		Assert.Equal("Wave", definition.Name);
		Assert.Equal("Arm positions", definition.Description);
		var item = Assert.Single(definition.Items);
		Assert.Equal(model.StateDefinitionModels[0].Items[0].Id, item.Id);
		Assert.Equal("Hand", item.Name);
		Assert.Equal(Color.Red, item.Color);
		Assert.Equal([assignedElementId], item.ElementNodeIds);
	}

	[Fact]
	public void Mapper_ApplyToModel_PreservesStatePropertyModelShape()
	{
		var stateData = new StateData
		{
			Id = Guid.NewGuid(),
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Id = Guid.NewGuid(),
					Name = "Static",
					Description = "Static pose",
					Items =
					[
						new StateItemData
						{
							Id = Guid.NewGuid(),
							Name = "Coat",
							Color = Color.Blue,
							ElementNodeIds = [Guid.NewGuid()]
						}
					]
				}
			]
		};
		var model = new ElementModel("Model");

		CustomPropStateDataMapper.ApplyToModel(model, stateData);

		Assert.Equal(stateData.Id, model.StatePropertyId);
		var definition = Assert.Single(model.StateDefinitionModels);
		Assert.Equal(stateData.StateDefinitions[0].Id, definition.Id);
		Assert.Equal("Static", definition.Name);
		Assert.Equal("Static pose", definition.Description);
		var item = Assert.Single(definition.Items);
		Assert.Equal(stateData.StateDefinitions[0].Items[0].Id, item.Id);
		Assert.Equal("Coat", item.Name);
		Assert.Equal(Color.Blue, item.Color);
		Assert.Equal(stateData.StateDefinitions[0].Items[0].ElementNodeIds, item.ElementModelIds);
	}

	[Fact]
	public void Resolver_GetModelElement_ReturnsExplicitModelOrRootFallback()
	{
		var prop = new Prop("Santa");
		var model = new ElementModel("Santa - Model", prop.RootNode);
		var subModel = new ElementModel("Santa - Arm", prop.RootNode);
		prop.RootNode.AddChild(model);
		prop.RootNode.AddChild(subModel);

		Assert.Same(prop.RootNode, PropStateModelResolver.GetModelElement(prop));

		model.ModelType = ElementModelType.Model;

		Assert.Same(model, PropStateModelResolver.GetModelElement(prop));
	}

	[Fact]
	public void Resolver_TrySetModelType_AllowsOnlyOneExplicitModel()
	{
		var prop = new Prop("Santa");
		var first = new ElementModel("First", prop.RootNode);
		var second = new ElementModel("Second", prop.RootNode);
		first.AddChild(new ElementModel("First Child", first));
		second.AddChild(new ElementModel("Second Child", second));
		prop.RootNode.AddChild(first);
		prop.RootNode.AddChild(second);

		Assert.True(PropStateModelResolver.TrySetModelType(prop, first, ElementModelType.Model));
		Assert.True(PropStateModelResolver.TrySetModelType(prop, second, ElementModelType.Model));

		Assert.Equal(ElementModelType.None, first.ModelType);
		Assert.Equal(ElementModelType.Model, second.ModelType);
	}

	[Fact]
	public void Resolver_TrySetModelType_RejectsLeafModelWhenPropHasOtherElements()
	{
		var prop = new Prop("Santa");
		var leaf = new ElementModel("Leaf", prop.RootNode);
		prop.RootNode.AddChild(leaf);

		Assert.False(PropStateModelResolver.TrySetModelType(prop, leaf, ElementModelType.Model));
		Assert.Equal(ElementModelType.None, leaf.ModelType);
	}

	[Fact]
	public void Migration_FromLegacyImportedRows_CreatesDefinitionModels()
	{
		var prop = new Prop("Santa");
		var model = new ElementModel("Santa - Model", prop.RootNode);
		var firstLeaf = new ElementModel("Px 1", model);
		var secondLeaf = new ElementModel("Px 2", model);
		model.AddChild(firstLeaf);
		model.AddChild(secondLeaf);
		prop.RootNode.AddChild(model);

		model.StateDefinitions.Add(new StateDefinition
		{
			StateDefinitionName = "Wave",
			Name = "Hand",
			DefaultColor = Color.Red,
			Index = 1,
			ElementModelIds = [firstLeaf.Id, secondLeaf.Id]
		});

		var migrated = CustomPropStateMigrationService.MigrateLegacyStateData(prop);

		Assert.True(migrated);
		var definition = Assert.Single(model.StateDefinitionModels);
		Assert.Equal("Wave", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Hand", item.Name);
		Assert.Equal(Color.Red, item.Color);
		Assert.Equal([firstLeaf.Id, secondLeaf.Id], item.ElementModelIds);

		Assert.False(CustomPropStateMigrationService.MigrateLegacyStateData(prop));
		Assert.Single(model.StateDefinitionModels);
	}

	[Fact]
	public void Migration_FromLegacyElementStateDefinition_UsesModelElementStorage()
	{
		var prop = new Prop("Santa");
		var model = new ElementModel("Santa - Model", prop.RootNode);
		var stateElement = new ElementModel("Santa - Wave", prop.RootNode)
		{
			StateDefinition = new StateDefinition
			{
				StateDefinitionName = "Wave",
				Name = "Hand",
				DefaultColor = Color.Green
			}
		};
		prop.RootNode.AddChild(model);
		prop.RootNode.AddChild(stateElement);
		model.ModelType = ElementModelType.Model;

		var migrated = CustomPropStateMigrationService.MigrateLegacyStateData(prop);

		Assert.True(migrated);
		var definition = Assert.Single(model.StateDefinitionModels);
		Assert.Equal("Wave", definition.Name);
		var item = Assert.Single(definition.Items);
		Assert.Equal("Hand", item.Name);
		Assert.Equal(Color.Green, item.Color);
		Assert.Equal([stateElement.Id], item.ElementModelIds);
	}
}
