using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.Sys;

public class ElementTagCollectionTests
{
	[Fact]
	public void Add_NewTagId_ReturnsTrueAndIsContained()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();

		// Act
		var result = tags.Add(tagId);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void Add_NewTagId_ContainsReturnsTrue()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();

		// Act
		tags.Add(tagId);

		// Assert
		Assert.True(tags.Contains(tagId));
	}

	[Fact]
	public void Add_SameTagIdTwice_SecondCallReturnsFalse()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();
		tags.Add(tagId);

		// Act
		var result = tags.Add(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void Add_SameTagIdTwice_CountStaysOne()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();
		tags.Add(tagId);

		// Act
		tags.Add(tagId);

		// Assert
		Assert.Equal(1, tags.Count);
	}

	[Fact]
	public void Remove_PresentTagId_ReturnsTrue()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();
		tags.Add(tagId);

		// Act
		var result = tags.Remove(tagId);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void Remove_PresentTagId_ContainsReturnsFalse()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();
		tags.Add(tagId);
		tags.Remove(tagId);

		// Act
		var result = tags.Contains(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void Remove_AbsentTagId_ReturnsFalse()
	{
		// Arrange
		var tags = new ElementTagCollection();
		var tagId = Guid.NewGuid();

		// Act
		var result = tags.Remove(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void Empty_Add_ReturnsFalse()
	{
		// Arrange
		var tagId = Guid.NewGuid();

		// Act
		var result = ElementTagCollection.Empty.Add(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void Empty_AddCalledRepeatedly_NeverBecomesContained()
	{
		// Arrange
		var tagId = Guid.NewGuid();
		ElementTagCollection.Empty.Add(tagId);
		ElementTagCollection.Empty.Add(tagId);

		// Act
		var result = ElementTagCollection.Empty.Contains(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void ProxyElementNode_Tags_IsTheSharedEmptyInstance()
	{
		// Arrange
		var node = new ProxyElementNode(Guid.NewGuid(), "proxy");

		// Act & Assert
		Assert.Same(ElementTagCollection.Empty, node.Tags);
	}

	[Fact]
	public void ProxyElementNode_TagsAdd_ReturnsFalse()
	{
		// Arrange
		var node = new ProxyElementNode(Guid.NewGuid(), "proxy");
		var tagId = Guid.NewGuid();

		// Act
		var result = node.Tags.Add(tagId);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void ProxyElementNode_TagsCount_IsZero()
	{
		// Arrange
		var node = new ProxyElementNode(Guid.NewGuid(), "proxy");

		// Act & Assert
		Assert.Equal(0, node.Tags.Count);
	}
}
