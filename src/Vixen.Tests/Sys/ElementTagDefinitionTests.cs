using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.Sys;

public class ElementTagDefinitionTests
{
	[Fact]
	public void Constructor_NullKey_KeyDefaultsToEmptyString()
	{
		// Arrange & Act
		var tag = new ElementTagDefinition(Guid.NewGuid(), null, "Custom", isBuiltIn: false);

		// Assert
		Assert.Equal(string.Empty, tag.Key);
	}

	[Fact]
	public void Constructor_SortOrderNotSet_DefaultsToZero()
	{
		// Arrange & Act
		var tag = new ElementTagDefinition(Guid.NewGuid(), "custom-key", "Custom", isBuiltIn: false);

		// Assert
		Assert.Equal(0, tag.SortOrder);
	}

	[Fact]
	public void Name_SetAfterConstruction_UpdatesValue()
	{
		// Arrange
		var tag = new ElementTagDefinition(Guid.NewGuid(), null, "Original", isBuiltIn: false);

		// Act
		tag.Name = "Renamed";

		// Assert
		Assert.Equal("Renamed", tag.Name);
	}
}
