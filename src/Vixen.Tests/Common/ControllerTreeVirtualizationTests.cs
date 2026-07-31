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
	[InlineData(255)]
	[InlineData(256)]
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
	[InlineData(257, 2, "Outputs 257-257")]
	[InlineData(5000, 20, "Outputs 4865-5000")]
	[InlineData(512, 2, "Outputs 257-512")]
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
		var controller = CreateController(5000, outputNames: index => $"Channel {index + 1}");
		var controllerTree = PopulateTree(controller);
		var populatedTree = controllerTree.TreeViewForTests;
		var controllerNode = Assert.Single(populatedTree.Nodes.Cast<TreeNode>());
		controllerTree.ExpandNodeForTests(controllerNode);
		var selectedRange = controllerNode.Nodes[1];

		controllerTree.ExpandNodeForTests(selectedRange);

		Assert.InRange(selectedRange.Nodes.Count, 1, 256);
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
		var controllerTree = PopulateTree(CreateController(5000));
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
	public void ExpandingAnotherController_DoesNotMaterializeOutputsForOtherControllers()
	{
		var controllerTree = PopulateTree(CreateController(5000), CreateController(5000));
		var populatedTree = controllerTree.TreeViewForTests;
		var firstController = populatedTree.Nodes[0];
		var secondController = populatedTree.Nodes[1];

		controllerTree.ExpandNodeForTests(secondController);

		AssertVirtualChild(firstController);
		Assert.DoesNotContain(AllNodes(firstController.Nodes), node => node.Tag is int);
		Assert.Equal(20, secondController.Nodes.Count);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(255)]
	[InlineData(256)]
	[InlineData(4999)]
	public void SelectingOutput_MaterializesOnlyItsContainingPage(int outputIndex)
	{
		var controller = CreateController(5000, outputNames: _ => "Duplicate output name");
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SelectOutputForTests(controller, outputIndex);

		var selectedNode = Assert.Single(controllerTree.SelectedTreeNodes);
		Assert.Equal(outputIndex, Assert.IsType<int>(selectedNode.Tag));
		Assert.InRange(AllNodes(controllerTree.TreeViewForTests.Nodes).Count(node => node.Tag is int), 1, 256);
	}

	[Fact]
	public void RepopulatingAfterOutputCountChange_RecreatesOnlyTheRequiredVirtualChild()
	{
		var controllerId = Guid.NewGuid();
		var originalController = CreateController(5000, controllerId);
		var resizedController = CreateController(257, controllerId);
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
		var controller = CreateController(5000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>>
		{
			[controller] = Enumerable.Range(0, 5000).ToHashSet()
		});

		var selected = Assert.Single(controllerTree.GetSelectedControllerOutputs());
		Assert.Equal(5000, selected.Value.Count);
		Assert.Equal(5000, AllNodes(controllerTree.TreeViewForTests.Nodes).Count(node => node.Tag is int));
	}

	[Fact]
	public void LogicalSelection_ReplacesPreviousOutputs()
	{
		var controller = CreateController(5000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);

		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [1, 257] });
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [4999] });

		var selected = Assert.Single(controllerTree.GetSelectedControllerOutputs());
		Assert.Equal([4999], selected.Value);
	}

	[Fact]
	public void LogicalSelection_ExpandsMatchedRangesAndHighlightsOnlyMatchingOutputs()
	{
		var controller = CreateController(5000);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>> { [controller] = [1, 257, 299] });

		var controllerNode = Assert.Single(controllerTree.TreeViewForTests.Nodes.Cast<TreeNode>());
		Assert.Equal(20, controllerNode.Nodes.Count);
		Assert.Equal("Outputs 257-512", controllerNode.Nodes[1].Text);
		var secondRange = controllerNode.Nodes[1];
		controllerTree.ExpandNodeForTests(secondRange);

		var selectedOutputs = secondRange.Nodes.Cast<TreeNode>()
			.Where(controllerTree.SelectedTreeNodes.Contains)
			.Select(node => Assert.IsType<int>(node.Tag));
		Assert.Equal([257, 299], selectedOutputs.Order());
		Assert.DoesNotContain(controllerTree.GetSelectedControllerOutputs().SelectMany(pair => pair.Value), output => output is < 0 or >= 5000);
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
