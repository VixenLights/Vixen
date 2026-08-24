using Vixen.Utility;
using Xunit;

namespace Vixen.Tests.Utility;

public sealed class HighResolutionTimerTests
{
	[Fact]
	public void Stop_WhenTimerHasNotStarted_DoesNotThrow()
	{
		// Arrange
		var timer = new HighResolutionTimer();

		// Act
		var exception = Record.Exception(() => timer.Stop());

		// Assert
		Assert.Null(exception);
		Assert.False(timer.IsRunning);
	}
}
