using VixenModules.App.CustomPropEditor.Import.XLights;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.Import.XLights;

public class XLightsStateNameNormalizerTests
{
	[Theory]
	[InlineData("  Operating Mode  ", "Operating Mode")]
	[InlineData("  Enabled  ", "Enabled")]
	public void NormalizeStateName_TrimsValidName(string importedName, string expectedName)
	{
		// Act
		var normalizedName = XLightsStateNameNormalizer.NormalizeStateName(importedName, 1);

		// Assert
		Assert.Equal(expectedName, normalizedName);
	}

	[Theory]
	[InlineData(null, 1, "State Name 1")]
	[InlineData("", 2, "State Name 2")]
	[InlineData("   ", 3, "State Name 3")]
	public void NormalizeStateName_UsesNumberedFallbackForBlankName(
		string? importedName,
		int stateNumber,
		string expectedName)
	{
		// Act
		var normalizedName = XLightsStateNameNormalizer.NormalizeStateName(importedName, stateNumber);

		// Assert
		Assert.Equal(expectedName, normalizedName);
	}

	[Theory]
	[InlineData("  Enabled  ", "s1", "Enabled")]
	[InlineData(null, "s1", "s1")]
	[InlineData("", "s2", "s2")]
	[InlineData("   ", "s3", "s3")]
	public void NormalizeStateItemName_TrimsNameOrUsesXmlTagFallback(
		string? importedName,
		string xmlTag,
		string expectedName)
	{
		// Act
		var normalizedName = XLightsStateNameNormalizer.NormalizeStateItemName(importedName, xmlTag);

		// Assert
		Assert.Equal(expectedName, normalizedName);
	}

	[Fact]
	public void NormalizeStateItemName_PreservesXmlTagFallbackWhenNormalizedRepeatedly()
	{
		// Arrange
		const string xmlTag = "s1";

		// Act
		var firstNormalization = XLightsStateNameNormalizer.NormalizeStateItemName(null, xmlTag);
		var secondNormalization = XLightsStateNameNormalizer.NormalizeStateItemName(firstNormalization, xmlTag);

		// Assert
		Assert.Equal(xmlTag, secondNormalization);
	}
}
