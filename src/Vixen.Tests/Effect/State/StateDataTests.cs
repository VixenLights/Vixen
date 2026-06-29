using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Effect.State;
using VixenModules.Property.State;
using Xunit;
using ColorData = VixenModules.Property.Color.ColorData;
using ColorDescriptor = VixenModules.Property.Color.ColorDescriptor;
using ColorModule = VixenModules.Property.Color.ColorModule;
using ElementColorType = VixenModules.Property.Color.ElementColorType;
using StateEffect = VixenModules.Effect.State.State;
using StateEffectData = VixenModules.Effect.State.StateData;
using StatePropertyDefinitionData = VixenModules.Property.State.StateDefinitionData;
using StatePropertyDescriptor = VixenModules.Property.State.StateDescriptor;
using StatePropertyItemData = VixenModules.Property.State.StateItemData;

namespace Vixen.Tests.Effect.State;

public class StateDataTests
{
	[Fact]
	public void Iterations_DefaultsToOne()
	{
		// Arrange / Act
		var data = new StateEffectData();

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
		var data = new StateEffectData();

		// Act
		data.Iterations = value;

		// Assert
		Assert.Equal(expected, data.Iterations);
	}

	[Fact]
	public void Clone_CopiesIterations()
	{
		// Arrange
		var data = new StateEffectData
		{
			Iterations = 7
		};

		// Act
		var clone = (StateEffectData)data.Clone();

		// Assert
		Assert.Equal(7, clone.Iterations);
	}

	[Fact]
	public void CustomStateItems_DefaultsToEmptyList()
	{
		// Arrange / Act
		var data = new StateEffectData();

		// Assert
		Assert.NotNull(data.CustomStateItems);
		Assert.Empty(data.CustomStateItems);
	}

	[Fact]
	public void Clone_DeepCopiesCustomStateItems()
	{
		// Arrange
		var stateItemId = Guid.NewGuid();
		var data = new StateEffectData
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
		var clone = (StateEffectData)data.Clone();

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
		var data = new StateEffectData
		{
			CustomStateItems = null!
		};

		// Act
		var clone = (StateEffectData)data.Clone();

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
		var data = new StateEffectData
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
		var data = Assert.IsType<StateEffectData>(effect.ModuleData);
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

	[Fact]
	public void VisualRepresentationText_StateItem_UsesStateDefinition()
	{
		// Arrange
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", CreateStateItem("Open", Color.Green)));

		// Act
		var text = effect.GetVisualRepresentationText();

		// Assert
		Assert.Equal("State - Door", text);
	}

	[Fact]
	public void VisualRepresentationText_Custom_AddsCustomHint()
	{
		// Arrange
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", CreateStateItem("Open", Color.Green)));
		effect.RenderSource = StateRenderSource.Custom;

		// Act
		var text = effect.GetVisualRepresentationText();

		// Assert
		Assert.Equal("State - Door - Custom", text);
	}

	[Fact]
	public void CustomStateItemOptions_Default_ExcludeAlreadySelectedRows()
	{
		// Arrange
		var open = CreateStateItem("Open", Color.Green);
		var closed = CreateStateItem("Closed", Color.Red);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", open, closed));
		var selectedRow = new CustomStateItem
		{
			StateItemId = open.Id
		};
		var newRow = new CustomStateItem();
		effect.CustomStateItems.Add(selectedRow);

		// Act
		var selectedOptions = effect.GetCustomStateItemOptions(selectedRow);
		var newOptions = effect.GetCustomStateItemOptions(newRow);

		// Assert
		Assert.Equal(["Open", "Closed"], selectedOptions);
		Assert.Equal(["Closed"], newOptions);
		Assert.DoesNotContain("<None>", newOptions);
	}

	[Fact]
	public void CustomStateItemOptions_Iterate_IncludeNoneAndSelectedRows()
	{
		// Arrange
		var open = CreateStateItem("Open", Color.Green);
		var closed = CreateStateItem("Closed", Color.Red);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", open, closed));
		effect.PlaybackMode = PlaybackMode.Iterate;
		effect.CustomStateItems.Add(new CustomStateItem
		{
			StateItemId = open.Id
		});
		var newRow = new CustomStateItem();

		// Act
		var options = effect.GetCustomStateItemOptions(newRow);

		// Assert
		Assert.Equal(["<None>", "Open", "Closed"], options);
	}

	[Fact]
	public void CustomStateItemLabel_IterateNone_ReturnsNone()
	{
		// Arrange
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", CreateStateItem("Open", Color.Green)));
		effect.PlaybackMode = PlaybackMode.Iterate;
		var row = new CustomStateItem
		{
			Parent = effect,
			StateItemId = Guid.Empty
		};

		// Act / Assert
		Assert.Equal("<None>", row.StateItem);
	}

	[Fact]
	public void PlaybackMode_Default_RemovesNoneAndDuplicateCustomRows()
	{
		// Arrange
		var open = CreateStateItem("Open", Color.Green);
		var closed = CreateStateItem("Closed", Color.Red);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", open, closed));
		effect.PlaybackMode = PlaybackMode.Iterate;
		effect.CustomStateItems.Add(new CustomStateItem { StateItemId = open.Id, Color = Color.Blue });
		effect.CustomStateItems.Add(new CustomStateItem { StateItemId = Guid.Empty, Color = Color.Yellow });
		effect.CustomStateItems.Add(new CustomStateItem { StateItemId = closed.Id, Color = Color.Red });
		effect.CustomStateItems.Add(new CustomStateItem { StateItemId = open.Id, Color = Color.Purple });

		// Act
		effect.PlaybackMode = PlaybackMode.Default;

		// Assert
		Assert.Equal([open.Id, closed.Id], effect.CustomStateItems.Select(item => item.StateItemId));
		Assert.Equal([Color.Blue, Color.Red], effect.CustomStateItems.Select(item => item.Color));
		var data = Assert.IsType<StateEffectData>(effect.ModuleData);
		Assert.Equal([open.Id, closed.Id], data.CustomStateItems.Select(item => item.StateItemId));
	}

	[Fact]
	public void StateDefinitionChange_ClearsCustomStateItems()
	{
		// Arrange
		var first = CreateDefinition("First", CreateStateItem("Open", Color.Green));
		var second = CreateDefinition("Second", CreateStateItem("Closed", Color.Red));
		var effect = CreateEffectWithDefinitions([first, second]);
		effect.CustomStateItems.Add(new CustomStateItem
		{
			StateItemId = first.Items[0].Id
		});

		// Act
		effect.StateDefinition = "Second";

		// Assert
		Assert.Empty(effect.CustomStateItems);
		var data = Assert.IsType<StateEffectData>(effect.ModuleData);
		Assert.Empty(data.CustomStateItems);
	}

	[Fact]
	public void SelectCustomStateItem_ResetsColorToSelectedStateItemColor()
	{
		// Arrange
		var open = CreateStateItem("Open", Color.Green);
		var closed = CreateStateItem("Closed", Color.Red);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", open, closed));
		var row = new CustomStateItem
		{
			Parent = effect,
			StateItemId = open.Id,
			Color = Color.Blue
		};

		// Act
		row.StateItem = "Closed";

		// Assert
		Assert.Equal(closed.Id, row.StateItemId);
		Assert.Equal(Color.Red, row.Color);
	}

	[Fact]
	public void CustomStateItemOptions_DuplicateNames_UseAssignmentSummary()
	{
		// Arrange
		var firstElementId = Guid.NewGuid();
		var secondElementId = Guid.NewGuid();
		var first = CreateStateItem("Open", Color.Green, firstElementId);
		var second = CreateStateItem("Open", Color.Red, secondElementId);
		var effect = CreateEffectWithDefinition(
			CreateDefinition("Door", first, second),
			CreateNode(firstElementId, "First"),
			CreateNode(secondElementId, "Second"));

		// Act
		var options = effect.GetCustomStateItemOptions(new CustomStateItem());

		// Assert
		Assert.Equal(["Open (First)", "Open (Second)"], options);
	}

	[Fact]
	public void CustomStateItemOptions_DuplicateNamesAndAssignments_UseOrdinal()
	{
		// Arrange
		var first = CreateStateItem("Open", Color.Green);
		var second = CreateStateItem("Open", Color.Red);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", first, second));

		// Act
		var options = effect.GetCustomStateItemOptions(new CustomStateItem());

		// Assert
		Assert.Equal(["Open (No assignments, 1)", "Open (No assignments, 2)"], options);
	}

	[Fact]
	public void CustomStateItemDiscreteColors_UseSelectedStateItemAssignments()
	{
		// Arrange
		var discreteElementId = Guid.NewGuid();
		var unassignedElementId = Guid.NewGuid();
		var item = CreateStateItem("Open", Color.Green, discreteElementId);
		var discreteNode = CreateNode(discreteElementId, "Discrete");
		var unassignedNode = CreateNode(unassignedElementId, "Unassigned");
		AddSingleColorModule(discreteNode, Color.Red);
		AddSingleColorModule(unassignedNode, Color.Blue);
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", item), discreteNode, unassignedNode);
		var row = new CustomStateItem
		{
			Parent = effect,
			StateItemId = item.Id
		};

		// Act
		var colors = row.GetDiscreteColors();

		// Assert
		Assert.Equal([Color.Red], colors);
	}

	[Fact]
	public void CustomStateItemDiscreteColors_FullColorAssignment_ReturnsEmpty()
	{
		// Arrange
		var fullColorElementId = Guid.NewGuid();
		var item = CreateStateItem("Open", Color.Green, fullColorElementId);
		var fullColorNode = CreateNode(fullColorElementId, "Full Color");
		var effect = CreateEffectWithDefinition(CreateDefinition("Door", item), fullColorNode);
		var row = new CustomStateItem
		{
			Parent = effect,
			StateItemId = item.Id
		};

		// Act
		var colors = row.GetDiscreteColors();

		// Assert
		Assert.Empty(colors);
	}

	private static bool IsBrowsable(StateEffect effect, string propertyName)
	{
		var property = TypeDescriptor.GetProperties(effect)[propertyName];
		Assert.NotNull(property);
		return property.IsBrowsable;
	}

	private static StateEffect CreateEffectWithDefinition(StatePropertyDefinitionData definition, params IElementNode[] children)
	{
		return CreateEffectWithDefinitions([definition], children);
	}

	private static StateEffect CreateEffectWithDefinitions(
		IReadOnlyList<StatePropertyDefinitionData> definitions,
		params IElementNode[] children)
	{
		var rootNode = CreateNode(Guid.NewGuid(), "Root", children);
		AddStateModule(rootNode, definitions);
		var effect = new StateEffect
		{
			TargetNodes = [rootNode],
			ModuleData = new StateEffectData
			{
				SelectedStateDefinitionId = definitions[0].Id
			}
		};

		return effect;
	}

	private static StatePropertyDefinitionData CreateDefinition(string name, params StatePropertyItemData[] items)
	{
		return new StatePropertyDefinitionData
		{
			Name = name,
			Items = items.ToList()
		};
	}

	private static StatePropertyItemData CreateStateItem(string name, Color color, params Guid[] elementNodeIds)
	{
		return new StatePropertyItemData
		{
			Name = name,
			Color = color,
			ElementNodeIds = elementNodeIds.ToList()
		};
	}

	private static IElementNode CreateNode(Guid id, string name, params IElementNode[] children)
	{
		var node = new Mock<IElementNode>();
		var propertyManager = new PropertyManager(node.Object);
		node.SetupGet(elementNode => elementNode.Id).Returns(id);
		node.SetupGet(elementNode => elementNode.Name).Returns(name);
		node.SetupGet(elementNode => elementNode.Children).Returns(children);
		node.SetupGet(elementNode => elementNode.Properties).Returns(propertyManager);
		return node.Object;
	}

	private static void AddStateModule(IElementNode node, IReadOnlyList<StatePropertyDefinitionData> definitions)
	{
		var stateModule = new StateModule
		{
			StateDefinitions = definitions.ToList()
		};
		var propertyItemsField = typeof(PropertyManager)
			.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(propertyItemsField);

		var propertyItems = Assert.IsType<Dictionary<Guid, IPropertyModuleInstance>>(
			propertyItemsField.GetValue(node.Properties));
		propertyItems[StatePropertyDescriptor.ModuleId] = stateModule;
	}

	private static void AddSingleColorModule(IElementNode node, Color color)
	{
		var colorModule = new ColorModule
		{
			ModuleData = new ColorData
			{
				ElementColorType = ElementColorType.SingleColor,
				SingleColor = color
			}
		};
		var propertyItemsField = typeof(PropertyManager)
			.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(propertyItemsField);

		var propertyItems = Assert.IsType<Dictionary<Guid, IPropertyModuleInstance>>(
			propertyItemsField.GetValue(node.Properties));
		propertyItems[ColorDescriptor.ModuleId] = colorModule;
	}
}
