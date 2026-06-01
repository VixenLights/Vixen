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
		};
		var clone = new StateModule();

		// Act
		clone.CloneValues(source);
		clone.Name = "Changed";
		clone.Items[0].Name = "Disabled";
		clone.Items[0].ElementNodeIds.Remove(firstElementNodeId);
		clone.Items[1].ElementNodeIds.Clear();

		// Assert
		Assert.NotEqual(Guid.Empty, source.Id);
		Assert.NotEqual(Guid.Empty, clone.Id);
		Assert.NotEqual(source.Id, clone.Id);
		Assert.Equal("Operating Mode", source.Name);
		Assert.Equal("Available operating modes", source.Description);
		Assert.Equal("Enabled", source.Items[0].Name);
		Assert.Equal(source.Items[0].Id, clone.Items[0].Id);
		Assert.Equal(source.Items[0].Color, clone.Items[0].Color);
		Assert.Equal([firstElementNodeId, secondElementNodeId], source.Items[0].ElementNodeIds);
		Assert.Equal(source.Items[1].Id, clone.Items[1].Id);
		Assert.Equal(source.Items[1].Color, clone.Items[1].Color);
		Assert.Equal([secondElementNodeId], source.Items[1].ElementNodeIds);
		Assert.NotSame(source.Items, clone.Items);
		Assert.NotSame(source.Items[0], clone.Items[0]);
		Assert.NotSame(source.Items[0].ElementNodeIds, clone.Items[0].ElementNodeIds);
		Assert.NotSame(source.Items[1], clone.Items[1]);
		Assert.NotSame(source.Items[1].ElementNodeIds, clone.Items[1].ElementNodeIds);
	}
}
