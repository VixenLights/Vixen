using Catel.IoC;
using Catel.Services;
using Moq;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.Import.XLights;

[Collection("CustomPropEditor")]
public sealed class XModelCircleImportTests
{
	[Fact]
	public async Task Import_WithStandaloneCircleModel_CreatesModelNodes()
	{
		// Arrange
		const string modelXml =
			"""
			<circlemodel name="Spinner" DisplayAs="Circle" LayerSizes="4" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="2" PixelCount="4" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.NotNull(prop);
		Assert.Equal("Spinner {1}", prop.Name);
		var modelGroup = GetModelGroup(prop, "Spinner");
		Assert.Equal(ElementModelType.Model, modelGroup.ModelType);
		Assert.Equal([1, 2, 3, 4], GetChildOrders(modelGroup));
		Assert.All(modelGroup.Children, child => Assert.Equal(2, child.LightSize));
	}

	[Fact]
	public async Task Import_WithWrappedCircleModel_ImportsWithoutSelection()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="Wrapped Spinner" DisplayAs="Circle" LayerSizes="4" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="4" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			ThrowOnSelection = true
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.NotNull(prop);
		Assert.Equal("Wrapped Spinner {1}", prop.Name);
		Assert.Equal([1, 2, 3, 4], GetChildOrders(GetModelGroup(prop, "Wrapped Spinner")));
		Assert.Equal(0, selectionService.CallCount);
	}

	[Fact]
	public async Task Import_WithOutsideInCircleModel_CreatesCircleGroupsInWiringOrder()
	{
		// Arrange
		const string modelXml =
			"""
			<circlemodel name="Outside In Spinner" DisplayAs="Circle" LayerSizes="3,4,5" InsideOut="0" StartSide="B" Dir="L" centerPercent="40" PixelSize="1" PixelCount="12" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var circlesGroup = GetCirclesGroup(prop, "Outside In Spinner");
		Assert.Equal(ElementModelType.SubModel, circlesGroup.ModelType);
		Assert.Equal(["Outside In Spinner {1} - Circle 1", "Outside In Spinner {1} - Circle 2", "Outside In Spinner {1} - Circle 3"], circlesGroup.Children.Select(child => child.Name).ToList());
		Assert.Equal([1, 2, 3, 4, 5], GetChildOrders(circlesGroup.Children[0]));
		Assert.Equal([6, 7, 8, 9], GetChildOrders(circlesGroup.Children[1]));
		Assert.Equal([10, 11, 12], GetChildOrders(circlesGroup.Children[2]));
	}

	[Fact]
	public async Task Import_WithInsideOutCircleModel_CreatesCircleGroupsInWiringOrder()
	{
		// Arrange
		const string modelXml =
			"""
			<circlemodel name="Inside Out Spinner" DisplayAs="Circle" LayerSizes="3,4,5" InsideOut="1" StartSide="B" Dir="L" centerPercent="40" PixelSize="1" PixelCount="12" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var circlesGroup = GetCirclesGroup(prop, "Inside Out Spinner");
		Assert.Equal([1, 2, 3], GetChildOrders(circlesGroup.Children[0]));
		Assert.Equal([4, 5, 6, 7], GetChildOrders(circlesGroup.Children[1]));
		Assert.Equal([8, 9, 10, 11, 12], GetChildOrders(circlesGroup.Children[2]));
	}

	[Fact]
	public async Task Import_WithTopAndBottomStartSides_PositionsFirstNodeOnRequestedSide()
	{
		// Arrange
		const string topStartXml =
			"""
			<circlemodel name="Top Start Spinner" DisplayAs="Circle" LayerSizes="8" InsideOut="0" StartSide="T" Dir="L" centerPercent="0" PixelSize="1" PixelCount="8" />
			""";
		const string bottomStartXml =
			"""
			<circlemodel name="Bottom Start Spinner" DisplayAs="Circle" LayerSizes="8" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="8" />
			""";

		// Act
		var topStartProp = await ImportAsync(topStartXml);
		var bottomStartProp = await ImportAsync(bottomStartXml);

		// Assert
		var topFirstLight = GetLight(topStartProp, "Top Start Spinner", 1);
		var bottomFirstLight = GetLight(bottomStartProp, "Bottom Start Spinner", 1);
		Assert.True(topFirstLight.Y < bottomFirstLight.Y);
		Assert.Equal(topFirstLight.X, bottomFirstLight.X, precision: 4);
	}

	[Fact]
	public async Task Import_WithClockwiseAndCounterClockwiseDirections_PositionsSecondNodeOnOppositeSides()
	{
		// Arrange
		const string clockwiseXml =
			"""
			<circlemodel name="Clockwise Spinner" DisplayAs="Circle" LayerSizes="8" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="8" />
			""";
		const string counterClockwiseXml =
			"""
			<circlemodel name="Counter Clockwise Spinner" DisplayAs="Circle" LayerSizes="8" InsideOut="0" StartSide="B" Dir="R" centerPercent="0" PixelSize="1" PixelCount="8" />
			""";

		// Act
		var clockwiseProp = await ImportAsync(clockwiseXml);
		var counterClockwiseProp = await ImportAsync(counterClockwiseXml);

		// Assert
		var clockwiseSecondLight = GetLight(clockwiseProp, "Clockwise Spinner", 2);
		var counterClockwiseSecondLight = GetLight(counterClockwiseProp, "Counter Clockwise Spinner", 2);
		Assert.True(clockwiseSecondLight.X > counterClockwiseSecondLight.X);
		Assert.Equal(clockwiseSecondLight.Y, counterClockwiseSecondLight.Y, precision: 4);
	}

	[Theory]
	[InlineData("")]
	[InlineData("not-a-number")]
	[InlineData("-25")]
	public async Task Import_WithMissingNonNumericOrNegativeCenterPercent_UsesZeroInnerRadius(string centerPercent)
	{
		// Arrange
		var modelXml = CreateTwoRingCircleXml("Center Default Spinner", centerPercent);

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		var firstCircle = GetCirclesGroup(prop, "Center Default Spinner").Children[0];
		Assert.Equal(0, GetCoordinateSpan(firstCircle.Children, light => light.X));
		Assert.Equal(0, GetCoordinateSpan(firstCircle.Children, light => light.Y));
	}

	[Fact]
	public async Task Import_WithPositiveCenterPercent_UsesLargerInnerRadius()
	{
		// Arrange
		var zeroCenterXml = CreateTwoRingCircleXml("Zero Center Spinner", "0");
		var fiftyCenterXml = CreateTwoRingCircleXml("Fifty Center Spinner", "50");

		// Act
		var zeroCenterProp = await ImportAsync(zeroCenterXml);
		var fiftyCenterProp = await ImportAsync(fiftyCenterXml);

		// Assert
		var zeroCenterCircle = GetCirclesGroup(zeroCenterProp, "Zero Center Spinner").Children[0];
		var fiftyCenterCircle = GetCirclesGroup(fiftyCenterProp, "Fifty Center Spinner").Children[0];
		Assert.True(GetCoordinateSpan(fiftyCenterCircle.Children, light => light.X) > GetCoordinateSpan(zeroCenterCircle.Children, light => light.X));
		Assert.True(GetCoordinateSpan(fiftyCenterCircle.Children, light => light.Y) > GetCoordinateSpan(zeroCenterCircle.Children, light => light.Y));
	}

	[Fact]
	public async Task Import_WithCenterPercentAboveOneHundred_ClampsInnerRadius()
	{
		// Arrange
		var oneHundredCenterXml = CreateTwoRingCircleXml("Hundred Center Spinner", "100");
		var aboveOneHundredCenterXml = CreateTwoRingCircleXml("Over Center Spinner", "125");

		// Act
		var oneHundredCenterProp = await ImportAsync(oneHundredCenterXml);
		var aboveOneHundredCenterProp = await ImportAsync(aboveOneHundredCenterXml);

		// Assert
		var oneHundredCircle = GetCirclesGroup(oneHundredCenterProp, "Hundred Center Spinner").Children[0];
		var aboveOneHundredCircle = GetCirclesGroup(aboveOneHundredCenterProp, "Over Center Spinner").Children[0];
		Assert.Equal(GetCoordinateSpan(oneHundredCircle.Children, light => light.X), GetCoordinateSpan(aboveOneHundredCircle.Children, light => light.X), precision: 4);
		Assert.Equal(GetCoordinateSpan(oneHundredCircle.Children, light => light.Y), GetCoordinateSpan(aboveOneHundredCircle.Children, light => light.Y), precision: 4);
	}

	[Fact]
	public async Task Import_WithCircleSubModel_AddsSubModelGroupUsingNodeOrder()
	{
		// Arrange
		const string modelXml =
			"""
			<circlemodel name="Submodel Spinner" DisplayAs="Circle" LayerSizes="4" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="4">
				<subModel name="FirstTwo" type="ranges" layout="" line0="1-2" />
			</circlemodel>
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.NotNull(prop);
		var subModelGroup = prop.RootNode.Children.Single(child => child.Name == "Submodel Spinner {1} - FirstTwo");
		Assert.Equal(ElementModelType.SubModel, subModelGroup.ModelType);
		Assert.Equal([1, 2], GetChildOrders(subModelGroup));
	}

	[Theory]
	[InlineData("")]
	[InlineData("0,4")]
	[InlineData("abc")]
	public async Task Import_WithInvalidLayerSizes_ReturnsNull(string layerSizes)
	{
		// Arrange
		var modelXml =
			$"""
			<circlemodel name="Invalid Spinner" DisplayAs="Circle" LayerSizes="{layerSizes}" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="4" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Null(prop);
	}

	[Fact]
	public async Task Import_WithMismatchedPositivePixelCount_ReturnsNull()
	{
		// Arrange
		const string modelXml =
			"""
			<circlemodel name="Mismatched Spinner" DisplayAs="Circle" LayerSizes="4,5" InsideOut="0" StartSide="B" Dir="L" centerPercent="0" PixelSize="1" PixelCount="10" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Null(prop);
	}

	[Theory]
	[InlineData("Dir", "X")]
	[InlineData("InsideOut", "2")]
	[InlineData("StartSide", "L")]
	public async Task Import_WithInvalidRequiredCircleAttribute_ReturnsNull(string attributeName, string attributeValue)
	{
		// Arrange
		var dir = attributeName == "Dir" ? attributeValue : "L";
		var insideOut = attributeName == "InsideOut" ? attributeValue : "0";
		var startSide = attributeName == "StartSide" ? attributeValue : "B";
		var modelXml =
			$"""
			<circlemodel name="Invalid Attribute Spinner" DisplayAs="Circle" LayerSizes="4" InsideOut="{insideOut}" StartSide="{startSide}" Dir="{dir}" centerPercent="0" PixelSize="1" PixelCount="4" />
			""";

		// Act
		var prop = await ImportAsync(modelXml);

		// Assert
		Assert.Null(prop);
	}

	[Fact]
	public async Task Import_WithWrappedCustomModel_StillImportsCustomModels()
	{
		// Arrange
		const string modelXml =
			"""
			<models type="exported">
				<model name="Wrapped Snowman" DisplayAs="Custom" CustomWidth="300" CustomHeight="300" PixelSize="1" CustomModelCompressed="1,0,0;2,1,0" />
			</models>
			""";
		var selectionService = new FakeXModelSelectionService
		{
			ThrowOnSelection = true
		};

		// Act
		var prop = await ImportAsync(modelXml, selectionService);

		// Assert
		Assert.NotNull(prop);
		Assert.Equal("Wrapped Snowman {1}", prop.Name);
		Assert.Equal([1, 2], GetChildOrders(GetModelGroup(prop, "Wrapped Snowman")));
		Assert.Equal(0, selectionService.CallCount);
	}

	private static string CreateTwoRingCircleXml(string name, string centerPercent)
	{
		var centerPercentAttribute = string.IsNullOrEmpty(centerPercent)
			? string.Empty
			: $" centerPercent=\"{centerPercent}\"";

		return $"""
			<circlemodel name="{name}" DisplayAs="Circle" LayerSizes="4,8" InsideOut="1" StartSide="B" Dir="L"{centerPercentAttribute} PixelSize="1" PixelCount="12" />
			""";
	}

	private static ElementModel GetModelGroup(Prop prop, string modelName)
	{
		Assert.NotNull(prop);
		return prop.RootNode.Children.Single(child => child.Name == $"{modelName} {{1}} - Model");
	}

	private static ElementModel GetCirclesGroup(Prop prop, string modelName)
	{
		Assert.NotNull(prop);
		return prop.RootNode.Children.Single(child => child.Name == $"{modelName} {{1}} - Circles");
	}

	private static List<int> GetChildOrders(ElementModel elementModel)
	{
		return elementModel.Children.Select(child => child.Order).Order().ToList();
	}

	private static Light GetLight(Prop prop, string modelName, int order)
	{
		return GetModelGroup(prop, modelName).Children.Single(child => child.Order == order).Lights.Single();
	}

	private static double GetCoordinateSpan(IEnumerable<ElementModel> nodes, Func<Light, double> selector)
	{
		var coordinates = nodes
			.Select(node => selector(node.Lights.Single()))
			.ToList();

		return coordinates.Max() - coordinates.Min();
	}

	private static Task<Prop> ImportAsync(string modelXml)
	{
		var import = new XModelImport();
		return ImportAsync(modelXml, import);
	}

	private static Task<Prop> ImportAsync(string modelXml, IXModelSelectionService selectionService)
	{
		var import = new XModelImport
		{
			SelectionService = selectionService
		};
		return ImportAsync(modelXml, import);
	}

	private static async Task<Prop> ImportAsync(string modelXml, XModelImport import)
	{
		RegisterMessageService();
		var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xmodel");
		await File.WriteAllTextAsync(filePath, modelXml);
		try
		{
			return await import.ImportAsync(filePath);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	private static void RegisterMessageService()
	{
		var messageService = new Mock<IMessageService>();
		messageService
			.Setup(service => service.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
			.Returns(Task.FromResult(default(MessageResult)));
		ServiceLocator.Default.RegisterInstance(messageService.Object);
	}

	private sealed class FakeXModelSelectionService : IXModelSelectionService
	{
		public int CallCount { get; private set; }

		public int SelectedIndex { get; set; }

		public bool CancelSelection { get; set; }

		public bool ThrowOnSelection { get; set; }

		public IReadOnlyList<XModelSelectionItem> SeenModels { get; private set; } = [];

		public Task<XModelSelectionItem> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models)
		{
			if (ThrowOnSelection)
			{
				throw new InvalidOperationException("Selection service should not be called.");
			}

			CallCount++;
			SeenModels = models;
			return Task.FromResult(CancelSelection ? null! : models.Single(model => model.Index == SelectedIndex));
		}
	}
}
