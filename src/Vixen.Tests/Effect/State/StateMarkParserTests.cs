using VixenModules.Effect.State;
using Xunit;

namespace Vixen.Tests.Effect.State;

public class StateMarkParserTests
{
	[Fact]
	public void ParseStateItemNames_TrimsSegmentsAndPreservesEmptySegments()
	{
		// Act
		var names = StateMarkParser.ParseStateItemNames(" Open,Closed , ,Blink,");

		// Assert
		Assert.Equal(["Open", "Closed", string.Empty, "Blink", string.Empty], names);
	}

	[Fact]
	public void ParseStateItemNames_NullTextProducesEmptySegment()
	{
		// Act
		var names = StateMarkParser.ParseStateItemNames(null);

		// Assert
		Assert.Equal([string.Empty], names);
	}
}
