using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.Sys;

public class BuiltInElementTagsTests
{
	[Fact]
	public void CreateDefaults_ReturnsThreeDefinitions()
	{
		// Arrange & Act
		var defaults = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.Equal(3, defaults.Count);
	}

	[Fact]
	public void CreateDefaults_AllReturnedDefinitionsAreBuiltIn()
	{
		// Arrange & Act
		var defaults = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.All(defaults, tag => Assert.True(tag.IsBuiltIn));
	}

	[Fact]
	public void CreateDefaults_ContainsDeprecatedWithStableId()
	{
		// Arrange & Act
		var defaults = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.Contains(defaults, tag => tag.Id == BuiltInElementTags.DeprecatedId && tag.Key == BuiltInElementTags.DeprecatedKey);
	}

	[Fact]
	public void CreateDefaults_ContainsHiddenWithStableId()
	{
		// Arrange & Act
		var defaults = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.Contains(defaults, tag => tag.Id == BuiltInElementTags.HiddenId && tag.Key == BuiltInElementTags.HiddenKey);
	}

	[Fact]
	public void CreateDefaults_ContainsPropWithStableId()
	{
		// Arrange & Act
		var defaults = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.Contains(defaults, tag => tag.Id == BuiltInElementTags.PropId && tag.Key == BuiltInElementTags.PropKey);
	}

	[Fact]
	public void CreateDefaults_CalledTwice_ReturnsEqualIdsEachTime()
	{
		// Arrange
		var first = BuiltInElementTags.CreateDefaults();

		// Act
		var second = BuiltInElementTags.CreateDefaults();

		// Assert
		Assert.Equal(first.Select(t => t.Id), second.Select(t => t.Id));
	}
}
