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

	[Fact]
	public void CustomStateItems_DefaultsToEmptyList()
	{
		// Arrange / Act
		var data = new StateData();

		// Assert
		Assert.NotNull(data.CustomStateItems);
		Assert.Empty(data.CustomStateItems);
	}

	[Fact]
	public void Clone_DeepCopiesCustomStateItems()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var data = new StateData
		{
			CustomStateItems =
			[
				new CustomStateItemData
				{
					StateItemId = stateItemId,
					Color = System.Drawing.Color.Red
				}
			]
		};

		// Act
		var clone = (StateData)data.Clone();

		// Assert
		Assert.Single(clone.CustomStateItems);
		Assert.NotSame(data.CustomStateItems[0], clone.CustomStateItems[0]);
		Assert.Equal(stateItemId, clone.CustomStateItems[0].StateItemId);
		Assert.Equal(System.Drawing.Color.Red, clone.CustomStateItems[0].Color);
	}

	[Fact]
	public void Clone_NormalizesNullCustomStateItems()
	{
		// Arrange
		var data = new StateData
		{
			CustomStateItems = null!
		};

		// Act
		var clone = (StateData)data.Clone();

		// Assert
		Assert.NotNull(data.CustomStateItems);
		Assert.NotNull(clone.CustomStateItems);
		Assert.Empty(clone.CustomStateItems);
	}
}
