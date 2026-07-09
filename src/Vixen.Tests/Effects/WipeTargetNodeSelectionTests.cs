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
using VixenModules.Effect.Wipe;
using VixenModules.Property.Location;
using Xunit;

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
	public void WipeDepthConverter_ExcludesZeroAndMaximumDepth()
	{
		// Arrange
		var effect = new WipeModule();
		SetTargetNodesWithoutPropertyValidation(effect, [CreateTargetNode(3)]);
		var context = new Mock<ITypeDescriptorContext>();
		context.SetupGet(typeDescriptorContext => typeDescriptorContext.Instance).Returns(effect);
		var converter = new WipeTargetElementDepthConverter();

		// Act
		var values = converter.GetStandardValues(context.Object).Cast<string>().ToArray();

		// Assert
		Assert.Equal(["1", "2"], values);
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
		var targetNode = new Mock<IElementNode>();
		targetNode.SetupGet(node => node.Name).Returns(name);
		targetNode.SetupGet(node => node.Children).Returns(children);
		targetNode.SetupGet(node => node.Properties).Returns(new PropertyManager(targetNode.Object));
		targetNode.Setup(node => node.GetLeafEnumerator()).Returns(children.SelectMany(child => child.GetLeafEnumerator()));
		targetNode.Setup(node => node.GetMaxChildDepth()).Returns(1);

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
