using System.Drawing;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateDataCloneTests
{
	[Fact]
	public void Constructor_CreatesDefaultStateDefinitionWithNonEmptyIds()
	{
		// Act
		var data = new StateData();

		// Assert
		Assert.NotEqual(Guid.Empty, data.Id);
		Assert.Single(data.StateDefinitions);
		Assert.NotEqual(Guid.Empty, data.StateDefinitions[0].Id);
		Assert.Equal("State - 1", data.StateDefinitions[0].Name);
		Assert.Single(data.StateDefinitions[0].Items);
		Assert.NotEqual(Guid.Empty, data.StateDefinitions[0].Items[0].Id);
	}

	[Fact]
	public void Clone_CopiesDefinitionsAndItemsDeeply()
	{
		// Arrange
		var elementNodeId = Guid.NewGuid();
		var secondElementNodeId = Guid.NewGuid();
		var definitionId = Guid.NewGuid();
		var firstItemId = Guid.NewGuid();
		var secondItemId = Guid.NewGuid();
		var source = new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Id = definitionId,
					Name = "Operating Mode",
					Description = "Available operating modes",
					Items =
					[
						new StateItemData
						{
							Id = firstItemId,
							Name = "Enabled",
							Color = Color.Green,
							ElementNodeIds = [elementNodeId]
						},
						new StateItemData
						{
							Id = secondItemId,
							Name = "Disabled",
							Color = Color.Red,
							ElementNodeIds = [secondElementNodeId]
						}
					]
				}
			]
		};

		// Act
		var clone = (StateData)source.Clone();
		clone.StateDefinitions[0].Name = "Changed";
		clone.StateDefinitions[0].Items[0].Name = "Disabled";
		clone.StateDefinitions[0].Items[0].ElementNodeIds.Add(Guid.NewGuid());
		clone.StateDefinitions[0].Items[1].ElementNodeIds.Clear();

		// Assert
		Assert.Equal(source.Id, clone.Id);
		Assert.Equal(definitionId, clone.StateDefinitions[0].Id);
		Assert.Equal(firstItemId, clone.StateDefinitions[0].Items[0].Id);
		Assert.Equal(secondItemId, clone.StateDefinitions[0].Items[1].Id);
		Assert.Equal("Operating Mode", source.StateDefinitions[0].Name);
		Assert.Equal("Enabled", source.StateDefinitions[0].Items[0].Name);
		Assert.Equal([elementNodeId], source.StateDefinitions[0].Items[0].ElementNodeIds);
		Assert.Equal([secondElementNodeId], source.StateDefinitions[0].Items[1].ElementNodeIds);
		Assert.NotSame(source.StateDefinitions, clone.StateDefinitions);
		Assert.NotSame(source.StateDefinitions[0], clone.StateDefinitions[0]);
		Assert.NotSame(source.StateDefinitions[0].Items, clone.StateDefinitions[0].Items);
		Assert.NotSame(source.StateDefinitions[0].Items[0], clone.StateDefinitions[0].Items[0]);
		Assert.NotSame(source.StateDefinitions[0].Items[0].ElementNodeIds, clone.StateDefinitions[0].Items[0].ElementNodeIds);
		Assert.NotSame(source.StateDefinitions[0].Items[1], clone.StateDefinitions[0].Items[1]);
		Assert.NotSame(source.StateDefinitions[0].Items[1].ElementNodeIds, clone.StateDefinitions[0].Items[1].ElementNodeIds);
	}

	[Fact]
	public void CloneForNewProperty_RegeneratesDefinitionAndItemIds()
	{
		// Arrange
		var source = new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Id = Guid.NewGuid(),
					Name = "Operating Mode",
					Description = "Available operating modes",
					Items =
					[
						new StateItemData
						{
							Id = Guid.NewGuid(),
							Name = "Enabled",
							Color = Color.Green,
							ElementNodeIds = [Guid.NewGuid()]
						}
					]
				}
			]
		};

		// Act
		var clone = source.CloneForNewProperty();

		// Assert
		Assert.NotEqual(source.Id, clone.Id);
		Assert.NotEqual(source.StateDefinitions[0].Id, clone.StateDefinitions[0].Id);
		Assert.NotEqual(source.StateDefinitions[0].Items[0].Id, clone.StateDefinitions[0].Items[0].Id);
		Assert.Equal(source.StateDefinitions[0].Name, clone.StateDefinitions[0].Name);
		Assert.Equal(source.StateDefinitions[0].Description, clone.StateDefinitions[0].Description);
		Assert.Equal(source.StateDefinitions[0].Items[0].Name, clone.StateDefinitions[0].Items[0].Name);
		Assert.Equal(source.StateDefinitions[0].Items[0].Color, clone.StateDefinitions[0].Items[0].Color);
		Assert.Equal(source.StateDefinitions[0].Items[0].ElementNodeIds, clone.StateDefinitions[0].Items[0].ElementNodeIds);
	}

	[Fact]
	public void ModuleData_InitializesMissingCollections()
	{
		// Arrange
		var data = new StateData
		{
			StateDefinitions =
			[
				new StateDefinitionData
				{
					Name = null!,
					Description = null!,
					Items =
					[
						new StateItemData
						{
							Name = null!,
							ElementNodeIds = null!
						}
					]
				}
			]
		};
		var module = new StateModule();

		// Act
		module.ModuleData = data;

		// Assert
		Assert.Equal("State - 1", data.StateDefinitions[0].Name);
		Assert.Equal(string.Empty, data.Description);
		Assert.Equal("Item Name 1", data.Items[0].Name);
		Assert.NotNull(data.Items[0].ElementNodeIds);

		data.StateDefinitions = null!;
		module.ModuleData = data;

		Assert.NotNull(data.StateDefinitions);
		Assert.Single(data.StateDefinitions);
	}

	[Fact]
	public void ModuleData_NormalizesEmptyId()
	{
		// Arrange
		var data = new StateData
		{
			Id = Guid.Empty
		};
		var module = new StateModule();

		// Act
		module.ModuleData = data;

		// Assert
		Assert.NotEqual(Guid.Empty, data.Id);
		Assert.Equal(data.Id, module.Id);
	}
}
