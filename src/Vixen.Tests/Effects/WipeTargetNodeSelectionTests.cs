using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Moq;
using Vixen.Module.Effect;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Wipe;
using VixenModules.Property.Location;
using Xunit;
using ZedGraph;

namespace Vixen.Tests.Effects;

public sealed class WipeTargetNodeSelectionTests
{
	[Fact]
	public void WipeModule_DefaultsToGroupTargetHandlingAndDepthZero()
	{
		// Arrange
		var effect = new WipeModule();

		// Act
		var targetNodeHandling = GetTargetNodeSelectionValue(effect, "TargetNodeHandling");
		var depthOfEffect = GetIntValue(effect, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeHandling);
		Assert.Equal(0, depthOfEffect);
	}

	[Fact]
	public void WipeData_DefaultsToGroupTargetSelectionAndDepthZero()
	{
		// Arrange
		var data = new WipeData();

		// Act
		var targetNodeSelection = GetTargetNodeSelectionValue(data, "TargetNodeSelection");
		var depthOfEffect = GetIntValue(data, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeSelection);
		Assert.Equal(0, depthOfEffect);
	}

	[Fact]
	public void WipeData_TargetSelectionFieldsAreSerialized()
	{
		// Arrange
		var dataType = typeof(WipeData);

		// Act
		var targetNodeSelection = GetRequiredProperty(dataType, "TargetNodeSelection");
		var depthOfEffect = GetRequiredProperty(dataType, "DepthOfEffect");

		// Assert
		Assert.Contains(targetNodeSelection.GetCustomAttributes(), attribute => attribute is DataMemberAttribute);
		Assert.Contains(depthOfEffect.GetCustomAttributes(), attribute => attribute is DataMemberAttribute);
	}

	[Fact]
	public void WipeData_LegacyPayloadDefaultsToGroupTargetSelectionAndDepthZero()
	{
		// Arrange
		const string legacyJson = @"{""PulseTime"":1000,""PassCount"":1,""PulsePercent"":33}";

		// Act
		var data = DeserializeJson(legacyJson);
		var targetNodeSelection = GetTargetNodeSelectionValue(data, "TargetNodeSelection");
		var depthOfEffect = GetIntValue(data, "DepthOfEffect");

		// Assert
		Assert.Equal(TargetNodeSelection.Group, targetNodeSelection);
		Assert.Equal(0, depthOfEffect);
	}

	/// <summary>
	/// Verifies that copied or deserialized settings survive assignment before destination targets are attached.
	/// </summary>
	/// <param name="useSerializedData">`<see langword="true" />` to use clipboard-style serialized data; otherwise, use a raw clone.</param>
	/// <param name="targetNodeSelection">One of the enumeration values that specifies the copied target handling mode.</param>
	[Theory]
	[InlineData(false, TargetNodeSelection.Group)]
	[InlineData(false, TargetNodeSelection.Individual)]
	[InlineData(true, TargetNodeSelection.Group)]
	[InlineData(true, TargetNodeSelection.Individual)]
	public void WipeModule_ModuleDataAssignedBeforeTargets_PreservesSettingsUntilDestinationValidation(bool useSerializedData, TargetNodeSelection targetNodeSelection)
	{
		// Arrange
		var sourceData = CreateCompleteWipeData(targetNodeSelection);
		var assignedData = useSerializedData ? RoundTrip(sourceData) : (WipeData)sourceData.Clone();
		var effect = new WipeModule();

		// Act
		effect.ModuleData = assignedData;

		// Assert
		Assert.Equal(targetNodeSelection, GetTargetNodeSelectionValue(effect, "TargetNodeHandling"));
		Assert.Equal(2, GetIntValue(effect, "DepthOfEffect"));
		AssertTargetingControlsHidden(effect);

		// Act
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);

		// Assert
		Assert.Equal(targetNodeSelection, GetTargetNodeSelectionValue(effect, "TargetNodeHandling"));
		Assert.Equal(targetNodeSelection == TargetNodeSelection.Individual ? 2 : 0, GetIntValue(effect, "DepthOfEffect"));
		Assert.Equal(targetNodeSelection, sourceData.TargetNodeSelection);
		Assert.Equal(2, sourceData.DepthOfEffect);
		Assert.Equal(50, sourceData.Curve.Points[1].Y);
		Assert.Equal(0.7, sourceData.ColorGradient.Alphas[1].Alpha);
	}

	/// <summary>
	/// Verifies that removing and reattaching targets hides targeting controls without clearing settings.
	/// </summary>
	[Fact]
	public void WipeModule_RemovingAndReassigningTargetsRetainsTargetingSettings()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);
		SetPropertyValue(effect, "DepthOfEffect", 2);

		// Act
		SetTargetNodesWithoutPropertyValidation(effect, []);

		// Assert
		Assert.Equal(TargetNodeSelection.Individual, GetTargetNodeSelectionValue(effect, "TargetNodeHandling"));
		Assert.Equal(2, GetIntValue(effect, "DepthOfEffect"));
		AssertTargetingControlsHidden(effect);

		// Act
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);

		// Assert
		Assert.Equal(TargetNodeSelection.Individual, GetTargetNodeSelectionValue(effect, "TargetNodeHandling"));
		Assert.Equal(2, GetIntValue(effect, "DepthOfEffect"));
	}

	[Fact]
	public void WipeProperties_DeepSingleTargetShowsTargetHandlingButHidesDepthInGroupMode()
	{
		// Arrange
		var effect = new WipeModule();
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

	[Fact]
	public void WipeProperties_DeepSingleTargetInIndividualModeShowsDepth()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(3)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);

		// Act
		var properties = TypeDescriptor.GetProperties(effect);
		var depthOfEffect = properties["DepthOfEffect"];

		// Assert
		Assert.NotNull(depthOfEffect);
		Assert.True(depthOfEffect.IsBrowsable);
	}

	[Fact]
	public void WipeRender_DefaultGroupModeRendersLocatedLeavesTogether()
	{
		// Arrange
		var firstLeaf = CreateLocatedLeaf("Leaf 1", 1, 1);
		var secondLeaf = CreateLocatedLeaf("Leaf 2", 3, 1);
		var effect = new WipeModule
		{
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetTargetNodesWithoutPropertyValidation(effect, [CreateGroupNode("Parent", firstLeaf, secondLeaf)]);

		// Act
		var preRenderSucceeded = effect.PreRender();
		var intents = effect.Render();

		// Assert
		Assert.True(preRenderSucceeded);
		Assert.Equal(new[] { firstLeaf.Element.Id, secondLeaf.Element.Id }.OrderBy(id => id), intents.ElementIds.OrderBy(id => id));
	}

	[Fact]
	public void WipeRender_IndividualModeRestartsForEachDepthGroup()
	{
		// Arrange
		var firstGroupStartLeaf = CreateLocatedLeaf("Group 1 Leaf 1", 1, 1);
		var firstGroupEndLeaf = CreateLocatedLeaf("Group 1 Leaf 2", 3, 1);
		var secondGroupStartLeaf = CreateLocatedLeaf("Group 2 Leaf 1", 101, 1);
		var secondGroupEndLeaf = CreateLocatedLeaf("Group 2 Leaf 2", 103, 1);
		var firstGroup = CreateGroupNode("Group 1", firstGroupStartLeaf, firstGroupEndLeaf);
		var secondGroup = CreateGroupNode("Group 2", secondGroupStartLeaf, secondGroupEndLeaf);
		var root = CreateGroupNode("Root", [firstGroup, secondGroup], 3);
		var effect = new WipeModule
		{
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetTargetNodesWithoutPropertyValidation(effect, [root]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);
		SetPropertyValue(effect, "DepthOfEffect", 1);

		// Act
		var preRenderSucceeded = effect.PreRender();
		var intents = effect.Render();

		// Assert
		Assert.True(preRenderSucceeded);
		Assert.Equal(TimeSpan.Zero, GetFirstIntentStartTime(intents, firstGroupStartLeaf));
		Assert.Equal(TimeSpan.Zero, GetFirstIntentStartTime(intents, secondGroupStartLeaf));
		Assert.True(GetFirstIntentStartTime(intents, firstGroupEndLeaf) > TimeSpan.Zero);
		Assert.True(GetFirstIntentStartTime(intents, secondGroupEndLeaf) > TimeSpan.Zero);
	}

	[Fact]
	public void WipeDepthConverter_ExcludesZeroAndMaximumDepth()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		var context = new Mock<ITypeDescriptorContext>();
		context.SetupGet(typeDescriptorContext => typeDescriptorContext.Instance).Returns(effect);
		var converter = new WipeTargetElementDepthConverter();

		// Act
		var values = converter.GetStandardValues(context.Object).Cast<string>().ToArray();

		// Assert
		Assert.Equal(["1", "2"], values);
	}

	[Fact]
	public void WipeProperties_IndividualModeResetsMaximumDepthToFirstUsefulDepth()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);
		SetPropertyValue(effect, "DepthOfEffect", 3);

		// Act
		TypeDescriptor.GetProperties(effect);
		var depthOfEffect = GetIntValue(effect, "DepthOfEffect");

		// Assert
		Assert.Equal(1, depthOfEffect);
	}

	[Fact]
	public void WipeProperties_ChangingDepthDoesNotRefreshItsPropertyDescriptor()
	{
		// Arrange
		var effect = new WipeModule();
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

	[Fact]
	public void WipeProperties_NormalizedStaleDepthDoesNotNotifyBindings()
	{
		// Arrange
		var effect = new WipeModule();
		var depthChangedCount = 0;
		PropertyChangedEventHandler propertyChanged = (_, eventArgs) =>
		{
			if (eventArgs.PropertyName == nameof(WipeModule.DepthOfEffect))
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

	[Fact]
	public void WipeProperties_TargetChangeNormalizingDepthNotifiesBindings()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(4)]);
		SetPropertyValue(effect, "TargetNodeHandling", TargetNodeSelection.Individual);
		SetPropertyValue(effect, "DepthOfEffect", 2);
		var depthChangedCount = 0;
		PropertyChangedEventHandler propertyChanged = (_, eventArgs) =>
		{
			if (eventArgs.PropertyName == nameof(WipeModule.DepthOfEffect))
			{
				depthChangedCount++;
			}
		};
		effect.PropertyChanged += propertyChanged;

		try
		{
			// Act
			SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(3)]);
		}
		finally
		{
			effect.PropertyChanged -= propertyChanged;
		}

		// Assert
		Assert.Equal(1, effect.DepthOfEffect);
		Assert.Equal(1, depthChangedCount);
	}

	[Fact]
	public void WipeProperties_ReapplyingTargetHandlingDoesNotRefreshItsPropertyDescriptor()
	{
		// Arrange
		var effect = new WipeModule();
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

	private static WipeData CreateCompleteWipeData(TargetNodeSelection targetNodeSelection)
	{
		var gradient = new ColorGradient(Color.White);
		gradient.Colors.Add(new ColorPoint(Color.Blue, 0.5));
		gradient.Alphas[1].Alpha = 0.7;
		return new WipeData
		{
			Curve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 0.0, 50.0, 100.0 })),
			MovementCurve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 100.0, 50.0, 0.0 })),
			ColorGradient = gradient,
			TargetNodeSelection = targetNodeSelection,
			DepthOfEffect = 2
		};
	}

	private static WipeData RoundTrip(WipeData data)
	{
		var serializer = new DataContractJsonSerializer(typeof(WipeData));
		using var stream = new MemoryStream();
		serializer.WriteObject(stream, data);
		stream.Position = 0;
		return (WipeData)serializer.ReadObject(stream)!;
	}

	private static void AssertTargetingControlsHidden(WipeModule effect)
	{
		var properties = TypeDescriptor.GetProperties(effect);
		var targetNodeHandling = properties[nameof(WipeModule.TargetNodeHandling)];
		var depthOfEffect = properties[nameof(WipeModule.DepthOfEffect)];

		Assert.NotNull(targetNodeHandling);
		Assert.False(targetNodeHandling.IsBrowsable);
		Assert.NotNull(depthOfEffect);
		Assert.False(depthOfEffect.IsBrowsable);
	}

	private static WipeData DeserializeJson(string json)
	{
		var serializer = new DataContractJsonSerializer(typeof(WipeData));
		using var readStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

		return (WipeData)serializer.ReadObject(readStream)!;
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
		var property = GetRequiredProperty(instance.GetType(), propertyName);

		property.SetValue(instance, value);
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

	private static TimeSpan GetFirstIntentStartTime(EffectIntents intents, IElementNode node)
	{
		var intentNodes = intents.GetIntentNodesForElement(node.Element.Id);
		Assert.NotNull(intentNodes);

		return intentNodes.Min(intentNode => intentNode.StartTime);
	}

	private static void SetTargetNodesWithoutPropertyValidation(WipeModule effect, IElementNode[] targetNodes)
	{
		var targetNodesField = typeof(EffectModuleInstanceBase).GetField("_targetNodes", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(targetNodesField);
		targetNodesField.SetValue(effect, targetNodes);

		var targetNodesChanged = typeof(WipeModule).GetMethod("TargetNodesChanged", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(targetNodesChanged);
		targetNodesChanged.Invoke(effect, []);
	}
}
