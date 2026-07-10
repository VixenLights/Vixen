using System.Drawing;
using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateModuleCloneTests
{
	[Fact]
	public void CloneValues_CopiesAllStateItemsAndAssignments()
	{
		// Arrange
		var firstElementNodeId = Guid.NewGuid();
		var secondElementNodeId = Guid.NewGuid();
		var source = new StateModule
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
							ElementNodeIds = [firstElementNodeId, secondElementNodeId]
						},
						new StateItemData
						{
							Id = Guid.NewGuid(),
							Name = "Disabled",
							Color = Color.Red,
							ElementNodeIds = [secondElementNodeId]
						}
					]
				}
			]
		};
		var clone = new StateModule();

		// Act
		clone.CloneValues(source);
		clone.StateDefinitions[0].Name = "Changed";
		clone.StateDefinitions[0].Items[0].Name = "Disabled";
		clone.StateDefinitions[0].Items[0].ElementNodeIds.Remove(firstElementNodeId);
		clone.StateDefinitions[0].Items[1].ElementNodeIds.Clear();

		// Assert
		Assert.NotEqual(Guid.Empty, source.Id);
		Assert.NotEqual(Guid.Empty, clone.Id);
		Assert.NotEqual(source.Id, clone.Id);
		Assert.Equal("Operating Mode", source.StateDefinitions[0].Name);
		Assert.Equal("Available operating modes", source.StateDefinitions[0].Description);
		Assert.Equal("Enabled", source.StateDefinitions[0].Items[0].Name);
		Assert.NotEqual(source.StateDefinitions[0].Id, clone.StateDefinitions[0].Id);
		Assert.NotEqual(source.StateDefinitions[0].Items[0].Id, clone.StateDefinitions[0].Items[0].Id);
		Assert.Equal(source.StateDefinitions[0].Items[0].Color, clone.StateDefinitions[0].Items[0].Color);
		Assert.Equal([firstElementNodeId, secondElementNodeId], source.StateDefinitions[0].Items[0].ElementNodeIds);
		Assert.NotEqual(source.StateDefinitions[0].Items[1].Id, clone.StateDefinitions[0].Items[1].Id);
		Assert.Equal(source.StateDefinitions[0].Items[1].Color, clone.StateDefinitions[0].Items[1].Color);
		Assert.Equal([secondElementNodeId], source.StateDefinitions[0].Items[1].ElementNodeIds);
		Assert.NotSame(source.StateDefinitions[0].Items, clone.StateDefinitions[0].Items);
		Assert.NotSame(source.StateDefinitions[0].Items[0], clone.StateDefinitions[0].Items[0]);
		Assert.NotSame(source.StateDefinitions[0].Items[0].ElementNodeIds, clone.StateDefinitions[0].Items[0].ElementNodeIds);
		Assert.NotSame(source.StateDefinitions[0].Items[1], clone.StateDefinitions[0].Items[1]);
		Assert.NotSame(source.StateDefinitions[0].Items[1].ElementNodeIds, clone.StateDefinitions[0].Items[1].ElementNodeIds);
	}
}
