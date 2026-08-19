using VixenModules.Editor.EffectEditor;
using Xunit;

namespace Vixen.Tests.EffectEditor;

/// <summary>
/// Tests collection item movement without requiring an Effect Editor visual tree.
/// </summary>
public sealed class PropertyItemCollectionMoveTests
{
	[Fact]
	public void TryMoveItem_MovesInteriorItemUp()
	{
		// Arrange
		var first = new object();
		var movedItem = new object();
		var last = new object();
		var items = new List<object> { first, movedItem, last };

		// Act
		var moved = PropertyItem.TryMoveItem(items, 1, 0);

		// Assert
		Assert.True(moved);
		Assert.Same(movedItem, items[0]);
		Assert.Same(first, items[1]);
		Assert.Same(last, items[2]);
	}

	[Fact]
	public void TryMoveItem_MovesInteriorItemDown()
	{
		// Arrange
		var first = new object();
		var movedItem = new object();
		var last = new object();
		var items = new List<object> { first, movedItem, last };

		// Act
		var moved = PropertyItem.TryMoveItem(items, 1, 2);

		// Assert
		Assert.True(moved);
		Assert.Same(first, items[0]);
		Assert.Same(last, items[1]);
		Assert.Same(movedItem, items[2]);
	}

	[Fact]
	public void TryMoveItem_PreservesMovedObjectIdentity()
	{
		// Arrange
		var first = new object();
		var movedItem = new object();
		var last = new object();
		var items = new List<object> { first, movedItem, last };

		// Act
		var moved = PropertyItem.TryMoveItem(items, 1, 2);

		// Assert
		Assert.True(moved);
		Assert.Same(movedItem, items[2]);
	}

	[Theory]
	[InlineData(-1, 0)]
	[InlineData(0, -1)]
	[InlineData(1, 1)]
	[InlineData(3, 0)]
	[InlineData(0, 3)]
	[InlineData(5, 0)]
	[InlineData(0, 5)]
	public void TryMoveItem_RejectsInvalidIndexesWithoutChangingTheCollection(int sourceIndex, int targetIndex)
	{
		// Arrange
		var first = new object();
		var middle = new object();
		var last = new object();
		var items = new List<object> { first, middle, last };

		// Act
		var moved = PropertyItem.TryMoveItem(items, sourceIndex, targetIndex);

		// Assert
		Assert.False(moved);
		Assert.Equal([first, middle, last], items);
	}

	[Fact]
	public void TryMoveItem_RejectsEmptyCollection()
	{
		// Arrange
		var items = new List<object>();

		// Act
		var moved = PropertyItem.TryMoveItem(items, 0, 1);

		// Assert
		Assert.False(moved);
		Assert.Empty(items);
	}

	[Fact]
	public void TryMoveItem_RejectsSingleItemCollection()
	{
		// Arrange
		var item = new object();
		var items = new List<object> { item };

		// Act
		var moved = PropertyItem.TryMoveItem(items, 0, 0);

		// Assert
		Assert.False(moved);
		Assert.Single(items);
		Assert.Same(item, items[0]);
	}
}
