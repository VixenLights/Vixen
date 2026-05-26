// =============================================================================
// VIXEN TEST AUTHORING GUIDE
//
// Every test class follows the AAA (Arrange, Act, Assert) pattern:
//   Arrange  — set up objects and data the method under test needs.
//   Act      — call the method under test exactly once.
//   Assert   — verify one observable outcome.
//
// Naming convention: MethodName_Condition_ExpectedBehavior
//
// One assertion per test keeps failure messages pinpoint-accurate.
// Use Assert.Equal(expected, actual) — expected value comes first.
//
// For classes that need mocked dependencies, inject them with Moq:
//   var mock = new Mock<IMyService>();
//   mock.Setup(s => s.DoThing()).Returns("value");
//   var sut = new MyClass(mock.Object);
// =============================================================================

using Vixen.Utility;
using Xunit;

namespace Vixen.Tests.Utility;

public class NamingUtilitiesTests
{
	[Fact]
	public void Uniquify_NameNotInSet_ReturnsNameUnchanged()
	{
		// Arrange
		var names = new HashSet<string> { "alpha", "beta" };
		var name = "gamma";

		// Act
		var result = NamingUtilities.Uniquify(names, name);

		// Assert
		Assert.Equal("gamma", result);
	}

	[Fact]
	public void Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2()
	{
		// Arrange
		var names = new HashSet<string> { "foo" };
		var name = "foo";

		// Act
		var result = NamingUtilities.Uniquify(names, name);

		// Assert
		Assert.Equal("foo - 2", result);
	}

	[Fact]
	public void Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3()
	{
		// Arrange
		var names = new HashSet<string> { "foo", "foo - 2" };
		var name = "foo";

		// Act
		var result = NamingUtilities.Uniquify(names, name);

		// Assert
		Assert.Equal("foo - 3", result);
	}
}