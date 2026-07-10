using System.Drawing;
using Moq;
using Vixen.Sys;
using VixenModules.Effect.State;
using Xunit;

namespace Vixen.Tests.Effect.State;

public class StateRenderSegmentCoalescerTests
{
	[Fact]
	public void Coalesce_MergesAdjacentMatchingSegments()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var elementId = Guid.NewGuid();
		var leafNode = CreateLeafNode(elementId);
		var segments = new[]
		{
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.Zero, TimeSpan.FromSeconds(1)),
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))
		};

		// Act
		var coalesced = StateRenderSegmentCoalescer.Coalesce(segments);

		// Assert
		var segment = Assert.Single(coalesced);
		Assert.Equal(TimeSpan.Zero, segment.Start);
		Assert.Equal(TimeSpan.FromSeconds(3), segment.Duration);
	}

	[Fact]
	public void Coalesce_DoesNotMergeAcrossGap()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var elementId = Guid.NewGuid();
		var leafNode = CreateLeafNode(elementId);
		var segments = new[]
		{
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.Zero, TimeSpan.FromSeconds(1)),
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1))
		};

		// Act
		var coalesced = StateRenderSegmentCoalescer.Coalesce(segments);

		// Assert
		Assert.Equal(2, coalesced.Count);
	}

	[Fact]
	public void Coalesce_DoesNotMergeDifferentStateItemsWithSameLeafAndColor()
	{
		// Arrange
		var elementId = Guid.NewGuid();
		var leafNode = CreateLeafNode(elementId);
		var segments = new[]
		{
			CreateSegment(Guid.NewGuid(), leafNode, elementId, Color.Red, TimeSpan.Zero, TimeSpan.FromSeconds(1)),
			CreateSegment(Guid.NewGuid(), leafNode, elementId, Color.Red, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
		};

		// Act
		var coalesced = StateRenderSegmentCoalescer.Coalesce(segments);

		// Assert
		Assert.Equal(2, coalesced.Count);
	}

	[Fact]
	public void Coalesce_DoesNotMergeAcrossInterveningSegment()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var elementId = Guid.NewGuid();
		var leafNode = CreateLeafNode(elementId);
		var interveningElementId = Guid.NewGuid();
		var interveningLeafNode = CreateLeafNode(interveningElementId);
		var segments = new[]
		{
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.Zero, TimeSpan.FromSeconds(1)),
			CreateSegment(Guid.NewGuid(), interveningLeafNode, interveningElementId, Color.Blue, TimeSpan.Zero, TimeSpan.FromSeconds(1)),
			CreateSegment(stateItemId, leafNode, elementId, Color.Red, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
		};

		// Act
		var coalesced = StateRenderSegmentCoalescer.Coalesce(segments);

		// Assert
		Assert.Equal(3, coalesced.Count);
		Assert.Equal(segments.Select(segment => segment.ElementId), coalesced.Select(segment => segment.ElementId));
	}

	private static StateRenderSegment CreateSegment(
		Guid stateItemId,
		IElementNode leafNode,
		Guid elementId,
		Color color,
		TimeSpan start,
		TimeSpan duration)
	{
		return new StateRenderSegment(stateItemId, leafNode, elementId, color, start, duration);
	}

	private static IElementNode CreateLeafNode(Guid elementId)
	{
		var leafNode = new Mock<IElementNode>();
		leafNode.SetupGet(node => node.Id).Returns(elementId);
		return leafNode.Object;
	}
}
