using Moq;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Effect.Effect;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class BaseEffectDepthTests
{
	[Fact]
	public void GetNodesAtEffectDepth_DepthZeroReturnsDistinctLeaves()
	{
		// Arrange
		var leaf = CreateLeafNode("Leaf");
		var root = CreateGroupNode("Root", leaf, leaf);
		var effect = new TestEffect();

		// Act
		var nodes = effect.GetNodesAtDepth(root, 0);

		// Assert
		AssertSameSequence([leaf], nodes);
	}

	[Fact]
	public void GetNodesAtEffectDepth_DepthOneReturnsDistinctChildren()
	{
		// Arrange
		var firstChild = CreateGroupNode("First", CreateLeafNode("First Leaf"));
		var secondChild = CreateGroupNode("Second", CreateLeafNode("Second Leaf"));
		var root = CreateGroupNode("Root", firstChild, secondChild, firstChild);
		var effect = new TestEffect();

		// Act
		var nodes = effect.GetNodesAtDepth(root, 1);

		// Assert
		AssertSameSequence([firstChild, secondChild], nodes);
	}

	[Fact]
	public void GetNodesAtEffectDepth_DeeperDepthReturnsDistinctNodesAtThatLevel()
	{
		// Arrange
		var firstLeaf = CreateLeafNode("First Leaf");
		var secondLeaf = CreateLeafNode("Second Leaf");
		var firstChild = CreateGroupNode("First", firstLeaf);
		var secondChild = CreateGroupNode("Second", secondLeaf);
		var root = CreateGroupNode("Root", firstChild, secondChild);
		var effect = new TestEffect();

		// Act
		var nodes = effect.GetNodesAtDepth(root, 2);

		// Assert
		AssertSameSequence([firstLeaf, secondLeaf], nodes);
	}

	[Fact]
	public void GetNodesAtEffectDepth_OverDeepSelectionFallsBackToDistinctLeaves()
	{
		// Arrange
		var firstLeaf = CreateLeafNode("First Leaf");
		var secondLeaf = CreateLeafNode("Second Leaf");
		var root = CreateGroupNode("Root", firstLeaf, secondLeaf);
		var effect = new TestEffect();

		// Act
		var nodes = effect.GetNodesAtDepth(root, 3);

		// Assert
		AssertSameSequence([firstLeaf, secondLeaf], nodes);
	}

	private static void AssertSameSequence(IElementNode[] expected, IReadOnlyList<IElementNode> actual)
	{
		Assert.Equal(expected.Length, actual.Count);
		for (var i = 0; i < expected.Length; i++)
		{
			Assert.Same(expected[i], actual[i]);
		}
	}

	private static IElementNode CreateLeafNode(string name)
	{
		var leafNode = new Mock<IElementNode>();
		leafNode.SetupGet(node => node.Name).Returns(name);
		leafNode.SetupGet(node => node.Children).Returns([]);
		leafNode.Setup(node => node.GetLeafEnumerator()).Returns([leafNode.Object]);

		return leafNode.Object;
	}

	private static IElementNode CreateGroupNode(string name, params IElementNode[] children)
	{
		var groupNode = new Mock<IElementNode>();
		groupNode.SetupGet(node => node.Name).Returns(name);
		groupNode.SetupGet(node => node.Children).Returns(children);
		groupNode.Setup(node => node.GetLeafEnumerator()).Returns(children.SelectMany(child => child.GetLeafEnumerator()));

		return groupNode.Object;
	}

	private sealed class TestEffect : BaseEffect
	{
		private readonly TestEffectData _data = new TestEffectData();

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		public List<IElementNode> GetNodesAtDepth(IElementNode node, int depthOfEffect)
		{
			return GetNodesAtEffectDepth(node, depthOfEffect);
		}

		protected override void TargetNodesChanged()
		{
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken)
		{
		}

		protected override EffectIntents _Render()
		{
			return new EffectIntents();
		}
	}

	private sealed class TestEffectData : EffectTypeModuleData
	{
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return new TestEffectData
			{
				TargetPositioning = TargetPositioning
			};
		}
	}
}
