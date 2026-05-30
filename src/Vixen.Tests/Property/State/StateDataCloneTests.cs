using System.Drawing;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateDataCloneTests
{
	[Fact]
	public void Clone_CopiesDefinitionAndItemsDeeply()
	{
		// Arrange
		var elementNodeId = Guid.NewGuid();
		var secondElementNodeId = Guid.NewGuid();
		var source = new StateData
		{
			Name = "Operating Mode",
			Description = "Available operating modes",
			Items =
			[
				new StateItemData
				{
					Id = Guid.NewGuid(),
					Name = "Enabled",
					Color = Color.Green,
					ElementNodeIds = [elementNodeId]
				},
				new StateItemData
				{
					Id = Guid.NewGuid(),
					Name = "Disabled",
					Color = Color.Red,
					ElementNodeIds = [secondElementNodeId]
				}
			]
		};

		// Act
		var clone = (StateData)source.Clone();
		clone.Name = "Changed";
		clone.Items[0].Name = "Disabled";
		clone.Items[0].ElementNodeIds.Add(Guid.NewGuid());
		clone.Items[1].ElementNodeIds.Clear();

		// Assert
		Assert.Equal("Operating Mode", source.Name);
		Assert.Equal("Enabled", source.Items[0].Name);
		Assert.Equal([elementNodeId], source.Items[0].ElementNodeIds);
		Assert.Equal([secondElementNodeId], source.Items[1].ElementNodeIds);
		Assert.NotSame(source.Items, clone.Items);
		Assert.NotSame(source.Items[0], clone.Items[0]);
		Assert.NotSame(source.Items[0].ElementNodeIds, clone.Items[0].ElementNodeIds);
		Assert.NotSame(source.Items[1], clone.Items[1]);
		Assert.NotSame(source.Items[1].ElementNodeIds, clone.Items[1].ElementNodeIds);
	}

	[Fact]
	public void ModuleData_InitializesMissingCollections()
	{
		// Arrange
		var data = new StateData
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
		};
		var module = new StateModule();

		// Act
		module.ModuleData = data;

		// Assert
		Assert.Equal("State Name 1", data.Name);
		Assert.Equal(string.Empty, data.Description);
		Assert.Equal("Item Name 1", data.Items[0].Name);
		Assert.NotNull(data.Items[0].ElementNodeIds);

		data.Items = null!;
		module.ModuleData = data;

		Assert.NotNull(data.Items);
		Assert.Empty(data.Items);
	}
}
