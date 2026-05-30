using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Models;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateAssignmentTreeTests
{
	[Fact]
	public void CheckedLeaf_ReturnsOneEffectiveAssignment()
	{
		// Arrange
		var tree = CreateTree(out _, out _, out var firstLeafId, out _);
		var firstLeaf = tree.Children[0].Children[0];

		// Act
		firstLeaf.IsChecked = true;

		// Assert
		Assert.Equal([firstLeafId], tree.GetSelectedNodeIds());
		Assert.Equal([firstLeafId], tree.GetEffectiveLeafNodeIds());
		Assert.Single(tree.GetEffectiveLeafNodeIds());
	}

	[Fact]
	public void CheckingGroup_ClearsSelectedDescendantsDisablesThemAndCountsEffectiveChildren()
	{
		// Arrange
		var tree = CreateTree(out _, out var groupId, out var firstLeafId, out var secondLeafId);
		var group = tree.Children[0];
		var firstLeaf = group.Children[0];
		var secondLeaf = group.Children[1];
		firstLeaf.IsChecked = true;
		secondLeaf.IsChecked = true;

		// Act
		group.IsChecked = true;

		// Assert
		Assert.False(firstLeaf.IsChecked);
		Assert.False(secondLeaf.IsChecked);
		Assert.False(firstLeaf.IsEnabled);
		Assert.False(secondLeaf.IsEnabled);
		Assert.Equal([groupId], tree.GetSelectedNodeIds());
		Assert.Equal([firstLeafId, secondLeafId], tree.GetEffectiveLeafNodeIds());
		Assert.Equal(2, tree.GetEffectiveLeafNodeIds().Count());
	}

	[Fact]
	public void UncheckingGroup_ReenablesDescendantsWithoutRestoringChecks()
	{
		// Arrange
		var tree = CreateTree(out _, out _, out _, out _);
		var group = tree.Children[0];
		var firstLeaf = group.Children[0];
		firstLeaf.IsChecked = true;
		group.IsChecked = true;

		// Act
		group.IsChecked = false;

		// Assert
		Assert.True(firstLeaf.IsEnabled);
		Assert.True(group.Children[1].IsEnabled);
		Assert.False(firstLeaf.IsChecked);
		Assert.Empty(tree.GetSelectedNodeIds());
		Assert.Empty(tree.GetEffectiveLeafNodeIds());
	}

	private static StateAssignmentTreeNode CreateTree(
		out Guid rootId,
		out Guid groupId,
		out Guid firstLeafId,
		out Guid secondLeafId)
	{
		rootId = Guid.NewGuid();
		groupId = Guid.NewGuid();
		firstLeafId = Guid.NewGuid();
		secondLeafId = Guid.NewGuid();
		var snapshot = new StateElementNodeSnapshot(
			rootId,
			"Root",
			[
				new StateElementNodeSnapshot(
					groupId,
					"Group",
					[
						new StateElementNodeSnapshot(firstLeafId, "First", []),
						new StateElementNodeSnapshot(secondLeafId, "Second", [])
					])
			]);

		return new StateAssignmentTreeNode(snapshot, new HashSet<Guid>());
	}
}
