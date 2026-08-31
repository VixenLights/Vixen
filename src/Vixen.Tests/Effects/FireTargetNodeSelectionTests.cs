using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Moq;
using Vixen.Module.Effect;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Fire;
using VixenModules.Property.Location;
using Xunit;

namespace Vixen.Tests.Effects;

/// <summary>
/// Characterizes the Fire target-node selection contract and preserves its group-mode location rendering.
/// </summary>
public sealed class FireTargetNodeSelectionTests
{
	/// <summary>
	/// Verifies that a new Fire effect defaults to across-target handling at depth zero.
	/// </summary>
	[Fact]
	public void Fire_DefaultsToGroupTargetHandlingAndDepthZero()
	{
		// Arrange
		var effect = new Fire();

		// Act
		var targetNodeHandling = GetTargetNodeSelectionValue(effect, "TargetNodeHandling");
		var depthOfEffect = GetIntValue(effect, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeHandling);
		Assert.Equal(0, depthOfEffect);
	}

	/// <summary>
	/// Verifies that new Fire data defaults to across-target handling at depth zero.
	/// </summary>
	[Fact]
	public void FireData_DefaultsToGroupTargetSelectionAndDepthZero()
	{
		// Arrange
		var data = new FireData();

		// Act
		var targetNodeSelection = GetTargetNodeSelectionValue(data, "TargetNodeSelection");
		var depthOfEffect = GetIntValue(data, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeSelection);
		Assert.Equal(0, depthOfEffect);
	}

	/// <summary>
	/// Verifies that Fire persists the target-handling settings.
	/// </summary>
	[Fact]
	public void FireData_TargetSelectionFieldsAreSerialized()
	{
		// Arrange
		var dataType = typeof(FireData);

		// Act
		var targetNodeSelection = GetRequiredProperty(dataType, "TargetNodeSelection");
		var depthOfEffect = GetRequiredProperty(dataType, "DepthOfEffect");

		// Assert
		Assert.Contains(targetNodeSelection.GetCustomAttributes(), attribute => attribute is DataMemberAttribute);
		Assert.Contains(depthOfEffect.GetCustomAttributes(), attribute => attribute is DataMemberAttribute);
	}

	/// <summary>
	/// Verifies that data saved before target-handling settings existed retains compatible defaults.
	/// </summary>
	[Fact]
	public void FireData_LegacyPayloadDefaultsToGroupTargetSelectionAndDepthZero()
	{
		// Arrange
		const string legacyJson = @"{""Location"":0}";

		// Act
		var data = DeserializeJson(legacyJson);
		var targetNodeSelection = GetTargetNodeSelectionValue(data, "TargetNodeSelection");
		var depthOfEffect = GetIntValue(data, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeSelection);
		Assert.Equal(0, depthOfEffect);
	}

	/// <summary>
	/// Verifies that deserialization normalizes invalid target settings.
	/// </summary>
	[Fact]
	public void FireData_OnDeserializedNormalizesInvalidTargetSettings()
	{
		// Arrange
		var data = new FireData
		{
			DepthOfEffect = -1,
			TargetNodeSelection = (TargetNodeSelection)99
		};

		// Act
		data.OnDeserialized(default);

		// Assert
		Assert.Equal(0, data.DepthOfEffect);
		Assert.Equal(TargetNodeSelection.Group, data.TargetNodeSelection);
	}

	/// <summary>
	/// Verifies that a deep target exposes handling selection while group mode hides its depth picker.
	/// </summary>
	[Fact]
	public void FireProperties_DeepSingleTargetShowsTargetHandlingButHidesDepthInGroupMode()
	{
		// Arrange
		var effect = new Fire();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(3)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Group);

		// Act
		var properties = TypeDescriptor.GetProperties(effect);
		var targetNodeHandling = properties["TargetNodeHandling"];
		var depthOfEffect = properties["DepthOfEffect"];

		// Assert
		Assert.NotNull(targetNodeHandling);
		Assert.True(targetNodeHandling.IsBrowsable);
		Assert.NotNull(depthOfEffect);
		Assert.False(depthOfEffect.IsBrowsable);
	}

	/// <summary>
	/// Verifies that a deep individual target exposes the useful target-depth picker.
	/// </summary>
	[Fact]
	public void FireProperties_DeepSingleTargetInIndividualModeShowsDepth()
	{
		// Arrange
		var effect = new Fire();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(3)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);

		// Act
		var depthOfEffect = TypeDescriptor.GetProperties(effect)["DepthOfEffect"];

		// Assert
		Assert.NotNull(depthOfEffect);
		Assert.True(depthOfEffect.IsBrowsable);
	}

	/// <summary>
	/// Verifies that Fire depth choices exclude leaf-equivalent values.
	/// </summary>
	[Fact]
	public void FireDepthConverter_ExcludesZeroAndMaximumDepth()
	{
		// Arrange
		var effect = new Fire();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		var context = new Mock<ITypeDescriptorContext>();
		context.SetupGet(typeDescriptorContext => typeDescriptorContext.Instance).Returns(effect);
		var converter = new FireTargetElementDepthConverter();

		// Act
		var values = converter.GetStandardValues(context.Object).Cast<string>().ToArray();

		// Assert
		Assert.Equal(["1", "2"], values);
	}

	/// <summary>
	/// Verifies that an invalid individual target depth resets to the first useful depth.
	/// </summary>
	[Fact]
	public void FireProperties_IndividualModeResetsMaximumDepthToFirstUsefulDepth()
	{
		// Arrange
		var effect = new Fire();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);

		// Act
		SetPropertyValue(effect, "DepthOfEffect", 3);
		var depthOfEffect = GetIntValue(effect, "DepthOfEffect");

		// Assert
		Assert.Equal(1, depthOfEffect);
	}

	/// <summary>
	/// Verifies that changing depth does not rebuild the property descriptor while a selector binding is updating.
	/// </summary>
	[Fact]
	public void FireProperties_ChangingDepthDoesNotRefreshItsPropertyDescriptor()
	{
		// Arrange
		var effect = new Fire();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		var refreshCount = 0;
		RefreshEventHandler refreshed = eventArgs =>
		{
			if (ReferenceEquals(eventArgs.ComponentChanged, effect))
			{
				refreshCount++;
			}
		};
		TypeDescriptor.Refreshed += refreshed;

		try
		{
			// Act
			effect.DepthOfEffect = 1;
		}
		finally
		{
			TypeDescriptor.Refreshed -= refreshed;
		}

		// Assert
		Assert.Equal(0, refreshCount);
	}

	/// <summary>
	/// Verifies that a stale depth selection which normalizes to the existing depth does not notify bindings.
	/// </summary>
	[Fact]
	public void FireProperties_NormalizedStaleDepthDoesNotNotifyBindings()
	{
		// Arrange
		var effect = new Fire();
		var depthChangedCount = 0;
		PropertyChangedEventHandler propertyChanged = (_, eventArgs) =>
		{
			if (eventArgs.PropertyName == nameof(Fire.DepthOfEffect))
			{
				depthChangedCount++;
			}
		};
		effect.PropertyChanged += propertyChanged;

		try
		{
			// Act
			effect.DepthOfEffect = 1;
		}
		finally
		{
			effect.PropertyChanged -= propertyChanged;
		}

		// Assert
		Assert.Equal(0, effect.DepthOfEffect);
		Assert.Equal(0, depthChangedCount);
	}

	/// <summary>
	/// Verifies that reapplying the existing target handling does not refresh the property descriptor.
	/// </summary>
	[Fact]
	public void FireProperties_ReapplyingTargetHandlingDoesNotRefreshItsPropertyDescriptor()
	{
		// Arrange
		var effect = new Fire();
		var refreshCount = 0;
		RefreshEventHandler refreshed = eventArgs =>
		{
			if (ReferenceEquals(eventArgs.ComponentChanged, effect))
			{
				refreshCount++;
			}
		};
		TypeDescriptor.Refreshed += refreshed;

		try
		{
			// Act
			effect.TargetNodeHandling = TargetNodeSelection.Group;
		}
		finally
		{
			TypeDescriptor.Refreshed -= refreshed;
		}

		// Assert
		Assert.Equal(0, refreshCount);
	}

	/// <summary>
	/// Verifies that Fire's current default location mode renders all leaves under one selected group.
	/// </summary>
	[Fact]
	public void FireRender_DefaultGroupModeRendersLocatedLeavesTogether()
	{
		// Arrange
		var firstLeaf = CreateLocatedLeaf("Leaf 1", 1, 1);
		var secondLeaf = CreateLocatedLeaf("Leaf 2", 3, 1);
		var effect = new Fire
		{
			TargetPositioning = TargetPositioningType.Locations,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetTargetNodesWithoutPropertyValidation(effect, [CreateGroupNode("Parent", firstLeaf, secondLeaf)]);

		// Act
		var preRenderSucceeded = effect.PreRender();
		var intents = effect.Render();

		// Assert
		Assert.True(preRenderSucceeded);
		Assert.Equal(
			new[] { firstLeaf.Element.Id, secondLeaf.Element.Id }.OrderBy(id => id),
			intents.ElementIds.OrderBy(id => id));
	}

	/// <summary>
	/// Verifies that individual depth groups use separate local location buffers.
	/// </summary>
	[Fact]
	public void FireRender_IndividualDepthGroupsUseLocalLocationBuffers()
	{
		// Arrange
		var firstGroup = CreateGroupNode("Group 1", CreateLocatedLeaf("Group 1 Leaf 1", 1, 1), CreateLocatedLeaf("Group 1 Leaf 2", 3, 1));
		var secondGroup = CreateGroupNode("Group 2", CreateLocatedLeaf("Group 2 Leaf 1", 101, 1), CreateLocatedLeaf("Group 2 Leaf 2", 103, 1));
		var effect = new RenderTrackingFire
		{
			TargetPositioning = TargetPositioningType.Locations,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetTargetNodesWithoutPropertyValidation(effect, [CreateGroupNode("Root", [firstGroup, secondGroup], 3)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);
		SetPropertyValue(effect, "DepthOfEffect", 1);

		// Act
		var preRenderSucceeded = effect.PreRender();

		// Assert
		Assert.True(preRenderSucceeded);
		Assert.Equal([(3, 1), (3, 1)], effect.RenderDimensions);
	}

	/// <summary>
	/// Verifies that individually selected target roots are not combined into one location buffer.
	/// </summary>
	[Fact]
	public void FireRender_IndividualMultipleTargetsUseSeparateLocationBuffers()
	{
		// Arrange
		var firstTarget = CreateGroupNode("Target 1", CreateLocatedLeaf("Target 1 Leaf 1", 1, 1), CreateLocatedLeaf("Target 1 Leaf 2", 3, 1));
		var secondTarget = CreateGroupNode("Target 2", CreateLocatedLeaf("Target 2 Leaf 1", 101, 1), CreateLocatedLeaf("Target 2 Leaf 2", 103, 1));
		var effect = new RenderTrackingFire
		{
			TargetPositioning = TargetPositioningType.Locations,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetTargetNodesWithoutPropertyValidation(effect, [firstTarget, secondTarget]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);

		// Act
		var preRenderSucceeded = effect.PreRender();

		// Assert
		Assert.True(preRenderSucceeded);
		Assert.Equal([(3, 1), (3, 1)], effect.RenderDimensions);
		Assert.Equal(0, effect.DepthOfEffect);
	}

	private static FireData DeserializeJson(string json)
	{
		var serializer = new DataContractJsonSerializer(typeof(FireData));
		using var readStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

		return (FireData)serializer.ReadObject(readStream)!;
	}

	private static TargetNodeSelection GetTargetNodeSelectionValue(object instance, string propertyName)
	{
		var property = GetRequiredProperty(instance.GetType(), propertyName);
		var value = property.GetValue(instance);

		Assert.IsType<TargetNodeSelection>(value);
		return (TargetNodeSelection)value!;
	}

	private static int GetIntValue(object instance, string propertyName)
	{
		var property = GetRequiredProperty(instance.GetType(), propertyName);
		var value = property.GetValue(instance);

		Assert.IsType<int>(value);
		return (int)value!;
	}

	private static void SetPropertyValue(object instance, string propertyName, object value)
	{
		GetRequiredProperty(instance.GetType(), propertyName).SetValue(instance, value);
	}

	private static PropertyInfo GetRequiredProperty(Type type, string propertyName)
	{
		var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

		Assert.NotNull(property);
		return property;
	}

	private static IElementNode CreateTargetNode(int maxChildDepth)
	{
		var targetNode = new Mock<IElementNode>();
		targetNode.SetupGet(node => node.Children).Returns([]);
		targetNode.SetupGet(node => node.Properties).Returns(new PropertyManager(targetNode.Object));
		targetNode.Setup(node => node.GetMaxChildDepth()).Returns(maxChildDepth);

		return targetNode.Object;
	}

	private static IElementNode CreateGroupNode(string name, params IElementNode[] children)
	{
		return CreateGroupNode(name, children, children.Any() ? children.Max(child => child.GetMaxChildDepth()) + 1 : 0);
	}

	private static IElementNode CreateGroupNode(string name, IElementNode[] children, int maxChildDepth)
	{
		var targetNode = new Mock<IElementNode>();
		targetNode.SetupGet(node => node.Name).Returns(name);
		targetNode.SetupGet(node => node.Children).Returns(children);
		targetNode.SetupGet(node => node.Properties).Returns(new PropertyManager(targetNode.Object));
		targetNode.Setup(node => node.GetLeafEnumerator()).Returns(children.SelectMany(child => child.GetLeafEnumerator()));
		targetNode.Setup(node => node.GetMaxChildDepth()).Returns(maxChildDepth);

		return targetNode.Object;
	}

	private static IElementNode CreateLocatedLeaf(string name, int x, int y)
	{
		var leafNode = new Mock<IElementNode>();
		var properties = new PropertyManager(leafNode.Object);
		var locationModule = new LocationModule
		{
			Descriptor = new LocationDescriptor(),
			ModuleData = new LocationData
			{
				X = x,
				Y = y,
				Z = 0
			}
		};
		AddPropertyWithoutModuleStore(properties, locationModule);

		leafNode.SetupGet(node => node.Element).Returns(CreateElement(name));
		leafNode.SetupGet(node => node.Id).Returns(Guid.NewGuid());
		leafNode.SetupGet(node => node.Name).Returns(name);
		leafNode.SetupGet(node => node.Children).Returns([]);
		leafNode.SetupGet(node => node.IsLeaf).Returns(true);
		leafNode.SetupGet(node => node.Properties).Returns(properties);
		leafNode.Setup(node => node.GetLeafEnumerator()).Returns([leafNode.Object]);
		leafNode.Setup(node => node.GetMaxChildDepth()).Returns(0);

		return leafNode.Object;
	}

	private static Element CreateElement(string name)
	{
		var constructor = typeof(Element).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(string)],
			null);

		Assert.NotNull(constructor);
		return (Element)constructor.Invoke([name]);
	}

	private static void AddPropertyWithoutModuleStore(PropertyManager properties, IPropertyModuleInstance property)
	{
		var itemsField = typeof(PropertyManager).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(itemsField);
		var items = (Dictionary<Guid, IPropertyModuleInstance>)itemsField.GetValue(properties)!;
		items[property.TypeId] = property;
	}

	private static void SetTargetNodesWithoutPropertyValidation(Fire effect, IElementNode[] targetNodes)
	{
		var targetNodesField = typeof(EffectModuleInstanceBase).GetField("_targetNodes", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(targetNodesField);
		targetNodesField.SetValue(effect, targetNodes);
	}

	private sealed class RenderTrackingFire : Fire
	{
		public List<(int Width, int Height)> RenderDimensions { get; } = [];

		protected override void SetupRender()
		{
			RenderDimensions.Add((BufferWi, BufferHt));
			base.SetupRender();
		}
	}
}
