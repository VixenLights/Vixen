using VixenModules.Effect.State;
using Xunit;

namespace Vixen.Tests.Effect.State;

public class StateDataTests
{
	[Fact]
	public void Iterations_DefaultsToOne()
	{
		// Arrange / Act
		var data = new StateData();

		// Assert
		Assert.Equal(1, data.Iterations);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(-1, 1)]
	[InlineData(1, 1)]
	[InlineData(20, 20)]
	[InlineData(21, 20)]
	public void Iterations_NormalizesRange(int value, int expected)
	{
		// Arrange
		var data = new StateData();

		// Act
		data.Iterations = value;

		// Assert
		Assert.Equal(expected, data.Iterations);
	}

	[Fact]
	public void Clone_CopiesIterations()
	{
		// Arrange
		var data = new StateData
		{
			Iterations = 7
		};

		// Act
		var clone = (StateData)data.Clone();

		// Assert
		Assert.Equal(7, clone.Iterations);
	}
}
