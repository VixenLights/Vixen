using VixenModules.Property.State.Setup.Models;
using Xunit;

namespace Vixen.Tests.Property.State;

public class StateAssignmentTreeSelectionTests
{
	[Fact]
	public void GetVisibleNodesInDisplayOrder_ReturnsExpandedNodesInDisplayOrder()
	{
		// Arrange
		var tree = CreateTree();
		tree.Root.IsExpanded = true;
		tree.Group.IsExpanded = true;
		tree.SecondGroup.IsExpanded = false;

		// Act
		var visibleNames = tree.Root
			.GetVisibleNodesInDisplayOrder()
			.Select(node => node.Name)
			.ToList();

		// Assert
		Assert.Equal(
			["Root", "Group", "First", "Second", "Second Group", "Standalone"],
			visibleNames);
	}

	[Fact]
	public void SelectSingle_SelectsNodeWithoutCheckingAssignment()
	{
		// Arrange
		var tree = CreateTree();
		var firstLeaf = tree.Group.Children[0];
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.SelectSingle(firstLeaf);

		// Assert
		Assert.True(firstLeaf.IsSelected);
		Assert.False(firstLeaf.IsChecked);
		Assert.Empty(tree.Root.GetSelectedNodeIds());
		Assert.Equal(1, controller.SelectedCount);
	}

	[Fact]
	public void ToggleSelection_AddsAndRemovesNode()
	{
		// Arrange
		var tree = CreateTree();
		var firstLeaf = tree.Group.Children[0];
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.ToggleSelection(firstLeaf);
		controller.ToggleSelection(firstLeaf);

		// Assert
		Assert.False(firstLeaf.IsSelected);
		Assert.Equal(0, controller.SelectedCount);
	}

	[Fact]
	public void SelectRange_WithoutAnchorSelectsSingleNode()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.SelectRange(tree.SecondGroup.Children[0]);

		// Assert
		Assert.Equal(["Third"], GetSelectedNodeNames(tree.Root));
	}

	[Fact]
	public void SelectRange_SelectsVisibleEnabledNodesBetweenAnchorAndTarget()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		var firstLeaf = tree.Group.Children[0];
		var secondGroupLeaf = tree.SecondGroup.Children[0];
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.SelectSingle(firstLeaf);
		controller.SelectRange(secondGroupLeaf);

		// Assert
		Assert.Equal(
			["First", "Second", "Second Group", "Third"],
			GetSelectedNodeNames(tree.Root));
	}

	[Fact]
	public void SelectRange_ExcludesCollapsedDescendants()
	{
		// Arrange
		var tree = CreateTree();
		tree.Root.IsExpanded = true;
		tree.Group.IsExpanded = false;
		tree.SecondGroup.IsExpanded = true;
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.SelectSingle(tree.Group);
		controller.SelectRange(tree.SecondGroup.Children[0]);

		// Assert
		Assert.Equal(
			["Group", "Second Group", "Third"],
			GetSelectedNodeNames(tree.Root));
		Assert.False(tree.Group.Children[0].IsSelected);
		Assert.False(tree.Group.Children[1].IsSelected);
	}

	[Fact]
	public void SelectRange_ExcludesDisabledDescendants()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		tree.Group.IsChecked = true;
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);

		// Act
		controller.SelectSingle(tree.Group);
		controller.SelectRange(tree.Standalone);

		// Assert
		Assert.Equal(
			["Group", "Second Group", "Third", "Standalone"],
			GetSelectedNodeNames(tree.Root));
		Assert.False(tree.Group.Children[0].IsSelected);
		Assert.False(tree.Group.Children[1].IsSelected);
	}

	[Fact]
	public void ToggleCheckedStateForSelectedNodes_InvertsMixedSelectedNodes()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		var firstLeaf = tree.Group.Children[0];
		var secondLeaf = tree.Group.Children[1];
		firstLeaf.IsChecked = true;
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);
		controller.SelectSingle(firstLeaf);
		controller.ToggleSelection(secondLeaf);

		// Act
		controller.ToggleCheckedStateForSelectedNodes();

		// Assert
		Assert.False(firstLeaf.IsChecked);
		Assert.True(secondLeaf.IsChecked);
	}

	[Fact]
	public void ToggleCheckedStateForSelectedNodes_CheckingGroupPreservesGroupRules()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		var firstLeaf = tree.Group.Children[0];
		var secondLeaf = tree.Group.Children[1];
		firstLeaf.IsChecked = true;
		secondLeaf.IsChecked = true;
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);
		controller.SelectSingle(tree.Group);

		// Act
		controller.ToggleCheckedStateForSelectedNodes();

		// Assert
		Assert.True(tree.Group.IsChecked);
		Assert.False(firstLeaf.IsChecked);
		Assert.False(secondLeaf.IsChecked);
		Assert.False(firstLeaf.IsEnabled);
		Assert.False(secondLeaf.IsEnabled);
	}

	[Fact]
	public void ToggleCheckedStateForSelectedNodes_UncheckingGroupReenablesDescendants()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		tree.Group.IsChecked = true;
		var firstLeaf = tree.Group.Children[0];
		var secondLeaf = tree.Group.Children[1];
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);
		controller.SelectSingle(tree.Group);

		// Act
		controller.ToggleCheckedStateForSelectedNodes();

		// Assert
		Assert.False(tree.Group.IsChecked);
		Assert.True(firstLeaf.IsEnabled);
		Assert.True(secondLeaf.IsEnabled);
		Assert.False(firstLeaf.IsChecked);
		Assert.False(secondLeaf.IsChecked);
	}

	[Fact]
	public void ClearSelection_ClearsSelectedNodesAndAnchor()
	{
		// Arrange
		var tree = CreateTree();
		ExpandAll(tree.Root);
		var controller = new StateAssignmentTreeSelectionController([tree.Root]);
		controller.SelectSingle(tree.Group.Children[0]);
		controller.ClearSelection();

		// Act
		controller.SelectRange(tree.SecondGroup.Children[0]);

		// Assert
		Assert.Equal(["Third"], GetSelectedNodeNames(tree.Root));
	}

	private static IReadOnlyList<string> GetSelectedNodeNames(StateAssignmentTreeNode root)
	{
		return root.GetVisibleNodesInDisplayOrder()
			.Where(node => node.IsSelected)
			.Select(node => node.Name)
			.ToList();
	}

	private static void ExpandAll(StateAssignmentTreeNode node)
	{
		node.IsExpanded = true;
		foreach (var child in node.Children)
		{
			ExpandAll(child);
		}
	}

	private static TestTree CreateTree()
	{
		var snapshot = new StateElementNodeSnapshot(
			Guid.NewGuid(),
			"Root",
			[
				new StateElementNodeSnapshot(
					Guid.NewGuid(),
					"Group",
					[
						new StateElementNodeSnapshot(Guid.NewGuid(), "First", []),
						new StateElementNodeSnapshot(Guid.NewGuid(), "Second", [])
					]),
				new StateElementNodeSnapshot(
					Guid.NewGuid(),
					"Second Group",
					[
						new StateElementNodeSnapshot(Guid.NewGuid(), "Third", [])
					]),
				new StateElementNodeSnapshot(Guid.NewGuid(), "Standalone", [])
			]);
		var root = new StateAssignmentTreeNode(snapshot, new HashSet<Guid>());

		return new TestTree(
			root,
			root.Children[0],
			root.Children[1],
			root.Children[2]);
	}

	private sealed record TestTree(
		StateAssignmentTreeNode Root,
		StateAssignmentTreeNode Group,
		StateAssignmentTreeNode SecondGroup,
		StateAssignmentTreeNode Standalone);
}
