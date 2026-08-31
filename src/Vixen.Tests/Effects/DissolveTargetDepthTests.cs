using System.ComponentModel;
using System.Reflection;
using Moq;
using Vixen.Module.Effect;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Effect.Dissolve;
using Xunit;

namespace Vixen.Tests.Effects;

/// <summary>
/// Verifies that Dissolve keeps its target-dependent depth selection valid.
/// </summary>
public sealed class DissolveTargetDepthTests
{
	/// <summary>
	/// Verifies that moving to a shallower target resets an invalid depth and notifies the editor binding.
	/// </summary>
	[Fact]
	public void TargetChange_NormalizesInvalidDepthAndNotifiesBindings()
	{
		// Arrange
		var effect = new Dissolve { EnableDepth = true };
		SetTargetNodes(effect, [CreateTargetNode(3)]);
		effect.DepthOfEffect = 2;
		var changedProperties = new List<string?>();
		effect.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);

		// Act
		SetTargetNodes(effect, [CreateTargetNode(2)]);

		// Assert
		Assert.Equal(0, effect.DepthOfEffect);
		Assert.Contains(nameof(Dissolve.DepthOfEffect), changedProperties);
	}

	/// <summary>
	/// Verifies that a stale selector value cannot restore an invalid target depth.
	/// </summary>
	[Fact]
	public void DepthSelection_ReapplyingInvalidValueDoesNotNotifyBindings()
	{
		// Arrange
		var effect = new Dissolve { EnableDepth = true };
		SetTargetNodes(effect, [CreateTargetNode(2)]);
		effect.DepthOfEffect = 0;
		var changedProperties = new List<string?>();
		effect.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);

		// Act
		effect.DepthOfEffect = 2;

		// Assert
		Assert.Equal(0, effect.DepthOfEffect);
		Assert.DoesNotContain(nameof(Dissolve.DepthOfEffect), changedProperties);
	}

	private static IElementNode CreateTargetNode(int maximumChildDepth)
	{
		var targetNode = new Mock<IElementNode>();
		targetNode.SetupGet(node => node.Children).Returns([]);
		targetNode.SetupGet(node => node.Properties).Returns(new PropertyManager(targetNode.Object));
		targetNode.Setup(node => node.GetMaxChildDepth()).Returns(maximumChildDepth);

		return targetNode.Object;
	}

	private static void SetTargetNodes(Dissolve effect, IElementNode[] targetNodes)
	{
		var targetNodesField = typeof(EffectModuleInstanceBase).GetField("_targetNodes", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(targetNodesField);
		targetNodesField.SetValue(effect, targetNodes);

		var targetNodesChanged = typeof(Dissolve).GetMethod("TargetNodesChanged", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(targetNodesChanged);
		targetNodesChanged.Invoke(effect, []);
	}
}
