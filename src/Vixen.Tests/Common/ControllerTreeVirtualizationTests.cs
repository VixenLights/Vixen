using System.Reflection;
using System.Windows.Forms;
using Common.Controls;
using Moq;
using Vixen.Sys.Output;
using Xunit;

namespace Vixen.Tests.Common;

/// <summary>
/// Characterizes the bounded controller-output materialization required by Display Setup.
/// </summary>
public sealed class ControllerTreeVirtualizationTests
{
	[Fact]
	public void InitialPopulation_CreatesOnlyRootsAndVirtualChildren()
	{
		var controllerTree = PopulateTree(CreateController(1), CreateController(5), CreateController(0));
		var populatedTree = controllerTree.TreeViewForTests;

		Assert.Equal(3, populatedTree.Nodes.Count);
		AssertVirtualChild(populatedTree.Nodes[0]);
		AssertVirtualChild(populatedTree.Nodes[1]);
		Assert.Empty(populatedTree.Nodes[2].Nodes.Cast<TreeNode>());
		Assert.DoesNotContain(AllNodes(populatedTree.Nodes), node => node.Tag is int);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(4999)]
	[InlineData(5000)]
	public void ExpandingSmallController_MaterializesDirectOutputLeaves(int outputCount)
	{
		var controllerTree = PopulateTree(CreateController(outputCount));
		var populatedTree = controllerTree.TreeViewForTests;
		var controllerNode = Assert.Single(populatedTree.Nodes.Cast<TreeNode>());

		controllerTree.ExpandNodeForTests(controllerNode);

		Assert.Equal(outputCount, controllerNode.Nodes.Count);
		Assert.All(controllerNode.Nodes.Cast<TreeNode>(), node => Assert.IsType<int>(node.Tag));
		Assert.DoesNotContain(controllerNode.Nodes.Cast<TreeNode>(), node => node.Name == "VIRT");
	}

	[Theory]
	[InlineData(5001, 2, "Outputs 5001-5001")]
	[InlineData(10000, 2, "Outputs 5001-10000")]
	[InlineData(15000, 3, "Outputs 10001-15000")]
	public void ExpandingLargeController_CreatesBoundedOutputRanges(int outputCount, int expectedRangeCount, string expectedLastRangeLabel)
	{
		var controllerTree = PopulateTree(CreateController(outputCount));
		var populatedTree = controllerTree.TreeViewForTests;
		var controllerNode = Assert.Single(populatedTree.Nodes.Cast<TreeNode>());

		controllerTree.ExpandNodeForTests(controllerNode);

		Assert.Equal(expectedRangeCount, controllerNode.Nodes.Count);
		Assert.Equal(expectedLastRangeLabel, controllerNode.LastNode!.Text);
		Assert.All(controllerNode.Nodes.Cast<TreeNode>(), AssertVirtualRange);
		Assert.DoesNotContain(AllNodes(controllerNode.Nodes), node => node.Tag is int);
	}

	[Fact]
	public void ExpandingOneRange_MaterializesOnlyThatPageWithOutputMetadata()
	{
		var controller = CreateController(10000, outputNames: index => $"Channel {index + 1}");
		var controllerTree = PopulateTree(controller);
		var populatedTree = controllerTree.TreeViewForTests;
		var controllerNode = Assert.Single(populatedTree.Nodes.Cast<TreeNode>());
		controllerTree.ExpandNodeForTests(controllerNode);
		var selectedRange = controllerNode.Nodes[1];

		controllerTree.ExpandNodeForTests(selectedRange);

		Assert.InRange(selectedRange.Nodes.Count, 1, 5000);
		Assert.All(selectedRange.Nodes.Cast<TreeNode>(), node =>
		{
			var outputIndex = Assert.IsType<int>(node.Tag);
			Assert.Equal($"Channel {outputIndex + 1}", node.Text);
			Assert.Equal("WhiteBall", node.ImageKey);
		});
		Assert.All(controllerNode.Nodes.Cast<TreeNode>().Where(node => !ReferenceEquals(node, selectedRange)), AssertVirtualRange);
	}

	[Fact]
	public void CollapseAndReexpand_DoesNotDuplicateRangesOrLeaves()
	{
		var controllerTree = PopulateTree(CreateController(10000));
		var populatedTree = controllerTree.TreeViewForTests;
		var controllerNode = Assert.Single(populatedTree.Nodes.Cast<TreeNode>());
		controllerTree.ExpandNodeForTests(controllerNode);
		var firstRange = controllerNode.FirstNode!;
		controllerTree.ExpandNodeForTests(firstRange);
		var initialRangeCount = controllerNode.Nodes.Count;
		var initialLeafCount = firstRange.Nodes.Count;

		firstRange.Collapse();
		controllerTree.ExpandNodeForTests(firstRange);
		controllerNode.Collapse();
		controllerTree.ExpandNodeForTests(controllerNode);

		Assert.Equal(initialRangeCount, controllerNode.Nodes.Count);
		Assert.Equal(initialLeafCount, firstRange.Nodes.Count);
	}

	[Fact]
	public void CollapsingRange_EvictsLeavesAndRestoresLogicalSelectionOnExpansion()
	{
		var controller = CreateController(10000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [1] });

		var rangeNode = controllerTree.TreeViewForTests.Nodes[0].Nodes[0];
		Assert.Equal(5000, rangeNode.Nodes.Count);

		controllerTree.CollapseNodeForTests(rangeNode);

		AssertVirtualChild(rangeNode);
		Assert.Equal([1], Assert.Single(controllerTree.GetSelectedControllerOutputs()).Value);

		controllerTree.ExpandNodeForTests(rangeNode);
		Assert.Contains(rangeNode.Nodes.Cast<TreeNode>(), node => node.Tag is int outputIndex && outputIndex == 1 &&
			controllerTree.SelectedTreeNodes.Contains(node));
	}

	[Fact]
	public void CollapsingController_EvictsRangesAndLeaves()
	{
		var controllerTree = PopulateTree(CreateController(10000));
		var controllerNode = controllerTree.TreeViewForTests.Nodes[0];
		controllerTree.ExpandNodeForTests(controllerNode);
		controllerTree.ExpandNodeForTests(controllerNode.Nodes[0]);

		controllerTree.CollapseNodeForTests(controllerNode);

		AssertVirtualChild(controllerNode);
		Assert.Single(AllNodes(controllerNode.Nodes));
	}

	[Fact]
	public void ExpandingAnotherController_DoesNotMaterializeOutputsForOtherControllers()
	{
		var controllerTree = PopulateTree(CreateController(10000), CreateController(10000));
		var populatedTree = controllerTree.TreeViewForTests;
		var firstController = populatedTree.Nodes[0];
		var secondController = populatedTree.Nodes[1];

		controllerTree.ExpandNodeForTests(secondController);

		AssertVirtualChild(firstController);
		Assert.DoesNotContain(AllNodes(firstController.Nodes), node => node.Tag is int);
		Assert.Equal(2, secondController.Nodes.Count);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(4999)]
	[InlineData(5000)]
	[InlineData(9999)]
	public void SelectingOutput_MaterializesOnlyItsContainingPage(int outputIndex)
	{
		var controller = CreateController(10000, outputNames: _ => "Duplicate output name");
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SelectOutputForTests(controller, outputIndex);

		var selectedNode = Assert.Single(controllerTree.SelectedTreeNodes);
		Assert.Equal(outputIndex, Assert.IsType<int>(selectedNode.Tag));
		Assert.InRange(AllNodes(controllerTree.TreeViewForTests.Nodes).Count(node => node.Tag is int), 1, 5000);
	}

	[Fact]
	public void RepopulatingAfterOutputCountChange_RecreatesOnlyTheRequiredVirtualChild()
	{
		var controllerId = Guid.NewGuid();
		var originalController = CreateController(10000, controllerId);
		var resizedController = CreateController(5001, controllerId);
		using var controllerTree = new ControllerTree();

		controllerTree.PopulateControllerTreeForTests([originalController]);
		controllerTree.TreeViewForTests.Nodes[0].Expand();
		controllerTree.PopulateControllerTreeForTests([resizedController]);

		var controllerNode = Assert.Single(controllerTree.TreeViewForTests.Nodes.Cast<TreeNode>());
		AssertVirtualChild(controllerNode);
		Assert.DoesNotContain(AllNodes(controllerNode.Nodes), node => node.Tag is int);
	}

	[Fact]
	public void LogicalSelection_ExportsAndMaterializesAllMatchingPages()
	{
		var controller = CreateController(10000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>>
		{
			[controller] = Enumerable.Range(0, 10000).ToHashSet()
		});

		var selected = Assert.Single(controllerTree.GetSelectedControllerOutputs());
		Assert.Equal(10000, selected.Value.Count);
		Assert.Equal(10000, AllNodes(controllerTree.TreeViewForTests.Nodes).Count(node => node.Tag is int));
	}

	[Fact]
	public void LogicalSelection_ReplacesPreviousOutputs()
	{
		var controller = CreateController(10000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [1, 257] });
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [9999] });

		var selected = Assert.Single(controllerTree.GetSelectedControllerOutputs());
		Assert.Equal([9999], selected.Value);
	}

	[Fact]
	public void LogicalSelection_ExpandsMatchedRangesAndHighlightsOnlyMatchingOutputs()
	{
		var controller = CreateController(10000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [1, 5001, 5599] });

		var controllerNode = Assert.Single(controllerTree.TreeViewForTests.Nodes.Cast<TreeNode>());
		Assert.Equal(2, controllerNode.Nodes.Count);
		Assert.Equal("Outputs 5001-10000", controllerNode.Nodes[1].Text);
		var secondRange = controllerNode.Nodes[1];
		controllerTree.ExpandNodeForTests(secondRange);

		var selectedOutputs = secondRange.Nodes.Cast<TreeNode>()
			.Where(controllerTree.SelectedTreeNodes.Contains)
			.Select(node => Assert.IsType<int>(node.Tag));
		Assert.Equal([5001, 5599], selectedOutputs.Order());
		Assert.DoesNotContain(controllerTree.GetSelectedControllerOutputs().SelectMany(pair => pair.Value), output => output is < 0 or >= 10000);
	}

	private static ControllerTree PopulateTree(params IControllerDevice[] controllers)
	{
		var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests(controllers);
		return controllerTree;
	}

	private static IControllerDevice CreateController(int outputCount, Guid? id = null, Func<int, string>? outputNames = null)
	{
		var controller = new Mock<IControllerDevice>();
		var outputs = Enumerable.Range(0, outputCount)
			.Select(index => CreateOutput(outputNames?.Invoke(index) ?? $"Output {index + 1}", index))
			.ToArray();

		controller.SetupGet(device => device.Id).Returns(id ?? Guid.NewGuid());
		controller.SetupGet(device => device.Name).Returns("Test controller");
		controller.SetupGet(device => device.IsRunning).Returns(false);
		controller.SetupGet(device => device.OutputCount).Returns(outputCount);
		controller.SetupGet(device => device.Outputs).Returns(outputs);
		return controller.Object;
	}

	private static CommandOutput CreateOutput(string name, int index)
	{
		var constructor = typeof(CommandOutput).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic,
			binder: null,
			types: [typeof(Guid), typeof(string), typeof(int)],
			modifiers: null)!;

		return (CommandOutput)constructor.Invoke([Guid.NewGuid(), name, index]);
	}

	private static IEnumerable<TreeNode> AllNodes(TreeNodeCollection nodes)
	{
		foreach (TreeNode node in nodes)
		{
			yield return node;
			foreach (TreeNode child in AllNodes(node.Nodes))
			{
				yield return child;
			}
		}
	}

	private static void AssertVirtualChild(TreeNode node)
	{
		var child = Assert.Single(node.Nodes.Cast<TreeNode>());
		Assert.Equal("VIRT", child.Name);
		Assert.IsNotType<int>(child.Tag);
	}

	private static void AssertVirtualRange(TreeNode node)
	{
		Assert.IsNotType<int>(node.Tag);
		AssertVirtualChild(node);
	}
}
