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
