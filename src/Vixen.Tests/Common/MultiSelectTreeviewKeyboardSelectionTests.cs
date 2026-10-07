using Common.Controls;
using System.Windows.Forms;
using Xunit;

namespace Vixen.Tests.Common;

public sealed class MultiSelectTreeviewKeyboardSelectionTests
{
	[Fact]
	public void ShiftDownExtendsSelectionToNextVisibleNode()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[1];

		bool handled = treeView.ProcessKeyboardSelection(Keys.Down, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["B", "C"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[2], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftUpExtendsSelectionToPreviousVisibleNode()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[1];

		bool handled = treeView.ProcessKeyboardSelection(Keys.Up, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["A", "B"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[0], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftUpAfterShiftDownShrinksSelectionTowardAnchor()
	{
		var treeView = CreateTreeView("A", "B", "C", "D");
		treeView.SelectedNode = treeView.Nodes[1];

		treeView.ProcessKeyboardSelection(Keys.Down, Keys.Shift);
		treeView.ProcessKeyboardSelection(Keys.Down, Keys.Shift);
		bool handled = treeView.ProcessKeyboardSelection(Keys.Up, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["B", "C"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[2], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftHomeSelectsRangeToFirstVisibleNode()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[1];

		bool handled = treeView.ProcessKeyboardSelection(Keys.Home, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["A", "B"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[0], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftEndSelectsRangeToLastVisibleNode()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[1];

		bool handled = treeView.ProcessKeyboardSelection(Keys.End, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["B", "C"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[2], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftUpExposesAnchorToEndpointOrderWithoutChangingCanonicalOrder()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[2];
		IReadOnlyList<TreeNode>? eventOrder = null;
		treeView.AfterSelect += (_, _) => eventOrder = treeView.SelectedNodesInSelectionOrder.ToArray();

		bool handled = treeView.ProcessKeyboardSelection(Keys.Up, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["B", "C"], SelectedNodeNames(treeView));
		Assert.Equal(["C", "B"], eventOrder!.Select(node => node.Text));
		Assert.Equal(["C", "B"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void ShiftHomeAndEndExposeTheirAnchorToEndpointDirection()
	{
		var treeView = CreateTreeView("A", "B", "C");
		treeView.SelectedNode = treeView.Nodes[2];

		treeView.ProcessKeyboardSelection(Keys.Home, Keys.Shift);

		Assert.Equal(["A", "B", "C"], SelectedNodeNames(treeView));
		Assert.Equal(["C", "B", "A"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));

		treeView.SelectedNode = treeView.Nodes[0];
		treeView.ProcessKeyboardSelection(Keys.End, Keys.Shift);

		Assert.Equal(["A", "B", "C"], SelectedNodeNames(treeView));
		Assert.Equal(["A", "B", "C"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void RangeSnapshotRemainsStableAndOrdinarySelectionInvalidatesIt()
	{
		var treeView = CreateTreeView("A", "B", "C", "D");
		treeView.SelectedNode = treeView.Nodes[2];
		treeView.ProcessKeyboardSelection(Keys.Up, Keys.Shift);
		TreeNode[] previousSnapshot = treeView.SelectedNodesInSelectionOrder.ToArray();

		treeView.ProcessKeyboardSelection(Keys.Up, Keys.Shift);

		Assert.Equal(["C", "B"], previousSnapshot.Select(node => node.Text));
		Assert.Equal(["C", "B", "A"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
		treeView.SelectedNode = treeView.Nodes[3];

		Assert.Equal(["D"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void ShiftDownWithNoSelectionSelectsTopNode()
	{
		var treeView = CreateTreeView("A", "B");

		bool handled = treeView.ProcessKeyboardSelection(Keys.Down, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["A"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[0], treeView.SelectedNode);
	}

	[Fact]
	public void CtrlDownDoesNothing()
	{
		var treeView = CreateTreeView("A", "B");
		treeView.SelectedNode = treeView.Nodes[0];

		bool handled = treeView.ProcessKeyboardSelection(Keys.Down, Keys.Control);

		Assert.False(handled);
		Assert.Equal(["A"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[0], treeView.SelectedNode);
	}

	[Fact]
	public void DeleteIsNotHandledBySharedKeyboardSelection()
	{
		var treeView = CreateTreeView("A", "B");
		treeView.SelectedNode = treeView.Nodes[0];

		bool handled = treeView.ProcessKeyboardSelection(Keys.Delete, Keys.None);

		Assert.False(handled);
		Assert.Equal(["A"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[0], treeView.SelectedNode);
	}

	[Fact]
	public void ShiftDownSkipsCollapsedDescendants()
	{
		var treeView = new MultiSelectTreeview();
		var parent = new TreeNode("A");
		parent.Nodes.Add(new TreeNode("A1"));
		treeView.Nodes.Add(parent);
		treeView.Nodes.Add(new TreeNode("B"));
		parent.Collapse();
		treeView.SelectedNode = parent;

		bool handled = treeView.ProcessKeyboardSelection(Keys.Down, Keys.Shift);

		Assert.True(handled);
		Assert.Equal(["A", "B"], SelectedNodeNames(treeView));
		Assert.Same(treeView.Nodes[1], treeView.SelectedNode);
	}

	private static MultiSelectTreeview CreateTreeView(params string[] nodeNames)
	{
		var treeView = new MultiSelectTreeview();
		foreach (string nodeName in nodeNames) {
			treeView.Nodes.Add(new TreeNode(nodeName));
		}

		return treeView;
	}

	private static string[] SelectedNodeNames(MultiSelectTreeview treeView)
	{
		return treeView.SelectedNodes.Select(node => node.Text).ToArray();
	}
}
