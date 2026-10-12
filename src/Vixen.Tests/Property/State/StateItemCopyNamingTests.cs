using VixenModules.Property.State;
using Xunit;

namespace Vixen.Tests.Property.State;

/// <summary>
/// Verifies source-based State Item naming and existing Add naming behavior.
/// </summary>
public sealed class StateItemCopyNamingTests
{
	/// <summary>
	/// Verifies that an unused source-based name starts with suffix 1.
	/// </summary>
	/// <param name="includeSource"><see langword="true"/> to include the source name; otherwise, <see langword="false"/>.</param>
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void GetNextStateItemName_FreeFirstSuffix_ReturnsSuffixOne(bool includeSource)
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = includeSource ? ["Arm"] : [];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm", existingNames);

		// Assert
		Assert.Equal("Arm - 1", name);
	}

	/// <summary>
	/// Verifies that consecutive occupied suffixes are skipped.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_OccupiedSuffixes_ReturnsNextUnusedSuffix()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["Arm", "Arm - 1", "Arm - 2"];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm", existingNames);

		// Assert
		Assert.Equal("Arm - 3", name);
	}

	/// <summary>
	/// Verifies that the first gap is used regardless of existing-name order.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_SuffixGap_ReturnsFirstUnusedSuffix()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["Arm - 3", "Arm", "Arm - 1"];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm", existingNames);

		// Assert
		Assert.Equal("Arm - 2", name);
	}

	/// <summary>
	/// Verifies that case-only differences do not reserve a candidate name.
	/// </summary>
	/// <param name="existingName">The differently cased candidate already present.</param>
	[Theory]
	[InlineData("arm - 1")]
	[InlineData("ARM - 1")]
	public void GetNextStateItemName_CaseOnlyCollision_ReturnsSuffixOne(string existingName)
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["Arm", existingName];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm", existingNames);

		// Assert
		Assert.Equal("Arm - 1", name);
	}

	/// <summary>
	/// Verifies that an existing source suffix remains part of the complete base name.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_SuffixedSource_AppendsAnotherSuffix()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["Arm - 1", "Arm - 2"];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm - 1", existingNames);

		// Assert
		Assert.Equal("Arm - 1 - 1", name);
	}

	/// <summary>
	/// Verifies that surrounding whitespace is trimmed before collision matching.
	/// </summary>
	/// <param name="baseName">The source name with surrounding whitespace.</param>
	[Theory]
	[InlineData("  Arm  ")]
	[InlineData("\tArm\r\n")]
	public void GetNextStateItemName_PaddedBase_TrimsBeforeMatching(string baseName)
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["Arm - 1"];

		// Act
		var name = StateNamingRules.GetNextStateItemName(baseName, existingNames);

		// Assert
		Assert.Equal("Arm - 2", name);
	}

	/// <summary>
	/// Verifies that a blank source base falls back to default State Item naming.
	/// </summary>
	/// <param name="baseName">The empty or whitespace-only source name.</param>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("\t\r\n")]
	public void GetNextStateItemName_BlankBase_UsesDefaultAndSkipsOccupiedNames(string baseName)
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["State Item - 1"];

		// Act
		var name = StateNamingRules.GetNextStateItemName(baseName, existingNames);

		// Assert
		Assert.Equal("State Item - 2", name);
	}

	/// <summary>
	/// Verifies that a null source name is rejected with the correct parameter name.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_NullBase_ThrowsArgumentNullException()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = [];

		// Act
		var exception = Assert.Throws<ArgumentNullException>(() =>
			StateNamingRules.GetNextStateItemName(null!, existingNames));

		// Assert
		Assert.Equal("baseName", exception.ParamName);
	}

	/// <summary>
	/// Verifies that a null existing-name collection is rejected with the correct parameter name.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_NullExistingNames_ThrowsArgumentNullException()
	{
		// Arrange
		const string baseName = "Arm";

		// Act
		var exception = Assert.Throws<ArgumentNullException>(() =>
			StateNamingRules.GetNextStateItemName(baseName, null!));

		// Assert
		Assert.Equal("existingNames", exception.ParamName);
	}

	/// <summary>
	/// Verifies that generating a copy name leaves existing names unchanged.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_ExistingNames_DoesNotMutateCollection()
	{
		// Arrange
		List<string> existingNames = ["Arm", "Arm - 1"];

		// Act
		var name = StateNamingRules.GetNextStateItemName("Arm", existingNames);

		// Assert
		Assert.Equal("Arm - 2", name);
		Assert.Equal(["Arm", "Arm - 1"], existingNames);
	}

	/// <summary>
	/// Verifies that the original overload still starts with the default name and suffix 1.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_DefaultOverload_ReturnsOriginalDefaultName()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = [];

		// Act
		var name = StateNamingRules.GetNextStateItemName(existingNames);

		// Assert
		Assert.Equal("State Item - 1", name);
	}

	/// <summary>
	/// Verifies that the original overload still skips occupied default suffixes.
	/// </summary>
	[Fact]
	public void GetNextStateItemName_DefaultOverload_SkipsOccupiedDefaultNames()
	{
		// Arrange
		IReadOnlyCollection<string> existingNames = ["State Item - 1", "State Item - 3"];

		// Act
		var name = StateNamingRules.GetNextStateItemName(existingNames);

		// Assert
		Assert.Equal("State Item - 2", name);
	}
}
