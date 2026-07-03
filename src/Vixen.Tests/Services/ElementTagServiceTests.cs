using Vixen.Services;
using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.Services;

public class ElementTagServiceTests
{
	[Fact]
	public void ValidateAndNormalizeTagName_BlankName_ThrowsArgumentException()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => ElementTagService.ValidateAndNormalizeTagName("   ", existing));
	}

	[Fact]
	public void ValidateAndNormalizeTagName_EmptyName_ThrowsArgumentException()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => ElementTagService.ValidateAndNormalizeTagName(string.Empty, existing));
	}

	[Fact]
	public void ValidateAndNormalizeTagName_CaseInsensitiveDuplicate_ThrowsArgumentException()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>
		{
			new ElementTagDefinition(Guid.NewGuid(), null, "Deprecated", isBuiltIn: false)
		};

		// Act & Assert
		Assert.Throws<ArgumentException>(() => ElementTagService.ValidateAndNormalizeTagName("deprecated", existing));
	}

	[Fact]
	public void ValidateAndNormalizeTagName_PaddedDuplicate_ThrowsArgumentException()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>
		{
			new ElementTagDefinition(Guid.NewGuid(), null, "Outdoor", isBuiltIn: false)
		};

		// Act & Assert
		Assert.Throws<ArgumentException>(() => ElementTagService.ValidateAndNormalizeTagName("  outdoor  ", existing));
	}

	[Fact]
	public void ValidateAndNormalizeTagName_DistinctName_DoesNotThrow()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>
		{
			new ElementTagDefinition(Guid.NewGuid(), null, "Deprecated", isBuiltIn: false)
		};

		// Act & Assert
		var exception = Record.Exception(() => ElementTagService.ValidateAndNormalizeTagName("Outdoor", existing));
		Assert.Null(exception);
	}

	[Fact]
	public void ValidateAndNormalizeTagName_PaddedName_ReturnsTrimmedValue()
	{
		// Arrange
		var existing = new List<ElementTagDefinition>();

		// Act
		var result = ElementTagService.ValidateAndNormalizeTagName("  Outdoor  ", existing);

		// Assert
		Assert.Equal("Outdoor", result);
	}

	[Fact]
	public void ValidateAndNormalizeTagName_RenameToOwnCurrentNameExcludedFromComparison_DoesNotThrow()
	{
		// Arrange
		var tag = new ElementTagDefinition(Guid.NewGuid(), null, "Outdoor", isBuiltIn: false);
		var existing = new List<ElementTagDefinition> { tag };

		// Act & Assert
		var exception = Record.Exception(() =>
			ElementTagService.ValidateAndNormalizeTagName("Outdoor", existing.Where(t => t.Id != tag.Id)));
		Assert.Null(exception);
	}
}
