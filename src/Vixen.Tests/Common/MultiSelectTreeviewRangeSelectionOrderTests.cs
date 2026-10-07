using System.Reflection;
using System.Windows.Forms;
using Common.Controls;
using Xunit;

namespace Vixen.Tests.Common;

public sealed class MultiSelectTreeviewRangeSelectionOrderTests
{
	[Theory]
	[InlineData(0, 2, new[] { "A", "B", "C" })]
	[InlineData(2, 0, new[] { "C", "B", "A" })]
	public void ShiftRangeUsesAnchorToEndpointOrder(int anchorIndex, int endpointIndex, string[] expectedOrder)
	{
		var treeView = new MultiSelectTreeview();
		foreach (string name in new[] { "A", "B", "C" }) {
			treeView.Nodes.Add(new TreeNode(name));
		}
		treeView.SelectedNode = treeView.Nodes[anchorIndex];

		SelectWithShift(treeView, treeView.Nodes[endpointIndex]);

		Assert.Equal(["A", "B", "C"], treeView.SelectedNodes.Select(node => node.Text));
		Assert.Equal(expectedOrder, treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void ShrinkingAndCrossingAnchorRecomputesDirection()
	{
		var treeView = new MultiSelectTreeview();
		foreach (string name in new[] { "A", "B", "C", "D" }) {
			treeView.Nodes.Add(new TreeNode(name));
		}
		treeView.SelectedNode = treeView.Nodes[1];

		SelectWithShift(treeView, treeView.Nodes[3]);
		SelectWithShift(treeView, treeView.Nodes[2]);
		Assert.Equal(["B", "C"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
		SelectWithShift(treeView, treeView.Nodes[0]);

		Assert.Equal(["A", "B"], treeView.SelectedNodes.Select(node => node.Text));
		Assert.Equal(["B", "A"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void AddSelectedNodeAndCtrlToggleInvalidateRangeSnapshot()
	{
		var treeView = new MultiSelectTreeview();
		foreach (string name in new[] { "A", "B", "C" }) {
			treeView.Nodes.Add(new TreeNode(name));
		}
		treeView.SelectedNode = treeView.Nodes[2];
		SelectWithShift(treeView, treeView.Nodes[1]);

		treeView.AddSelectedNode(treeView.Nodes[0]);

		Assert.Equal(treeView.SelectedNodes.Select(node => node.Text), treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
		SelectWithShift(treeView, treeView.Nodes[0]);
		SelectWithControl(treeView, treeView.Nodes[1]);

		Assert.Equal(treeView.SelectedNodes.Select(node => node.Text), treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void ReplacingSelectedNodesInvalidatesRangeSnapshot()
	{
		var treeView = new MultiSelectTreeview();
		foreach (string name in new[] { "A", "B", "C" }) {
			treeView.Nodes.Add(new TreeNode(name));
		}
		treeView.SelectedNode = treeView.Nodes[2];
		SelectWithShift(treeView, treeView.Nodes[1]);

		treeView.SelectedNodes = [treeView.Nodes[0], treeView.Nodes[2]];

		Assert.Equal(treeView.SelectedNodes.Select(node => node.Text), treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void VisibleRangeOrderSkipsCollapsedDescendants()
	{
		var treeView = new MultiSelectTreeview();
		var parent = new TreeNode("A");
		parent.Nodes.Add(new TreeNode("A1"));
		parent.Collapse();
		treeView.Nodes.Add(parent);
		treeView.Nodes.Add(new TreeNode("B"));
		treeView.SelectedNode = treeView.Nodes[1];

		SelectWithShift(treeView, parent);

		Assert.Equal(["B", "A"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
	}

	[Fact]
	public void SingletonRangeAndClearExposeCurrentSelectionWithoutStaleDirection()
	{
		var treeView = new MultiSelectTreeview();
		treeView.Nodes.Add(new TreeNode("A"));
		treeView.Nodes.Add(new TreeNode("B"));
		treeView.SelectedNode = treeView.Nodes[1];

		SelectWithShift(treeView, treeView.Nodes[1]);

		Assert.Equal(["B"], treeView.SelectedNodesInSelectionOrder.Select(node => node.Text));
		treeView.ClearSelectedNodes();

		Assert.Empty(treeView.SelectedNodesInSelectionOrder);
	}

	private static void SelectWithShift(MultiSelectTreeview treeView, TreeNode endpoint) => SelectNode(treeView, endpoint, Keys.Shift);

	private static void SelectWithControl(MultiSelectTreeview treeView, TreeNode endpoint) => SelectNode(treeView, endpoint, Keys.Control);

	private static void SelectNode(MultiSelectTreeview treeView, TreeNode endpoint, Keys modifiers)
	{
		MethodInfo selectNode = typeof(MultiSelectTreeview).GetMethod(
			"SelectNode",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(TreeNode), typeof(Keys)],
			null)!;
		selectNode.Invoke(treeView, [endpoint, modifiers]);
	}
}
