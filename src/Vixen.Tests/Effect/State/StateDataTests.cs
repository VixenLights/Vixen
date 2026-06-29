using System.ComponentModel;
using System.Drawing;
using VixenModules.Effect.State;
using Xunit;
using StateEffect = VixenModules.Effect.State.State;

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

	[Fact]
	public void CustomStateItemCollection_AllowsEmptyCollection()
	{
		// Arrange / Act
		var collection = new CustomStateItemCollection();

		// Assert
		Assert.Equal(0, collection.GetMinimumItemCount());
	}

	[Fact]
	public void CustomStateItem_CreateData_CopiesStateItemIdAndColor()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var item = new CustomStateItem
		{
			StateItemId = stateItemId,
			Color = Color.Blue
		};

		// Act
		var data = item.CreateData();

		// Assert
		Assert.Equal(stateItemId, data.StateItemId);
		Assert.Equal(Color.Blue, data.Color);
	}

	[Fact]
	public void ModuleData_LoadsCustomStateItemsIntoEditableCollection()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var effect = new StateEffect();
		var data = new StateData
		{
			CustomStateItems =
			[
				new CustomStateItemData
				{
					StateItemId = stateItemId,
					Color = Color.Green
				}
			]
		};

		// Act
		effect.ModuleData = data;

		// Assert
		var item = Assert.Single(effect.CustomStateItems);
		Assert.Equal(stateItemId, item.StateItemId);
		Assert.Equal(Color.Green, item.Color);
		Assert.Same(effect, item.Parent);
	}

	[Fact]
	public void CustomStateItemCollectionChanges_UpdateModuleData()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var effect = new StateEffect();
		var item = new CustomStateItem
		{
			StateItemId = stateItemId,
			Color = Color.Red
		};

		// Act
		effect.CustomStateItems.Add(item);
		item.Color = Color.Yellow;

		// Assert
		var data = Assert.IsType<StateData>(effect.ModuleData);
		var itemData = Assert.Single(data.CustomStateItems);
		Assert.Equal(stateItemId, itemData.StateItemId);
		Assert.Equal(Color.Yellow, itemData.Color);
		Assert.Same(effect, item.Parent);
	}

	[Fact]
	public void StateRenderSource_Custom_HasDescription()
	{
		// Arrange
		var memberInfo = typeof(StateRenderSource).GetMember(nameof(StateRenderSource.Custom));

		// Act
		var description = memberInfo[0]
			.GetCustomAttributes(typeof(DescriptionAttribute), false)
			.Cast<DescriptionAttribute>()
			.Single();

		// Assert
		Assert.Equal("Custom", description.Description);
	}

	[Fact]
	public void RenderSource_DefaultBrowsability_ShowsOnlyStateItemSelector()
	{
		// Arrange
		var effect = new StateEffect();

		// Assert
		Assert.True(IsBrowsable(effect, nameof(StateEffect.StateItem)));
		Assert.False(IsBrowsable(effect, nameof(StateEffect.MarkCollectionId)));
		Assert.False(IsBrowsable(effect, nameof(StateEffect.CustomStateItems)));
	}

	[Fact]
	public void RenderSource_MarkCollectionBrowsability_ShowsOnlyMarkCollectionSelector()
	{
		// Arrange
		var effect = new StateEffect();

		// Act
		effect.RenderSource = StateRenderSource.MarkCollection;

		// Assert
		Assert.False(IsBrowsable(effect, nameof(StateEffect.StateItem)));
		Assert.True(IsBrowsable(effect, nameof(StateEffect.MarkCollectionId)));
		Assert.False(IsBrowsable(effect, nameof(StateEffect.CustomStateItems)));
	}

	[Fact]
	public void RenderSource_CustomBrowsability_ShowsOnlyCustomStateItems()
	{
		// Arrange
		var effect = new StateEffect
		{
			PlaybackMode = PlaybackMode.Iterate
		};

		// Act
		effect.RenderSource = StateRenderSource.Custom;

		// Assert
		Assert.False(IsBrowsable(effect, nameof(StateEffect.StateItem)));
		Assert.False(IsBrowsable(effect, nameof(StateEffect.MarkCollectionId)));
		Assert.True(IsBrowsable(effect, nameof(StateEffect.CustomStateItems)));
		Assert.Equal(PlaybackMode.Iterate, effect.PlaybackMode);
	}

	[Fact]
	public void CustomStateItems_UsesResourceBackedDisplayMetadata()
	{
		// Arrange
		var effect = new StateEffect
		{
			RenderSource = StateRenderSource.Custom
		};

		// Act
		var property = TypeDescriptor.GetProperties(effect)[nameof(StateEffect.CustomStateItems)];

		// Assert
		Assert.NotNull(property);
		Assert.Equal("Custom State Items", property.DisplayName);
		Assert.Equal("Defines the custom State item rows and color overrides to render.", property.Description);
	}

	private static bool IsBrowsable(StateEffect effect, string propertyName)
	{
		var property = TypeDescriptor.GetProperties(effect)[propertyName];
		Assert.NotNull(property);
		return property.IsBrowsable;
	}
}
